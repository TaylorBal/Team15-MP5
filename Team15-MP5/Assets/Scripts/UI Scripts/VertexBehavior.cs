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
    public void Init(MyMesh mesh, int index, bool selectable = true)
    {
        manipMesh = mesh;
        vertIndex = index;
        this.selectable = selectable;
        if (selectable)
        {
            basicMat = selectableMat;
            GetComponent<MeshRenderer>().material = basicMat;
        }
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

    public void MoveX(float deltaX)
    {
        if (manipMesh != null)
        {
            manipMesh.MoveVertex(vertIndex, deltaX * Vector3.right);
        }
    }

    public void MoveY(float deltaY)
    {
        if (manipMesh != null)
        {
            manipMesh.MoveVertex(vertIndex, deltaY * Vector3.up);
        }
    }

    public void MoveZ(float deltaZ)
    {
        if (manipMesh != null)
        {
            manipMesh.MoveVertex(vertIndex, deltaZ * Vector3.forward);
        }
    }

    public bool IsSelectable() { return selectable; }
}
