using System;

using PAC.Extensions;

using UnityEngine;

namespace PAC.Colour
{
    /// <summary>
    /// A colour in HSV (Hue, Saturation, Value) form with an alpha component.
    /// </summary>
    public readonly struct HSVA : IEquatable<HSVA>
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

        /// <summary>
        /// Converts from <see cref="Color"/> to <see cref="HSVA"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Does not do any clamping.
        /// </para>
        /// <para>
        /// This is independent of colour space.
        /// </para>
        /// </remarks>
        public static explicit operator HSVA(Color rgba) => ((HSV)(RGB)rgba).WithAlpha(rgba.a);
        /// <summary>
        /// Converts from <see cref="HSVA"/> to <see cref="Color"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Does not do any clamping.
        /// </para>
        /// <para>
        /// This is independent of colour space.
        /// </para>
        /// </remarks>
        public static explicit operator Color(HSVA hsva) => ((RGB)(HSV)hsva).WithAlpha(hsva.a);

        /// <summary>
        /// Converts from <see cref="HSVA"/> to <see cref="HSLA"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Does not do any clamping.
        /// </para>
        /// <para>
        /// This is independent of colour space.
        /// </para>
        /// </remarks>
        public static explicit operator HSLA(HSVA hsva) => ((HSL)(HSV)hsva).WithAlpha(hsva.a);
        #endregion

        #region Comparison
        /// <summary>
        /// Component-wise equality.
        /// </summary>
        public static bool operator ==(HSVA x, HSVA y) => x.h == y.h && x.s == y.s && x.v == y.v && x.a == y.a;
        /// <summary>
        /// See <see cref="operator ==(HSVA, HSVA)"/>.
        /// </summary>
        public static bool operator !=(HSVA x, HSVA y) => !(x == y);
        /// <summary>
        /// See <see cref="operator ==(HSVA, HSVA)"/>.
        /// </summary>
        public bool Equals(HSVA other) => this == other;
        /// <summary>
        /// See <see cref="Equals(HSVA)"/>.
        /// </summary>
        public override bool Equals(object obj) => obj is HSVA other && Equals(other);
        /// <summary>
        /// Returns whether each component of <paramref name="other"/> differs from the corresponding component of <see langword="this"/> by &lt;= <paramref name="tolerance"/>.
        /// </summary>
        public bool Equals(HSVA other, float tolerance)
            => Mathf.Abs(h - other.h) <= tolerance
            && Mathf.Abs(s - other.s) <= tolerance
            && Mathf.Abs(v - other.v) <= tolerance
            && Mathf.Abs(a - other.a) <= tolerance;

        public override int GetHashCode() => HashCode.Combine(h, s, v, a);
        #endregion

        public override string ToString() => ToString("n3");
        /// <summary>
        /// Applies <paramref name="format"/> to each component.
        /// </summary>
        public string ToString(string format) => $"{nameof(HSVA)}({h.ToString(format)}, {s.ToString(format)}, {v.ToString(format)}, {a.ToString(format)})";
    }

    /// <summary>
    /// Extension methods for <see cref="HSVA"/>.
    /// </summary>
    public static class HSVAExtensions
    {
        /// <summary>
        /// Generates a random <see cref="HSVA"/> by independently generating a uniformly random value in <c>[0, 1)</c> for each component.
        /// </summary>
        public static HSVA NextHSVA(this System.Random random) => new HSVA(random.NextFloat(), random.NextFloat(), random.NextFloat(), random.NextFloat());
    }
}
