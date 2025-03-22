using UnityEngine;

namespace PAC.Colour
{
    /// <summary>
    /// A colour in HSL (Hue, Saturation, Lightness) form with an alpha component.
    /// </summary>
    public readonly struct HSLA
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
        /// <summary>
        /// Alpha.
        /// </summary>
        public readonly float a { get; init; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an <see cref="HSLA"/> with the given components without clamping.
        /// </summary>
        /// <param name="hue">See <see cref="h"/>.</param>
        /// <param name="saturation">See <see cref="s"/>.</param>
        /// <param name="lightness">See <see cref="l"/>.</param>
        /// <param name="alpha">See <see cref="a"/>.</param>
        public HSLA(float hue, float saturation, float lightness, float alpha)
        {
            h = hue;
            s = saturation;
            l = lightness;
            a = alpha;
        }
        #endregion

        #region Conversion
        /// <summary>
        /// Returns an <see cref="HSL"/> with the same HSL values as the <see cref="HSLA"/>, but discarding the alpha.
        /// </summary>
        public static explicit operator HSL(HSLA hsla) => new HSL(hsla.h, hsla.s, hsla.l);

        public static explicit operator HSLA(HSVA hsva) => ((HSL)(HSV)hsva).WithAlpha(hsva.a);

        public static explicit operator HSLA(Color rgba) => ((HSL)(RGB)rgba).WithAlpha(rgba.a);
        public static explicit operator Color(HSLA hsla) => ((RGB)(HSL)hsla).WithAlpha(hsla.a);
        #endregion

        public override string ToString() => ToString("n3");
        /// <summary>
        /// Applies <paramref name="format"/> to each component.
        /// </summary>
        public string ToString(string format) => $"{nameof(HSLA)}({h.ToString(format)}, {s.ToString(format)}, {l.ToString(format)}, {a.ToString(format)})";
    }
}
