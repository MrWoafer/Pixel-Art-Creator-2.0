using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HSLColourPicker : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField]
    private float _mouseSensitivity = 0.1f;
    public float mouseSensitivity
    {
        get
        {
            return _mouseSensitivity;
        }
    }
    [SerializeField]
    private float _slowSensitivityScalar = 0.5f;
    public float slowSensitivityScalar
    {
        get
        {
            return _slowSensitivityScalar;
        }
    }

    [Header("Events")]
    [SerializeField]
    private UnityEvent onColourChanged = new UnityEvent();

    [Header("References")]
    private HSLColourBox hueSaturationBox;
    private HSLColourSlider lightnessSlider;

    public float hue
    {
        get
        {
            return hueSaturationBox.hue;
        }
    }
    public float saturation
    {
        get
        {
            return hueSaturationBox.saturation;
        }
    }
    public float lightness
    {
        get
        {
            return lightnessSlider.lightness;
        }
    }
    public float alpha { get; private set; } = 1f;

    public HSL hsl
    {
        get
        {
            return new HSL(hue, saturation, lightness, alpha);
        }
    }

    void Awake()
    {
        hueSaturationBox = transform.Find("Hue Saturation Box").GetComponent<HSLColourBox>();
        lightnessSlider = transform.Find("Lightness Slider").GetComponent<HSLColourSlider>();
    }

    public void UpdateColour()
    {
        onColourChanged.Invoke();
        lightnessSlider.SetHueSaturation(hsl.h, hsl.s);
    }

    public void SetColour(Color colour)
    {
        HSL newHSL = new HSL(colour);
        hueSaturationBox.hue = newHSL.h;
        hueSaturationBox.saturation = newHSL.s;
        lightnessSlider.lightness = newHSL.l;
        alpha = colour.a;

        UpdateColour();
    }

    public void SubscribeToColourChange(UnityAction call)
    {
        onColourChanged.AddListener(call);
    }
}
