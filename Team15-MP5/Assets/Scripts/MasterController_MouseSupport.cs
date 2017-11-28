using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class MasterController : MonoBehaviour {

    public enum ManipMode
    {
        None,
        CamManip,
        VertexManip
    }
    ManipMode curManipMode = ManipMode.None;

    public KeyCode CameraKey = KeyCode.LeftAlt;
    public KeyCode VertexKey = KeyCode.LeftControl;
    //public CamModeIndicator modeIndicator = null;

    /// <summary>
    ///  0 = null; 1 = x; 2 = y; 3 = z
    /// </summary>
    int orientation = 0;

    void InputService()
    {
        UpdateManipMode();

        switch(curManipMode)
        {
            case ManipMode.CamManip:
                {
                    ProcessCameraControl();
                    break;
                }
            case ManipMode.VertexManip:
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        SelectAnObject();
                    }
                    else if (Input.GetMouseButton(0))
                    {
                        DragVert();
                    }
                    break;
                }
            case ManipMode.None:
                break;
        }
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

    void UpdateManipMode()
    {
        switch(curManipMode)
        {
            case ManipMode.CamManip:
                {
                    if(Input.GetKeyUp(CameraKey))
                    {
                        if(Input.GetKey(VertexKey))                 //revert to vertex manipulation if user holding key
                        {
                            curManipMode = ManipMode.VertexManip;
                            SetVertexHandles(true);
                        }
                        else
                        {
                            curManipMode = ManipMode.None;
                        }
                    }
                    break;
                }
            case ManipMode.VertexManip:             
                {
                    if(Input.GetKeyUp(VertexKey) && vertBehavior == null)       //if we release the key leave vertex mode (unless there is a vertex still selected
                    {
                        curManipMode = ManipMode.None;
                        SetVertexHandles(false);
                    }
                    else if(!Input.GetKey(VertexKey) && vertBehavior == null)     //if we're currently being held in vertex mode by a selected handle, but just lost it, and we're not holding the vertex key
                    {
                        curManipMode = ManipMode.None;
                        SetVertexHandles(false);
                    }
                    else if(Input.GetKeyDown(CameraKey))        //CamManip can override VertexManip
                    {
                        curManipMode = ManipMode.CamManip;
                        SetVertexHandles(false);

                        //make sure we clear out the vetBehavior
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
                    }
                    break;
                }
            case ManipMode.None:
                {
                    if(Input.GetKeyDown(VertexKey))
                    {
                        curManipMode = ManipMode.VertexManip;
                        SetVertexHandles(true);
                    }
                    else if(Input.GetKeyDown(CameraKey))
                    {
                        curManipMode = ManipMode.CamManip;
                    }

                    break;
                }
                
        }
    }

    void SelectAnObject()
    {
        GameObject selectedObject;
        Vector3 hitPoint;

        bool hit = MouseSelectObject(out selectedObject, out hitPoint, LayerMask.GetMask("VertManip"));

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
                    }
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

        Vector3 mousePos = Input.mousePosition;
        //find a vector in world space corresponding to the deltaMouse
        Vector3 vEnd = MainCamera.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, MainCamera.nearClipPlane));
        Vector3 vStart = MainCamera.ScreenToWorldPoint(new Vector3(mousePos.x - 100 * deltaMouse.x,  mousePos.y - 100 * deltaMouse.y, MainCamera.nearClipPlane));
        Vector3 worldDir = vEnd - vStart;

        if (vertBehavior == null)
            return;

        switch(curManipAxis)
        {
            case manipAxis.xAxis:
                vertBehavior.MoveX(worldDir);
                break;
            case manipAxis.yAxis:
                vertBehavior.MoveY(worldDir);
                break;
            case manipAxis.zAxis:
                vertBehavior.MoveZ(worldDir);
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

        //if (Input.GetKey(CameraKey)) //enable camera manipulation
        //{
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
        //}
        //else
        //{
            //modeIndicator.SetMode(CamMode.None);
        //}
    }
}
