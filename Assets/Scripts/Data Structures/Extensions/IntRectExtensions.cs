using UnityEngine;

namespace PAC.DataStructures.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IntRect"/>.
    /// </summary>
    public static class IntRectExtensions
    {
        /// <summary>
        /// Returns an <see cref="IntRect"/> containing precisely the coordinates within the bounds of the <see cref="Texture2D"/>.
        /// </summary>
        public static IntRect GetRect(this Texture2D texture) => new IntRect((0, 0), (texture.width - 1, texture.height - 1));
    }
}
