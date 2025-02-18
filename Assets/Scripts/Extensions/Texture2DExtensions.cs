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

            IntRect textureRect = new IntRect(IntVector2.zero, new IntVector2(width * 2 - 1, height * 2 - 1));
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

        public static Texture2D Offset(Texture2D texture, IntVector2 offset)
        {
            Texture2D offsetTexture = new Texture2D(texture.width, texture.height);
            IntRect textureRect = new IntRect(new IntVector2(0, 0), new IntVector2(texture.width - 1, texture.height - 1));

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    if (textureRect.Contains(new IntVector2(x - offset.x, y - offset.y)))
                    {
                        offsetTexture.SetPixel(x, y, texture.GetPixel(x - offset.x, y - offset.y));
                    }
                    else
                    {
                        offsetTexture.SetPixel(x, y, Config.Colours.transparent);
                    }
                }
            }

            offsetTexture.Apply();
            return offsetTexture;
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

            return Blend(
                texture,
                Transparent(texture.width + left + right, texture.height + up + down),
                new IntVector2(left, down),
                BlendMode.Normal
                );
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
        public static Texture2D Blend(Texture2D topTexture, Texture2D bottomTexture, BlendMode blendMode)
        {
            return Blend(topTexture, bottomTexture, IntVector2.zero, blendMode);
        }
        /// <summary>
        /// Overlays topTex onto bottomTex using the given blend mode, placing the bottom-left corner at the coordinates topTexOffset (which don't have to be within the image).
        /// </summary>
        public static Texture2D Blend(Texture2D topTexture, Texture2D bottomTexture, IntVector2 topTextureOffset, BlendMode blendMode)
        {
            Texture2D blended = new Texture2D(bottomTexture.width, bottomTexture.height);
            IntRect textureRect = new IntRect(new IntVector2(0, 0), new IntVector2(topTexture.width - 1, topTexture.height - 1));

            for (int x = 0; x < bottomTexture.width; x++)
            {
                for (int y = 0; y < bottomTexture.height; y++)
                {
                    if (textureRect.Contains(new IntVector2(x - topTextureOffset.x, y - topTextureOffset.y)))
                    {
                        blended.SetPixel(x, y, blendMode.Blend(topTexture.GetPixel(x - topTextureOffset.x, y - topTextureOffset.y), bottomTexture.GetPixel(x, y)));
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
                    blended.SetPixel(x, y, blendMode.Blend(topColour, bottomTexture.GetPixel(x, y)));
                }
            }

            blended.Apply();
            return blended;
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
            AssertValidTextureDimensions(newWidth, newHeight, nameof(newWidth), nameof(newHeight));

            Texture2D scaled = new Texture2D(newWidth, newHeight);
            float xScalar = (float)newWidth / texture.width;
            float yScalar = (float)newHeight / texture.height;

            for (int x = 0; x < newWidth; x++)
            {
                for (int y = 0; y < newHeight; y++)
                {
                    scaled.SetPixel(x, y, texture.GetPixel(Mathf.FloorToInt(x / xScalar), Mathf.FloorToInt(y / yScalar)));
                }
            }

            scaled.Apply();
            return scaled;
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
            Texture2D filled = Copy(texture);

            foreach (IntVector2 pixel in GetPixelsToFill(texture, startPoint, maxNumOfIterations))
            {
                filled.SetPixel(pixel.x, pixel.y, colour);
            }

            filled.Apply();
            return filled;
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
            IntRect textureRect = new IntRect(IntVector2.zero, new IntVector2(texture.width - 1, texture.height - 1));

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
                    if (textureRect.Contains(offsetCoord) && texture.GetPixel(offsetCoord.x, offsetCoord.y) == colourToReplace && !visited.Contains(offsetCoord))
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
            Texture2D texture = null;
            byte[] fileData;

            if (System.IO.File.Exists(filePath))
            {
                fileData = System.IO.File.ReadAllBytes(filePath);
                texture = new Texture2D(1, 1);
                texture.LoadImage(fileData);
            }
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
            Texture2D outlined = Copy(texture);

            IntRect textureRect = new IntRect(IntVector2.zero, new IntVector2(outlined.width - 1, outlined.height - 1));
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

            for (int x = 0; x < outlined.width; x++)
            {
                for (int y = 0; y < outlined.height; y++)
                {
                    if (outlineOutside && outlined.GetPixel(x, y).a == 0)
                    {
                        foreach (IntVector2 offset in offsets)
                        {
                            if (textureRect.Contains(new IntVector2(x, y) + offset) && outlined.GetPixel(x + offset.x, y + offset.y).a != 0)
                            {
                                pixelsInOutline.Add(new IntVector2(x, y));
                            }
                        }
                    }
                    else if (!outlineOutside && outlined.GetPixel(x, y).a != 0)
                    {
                        foreach (IntVector2 offset in offsets)
                        {
                            if (!textureRect.Contains(new IntVector2(x, y) + offset) || outlined.GetPixel(x + offset.x, y + offset.y).a == 0)
                            {
                                pixelsInOutline.Add(new IntVector2(x, y));
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