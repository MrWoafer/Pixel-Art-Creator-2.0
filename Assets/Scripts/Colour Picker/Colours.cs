using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HSL
{
    public float h { get; set; }
    public float s { get; set; }
    public float l { get; set; }
    public float a { get; set; }

    public HSV hsv
    {
        get
        {
            return ToHSV();
        }
    }
    public Color color
    {
        get
        {
            return ToColor();
        }
    }

    public HSL(float hue, float saturation, float lightness)
    {
        h = hue;
        s = saturation;
        l = lightness;
        a = 1f;
    }

    public HSL(float hue, float saturation, float lightness, float alpha)
    {
        h = hue;
        s = saturation;
        l = lightness;
        a = alpha;
    }

    public HSL(Color rgb)
    {
        HSL hsl = new HSV(rgb).ToHSL();
        h = hsl.h;
        s = hsl.s;
        l = hsl.l;
        a = hsl.a;
    }

    public HSV ToHSV()
    {
        float v = l + s * Mathf.Min(l, 1f - l);
        float s_v;
        if (v == 0f)
        {
            s_v = 0f;
        }
        else
        {
            s_v = 2f * (1f - l / v);
        }

        return new HSV(h, s_v, v, a);
    }

    public Color ToColor()
    {
        return ToHSV().ToColor();
    }

    public static explicit operator HSV(HSL hsl)
    {
        return hsl.ToHSV();
    }
    public static explicit operator Color(HSL hsl)
    {
        return hsl.ToColor();
    }

    public override string ToString()
    {
        return "(" + h + ", " + s + ", " + l + ")";
    }
}

public class HSV
{
    public float h { get; set; }
    public float s { get; set; }
    public float v { get; set; }
    public float a { get; set; }

    public HSL hsl
    {
        get
        {
            return ToHSL();
        }
    }
    public Color color
    {
        get
        {
            return ToColor();
        }
    }

    public HSV(float hue, float saturation, float value)
    {
        h = hue;
        s = saturation;
        v = value;
        a = 1f;
    }

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
        return "(" + h + ", " + s + ", " + v + ")";
    }
}

public enum BlendMode
{
    Normal = 0,
    Overlay = 1,
    Multiply = 2,
    Screen = 3,
    Add = 4,
    Subtract = 5
}

public static class Colours
{
    public static Color Blend(Color topColour, Color bottomColour, BlendMode blendMode)
    {
        switch (blendMode)
        {
            case BlendMode.Normal: return NormalBlend(topColour, bottomColour);
            case BlendMode.Overlay: return OverlayBlend(topColour, bottomColour);
            case BlendMode.Multiply: return MultiplyBlend(topColour, bottomColour);
            case BlendMode.Screen: return ScreenBlend(topColour, bottomColour);
            case BlendMode.Add: return AddBlend(topColour, bottomColour);
            case BlendMode.Subtract: return SubtractBlend(topColour, bottomColour);
            default: throw new System.Exception("Unknown / unimplemented blend mode: " + blendMode);
        }
    }

    public static Color NormalBlend(Color topColour, Color bottomColour)
    {
        float a = (1f - topColour.a) * bottomColour.a + topColour.a;

        if (a == 0)
        {
            return bottomColour;
        }
        else
        {
            float r = ((1f - topColour.a) * bottomColour.a * bottomColour.r + topColour.a * topColour.r) / a;
            float g = ((1f - topColour.a) * bottomColour.a * bottomColour.g + topColour.a * topColour.g) / a;
            float b = ((1f - topColour.a) * bottomColour.a * bottomColour.b + topColour.a * topColour.b) / a;

            return new Color(r, g, b, a);
        }
    }

    public static Color OverlayBlend(Color topColour, Color bottomColour)
    {
        if (topColour.a == 0f)
        {
            return bottomColour;
        }
        if (bottomColour.a == 0f)
        {
            return topColour;
        }

        float r, g, b, a;

        if (bottomColour.r < 0.5f)
        {
            r = 2f * bottomColour.r * topColour.r;
        }
        else
        {
            r = 1f - 2f * (1f - bottomColour.r) * (1f - topColour.r);
        }

        if (bottomColour.g < 0.5f)
        {
            g = 2f * bottomColour.g * topColour.g;
        }
        else
        {
            g = 1f - 2f * (1f - bottomColour.g) * (1f - topColour.g);
        }

        if (bottomColour.b < 0.5f)
        {
            b = 2f * bottomColour.b * topColour.b;
        }
        else
        {
            b = 1f - 2f * (1f - bottomColour.b) * (1f - topColour.b);
        }

        if (bottomColour.a < 0.5f)
        {
            a = 2f * bottomColour.a * topColour.a;
        }
        else
        {
            a = 1f - 2f * (1f - bottomColour.a) * (1f - topColour.a);
        }

        return new Color(r, g, b, a);
    }

    public static Color Multiply(Color colour1, Color colour2)
    {
        return colour1 * colour2;
    }
    public static Color MultiplyBlend(Color colour1, Color colour2)
    {
        if (colour1.a == 0f)
        {
            return colour2;
        }
        if (colour2.a == 0f)
        {
            return colour1;
        }
        return colour1 * colour2;
    }

    public static Color ScreenBlend(Color colour1, Color colour2)
    {
        if (colour1.a == 0f)
        {
            return colour2;
        }
        if (colour2.a == 0f)
        {
            return colour1;
        }
        return new Color(1f, 1f, 1f, 1f) - (new Color(1f, 1f, 1f, 1f) - colour1) * (new Color(1f, 1f, 1f, 1f) - colour2);
    }

    public static Color AddBlend(Color colour1, Color colour2)
    {
        return colour1 + colour2;
    }

    public static Color SubtractBlend(Color subtractFrom, Color toSubtract)
    {
        return subtractFrom - toSubtract;
    }

    public static BlendMode StringToBlendMode(string blendMode)
    {
        switch (blendMode.ToLower())
        {
            case "normal": return BlendMode.Normal;
            case "overlay": return BlendMode.Overlay;
            case "multiply": return BlendMode.Multiply;
            case "screen": return BlendMode.Screen;
            case "add": return BlendMode.Add;
            case "subtract": return BlendMode.Subtract;
            default: throw new System.Exception("Unknown / unimplemented blend mode: " + blendMode);
        }
    }

    public static Color Invert(Color colour, bool invertAlpha = false)
    {
        return new Color(1f - colour.r, 1f - colour.g, 1f - colour.b, invertAlpha ? 1f - colour.a : colour.a);
    }
}
