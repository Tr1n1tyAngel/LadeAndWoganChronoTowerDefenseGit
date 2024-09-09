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

    
    public int pathWidth = 1;  // Width of the paths
    public Material terrainMaterial; // Material with texture atlas
    public GameObject[] randomObjects;  // Array of random objects to place on the terrain

    private bool[,] pathMap;  // 2D array to track path positions

    public List<Vector3> defenderPositions = new List<Vector3>();  // List of defender positions for defender placement
    public int numberOfDefenderPositions = 10; // Number of valid defender placement positions
    public float maxDistanceToPath = 5f; // Maximum distance to place towers near the path
    public List<Vector3> pathStartPoints = new List<Vector3>(); // Store path start points for enemy spawning

    // Store all path waypoints for each path
    public Dictionary<int, List<Vector3>> pathWaypoints = new Dictionary<int, List<Vector3>>();

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = terrainMaterial;
        //generates random seed every new game
        if (useRandomSeed)
        {
            seed = Random.Range(0, 10000); 
        }

        pathMap = new bool[xSize + 1, zSize + 1];  

        CreateMesh();
        CreatePaths();  
        CreateDefenderPositions();  
        PlaceRandomObjects();
        UpdateMesh();
        
    }
    //creates the mesh, it does this by making a grid with a randomised seed so that perlin noise can be used for the difference in height
    void CreateMesh()
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
    //creates the pathways first by finding starting points at the edges of the grid, then stores the center of these starting points so that enemies can spawn there, then initializes the path from the edge to the center square of the grid
    void CreatePaths()
    {
        // Define starting points at the edges
        Vector2Int[] startPoints = new Vector2Int[]
        {
            new Vector2Int(0, Random.Range(0, zSize)),         
            new Vector2Int(xSize, Random.Range(0, zSize)),     
            new Vector2Int(Random.Range(0, xSize), 0)          
        };

        
        for (int i = 0; i < startPoints.Length; i++)
        {
            pathStartPoints.Add(new Vector3(startPoints[i].x + 0.5f, 1f, startPoints[i].y + 0.5f));
            pathWaypoints[i] = new List<Vector3>(); 
        }

        Vector2Int centerPoint1 = new Vector2Int(xSize / 2, zSize / 2);

        for (int i = 0; i < startPoints.Length; i++)
        {
            CreateWindingPath(startPoints[i], centerPoint1, i);
        }
    }

    // this function is so that the paths arent just straight from the outside to the center it adds abit of randomness, also adds the path points to a waypoint array so that the enemies can use it
    void CreateWindingPath(Vector2Int start, Vector2Int end, int pathIndex)
    {
        Vector2Int currentPos = start;
        Vector2Int direction;
        int maxAttempts = 1000; 
        int attempts = 0;

        while (currentPos != end && attempts < maxAttempts)
        {
            attempts++;
            FlattenSquare(currentPos.x, currentPos.y);
            pathWaypoints[pathIndex].Add(new Vector3(currentPos.x + 0.5f, 0.5f, currentPos.y + 0.5f)); 

            
            direction = new Vector2Int(
                end.x - currentPos.x > 0 ? 1 : (end.x - currentPos.x < 0 ? -1 : 0),
                end.y - currentPos.y > 0 ? 1 : (end.y - currentPos.y < 0 ? -1 : 0)
            );

            if (Random.value > 0.4f) direction.x += Random.Range(-1, 2);
            if (Random.value > 0.4f) direction.y += Random.Range(-1, 2);

            
            int nextX = Mathf.Clamp(currentPos.x + direction.x, 0, xSize);
            int nextZ = Mathf.Clamp(currentPos.y + direction.y, 0, zSize);

            if (IsPathOccupied(nextX, nextZ)) continue;

            currentPos = new Vector2Int(nextX, nextZ);
        }
    }

    //checks how wide the path needs to be and then flattens the squares in correlation to this
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

    //checks if a random position is near enough to a path then flattens that square and makes the center avaliable for defenders to be placed
    void CreateDefenderPositions()
    {
        int count = 0;
        while (count < numberOfDefenderPositions)
        {
            int x = Random.Range(1, xSize - 1);
            int z = Random.Range(1, zSize - 1);

            if (!pathMap[x, z] && !pathMap[x + 1, z] && !pathMap[x, z + 1] && !pathMap[x + 1, z + 1])
            {
                
                if (IsNearPath(x, z, maxDistanceToPath))
                {
                    
                    FlattenGridSquare(x, z);

                    Vector3 centerPosition = new Vector3(x + 0.5f, 1f, z + 0.5f);
                    defenderPositions.Add(centerPosition);

                    count++;
                }
            }
        }
    }

    //flattens the squares that are set for defender placement
    void FlattenGridSquare(int x, int z)
    {
        vertices[z * (xSize + 1) + x].y = 1f;           
        vertices[z * (xSize + 1) + (x + 1)].y = 1f;     
        vertices[(z + 1) * (xSize + 1) + x].y = 1f;     
        vertices[(z + 1) * (xSize + 1) + (x + 1)].y = 1f; 

        pathMap[x, z] = true;
        pathMap[x + 1, z] = true;
        pathMap[x, z + 1] = true;
        pathMap[x + 1, z + 1] = true;
    }

    //checks to see if a square is occupied by a path so that the defenders placement position isnt placed there
    bool IsPathOccupied(int x, int z)
    {
        if (x < 0 || x > xSize || z < 0 || z > zSize)
        {
            return true;  
        }
        return pathMap[x, z];
    }

    //checks for if there is a nearby path so that the flattened positions avaliable for defenders are close enough to the paths
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
                    return true;  
                }
            }
        }

        return false;
    }

    // this function was an additional added in feature to allow for random objects to be placed so that the scene doesnt look to empty
    void PlaceRandomObjects()
    {
        foreach (GameObject randomObject in randomObjects)
        {
            int numInstancesToPlace = Random.Range(1, 4);

            int instancesPlaced = 0;

            while (instancesPlaced < numInstancesToPlace)
            {
                int x = Random.Range(1, xSize - 1);
                int z = Random.Range(1, zSize - 1);

                if (!pathMap[x, z] && !defenderPositions.Contains(new Vector3(x, 1f, z)))
                {
                    Vector3 spawnPosition = new Vector3(x + 0.5f, 1f, z + 0.5f);

                    Instantiate(randomObject, spawnPosition, Quaternion.identity);

                    instancesPlaced++;
                }
            }
        }
    }

    //updates the mesh that is generated
    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;  
        mesh.RecalculateNormals();
    }

    
    //optional, shows the vertices and the flattened places where a defender can be placed but only in scene view  
    public void OnDrawGizmos()
    {
        if (vertices != null)
        {
            for (int i = 0; i < vertices.Length; i++)
            {
                Gizmos.DrawSphere(vertices[i], .1f);
            }

            Gizmos.color = Color.green;
            foreach (var pos in defenderPositions)
            {
                Gizmos.DrawSphere(pos, 0.2f);
            }
        }
    }
}