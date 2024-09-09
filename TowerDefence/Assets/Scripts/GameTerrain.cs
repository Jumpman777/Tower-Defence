using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameTerrain : MonoBehaviour
{
    public int mDivisions;
    public float mSize;
    public float mHeight;

    private Vector3[] mVerts;
    private int mVertCount;

    void Start()
    {
        CreateTerrain();
    }


    void Update()
    {

    }

    void CreateTerrain()
    {
        mVertCount = (mDivisions + 1) * (mDivisions + 1);

        mVerts = new Vector3[mVertCount];
        Vector2[] uvs = new Vector2[mVertCount];
        int[] tris = new int[mDivisions * mDivisions * 6];

        float halfSize = mSize * 0.5f;
        float divisionSize = mSize / mDivisions;

        Mesh mesh = new Mesh();

        GetComponent<MeshFilter>().mesh= mesh;

        int triOffset = 0;

        for (int i = 0; i <= mDivisions; i++)
        {
            for (int j = 0; j <= mDivisions; j++)
            {
                mVerts[i * (mDivisions + 1) + j] = new Vector3(-halfSize + j * divisionSize, 0.0f, halfSize - i * divisionSize);
                uvs[i * (mDivisions + 1) + j] = new Vector2((float)i / mDivisions, (float)j / mDivisions);

                if (i < mDivisions && j < mDivisions)
                {
                    int topLeft = i * (mDivisions + 1) + j;
                    int botLeft = (i + 1) * (mDivisions + 1) + j;

                    tris[triOffset] = topLeft;
                    tris[triOffset + 1] = topLeft + 1;
                    tris[triOffset + 2] = botLeft + 1;

                    tris[triOffset + 3] = topLeft;
                    tris[triOffset + 4] = botLeft + 1;
                    tris[triOffset + 5] = botLeft;

                    triOffset += 6;
                }

            }
        }

        mesh.vertices = mVerts;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();


    }
}
