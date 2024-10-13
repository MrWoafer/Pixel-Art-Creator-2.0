using UnityEngine;

namespace PAC.Colour
{
    /// <summary>
    /// Represents a colour in HSL form.
    /// </summary>
    public struct HSL
    {
        /// <summary>Hue.</summary>
        public float h { get; set; }
        /// <summary>Saturation.</summary>
        public float s { get; set; }
        /// <summary>Lightness.</summary>
        public float l { get; set; }
        /// <summary>Alpha.</summary>
        public float a { get; set; }

        public HSV hsv => ToHSV();
        public Color color => ToColor();

        public HSL(float hue, float saturation, float lightness) : this(hue, saturation, lightness, 1f) { }
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
            return ToString(3);
        }
        public string ToString(int decimalPlaces)
        {
            if (decimalPlaces < 0)
            {
                throw new System.Exception("Cannot have a negative number of decimal places. Decimal places: " + decimalPlaces);
            }
            return "(" + h.ToString("n" + decimalPlaces) + ", " + s.ToString("n" + decimalPlaces) + ", " + l.ToString("n" + decimalPlaces) + ", " + a.ToString("n" + decimalPlaces) + ")";
        }
    }
}
