using PAC.Extensions;

using UnityEngine;

namespace PAC.Colour
{
    public static class ColorExtensions
    {
        /// <summary>
        /// Inverts the colour, i.e. does 1 - value for each component.
        /// </summary>
        /// <param name="invertAlpha">Whether to invert the alpha value as well.</param>
        public static Color Invert(this Color colour, bool invertAlpha = false)
        {
            return new Color(1f - colour.r, 1f - colour.g, 1f - colour.b, invertAlpha ? 1f - colour.a : colour.a);
        }

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
        public static Color NextColor(this System.Random random) =>new Color(random.NextFloat(), random.NextFloat(), random.NextFloat(), random.NextFloat());
    }
}
