using System;
using System.IO;

using UnityEngine;

namespace PAC.Extensions
{
    /// <summary>
    /// Extension methods for Unity's <see cref="Texture2D"/>.
    /// </summary>
    public static class Texture2DExtensions
    {
        /// <summary>
        /// Creates a <see cref="Sprite"/> from the <see cref="Texture2D"/>, using the <see cref="FilterMode.Point"/> filter mode.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This does not deep copy the <see cref="Texture2D"/>. This means that subsequent changes to the <see cref="Texture2D"/> will affect the <see cref="Sprite"/> once
        /// <see cref="Texture2D.Apply()"/> is called.
        /// </para>
        /// <para>
        /// Does not call <see cref="Texture2D.Apply()"/> on the <see cref="Texture2D"/>.
        /// </para>
        /// </remarks>
        public static Sprite ToSprite(this Texture2D texture)
        {
            Sprite sprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                Math.Max(texture.width, texture.height),
                0,
                SpriteMeshType.FullRect
                );
            sprite.texture.filterMode = FilterMode.Point;
            return sprite;
        }

        /// <summary>
        /// Creates a deep copy of the <see cref="Texture2D"/>.
        /// </summary>
        public static Texture2D DeepCopy(this Texture2D texture)
        {
            Texture2D copy = new Texture2D(texture.width, texture.height);
            copy.SetPixels(texture.GetPixels());
            return copy.Applied();
        }

        /// <summary>
        /// Loads a <see cref="Texture2D"/> from the file at the given file path.
        /// </summary>
        /// <remarks>
        /// Does not call <see cref="Texture2D.Apply()"/> on the returned <see cref="Texture2D"/>.
        /// </remarks>
        /// <exception cref="FileNotFoundException">The file path <paramref name="filePath"/> does not exist.</exception>
        public static Texture2D LoadFromFile(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new FileNotFoundException($"File path does not exist: {filePath}", nameof(filePath));
            }

            byte[] fileData = System.IO.File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(fileData);

            return texture;
        }

        /// <summary>
        /// Returns whether the coordinates <c>(<paramref name="x"/>, <paramref name="y"/>)</c> are within the bounds of the <see cref="Texture2D"/>.
        /// </summary>
        public static bool ContainsPixel(this Texture2D texture, int x, int y) => 0 <= x && x < texture.width && 0 <= y && y < texture.height;

        /// <summary>
        /// Calls <see cref="Texture2D.Apply()"/> on the <see cref="Texture2D"/> then returns it.
        /// </summary>
        public static Texture2D Applied(this Texture2D texture)
        {
            texture.Apply();
            return texture;
        }

        /// <summary>
        /// Checks that <paramref name="width"/> and <paramref name="height"/> are both &gt; 0, and throws an <see cref="ArgumentOutOfRangeException"/> otherwise.
        /// </summary>
        public static void AssertValidTextureDimensions(int width, int height, string widthParamName, string heightParamName)
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
        public static Texture2D Transparent(int width, int height) => Solid(width, height, Color.clear);

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
    }
}