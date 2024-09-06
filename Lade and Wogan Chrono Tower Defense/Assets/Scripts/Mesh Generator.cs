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
    public List<Vector3> flattenedPositions = new List<Vector3>();  // List of flattened positions for object placement
    public int numberOfFlattenedPositions = 10; // Number of valid object placement positions
    public float maxDistanceToPath = 5f; // Maximum distance to place towers near the path

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
        CreateFlattenedPositions();  // Create flattened positions for object placement
        UpdateMesh();
        PlaceObjectAtCenter(); // Place object at the center of the grid
    }

    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];
        uvs = new Vector2[(xSize + 1) * (zSize + 1)];
        Random.InitState(seed);

        int index = 0;
        for (int z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y;
                if (pathMap[x, z])
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
        Vector2Int currentPos = start;
        Vector2Int direction;
        int maxAttempts = 1000; // Limit to avoid infinite loops
        int attempts = 0;

        while (currentPos != end && attempts < maxAttempts)
        {
            attempts++;
            FlattenSquare(currentPos.x, currentPos.y);

            // Calculate movement direction
            direction = new Vector2Int(
                end.x - currentPos.x > 0 ? 1 : (end.x - currentPos.x < 0 ? -1 : 0),
                end.y - currentPos.y > 0 ? 1 : (end.y - currentPos.y < 0 ? -1 : 0)
            );

            if (Random.value > 0.4f) direction.x += Random.Range(-1, 2);
            if (Random.value > 0.4f) direction.y += Random.Range(-1, 2);

            // Ensure the next move is not into a previously created path
            int nextX = Mathf.Clamp(currentPos.x + direction.x, 0, xSize);
            int nextZ = Mathf.Clamp(currentPos.y + direction.y, 0, zSize);

            if (IsPathOccupied(nextX, nextZ)) continue;

            currentPos = new Vector2Int(nextX, nextZ);
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

                vertices[index].y = 0;
                uvs[index] = new Vector2(0.5f + (float)newX / xSize * 0.5f, (float)newZ / zSize);
                pathMap[newX, newZ] = true;
            }
        }
    }

    void CreateFlattenedPositions()
    {
        int count = 0;
        while (count < numberOfFlattenedPositions)
        {
            int x = Random.Range(1, xSize - 1);
            int z = Random.Range(1, zSize - 1);

            if (!pathMap[x, z] && !pathMap[x + 1, z] && !pathMap[x, z + 1] && !pathMap[x + 1, z + 1])
            {
                // Check if this position is near the path (within maxDistanceToPath)
                if (IsNearPath(x, z, maxDistanceToPath))
                {
                    // Flatten the 2x2 area
                    FlattenGridSquare(x, z);

                    // Add the center of this flattened area as a possible placement position
                    Vector3 centerPosition = new Vector3(x + 0.5f, 1f, z + 0.5f);
                    flattenedPositions.Add(centerPosition);

                    count++;
                }
            }
        }
    }

    // Flatten a 2x2 square of the grid by adjusting the heights of the vertices
    void FlattenGridSquare(int x, int z)
    {
        // Flatten the 4 vertices of a 2x2 square
        vertices[z * (xSize + 1) + x].y = 1f;           // Bottom-left
        vertices[z * (xSize + 1) + (x + 1)].y = 1f;     // Bottom-right
        vertices[(z + 1) * (xSize + 1) + x].y = 1f;     // Top-left
        vertices[(z + 1) * (xSize + 1) + (x + 1)].y = 1f; // Top-right

        // Mark the vertices as part of the flattened position
        pathMap[x, z] = true;
        pathMap[x + 1, z] = true;
        pathMap[x, z + 1] = true;
        pathMap[x + 1, z + 1] = true;
    }

    bool IsPathOccupied(int x, int z)
    {
        if (x < 0 || x > xSize || z < 0 || z > zSize)
        {
            return true;  // Treat out-of-bounds areas as occupied
        }
        return pathMap[x, z];
    }

    // Check if a given position is within a certain distance from the path
    bool IsNearPath(int x, int z, float maxDistance)
    {
        for (int dx = -Mathf.CeilToInt(maxDistance); dx <= Mathf.CeilToInt(maxDistance); dx++)
        {
            for (int dz = -Mathf.CeilToInt(maxDistance); dz <= Mathf.CeilToInt(maxDistance); dz++)
            {
                int checkX = Mathf.Clamp(x + dx, 0, xSize);
                int checkZ = Mathf.Clamp(z + dz, 0, zSize);

                if (pathMap[checkX, checkZ])
                {
                    return true;  // Found a nearby path
                }
            }
        }

        return false;
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;  // Apply UVs to the mesh
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

            // Draw Gizmos for the flattened positions
            Gizmos.color = Color.green;
            foreach (var pos in flattenedPositions)
            {
                Gizmos.DrawSphere(pos, 0.2f);
            }
        }
    }
}
