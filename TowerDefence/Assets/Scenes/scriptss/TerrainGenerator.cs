using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;  // For NavMesh

public class TerrainGenerator : MonoBehaviour
{
    public int terrainWidth = 128;
    public int terrainHeight = 128;
    public GameObject tower;
    public NavMeshSurface navMeshSurface; // Reference to NavMeshSurface

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private System.Random rand;

    private void Start()
    {
        rand = new System.Random();  // Seed the random number generator
        GenerateTerrain();
        navMeshSurface.BuildNavMesh(); // Build NavMesh after terrain is generated
    }

    void GenerateTerrain()
    {
        float noiseScale = Random.Range(10f, 30f); // Random noise scale (controls smoothness/spikiness)
        float heightMultiplier = Random.Range(2f, 8f); // Random height multiplier (controls spikiness)

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        vertices = new Vector3[(terrainWidth + 1) * (terrainHeight + 1)];

        float offsetX = rand.Next(0, 10000);  // Random offset for Perlin noise
        float offsetZ = rand.Next(0, 10000);

        // Generate the terrain using Perlin noise
        for (int i = 0, z = 0; z <= terrainHeight; z++)
        {
            for (int x = 0; x <= terrainWidth; x++, i++)
            {
                float y = Mathf.PerlinNoise((x + offsetX) / noiseScale, (z + offsetZ) / noiseScale) * heightMultiplier;
                vertices[i] = new Vector3(x, y, z);
            }
        }

        // Create flat paths and the center
        CreateFixedFlatPathsAndCenter();

        UpdateMesh();
    }

    void CreateFixedFlatPathsAndCenter()
    {
        // Flatten the center where the tower will be placed
        int centerX = terrainWidth / 2;
        int centerZ = terrainHeight / 2;
        int radius = 10; // Radius of the flat area for the center

        for (int z = centerZ - radius; z <= centerZ + radius; z++)
        {
            for (int x = centerX - radius; x <= centerX + radius; x++)
            {
                int index = z * (terrainWidth + 1) + x;
                vertices[index].y = 0f; // Make the center flat
            }
        }

        // Create 3 fixed paths to the center (no randomization)
        CreateFlatPath(terrainWidth / 4, 0, centerX, centerZ);  // Path from bottom-left corner
        CreateFlatPath(terrainWidth / 4, terrainHeight, centerX, centerZ);  // Path from top-left corner
        CreateFlatPath(terrainWidth - terrainWidth / 4, terrainHeight, centerX, centerZ);  // Path from top-right corner
    }

    void CreateFlatPath(int startX, int startZ, int endX, int endZ)
    {
        int pathWidth = 5;  // Increase the width of the path

        // Use Bresenham's line algorithm to create flat paths
        int dx = Mathf.Abs(endX - startX);
        int dz = Mathf.Abs(endZ - startZ);
        int sx = startX < endX ? 1 : -1;
        int sz = startZ < endZ ? 1 : -1;
        int err = dx - dz;

        while (true)
        {
            // Flatten a wider area for the path
            for (int x = -pathWidth / 2; x <= pathWidth / 2; x++)
            {
                for (int z = -pathWidth / 2; z <= pathWidth / 2; z++)
                {
                    int index = (startZ + z) * (terrainWidth + 1) + (startX + x);
                    if (index >= 0 && index < vertices.Length)
                    {
                        vertices[index].y = 0f;  // Make path flat
                    }
                }
            }

            if (startX == endX && startZ == endZ) break;

            int e2 = err * 2;
            if (e2 > -dz)
            {
                err -= dz;
                startX += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                startZ += sz;
            }
        }
    }

    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;

        // Generate triangles
        triangles = new int[terrainWidth * terrainHeight * 6];
        int vert = 0;
        int tris = 0;

        for (int z = 0; z < terrainHeight; z++)
        {
            for (int x = 0; x < terrainWidth; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + terrainWidth + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + terrainWidth + 1;
                triangles[tris + 5] = vert + terrainWidth + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }

        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}
