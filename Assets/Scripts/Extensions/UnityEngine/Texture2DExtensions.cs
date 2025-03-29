using System;
using System.IO;

using PAC.DataStructures;

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
        /// Returns an <see cref="IntRect"/> containing precisely the coordinates within the bounds of the <see cref="Texture2D"/>.
        /// </summary>
        public static IntRect GetRect(this Texture2D texture) => new IntRect((0, 0), (texture.width - 1, texture.height - 1));

        /// <summary>
        /// Returns whether the coordinates <c>(<paramref name="x"/>, <paramref name="y"/>)</c> are within the bounds of the <see cref="Texture2D"/>.
        /// </summary>
        public static bool ContainsPixel(this Texture2D texture, int x, int y) => 0 <= x && x < texture.width && 0 <= y && y < texture.height;
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

        /// <summary>
        /// Calls <see cref="Texture2D.Apply()"/> on the <see cref="Texture2D"/> then returns it.
        /// </summary>
        public static Texture2D Applied(this Texture2D texture)
        {
            texture.Apply();
            return texture;
        }
    }
}