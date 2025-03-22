using UnityEngine;

namespace PAC.Colour
{
    /// <summary>
    /// A colour in HSL (Hue, Saturation, Lightness) form with no alpha component.
    /// </summary>
    public readonly struct HSL
    {
        #region Fields
        /// <summary>
        /// Hue.
        /// </summary>
        public readonly float h { get; init; }
        /// <summary>
        /// Saturation.
        /// </summary>
        public readonly float s { get; init; }
        /// <summary>
        /// Lightness.
        /// </summary>
        public readonly float l { get; init; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an <see cref="HSL"/> with the given components without clamping.
        /// </summary>
        /// <param name="hue">See <see cref="h"/>.</param>
        /// <param name="saturation">See <see cref="s"/>.</param>
        /// <param name="lightness">See <see cref="l"/>.</param>
        public HSL(float hue, float saturation, float lightness)
        {
            h = hue;
            s = saturation;
            l = lightness;
        }
        #endregion

        #region Conversion
        public static explicit operator HSL(HSV hsv)
        {
            float l = hsv.v * (1f - hsv.s / 2f);
            float s = l switch
            {
                0f or 1f => 0f,
                _ => (hsv.v - l) / Mathf.Min(l, 1f - l)
            };
            return new HSL(hsv.h, s, l);
        }

        public static explicit operator HSL(RGB rgb) => (HSL)(HSV)rgb;
        public static explicit operator RGB(HSL hsl) => (RGB)(HSV)hsl;

        /// <summary>
        /// Returns an <see cref="HSL"/> with the same HSL values and with the given alpha.
        /// </summary>
        public HSLA WithAlpha(float alpha) => new HSLA(h, s, l, alpha);
        #endregion

        public override string ToString() => ToString("n3");
        /// <summary>
        /// Applies <paramref name="format"/> to each component.
        /// </summary>
        public string ToString(string format) => $"{nameof(HSL)}({h.ToString(format)}, {s.ToString(format)}, {l.ToString(format)})";
    }
}
