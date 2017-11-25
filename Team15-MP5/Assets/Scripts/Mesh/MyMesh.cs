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

    protected virtual void MakeUV() { }

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
        vertexHandles = new GameObject[vertices.Length];

        for(int i = 0; i < vertexHandles.Length; i++)
        {
            
            vertexHandles[i] = Instantiate(vertexHandleType, vertices[i], Quaternion.identity);

            Quaternion outRot = Quaternion.FromToRotation(vertexHandles[i].transform.up, normals[i]);

            vertexHandles[i].transform.localRotation *= outRot;
            //Vector3 left = Vector3.Cross(transform.up, vertexHandles[i].transform.forward);
        }
    }

    public void UpdateVertexHandles()
    {
        //we can make this more efficient, but for now...
        ClearVertexHandles();
        MakeVertexHandles();
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