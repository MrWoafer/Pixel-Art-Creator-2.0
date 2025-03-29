using System;

using PAC.DataStructures;
using PAC.Extensions;

using UnityEngine;

namespace PAC.Patterns.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IPattern2D{T}"/>.
    /// </summary>
    public static class IPattern2DExtensions
    {
        /// <summary>
        /// Turns the section of the pattern in the given rect into a Unity <see cref="Texture2D"/>. 
        /// </summary>
        /// <remarks>
        /// Uses <see cref="Texture2D.SetPixels(Color[])"/>, so won't be as fast as <see cref="ToTexture(IPattern2D{Color32}, IntRect)"/> which uses <see cref="Texture2D.SetPixels32(Color32[])"/>.
        /// </remarks>
        public static Texture2D ToTexture(this IPattern2D<Color> pattern, IntRect textureRect)
        {
            if (pattern is null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            Texture2D texture = new Texture2D(textureRect.width, textureRect.height);
            Color[] pixels = new Color[textureRect.width * textureRect.height];

            foreach ((IntVector2 coord, int index) in textureRect.Enumerate())
            {
                pixels[index] = pattern[coord];
            }

            texture.SetPixels(pixels);
            return texture.Applied();
        }
        /// <summary>
        /// Turns the section of the pattern in the given rect into a Unity <see cref="Texture2D"/>. 
        /// </summary>
        /// <remarks>
        /// Uses <see cref="Texture2D.SetPixels32(Color32[])"/>, so should be faster than <see cref="ToTexture(IPattern2D{Color}, IntRect)"/> which uses <see cref="Texture2D.SetPixels(Color[])"/>.
        /// </remarks>
        public static Texture2D ToTexture(this IPattern2D<Color32> pattern, IntRect textureRect)
        {
            if (pattern is null)
            {
                throw new ArgumentNullException(nameof(pattern));
            }

            Texture2D texture = new Texture2D(textureRect.width, textureRect.height);
            Color32[] pixels = new Color32[textureRect.width * textureRect.height];

            foreach ((IntVector2 coord, int index) in textureRect.Enumerate())
            {
                pixels[index] = pattern[coord];
            }

            texture.SetPixels32(pixels);
            return texture.Applied();
        }
    }
}
