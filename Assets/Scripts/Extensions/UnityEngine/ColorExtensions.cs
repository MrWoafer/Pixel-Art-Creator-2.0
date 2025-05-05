using PAC.Extensions.System;

using UnityEngine;

namespace PAC.Extensions.UnityEngine
{
    /// <summary>
    /// Extension methods for <see cref="Color"/>.
    /// </summary>
    public static class ColorExtensions
    {
        /// <summary>
        /// Returns whether each component of <paramref name="colour"/> differs from the corresponding component of <paramref name="otherColour"/> by &lt;= <paramref name="tolerance"/>.
        /// </summary>
        public static bool Equals(this Color colour, Color otherColour, float tolerance)
            => Mathf.Abs(colour.r - otherColour.r) <= tolerance
            && Mathf.Abs(colour.g - otherColour.g) <= tolerance
            && Mathf.Abs(colour.b - otherColour.b) <= tolerance
            && Mathf.Abs(colour.a - otherColour.a) <= tolerance;

        /// <summary>
        /// Returns the <see cref="Color"/> with its RGB values multiplied by its alpha.
        /// </summary>
        /// <seealso cref="Straight(Color)"/>
        public static Color Premultiplied(this Color colour) => new Color(colour.r * colour.a, colour.g * colour.a, colour.b * colour.a, colour.a);
        /// <summary>
        /// Returns the <see cref="Color"/> with its RGB values divided by its alpha, and its alpha unchanged.
        /// </summary>
        /// <returns>
        /// <list type="bullet">
        /// <item>
        /// If <paramref name="colour"/> has non-zero alpha: <paramref name="colour"/> with its RGB values divided by its alpha, and its alpha unchanged
        /// </item>
        /// <item>
        /// If <paramref name="colour"/> has zero alpha: <c>(0, 0, 0, 0)</c>
        /// </item>
        /// </list>
        /// </returns>
        /// <seealso cref="Premultiplied(Color)"/>
        public static Color Straight(this Color colour)
            => colour.a == 0f
            ? new Color(0f, 0f, 0f, 0f)
            : new Color(colour.r / colour.a, colour.g / colour.a, colour.b / colour.a, colour.a);

        /// <summary>
        /// Generates a random <see cref="Color"/> by independently generating a uniformly random value in <c>[0, 1)</c> for each component.
        /// </summary>
        /// <remarks>
        /// Note that this will always be a valid colour in straight alpha form, but not necessarily in premultiplied alpha form.
        /// </remarks>
        public static Color NextColor(this global::System.Random random) => new Color(random.NextFloat(), random.NextFloat(), random.NextFloat(), random.NextFloat());
    }
}
