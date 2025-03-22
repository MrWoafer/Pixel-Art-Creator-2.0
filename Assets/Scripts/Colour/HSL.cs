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

        public HSL(float hue, float saturation, float lightness) : this(hue, saturation, lightness, 1f) { }
        public HSL(float hue, float saturation, float lightness, float alpha)
        {
            h = hue;
            s = saturation;
            l = lightness;
            a = alpha;
        }

        public static explicit operator HSL(HSV hsv)
        {
            float l = hsv.v * (1f - hsv.s / 2f);
            float s_l;
            if (l == 0f || l == 1f)
            {
                s_l = 0f;
            }
            else
            {
                s_l = (hsv.v - l) / Mathf.Min(l, 1f - l);
            }

            return new HSL(hsv.h, s_l, l, hsv.a);
        }

        public static explicit operator HSL(Color rgba) => (HSL)(HSV)rgba;
        public static explicit operator Color(HSL hsl) => (Color)(HSV)hsl;

        public override string ToString() => ToString("n3");
        /// <summary>
        /// Applies <paramref name="format"/> to each component.
        /// </summary>
        public string ToString(string format) => $"HSL({h.ToString(format)}, {s.ToString(format)}, {l.ToString(format)}, {a.ToString(format)})";
    }
}
