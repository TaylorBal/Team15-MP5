using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TexXFormControl : MonoBehaviour
{

    private MyMesh mSelected = null;

    public Text title = null;
    public SlidersController sliders = null;
    public Vector2 previousSliderVals = Vector2.zero;
    public ModeSelector modes = null;

    private MMode currentMode;

    // Use this for initialization
    void Start()
    {
        Debug.Assert(title != null);
        Debug.Assert(sliders != null);
        Debug.Assert(modes != null);

        modes.SetModeListener(ModeChanged);
        sliders.SetSlidersControllerListeners(XValueChanged, YValueChanged, ZValueChanged);

        currentMode = modes.GetMode();
        ModeChanged(currentMode);
        SetText();
        SetSliders();
    }


    public void SetSelected(MyMesh selected)
    {
        mSelected = selected;
        previousSliderVals = Vector2.zero;
        SetText();
        SetSliders();
    }

    public MyMesh GetSelected()
    {
        return mSelected;
    }

    //if the Mode changed, update the slider mode
    void ModeChanged(MMode mode)
    {
        currentMode = mode;
        previousSliderVals = GetSelectedXformParam();
        SetSlidersMinMax();
    }

    void SetSlidersMinMax()
    {
        float min = 0.0f;
        float max = 0.0f;

        switch (currentMode)
        {
            case MMode.translate:
                {
                    min = -30.0f;
                    max = 30.0f;
                    break;
                }
            case MMode.rotate:
                {
                    min = -180.0f;
                    max = 180.0f;
                    break;
                }
            case MMode.scale:
                {
                    min = 0.1f;
                    max = 20.0f;
                    break;
                }
        }

        sliders.SetSliderRanges(min, max, previousSliderVals);
    }

    //change the text to reflect mSelected
    public void SetText()
    {
        if (mSelected != null)
        {
            title.text = "Selected Object: " + mSelected.name;
        }
        else
        {
            title.text = "No Selected Object";
        }
    }

    //Set the sliders to reflect the scene
    public void SetSliders()
    {
        Vector3 p = GetSelectedXformParam();
        sliders.SetValues(p);
    }

    void XValueChanged(float v)
    {
        Vector3 p = GetSelectedXformParam();
        // if not in rotation, next two lines of work would be wasted
        float dx = v - previousSliderVals.x;
        previousSliderVals.x = v;
        p.x = v;
        SetSelectedXform(ref p, ref q);
    }

    void YValueChanged(float v)
    {
        Vector3 p = GetSelectedXformParam();
        // if not in rotation, next two lines of work would be wasted
        float dy = v - previousSliderVals.y;
        previousSliderVals.y = v;
        Quaternion q = Quaternion.AngleAxis(dy, Vector3.up);
        p.y = v;
        SetSelectedXform(ref p, ref q);
    }

    void ZValueChanged(float v)
    {
        Vector3 p = GetSelectedXformParam();
        // if not in rotation, next two lines of work would be wasterd
        float dz = v - previousSliderVals.z;
        previousSliderVals.z = v;
        Quaternion q = Quaternion.AngleAxis(dz, Vector3.forward);
        p.z = v;
        SetSelectedXform(ref p, ref q);
    }

    void SetSelectedXform(ref Vector3 p, ref Quaternion q)
    {
        if (mSelected == null)
            return;

        switch (currentMode)
        {
            case MMode.translate:
                mSelected.localPosition = p;
                break;
            case MMode.rotate:
                mSelected.localRotation *= q;
                break;
            case MMode.scale:
                mSelected.localScale = p;
                break;
        }
    }

    Vector3 GetSelectedXformParam()
    {
        Vector3 p = Vector3.zero;

        switch (currentMode)
        {
            case MMode.translate:
                if (mSelected != null)
                    p = mSelected.localPosition;
                else
                    p = Vector3.zero;
                break;
            case MMode.scale:
                if (mSelected != null)
                    p = mSelected.localScale;
                else
                    p = Vector3.one;
                break;
            case MMode.rotate:
                p = Vector3.zero;
                break;
        }

        return p;
    }
}
