using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneMesh : MyMesh {

    protected int n = 0;            //number of vertices (x-axis for plane, circle cross-section vertices in cylinder)
    protected int m = 0;            //number of vertices (z-axis for plane, y-axis for cylinder)
    protected int minN = 0;         //the minimum value of n (and m) that makes sense
    protected int minM = 0;

    // Use this for initialization
    public override void Start ()
    {
        meshType = MeshType.Plane;
        minN = minM = 2;

        if (n < minN)
            n = minN;
        if (m < minM)
            m = minM;

        base.Start();
    }
	
	// Update is called once per frame
	public override void Update () {
        base.Update();
    }

    /******************
    * MESH MANAGEMENT
    * ****************/

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
            float zVal = 0.5f - i * (1.0f / (m - 1));

            for (int j = 0; j < n; j++)
            {
                float xVal = -0.5f + j * (1.0f / (n - 1));

                int index = i * n + j;
                vertices[index] = new Vector3(xVal, 0, zVal);
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

    protected override void MakeUV()
    {
        //Rotation needs to be about (0.5, 0.5)
        Matrix3x3 pivot = Matrix3x3Helpers.CreateTranslation(new Vector2(-0.5f, -0.5f));
        Matrix3x3 TRS = Matrix3x3Helpers.CreateTRS(textureOffset, textureRotation, textureScale);

        for (int i = 0; i < m; i++)
        {
            float v = 1.0f - (float)i / (m - 1);
            for (int j = 0; j < n; j++)
            {
                int index = i * n + j;
                float u = 1.0f - (float)j / (n - 1);

                uv[index] = Matrix3x3.MultiplyVector2(pivot, new Vector2(u, v));
                uv[index] = Matrix3x3.MultiplyVector2(TRS, uv[index]);
                uv[index] = Matrix3x3.MultiplyVector2(pivot.Invert(), uv[index]);

            }
        }
    }

    public override void MakeVertexHandles()
    {
        vertexHandles = new GameObject[vertices.Length];

        Matrix4x4 LtW = transform.localToWorldMatrix;
        for (int i = 0; i < vertexHandles.Length; i++)
        {
            vertexHandles[i] = Instantiate(vertexHandleType, LtW * vertices[i], Quaternion.identity);
            VertexBehavior vb = vertexHandles[i].GetComponent<VertexBehavior>();
            vb.Init(this, i, true, handlesVisible);
        }
    }

    /*  **********
    * ACCESSORS
    * ***********/

    public bool SetN(int newN)
    {
        if (newN < minN)
            return false;

        n = newN;
        MakeMesh();
        return true;
    }

    public int GetN() { return n; }

    public bool SetM(int newM)
    {
        if (newM < minM)
            return false;

        m = newM;
        MakeMesh();
        return true;
    }

    public int GetM() { return m; }


    public override void MoveVertex(int index, Vector3 delta)
    {
        //in this case we only care about the vertical direction
        base.MoveVertex(index, new Vector3(0.0f, delta.y, 0.0f));
    }
}
