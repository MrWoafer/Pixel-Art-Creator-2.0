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
            if (width <= 0)
            {
                throw new ArgumentException($"{nameof(width)} is non-positive: {width}.", nameof(width));
            }
            if (height <= 0)
            {
                throw new ArgumentException($"{nameof(height)} is non-positive: {height}.", nameof(height));
            }

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
            if (width <= 0)
            {
                throw new ArgumentException($"{nameof(width)} is non-positive: {width}.", nameof(width));
            }
            if (height <= 0)
            {
                throw new ArgumentException($"{nameof(height)} is non-positive: {height}.", nameof(height));
            }

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
            if (width <= 0 || height <= 0)
            {
                throw new System.Exception("Dimensions must be positive: (width, height) = (" + width + ", " + height + ")");
            }

            IntRect texRect = new IntRect(IntVector2.zero, new IntVector2(width * 2 - 1, height * 2 - 1));
            return new Checkerboard<Color32>(Preferences.transparentCheckerboardColour1, Preferences.transparentCheckerboardColour2).ToTexture(texRect);
        }

        public static Texture2D HSLHueSaturationGrid(int width, int height)
        {
            if (width <= 0 || height <= 0)
            {
                throw new System.Exception("Invalid dimensions: " + width.ToString() + "x" + height.ToString());
            }

            Texture2D tex = new Texture2D(width, height);

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Color colour = new HSL(x / (float)width, y / (float)(height - 1), 0.5f).ToColor();
                    tex.SetPixel(x, y, colour);
                }
            }

            tex.Apply();
            return tex;
        }

        public static Texture2D Flip(Texture2D texture, FlipAxis axis)
        {
            if (axis == FlipAxis.None)
            {
                return texture;
            }
            else if (axis == FlipAxis.Vertical)
            {
                return FlipX(texture);
            }
            else if (axis == FlipAxis.Horizontal)
            {
                return FlipY(texture);
            }
            else
            {
                throw new NotImplementedException("Unknown / unimplemented FlipAxis: " + axis);
            }
        }

        public static Texture2D FlipX(Texture2D texture)
        {
            Texture2D tex = new Texture2D(texture.width, texture.height);

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    tex.SetPixel(x, y, texture.GetPixel(texture.width - 1 - x, y));
                }
            }

            tex.Apply();
            return tex;
        }

        public static Texture2D FlipY(Texture2D texture)
        {
            Texture2D tex = new Texture2D(texture.width, texture.height);

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    tex.SetPixel(x, y, texture.GetPixel(x, texture.height - 1 - y));
                }
            }

            tex.Apply();
            return tex;
        }

        /// <summary>
        /// Rotation is clockwise.
        /// </summary>
        public static Texture2D Rotate(Texture2D texture, RotationAngle angle)
        {
            if (angle == RotationAngle._0)
            {
                return texture;
            }
            else if (angle == RotationAngle._90)
            {
                return Rotate90(texture);
            }
            else if (angle == RotationAngle.Minus90)
            {
                return RotateMinus90(texture);
            }
            else if (angle == RotationAngle._180)
            {
                return Rotate180(texture);
            }
            else
            {
                throw new System.Exception("Unknown / unimplemented RotationAngle: " + angle);
            }
        }

        /// <summary>
        /// Rotation is clockwise.
        /// </summary>
        public static Texture2D Rotate90(Texture2D texture)
        {
            Texture2D tex = new Texture2D(texture.height, texture.width);

            for (int x = 0; x < texture.height; x++)
            {
                for (int y = 0; y < texture.width; y++)
                {
                    tex.SetPixel(x, y, texture.GetPixel(texture.width - 1 - y, x));
                }
            }

            tex.Apply();
            return tex;
        }

        /// <summary>
        /// Rotation is clockwise.
        /// </summary>
        public static Texture2D RotateMinus90(Texture2D texture)
        {
            Texture2D tex = new Texture2D(texture.height, texture.width);

            for (int x = 0; x < texture.height; x++)
            {
                for (int y = 0; y < texture.width; y++)
                {
                    tex.SetPixel(x, y, texture.GetPixel(y, texture.height - 1 - x));
                }
            }

            tex.Apply();
            return tex;
        }

        public static Texture2D Rotate180(Texture2D texture)
        {
            Texture2D tex = new Texture2D(texture.width, texture.height);

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    tex.SetPixel(x, y, texture.GetPixel(texture.width - 1 - x, texture.height - 1 - y));
                }
            }

            tex.Apply();
            return tex;
        }

        public static Texture2D Offset(Texture2D texture, IntVector2 offset)
        {
            Texture2D offsetTex = new Texture2D(texture.width, texture.height);
            IntRect texRect = new IntRect(new IntVector2(0, 0), new IntVector2(texture.width - 1, texture.height - 1));

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    if (texRect.Contains(new IntVector2(x - offset.x, y - offset.y)))
                    {
                        offsetTex.SetPixel(x, y, texture.GetPixel(x - offset.x, y - offset.y));
                    }
                    else
                    {
                        offsetTex.SetPixel(x, y, Config.Colours.transparent);
                    }
                }
            }

            offsetTex.Apply();
            return offsetTex;
        }

        /// <summary>
        /// Adds the given number of transparent pixels to each side of the texture. Negative amounts will crop the image.
        /// </summary>
        public static Texture2D Extend(Texture2D texture, int left, int right, int up, int down)
        {
            if (left + right <= -texture.width || up + down <= -texture.height)
            {
                throw new System.Exception("Cannot crop by >= the image width/height: (width, height) = (" + texture.width + ", " + texture.height + ");" +
                                           " (left, right, up, down) = (" + left + ", " + right + ", " + up + ", " + down + ")");
            }

            return Overlay(texture, Transparent(texture.width + left + right, texture.height + up + down), new IntVector2(left, down));
        }

        /// <summary>
        /// Changes the dimensions of the texture to the new rect.
        /// </summary>
        /// <param name="newRect">The coords of the new rect relative to the coords of the old rect.</param>
        public static Texture2D ChangeRect(Texture2D texture, IntRect newRect)
        {
            return Extend(texture, -newRect.bottomLeft.x, newRect.topRight.x - texture.width + 1, newRect.topRight.y - texture.height + 1, -newRect.bottomLeft.y);
        }

        /// <summary>
        /// Overlays topTex onto bottomTex using the given blend mode, placing the bottom-left corner on the bottom-left corner.
        /// </summary>
        public static Texture2D Blend(Texture2D topTex, Texture2D bottomTex, BlendMode blendMode)
        {
            return Blend(topTex, bottomTex, IntVector2.zero, blendMode);
        }
        /// <summary>
        /// Overlays topTex onto bottomTex using the given blend mode, placing the bottom-left corner at the coordinates topTexOffset (which don't have to be within the image).
        /// </summary>
        public static Texture2D Blend(Texture2D topTex, Texture2D bottomTex, IntVector2 topTexOffset, BlendMode blendMode)
        {
            Texture2D tex = new Texture2D(bottomTex.width, bottomTex.height);
            IntRect texRect = new IntRect(new IntVector2(0, 0), new IntVector2(topTex.width - 1, topTex.height - 1));

            for (int x = 0; x < bottomTex.width; x++)
            {
                for (int y = 0; y < bottomTex.height; y++)
                {
                    if (texRect.Contains(new IntVector2(x - topTexOffset.x, y - topTexOffset.y)))
                    {
                        tex.SetPixel(x, y, blendMode.Blend(topTex.GetPixel(x - topTexOffset.x, y - topTexOffset.y), bottomTex.GetPixel(x, y)));
                    }
                    else
                    {
                        tex.SetPixel(x, y, bottomTex.GetPixel(x, y));
                    }
                }
            }

            tex.Apply();
            return tex;
        }
        /// <summary>
        /// Overlays topColour onto each pixel of bottomTex using the given blend mode.
        /// </summary>
        public static Texture2D Blend(Color topColour, Texture2D bottomTex, BlendMode blendMode)
        {
            Texture2D tex = new Texture2D(bottomTex.width, bottomTex.height);

            for (int x = 0; x < bottomTex.width; x++)
            {
                for (int y = 0; y < bottomTex.height; y++)
                {
                    tex.SetPixel(x, y, blendMode.Blend(topColour, bottomTex.GetPixel(x, y)));
                }
            }

            tex.Apply();
            return tex;
        }

        /// <summary>
        /// Overlays topTex onto bottomTex, placing the bottom-left corner on the bottom-left corner. Uses Normal blend mode.
        /// </summary>
        public static Texture2D Overlay(Texture2D topTex, Texture2D bottomTex)
        {
            return Overlay(topTex, bottomTex, IntVector2.zero);
        }
        /// <summary>
        /// Overlays topTex onto bottomTex, placing the bottom-left corner at the coordinates topTexOffset (which don't have to be within the image). Uses Normal blend mode.
        /// </summary>
        public static Texture2D Overlay(Texture2D topTex, Texture2D bottomTex, IntVector2 topTexOffset)
        {
            return Blend(topTex, bottomTex, topTexOffset, BlendMode.Normal);
        }

        public static Texture2D Multiply(Texture2D texture, Color colour)
        {
            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    texture.SetPixel(x, y, texture.GetPixel(x, y) * colour);
                }
            }

            texture.Apply();
            return texture;
        }

        public static Texture2D Scale(Texture2D texture, float scaleFactor)
        {
            if (scaleFactor <= 0)
            {
                throw new System.Exception("Cannot scale texture by a non-positive amount: " + scaleFactor);
            }

            return Scale(texture, scaleFactor, scaleFactor);
        }
        public static Texture2D Scale(Texture2D texture, float xScaleFactor, float yScaleFactor)
        {
            if (xScaleFactor <= 0)
            {
                throw new System.Exception("Cannot scale width by a non-positive amount: " + xScaleFactor);
            }
            if (yScaleFactor <= 0)
            {
                throw new System.Exception("Cannot scale height by a non-positive amount: " + yScaleFactor);
            }

            return Scale(texture, Mathf.RoundToInt(texture.width * xScaleFactor), Mathf.RoundToInt(texture.height * yScaleFactor));
        }
        public static Texture2D Scale(Texture2D texture, int newWidth, int newHeight)
        {
            if (newWidth <= 0 || newHeight <= 0)
            {
                throw new System.Exception("Invalid dimensions: " + newWidth.ToString() + "x" + newHeight.ToString());
            }

            Texture2D scaledTex = new Texture2D(newWidth, newHeight);
            float xScalar = (float)newWidth / texture.width;
            float yScalar = (float)newHeight / texture.height;

            for (int x = 0; x < newWidth; x++)
            {
                for (int y = 0; y < newHeight; y++)
                {
                    scaledTex.SetPixel(x, y, texture.GetPixel(Mathf.FloorToInt(x / xScalar), Mathf.FloorToInt(y / yScalar)));
                }
            }

            scaledTex.Apply();
            return scaledTex;
        }

        public static Texture2D Subtract(Texture2D topTex, Texture2D bottomTex)
        {
            if (topTex.width != bottomTex.width || topTex.height != bottomTex.height)
            {
                throw new System.Exception("Dimensions don't match: " + topTex.width + "x" + topTex.height + " and " + bottomTex.width + "x" + bottomTex.height);
            }

            Texture2D tex = new Texture2D(bottomTex.width, bottomTex.height);

            for (int x = 0; x < bottomTex.width; x++)
            {
                for (int y = 0; y < bottomTex.height; y++)
                {
                    tex.SetPixel(x, y, topTex.GetPixel(x, y) - bottomTex.GetPixel(x, y));
                }
            }

            tex.Apply();
            return tex;
        }

        public static Texture2D ApplyMask(Texture2D texture, Texture2D mask)
        {
            if (texture.width != mask.width || texture.height != mask.height)
            {
                throw new System.Exception("Texture dimensions don't match mask dimensions: " + texture.width + "x" + texture.height + " and " + mask.width + "x" + mask.height);
            }

            Texture2D maskedTexture = new Texture2D(texture.width, texture.height);

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    if (mask.GetPixel(x, y).a != 0f)
                    {
                        maskedTexture.SetPixel(x, y, texture.GetPixel(x, y));
                    }
                    else
                    {
                        maskedTexture.SetPixel(x, y, Config.Colours.mask);
                    }
                }
            }

            maskedTexture.Apply();
            return maskedTexture;
        }
        public static Texture2D ApplyMask(Texture2D texture, IntVector2[] mask)
        {
            Texture2D maskedTexture = Transparent(texture.width, texture.height);

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    if (mask.Contains(new IntVector2(x, y)))
                    {
                        maskedTexture.SetPixel(x, y, texture.GetPixel(x, y));
                    }
                    else
                    {
                        maskedTexture.SetPixel(x, y, Config.Colours.mask);
                    }
                }
            }

            maskedTexture.Apply();
            return maskedTexture;
        }

        public static Texture2D Fill(Texture2D texture, IntVector2 startPoint, Color colour, int maxNumOfIterations = 1_000_000)
        {
            Texture2D filledTex = Copy(texture);

            foreach (IntVector2 pixel in GetPixelsToFill(texture, startPoint, maxNumOfIterations))
            {
                filledTex.SetPixel(pixel.x, pixel.y, colour);
            }

            filledTex.Apply();
            return filledTex;
        }

        public static Texture2D GetFillMask(Texture2D texture, IntVector2 startPoint, int maxNumOfIterations = 1_000_000)
        {
            Texture2D fillMask = Transparent(texture.width, texture.height);

            foreach (IntVector2 pixel in GetPixelsToFill(texture, startPoint, maxNumOfIterations))
            {
                fillMask.SetPixel(pixel.x, pixel.y, Config.Colours.mask);
            }

            fillMask.Apply();
            return fillMask;
        }

        public static IEnumerable<IntVector2> GetPixelsToFill(Texture2D texture, IntVector2 startPoint, int maxNumOfIterations = 1_000_000)
        {
            IntRect texRect = new IntRect(IntVector2.zero, new IntVector2(texture.width - 1, texture.height - 1));

            Queue<IntVector2> toVisit = new Queue<IntVector2>();
            HashSet<IntVector2> visited = new HashSet<IntVector2>();

            Color colourToReplace = texture.GetPixel(startPoint.x, startPoint.y);

            toVisit.Enqueue(startPoint);
            visited.Add(startPoint);
            yield return startPoint;

            int iterations = 0;
            while (toVisit.Count > 0 && iterations < maxNumOfIterations)
            {
                IntVector2 coord = toVisit.Dequeue();

                foreach (IntVector2 offset in new IntVector2[] { new IntVector2(1, 0), new IntVector2(0, 1), new IntVector2(-1, 0), new IntVector2(0, -1) })
                {
                    IntVector2 offsetCoord = coord + offset;
                    if (texRect.Contains(offsetCoord) && texture.GetPixel(offsetCoord.x, offsetCoord.y) == colourToReplace && !visited.Contains(offsetCoord))
                    {
                        toVisit.Enqueue(offsetCoord);
                        visited.Add(offsetCoord);
                        yield return offsetCoord;
                    }
                }

                iterations++;
            }
        }

        /// <summary>
        /// Creates a deepcopy of the texture using Color colours.
        /// </summary>
        public static Texture2D Copy(Texture2D texture)
        {
            Texture2D copy = new Texture2D(texture.width, texture.height);
            copy.SetPixels(texture.GetPixels());
            copy.Apply();
            return copy;
        }
        /// <summary>
        /// Creates a deepcopy of the texture using Color32 colours.
        /// </summary>
        public static Texture2D Copy32(Texture2D texture)
        {
            Texture2D copy = new Texture2D(texture.width, texture.height);
            copy.SetPixels32(texture.GetPixels32());
            copy.Apply();
            return copy;
        }

        public static Texture2D LoadFromFile(string filePath)
        {
            Texture2D tex = null;
            byte[] fileData;

            if (System.IO.File.Exists(filePath))
            {
                fileData = System.IO.File.ReadAllBytes(filePath);
                tex = new Texture2D(1, 1);
                tex.LoadImage(fileData);
            }
            return tex;
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
            Texture2D tex = Copy(texture);

            IntRect texRect = new IntRect(IntVector2.zero, new IntVector2(tex.width - 1, tex.height - 1));
            List<IntVector2> offsets = new List<IntVector2>();

            if (outlineSideFill.topLeft)
            {
                offsets.Add(new IntVector2(-1, 1) * (outlineOutside ? -1 : 1));
            }
            if (outlineSideFill.topMiddle)
            {
                offsets.Add(new IntVector2(0, 1) * (outlineOutside ? -1 : 1));
            }
            if (outlineSideFill.topRight)
            {
                offsets.Add(new IntVector2(1, 1) * (outlineOutside ? -1 : 1));
            }
            if (outlineSideFill.middleLeft)
            {
                offsets.Add(new IntVector2(-1, 0) * (outlineOutside ? -1 : 1));
            }
            if (outlineSideFill.middleRight)
            {
                offsets.Add(new IntVector2(1, 0) * (outlineOutside ? -1 : 1));
            }
            if (outlineSideFill.bottomLeft)
            {
                offsets.Add(new IntVector2(-1, -1) * (outlineOutside ? -1 : 1));
            }
            if (outlineSideFill.bottomMiddle)
            {
                offsets.Add(new IntVector2(0, -1) * (outlineOutside ? -1 : 1));
            }
            if (outlineSideFill.bottomRight)
            {
                offsets.Add(new IntVector2(1, -1) * (outlineOutside ? -1 : 1));
            }

            List<IntVector2> pixelsInOutline = new List<IntVector2>();

            for (int x = 0; x < tex.width; x++)
            {
                for (int y = 0; y < tex.height; y++)
                {
                    if (outlineOutside && tex.GetPixel(x, y).a == 0)
                    {
                        foreach (IntVector2 offset in offsets)
                        {
                            if (texRect.Contains(new IntVector2(x, y) + offset) && tex.GetPixel(x + offset.x, y + offset.y).a != 0)
                            {
                                pixelsInOutline.Add(new IntVector2(x, y));
                            }
                        }
                    }
                    else if (!outlineOutside && tex.GetPixel(x, y).a != 0)
                    {
                        foreach (IntVector2 offset in offsets)
                        {
                            if (!texRect.Contains(new IntVector2(x, y) + offset) || tex.GetPixel(x + offset.x, y + offset.y).a == 0)
                            {
                                pixelsInOutline.Add(new IntVector2(x, y));
                            }
                        }
                    }
                }
            }

            foreach (IntVector2 point in pixelsInOutline)
            {
                tex.SetPixel(point.x, point.y, outlineColour);
            }

            tex.Apply();
            return tex;
        }

        public static Texture2D ReplaceColour(Texture2D texture, Color toReplace, Color replaceWith)
        {
            Texture2D tex = new Texture2D(texture.width, texture.height);

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    Color pixelColour = texture.GetPixel(x, y);
                    if (Vector4.Distance(pixelColour, toReplace) < 0.01f)
                    {
                        tex.SetPixel(x, y, replaceWith);
                    }
                    else
                    {
                        tex.SetPixel(x, y, texture.GetPixel(x, y));
                    }
                }
            }

            tex.Apply();
            return tex;
        }

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