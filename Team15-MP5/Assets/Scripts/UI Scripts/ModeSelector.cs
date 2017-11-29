using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum MMode      //modify mode
{
    translate,
    rotate,
    scale
}

public class ModeSelector : MonoBehaviour
{
    public Toggle translateToggle;
    public Toggle rotateToggle;
    public Toggle scaleToggle;

    private MMode currentMode;

    public delegate void ModeCallbackDelegate(MMode mode);
    private ModeCallbackDelegate mCallback = null;

    // Use this for initialization
    void Start()
    {
        Debug.Assert(translateToggle != null);
        Debug.Assert(rotateToggle != null);
        Debug.Assert(scaleToggle != null);

        translateToggle.onValueChanged.AddListener(TranslateChanged);
        rotateToggle.onValueChanged.AddListener(RotateChanged);
        scaleToggle.onValueChanged.AddListener(ScaleChanged);

        currentMode = MMode.translate;
    }

    public void SetModeListener(ModeCallbackDelegate listener)
    {
        mCallback = listener;
    }

    void TranslateChanged(bool val)
    {
        if (val)
        {
            currentMode = MMode.translate;

            if (mCallback != null)
            {
                mCallback(currentMode);
            }
        }
    }

    void RotateChanged(bool val)
    {
        if (val)
        {
            currentMode = MMode.rotate;

            if (mCallback != null)
            {
                mCallback(currentMode);
            }
        }
    }

    void ScaleChanged(bool val)
    {
        if (val)
        {
            currentMode = MMode.scale;

            if (mCallback != null)
            {
                mCallback(currentMode);
            }
        }
    }

    public MMode GetMode()
    {
        return currentMode;
    }
}
