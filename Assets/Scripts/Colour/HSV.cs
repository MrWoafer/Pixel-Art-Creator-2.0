using UnityEngine;

namespace PAC.Colour
{
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
}
