using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a colour in HSV form.
/// </summary>
public struct HSV
{
    /// <summary>Hue.</summary>
    public float h { get; set; }
    /// <summary>Saturation.</summary>
    public float s { get; set; }
    /// <summary>Value.</summary>
    public float v { get; set; }
    /// <summary>Alpha.</summary>
    public float a { get; set; }

    public HSL hsl => ToHSL();
    public Color color => ToColor();

    public HSV(float hue, float saturation, float value) : this(hue, saturation, value, 1f) { }
    public HSV(float hue, float saturation, float value, float alpha)
    {
        h = hue;
        s = saturation;
        v = value;
        a = alpha;
    }

    public HSV(Color rgb)
    {
        float _h, _s, _v;
        Color.RGBToHSV(rgb, out _h, out _s, out _v);

        h = _h;
        s = _s;
        v = _v;
        a = rgb.a;
    }

    public HSL ToHSL()
    {
        float l = v * (1f - s / 2f);
        float s_l;
        if (l == 0f || l == 1f)
        {
            s_l = 0f;
        }
        else
        {
            s_l = (v - l) / Mathf.Min(l, 1f - l);
        }

        return new HSL(h, s_l, l, a);
    }

    public Color ToColor()
    {
        Color colour = Color.HSVToRGB(h, s, v);
        colour.a = a;
        return colour;
    }

    public static explicit operator HSL(HSV hsv)
    {
        return hsv.ToHSL();
    }
    public static explicit operator Color(HSV hsv)
    {
        return hsv.ToColor();
    }

    public override string ToString()
    {
        return ToString(3);
    }
    public string ToString(int decimalPlaces)
    {
        if (decimalPlaces < 0)
        {
            throw new System.Exception("Cannot have a negative number of decimal places. Decimal places: " + decimalPlaces);
        }
        return "(" + h.ToString("n" + decimalPlaces) + ", " + s.ToString("n" + decimalPlaces) + ", " + v.ToString("n" + decimalPlaces) + ", " + a.ToString("n" + decimalPlaces) + ")";
    }
}
