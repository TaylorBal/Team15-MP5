using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylMesh : MyMesh
{
    private float radius = 0.5f;
    public float minRadius = 0.25f;
    private float rotation = 270.0f;

    private int circleRes = 8;          //# pixels in circle cross-section
    private int vertRes = 8;            //vertical resolution
    private int minCircleRes = 3;
    private int minVertRes = 2;

    // Use this for initialization
    public override void Start()
    {
        meshType = MeshType.Cylinder;
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
        vertices = new Vector3[circleRes * vertRes];
        triangles = new int[(circleRes - 1) * (vertRes - 1) * 2 * 3];
        normals = new Vector3[vertices.Length];                         // MUST be the same as number of vertices
        uv = new Vector2[vertices.Length];                              //same as the number of vertices
    }

    protected override void MakeVertices()
    {
        for (int i = 0; i < vertRes; i++)      
        {
            float yVal = 0.5f - i * (1.0f / (vertRes - 1));

            float theta = 0.0f;
            for (int j = 0; j < circleRes; j++)
            {
                float xVal = radius * Mathf.Sin(theta);
                float zVal = radius * Mathf.Cos(theta);

                int index = i * circleRes + j;
                vertices[index] = new Vector3(xVal, yVal, zVal);

                //increment theta
                theta += ((rotation / 360.0f) * (2 * Mathf.PI) / (circleRes - 1));
            }
        }
    }

    protected override void MakeTriangles()
    {
        int idx = 0;

        int v1, v2, v3;

        for(int i = 0; i < vertRes - 1; i++)
        {
            for(int j = 0; j < circleRes - 1; j++)
            {
                v1 = i * circleRes + j;
                v2 = v1 + circleRes;
                v3 = v1 + circleRes + 1;
                triangles[idx++] = v1;
                triangles[idx++] = v2;
                triangles[idx++] = v3;
            }
        }


        for (int i = 0; i < vertRes - 1; i++)
        {
            for (int j = 0; j < circleRes - 1; j++)
            {
                v1 = i * circleRes + j;
                v2 = v1 + circleRes + 1;
                v3 = v1 + 1;
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

        for (int i = 0; i < vertRes; i++)
        {
            float v = 1.0f - (float)i / (vertRes - 1);
            for (int j = 0; j < circleRes; j++)
            {
                int index = i * circleRes + j;
                float u = 1.0f - (float)j / (circleRes - 1);

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

            Quaternion q = Quaternion.LookRotation(-transform.up, normals[i]);
            vertexHandles[i].transform.localRotation = q;

            VertexBehavior vb = vertexHandles[i].GetComponent<VertexBehavior>();

            //you can only select the first of each row
            bool selectable = i % circleRes == 0;
            vb.Init(this, i, selectable, handlesVisible);
        }
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

    public bool SetRotation(float newRotation)
    {
        if (newRotation <= 0.0f || newRotation > 360.0f)
            return false;

        rotation = newRotation;
        MakeMesh();
        return true;
    }

    public float GetRotation() { return rotation; }

    public bool SetCircleRes(int newResolution)
    {
        if (newResolution < minCircleRes)
            return false;

        circleRes = newResolution;
        MakeMesh();
        return true;
    }

    public int GetCircleRes() { return circleRes; }

    public bool SetVertRes(int newResolution)
    {
        if (newResolution < minVertRes)
            return false;

        vertRes = newResolution;
        MakeMesh();
        return true;
    }

    public int GetVertRes() { return vertRes; }

    public override void MoveVertex(int index, Vector3 delta)
    {
        //this vertex is connected to all the others in its row
        //in this case guaranteed to be the first in the row

        //The movement is always relative to the normal
        //y stays the same
        //all verts connected match the changes relative to their normal

        Matrix4x4 rotate = Matrix4x4.TRS(Vector3.zero, Quaternion.AngleAxis(-delta.x, transform.up), Vector3.one);

        for (int i = 0; i < circleRes; i++)
        {
            Vector3 xz = (new Vector3(vertices[index + i].x, 0.0f, vertices[index + i].z));

            //rotate around at current radius
            Matrix4x4 pivot = Matrix4x4.TRS(vertices[index + i], Quaternion.identity, Vector3.one);
            vertices[index + i] = pivot * rotate * pivot.inverse * vertices[index + i];

            vertices[index + i] += transform.up * delta.y;
            if (delta.z > 0 || (delta.z < 0 && xz.magnitude > minRadius))
            {
                vertices[index + i] += xz.normalized * delta.z;
            }

            vertexHandles[index + i].transform.localPosition = transform.localToWorldMatrix * vertices[index + i];
            if(i == 0)
            {
                VertexBehavior vb = vertexHandles[index].GetComponent<VertexBehavior>();   
                Quaternion lr = Quaternion.LookRotation(xz.normalized, transform.up);
                vb.SetAxesOrientation(lr);
            }
        }


        //update the orientations of all of the handles
        for (int j = 0; j < vertexHandles.Length; j++)
        {
            Vector3 right = vertices[(j + 1) % vertices.Length] - vertices[j];
            Quaternion q = Quaternion.LookRotation(Vector3.Cross(normals[j], right), normals[j]);
            vertexHandles[j].transform.localRotation = q;
        }
    }
}
