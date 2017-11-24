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


    //Meshes
    public PlaneMesh planeMesh;
    public GameObject PlaneMeshObject;
    public CylMesh cylMesh;

    //UI: Dropdown
    public Dropdown dropMode;

    //UI: Resolution
    public Slider sliderN;
    public Slider sliderM;

    //UI: XformControl
    public XformControl xFormControl;


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

        //XformControl
        xFormControl.X.TheSlider.onValueChanged.AddListener(xChanged);
        xFormControl.Y.TheSlider.onValueChanged.AddListener(yChanged);
        xFormControl.Z.TheSlider.onValueChanged.AddListener(zChanged);
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
            planeMesh.Disable();
            cylMesh.Enable();
            
        }
        else if (mode == 1)
        {
            cylMesh.Disable();
            planeMesh.Enable();
        }
    }

    public void ChangeN(float val)
    {
        planeMesh.SetN((int)val);

        planeMesh.ClearVertexHandles();
        planeMesh.MakeVertexHandles();
    }

    public void ChangeM(float val)
    {
        planeMesh.SetM((int)val);

        planeMesh.ClearVertexHandles();
        planeMesh.MakeVertexHandles();
    }

    public void xChanged(float val)
    {
        if(xFormControl.curMode == XformControl.mode.translate)
        {

        }

        if (xFormControl.curMode == XformControl.mode.scale)
        {

        }
    }

    public void yChanged(float val)
    {
        if (xFormControl.curMode == XformControl.mode.translate)
        {

        }

        if (xFormControl.curMode == XformControl.mode.scale)
        {

        }
    }

    public void zChanged(float val)
    {
        if (xFormControl.curMode == XformControl.mode.rotate)
        {
            planeMesh.Rotate(val);
        }
    }
}
