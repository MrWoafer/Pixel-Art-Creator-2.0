using System;
using System.Collections.Generic;

using PAC.DataStructures;
using PAC.Extensions;

using UnityEngine;

namespace PAC.ImageEditing
{
    /// <summary>
    /// Handles creating pixel art outlines.
    /// </summary>
    public static class Outline
    {
        /// <summary>
        /// Defines how the outline in <see cref="DrawOutline(Texture2D, Color, in Options)"/> will look.
        /// </summary>
        public struct Options
        {
            public enum OutlineType
            {
                /// <summary>
                /// The outline will be drawn on non-transparent pixels that are adjacent to transparent pixels.
                /// </summary>
                Inside,
                /// <summary>
                /// The outline will be drawn on transparent pixels that are adjacent to non-transparent pixels.
                /// </summary>
                Outside
            }
            public OutlineType outlineType;

            /// <summary>
            /// Whether to draw the outline on the top-left edges.
            /// </summary>
            public bool includeTopLeft;
            /// <summary>
            /// Whether to draw the outline on the top-middle edges.
            /// </summary>
            public bool includeTopMiddle;
            /// <summary>
            /// Whether to draw the outline on the top-right edges.
            /// </summary>
            public bool includeTopRight;
            /// <summary>
            /// Whether to draw the outline on the middle-left edges.
            /// </summary>
            public bool includeMiddleLeft;
            /// <summary>
            /// Whether to draw the outline on the middle-right edges.
            /// </summary>
            public bool includeMiddleRight;
            /// <summary>
            /// Whether to draw the outline on the bottom-left edges.
            /// </summary>
            public bool includeBottomLeft;
            /// <summary>
            /// Whether to draw the outline on the bottom-middle edges.
            /// </summary>
            public bool includeBottomMiddle;
            /// <summary>
            /// Whether to draw the outline on the bottom-right edges.
            /// </summary>
            public bool includeBottomRight;

            /// <summary>
            /// Iterates over the directions associated with <see cref="includeTopLeft"/> etc for those that are <see langword="true"/>.
            /// </summary>
            public readonly IEnumerable<IntVector2> EnumerateDirectionsToInclude()
            {
                if (includeTopLeft)
                {
                    yield return IntVector2.upLeft;
                }
                if (includeTopMiddle)
                {
                    yield return IntVector2.up;
                }
                if (includeTopRight)
                {
                    yield return IntVector2.upRight;
                }
                if (includeMiddleLeft)
                {
                    yield return IntVector2.left;
                }
                if (includeMiddleRight)
                {
                    yield return IntVector2.right;
                }
                if (includeBottomLeft)
                {
                    yield return IntVector2.downLeft;
                }
                if (includeBottomMiddle)
                {
                    yield return IntVector2.down;
                }
                if (includeBottomRight)
                {
                    yield return IntVector2.downRight;
                }
            }
        }

        /// <summary>
        /// Returns a deep copy of the <see cref="Texture2D"/> with an outline around the non-transparent pixels.
        /// </summary>
        /// <remarks>
        /// Calls <see cref="Texture2D.Apply()"/> on the returned <see cref="Texture2D"/>.
        /// </remarks>
        public static Texture2D DrawOutline(Texture2D texture, Color outlineColour, in Options outlineOptions)
        {
            Color[] pixels = texture.GetPixels();

            if (outlineOptions.outlineType == Options.OutlineType.Outside)
            {
                foreach ((IntVector2 pixel, int index) in texture.GetRect().Enumerate())
                {
                    if (texture.GetPixel(pixel).a == 0f)
                    {
                        foreach (IntVector2 offset in outlineOptions.EnumerateDirectionsToInclude())
                        {
                            if (texture.ContainsPixel(pixel - offset) && texture.GetPixel(pixel - offset).a != 0f)
                            {
                                pixels[index] = outlineColour;
                                break;
                            }
                        }
                    }
                }
            }
            else if (outlineOptions.outlineType == Options.OutlineType.Inside)
            {
                foreach ((IntVector2 pixel, int index) in texture.GetRect().Enumerate())
                {
                    if (texture.GetPixel(pixel).a != 0f)
                    {
                        foreach (IntVector2 offset in outlineOptions.EnumerateDirectionsToInclude())
                        {
                            if (!texture.ContainsPixel(pixel + offset) || texture.GetPixel(pixel + offset).a == 0f)
                            {
                                pixels[index] = outlineColour;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                throw new NotImplementedException($"Unknown / unimplemented {nameof(Options.OutlineType)}: {outlineOptions.outlineType}.");
            }

            Texture2D outlined = new Texture2D(texture.width, texture.height);
            outlined.SetPixels(pixels);

            return outlined.Applied();
        }
    }
}