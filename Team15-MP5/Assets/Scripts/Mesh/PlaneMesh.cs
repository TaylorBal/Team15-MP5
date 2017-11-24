using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneMesh : MyMesh {

	// Use this for initialization
	public override void Start ()
    {
        meshType = MeshType.Plane;
        minN = minM = 2;
        base.Start();
    }
	
	// Update is called once per frame
	public override void Update () {
    }

    public void Rotate(float val)
    {
        textureRotation = val;
        base.Update();
    }

    protected override void AllocateMeshData()
    {
        vertices = new Vector3[n * m];
        triangles = new int[(n - 1) * (m - 1) * 2 * 3];
        normals = new Vector3[vertices.Length];
        uv = new Vector2[vertices.Length];
    }

    protected override void MakeVertices()
    {
        for (int i = 0; i < m; i++)
        {
            float yVal = 0.5f - i * (1.0f / (m - 1));

            for (int j = 0; j < n; j++)
            {
                float xVal = -0.5f + j * (1.0f / (n - 1));

                int index = i * n + j;
                vertices[index] = new Vector3(xVal, yVal, 0);
            }
        }
    }

    protected override void MakeTriangles()
    {
        int idx = 0;
        int v1, v2, v3;

        //make first set
        for (int i = 0; i < m - 1; i++)
        {
            for (int j = 0; j < n - 1; j++)
            {
                v1 = i * n + j;
                v3 = v1 + n;
                v2 = v1 + n + 1;
                triangles[idx++] = v1;
                triangles[idx++] = v2;
                triangles[idx++] = v3;
            }
        }

        //make second set
        for (int i = 0; i < m - 1; i++)
        {
            for (int j = 0; j < n - 1; j++)
            {
                v1 = i * n + j;
                v3 = v1 + n + 1;
                v2 = v1 + 1;
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
            normals[i] = new Vector3(0, 0, 1);
        }
    }

    protected override void MakeUV()
    {
        base.MakeUV();
    }
}
