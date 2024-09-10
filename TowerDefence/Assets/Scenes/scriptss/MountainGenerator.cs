using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountainGenerator : MonoBehaviour
{
    public int terrainWidth = 128;
    public int terrainHeight = 128;

    private Mesh mesh;
    private Vector3[] vertices;
    private int[] triangles;
    private System.Random rand;

    private void Start()
    {
        rand = new System.Random();
        GenerateTerrain();
    }

    void GenerateTerrain()
    {
        float noiseScale = Random.Range(10f, 30f);
        float heightMultiplier = Random.Range(2f, 8f);

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        vertices = new Vector3[(terrainWidth + 1) * (terrainHeight + 1)];

        float offsetX = rand.Next(0, 10000);
        float offsetZ = rand.Next(0, 10000);

        for (int i = 0, z = 0; z <= terrainHeight; z++)
        {
            for (int x = 0; x <= terrainWidth; x++, i++)
            {
                float y = Mathf.PerlinNoise((x + offsetX) / noiseScale, (z + offsetZ) / noiseScale) * heightMultiplier;
                vertices[i] = new Vector3(x, y, z);
            }
        }

        UpdateMesh();
    }

    void UpdateMesh()
    {
        mesh.Clear();
        mesh.vertices = vertices;
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
