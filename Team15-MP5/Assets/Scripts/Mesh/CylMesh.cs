using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylMesh : MyMesh
{
    private float radius = 0.5f;
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
}
