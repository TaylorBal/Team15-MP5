using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlidersController : MonoBehaviour
{

    public SliderWithEcho XSlider;
    public SliderWithEcho YSlider;
    public SliderWithEcho ZSlider;


    private Vector3 vecVal = Vector3.zero;

    public delegate void SliderCallbackDelegate(float val);
    private SliderCallbackDelegate mXCallback = null;
    private SliderCallbackDelegate mYCallback = null;
    private SliderCallbackDelegate mZCallback = null;

    // Use this for initialization
    void Start()
    {
        Debug.Assert(XSlider != null);
        Debug.Assert(YSlider != null);
        Debug.Assert(ZSlider != null);

        XSlider.SetSliderLabel("X:");
        YSlider.SetSliderLabel("Y:");
        ZSlider.SetSliderLabel("Z:");

        XSlider.SetSliderListener(NewXValue);
        YSlider.SetSliderListener(NewYValue);
        ZSlider.SetSliderListener(NewZValue);

    }

    public void SetSlidersControllerListeners(SliderCallbackDelegate Xlistener, SliderCallbackDelegate YListener, SliderCallbackDelegate ZListener)
    {
        mXCallback = Xlistener;
        mYCallback = YListener;
        mZCallback = ZListener;
    }

    public void SetSliderRanges(float min, float max, Vector3 startValues)
    {
        XSlider.InitSliderRange(min, max, startValues.x);
        YSlider.InitSliderRange(min, max, startValues.y);
        ZSlider.InitSliderRange(min, max, startValues.z);
    }

    public void SetValues(Vector3 values)
    {
        XSlider.SetSliderValue(values.x);
        YSlider.SetSliderValue(values.y);
        ZSlider.SetSliderValue(values.z);
    }

    void NewXValue(float newX)
    {
        vecVal.x = newX;

        if (mXCallback != null)
            mXCallback(newX);
    }

    void NewYValue(float newY)
    {
        vecVal.y = newY;

        if (mYCallback != null)
            mYCallback(newY);
    }

    void NewZValue(float newZ)
    {
        vecVal.z = newZ;

        if (mZCallback != null)
            mZCallback(newZ);
    }
}
