﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyMesh : MonoBehaviour {

    private Mesh theMesh = null;

	// Use this for initialization
	void Start () {
        theMesh = GetComponent<MeshFilter>().mesh;

        MakeMesh(); // (Nick) i broke the mesh making into its own function
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void MakeMesh()
    {
        theMesh.Clear();    //clear out the existing mesh so we can replace it

        Vector3[] v = new Vector3[9];   // 2x2 mesh needs 3x3 vertices
        int[] t = new int[8 * 3];         // Number of triangles: 2x2 mesh and 2x triangles on each mesh-unit
        Vector3[] n = new Vector3[9];   // MUST be the same as number of vertices

        v[0] = new Vector3(-1, 0, -1);
        v[1] = new Vector3(0, 0, -1);
        v[2] = new Vector3(1, 0, -1);

        v[3] = new Vector3(-1, 0, 0);
        v[4] = new Vector3(0, 0, 0);
        v[5] = new Vector3(1, 0, 0);

        v[6] = new Vector3(-1, 0, 1);
        v[7] = new Vector3(0, 0, 1);
        v[8] = new Vector3(1, 0, 1);

        n[0] = new Vector3(0, 1, 0);
        n[1] = new Vector3(0, 1, 0);
        n[2] = new Vector3(0, 1, 0);
        n[3] = new Vector3(0, 1, 0);
        n[4] = new Vector3(0, 1, 0);
        n[5] = new Vector3(0, 1, 0);
        n[6] = new Vector3(0, 1, 0);
        n[7] = new Vector3(0, 1, 0);
        n[8] = new Vector3(0, 1, 0);

        // First triangle
        t[0] = 0; t[1] = 3; t[2] = 4;  // 0th triangle
        t[3] = 0; t[4] = 4; t[5] = 1;  // 1st triangle

        t[6] = 1; t[7] = 4; t[8] = 5;  // 2nd triangle
        t[9] = 1; t[10] = 5; t[11] = 2;  // 3rd triangle

        t[12] = 3; t[13] = 6; t[14] = 7;  // 4th triangle
        t[15] = 3; t[16] = 7; t[17] = 4;  // 5th triangle

        t[18] = 4; t[19] = 7; t[20] = 8;  // 6th triangle
        t[21] = 4; t[22] = 8; t[23] = 5;  // 7th triangle

        theMesh.vertices = v; //  new Vector3[3];
        theMesh.triangles = t; //  new int[3];
        theMesh.normals = n;
    }
}