using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum CamMode
{
    None,
    Tumble,
    Track,
    Dolly
}

public class MainCameraController : MonoBehaviour {

    //Centered Mode variables
    public Transform LookAtPosition = null;
    public Vector3 sensitivity = Vector3.one;

    public float zoomMin = 0.5f;
    public float zoomMax = 20.0f;

    public float tiltMin = -45.0f;
    public float tiltMax = 45.0f;

    void OnValidate()
    {
        Debug.Assert(zoomMin <= zoomMax);
        Debug.Assert(tiltMin <= tiltMax);
        Debug.Assert(LookAtPosition != null);
    }

    // Use this for initialization
    void Start () {
        transform.LookAt(LookAtPosition);
	}
	
	// Update is called once per frame
	void Update () {

        //make sure we're still looking at lookAtPos
        Tumble(Vector3.zero);
    }

    public void SetLookAtPos(Transform target)
    {
        LookAtPosition = target;
    }

    //moves camera using mouse delta values
    //according to its mode
    public void MoveCamera(CamMode mode, Vector3 deltaMouse)
    {
        switch(mode)
        {
            case CamMode.Tumble:
                Tumble(deltaMouse);
                break;
            case CamMode.Track:
                Track(deltaMouse);
                break;
            case CamMode.Dolly:
                Dolly(deltaMouse);
                break;
        }
    }

    void Tumble(Vector3 delta)
    {
        //Orbiting around Y axis presents no problems
        OrbitOnAxis(delta.x * sensitivity.x, transform.up);

        //Only orbit around horiz axis if within range

        //check if it will be with bounds
        float tmpAngle = transform.localEulerAngles.x + delta.y;
        if (tmpAngle > 360.0f + tiltMin || tmpAngle < tiltMax)
        {
            OrbitOnAxis(delta.y * sensitivity.x, transform.right);
        }
    }

    //Move LookAt with us so we stay on a plane
    void Track(Vector3 delta)
    {
        Vector3 up = transform.up.normalized * -delta.y;
        Vector3 right = transform.right.normalized * -delta.x;

        Vector3 transVec = up + right;

        
        LookAtPosition.localPosition += transVec * sensitivity.y;
        transform.localPosition += transVec * sensitivity.y;
    }

    //Scroll wheel moves towards/away from lookAtPosition
    void Dolly(Vector3 delta)
    {
        float moveDist = delta.z * sensitivity.z;
        Vector3 V = LookAtPosition.localPosition - transform.localPosition;

        if (V.magnitude < zoomMin && moveDist > 0)
            return;

        if (V.magnitude > zoomMax && moveDist < 0)
            return;

        transform.localPosition += moveDist * V.normalized;
    }

    void OrbitOnAxis(float deltaAngle, Vector3 axis)
    {
        // orbit with respect to the transform.right axis

        // 1. Rotation of the viewing direction by right axis
        Quaternion q = Quaternion.AngleAxis(deltaAngle, axis);

        // 2. we need to rotate the camera position
        Matrix4x4 r = Matrix4x4.TRS(Vector3.zero, q, Vector3.one);
        Matrix4x4 invP = Matrix4x4.TRS(-LookAtPosition.localPosition, Quaternion.identity, Vector3.one);

        r = invP.inverse * r * invP;

        Vector3 newCameraPos = r.MultiplyPoint(transform.localPosition);
        transform.localPosition = newCameraPos;
        transform.LookAt(LookAtPosition);
    }
}
