using System;
using System.IO;

using PAC.Colour.Compositing;
using PAC.DataStructures;
using PAC.Exceptions;
using PAC.Geometry;
using PAC.Geometry.Axes;

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
        /// Returns a deep copy of the <see cref="Texture2D"/> reflected across the given axis.
        /// </summary>
        /// <remarks>
        /// Calls <see cref="Texture2D.Apply()"/> on the returned <see cref="Texture2D"/>.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="axis"/> is null.</exception>
        public static Texture2D Flip(this Texture2D texture, CardinalAxis axis) => axis switch
        {
            null => throw new ArgumentNullException(nameof(axis), $"{nameof(axis)} is null."),
            VerticalAxis => texture.FlipX(),
            HorizontalAxis => texture.FlipY(),
            _ => throw new UnreachableException()
        };
        /// <summary>
        /// Returns a deep copy of the <see cref="Texture2D"/> reflected across the central vertical axis.
        /// </summary>
        /// <remarks>
        /// Calls <see cref="Texture2D.Apply()"/> on the returned <see cref="Texture2D"/>.
        /// </remarks>
        private static Texture2D FlipX(this Texture2D texture)
        {
            Texture2D flipped = new Texture2D(texture.width, texture.height);

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    flipped.SetPixel(x, y, texture.GetPixel(texture.width - 1 - x, y));
                }
            }

            return flipped.Applied();
        }
        /// <summary>
        /// Returns a deep copy of the <see cref="Texture2D"/> reflected across the central horizontal axis.
        /// </summary>
        /// <remarks>
        /// Calls <see cref="Texture2D.Apply()"/> on the returned <see cref="Texture2D"/>.
        /// </remarks>
        private static Texture2D FlipY(this Texture2D texture)
        {
            Texture2D flipped = new Texture2D(texture.width, texture.height);

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    flipped.SetPixel(x, y, texture.GetPixel(x, texture.height - 1 - y));
                }
            }

            return flipped.Applied();
        }

        /// <summary>
        /// Returns a deep copy of the <see cref="Texture2D"/> rotated by the given angle.
        /// </summary>
        /// <remarks>
        /// Calls <see cref="Texture2D.Apply()"/> on the returned <see cref="Texture2D"/>.
        /// </remarks>
        public static Texture2D Rotate(this Texture2D texture, QuadrantalAngle angle) => angle switch
        {
            QuadrantalAngle._0 => texture,
            QuadrantalAngle.Clockwise90 => texture.Rotate90(),
            QuadrantalAngle.Anticlockwise90 => texture.RotateMinus90(),
            QuadrantalAngle._180 => texture.Rotate180(),
            _ => throw new ArgumentException($"Unknown / unimplemented {nameof(QuadrantalAngle)}: {angle}", nameof(angle))
        };
        /// <summary>
        /// Returns a deep copy of the <see cref="Texture2D"/> rotated 90 degrees clockwise.
        /// </summary>
        /// <remarks>
        /// Calls <see cref="Texture2D.Apply()"/> on the returned <see cref="Texture2D"/>.
        /// </remarks>
        private static Texture2D Rotate90(this Texture2D texture)
        {
            Texture2D rotated = new Texture2D(texture.height, texture.width);

            for (int x = 0; x < texture.height; x++)
            {
                for (int y = 0; y < texture.width; y++)
                {
                    rotated.SetPixel(x, y, texture.GetPixel(texture.width - 1 - y, x));
                }
            }

            return rotated.Applied();
        }
        /// <summary>
        /// Returns a deep copy of the <see cref="Texture2D"/> rotated 90 degrees anticlockwise.
        /// </summary>
        /// <remarks>
        /// Calls <see cref="Texture2D.Apply()"/> on the returned <see cref="Texture2D"/>.
        /// </remarks>
        private static Texture2D RotateMinus90(this Texture2D texture)
        {
            Texture2D rotated = new Texture2D(texture.height, texture.width);

            for (int x = 0; x < texture.height; x++)
            {
                for (int y = 0; y < texture.width; y++)
                {
                    rotated.SetPixel(x, y, texture.GetPixel(y, texture.height - 1 - x));
                }
            }

            return rotated.Applied();
        }
        /// <summary>
        /// Returns a deep copy of the <see cref="Texture2D"/> rotated 180 degrees.
        /// </summary>
        /// <remarks>
        /// Calls <see cref="Texture2D.Apply()"/> on the returned <see cref="Texture2D"/>.
        /// </remarks>
        private static Texture2D Rotate180(this Texture2D texture)
        {
            Texture2D rotated = new Texture2D(texture.width, texture.height);

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    rotated.SetPixel(x, y, texture.GetPixel(texture.width - 1 - x, texture.height - 1 - y));
                }
            }

            return rotated.Applied();
        }

        /// <summary>
        /// Options for <see cref="ExtendCrop(Texture2D, in ExtendCropOptions)"/>.
        /// </summary>
        public struct ExtendCropOptions
        {
            /// <summary>
            /// How many columns of pixels to add to the left side of the texture.
            /// </summary>
            /// <remarks>
            /// Negative values will crop the texture.
            /// </remarks>
            public int left;
            /// <summary>
            /// How many columns of pixels to add to the right side of the texture.
            /// </summary>
            /// <remarks>
            /// Negative values will crop the texture.
            /// </remarks>
            public int right;
            /// <summary>
            /// How many rows of pixels to add to the bottom side of the texture.
            /// </summary>
            /// <remarks>
            /// Negative values will crop the texture.
            /// </remarks>
            public int bottom;
            /// <summary>
            /// How many rows of pixels to add to the top side of the texture.
            /// </summary>
            /// <remarks>
            /// Negative values will crop the texture.
            /// </remarks>
            public int top;
        }
        /// <summary>
        /// Creates a deep copy of the <see cref="Texture2D"/> with rows/columns of pixels added to / removed from the sides.
        /// </summary>
        /// <remarks>
        /// Calls <see cref="Texture2D.Apply()"/> on the returned <see cref="Texture2D"/>.
        /// </remarks>
        /// <exception cref="ArgumentException">The resulting width is &lt;= 0 or the resulting height is &lt;= 0.</exception>
        public static Texture2D ExtendCrop(this Texture2D texture, in ExtendCropOptions options)
        {
            if (options.left + options.right <= -texture.width)
            {
                throw new ArgumentException($"Cannot crop by >= the texture's width. Width = {texture.width}, ({nameof(options.left)}, {nameof(options.right)}) = ({options.left}, {options.right}).");
            }
            if (options.top + options.bottom <= -texture.height)
            {
                throw new ArgumentException($"Cannot crop by >= the texture's height. Height = {texture.height}, ({nameof(options.bottom)}, {nameof(options.top)}) = ({options.bottom}, {options.top}).");
            }

            return ExtendCrop(texture, new IntRect((-options.left, -options.bottom), (texture.width - 1 + options.right, texture.height - 1 + options.top)));
        }
        /// <summary>
        /// Creates a deep copy of the <see cref="Texture2D"/> with the dimensions changed to fit the given <see cref="IntRect"/> by adding/removing rows/columns of pixels from the sides of the
        /// <see cref="Texture2D"/>.
        /// </summary>
        /// <remarks>
        /// Calls <see cref="Texture2D.Apply()"/> on the returned <see cref="Texture2D"/>.
        /// </remarks>
        /// <param name="newRect">The coords of the new rect, relative to the coords of the old rect.</param>
        public static Texture2D ExtendCrop(this Texture2D texture, IntRect newRect)
        {
            Texture2D newTexture = new Texture2D(newRect.width, newRect.height);

            for (int x = 0; x < newTexture.width; x++)
            {
                for (int y = 0; y < newTexture.height; y++)
                {
                    IntVector2 coordRelativeToOriginalTexture = (x, y) + newRect.bottomLeft;

                    if (texture.ContainsPixel(coordRelativeToOriginalTexture))
                    {
                        newTexture.SetPixel(x, y, texture.GetPixel(coordRelativeToOriginalTexture));
                    }
                    else
                    {
                        newTexture.SetPixel(x, y, Config.Colours.transparent);
                    }
                }
            }

            return newTexture.Applied();
        }

        /// <summary>
        /// Blends a deep copy of <paramref name="topTexture"/> onto a deep copy of <paramref name="bottomTexture"/> using the given <see cref="BlendMode"/>, placing the bottom-left corner
        /// of <paramref name="topTexture"/> on the bottom-left corner of <paramref name="bottomTexture"/>.
        /// </summary>
        /// <remarks>
        /// Calls <see cref="Texture2D.Apply()"/> on the returned <see cref="Texture2D"/>.
        /// </remarks>
        public static Texture2D Blend(this Texture2D topTexture, Texture2D bottomTexture, BlendMode blendMode) => Blend(topTexture, bottomTexture, blendMode, (0, 0));
        /// <summary>
        /// Blends a deep copy of <paramref name="topTexture"/> onto a deep copy of <paramref name="bottomTexture"/> using the given <see cref="BlendMode"/>, placing the bottom-left corner
        /// of <paramref name="topTexture"/> at the coordinates <paramref name="topTextureOffset"/> on <paramref name="bottomTexture"/>.
        /// </summary>
        /// <remarks>
        /// Calls <see cref="Texture2D.Apply()"/> on the returned <see cref="Texture2D"/>.
        /// </remarks>
        public static Texture2D Blend(this Texture2D topTexture, Texture2D bottomTexture, BlendMode blendMode, IntVector2 topTextureOffset)
        {
            Texture2D blended = new Texture2D(bottomTexture.width, bottomTexture.height);

            for (int x = 0; x < bottomTexture.width; x++)
            {
                for (int y = 0; y < bottomTexture.height; y++)
                {
                    if (blended.ContainsPixel(x - topTextureOffset.x, y - topTextureOffset.y))
                    {
                        Color topColour = topTexture.GetPixel(x - topTextureOffset.x, y - topTextureOffset.y);
                        Color bottomColour = bottomTexture.GetPixel(x, y);
                        blended.SetPixel(x, y, blendMode.Blend(topColour, bottomColour));
                    }
                    else
                    {
                        blended.SetPixel(x, y, bottomTexture.GetPixel(x, y));
                    }
                }
            }

            return blended.Applied();
        }
        /// <summary>
        /// Blends <paramref name="topColour"/> onto each pixel of a deep copy of <paramref name="bottomTexture"/> using the given <see cref="BlendMode"/>.
        /// </summary>
        /// <remarks>
        /// Calls <see cref="Texture2D.Apply()"/> on the returned <see cref="Texture2D"/>.
        /// </remarks>
        public static Texture2D Blend(Color topColour, Texture2D bottomTexture, BlendMode blendMode)
        {
            Texture2D blended = new Texture2D(bottomTexture.width, bottomTexture.height);

            for (int x = 0; x < bottomTexture.width; x++)
            {
                for (int y = 0; y < bottomTexture.height; y++)
                {
                    Color bottomColour = bottomTexture.GetPixel(x, y);
                    blended.SetPixel(x, y, blendMode.Blend(topColour, bottomColour));
                }
            }

            return blended.Applied();
        }

        /// <summary>
        /// Calls <see cref="Scale(Texture2D, float, float)"/> with both scale factors equal to <paramref name="scaleFactor"/>.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="scaleFactor"/> is &lt;= 0.</exception>
        public static Texture2D Scale(this Texture2D texture, float scaleFactor)
        {
            if (scaleFactor <= 0f)
            {
                throw new ArgumentOutOfRangeException($"{nameof(scaleFactor)} must be positive: {scaleFactor}.", nameof(scaleFactor));
            }

            return Scale(texture, scaleFactor, scaleFactor);
        }
        /// <summary>
        /// Returns a deep copy of the <see cref="Texture2D"/> with the width scaled by <paramref name="xScaleFactor"/> and the height scaled by <paramref name="yScaleFactor"/>, using the
        /// nearest-neighbour algorithm.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The width is multiplied by <paramref name="xScaleFactor"/> then rounded. Similarly for the height.
        /// </para>
        /// <para>
        /// Calls <see cref="Texture2D.Apply()"/> on the returned <see cref="Texture2D"/>.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="xScaleFactor"/> is &lt;= 0 or <paramref name="yScaleFactor"/> is &lt;= 0.</exception>
        /// <exception cref="ArgumentException">Rounding the scaled width results in a new width of 0 or rounding the scaled height results in a new height of 0.</exception>
        public static Texture2D Scale(this Texture2D texture, float xScaleFactor, float yScaleFactor)
        {
            if (xScaleFactor <= 0f)
            {
                throw new ArgumentOutOfRangeException($"{nameof(xScaleFactor)} must be positive: {xScaleFactor}.", nameof(xScaleFactor));
            }
            if (yScaleFactor <= 0f)
            {
                throw new ArgumentOutOfRangeException($"{nameof(yScaleFactor)} must be positive: {yScaleFactor}.", nameof(yScaleFactor));
            }

            int newWidth = Mathf.RoundToInt(texture.width * xScaleFactor);
            int newHeight = Mathf.RoundToInt(texture.height * yScaleFactor);

            if (newWidth == 0)
            {
                throw new ArgumentException($"Rounding the scaled width resulted in a new width of 0.", nameof(xScaleFactor));
            }
            if (newHeight == 0)
            {
                throw new ArgumentException($"Rounding the scaled height resulted in a new height of 0.", nameof(yScaleFactor));
            }

            return Scale(texture, newWidth, newHeight);
        }
        /// <summary>
        /// Returns a deep copy of the <see cref="Texture2D"/> scaled to size <paramref name="newWidth"/> x <paramref name="newHeight"/> using the nearest-neighbour algorithm.
        /// </summary>
        /// <remarks>
        /// Calls <see cref="Texture2D.Apply()"/> on the returned <see cref="Texture2D"/>.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="newWidth"/> is &lt;= 0 or <paramref name="newHeight"/> is &lt;= 0.</exception>
        public static Texture2D Scale(this Texture2D texture, int newWidth, int newHeight)
        {
            if (newWidth <= 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(newWidth)} is non-positive: {newWidth}.", nameof(newWidth));
            }
            if (newHeight <= 0)
            {
                throw new ArgumentOutOfRangeException($"{nameof(newHeight)} is non-positive: {newHeight}.", nameof(newHeight));
            }

            Texture2D scaled = new Texture2D(newWidth, newHeight);

            float xScaleFactor = newWidth / (float)texture.width;
            float yScaleFactor = newHeight / (float)texture.height;

            for (int x = 0; x < newWidth; x++)
            {
                for (int y = 0; y < newHeight; y++)
                {
                    int descaledX = Mathf.FloorToInt(x / xScaleFactor);
                    int descaledY = Mathf.FloorToInt(y / yScaleFactor);
                    scaled.SetPixel(x, y, texture.GetPixel(descaledX, descaledY));
                }
            }

            return scaled.Applied();
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
        /// Returns a deep copy of the <see cref="Texture2D"/> with all occurrences of the colour <paramref name="toReplace"/> replaced with <paramref name="replaceWith"/>.
        /// </summary>
        /// <remarks>
        /// Calls <see cref="Texture2D.Apply()"/> on the returned <see cref="Texture2D"/>.
        /// </remarks>
        /// <param name="tolerance">How close a colour has to be to <paramref name="toReplace"/> to be replaced.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="tolerance"/> is negative.</exception>
        public static Texture2D ReplaceColour(this Texture2D texture, Color toReplace, Color replaceWith, float tolerance = 0f)
        {
            if (tolerance < 0f)
            {
                throw new ArgumentOutOfRangeException(nameof(tolerance), $"{nameof(tolerance)} should be non-negative: {tolerance}.");
            }

            Color[] pixels = texture.GetPixels();
            foreach ((IntVector2 pixel, int index) in texture.GetRect().Enumerate())
            {
                if (texture.GetPixel(pixel).Equals(toReplace, tolerance))
                {
                    pixels[index] = replaceWith;
                }
            }

            Texture2D replaced = new Texture2D(texture.width, texture.height);
            replaced.SetPixels(pixels);

            return replaced.Applied();
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