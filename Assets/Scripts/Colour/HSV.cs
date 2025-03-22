using System;

using UnityEngine;

namespace PAC.Colour
{
    /// <summary>
    /// A colour in HSV (Hue, Saturation, Value) form with no alpha component.
    /// </summary>
    public readonly struct HSV : IEquatable<HSV>
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
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an <see cref="HSV"/> with the given components without clamping or wrapping.
        /// </summary>
        /// <param name="hue">See <see cref="h"/>.</param>
        /// <param name="saturation">See <see cref="s"/>.</param>
        /// <param name="value">See <see cref="v"/>.</param>
        public HSV(float hue, float saturation, float value)
        {
            h = hue;
            s = saturation;
            v = value;
        }
        #endregion

        #region Conversion
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

        public static explicit operator HSV(RGB rgb)
        {
            Color.RGBToHSV(rgb.WithAlpha(1f), out float h, out float s, out float v);
            return new HSV(h, s, v);
        }
        public static explicit operator RGB(HSV hsv) => (RGB)Color.HSVToRGB(hsv.h, hsv.s, hsv.v);

        /// <summary>
        /// Returns an <see cref="HSVA"/> with the same HSV values and with the given alpha.
        /// </summary>
        public HSVA WithAlpha(float alpha) => new HSVA(h, s, v, alpha);
        #endregion

        #region Comparison
        /// <summary>
        /// Component-wise equality.
        /// </summary>
        public static bool operator ==(HSV x, HSV y) => x.h == y.h && x.s == y.s && x.v == y.v;
        /// <summary>
        /// See <see cref="operator ==(HSV, HSV)"/>.
        /// </summary>
        public static bool operator !=(HSV x, HSV y) => !(x == y);
        /// <summary>
        /// See <see cref="operator ==(HSV, HSV)"/>.
        /// </summary>
        public bool Equals(HSV other) => this == other;
        /// <summary>
        /// See <see cref="Equals(HSV)"/>.
        /// </summary>
        public override bool Equals(object obj) => obj is HSV other && Equals(other);
        /// <summary>
        /// Returns whether each component of <paramref name="other"/> differs from the corresponding component of <see langword="this"/> by &lt;= <paramref name="tolerance"/>.
        /// </summary>
        public bool Equals(HSV other, float tolerance)
            => Mathf.Abs(h - other.h) <= tolerance
            && Mathf.Abs(s - other.s) <= tolerance
            && Mathf.Abs(v - other.v) <= tolerance;

        public override int GetHashCode() => HashCode.Combine(h, s, v);
        #endregion

        public override string ToString() => ToString("n3");
        /// <summary>
        /// Applies <paramref name="format"/> to each component.
        /// </summary>
        public string ToString(string format) => $"{nameof(HSV)}({h.ToString(format)}, {s.ToString(format)}, {v.ToString(format)})";
    }
}
