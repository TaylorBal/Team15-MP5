using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public partial class MasterController : MonoBehaviour {

    //Camera
    public Camera MainCamera = null;

    //Vertex Handle and Axis selection
    GameObject vertHandle = null;
    VertexBehavior vertBehavior = null;
    GameObject axis = null;
    AxisBehavior axisBehavior = null;
    public enum manipAxis { nullAxis, xAxis, yAxis, zAxis};
    manipAxis curManipAxis;
    

    //Plane Mesh
    public MyMesh MeshScript;
    public GameObject MeshObject;

    //UI: Dropdown
    public Dropdown dropMode;

    //UI: Resolution
    public Slider sliderN;
    public Slider sliderM;

	// Use this for initialization
	void Start ()
    {
        //Camera
        Debug.Assert(MainCamera != null);

        //Mode
        dropMode.onValueChanged.AddListener(ChangeMode);

        //Resolution
        sliderN.onValueChanged.AddListener(ChangeN);
        sliderM.onValueChanged.AddListener(ChangeM);
	}
	
	// Update is called once per frame
	void Update ()
    {
        LMBService();
	}

    /// <summary>
    /// Change modes. 0 = "Plane", 1 = "Cylinder".
    /// </summary>
    /// <param name="mode"></param>
    public void ChangeMode(int mode)
    {
        if (mode == 0)
        {
            MeshObject.SetActive(true);
        }
        else if (mode == 1)
        {
            MeshObject.SetActive(false);
        }
    }

    public void ChangeN(float val)
    {
        MeshScript.n = (int)val;

        //theMesh.MakeMesh();
        MeshScript.ClearVertexHandles();
        MeshScript.MakeVertexHandles();
    }

    public void ChangeM(float val)
    {
        MeshScript.m = (int)val;

        //theMesh.MakeMesh();
        MeshScript.ClearVertexHandles();
        MeshScript.MakeVertexHandles();
    }
}
