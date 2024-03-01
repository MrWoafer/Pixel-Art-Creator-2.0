using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorExtensions
{
    /// <summary>
    /// Inverts the colour, i.e. does 1 - value for each component.
    /// </summary>
    /// <param name="invertAlpha">Whether to invert the alpha value as well.</param>
    public static Color Invert(this Color colour, bool invertAlpha = false)
    {
        return new Color(1f - colour.r, 1f - colour.g, 1f - colour.b, invertAlpha ? 1f - colour.a : colour.a);
    }

    public static HSV ToHSV(this Color color)
    {
        return new HSV(color);
    }

    public static HSL ToHSL(this Color color)
    {
        return new HSL(color);
    }
}
