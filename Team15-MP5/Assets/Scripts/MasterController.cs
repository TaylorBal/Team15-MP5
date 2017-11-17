using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MasterController : MonoBehaviour {

    public MyMesh theMesh;
    public Slider sliderN;
    public Slider sliderM;

	// Use this for initialization
	void Start ()
    {
        sliderN.onValueChanged.AddListener(ChangeN);
        sliderM.onValueChanged.AddListener(ChangeM);
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void ChangeN(float val)
    {
        theMesh.n = (int)val;
    }

    public void ChangeM(float val)
    {
        theMesh.m = (int)val;
    }
}
