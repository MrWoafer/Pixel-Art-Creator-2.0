using PAC.Extensions;

using UnityEngine;

namespace PAC.DataStructures.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IntVector2"/>.
    /// </summary>
    public static class IntVector2Extensions
    {
        /// <summary>
        /// Returns whether the given coordinates are within the bounds of the <see cref="Texture2D"/>.
        /// </summary>
        public static bool ContainsPixel(this Texture2D texture, IntVector2 pixel) => texture.ContainsPixel(pixel.x, pixel.y);

        /// <summary>
        /// Sets the pixel colour at the given coordinates.
        /// </summary>
        public static void SetPixel(this Texture2D texture, IntVector2 pixel, Color colour) => texture.SetPixel(pixel.x, pixel.y, colour);

        /// <summary>
        /// Gets the pixel colour at the given coordinates.
        /// </summary>
        public static Color GetPixel(this Texture2D texture, IntVector2 pixel) => texture.GetPixel(pixel.x, pixel.y);
    }
}
