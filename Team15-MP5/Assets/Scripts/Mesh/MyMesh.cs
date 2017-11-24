using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum MeshType
{
    None,
    Plane,
    Cylinder
};

public class MyMesh : MonoBehaviour {

    //what type of mesh this is
    protected MeshType meshType = MeshType.None;
    protected Mesh theMesh = null;

    //vertex/mesh arrays for setting in the MeshFilter object
    protected Vector3[] vertices = null;
    protected int[] triangles = null;
    protected Vector3[] normals = null;
    protected Vector2[] uv = null;

    // 2D array of sphere handle prefabs
    public GameObject vertexHandleType;
    public GameObject[,] vertexHandles;

    protected int n = 0;            //number of vertices (x-axis for plane, circle cross-section vertices in cylinder)
    protected int m = 0;            //number of vertices (z-axis for plane, y-axis for cylinder)
    protected int minN = 0;         //the minimum value of n (and m) that makes sense
    protected int minM = 0;

    public Vector2 textureOffset = new Vector2(0, 0);
    public Vector2 textureScale = new Vector2(1, 1);
    public float textureRotation = 0.0f;

    // Use this for initialization
    public virtual void Start()
    {
        theMesh = GetComponent<MeshFilter>().mesh;

        if (n < minN)
            n = minN;
        if (m < minM)
            m = minM;

        MakeMesh();
        MakeVertexHandles();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        //update the normals and textures to correspond
        //with the new vertex positions2
        UpdateMesh();
    }

    public void Enable()
    {
        Start();
        gameObject.SetActive(true);
    }

    public void Disable()
    {
        ClearVertexHandles();
        gameObject.SetActive(false);
    }

    /******************
    * MESH MANAGEMENT
    * ****************/

    protected void MakeMesh()
    {
        theMesh.Clear();

        if (meshType == MeshType.None)
            return;

        AllocateMeshData();
        MakeMeshData();
        SetMeshObject();     
    }

    protected void UpdateMesh()
    {
        if (meshType == MeshType.None)
            return;

        MakeNormals();
        MakeUV();

        theMesh.normals = normals;
        theMesh.uv = uv;
    }


    private void SetMeshObject()
    {
        theMesh.vertices = vertices;
        theMesh.triangles = triangles;
        theMesh.normals = normals;
        theMesh.uv = uv;
    }

    private void MakeMeshData()
    {
        MakeVertices();
        MakeTriangles();
        MakeNormals();
        MakeUV();
    }


    protected virtual void AllocateMeshData() { }

    protected virtual void MakeVertices() { }

    protected virtual void MakeTriangles() { }

    protected virtual void MakeNormals() { }

    protected virtual void MakeUV()
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

        /* for (int i = 0; i < n; i++) //Increase n -> i goes farther
         {
             float xVal = -1.0f + i * 2.0f / (n - 1);

             for (int j = 0; j < m; j++) //Increase m -> j goes farther
             {
                 float zVal = -1.0f + j * 2.0f / (m - 1);

                 vertexHandles[i, j] = Instantiate(vertexHandleType, new Vector3(xVal, 0, zVal), Quaternion.identity);
             }
         }*/

        for (int i = 0; i < m; i++)
        {
            for(int j = 0; j < n; j++)
            {
                int index = i * n + j;
                Quaternion q = Quaternion.FromToRotation(Vector3.up, normals[index]);       //this isn't correct but it gets us in the ballpark
                vertexHandles[j, i] = Instantiate(vertexHandleType, vertices[index], q);
            }
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

    public bool SetVertex(int index, Vector3 vertex)
    {
        if (index < 0 || index >= vertices.Length)
            return false;

        vertices[index] = vertex;
        return true;
    }

    public bool GetVertex(int index, ref Vector3 vertex)
    {
        if (index < 0 || index >= vertices.Length)
        {
            vertex = Vector3.zero;
            return false;
        }

        vertex = vertices[index];
        return true;
    }

    public bool SetNormal(int index, Vector3 normal)
    {
        if (index < 0 || index >= normals.Length)
            return false;

        normals[index] = normal;
        return true;
    }

    public bool GetNormal(int index, ref Vector3 normal)
    {
        if (index < 0 || index >= normals.Length)
        {
            normal = Vector3.zero;
            return false;
        }

        normal = normals[index];
        return true;
    }
}