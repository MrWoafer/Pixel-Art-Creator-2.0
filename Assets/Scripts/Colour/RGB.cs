using System;

using PAC.Extensions;

using UnityEngine;

namespace PAC.Colour
{
    /// <summary>
    /// A colour in RGB (red, green, blue) format with no alpha channel.
    /// </summary>
    /// <remarks>
    /// The RGB values are in the inclusive range <c>[0, 1]</c>.
    /// </remarks>
    public readonly struct RGB : IEquatable<RGB>
    {
        #region Fields
        /// <summary>
        /// The red channel of the colour.
        /// </summary>
        /// <remarks>
        /// This is intended to be in the inclusive range <c>[0, 1]</c>, but can be outside.
        /// </remarks>
        public readonly float r;
        /// <summary>
        /// The green channel of the colour.
        /// </summary>
        /// <remarks>
        /// This is intended to be in the inclusive range <c>[0, 1]</c>, but can be outside.
        /// </remarks>
        public readonly float g;
        /// <summary>
        /// The blue channel of the colour.
        /// </summary>
        /// <remarks>
        /// This is intended to be in the inclusive range <c>[0, 1]</c>, but can be outside.
        /// </remarks>
        public readonly float b;
        #endregion

        #region Predefined Instances
        /// <summary>
        /// The colour <c>(1, 0, 0)</c>.
        /// </summary>
        public static readonly RGB Red = new RGB(1f, 0f, 0f);
        /// <summary>
        /// The colour <c>(0, 1, 0)</c>.
        /// </summary>
        public static readonly RGB Green = new RGB(0f, 1f, 0f);
        /// <summary>
        /// The colour <c>(0, 0, 1)</c>.
        /// </summary>
        public static readonly RGB Blue = new RGB(0f, 0f, 1f);
        /// <summary>
        /// The colour <c>(1, 1, 1)</c>.
        /// </summary>
        public static readonly RGB White = new RGB(1f, 1f, 1f);
        /// <summary>
        /// The colour <c>(0, 0, 0)</c>.
        /// </summary>
        public static readonly RGB Black = new RGB(0f, 0f, 0f);
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an <see cref="RGB"/> with the given channels without clamping.
        /// </summary>
        /// <param name="r">See <see cref="r"/>.</param>
        /// <param name="g">See <see cref="g"/>.</param>
        /// <param name="b">See <see cref="b"/>.</param>
        public RGB(float r, float g, float b)
        {
            this.r = r;
            this.g = g;
            this.b = b;
        }
        #endregion

        #region Conversion
        /// <summary>
        /// Returns an <see cref="RGB"/> with the same RGB values as the <see cref="Color"/>, but discarding the alpha.
        /// </summary>
        /// <param name="rgba"></param>
        public static explicit operator RGB(Color rgba) => new RGB(rgba.r, rgba.g, rgba.b);

        /// <summary>
        /// Returns a <see cref="Color"/> with the same RGB values and with the given alpha.
        /// </summary>
        public Color WithAlpha(float alpha) => new Color(r, g, b, alpha);
        #endregion

        #region Comparison
        /// <summary>
        /// Component-wise equality.
        /// </summary>
        public static bool operator ==(RGB x, RGB y) => x.r == y.r && x.g == y.g && x.b == y.b;
        /// <summary>
        /// See <see cref="operator ==(RGB, RGB)"/>.
        /// </summary>
        public static bool operator !=(RGB x, RGB y) => !(x == y);
        /// <summary>
        /// See <see cref="operator ==(RGB, RGB)"/>.
        /// </summary>
        public bool Equals(RGB other) => this == other;
        /// <summary>
        /// See <see cref="Equals(RGB)"/>.
        /// </summary>
        public override bool Equals(object obj) => obj is RGB other && Equals(other);
        /// <summary>
        /// Returns whether each component of <paramref name="other"/> differs from the corresponding component of <see langword="this"/> by &lt;= <paramref name="tolerance"/>.
        /// </summary>
        public bool Equals(RGB other, float tolerance)
            => Mathf.Abs(r - other.r) <= tolerance
            && Mathf.Abs(g - other.g) <= tolerance
            && Mathf.Abs(b - other.b) <= tolerance;

        public override int GetHashCode() => HashCode.Combine(r, g, b);
        #endregion

        #region Operations
        /// <summary>
        /// Adds component-wise, with no clamping.
        /// </summary>
        public static RGB operator +(RGB x, RGB y) => new RGB(x.r + y.r, x.g + y.g, x.b + y.b);
        /// <summary>
        /// Subtracts component-wise, with no clamping.
        /// </summary>
        public static RGB operator -(RGB x, RGB y) => new RGB(x.r - y.r, x.g - y.g, x.b - y.b);
        /// <summary>
        /// Multiplies component-wise, with no clamping.
        /// </summary>
        public static RGB operator *(RGB x, RGB y) => new RGB(x.r * y.r, x.g * y.g, x.b * y.b);
        /// <summary>
        /// Divides component-wise, with no clamping.
        /// </summary>
        public static RGB operator /(RGB x, RGB y) => new RGB(x.r / y.r, x.g / y.g, x.b / y.b);

        /// <summary>
        /// Multiplies each component of <paramref name="rgb"/> by <paramref name="scalar"/> with no clamping.
        /// </summary>
        public static RGB operator *(RGB rgb, float scalar) => new RGB(rgb.r * scalar, rgb.g * scalar, rgb.b * scalar);
        /// <summary>
        /// Multiplies each component of <paramref name="rgb"/> by <paramref name="scalar"/> with no clamping.
        /// </summary>
        public static RGB operator *(float scalar, RGB rgb) => rgb * scalar;
        /// <summary>
        /// Divides each component of <paramref name="rgb"/> by <paramref name="scalar"/> with no clamping.
        /// </summary>
        public static RGB operator /(RGB rgb, float scalar) => new RGB(rgb.r / scalar, rgb.g / scalar, rgb.b / scalar);

        /// <summary>
        /// Linearly interpolates component-wise from <paramref name="start"/> (<paramref name="t"/> = 0) to <paramref name="end"/> (<paramref name="t"/> = 1), clamping <paramref name="t"/> to
        /// the inclusive range <c>[0, 1]</c>.
        /// </summary>
        /// <seealso cref="LerpUnclamped(RGB, RGB, float)"/>
        public static RGB LerpClamped(RGB start, RGB end, float t) => LerpUnclamped(start, end, Mathf.Clamp01(t));
        /// <summary>
        /// Linearly interpolates component-wise from <paramref name="start"/> (<paramref name="t"/> = 0) to <paramref name="end"/> (<paramref name="t"/> = 1), without clamping
        /// <paramref name="t"/>.
        /// </summary>
        /// <remarks>
        /// Values of <paramref name="t"/> outside the range <c>[0, 1]</c> will give outputs beyond <paramref name="start"/> or <paramref name="end"/>.
        /// </remarks>
        /// /// <seealso cref="LerpClamped(RGB, RGB, float)"/>
        public static RGB LerpUnclamped(RGB start, RGB end, float t) => (1f - t) * start + t * end;

        /// <summary>
        /// Returns the <see cref="RGB"/> with each component clamped to the inclusive range <c>[0, 1]</c>.
        /// </summary>
        /// <returns></returns>
        public RGB Clamp01() => new RGB(Mathf.Clamp01(r), Mathf.Clamp01(g), Mathf.Clamp01(b));
        #endregion

        public override string ToString() => $"{nameof(RGB)}({r}, {g}, {b})";
    }

    /// <summary>
    /// Extension methods for <see cref="RGB"/>.
    /// </summary>
    public static class RGBExtensions
    {
        /// <summary>
        /// Generates a random <see cref="RGB"/> by independently generating a uniformly random value in <c>[0, 1)</c> for each component.
        /// </summary>
        public static RGB NextRGB(this System.Random random) => new RGB(random.NextFloat(), random.NextFloat(), random.NextFloat());
    }
}