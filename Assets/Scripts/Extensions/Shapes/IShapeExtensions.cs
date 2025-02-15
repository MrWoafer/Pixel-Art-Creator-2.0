using PAC.DataStructures;
using PAC.Shapes.Interfaces;

using UnityEngine;

namespace PAC.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IShape"/>.
    /// </summary>
    public static class IShapeExtensions
    {
        /// <summary>
        /// Turns the pixels in the shape's bounding rect into a Unity <see cref="Texture2D"/> with the given colour.
        /// </summary>
        public static Texture2D ToTexture(this IShape shape, Color colour) => shape.ToTexture(colour, shape.boundingRect);
        /// <summary>
        /// Turns the pixels in the given rect into a Unity <see cref="Texture2D"/> with the given colour, using any of the shape's points that lie within that rect. 
        /// </summary>
        public static Texture2D ToTexture(this IShape shape, Color colour, IntRect textureRect)
        {
            Texture2D texture = Texture2DExtensions.BlankTexture(textureRect.width, textureRect.height);

            foreach (IntVector2 pixel in shape)
            {
                if (textureRect.Contains(pixel))
                {
                    texture.SetPixel(pixel - textureRect.bottomLeft, colour);
                }
            }

            texture.Apply();
            return texture;
        }
    }
}