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
    public GameObject PlaneMeshObject;
    public PlaneMesh planeMesh;
    public CylMesh cylMesh;
    MeshType curMesh = MeshType.Plane;

    //UI: Dropdown
    public Dropdown dropMode;

    //UI: Resolution
    public Slider sliderN;
    public Slider sliderM;

    //Cylinder Sliders
    public Slider sliderCylinderRes;
    public Slider sliderCylinderRot;

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
        sliderN.value = planeMesh.GetN();
        sliderM.value = planeMesh.GetM();

        //Cylinder Sliders
        sliderCylinderRes.onValueChanged.AddListener(ChangeCylRes);
        sliderCylinderRot.onValueChanged.AddListener(ChangeCylRot);
        sliderCylinderRes.value = cylMesh.GetCircleRes();
        sliderCylinderRot.value = cylMesh.GetRotation();

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
            cylMesh.Disable();
            planeMesh.Enable();
            curMesh = MeshType.Plane;
        }
        else if (mode == 1)
        {
            planeMesh.Disable();
            cylMesh.Enable();
            curMesh = MeshType.Cylinder;
        }
    }

    public void ChangeN(float val)
    {
        planeMesh.SetN((int)val);

        if (curMesh == MeshType.Plane)
            planeMesh.UpdateVertexHandles();
    }

    public void ChangeM(float val)
    {
        planeMesh.SetM((int)val);

        if(curMesh == MeshType.Plane)
            planeMesh.UpdateVertexHandles();
    }

    public void ChangeCylRes(float val)
    {
        cylMesh.SetCircleRes((int)val);

        if (curMesh == MeshType.Cylinder)
            cylMesh.UpdateVertexHandles();
    }

    public void ChangeCylRot(float val)
    {
        cylMesh.SetRotation(val);

        if(curMesh == MeshType.Cylinder)
            cylMesh.UpdateVertexHandles();
    }

    public void xChanged(float val)
    {
        if(xFormControl.curMode == XformControl.mode.translate)
        {
            if (curMesh == MeshType.Plane)
                planeMesh.textureOffset.x = val;
            else if (curMesh == MeshType.Cylinder)
                cylMesh.textureOffset.x = val;
        }

        if (xFormControl.curMode == XformControl.mode.scale)
        {
            if (curMesh == MeshType.Plane)
                planeMesh.textureScale.x = val;
            else if (curMesh == MeshType.Cylinder)
                cylMesh.textureScale.x = val;
        }
    }

    public void yChanged(float val)
    {
        if (xFormControl.curMode == XformControl.mode.translate)
        {
            if (curMesh == MeshType.Plane)
                planeMesh.textureOffset.y = val;
            else if (curMesh == MeshType.Cylinder)
                cylMesh.textureOffset.y = val;
        }

        if (xFormControl.curMode == XformControl.mode.scale)
        {
            if (curMesh == MeshType.Plane)
                planeMesh.textureScale.y = val;
            else if (curMesh == MeshType.Cylinder)
                cylMesh.textureScale.y = val;
        }
    }

    public void zChanged(float val)
    {
        if (xFormControl.curMode == XformControl.mode.rotate)
        {
            if (curMesh == MeshType.Plane)
                planeMesh.textureRotation = val;
            if (curMesh == MeshType.Cylinder)
                cylMesh.textureRotation = val;
        }
    }
}
