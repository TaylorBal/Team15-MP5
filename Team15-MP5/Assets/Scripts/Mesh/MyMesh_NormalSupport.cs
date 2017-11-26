using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MyMesh : MonoBehaviour {

    //For computing normals
    protected Vector3[] faceNormals = null;         //corresponds to each triplet in <triangles>
    protected List<int>[] adjFaces = null;          //faces adjacent to the vertex at index i

    private Vector3 ComputeFaceNormal(int v1, int v2, int v3)
    {
        Vector3 a = vertices[v2] - vertices[v1];
        Vector3 b = vertices[v3] - vertices[v1];
        return Vector3.Cross(a, b).normalized;
    }

    //compute a normal for a specific vertex
    //weight each face according to how much it contributes to
    //the area surrounding the vertex
    private Vector3 ComputeVertexNormal(int vertex, List<int> faces, Vector3[] faceNormals)
    {
        float[] weights = new float[faces.Count];
        Vector3 sumNormal = Vector3.zero;

        //find the angle each face makes about the vertex
        for(int i = 0; i < faces.Count; i++)
        {
            float theta = GetFaceAngleOnVertex(vertex, faces[i]);
            weights[i] = theta;
        }
        
        //add up each angle, modified by a weight
        for(int i = 0; i < faces.Count; i++)
        {
            sumNormal += weights[i] * faceNormals[faces[i]];
        }

        return sumNormal.normalized;
    }

    //given a vertex and a face, find the angle the face makes on the vertex
    //vertex is the index at which the vertex in question can be found
    //face is the index at which you can find the vertex in question and the other two in triangles[]
    private float GetFaceAngleOnVertex(int vertex, int face)
    {
        Vector3 vertVec = vertices[vertex];
        Vector3[] otherVec = new Vector3[2];
        int idx = 0;

        //find the other two vertices, and ourself
        bool found = false; //did we find ourselves?
        for(int i = 0; i < 3; i++)
        {
            int curVert = 3 * face + i;
            if(triangles[curVert] != vertex)
            {
                otherVec[idx++] = vertices[triangles[curVert]];
            }
            else
            {
                found = true;
            }
        }

        //if we didn't find our own vertex, something is wrong
        if(!found)
        {
            Debug.LogError("Could not find vertex!");
            return 0.0f;
        }

        //otherwise lets get this on
        Vector3 a = otherVec[0] - vertVec;
        Vector3 b = otherVec[1] - vertVec;

        return Vector3.Angle(a, b);
    }
}
