using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;
    Vector3[] vertices;
    int[] triangles;
    Vector2[] uvs;

    public int xSize = 15;
    public int zSize = 15;

    public bool useRandomSeed = true;
    public int seed;

    public GameObject objectToPlace; // Object to place at the center

    public int pathWidth = 1;  // Width of the paths

    public Material terrainMaterial; // Material with texture atlas (terrain + path)

    private bool[,] pathMap;  // 2D array to track path positions

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = terrainMaterial;

        if (useRandomSeed)
        {
            seed = Random.Range(0, 10000);  // Use Unity's Random to generate a new seed at runtime
        }

        pathMap = new bool[xSize + 1, zSize + 1];  // Initialize path tracking map

        CreateShape();
        CreatePaths();  // Generate non-overlapping winding paths to the center
        UpdateMesh();
        PlaceObjectAtCenter(); // Place object at the center of the grid
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        uvs = new Vector2[(xSize + 1) * (zSize + 1)];
        Random.InitState(seed);

        int centerX = xSize / 2;
        int centerZ = zSize / 2;

        int index = 0;
        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y;
                if ((x == centerX || x == centerX + 1) && (z == centerZ || z == centerZ + 1))
                {
                    y = 0;
                }
                else
                {
                    y = Mathf.PerlinNoise(x * .3f + seed, z * .3f + seed) * 2;
                }

                vertices[index] = new Vector3(x, y, z);

                // Set default UVs for terrain (0 to 0.5) - assuming terrain is on the left side of the texture atlas
                uvs[index] = new Vector2((float)x / xSize * 0.5f, (float)z / zSize);

                index++;
            }
        }

        int vert = 0;
        int tris = 0;
        triangles = new int[xSize * zSize * 6];
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    void CreatePaths()
    {
        // Define starting points at the edges
        Vector2Int[] startPoints = new Vector2Int[]
        {
            new Vector2Int(0, Random.Range(0, zSize)),         // Left edge
            new Vector2Int(xSize, Random.Range(0, zSize)),     // Right edge
            new Vector2Int(Random.Range(0, xSize), 0)          // Top edge
        };

        // The center 2x2 square
        Vector2Int centerPoint1 = new Vector2Int(xSize / 2, zSize / 2);

        foreach (Vector2Int start in startPoints)
        {
            CreateWindingPath(start, centerPoint1);
        }
    }

    void CreateWindingPath(Vector2Int start, Vector2Int end)
    {
        // Start from the starting point
        Vector2Int currentPos = start;

        // A direction vector to store the general movement towards the center
        Vector2Int direction;

        int maxAttempts = 1000; // Limit the number of direction changes to avoid infinite loops
        int attempts = 0;

        // Keep moving towards the center with more random deviations
        while (currentPos != end && attempts < maxAttempts)
        {
            attempts++;
            FlattenSquare(currentPos.x, currentPos.y);

            // Calculate a random deviation that increases the winding nature of the path
            direction = new Vector2Int(
                end.x - currentPos.x > 0 ? 1 : (end.x - currentPos.x < 0 ? -1 : 0),
                end.y - currentPos.y > 0 ? 1 : (end.y - currentPos.y < 0 ? -1 : 0)
            );

            // Add more significant randomness to the path
            if (Random.value > 0.4f)  // Increasing randomness
            {
                direction.x += Random.Range(-1, 2); // Deviation in x-axis
            }
            if (Random.value > 0.4f)  // Increasing randomness
            {
                direction.y += Random.Range(-1, 2); // Deviation in y-axis
            }

            // Ensure the next move is not into a previously created path
            int nextX = Mathf.Clamp(currentPos.x + direction.x, 0, xSize);
            int nextZ = Mathf.Clamp(currentPos.y + direction.y, 0, zSize);

            if (IsPathOccupied(nextX, nextZ))
            {
                // Adjust direction if necessary
                direction = new Vector2Int(Random.Range(-1, 2), Random.Range(-1, 2));
            }

            // Move to the next position
            currentPos = new Vector2Int(nextX, nextZ);
        }

        if (attempts >= maxAttempts)
        {
            Debug.LogWarning("Path generation stopped to avoid infinite loop.");
        }
    }

    void FlattenSquare(int x, int z)
    {
        for (int dx = -pathWidth / 2; dx <= pathWidth / 2; dx++)
        {
            for (int dz = -pathWidth / 2; dz <= pathWidth / 2; dz++)
            {
                int newX = Mathf.Clamp(x + dx, 0, xSize);
                int newZ = Mathf.Clamp(z + dz, 0, zSize);

                int index = newZ * (xSize + 1) + newX;
                vertices[index].y = 0;  // Flatten the square

                // Adjust UVs for the path texture (use the right side of the texture atlas)
                uvs[index] = new Vector2(0.5f + (float)newX / xSize * 0.5f, (float)newZ / zSize);

                // Mark this square as part of a path
                pathMap[newX, newZ] = true;
            }
        }
    }

    bool IsPathOccupied(int x, int z)
    {
        // Check if the position is occupied by a path, and ensure the position is within bounds
        if (x < 0 || x > xSize || z < 0 || z > zSize)
        {
            return true;  // Treat out-of-bounds areas as occupied
        }
        return pathMap[x, z];
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs; // Apply UVs to the mesh

        mesh.RecalculateNormals();
    }

    void PlaceObjectAtCenter()
    {
        if (objectToPlace != null)
        {
            int centerX = xSize / 2;
            int centerZ = zSize / 2;

            Vector3 centerPosition = (vertices[centerZ * (xSize + 1) + centerX] +
                                      vertices[centerZ * (xSize + 1) + (centerX + 1)] +
                                      vertices[(centerZ + 1) * (xSize + 1) + centerX] +
                                      vertices[(centerZ + 1) * (xSize + 1) + (centerX + 1)]) / 4;

            centerPosition.y += 1;

            Instantiate(objectToPlace, centerPosition, Quaternion.identity);
        }
    }

    public void OnDrawGizmos()
    {
        if (vertices != null)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Gizmos.DrawSphere(vertices[i], .1f);
            }
        }
    }
}
