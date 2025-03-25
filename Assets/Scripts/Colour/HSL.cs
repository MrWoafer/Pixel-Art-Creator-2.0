using System;

using PAC.Extensions;

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
        /// <summary>
        /// Returns an <see cref="HSL"/> with the same HSL values and with the given alpha.
        /// </summary>
        public HSLA WithAlpha(float alpha) => new HSLA(h, s, l, alpha);

        /// <summary>
        /// Converts from <see cref="HSL"/> to <see cref="RGB"/>.
        /// </summary>
        /// <remarks>
        /// This is independent of colour space.
        /// </remarks>
        public static explicit operator RGB(HSL hsl) => (RGB)(HSV)hsl;

        /// <summary>
        /// Converts from <see cref="HSL"/> to <see cref="HSV"/>.
        /// </summary>
        /// <remarks>
        /// This is independent of colour space.
        /// </remarks>
        public static explicit operator HSV(HSL hsl)
        {
            float v = hsl.l + hsl.s * Mathf.Min(hsl.l, 1f - hsl.l);
            float s = v switch
            {
                0f => 0f,
                _ => 2f * (1f - hsl.l / v),
            };
            return new HSV(hsl.h, s, v);
        }
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

    /// <summary>
    /// Extension methods for <see cref="HSL"/>.
    /// </summary>
    public static class HSLExtensions
    {
        /// <summary>
        /// Generates a random <see cref="HSL"/> by independently generating a uniformly random value in <c>[0, 1)</c> for each component.
        /// </summary>
        public static HSL NextHSL(this System.Random random) => new HSL(random.NextFloat(), random.NextFloat(), random.NextFloat());
    }
}
