using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MasterController : MonoBehaviour {

    /// <summary>
    ///  0 = null; 1 = x; 2 = y; 3 = z
    /// </summary>
    int orientation = 0;

    void LMBService()
    {
        if(Input.GetMouseButtonDown(0) || Input.GetMouseButton(0))
        {
            SelectAnObject();
        }
    }

    void SelectAnObject()
    {
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(MainCamera.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, 1);
        //1 is mask for default layer

        if (hit) //hit vertex and axes
        {
            //Select vertex
            if (hitInfo.transform.gameObject.tag == "Handle")
            {
                //Deselect any object currently selected
                if (vertBehavior != null)
                {
                    vertBehavior.Deselect();
                    vertBehavior = null;
                    vertHandle = null;
                }
                if (axisBehavior != null)
                {
                    axisBehavior.Deselect();
                    axisBehavior = null;
                    axis = null;
                }

                vertHandle = hitInfo.transform.gameObject;
                vertBehavior = vertHandle.GetComponent<VertexBehavior>();
                if (vertBehavior != null)
                    vertBehavior.Select();
            }

            //Select axes
            if(hitInfo.transform.gameObject.tag == "Axes")
            {
                //Deselect any axis currently selected
                if (axisBehavior != null)
                {
                    axisBehavior.Deselect();
                    axisBehavior = null;
                    axis = null;
                }

                axis = hitInfo.transform.gameObject;
                axisBehavior = axis.GetComponent<AxisBehavior>();
                if (axisBehavior != null)
                {
                    // Select axis and save orientation
                    switch(axisBehavior.Select())
                    {
                        case 1:
                            curManipAxis = manipAxis.xAxis;
                            break;
                        case 2:
                            curManipAxis = manipAxis.yAxis;
                            break;
                        case 3:
                            curManipAxis = manipAxis.zAxis;
                            break;
                        default:
                            curManipAxis = manipAxis.nullAxis;
                            break;
                    };
                    Debug.Log(curManipAxis);
                }
            }
        }
        else //did not hit anything
        {
            //Deselect any previous selection
            if (vertBehavior != null)
            {
                vertBehavior.Deselect();
                vertBehavior = null;
                vertHandle = null;
            }

            if(axisBehavior != null)
            {
                axisBehavior.Deselect();
                axisBehavior = null;
                axis = null;
            }
        }
    }
}
