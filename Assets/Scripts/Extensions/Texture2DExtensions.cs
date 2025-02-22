using System;
using System.Collections.Generic;
using System.Linq;

using PAC.Colour;
using PAC.DataStructures;
using PAC.Patterns;

using UnityEngine;

namespace PAC.Extensions
{
    public struct OutlineSideFill
    {
        public bool topLeft;
        public bool topMiddle;
        public bool topRight;
        public bool middleLeft;
        public bool middleRight;
        public bool bottomLeft;
        public bool bottomMiddle;
        public bool bottomRight;

        public OutlineSideFill(bool topLeft, bool topMiddle, bool topRight, bool middleLeft, bool middleRight, bool bottomLeft, bool bottomMiddle, bool bottomRight)
        {
            this.topLeft = topLeft;
            this.topMiddle = topMiddle;
            this.topRight = topRight;
            this.middleLeft = middleLeft;
            this.middleRight = middleRight;
            this.bottomLeft = bottomLeft;
            this.bottomMiddle = bottomMiddle;
            this.bottomRight = bottomRight;
        }
    }

    public static class Texture2DExtensions
    {
        /// <summary>
        /// Checks that <paramref name="width"/> and <paramref name="height"/> are both &gt; 0, and throws an <see cref="ArgumentException"/> otherwise.
        /// </summary>
        private static void AssertValidTextureDimensions(int width, int height, string widthParamName, string heightParamName)
        {
            if (width <= 0)
            {
                throw new ArgumentException($"{widthParamName} is non-positive: {width}.", widthParamName);
            }
            if (height <= 0)
            {
                throw new ArgumentException($"{heightParamName} is non-positive: {height}.", heightParamName);
            }
        }

