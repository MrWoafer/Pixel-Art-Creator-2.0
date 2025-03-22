using UnityEngine;

namespace PAC.Colour
{
    /// <summary>
    /// A colour in HSV (Hue, Saturation, Value) form with an alpha component.
    /// </summary>
    public readonly struct HSVA
    {
        #region Fields
        /// <summary>
        /// Hue.
        /// </summary>
        /// <remarks>
        /// This is intended to be in the inclusive-exclusive range <c>[0, 1)</c>, but can be outside.
        /// </remarks>
        public readonly float h { get; init; }
        /// <summary>
        /// Saturation.
        /// </summary>
        /// <remarks>
        /// This is intended to be in the inclusive range <c>[0, 1]</c>, but can be outside.
        /// </remarks>
        public readonly float s { get; init; }
        /// <summary>
        /// Value.
        /// </summary>
        /// <remarks>
        /// This is intended to be in the inclusive range <c>[0, 1]</c>, but can be outside.
        /// </remarks>
        public readonly float v { get; init; }
        /// <summary>
        /// Alpha.
        /// </summary>
        /// <remarks>
        /// This is intended to be in the inclusive range <c>[0, 1]</c>, but can be outside.
        /// </remarks>
        public readonly float a { get; init; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an <see cref="HSV"/> with the given components without clamping or wrapping.
        /// </summary>
        /// <param name="hue">See <see cref="h"/>.</param>
        /// <param name="saturation">See <see cref="s"/>.</param>
        /// <param name="value">See <see cref="v"/>.</param>
        /// <param name="alpha">See <see cref="a"/>.</param>
        public HSVA(float hue, float saturation, float value, float alpha)
        {
            h = hue;
            s = saturation;
            v = value;
            a = alpha;
        }
        #endregion

        #region Conversion
        /// <summary>
        /// Returns an <see cref="HSV"/> with the same HSV values as the <see cref="HSVA"/>, but discarding the alpha.
        /// </summary>
        public static explicit operator HSV(HSVA hsva) => new HSV(hsva.h, hsva.s, hsva.v);

        public static explicit operator HSVA(HSLA hsla) => ((HSV)(HSL)hsla).WithAlpha(hsla.a);

        public static explicit operator HSVA(Color rgba) => ((HSV)(RGB)rgba).WithAlpha(rgba.a);
        public static explicit operator Color(HSVA hsva) => ((RGB)(HSV)hsva).WithAlpha(hsva.a);
        #endregion

        public override string ToString() => ToString("n3");
        /// <summary>
        /// Applies <paramref name="format"/> to each component.
        /// </summary>
        public string ToString(string format) => $"{nameof(HSVA)}({h.ToString(format)}, {s.ToString(format)}, {v.ToString(format)}, {a.ToString(format)})";
    }
}
