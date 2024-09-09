
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter))]
public class MeshGeneratorV1 : MonoBehaviour
{
    Mesh mesh;
    private int MESH_SCALE = 500;
    public AnimationCurve heightCurve;
    private Vector3[] vertices;
    private int[] triangles;

    public int xSize;
    public int zSize;

    public float scale;
    public int octaves;
    public float lacunarity;

    public int seed;
    private System.Random prng;
    private Vector2[] octaveOffsets;

    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        CreateNewMap();
    }

    public void CreateNewMap()
    {
        CreateMeshShape();
        CreateTriangles();
        UpdateMesh();
    }

    private void CreateMeshShape()
    {
        /*
         Vector2[] octaveOffsets = GetOffsetSeed();

         if (scale <= 0)
             scale = 0.0001f;

         vertices = new Vector3[(xSize + 1) * (zSize + 1)];

         List<Vector3Int> outerSquares = GetOuterSquares();
         Vector3Int center = GetCenterSquare();



         foreach (var start in outerSquares)
         {
             List<Vector3Int> path = FindPath(start, center);

             foreach (var point in path)
             {
                 // Flatten the path; adjust the height as needed
                 vertices[point.z * (xSize + 1) + point.x] = new Vector3(point.x, 0.00248f, point.z);
             }
         }

         for (int i = 0, z = 0; z <= zSize; z++)
         {
             for (int x = 0; x <= xSize; x++)
             {


                 float noiseHeight = GenerateNoiseHeight(z, x, octaveOffsets);
                 vertices[i] = new Vector3(x, noiseHeight, z);
                 i++;
             }
         }
        */


        // Generate seed offsets for noise
        Vector2[] octaveOffsets = GetOffsetSeed();

        if (scale <= 0)
            scale = 0.0001f;


        // Create vertices array
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        // Generate the terrain first, without paths
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float noiseHeight = GenerateNoiseHeight(z, x, octaveOffsets);
                vertices[i] = new Vector3(x, noiseHeight, z);
                i++;
            }
        }

        // Get the outer squares and the center for pathfinding
        List<Vector3Int> outerSquares = GetOuterSquares();
        Vector3Int center = GetCenterSquare();

        // Now, adjust the vertices that form the path
        foreach (var start in outerSquares)
        {
            List<Vector3Int> path = FindPath(start, center);

            foreach (var point in path)
            {
                // Calculate the index for the path vertex in the vertices array
                int index = point.z * (xSize + 1) + point.x;

                if (index >= 0 && index < vertices.Length)
                {
                    // Flatten the path; set a lower height for path vertices
                    vertices[index] = new Vector3(point.x, 0.00248f, point.z);
                }
            }
        }

        // Optional: Debugging to visualize the path vertices in the mesh
        for (int i = 0; i < vertices.Length; i++)
        {
            Debug.Log($"Vertex {i}: {vertices[i]}");
        }
    }

    private Vector2[] GetOffsetSeed()
    {
        seed = Random.Range(0, 1000);
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];

        for (int o = 0; o < octaves; o++)
        {
            float offsetX = prng.Next(-100000, 100000);
            float offsetY = prng.Next(-100000, 100000);
            octaveOffsets[o] = new Vector2(offsetX, offsetY);
        }
        return octaveOffsets;
    }

    private float GenerateNoiseHeight(int z, int x, Vector2[] octaveOffsets)
    {
        float amplitude = 12;
        float frequency = 1;
        float persistence = 0.5f;
        float noiseHeight = 0;

        for (int y = 0; y < octaves; y++)
        {
            float mapZ = z / scale * frequency + octaveOffsets[y].y;
            float mapX = x / scale * frequency + octaveOffsets[y].x;

            float perlinValue = (Mathf.PerlinNoise(mapZ, mapX)) * 2 - 1;
            noiseHeight += heightCurve.Evaluate(perlinValue) * amplitude;
            frequency *= lacunarity;
            amplitude *= persistence;
        }
        return noiseHeight;
    }

    private void CreateTriangles()
    {
        triangles = new int[xSize * zSize * 6];
        int vert = 0;
        int tris = 0;

        for (int z = 0; z < xSize; z++)
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

    private void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateTangents();
        GetComponent<MeshCollider>().sharedMesh = mesh;

        gameObject.transform.localScale = new Vector3(MESH_SCALE, MESH_SCALE, MESH_SCALE);
    }

    private List<Vector3Int> GetOuterSquares()
    {
        List<Vector3Int> outerSquares = new List<Vector3Int>();

        for (int x = 0; x <= xSize; x++)
        {
            outerSquares.Add(new Vector3Int(x, 0, 0));       // Bottom edge
            outerSquares.Add(new Vector3Int(x, 0, zSize));   // Top edge
        }

        for (int z = 1; z < zSize; z++)
        {
            outerSquares.Add(new Vector3Int(0, 0, z));       // Left edge
            outerSquares.Add(new Vector3Int(xSize, 0, z));   // Right edge
        }

        return outerSquares;
    }

    private Vector3Int GetCenterSquare()
    {
        return new Vector3Int(xSize / 2, 0, zSize / 2);
    }

    private List<Vector3Int> FindPath(Vector3Int start, Vector3Int target)
    {
        List<Vector3Int> openList = new List<Vector3Int>();
        HashSet<Vector3Int> closedList = new HashSet<Vector3Int>();
        Dictionary<Vector3Int, Vector3Int> cameFrom = new Dictionary<Vector3Int, Vector3Int>();

        openList.Add(start);

        while (openList.Count > 0)
        {
            Vector3Int current = openList[0];
            openList.RemoveAt(0);

            if (current == target)
                return ReconstructPath(cameFrom, current);

            closedList.Add(current);

            foreach (Vector3Int neighbor in GetNeighbors(current))
            {
                if (closedList.Contains(neighbor)) continue;

                if (!openList.Contains(neighbor))
                {
                    openList.Add(neighbor);
                    cameFrom[neighbor] = current;
                }
            }
        }

        return new List<Vector3Int>(); // Return empty if no path found
    }

    private List<Vector3Int> ReconstructPath(Dictionary<Vector3Int, Vector3Int> cameFrom, Vector3Int current)
    {
        List<Vector3Int> totalPath = new List<Vector3Int> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current);
        }
        return totalPath;
    }

    private List<Vector3Int> GetNeighbors(Vector3Int node)
    {
        List<Vector3Int> neighbors = new List<Vector3Int>();

        if (node.x > 0) neighbors.Add(new Vector3Int(node.x - 1, 0, node.z)); // Left
        if (node.x < xSize) neighbors.Add(new Vector3Int(node.x + 1, 0, node.z)); // Right
        if (node.z > 0) neighbors.Add(new Vector3Int(node.x, 0, node.z - 1)); // Down
        if (node.z < zSize) neighbors.Add(new Vector3Int(node.x, 0, node.z + 1)); // Up

        return neighbors;
    }
}


