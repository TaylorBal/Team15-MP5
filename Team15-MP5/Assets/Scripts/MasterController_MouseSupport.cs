using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MasterController : MonoBehaviour {

    public Vector3 mouseSensitivity = new Vector3(1.0f, 1.0f, 1.0f);
    public Vector3 cameraSensitivity = new Vector3(1.0f, 1.0f, 1.0f);
    //public CamModeIndicator modeIndicator = null;

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

        ProcessCameraControl();
    }

    bool MouseSelectObject(out GameObject obj, out Vector3 point, int mask)
    {
        RaycastHit hit = new RaycastHit();
        bool hitSuccess = Physics.Raycast(MainCamera.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, mask);

        if (hitSuccess)
        {
            obj = hit.transform.gameObject;
            point = hit.point;
        }
        else
        {
            obj = null;
            point = Vector3.zero;
        }

        return hitSuccess;
    }

    void SelectAnObject()
    {
        GameObject selectedObject;
        Vector3 hitPoint;

        bool hit = MouseSelectObject(out selectedObject, out hitPoint, LayerMask.GetMask("VertManip"));
        //1 is mask for default layer

        if (hit) //hit vertex and axes
        {
            //Select vertex
            if (selectedObject.tag == "Handle")
            {
                GameObject newHandle = selectedObject;
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
            else if(selectedObject.tag == "Axes")
            {
                //Deselect any axis currently selected
                if (axisBehavior != null)
                {
                    axisBehavior.Deselect();
                    axisBehavior = null;
                    axis = null;
                }

                axis = selectedObject;
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

        deltaMouse.x *= cameraSensitivity.x;
        deltaMouse.y *= cameraSensitivity.y;
        deltaMouse.z *= cameraSensitivity.z;

        if (vertBehavior == null)
            return;

        switch(curManipAxis)
        {
            case manipAxis.xAxis:
                vertBehavior.MoveX(deltaMouse.x * cameraSensitivity.x);
                break;
            case manipAxis.yAxis:
                vertBehavior.MoveY(deltaMouse.y * cameraSensitivity.y);
                break;
            case manipAxis.zAxis:
                vertBehavior.MoveZ(deltaMouse.z * cameraSensitivity.z);
                break;
            case manipAxis.nullAxis:
                break;
        }
    }

    private void ProcessCameraControl()
    {
        //There are three types of camera movement
        //Tumble - Alt + RMB + mouse X/Y
        //Track - Alt + LMB + mouse X/Y
        //Dolly - Alt + Scroll Wheel
        Vector3 deltaMouse;// = Input.mousePosition - oldMousePosition;
        deltaMouse.x = Input.GetAxis("Mouse X");
        deltaMouse.y = Input.GetAxis("Mouse Y");
        deltaMouse.z = Input.GetAxis("Mouse ScrollWheel");     //Input.mouseposition only stores in x, y

        deltaMouse.x *= mouseSensitivity.x;
        deltaMouse.y *= mouseSensitivity.y;
        deltaMouse.z *= mouseSensitivity.z;

        if (Input.GetKey(KeyCode.LeftAlt)) //enable camera manipulation
        {
            if (Input.GetMouseButton(0))         //Track
            {
                CamControl.MoveCamera(CamMode.Track, deltaMouse);
                //modeIndicator.SetMode(CamMode.Track);
            }
            else if (Input.GetMouseButton(1))    //Tumble
            {
                deltaMouse.y *= -1.0f;
                CamControl.MoveCamera(CamMode.Tumble, deltaMouse);
                //modeIndicator.SetMode(CamMode.Tumble);
            }
            else                                    //Dolly
            {
                CamControl.MoveCamera(CamMode.Dolly, deltaMouse);
                //modeIndicator.SetMode(CamMode.Dolly);
            }
        }
        else
        {
            //modeIndicator.SetMode(CamMode.None);
        }
    }
}
