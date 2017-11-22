using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MyMesh : MonoBehaviour {

    public int n = 2;      //# of vertices horizontally (X local)
    public int m = 2;      //# of vertices vertically (Y local)
    public Vector2 textureOffset = new Vector2(0, 0);
    public Vector2 textureScale = new Vector2(1, 1);
    public float textureRotation = 0.0f;

    // 2D array of sphere handle prefabs
    public GameObject vertexHandleType;
    public GameObject[,] vertexHandles;

    //The Mesh
    private Mesh theMesh = null;

    //declare as class vars so we can modify and access without reallocating
    //only reallocate when # of vertices changes
    private Vector3[] vertices;
    private int[] triangles;
    private Vector3[] normals;
    private Vector2[] uv;

	// Use this for initialization
	void Start () {
        theMesh = GetComponent<MeshFilter>().mesh;

        MakeVertexHandles();

        MakeMesh();
	}
	
	// Update is called once per frame
	void Update () {

        //REALLY bad idea (runs even we don't need it to)
        //BUT it shows that this stuff can change on the fly
        textureRotation += 0.1f;

        MakeMesh();
	}

    public bool ChangeNumVerts(int newN, int newM)
    {
        if (newN < 2 || newM < 2)
        {
            return false;
        }

        n = newN;
        m = newM;

        MakeMesh();     //update the mesh
        return true;
    }

    public void MakeMesh()
    {
        if(n < 2 || m < 2)
        {
            Debug.LogError("Invalid number of vertices!");
            return;
        }

        theMesh.Clear();    //clear out the existing mesh so we can replace it

        vertices = new Vector3[n * m];//new Vector3[9];   // 2x2 mesh needs 3x3 vertices
        triangles = new int[(n - 1) * (m - 1) * 2 * 3];//new int[8 * 3];         // Number of triangles: 2x2 mesh and 2x triangles on each mesh-unit
        normals = new Vector3[n * m];   // MUST be the same as number of vertices
        uv = new Vector2[n * m];

        MakeVertices(2.0f, 2.0f);
        MakeNormals();
        MakeTriangles();
        MakeUV();

        theMesh.vertices = vertices;
        theMesh.triangles = triangles;
        theMesh.normals = normals;
        theMesh.uv = uv;
    }

    //right now this assumes a plane of size 1
    //with normal <0, 1, 0> in local space
    //corners: <+/- scaleX/2.0f, 1, +/- scaleZ/2.0f>
    void MakeVertices(float scaleX, float scaleZ)
    {
        for(int i = 0; i < m; i++)
        {
            float zVal = -scaleZ / 2.0f + i * (1.0f / (m - 1)) * scaleZ;

            for(int j = 0; j < n; j++)
            {
                float xVal = -scaleX / 2.0f + j * (1.0f / (n - 1)) * scaleX;

                int index = i * n + j;
                vertices[index] = new Vector3(xVal, 0, zVal);
            }
        }
    }

    //set to (0, 1, 0) for now
    //will be more advanced later
    void MakeNormals()
    {
        for(int i = 0; i < n * m; i++)
        {
            normals[i] = new Vector3(0, 1, 0);
        }
    }

    void MakeTriangles()
    {
        int idx = 0;
        int v1, v2, v3;

        //make first set
        for(int j = 0; j < m - 1; j++)
        {
            for(int i = 0; i < n - 1; i++)
            {
                v1 = n * j + i;
                v2 = v1 + n;
                v3 = v1 + n + 1;
                triangles[idx++] = v1;
                triangles[idx++] = v2;
                triangles[idx++] = v3;
            }
        }

        //make second set
        for (int j = 0; j < m - 1; j++)
        {
            for (int i = 0; i < n - 1; i++)
            {
                v1 = n * j + i;
                v2 = v1 + n + 1;
                v3 = v1 + 1;
                triangles[idx++] = v1;
                triangles[idx++] = v2;
                triangles[idx++] = v3;
            }
        }
    }
            
    void MakeUV()
    {
        //Rotation needs to be about (0.5, 0.5)
        Matrix3x3 pivot = Matrix3x3Helpers.CreateTranslation(new Vector2(-0.5f, -0.5f));
        Matrix3x3 TRS = Matrix3x3Helpers.CreateTRS(textureOffset, textureRotation, textureScale);

        for(int j = 0; j < m; j++)
        {
            float v = (float)j / (m - 1);
            for (int i = 0; i < n; i++)
            {
                int index = j * n + i;
                float u = (float)i / (n - 1);

                uv[index] = Matrix3x3.MultiplyVector2(pivot, new Vector2(u, v));
                uv[index] = Matrix3x3.MultiplyVector2(TRS, uv[index]);
                uv[index] = Matrix3x3.MultiplyVector2(pivot.Invert(), uv[index]);

            }
        }
    }

    public void ClearVertexHandles()
    {
        GameObject[] allHandles = GameObject.FindGameObjectsWithTag("Handle");
        for(int i = 0; i < allHandles.Length; i++)
        {
            Destroy(allHandles[i]);
        }
    }

    public void MakeVertexHandles()
    {
        vertexHandles = new GameObject[n, m];

        for (int i = 0; i < n; i++) //Increase n -> i goes farther
        {
            float xVal = -1.0f + i * 2.0f / (n - 1);

            for (int j = 0; j < m; j++) //Increase m -> j goes farther
            {
                float zVal = -1.0f + j * 2.0f / (m - 1);

                vertexHandles[i, j] = Instantiate(vertexHandleType, new Vector3(xVal, 0, zVal), Quaternion.identity);
            }
        }
    }
}