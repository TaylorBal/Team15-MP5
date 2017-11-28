using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexBehavior : MonoBehaviour {

    public Material basicMat;
    public Material selectableMat;
    public Material selectMat;

    public GameObject axesType;
    private GameObject axes;
    private bool hasAxes = false; //prevents multiple axes instantiating

    //necessary variables for manipulation
    private MyMesh manipMesh = null;
    private Vector3 manipSensitivity = new Vector3(1.0f, 1.0f, 1.0f);
    public int vertIndex = -1;
    private bool selectable = true;

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update ()
    {
        //If axes exist, keep position and rotation synced
        if (hasAxes)
        {
            axes.transform.localPosition = transform.localPosition;
            //axes.transform.localRotation = transform.localRotation;
        }
	}

    //Set the values necessary to manipulate our mesh
    //TODO: set material according to selectable status
    public void Init(MyMesh mesh, int index, bool selectable, bool startActive)
    {
        manipMesh = mesh;
        vertIndex = index;
        this.selectable = selectable;
        if (selectable)
        {
            basicMat = selectableMat;
            GetComponent<MeshRenderer>().material = basicMat;
        }

        gameObject.SetActive(startActive);
    }

    public void SetAxesOrientation(Quaternion orientation)
    {
        if(hasAxes)
        {
            axes.transform.localRotation = orientation;
        }
    }

    public void Select()
    {
        GetComponent<MeshRenderer>().material = selectMat;
        if (!hasAxes)
        {
            axes = Instantiate(axesType);
            hasAxes = true;
        }
        SetAxesOrientation(manipMesh.GetVBOrientation(vertIndex));
    }

    public void Deselect()
    {   
        GetComponent<MeshRenderer>().material = basicMat;
        if(hasAxes)
        {
            Destroy(axes);
            hasAxes = false;
        }
    }

    public void MoveX(Vector3 inputVec)
    {
        if (manipMesh != null && hasAxes == true)
        {
            //project the input vector along the axis of the 
            float mag = manipSensitivity.x * Vector3.Dot(inputVec, axes.transform.right);
            manipMesh.MoveVertex(vertIndex, mag * Vector3.right);// axes.transform.right);
        }
    }

    public void MoveY(Vector3 inputVec)
    {
        if (manipMesh != null && hasAxes == true)
        {
            //project the input vector along the axis of the 
            float mag = manipSensitivity.y * Vector3.Dot(inputVec, axes.transform.up);
            manipMesh.MoveVertex(vertIndex, mag * Vector3.up);// axes.transform.up);
        }
    }

    public void MoveZ(Vector3 inputVec)
    {
        if (manipMesh != null && hasAxes == true)
        {
            //project the input vector along the axis of the 
            float mag = manipSensitivity.z * Vector3.Dot(inputVec, axes.transform.forward);
            manipMesh.MoveVertex(vertIndex, mag * Vector3.forward);// axes.transform.forward);
        }
    }
    public bool IsSelectable() { return selectable; }
}
