using System;

using PAC.Colour;
using PAC.DataStructures;
using PAC.Extensions;
using PAC.Patterns;
using PAC.Patterns.Extensions;

using UnityEngine;

namespace PAC.ImageEditing
{
    /// <summary>
    /// Provides methods to create <see cref="Texture2D"/>s.
    /// </summary>
    public static class Texture2DCreator
    {
        /// <summary>
        /// Checks that <paramref name="width"/> and <paramref name="height"/> are both &gt; 0, and throws an <see cref="ArgumentOutOfRangeException"/> otherwise.
        /// </summary>
        private static void AssertValidTextureDimensions(int width, int height, string widthParamName, string heightParamName)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException($"{widthParamName} is non-positive: {width}.", widthParamName);
            }
            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException($"{heightParamName} is non-positive: {height}.", heightParamName);
            }
        }

        /// <summary>
        /// Creates a transparent (alpha 0) <see cref="Texture2D"/> of size <paramref name="width"/> x <paramref name="height"/>.
        /// </summary>
        /// <remarks>
        /// Calls <see cref="Texture2D.Apply()"/> on the returned <see cref="Texture2D"/>.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="width"/> or <paramref name="height"/> is &lt;= 0.</exception>
        public static Texture2D Transparent(int width, int height) => Solid(width, height, Config.Colours.transparent);

        /// <summary>
        /// Creates a <see cref="Texture2D"/> of size <paramref name="width"/> x <paramref name="height"/>, filled with the given colour.
        /// </summary>
        /// <remarks>
        /// <para>
        /// <see cref="Solid(int, int, Color32)"/> is faster.
        /// </para>
        /// <para>
        /// Calls <see cref="Texture2D.Apply()"/> on the returned <see cref="Texture2D"/>.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="width"/> or <paramref name="height"/> is &lt;= 0.</exception>
        public static Texture2D Solid(int width, int height, Color colour)
        {
            AssertValidTextureDimensions(width, height, nameof(width), nameof(height));

            Texture2D texture = new Texture2D(width, height);

            Color[] pixels = new Color[width * height];
            for (int index = 0; index < pixels.Length; index++)
            {
                pixels[index] = colour;
            }

            texture.SetPixels(pixels);
            return texture.Applied();
        }
        /// <summary>
        /// Creates a <see cref="Texture2D"/> of size <paramref name="width"/> x <paramref name="height"/>, filled with the given colour.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This is faster than <see cref="Solid(int, int, Color)"/>.
        /// </para>
        /// <para>
        /// Calls <see cref="Texture2D.Apply()"/> on the returned <see cref="Texture2D"/>.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="width"/> or <paramref name="height"/> is &lt;= 0.</exception>
        public static Texture2D Solid(int width, int height, Color32 colour)
        {
            AssertValidTextureDimensions(width, height, nameof(width), nameof(height));

            Texture2D texture = new Texture2D(width, height);

            Color32[] pixels = new Color32[width * height];
            for (int index = 0; index < pixels.Length; index++)
            {
                pixels[index] = colour;
            }

            texture.SetPixels32(pixels);
            return texture.Applied();
        }

        /// <summary>
        /// Creates a checkerboard <see cref="Texture2D"/> to act as the background for transparent pixels for a <paramref name="width"/> x <paramref name="height"/> image.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The checkerboard texture will have dimensions 2 * <paramref name="width"/> x 2 * <paramref name="height"/>.
        /// </para>
        /// <para>
        /// Calls <see cref="Texture2D.Apply()"/> on the returned <see cref="Texture2D"/>.
        /// </para>
        /// </remarks>
        public static Texture2D TransparentCheckerboardBackground(int width, int height)
        {
            AssertValidTextureDimensions(width, height, nameof(width), nameof(height));

            IntRect textureRect = new IntRect((0, 0), (width * 2 - 1, height * 2 - 1));
            return new Checkerboard<Color32>(Preferences.transparentCheckerboardColour1, Preferences.transparentCheckerboardColour2).ToTexture(textureRect);
        }

        /// <summary>
        /// Creates a <see cref="Texture2D"/> of size <paramref name="width"/> x <paramref name="height"/>, where the colour of a pixel is determined in HSL as follows:
        /// <list type="bullet">
        /// <item>
        /// <b>Hue</b>: linearly interpolated based on the x coord, such that the left-most pixels have hue 0 and the texture goes through all hues exactly once.
        /// </item>
        /// <item>
        /// <b>Saturation</b>: linearly interpolated based on the y coord, such that the top-most pixels have saturation 1 and the bottom-most pixels have saturation 0.
        /// </item>
        /// <item>
        /// <b>Lightness</b>: 0.5.
        /// </item>
        /// </list>
        /// </summary>
        /// <remarks>
        /// Calls <see cref="Texture2D.Apply()"/> on the returned <see cref="Texture2D"/>.
        /// </remarks>
        public static Texture2D HSLHueSaturationGrid(int width, int height)
        {
            AssertValidTextureDimensions(width, height, nameof(width), nameof(height));

            Texture2D texture = new Texture2D(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color colour = (Color)new HSLA(x / (float)width, y / (float)(height - 1), 0.5f, 1f);
                    texture.SetPixel(x, y, colour);
                }
            }

            return texture.Applied();
        }
    }
}