        /// <summary>
        /// Turns the <see cref="Texture2D"/> into a <see cref="Sprite"/> with the <see cref="FilterMode.Point"/> filter mode.
        /// </summary>
        public static Sprite ToSprite(this Texture2D texture)
        {
            Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, texture.width, texture.height), new Vector2(0.5f, 0.5f), Math.Max(texture.width, texture.height), 0, SpriteMeshType.FullRect);
            sprite.texture.filterMode = FilterMode.Point;
            return sprite;
        }

        /// <summary>
        /// Creates a transparent (alpha 0) <see cref="Texture2D"/> of the given dimensions.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="width"/> or <paramref name="height"/> is &lt;= 0.</exception>
        public static Texture2D Transparent(int width, int height) => Solid(width, height, Config.Colours.transparent);

        /// <summary>
        /// Creates a <see cref="Texture2D"/> of the given dimensions, filled with the given colour.
        /// </summary>
        /// <remarks>
        /// <see cref="Solid(int, int, Color32)"/> is faster.
        /// </remarks>
        /// <exception cref="ArgumentException"><paramref name="width"/> or <paramref name="height"/> is &lt;= 0.</exception>
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
            texture.Apply();
            return texture;
        }
        /// <summary>
        /// Creates a <see cref="Texture2D"/> of the given dimensions, filled with the given colour.
        /// </summary>
        /// <remarks>
        /// This is faster than <see cref="Solid(int, int, Color)"/>.
        /// </remarks>
        /// <exception cref="ArgumentException"><paramref name="width"/> or <paramref name="height"/> is &lt;= 0.</exception>
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
            texture.Apply();
            return texture;
        }

        public static Texture2D CheckerboardBackground(int width, int height)
        {
            AssertValidTextureDimensions(width, height, nameof(width), nameof(height));

            IntRect textureRect = new IntRect((0, 0), (width * 2 - 1, height * 2 - 1));
            return new Checkerboard<Color32>(Preferences.transparentCheckerboardColour1, Preferences.transparentCheckerboardColour2).ToTexture(textureRect);
        }

        public static Texture2D HSLHueSaturationGrid(int width, int height)
        {
            AssertValidTextureDimensions(width, height, nameof(width), nameof(height));

            Texture2D texture = new Texture2D(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color colour = new HSL(x / (float)width, y / (float)(height - 1), 0.5f).ToColor();
                    texture.SetPixel(x, y, colour);
                }
            }

            texture.Apply();
            return texture;
        }

        public static Texture2D Flip(Texture2D texture, FlipAxis axis) => axis switch
        {
            FlipAxis.None => texture,
            FlipAxis.Vertical => FlipX(texture),
            FlipAxis.Horizontal => FlipY(texture),
            _ => throw new ArgumentException($"Unknown / unimplemented FlipAxis: {axis}.", nameof(axis))
        };
        private static Texture2D FlipX(Texture2D texture)
        {
            Texture2D flipped = new Texture2D(texture.width, texture.height);

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    flipped.SetPixel(x, y, texture.GetPixel(texture.width - 1 - x, y));
                }
            }

            flipped.Apply();
            return flipped;
        }
        private static Texture2D FlipY(Texture2D texture)
        {
            Texture2D flipped = new Texture2D(texture.width, texture.height);

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    flipped.SetPixel(x, y, texture.GetPixel(x, texture.height - 1 - y));
                }
            }

            flipped.Apply();
            return flipped;
        }

        public static Texture2D Rotate(Texture2D texture, RotationAngle angle) => angle switch
        {
            RotationAngle._0 => texture,
            RotationAngle._90 => Rotate90(texture),
            RotationAngle.Minus90 => RotateMinus90(texture),
            RotationAngle._180 => Rotate180(texture),
            _ => throw new ArgumentException($"Unknown / unimplemented RotationAngle: {angle}", nameof(angle))
        };
        /// <summary>
        /// Rotation is clockwise.
        /// </summary>
        private static Texture2D Rotate90(Texture2D texture)
        {
            Texture2D rotated = new Texture2D(texture.height, texture.width);

            for (int x = 0; x < texture.height; x++)
            {
                for (int y = 0; y < texture.width; y++)
                {
                    rotated.SetPixel(x, y, texture.GetPixel(texture.width - 1 - y, x));
                }
            }

            rotated.Apply();
            return rotated;
        }
        /// <summary>
        /// Rotation is clockwise.
        /// </summary>
        private static Texture2D RotateMinus90(Texture2D texture)
        {
            Texture2D rotated = new Texture2D(texture.height, texture.width);

            for (int x = 0; x < texture.height; x++)
            {
                for (int y = 0; y < texture.width; y++)
                {
                    rotated.SetPixel(x, y, texture.GetPixel(y, texture.height - 1 - x));
                }
            }

            rotated.Apply();
            return rotated;
        }
        private static Texture2D Rotate180(Texture2D texture)
        {
            Texture2D rotated = new Texture2D(texture.width, texture.height);

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    rotated.SetPixel(x, y, texture.GetPixel(texture.width - 1 - x, texture.height - 1 - y));
                }
            }

            rotated.Apply();
            return rotated;
        }

        /// <summary>
        /// Adds the given number of transparent pixels to each side of the texture. Negative amounts will crop the image.
        /// </summary>
        public static Texture2D ExtendCrop(Texture2D texture, int left, int right, int down, int up)
        {
            if (left + right <= -texture.width)
            {
                throw new ArgumentException($"Cannot crop by >= the texture's width. Width = {texture.width}, ({nameof(left)}, {nameof(right)}) = ({left}, {right}).");
            }
            if (up + down <= -texture.height)
            {
                throw new ArgumentException($"Cannot crop by >= the texture's height. Height = {texture.height}, ({nameof(down)}, {nameof(up)}) = ({down}, {up}).");
            }

            return ExtendCrop(texture, new IntRect((-left, -down), (texture.width - 1 + right, texture.height - 1 + up)));
        }
        /// <summary>
        /// Changes the dimensions of the texture to the new rect.
        /// </summary>
        /// <param name="newRect">The coords of the new rect relative to the coords of the old rect.</param>
        public static Texture2D ExtendCrop(Texture2D texture, IntRect newRect)
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

            newTexture.Apply();
            return newTexture;
        }

        /// <summary>
        /// Overlays topTex onto bottomTex using the given blend mode, placing the bottom-left corner on the bottom-left corner.
        /// </summary>
        public static Texture2D Blend(Texture2D topTexture, Texture2D bottomTexture, BlendMode blendMode) => Blend(topTexture, bottomTexture, blendMode, (0, 0));
        /// <summary>
        /// Overlays topTex onto bottomTex using the given blend mode, placing the bottom-left corner at the coordinates topTexOffset (which don't have to be within the image).
        /// </summary>
        public static Texture2D Blend(Texture2D topTexture, Texture2D bottomTexture, BlendMode blendMode, IntVector2 topTextureOffset)
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

            blended.Apply();
            return blended;
        }
        /// <summary>
        /// Overlays topColour onto each pixel of bottomTex using the given blend mode.
        /// </summary>
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

            blended.Apply();
            return blended;
        }

        public static Texture2D Scale(Texture2D texture, float scaleFactor)
        {
            if (scaleFactor <= 0f)
            {
                throw new ArgumentOutOfRangeException($"{nameof(scaleFactor)} must be positive: {scaleFactor}.", nameof(scaleFactor));
            }

            return Scale(texture, scaleFactor, scaleFactor);
        }
        public static Texture2D Scale(Texture2D texture, float xScaleFactor, float yScaleFactor)
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
            return Scale(texture, newWidth, newHeight);
        }
        public static Texture2D Scale(Texture2D texture, int newWidth, int newHeight)
        {
            AssertValidTextureDimensions(newWidth, newHeight, nameof(newWidth), nameof(newHeight));

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

            scaled.Apply();
            return scaled;
        }

        public static IEnumerable<IntVector2> GetFloodFillPixels(Texture2D texture, IntVector2 startPoint, int maxNumOfIterations = 1_000_000)
        {
            Color colourToReplace = texture.GetPixel(startPoint);

            Queue<IntVector2> toVisit = new Queue<IntVector2>();
            HashSet<IntVector2> visited = new HashSet<IntVector2>();

            toVisit.Enqueue(startPoint);
            visited.Add(startPoint);
            yield return startPoint;

            int iterations = 0;
            while (toVisit.Count > 0 && iterations < maxNumOfIterations)
            {
                IntVector2 coord = toVisit.Dequeue();

                foreach (IntVector2 offset in IntVector2.upDownLeftRight)
                {
                    IntVector2 adjacentCoord = coord + offset;
                    if (!visited.Contains(adjacentCoord) && texture.ContainsPixel(adjacentCoord) && texture.GetPixel(adjacentCoord) == colourToReplace)
                    {
                        toVisit.Enqueue(adjacentCoord);
                        visited.Add(adjacentCoord);
                        yield return adjacentCoord;
                    }
                }

                iterations++;
            }
        }

        /// <summary>
        /// Creates a deepcopy of the texture using Color colours.
        /// </summary>
        public static Texture2D DeepCopy(Texture2D texture)
        {
            Texture2D copy = new Texture2D(texture.width, texture.height);
            copy.SetPixels(texture.GetPixels());
            copy.Apply();
            return copy;
        }

        public static Texture2D LoadFromFile(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                throw new ArgumentException($"File path does not exist: {filePath}", nameof(filePath));
            }

            byte[] fileData = System.IO.File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(1, 1);
            texture.LoadImage(fileData);

            return texture;
        }

        /// <summary>
        /// Makes an outline around the non-transparent pixels of the given texture.
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="outlineColour"></param>
        /// <param name="outlineOutside">When true: the outline is created next to existing pixels (widens the sprite). When false: replaces the outer pixels.</param>
        /// <param name="outlineCorners">When true: the outline makes a full right-angle at corners. When false: corners are 'rounded'.</param>
        /// <returns></returns>
        public static Texture2D Outline(Texture2D texture, Color outlineColour, bool outlineOutside, OutlineSideFill outlineSideFill)
        {
            Texture2D outlined = DeepCopy(texture);

            List<IntVector2> offsets = new List<IntVector2>();

            if (outlineSideFill.topLeft)
            {
                offsets.Add(IntVector2.upLeft * (outlineOutside ? -1 : 1));
            }
            if (outlineSideFill.topMiddle)
            {
                offsets.Add(IntVector2.up * (outlineOutside ? -1 : 1));
            }
            if (outlineSideFill.topRight)
            {
                offsets.Add(IntVector2.upRight * (outlineOutside ? -1 : 1));
            }
            if (outlineSideFill.middleLeft)
            {
                offsets.Add(IntVector2.left * (outlineOutside ? -1 : 1));
            }
            if (outlineSideFill.middleRight)
            {
                offsets.Add(IntVector2.right * (outlineOutside ? -1 : 1));
            }
            if (outlineSideFill.bottomLeft)
            {
                offsets.Add(IntVector2.downLeft * (outlineOutside ? -1 : 1));
            }
            if (outlineSideFill.bottomMiddle)
            {
                offsets.Add(IntVector2.down * (outlineOutside ? -1 : 1));
            }
            if (outlineSideFill.bottomRight)
            {
                offsets.Add(IntVector2.downRight * (outlineOutside ? -1 : 1));
            }

            List<IntVector2> pixelsInOutline = new List<IntVector2>();

            for (int x = 0; x < outlined.width; x++)
            {
                for (int y = 0; y < outlined.height; y++)
                {
                    if (outlineOutside && outlined.GetPixel(x, y).a == 0)
                    {
                        foreach (IntVector2 offset in offsets)
                        {
                            if (texture.ContainsPixel((x, y) + offset) && outlined.GetPixel(x + offset.x, y + offset.y).a != 0)
                            {
                                pixelsInOutline.Add((x, y));
                            }
                        }
                    }
                    else if (!outlineOutside && outlined.GetPixel(x, y).a != 0)
                    {
                        foreach (IntVector2 offset in offsets)
                        {
                            if (!texture.ContainsPixel((x, y) + offset) || outlined.GetPixel(x + offset.x, y + offset.y).a == 0)
                            {
                                pixelsInOutline.Add((x, y));
                            }
                        }
                    }
                }
            }

            foreach (IntVector2 point in pixelsInOutline)
            {
                outlined.SetPixel(point.x, point.y, outlineColour);
            }

            outlined.Apply();
            return outlined;
        }

        public static Texture2D ReplaceColour(Texture2D texture, Color toReplace, Color replaceWith)
        {
            Texture2D replaced = new Texture2D(texture.width, texture.height);

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    Color pixelColour = texture.GetPixel(x, y);
                    if (Vector4.Distance(pixelColour, toReplace) < 0.01f)
                    {
                        replaced.SetPixel(x, y, replaceWith);
                    }
                    else
                    {
                        replaced.SetPixel(x, y, texture.GetPixel(x, y));
                    }
                }
            }

            replaced.Apply();
            return replaced;
        }

        public static IntRect GetRect(this Texture2D texture) => new IntRect((0, 0), (texture.width - 1, texture.height - 1));

        public static bool ContainsPixel(this Texture2D texture, int x, int y)
        {
            return x >= 0 && y >= 0 && x < texture.width && y < texture.height;
        }
        public static bool ContainsPixel(this Texture2D texture, IntVector2 pixel)
        {
            return texture.ContainsPixel(pixel.x, pixel.y);
        }

        public static void SetPixel(this Texture2D texture, IntVector2 coords, Color colour)
        {
            texture.SetPixel(coords.x, coords.y, colour);
        }

        public static Color GetPixel(this Texture2D texture, IntVector2 coords)
        {
            return texture.GetPixel(coords.x, coords.y);
        }
    }
}