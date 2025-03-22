using UnityEngine;

namespace PAC.Colour
{
    /// <summary>
    /// Represents a colour in HSL form.
    /// </summary>
    public readonly struct HSL
    {
        /// <summary>Hue.</summary>
        public readonly float h { get; init; }
        /// <summary>Saturation.</summary>
        public readonly float s { get; init; }
        /// <summary>Lightness.</summary>
        public readonly float l { get; init; }
        /// <summary>Alpha.</summary>
        public readonly float a { get; init; }

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
            float s = l switch
            {
                0f or 1f => 0f,
                _ => (hsv.v - l) / Mathf.Min(l, 1f - l)
            };
            return new HSL(hsv.h, s, l, hsv.a);
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
