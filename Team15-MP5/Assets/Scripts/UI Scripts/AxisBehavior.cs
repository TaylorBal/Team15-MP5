using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisBehavior : MonoBehaviour {

    public Material basicMat;
    public Material selectMat;

    /// <summary>
    /// 0 = null; 1 = x; 2 = y; 3 = z
    /// </summary>
    public int orientation = 0;

    public int Select()
    {
        GetComponent<MeshRenderer>().material = selectMat;
        return orientation; //return selected orientation
    }

    public int Deselect()
    {
        GetComponent<MeshRenderer>().material = basicMat;
        return 0; //null orientation
    }
}
