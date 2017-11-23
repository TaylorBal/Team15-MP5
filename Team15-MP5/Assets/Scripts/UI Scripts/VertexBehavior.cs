using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexBehavior : MonoBehaviour {

    public Material basicMat;
    public Material selectMat;

    public GameObject axesType;
    private GameObject axes;
    private bool hasAxes = false; //prevents multiple axes instantiating

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
            axes.transform.localRotation = transform.localRotation;
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
}
