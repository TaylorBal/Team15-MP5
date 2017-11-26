using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MasterController : MonoBehaviour {

    public Vector3 mouseSensitivity = new Vector3(1.0f, 1.0f, 1.0f);

    /// <summary>
    ///  0 = null; 1 = x; 2 = y; 3 = z
    /// </summary>
    int orientation = 0;

    void LMBService()
    {
        if(Input.GetMouseButtonDown(0))
        {
            SelectAnObject();
        }
        else if(Input.GetMouseButton(0))
        {
            DragVert();
        }

        //ProcessCameraInput()
    }

    void SelectAnObject()
    {
        RaycastHit hitInfo = new RaycastHit();
        bool hit = Physics.Raycast(MainCamera.ScreenPointToRay(Input.mousePosition), out hitInfo, Mathf.Infinity, LayerMask.GetMask("VertManip"));
        //1 is mask for default layer

        if (hit) //hit vertex and axes
        {
            //Select vertex
            if (hitInfo.transform.gameObject.tag == "Handle")
            {
                GameObject newHandle = hitInfo.transform.gameObject;
                VertexBehavior newVB = newHandle.GetComponent<VertexBehavior>();

                if (newVB.IsSelectable())
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

                    vertHandle = newHandle;
                    vertBehavior = newVB;
                    if (vertBehavior != null)
                        vertBehavior.Select();
                }
            }

            //Select axes
            else if(hitInfo.transform.gameObject.tag == "Axes")
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

    private void DragVert()
    {
        //find the delta mouse
        Vector3 deltaMouse;
        deltaMouse.x = Input.GetAxis("Mouse X");
        deltaMouse.y = Input.GetAxis("Mouse Y");
        deltaMouse.z = Input.GetAxis("Mouse ScrollWheel");     //Input.mouseposition only stores in x, y

        deltaMouse.x *= mouseSensitivity.x;
        deltaMouse.y *= mouseSensitivity.y;
        deltaMouse.z *= mouseSensitivity.z;

        if (vertBehavior == null)
            return;

        switch(curManipAxis)
        {
            case manipAxis.xAxis:
                vertBehavior.MoveX(deltaMouse.x * mouseSensitivity.x);
                break;
            case manipAxis.yAxis:
                vertBehavior.MoveY(deltaMouse.y * mouseSensitivity.y);
                break;
            case manipAxis.zAxis:
                vertBehavior.MoveZ(deltaMouse.z * mouseSensitivity.z);
                break;
            case manipAxis.nullAxis:
                break;
        }
    }
}
