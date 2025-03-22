using UnityEngine;

namespace PAC.Colour
{
    /// <summary>
    /// Represents a colour in HSV form.
    /// </summary>
    public readonly struct HSV
    {
        /// <summary>Hue.</summary>
        public readonly float h { get; init; }
        /// <summary>Saturation.</summary>
        public readonly float s { get; init; }
        /// <summary>Value.</summary>
        public readonly float v { get; init; }
        /// <summary>Alpha.</summary>
        public readonly float a { get; init; }

        public HSV(float hue, float saturation, float value) : this(hue, saturation, value, 1f) { }
        public HSV(float hue, float saturation, float value, float alpha)
        {
            h = hue;
            s = saturation;
            v = value;
            a = alpha;
        }

        public static explicit operator HSV(HSL hsl)
        {
            float v = hsl.l + hsl.s * Mathf.Min(hsl.l, 1f - hsl.l);
            float s_v;
            if (v == 0f)
            {
                s_v = 0f;
            }
            else
            {
                s_v = 2f * (1f - hsl.l / v);
            }

            return new HSV(hsl.h, s_v, v, hsl.a);
        }

        public static explicit operator HSV(Color rgba)
        {
            Color.RGBToHSV(rgba, out float h, out float s, out float v);
            return new HSV(h, s, v, rgba.a);
        }
        public static explicit operator Color(HSV hsv)
        {
            Color colour = Color.HSVToRGB(hsv.h, hsv.s, hsv.v);
            colour.a = hsv.a;
            return colour;
        }

        public override string ToString() => ToString("n3");
        /// <summary>
        /// Applies <paramref name="format"/> to each component.
        /// </summary>
        public string ToString(string format) => $"HSV({h.ToString(format)}, {s.ToString(format)}, {v.ToString(format)}, {a.ToString(format)})";
    }
}
