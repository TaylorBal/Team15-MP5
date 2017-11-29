using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class XformControl_Tex: MonoBehaviour
{
    public Toggle T, R, S;
    public SliderWithEcho X, Y;
    public Text ObjectName;

    public enum mode { translate, rotate, scale };
    public mode curMode = mode.translate;

    private MyMesh mSelected;                            //It would be better to pass in a MyTex class, but i don't have time at this point :(

    // Use this for initialization
    void Start()
    {
        T.onValueChanged.AddListener(SetToTranslation);
        R.onValueChanged.AddListener(SetToRotation);
        S.onValueChanged.AddListener(SetToScaling);

        X.SetSliderListener(XValueChanged);
        Y.SetSliderListener(YValueChanged);

        T.isOn = true;
        R.isOn = false;
        S.isOn = false;
        SetToTranslation(true);
    }

    void SetToTranslation(bool v)
    {
        curMode = mode.translate;
        Vector2 p = GetSelectedXformParameter();
        X.InitSliderRange(-20, 20, p.x);
        Y.InitSliderRange(-20, 20, p.y);
        X.TheSlider.interactable = true;
        Y.TheSlider.interactable = true;
    }

    void SetToScaling(bool v)
    {
        curMode = mode.scale;
        Vector2 s = GetSelectedXformParameter();
        X.InitSliderRange(0.1f, 20, s.x);
        Y.InitSliderRange(0.1f, 20, s.y);
        X.TheSlider.interactable = true;
        Y.TheSlider.interactable = true;
    }

    void SetToRotation(bool v)
    {
        curMode = mode.rotate;
        Vector2 r = GetSelectedXformParameter();
        X.InitSliderRange(-180, 180, r.x);
        Y.InitSliderRange(-180, 180, r.y);
        X.TheSlider.interactable = false;
        Y.TheSlider.interactable = true;
    }

    void XValueChanged(float v)
    {
        Vector2 p = GetSelectedXformParameter();
        // if not in rotation, next line of work would be wasted
        p.x = v;
        SetSelectedXform(ref p, ref v);
    }

    void YValueChanged(float v)
    {
        Vector2 p = GetSelectedXformParameter();
        p.y = v;        
        SetSelectedXform(ref p, ref v);
    }

    public void SetSelectedObject(MyMesh m)
    {
        mSelected = m;
        if (m != null)
            ObjectName.text = "Selected:" + m.name;
        else
            ObjectName.text = "Selected: none";
        ObjectSetUI();
    }

    public void ObjectSetUI()
    {
        Vector2 p = GetSelectedXformParameter();
        X.SetSliderValue(p.x);
        Y.SetSliderValue(p.y);
    }

    private Vector2 GetSelectedXformParameter()
    {
        Vector2 p;

        if (T.isOn)
        {
            if (mSelected != null)
                p = mSelected.textureOffset;
            else
                p = Vector2.zero;
        }
        else if (S.isOn)
        {
            if (mSelected != null)
                p = mSelected.textureScale;
            else
                p = Vector3.one;
        }
        else
        {
            p = Vector2.zero;
        }
        return p;
    }

    private void SetSelectedXform(ref Vector2 p, ref float r)
    {
        if (mSelected == null)
            return;

        if (T.isOn)
        {
            mSelected.textureOffset = p;
        }
        else if (S.isOn)
        {
            mSelected.textureScale = p;
        }
        else
        {
            mSelected.textureRotation = r;
        }
    }
}