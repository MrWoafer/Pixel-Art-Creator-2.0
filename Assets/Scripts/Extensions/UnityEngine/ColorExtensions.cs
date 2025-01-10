using UnityEngine;

namespace PAC.Extensions
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
    }
}
