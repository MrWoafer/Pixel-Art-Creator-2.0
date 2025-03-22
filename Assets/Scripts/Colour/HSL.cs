using System;

using UnityEngine;

namespace PAC.Colour
{
    /// <summary>
    /// A colour in HSL (Hue, Saturation, Lightness) form with no alpha component.
    /// </summary>
    public readonly struct HSL : IEquatable<HSL>
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
        /// Lightness.
        /// </summary>
        /// <remarks>
        /// This is intended to be in the inclusive range <c>[0, 1]</c>, but can be outside.
        /// </remarks>
        public readonly float l { get; init; }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an <see cref="HSL"/> with the given components without clamping or wrapping.
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

        #region Comparison
        /// <summary>
        /// Component-wise equality.
        /// </summary>
        public static bool operator ==(HSL x, HSL y) => x.h == y.h && x.s == y.s && x.l == y.l;
        /// <summary>
        /// See <see cref="operator ==(HSL, HSL)"/>.
        /// </summary>
        public static bool operator !=(HSL x, HSL y) => !(x == y);
        /// <summary>
        /// See <see cref="operator ==(HSL, HSL)"/>.
        /// </summary>
        public bool Equals(HSL other) => this == other;
        /// <summary>
        /// See <see cref="Equals(HSL)"/>.
        /// </summary>
        public override bool Equals(object obj) => obj is HSL other && Equals(other);
        /// <summary>
        /// Returns whether each component of <paramref name="other"/> differs from the corresponding component of <see langword="this"/> by &lt;= <paramref name="tolerance"/>.
        /// </summary>
        public bool Equals(HSL other, float tolerance)
            => Mathf.Abs(h - other.h) <= tolerance
            && Mathf.Abs(s - other.s) <= tolerance
            && Mathf.Abs(l - other.l) <= tolerance;

        public override int GetHashCode() => HashCode.Combine(h, s, l);
        #endregion

        public override string ToString() => ToString("n3");
        /// <summary>
        /// Applies <paramref name="format"/> to each component.
        /// </summary>
        public string ToString(string format) => $"{nameof(HSL)}({h.ToString(format)}, {s.ToString(format)}, {l.ToString(format)})";
    }
}
