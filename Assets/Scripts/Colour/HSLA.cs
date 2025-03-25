using System;

using PAC.Extensions;

using UnityEngine;

namespace PAC.Colour
{
    /// <summary>
    /// A colour in HSL (Hue, Saturation, Lightness) form with an alpha component.
    /// </summary>
    public readonly struct HSLA : IEquatable<HSLA>
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
        /// Creates an <see cref="HSLA"/> with the given components without clamping or wrapping.
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

        /// <summary>
        /// Converts from <see cref="Color"/> to <see cref="HSLA"/>.
        /// </summary>
        /// <remarks>
        /// This is independent of colour space.
        /// </remarks>
        public static explicit operator HSLA(Color rgba) => ((HSL)(RGB)rgba).WithAlpha(rgba.a);
        /// <summary>
        /// Converts from <see cref="HSLA"/> to <see cref="Color"/>.
        /// </summary>
        /// <remarks>
        /// This is independent of colour space.
        /// </remarks>
        public static explicit operator Color(HSLA hsla) => ((RGB)(HSL)hsla).WithAlpha(hsla.a);

        /// <summary>
        /// Converts from <see cref="HSLA"/> to <see cref="HSVA"/>.
        /// </summary>
        /// <remarks>
        /// This is independent of colour space.
        /// </remarks>
        public static explicit operator HSVA(HSLA hsla) => ((HSV)(HSL)hsla).WithAlpha(hsla.a);
        #endregion

        #region Comparison
        /// <summary>
        /// Component-wise equality.
        /// </summary>
        public static bool operator ==(HSLA x, HSLA y) => x.h == y.h && x.s == y.s && x.l == y.l && x.a == y.a;
        /// <summary>
        /// See <see cref="operator ==(HSLA, HSLA)"/>.
        /// </summary>
        public static bool operator !=(HSLA x, HSLA y) => !(x == y);
        /// <summary>
        /// See <see cref="operator ==(HSLA, HSLA)"/>.
        /// </summary>
        public bool Equals(HSLA other) => this == other;
        /// <summary>
        /// See <see cref="Equals(HSLA)"/>.
        /// </summary>
        public override bool Equals(object obj) => obj is HSLA other && Equals(other);
        /// <summary>
        /// Returns whether each component of <paramref name="other"/> differs from the corresponding component of <see langword="this"/> by &lt;= <paramref name="tolerance"/>.
        /// </summary>
        public bool Equals(HSLA other, float tolerance)
            => Mathf.Abs(h - other.h) <= tolerance
            && Mathf.Abs(s - other.s) <= tolerance
            && Mathf.Abs(l - other.l) <= tolerance
            && Mathf.Abs(a - other.a) <= tolerance;

        public override int GetHashCode() => HashCode.Combine(h, s, l, a);
        #endregion

        public override string ToString() => ToString("n3");
        /// <summary>
        /// Applies <paramref name="format"/> to each component.
        /// </summary>
        public string ToString(string format) => $"{nameof(HSLA)}({h.ToString(format)}, {s.ToString(format)}, {l.ToString(format)}, {a.ToString(format)})";
    }

    /// <summary>
    /// Extension methods for <see cref="HSLA"/>.
    /// </summary>
    public static class HSLAExtensions
    {
        /// <summary>
        /// Generates a random <see cref="HSLA"/> by independently generating a uniformly random value in <c>[0, 1)</c> for each component.
        /// </summary>
        public static HSLA NextHSLA(this System.Random random) => new HSLA(random.NextFloat(), random.NextFloat(), random.NextFloat(), random.NextFloat());
    }
}
