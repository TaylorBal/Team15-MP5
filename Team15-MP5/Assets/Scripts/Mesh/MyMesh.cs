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

public partial class MyMesh : MonoBehaviour {

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
    public GameObject[] vertexHandles;

    // Kind of a proto-transform for the texture
    public Vector2 textureOffset = new Vector2(0, 0);
    public Vector2 textureScale = new Vector2(1, 1);
    public float textureRotation = 0.0f;

    // Use this for initialization
    public virtual void Start()
    {
        theMesh = GetComponent<MeshFilter>().mesh;
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
        gameObject.SetActive(true);
        ShowVertexHandles();
    }

    public void Disable()
    {
        HideVertexHandles();
        gameObject.SetActive(false);
    }

    /******************
    * MESH MANAGEMENT
    * ****************/

    protected void MakeMesh()
    {
        if (theMesh == null)
        {
            theMesh = GetComponent<MeshFilter>().mesh;
        }
        else
        {
            theMesh.Clear();
        }

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

        UpdateNormals();
        MakeUV();

        theMesh.vertices = vertices;
        theMesh.triangles = triangles;
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

    private void MakeNormals() {

        faceNormals = new Vector3[triangles.Length / 3];
        adjFaces = new List<int>[vertices.Length];
        for (int i = 0; i < adjFaces.Length; i++)
            adjFaces[i] = new List<int>();

        //first calculate face normals
        for(int i = 0; i < triangles.Length / 3; i++)
        {
            int idx = i * 3;
            faceNormals[i] = ComputeFaceNormal(triangles[idx], triangles[idx + 1], triangles[idx + 2]);

            //for each vertex in the triangle
            //add the triangle to the vertex's list for computing vertex normals
            for(int j = 0; j < 3; j++)
            {
                int vnIdx = triangles[idx + j];
                adjFaces[vnIdx].Add(i);
            }
        }

        //use the face normals to calculate vertex normals
        for(int i = 0; i < adjFaces.Length; i++)
        {
            normals[i] = ComputeVertexNormal(i, adjFaces[i], faceNormals);
        }
    }

    private void UpdateNormals()
    {

        //first calculate face normals
        for (int i = 0; i < triangles.Length / 3; i++)
        {
            int idx = i * 3;
            faceNormals[i] = ComputeFaceNormal(triangles[idx], triangles[idx + 1], triangles[idx + 2]);
        }

        //use the face normals to calculate vertex normals
        for (int i = 0; i < adjFaces.Length; i++)
        {
            normals[i] = ComputeVertexNormal(i, adjFaces[i], faceNormals);
        }
    }

    protected virtual void MakeUV() { }

    public void ClearVertexHandles()
    {
        GameObject[] allHandles = GameObject.FindGameObjectsWithTag("Handle");
        for(int i = 0; i < allHandles.Length; i++)
        {
            Destroy(allHandles[i]);
        }
    }
    
    public virtual void MakeVertexHandles()
    {
    }

    public void UpdateVertexHandles()
    {
        //we can make this more efficient, but for now...
        ClearVertexHandles();
        MakeVertexHandles();
    }

    private void HideVertexHandles()
    {
        foreach (GameObject g in vertexHandles)
            g.SetActive(false);
    }

    private void ShowVertexHandles()
    {
        foreach (GameObject g in vertexHandles)
            g.SetActive(true);
    }

    /*  **********
     * ACCESSORS
     * ***********/

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

    public virtual void MoveVertex(int index, Vector3 delta)
    {
        vertices[index] += delta;
        //UpdateMesh();
        vertexHandles[index].transform.localPosition = transform.localToWorldMatrix * vertices[index];
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