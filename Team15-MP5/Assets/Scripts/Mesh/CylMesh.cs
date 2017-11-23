using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylMesh : MyMesh
{
    private float radius = 0.5f;

    // Use this for initialization
    public override void Start()
    {
        meshType = MeshType.Cylinder;
        minN = 8;
        minM = 4;
        base.Start();
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
    }

    /******************
    * MESH MANAGEMENT
    * ****************/

    protected override void AllocateMeshData()
    {
        vertices = new Vector3[n * m];
        triangles = new int[(n - 1) * (m - 1) * 2 * 3];           
        normals = new Vector3[vertices.Length];                         // MUST be the same as number of vertices
        uv = new Vector2[vertices.Length];                              //same as the number of vertices
    }

    protected override void MakeVertices()
    {
        for (int i = 0; i < m; i++)      
        {
            float yVal = 0.5f - i * (1.0f / (m - 1));

            float theta = 0.0f;
            for (int j = 0; j < n; j++)
            {
                float xVal = radius * Mathf.Sin(theta);
                float zVal = radius * Mathf.Cos(theta);

                int index = i * n + j;
                vertices[index] = new Vector3(xVal, yVal, zVal);

                //increment theta
                theta += (2 * Mathf.PI / (n - 1));
            }
        }
    }

    protected override void MakeTriangles()
    {
        int idx = 0;
        int v1, v2, v3;

        for(int i = 0; i < m - 1; i++)
        {
            for(int j = 0; j < n - 1; j++)
            {
                v1 = i * n + j;
                v2 = v1 + n;
                v3 = v1 + n + 1;
                triangles[idx++] = v1;
                triangles[idx++] = v2;
                triangles[idx++] = v3;
            }
        }


        for (int i = 0; i < m - 1; i++)
        {
            for (int j = 0; j < n - 1; j++)
            {
                v1 = i * n + j;
                v2 = v1 + n + 1;
                v3 = v1 + 1;
                triangles[idx++] = v1;
                triangles[idx++] = v2;
                triangles[idx++] = v3;
            }
        }
    }

    protected override void MakeNormals()
    {
        for (int i = 0; i < vertices.Length; i++)
        {
            normals[i] = new Vector3(vertices[i].x, 0, vertices[i].z).normalized;
        }
    }

    protected override void MakeUV()
    {
        base.MakeUV();
    }

    /*  **********
    * ACCESSORS
    * ***********/

    public bool SetRadius(float newRadius)
    {
        if (newRadius <= 0.0f)
            return false;

        radius = newRadius;
        MakeMesh();
        return true;
    }

    public float GetRadius() { return radius; }
}
