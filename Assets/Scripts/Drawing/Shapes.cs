using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PAC.DataStructures;
using PAC.Exceptions;
using PAC.Extensions;
using UnityEngine;

namespace PAC.Drawing
{
    /// <summary>
    /// <para>
    /// These methods could be defined as default implementations in IShape, but are defined as extension methods for one (or both) of the following reasons:
    /// </para>
    /// <para>
    /// 1)
    ///     They are not directly related to the purpose of the IShape, which is just to represent a shape. E.g. ToTexture() is not directly related to this.
    /// </para>
    /// <para>
    /// 2)  
    ///     That would require casting to IShape to use them. Making them as extension methods avoids needing this cast. We could turn IShape into an abstract class to avoid this default
    ///     implementation casting issue, but then to have a more specific return type in methods like Translate() (e.g. Line.Translate() returns a Line instead of just IShape) we need covariant
    ///     return types, which isn't yet supported in Unity's compiler.
    /// </para>
    /// </summary>
    public static class IShapeExtensions
    {
        /// <summary>
        /// Returns whether the shape contains all the given pixels.
        /// </summary>
        public static bool Contains(this Shapes.IShape shape, IEnumerable<IntVector2> pixels)
        {
            foreach (IntVector2 pixel in pixels)
            {
                if (!shape.Contains(pixel))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Turns the pixels in the shape's bounding rect into a Texture2D.
        /// </summary>
        public static Texture2D ToTexture(this Shapes.IShape shape, Color colour) => shape.ToTexture(colour, shape.boundingRect);
        /// <summary>
        /// Turns the pixels in the given IntRect into a Texture2D, using any of the shape's pixels that lie within that rect. 
        /// </summary>
        public static Texture2D ToTexture(this Shapes.IShape shape, Color colour, IntRect texRect)
        {
            Texture2D tex = Tex2DSprite.BlankTexture(texRect.width, texRect.height);

            foreach (IntVector2 pixel in shape)
            {
                if (texRect.Contains(pixel))
                {
                    tex.SetPixel(pixel - texRect.bottomLeft, colour);
                }
            }

            tex.Apply();
            return tex;
        }
    }

    /// <summary>
    /// A class to define how different shapes are drawn.
    /// </summary>
    public static class Shapes
    {
        public interface IShape : IReadOnlyCollection<IntVector2>
        {
            /// <summary>
            /// The smallest IntRect containing the whole shape.
            /// </summary>
            public IntRect boundingRect { get; }

            /// <summary>
            /// Returns whether the pixel is in the shape.
            /// </summary>
            public bool Contains(IntVector2 pixel);

            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public static IShape operator +(IShape shape, IntVector2 translation) => shape.Translate(translation);
            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public static IShape operator +(IntVector2 translation, IShape shape) => shape + translation;
            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public static IShape operator -(IShape shape, IntVector2 translation) => shape + (-translation);
            /// <summary>
            /// Reflects the shape through the origin.
            /// </summary>
            public static IShape operator -(IShape shape) => shape.Flip(FlipAxis.Vertical).Flip(FlipAxis.Horizontal);

            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public IShape Translate(IntVector2 translation);
            /// <summary>
            /// Reflects the shape across the given axis.
            /// </summary>
            public IShape Flip(FlipAxis axis);

            public IShape DeepCopy();
        }

        public interface IRotatableShape : IShape
        {
            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public static IRotatableShape operator +(IRotatableShape shape, IntVector2 translation) => shape.Translate(translation);
            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public static IRotatableShape operator +(IntVector2 translation, IRotatableShape shape) => shape + translation;
            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public static IRotatableShape operator -(IRotatableShape shape, IntVector2 translation) => shape + (-translation);
            /// <summary>
            /// Reflects the shape through the origin.
            /// </summary>
            public static IRotatableShape operator -(IRotatableShape shape) => shape.Rotate(RotationAngle._180);

            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public new IRotatableShape Translate(IntVector2 translation);
            IShape IShape.Translate(IntVector2 translation) => Translate(translation);
            /// <summary>
            /// Reflects the shape across the given axis.
            /// </summary>
            public new IRotatableShape Flip(FlipAxis axis);
            IShape IShape.Flip(FlipAxis axis) => Flip(axis);
            /// <summary>
            /// Rotates the shape by the given angle.
            /// </summary>
            public IRotatableShape Rotate(RotationAngle angle);

            public new IRotatableShape DeepCopy();
            IShape IShape.DeepCopy() => DeepCopy();
        }

        public interface I1DShape : IRotatableShape
        {
            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public static I1DShape operator +(I1DShape shape, IntVector2 translation) => shape.Translate(translation);
            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public static I1DShape operator +(IntVector2 translation, I1DShape shape) => shape + translation;
            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public static I1DShape operator -(I1DShape shape, IntVector2 translation) => shape + (-translation);
            /// <summary>
            /// Reflects the shape through the origin.
            /// </summary>
            public static I1DShape operator -(I1DShape shape) => shape.Rotate(RotationAngle._180);

            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public new I1DShape Translate(IntVector2 translation);
            IRotatableShape IRotatableShape.Translate(IntVector2 translation) => Translate(translation);
            /// <summary>
            /// Reflects the shape across the given axis.
            /// </summary>
            public new I1DShape Flip(FlipAxis axis);
            IRotatableShape IRotatableShape.Flip(FlipAxis axis) => Flip(axis);
            /// <summary>
            /// Rotates the shape by the given angle.
            /// </summary>
            public new I1DShape Rotate(RotationAngle angle);
            IRotatableShape IRotatableShape.Rotate(RotationAngle angle) => Rotate(angle);

            public new I1DShape DeepCopy();
            IRotatableShape IRotatableShape.DeepCopy() => DeepCopy();
        }

        /// <summary>
        /// A pixel-perfect line between two points, ordered from start to end.
        /// </summary>
        public class Line : I1DShape
        {
            // NOTE: For this shape, we work in a coordinate system where integer coordinates refer to the CENTRE of a pixel - e.g. the centre of pixel (0, 0) is (0, 0), not (0.5, 0.5).

            private IntVector2 _start;
            public IntVector2 start
            {
                get => _start;
                set => SetValues(value, end);
            }

            private IntVector2 _end;
            public IntVector2 end
            {
                get => _end;
                set => SetValues(start, value);
            }

            // These values are calculated and cached when you set the start/end of the line
            /// <summary>
            /// The start point of the imaginary non-pixel-perfect line. We adjust these to the outside corners of the start/end pixels so that the line will always use blocks of
            /// equal size when possible.
            /// </summary>
            private Vector2 imaginaryStart;
            /// <summary>
            /// The start point of the imaginary non-pixel-perfect line. We adjust these to the outside corners of the start/end pixels so that the line will always use blocks of
            /// equal size when possible.
            /// </summary>
            private Vector2 imaginaryEnd;
            /// <summary>
            /// The gradient of the imaginary non-pixel-perfect line.
            /// </summary>
            private float imaginaryGradient;
            /// <summary>
            /// Whether the line is more horizontal than vertical. If the line is at +/- 45 degrees then it is true.
            /// </summary>
            private bool isMoreHorizontal;

            /// <summary>
            /// Whether this line is divided into blocks of a constant size. E.g. a 12x4 line is perfect as it is drawn as 4 horizontal blocks of 3 pixels.
            /// </summary>
            public bool isPerfect =>
                isMoreHorizontal ?
                (Math.Abs(end.x - start.x) + 1) % (Math.Abs(end.y - start.y) + 1) == 0 :
                (Math.Abs(end.y - start.y) + 1) % (Math.Abs(end.x - start.x) + 1) == 0;
            /// <summary>
            /// True iff the start and end are equal.
            /// </summary>
            public bool isPoint => start == end;
            /// <summary>
            /// True iff the line has a constant x coord.
            /// </summary>
            public bool isVertical => start.x == end.x;
            /// <summary>
            /// True iff the line has a constant y coord.
            /// </summary>
            public bool isHorizontal => start.y == end.y;

            /// <summary>
            /// <para>
            /// Returns a new line with the start / end swapped.
            /// </para>
            /// <para>
            /// Note: this is not guaranteed to give the same shape - the centre pixel can change when swapping the start / end.
            /// </para>
            /// </summary>
            public Line reverse => new Line(end, start);

            public IntRect boundingRect => new IntRect(start, end);

            public int Count => IntVector2.SupDistance(start, end) + 1;

            public Line(IntVector2 start, IntVector2 end)
            {
                // Initialise fields before we actually define them in SetValues()
                _start = start;
                _end = end;
                imaginaryStart = start;
                imaginaryEnd = end;
                imaginaryGradient = 0f;
                isMoreHorizontal = false;

                SetValues(start, end);
            }

            private void SetValues(IntVector2 start, IntVector2 end)
            {
                _start = start;
                _end = end;

                if (start == end)
                {
                    imaginaryStart = start;
                    imaginaryEnd = end;
                }
                if (start.x == end.x)
                {
                    // end is directly above start
                    if (start.y < end.y)
                    {
                        imaginaryStart = start + 0.5f * Vector2.down;
                        imaginaryEnd = end + 0.5f * Vector2.up;
                    }
                    // end is directly below start
                    else
                    {
                        imaginaryStart = start + 0.5f * Vector2.up;
                        imaginaryEnd = end + 0.5f * Vector2.down;
                    }
                }
                else if (start.y == end.y)
                {
                    // end is directly to the right of start
                    if (start.x < end.x)
                    {
                        imaginaryStart = start + 0.5f * Vector2.left;
                        imaginaryEnd = end + 0.5f * Vector2.right;
                    }
                    // end is directly to the left of start
                    else
                    {
                        imaginaryStart = start + 0.5f * Vector2.right;
                        imaginaryEnd = end + 0.5f * Vector2.left;
                    }
                }
                if (start.x < end.x)
                {
                    // end is to top-right of start
                    if (start.y < end.y)
                    {
                        imaginaryStart = start - 0.5f * Vector2.one;
                        imaginaryEnd = end + 0.5f * Vector2.one;
                    }
                    // end is to bottom-right of start
                    else
                    {
                        imaginaryStart = start + 0.5f * new Vector2(-1, 1);
                        imaginaryEnd = end + 0.5f * new Vector2(1, -1);
                    }
                }
                else
                {
                    // end is to top-left of start
                    if (start.y < end.y)
                    {
                        imaginaryStart = start + 0.5f * new Vector2(1, -1);
                        imaginaryEnd = end + 0.5f * new Vector2(-1, 1);
                    }
                    // end is to bottom-left of start
                    else
                    {
                        imaginaryStart = start + 0.5f * Vector2.one;
                        imaginaryEnd = end - 0.5f * Vector2.one;
                    }
                }

                // True iff 1 >= gradient >= -1, i.e. the line is more (or equally) horizontal than vertical
                isMoreHorizontal = Math.Abs(end.y - start.y) <= Math.Abs(end.x - start.x);

                if (imaginaryStart.x == imaginaryEnd.x)
                {
                    if (imaginaryStart.y == imaginaryEnd.y)
                    {
                        imaginaryGradient = float.NaN;
                    }
                    else
                    {
                        imaginaryGradient = float.PositiveInfinity;
                    }
                }
                else if (isMoreHorizontal)
                {
                    imaginaryGradient = (imaginaryEnd.y - imaginaryStart.y) / (imaginaryEnd.x - imaginaryStart.x);
                }
                else
                {
                    // This is the gradient of x with respect to y, whereas all the others are of y with respect to x
                    imaginaryGradient = (imaginaryEnd.x - imaginaryStart.x) / (imaginaryEnd.y - imaginaryStart.y);
                }
            }

            /// <summary>
            /// Returns whether the pixel is in the line.
            /// </summary>
            public bool Contains(IntVector2 pixel)
            {
                int index = isMoreHorizontal ? (pixel.x - start.x) * Math.Sign(end.x - start.x) : (pixel.y - start.y) * Math.Sign(end.y - start.y);
                if (index < 0 || index >= Count)
                {
                    return false;
                }
                return this[index] == pixel;
            }

            /// <summary>
            /// Whether the point lies to the left of / on the line (and lies within the y range of the line).
            /// </summary>
            public bool PointIsToLeft(IntVector2 pixel)
            {
                if (!isMoreHorizontal)
                {
                    int index = (pixel.y - start.y) * Math.Sign(end.y - start.y);
                    if (index < 0 || index >= Count)
                    {
                        return false;
                    }
                    return pixel.x <= this[index].x;
                }
                else
                {
                    if (pixel.y < boundingRect.bottomLeft.y || pixel.y > boundingRect.topRight.y || pixel.x > boundingRect.topRight.x)
                    {
                        return false;
                    }
                    if (pixel.x < boundingRect.bottomLeft.x)
                    {
                        return true;
                    }

                    // pixel now lies within bounding rect

                    int index = (pixel.x - start.x) * Math.Sign(end.x - start.x);
                    if (index < 0 || index >= Count)
                    {
                        return false;
                    }
                    bool positiveGradient = Math.Sign(end.x - start.x) == Math.Sign(end.y - start.y);
                    return positiveGradient ? pixel.y >= this[index].y : pixel.y <= this[index].y;
                }
            }

            /// <summary>
            /// Whether the point lies to the right of / on the line (and lies within the y range of the line).
            /// </summary>
            public bool PointIsToRight(IntVector2 pixel)
            {
                if (!isMoreHorizontal)
                {
                    int index = (pixel.y - start.y) * Math.Sign(end.y - start.y);
                    if (index < 0 || index >= Count)
                    {
                        return false;
                    }
                    return pixel.x >= this[index].x;
                }
                else
                {
                    if (pixel.y < boundingRect.bottomLeft.y || pixel.y > boundingRect.topRight.y || pixel.x < boundingRect.bottomLeft.x)
                    {
                        return false;
                    }
                    if (pixel.x > boundingRect.topRight.x)
                    {
                        return true;
                    }

                    // pixel now lies within bounding rect

                    int index = (pixel.x - start.x) * Math.Sign(end.x - start.x);
                    if (index < 0 || index >= Count)
                    {
                        return false;
                    }
                    bool positiveGradient = Math.Sign(end.x - start.x) == Math.Sign(end.y - start.y);
                    return positiveGradient ? pixel.y <= this[index].y : pixel.y >= this[index].y;
                }
            }

            /// <summary>
            /// Returns the minimum x coord of the pixels on the line that have the given y coord.
            /// </summary>
            public int MinX(int y)
            {
                if (y < boundingRect.bottomLeft.y || y > boundingRect.topRight.y)
                {
                    throw new ArgumentOutOfRangeException("y must be within the y range of the line. y: " + y + "; line y range: [" + boundingRect.bottomLeft.y + ", " +  boundingRect.topRight.y + "]");
                }

                if (Math.Abs(end.y - start.y) >= Math.Abs(end.x - start.x))
                {
                    return this[(y - start.y) * Math.Sign(end.y - start.y)].x;
                }

                if (start.y == end.y)
                {
                    return Math.Min(start.x, end.x);
                }

                // The minimum value of x such that plugging it into the the line and rounding gives you the parameter y.
                // The line is:
                //      y = (x - imaginaryStart.x) * imaginaryGradient + imaginaryStart.y
                // Which rearranges to:
                //      x = (y - imaginaryStart.y) / imaginaryGradient + imaginaryStart.x
                float minXWhereYOnLineRoundsToGivenY = (y - 0.5f * Mathf.Sign(end.y - start.y) * Mathf.Sign(end.x - start.x) - imaginaryStart.y) / imaginaryGradient + imaginaryStart.x;

                // The case where minXWhereYOnLineRoundsToGivenY is an integer
                if (Functions.Mod((y - 0.5f * Math.Sign(end.y - start.y) * Math.Sign(end.x - start.x) - imaginaryStart.y) * (imaginaryEnd.x - imaginaryStart.x), imaginaryEnd.y - imaginaryStart.y)
                    == Functions.Mod((imaginaryEnd.y - imaginaryStart.y) / 2f, imaginaryEnd.y - imaginaryStart.y))
                {
                    // If minXWhereYOnLineRoundsToGivenY is in the first half of the line
                    if (2 * Math.Abs(Mathf.RoundToInt(minXWhereYOnLineRoundsToGivenY) - start.x) <= Math.Abs(end.x - start.x))
                    {
                        return start.x < end.x ? Mathf.RoundToInt(minXWhereYOnLineRoundsToGivenY) + 1 : Mathf.RoundToInt(minXWhereYOnLineRoundsToGivenY);
                    }
                    else
                    {
                        return start.x < end.x ? Mathf.RoundToInt(minXWhereYOnLineRoundsToGivenY) : Mathf.RoundToInt(minXWhereYOnLineRoundsToGivenY) + 1;
                    }
                }
                else
                {
                    return Mathf.CeilToInt(minXWhereYOnLineRoundsToGivenY);
                }
            }

            /// <summary>
            /// Returns the maximum x coord of the pixels on the line that have the given y coord.
            /// </summary>
            public int MaxX(int y)
            {
                if (y < boundingRect.bottomLeft.y || y > boundingRect.topRight.y)
                {
                    throw new ArgumentOutOfRangeException("y must be within the y range of the line. y: " + y + "; line y range: [" + boundingRect.bottomLeft.y + ", " +  boundingRect.topRight.y + "]");
                }

                if (Math.Abs(end.y - start.y) >= Math.Abs(end.x - start.x))
                {
                    return this[(y - start.y) * Math.Sign(end.y - start.y)].x;
                }

                if (start.y == end.y)
                {
                    return Math.Max(start.x, end.x);
                }

                // The maximum value of x such that plugging it into the the line and rounding gives you the parameter y.
                // The line is:
                //      y = (x - imaginaryStart.x) * imaginaryGradient + imaginaryStart.y
                // Which rearranges to:
                //      x = (y - imaginaryStart.y) / imaginaryGradient + imaginaryStart.x
                float maxXWhereYOnLineRoundsToGivenY = (y + 0.5f * Mathf.Sign(end.y - start.y) * Mathf.Sign(end.x - start.x) - imaginaryStart.y) / imaginaryGradient + imaginaryStart.x;

                // The case where maxXWhereYOnLineRoundsToGivenY is an integer
                if (Functions.Mod((y + 0.5f * Math.Sign(end.y - start.y) * Math.Sign(end.x - start.x) - imaginaryStart.y) * (imaginaryEnd.x - imaginaryStart.x), imaginaryEnd.y - imaginaryStart.y)
                    == Functions.Mod((imaginaryEnd.y - imaginaryStart.y) / 2f, imaginaryEnd.y - imaginaryStart.y))
                {
                    // If maxXWhereYOnLineRoundsToGivenY is in the first half of the line
                    if (2 * Math.Abs(Mathf.RoundToInt(maxXWhereYOnLineRoundsToGivenY) - start.x) <= Math.Abs(end.x - start.x))
                    {
                        return start.x < end.x ? Mathf.RoundToInt(maxXWhereYOnLineRoundsToGivenY) : Mathf.RoundToInt(maxXWhereYOnLineRoundsToGivenY) - 1;
                    }
                    else
                    {
                        return start.x < end.x ? Mathf.RoundToInt(maxXWhereYOnLineRoundsToGivenY) - 1 : Mathf.RoundToInt(maxXWhereYOnLineRoundsToGivenY);
                    }
                }
                else
                {
                    return Mathf.FloorToInt(maxXWhereYOnLineRoundsToGivenY);
                }
            }

            /// <summary>
            /// Returns the minimum y coord of the pixels on the line that have the given x coord.
            /// </summary>
            public int MinY(int x)
            {
                if (x < boundingRect.bottomLeft.x || x > boundingRect.topRight.x)
                {
                    throw new ArgumentOutOfRangeException("x must be within the x range of the line. x: " + x + "; line y range: [" + boundingRect.bottomLeft.x + ", " + boundingRect.topRight.x + "]");
                }

                if (Math.Abs(end.x - start.x) >= Math.Abs(end.y - start.y))
                {
                    return this[(x - start.x) * Math.Sign(end.x - start.x)].y;
                }

                if (start.x == end.x)
                {
                    return Math.Min(start.y, end.y);
                }

                // The minimum value of y such that plugging it into the the line and rounding gives you the parameter x.
                // The line is:
                //      x = (y - imaginaryStart.y) * imaginaryGradient + imaginaryStart.x
                // Which rearranges to:
                //      y = (x - imaginaryStart.x) / imaginaryGradient + imaginaryStart.y
                float minYWhereXOnLineRoundsToGivenX = (x - 0.5f * Mathf.Sign(end.x - start.x) * Mathf.Sign(end.y - start.y) - imaginaryStart.x) / imaginaryGradient + imaginaryStart.y;

                // The case where minYWhereXOnLineRoundsToGivenX is an integer
                if (Functions.Mod((x - 0.5f * Math.Sign(end.x - start.x) * Math.Sign(end.y - start.y) - imaginaryStart.x) * (imaginaryEnd.y - imaginaryStart.y), imaginaryEnd.x - imaginaryStart.x)
                    == Functions.Mod((imaginaryEnd.x - imaginaryStart.x) / 2f, imaginaryEnd.x - imaginaryStart.x))
                {
                    // If minYWhereXOnLineRoundsToGivenX is in the first half of the line
                    if (2 * Math.Abs(Mathf.RoundToInt(minYWhereXOnLineRoundsToGivenX) - start.y) <= Math.Abs(end.y - start.y))
                    {
                        return start.y < end.y ? Mathf.RoundToInt(minYWhereXOnLineRoundsToGivenX) + 1 : Mathf.RoundToInt(minYWhereXOnLineRoundsToGivenX);
                    }
                    else
                    {
                        return start.y < end.y ? Mathf.RoundToInt(minYWhereXOnLineRoundsToGivenX) : Mathf.RoundToInt(minYWhereXOnLineRoundsToGivenX) + 1;
                    }
                }
                else
                {
                    return Mathf.CeilToInt(minYWhereXOnLineRoundsToGivenX);
                }
            }

            /// <summary>
            /// Returns the maximum y coord of the pixels on the line that have the given x coord.
            /// </summary>
            public int MaxY(int x)
            {
                if (x < boundingRect.bottomLeft.x || x > boundingRect.topRight.x)
                {
                    throw new ArgumentOutOfRangeException("x must be within the x range of the line. x: " + x + "; line y range: [" + boundingRect.bottomLeft.x + ", " + boundingRect.topRight.x + "]");
                }

                if (Math.Abs(end.x - start.x) >= Math.Abs(end.y - start.y))
                {
                    return this[(x - start.x) * Math.Sign(end.x - start.x)].y;
                }

                if (start.x == end.x)
                {
                    return Math.Max(start.y, end.y);
                }

                // The maximum value of y such that plugging it into the the line and rounding gives you the parameter x.
                // The line is:
                //      x = (y - imaginaryStart.y) * imaginaryGradient + imaginaryStart.x
                // Which rearranges to:
                //      y = (x - imaginaryStart.x) / imaginaryGradient + imaginaryStart.y
                float maxYWhereXOnLineRoundsToGivenX = (x + 0.5f * Mathf.Sign(end.x - start.x) * Mathf.Sign(end.y - start.y) - imaginaryStart.x) / imaginaryGradient + imaginaryStart.y;

                // The case where maxYWhereXOnLineRoundsToGivenX is an integer
                if (Functions.Mod((x + 0.5f * Math.Sign(end.x - start.x) * Math.Sign(end.y - start.y) - imaginaryStart.x) * (imaginaryEnd.y - imaginaryStart.y), imaginaryEnd.x - imaginaryStart.x)
                    == Functions.Mod((imaginaryEnd.x - imaginaryStart.x) / 2f, imaginaryEnd.x - imaginaryStart.x))
                {
                    // If maxYWhereXOnLineRoundsToGivenX is in the first half of the line
                    if (2 * Math.Abs(Mathf.RoundToInt(maxYWhereXOnLineRoundsToGivenX) - start.y) <= Math.Abs(end.y - start.y))
                    {
                        return start.y < end.y ? Mathf.RoundToInt(maxYWhereXOnLineRoundsToGivenX) : Mathf.RoundToInt(maxYWhereXOnLineRoundsToGivenX) - 1;
                    }
                    else
                    {
                        return start.y < end.y ? Mathf.RoundToInt(maxYWhereXOnLineRoundsToGivenX) - 1 : Mathf.RoundToInt(maxYWhereXOnLineRoundsToGivenX);
                    }
                }
                else
                {
                    return Mathf.FloorToInt(maxYWhereXOnLineRoundsToGivenX);
                }
            }

            /// <summary>
            /// Returns the number of pixels on the line that have the given x coord.
            /// </summary>
            public int CountOnX(int x) => MaxY(x) - MinY(x) + 1;
            /// <summary>
            /// Returns the number of pixels on the line that have the given y coord.
            /// </summary>
            public int CountOnY(int y) => MaxX(y) - MinX(y) + 1;

            /// <summary>
            /// Translates the line by the given vector.
            /// </summary>
            public static Line operator +(Line line, IntVector2 translation) => line.Translate(translation);
            /// <summary>
            /// Translates the line by the given vector.
            /// </summary>
            public static Line operator +(IntVector2 translation, Line line) => line + translation;
            /// <summary>
            /// Translates the line by the given vector.
            /// </summary>
            public static Line operator -(Line line, IntVector2 translation) => line + (-translation);
            /// <summary>
            /// Reflects the line through the origin.
            /// </summary>
            public static Line operator -(Line line) => new Line(-line.start, -line.end);

            I1DShape I1DShape.Translate(IntVector2 translation) => Translate(translation);
            /// <summary>
            /// Translates the line by the given vector.
            /// </summary>
            public Line Translate(IntVector2 translation) => new Line(start + translation, end + translation);

            I1DShape I1DShape.Flip(FlipAxis axis) => Flip(axis);
            /// <summary>
            /// Reflects the line across the given axis.
            /// </summary>
            public Line Flip(FlipAxis axis) => new Line(start.Flip(axis), end.Flip(axis));

            I1DShape I1DShape.Rotate(RotationAngle angle) => Rotate(angle);
            /// <summary>
            /// Rotates the line by the given angle.
            /// </summary>
            public Line Rotate(RotationAngle angle) => new Line(start.Rotate(angle), end.Rotate(angle));

            public IntVector2 this[int index]
            {
                get
                {
                    if (index < 0)
                    {
                        throw new IndexOutOfRangeException("Index cannot be negative. Index: " + index);
                    }
                    if (index >= Count)
                    {
                        throw new IndexOutOfRangeException("Index cannot be >= Count. Index: " + index + ". Count: " + Count);
                    }

                    if (index == 0)
                    {
                        return start;
                    }
                    if (index == Count - 1)
                    {
                        return end;
                    }

                    // Index is in the segment from start to end (exclusive).

                    if (start.x == end.x)
                    {
                        // Single point
                        if (start.y == end.y)
                        {
                            return start;
                        }

                        // Vertical line
                        return new IntVector2(start.x, start.y + index * Math.Sign(end.y - start.y));
                    }
                    // Horizontal line
                    if (start.y == end.y)
                    {
                        return new IntVector2(start.x + index * Math.Sign(end.x - start.x), start.y);
                    }

                    if (isMoreHorizontal)
                    {
                        int x = start.x + index * Math.Sign(end.x - start.x);

                        // Line equation is:
                        //      gradient = (y - lineStart.y) / (x - lineStart.x)
                        float y = (x - imaginaryStart.x) * imaginaryGradient + imaginaryStart.y;

                        // Deal with edge case of y being exactly on the border of two pixels
                        // We always pull the y so it's closer to the endpoint x is closest to (e.g. if x is closer to start.x than end.x then we round y up/down to whichever is closer to start.y)
                        if ((x - imaginaryStart.x) * (imaginaryEnd.y - imaginaryStart.y) % (imaginaryEnd.x - imaginaryStart.x) == 0)
                        {
                            // If we're in the first half of the line
                            if (2 * Math.Abs(x - start.x) <= Math.Abs(end.x - start.x))
                            {
                                return new IntVector2(x, Mathf.FloorToInt(y) + (start.y < end.y ? 0 : 1));
                            }
                            else
                            {
                                return new IntVector2(x, Mathf.FloorToInt(y) + (start.y < end.y ? 1 : 0));
                            }
                        }
                        else
                        {
                            return new IntVector2(x, Mathf.RoundToInt(y));
                        }
                    }
                    else
                    {
                        int y = start.y + index * Math.Sign(end.y - start.y);

                        // Line equation is:
                        //      gradient = (x - lineStart.x) / (y - lineStart.y)
                        float x = (y - imaginaryStart.y) * imaginaryGradient + imaginaryStart.x;

                        // Deal with edge case of x being exactly on the border of two pixels
                        // We always pull the x so it's closer to the endpoint y is closest to (e.g. if y is closer to start.y than end.y then we round x up/down to whichever is closer to start.x)
                        if ((y - imaginaryStart.y) * (imaginaryEnd.x - imaginaryStart.x) % (imaginaryEnd.y - imaginaryStart.y) == 0)
                        {
                            // If we're in the first half of the line
                            if (2 * Math.Abs(y - start.y) <= Math.Abs(end.y - start.y))
                            {
                                return new IntVector2(Mathf.FloorToInt(x) + (start.x < end.x ? 0 : 1), y);
                            }
                            else
                            {
                                return new IntVector2(Mathf.FloorToInt(x) + (start.x < end.x ? 1 : 0), y);
                            }
                        }
                        else
                        {
                            return new IntVector2(Mathf.RoundToInt(x), y);
                        }
                    }
                }
            }
            public IEnumerable<IntVector2> this[Range range]
            {
                get
                {
                    foreach (int i in range.AsIEnumerable(Count))
                    {
                        yield return this[i];
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public IEnumerator<IntVector2> GetEnumerator()
            {
                for (int i = 0; i < Count; i++)
                {
                    yield return this[i];
                }
            }

            public static bool operator ==(Line a, Line b) => a.start == b.start && a.end == b.end;
            public static bool operator !=(Line a, Line b) => !(a == b);
            public override bool Equals(object obj)
            {
                if (obj == null || !GetType().Equals(obj.GetType()))
                {
                    return false;
                }
                else
                {
                    return this == (Line)obj;
                }
            }

            public override int GetHashCode() => HashCode.Combine(start, end);

            public override string ToString() => "Line(" + start + ", " + end + ")";

            I1DShape I1DShape.DeepCopy() => DeepCopy();
            public Line DeepCopy() => new Line(start, end);
        }

        /// <summary>
        /// <para>
        /// Represents a sequence of Lines.
        /// The lines must connect, meaning the end of each line must be equal to or adjacent to (including diagonally) the start of the next line.
        /// </para>
        /// <para>
        /// When iterating through the Path, pixels will never appear twice in succession. E.g. if the end of a line equals the start of the next line, that pixel with not be double-counted.
        /// Additionally, the last pixel of the path will not be counted if it's the same as first pixel of the path. Other than those cases, pixels can be double-counted.
        /// </para>
        /// </summary>
        public class Path : I1DShape
        {
            /// <summary>
            /// The lines in this list should not be referenced by anything other than this object, as editing them can potentially causing us to lose the property that the lines connect. For this
            /// reason, we deep-copy lines in the constructor.
            /// </summary>
            private List<Line> _lines = new List<Line>();
            public ReadOnlyCollection<Line> lines => _lines.AsReadOnly();

            public IntVector2 start => _lines[0].start;
            public IntVector2 end => _lines[^1].end;

            /// <summary>
            /// Whether the end of the last line is adjacent to (including diagonally) the start of the first line.
            /// </summary>
            public bool isLoop => IntVector2.SupDistance(start, end) <= 1;

            /// <summary>
            /// <para>
            /// Returns a new path going through the lines in reverse order and with their starts / ends swapped.
            /// </para>
            /// <para>
            /// Note: this is not guaranteed to give the same shape - the centre pixel of a line can change when swapping its start / end.
            /// </para>
            /// </summary>
            public Path reverse
            {
                get
                {
                    List<Line> reversedLines = new List<Line>(_lines.Count);
                    for (int i = _lines.Count - 1; i >= 0; i--)
                    {
                        reversedLines.Add(_lines[i].reverse);
                    }
                    return new Path(reversedLines);
                }
            }

            /// <summary>
            /// True iff the path never changes pixel.
            /// </summary>
            public bool isPoint
            {
                get
                {
                    // First and middle lines (not last)
                    for (int i = 0; i < _lines.Count - 1; i++)
                    {
                        if (_lines[i].start != _lines[i].end)
                        {
                            return false;
                        }
                        if (_lines[i].end != _lines[i + 1].start)
                        {
                            return false;
                        }
                    }

                    // Last line
                    if (_lines[^1].start != _lines[^1].end)
                    {
                        return false;
                    }

                    return true;
                }
            }
            /// <summary>
            /// True iff the path never changes x coord.
            /// </summary>
            public bool isVertical
            {
                get
                {
                    // First and middle lines (not last)
                    for (int i = 0; i < _lines.Count - 1; i++)
                    {
                        if (_lines[i].start.x != _lines[i].end.x)
                        {
                            return false;
                        }
                        if (_lines[i].end.x != _lines[i + 1].start.x)
                        {
                            return false;
                        }
                    }

                    // Last line
                    if (_lines[^1].start.x != _lines[^1].end.x)
                    {
                        return false;
                    }

                    return true;
                }
            }
            /// <summary>
            /// True iff the path never changes y coord.
            /// </summary>
            public bool isHorizontal
            {
                get
                {
                    // First and middle lines (not last)
                    for (int i = 0; i < _lines.Count - 1; i++)
                    {
                        if (_lines[i].start.y != _lines[i].end.y)
                        {
                            return false;
                        }
                        if (_lines[i].end.y != _lines[i + 1].start.y)
                        {
                            return false;
                        }
                    }

                    // Last line
                    if (_lines[^1].start.y != _lines[^1].end.y)
                    {
                        return false;
                    }

                    return true;
                }
            }

            /// <summary>
            /// Whether the path crosses itself. Does not include the end of one line being equal to the start of the next.
            /// </summary>
            public bool selfIntersects
            {
                get
                {
                    HashSet<IntVector2> visited = new HashSet<IntVector2>();
                    foreach (IntVector2 point in this)
                    {
                        if (visited.Contains(point))
                        {
                            return true;
                        }
                        visited.Add(point);
                    }
                    return false;
                }
            }
            /// <summary>
            /// Whether the path defines a simple polygon (one that doesn't self-intersect - note however that lines are allowed to go back over).
            /// </summary>
            public bool isSimplePolygon
            {
                get
                {
                    /// <summary>
                    /// Returns whether the line (in1, meetingPoint, out1) crosses the line (in2, meetingPoint, out2).
                    /// </summary>
                    static bool IsCrossingPoint(IntVector2 meetingPoint, IntVector2 in1, IntVector2 out1, IntVector2 in2, IntVector2 out2)
                    {
                        foreach (IntVector2 point in new IntVector2[] { in1, out1, in2, out2 })
                        {
                            if (IntVector2.SupDistance(point, meetingPoint) > 1)
                            {
                                throw new ArgumentException("The points do not form connected lines.");
                            }
                        }

                        // If any of the points are equal, the lines do not cross
                        if (new IntVector2[] { in1, out1, in2, out2 }.CountDistinct() < 4)
                        {
                            return false;
                        }

                        // Go round the border of the 3x3 square centred on meeting point. The lines cross if and only if we see in2 or out2 (but not both) strictly between in1 and out1 or
                        // vice versa.
                        // E.g.
                        //
                        // Lines cross:
                        //      1  _  2
                        //      _     1
                        //      _  2  _
                        //
                        // Lines don't cross:
                        //      1  _  _
                        //      _     1
                        //      _  2  2
                        //

                        // How many of in1 / out1 we've seen
                        int count1 = 0;
                        // How many of in2 / out2 we've seen
                        int count2 = 0;
                        // Whether the last of in1 / out1 / in2 / out2 we saw was on line 1 or line 2
                        int lastSeen = 0;
                        foreach (IntVector2 point in new Rectangle(meetingPoint + IntVector2.downLeft, meetingPoint + IntVector2.upRight, false))
                        {
                            if (point == in1 || point == out1)
                            {
                                count1++;
                                if (count1 == 2)
                                {
                                    return lastSeen == 2;
                                }
                                lastSeen = 1;
                            }
                            if (point == in2 || point == out2)
                            {
                                count2++;
                                if (count2 == 2)
                                {
                                    return lastSeen == 1;
                                }
                                lastSeen = 2;
                            }
                        }

                        throw new UnreachableException();
                    }

                    if (!isLoop)
                    {
                        return false;
                    }

                    IntVector2[] points = this.ToArray();
                    HashSet<IntVector2> visited = new HashSet<IntVector2>();
                    foreach ((IntVector2 pixel, int index) in points.Enumerate())
                    {
                        if (visited.Contains(pixel))
                        {
                            int indexOfPreviousVisit = Functions.Mod(index - 1, points.Length);
                            while (points[indexOfPreviousVisit] != pixel)
                            {
                                indexOfPreviousVisit = Functions.Mod(indexOfPreviousVisit - 1, points.Length);
                            }

                            if (IsCrossingPoint(pixel,
                                points[Functions.Mod(index - 1, points.Length)], points[Functions.Mod(index + 1, points.Length)],
                                points[Functions.Mod(indexOfPreviousVisit - 1, points.Length)], points[Functions.Mod(indexOfPreviousVisit + 1, points.Length)]
                                )
                            )
                            {
                                return false;
                            }
                        }
                        visited.Add(points[index]);
                    }
                    return true;
                }
            }

            public IntRect boundingRect => IntRect.BoundingRect(from line in _lines select line.boundingRect);

            public int Count
            {
                get
                {
                    // First line
                    int count = _lines[0].Count;
                    if (_lines.Count == 1)
                    {
                        return count;
                    }

                    // Middle lines (not first or last)
                    for (int i = 1; i < _lines.Count - 1; i++)
                    {
                        if (_lines[i - 1].end == _lines[i].start)
                        {
                            count += _lines[i].Count - 1;
                        }
                        else
                        {
                            count += _lines[i].Count;
                        }
                    }

                    // Last line
                    int start = 0;
                    int end = _lines[^1].Count;
                    if (_lines[^2].end == _lines[^1].start)
                    {
                        start++;
                    }
                    if (_lines[^1].end == _lines[0].start)
                    {
                        end--;
                    }

                    if (end >= start)
                    {
                        count += end - start;
                    }

                    return count;
                }
            }

            // To prevent creation of paths from 0 points/lines. (Those constructors have checks anyway but this stops the 'ambiguous call' error those give when trying to use an empty constructor)
            private Path() { }
            /// <summary>
            /// Creates a path through the given points.
            /// </summary>
            public Path(params IntVector2[] points) : this((IEnumerable<IntVector2>)points) { }
            /// <summary>
            /// Creates a path through the given points.
            /// </summary>
            public Path(IEnumerable<IntVector2> points)
            {
                if (points.IsEmpty())
                {
                    throw new ArgumentException("Cannot create a path from 0 points.", "points");
                }

                if (points.CountExactly(1))
                {
                    _lines.Add(new Line(points.First(), points.First()));
                }
                else
                {
                    foreach ((IntVector2 point, IntVector2 nextPoint) in points.ZipCurrentAndNext())
                    {
                        _lines.Add(new Line(point, nextPoint));
                    }
                }
            }
            /// <summary>
            /// <para>
            /// The lines must connect, meaning the end of each line must be equal to or adjacent to (including diagonally) the start of the next line.
            /// </para>
            /// <para>
            /// Creates deep copies of the lines.
            /// </para>
            /// </summary>
            public Path(params Line[] lines) : this((IEnumerable<Line>)lines) { }
            /// <summary>
            /// <para>
            /// The lines must connect, meaning the end of each line must be equal to or adjacent to (including diagonally) the start of the next line.
            /// </para>
            /// <para>
            /// Creates deep copies of the lines.
            /// </para>
            /// </summary>
            public Path(IEnumerable<Line> lines)
            {
                if (lines.IsEmpty())
                {
                    throw new ArgumentException("Cannot create a path from 0 lines.", "lines");
                }

                if (lines.CountExactly(1))
                {
                    _lines.Add(lines.First().DeepCopy());
                }
                else
                {
                    _lines.Add(lines.First().DeepCopy());
                    foreach ((Line line, Line nextLine) in lines.ZipCurrentAndNext())
                    {
                        if (IntVector2.SupDistance(line.end, nextLine.start) > 1)
                        {
                            throw new ArgumentException(line + " and " + nextLine + " do not connect.", "lines");
                        }
                        _lines.Add(nextLine.DeepCopy());
                    }
                }
            }

            /// <summary>
            /// <para>
            /// Puts the paths one after the other to create a new path. The paths must connect.
            /// </para>
            /// <para>
            /// Creates deep copies of the paths.
            /// </para>
            /// </summary>
            public static Path Concat(params Path[] paths) => Concat((IEnumerable<Path>)paths);
            /// <summary>
            /// <para>
            /// Puts the paths one after the other to create a new path. The paths must connect.
            /// </para>
            /// <para>
            /// Creates deep copies of the paths.
            /// </para>
            /// </summary>
            public static Path Concat(IEnumerable<Path> paths)
            {
                if (paths.IsEmpty())
                {
                    throw new ArgumentException("Cannot concat an empty collection of paths.", "paths");
                }

                if (paths.CountExactly(1))
                {
                    return paths.First().DeepCopy();
                }
                else
                {
                    List<Line> lines = new List<Line>();
                    lines.AddRange(paths.First()._lines);
                    foreach ((Path path, Path nextPath) in paths.ZipCurrentAndNext())
                    {
                        if (IntVector2.SupDistance(path.end, nextPath.start) > 1)
                        {
                            throw new ArgumentException(path + " and " + nextPath + " do not connect.", "paths");
                        }
                        lines.AddRange(nextPath._lines);
                    }
                    return new Path(lines);
                }
            }

            public bool Contains(IntVector2 pixel)
            {
                foreach (Line line in _lines)
                {
                    if (line.Contains(pixel))
                    {
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Returns the minimum x coord of the pixels on the path that have the given y coord.
            /// </summary>
            public int MinX(int y)
            {
                if (!boundingRect.ContainsY(y))
                {
                    throw new ArgumentOutOfRangeException("y must be within the y range of the path. y: " + y + "; path y range: [" + boundingRect.bottomLeft.y + ", " + boundingRect.topRight.y + "]");
                }
                return _lines.Where(l => l.boundingRect.ContainsY(y)).Min(l => l.MinX(y));
            }
            /// <summary>
            /// Returns the maximum x coord of the pixels on the path that have the given y coord.
            /// </summary>
            public int MaxX(int y)
            {
                if (!boundingRect.ContainsY(y))
                {
                    throw new ArgumentOutOfRangeException("y must be within the y range of the path. y: " + y + "; path y range: [" + boundingRect.bottomLeft.y + ", " + boundingRect.topRight.y + "]");
                }
                return _lines.Where(l => l.boundingRect.ContainsY(y)).Max(l => l.MaxX(y));
            }
            /// <summary>
            /// Returns the minimum y coord of the pixels on the path that have the given x coord.
            /// </summary>
            public int MinY(int x)
            {
                if (!boundingRect.ContainsX(x))
                {
                    throw new ArgumentOutOfRangeException("x must be within the x range of the path. x: " + x + "; path x range: [" + boundingRect.bottomLeft.x + ", " + boundingRect.topRight.x + "]");
                }
                return _lines.Where(l => l.boundingRect.ContainsX(x)).Min(l => l.MinY(x));
            }
            /// <summary>
            /// Returns the maximum y coord of the pixels on the path that have the given x coord.
            /// </summary>
            public int MaxY(int x)
            {
                if (!boundingRect.ContainsX(x))
                {
                    throw new ArgumentOutOfRangeException("x must be within the x range of the path. x: " + x + "; path x range: [" + boundingRect.bottomLeft.x + ", " + boundingRect.topRight.x + "]");
                }
                return _lines.Where(l => l.boundingRect.ContainsX(x)).Max(l => l.MaxY(x));
            }

            /// <summary>
            /// Translates the path by the given vector.
            /// </summary>
            public static Path operator +(Path path, IntVector2 translation) => path.Translate(translation);
            /// <summary>
            /// Translates the path by the given vector.
            /// </summary>
            public static Path operator +(IntVector2 translation, Path path) => path + translation;
            /// <summary>
            /// Translates the path by the given vector.
            /// </summary>
            public static Path operator -(Path path, IntVector2 translation) => path + (-translation);
            /// <summary>
            /// Reflects the path through the origin.
            /// </summary>
            public static Path operator -(Path path) => path.Rotate(RotationAngle._180);

            I1DShape I1DShape.Translate(IntVector2 translation) => Translate(translation);
            /// <summary>
            /// Translates the path by the given vector.
            /// </summary>
            public Path Translate(IntVector2 translation) => new Path(_lines.Select(l => l.Translate(translation)));

            I1DShape I1DShape.Flip(FlipAxis axis) => Flip(axis);
            /// <summary>
            /// Reflects the path across the given axis.
            /// </summary>
            public Path Flip(FlipAxis axis) => new Path(_lines.Select(l => l.Flip(axis)));

            I1DShape I1DShape.Rotate(RotationAngle angle) => Rotate(angle);
            /// <summary>
            /// Rotates the path by the given angle.
            /// </summary>
            public Path Rotate(RotationAngle angle) => new Path(_lines.Select(l => l.Rotate(angle)));

            private enum WindingNumberCornerType
            {
                /// <summary>
                /// At this point, the path is going downward (or horizontal between two downward sections).
                /// </summary>
                Downward = -1,
                /// <summary>
                /// On both sides of this point, the path goes down before it goes up; or on both sides of this point, the path goes up before it goes down.
                /// </summary>
                LocalYExtremum = 0,
                /// <summary>
                /// At this point, the path is going upward (or horizontal between two upward sections).
                /// </summary>
                Upward = 1
            }
            /// <summary>
            /// <para>
            /// Computes how many times the path goes round the given point. Each anticlockwise rotation adds +1; each clockwise rotation adds -1.
            /// </para>
            /// <para>
            /// Only defined for paths that are loops and for points not on the path.
            /// </para>
            /// </summary>
            public int WindingNumber(IntVector2 pixel)
            {
                if (!isLoop)
                {
                    throw new ArgumentException("Winding number is only defined for paths that are loops.");
                }
                if (Contains(pixel))
                {
                    throw new ArgumentException("Winding number is undefined for points on the path.");
                }

                if (isVertical || isHorizontal)
                {
                    return 0;
                }

                int windingNumber = 0;
                for (int i = 0; i < _lines.Count ; i++)
                {
                    Line line = _lines[i];

                    // Check if start of line is a local y extremum

                    // This starting value doesn't mean anything as it should always get overwritten in the loop
                    WindingNumberCornerType startCornerType = WindingNumberCornerType.Downward;
                    for (int index = Functions.Mod(i - 1, _lines.Count); index != i; index = Functions.Mod(index - 1, _lines.Count))
                    {
                        if (_lines[index].end.y != line.start.y)
                        {
                            if (Math.Sign(line.start.y - _lines[index].end.y) == Math.Sign(line.start.y - line.end.y))
                            {
                                startCornerType = WindingNumberCornerType.LocalYExtremum;
                            }
                            else
                            {
                                // Will be Upward or Downward
                                startCornerType = (WindingNumberCornerType)Math.Sign(line.start.y - _lines[index].end.y);
                            }
                            break;
                        }
                        else if (!_lines[index].isHorizontal)
                        {
                            if (Math.Sign(_lines[index].end.y - _lines[index].start.y) != Math.Sign(line.end.y - line.start.y))
                            {
                                startCornerType = WindingNumberCornerType.LocalYExtremum;
                            }
                            else
                            {
                                // Will be Upward or Downward
                                startCornerType = (WindingNumberCornerType)Math.Sign(_lines[index].end.y - _lines[index].start.y);
                            }
                            break;
                        }
                    }

                    // Check if end of line is a local y extremum

                    // This starting value doesn't mean anything as it should should always get overwritten in the loop
                    WindingNumberCornerType endCornerType = WindingNumberCornerType.Downward;
                    for (int index = (i + 1) % _lines.Count; index != i; index = (index + 1) % _lines.Count)
                    {
                        if (_lines[index].start.y != line.end.y)
                        {
                            if (Math.Sign(line.end.y - _lines[index].start.y) == Math.Sign(line.end.y - line.start.y))
                            {
                                endCornerType = WindingNumberCornerType.LocalYExtremum;
                            }
                            else
                            {
                                // Will be Upward or Downward
                                endCornerType = (WindingNumberCornerType)Math.Sign(_lines[index].start.y - line.end.y);
                            }
                            break;
                        }
                        else if (!_lines[index].isHorizontal)
                        {
                            if (Math.Sign(_lines[index].end.y - _lines[index].start.y) != Math.Sign(line.end.y - line.start.y))
                            {
                                endCornerType = WindingNumberCornerType.LocalYExtremum;
                            }
                            else
                            {
                                // Will be Upward or Downward
                                endCornerType = (WindingNumberCornerType)Math.Sign(_lines[index].end.y - _lines[index].start.y);
                            }
                            break;
                        }
                    }

                    // The loops above don't identify local y extrema for horizontal lines. They do calculate whether the next change in y is up or down. Using this we can deduce whether it's
                    // a local y extremum or not.
                    if (line.isHorizontal && startCornerType != endCornerType)
                    {
                        startCornerType = WindingNumberCornerType.LocalYExtremum;
                        endCornerType = WindingNumberCornerType.LocalYExtremum;
                    }

                    // Decide whether to count the start/end y coord of the line in the raycast

                    Line nextLine = _lines[(i + 1) % _lines.Count];
                    bool countStartY = startCornerType != WindingNumberCornerType.LocalYExtremum;
                    bool countEndY = endCornerType != WindingNumberCornerType.LocalYExtremum && line.end.y != nextLine.start.y;

                    if (line.PointIsToLeft(pixel))
                    {
                        if (pixel.y == line.end.y)
                        {
                            if (countEndY)
                            {
                                windingNumber += (int)endCornerType;
                            }
                        }
                        else if (pixel.y == line.start.y)
                        {
                            if (countStartY)
                            {
                                windingNumber += (int)startCornerType;
                            }
                        }
                        else
                        {
                            windingNumber += Math.Sign(line.end.y - line.start.y);
                        }
                    }
                }

                return windingNumber;
            }

            /// <summary>
            /// Iterates through the points in the path, but also returns the index of the line that pixel came from. (If a pixel appears multiple times in the path, the line index of the i-th
            /// occurrence will be the index of the i-th line it appears in. In other words, the line index is non-decreasing and can only increment in steps of 1.)
            /// </summary>
            private IEnumerable<(IntVector2 pixel, int lineIndex)> EnumerateWithLineIndex()
            {
                // This is an initial value guaranteed to not be any of the pixels on the path.
                IntVector2 previousPixel = boundingRect.bottomLeft + IntVector2.downLeft;

                // First line
                foreach (IntVector2 pixel in _lines[0])
                {
                    yield return (pixel, 0);
                }
                if (_lines.Count == 1)
                {
                    yield break;
                }
                previousPixel = _lines[0].end;

                // Middle lines (not first or last)
                for (int i = 1; i < _lines.Count - 1; i++)
                {
                    int start = 0;
                    if (_lines[i].start == previousPixel)
                    {
                        start = 1;
                    }
                    foreach (IntVector2 pixel in _lines[i][start..])
                    {
                        yield return (pixel, i);
                    }
                    previousPixel = _lines[i].end;
                }

                // Last line
                {
                    int start = 0;
                    int end = _lines[^1].Count;
                    if (_lines[^1].start == previousPixel)
                    {
                        start++;
                    }
                    if (_lines[^1].end == _lines[0].start)
                    {
                        end--;
                    }

                    if (end < start)
                    {
                        yield break;
                    }

                    foreach (IntVector2 pixel in _lines[^1][start..end])
                    {
                        yield return (pixel, _lines.Count - 1);
                    }
                }
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public IEnumerator<IntVector2> GetEnumerator() => EnumerateWithLineIndex().Select(x => x.pixel).GetEnumerator();

            public static bool operator ==(Path a, Path b)
            {
                if (a._lines.Count != b._lines.Count)
                {
                    return false;
                }

                for (int i = 0; i < a._lines.Count; i++)
                {
                    if (a._lines[i] != b._lines[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            public static bool operator !=(Path a, Path b) => !(a == b);
            public override bool Equals(object obj)
            {
                if (obj == null || !GetType().Equals(obj.GetType()))
                {
                    return false;
                }
                else
                {
                    return this == (Path)obj;
                }
            }

            public override int GetHashCode() => _lines.GetHashCode();

            public override string ToString() => "Path(" + string.Join(", ", _lines) + ")";

            I1DShape I1DShape.DeepCopy() => DeepCopy();
            public Path DeepCopy()
            {
                List<Line> copiedLines = new List<Line>(_lines.Count);
                foreach (Line line in _lines)
                {
                    copiedLines.Add(line.DeepCopy());
                }
                return new Path(copiedLines);
            }
        }

        public interface IFillableShape : IShape
        {
            /// <summary>
            /// Whether the shape has its inside filled-in, or whether it's just the border.
            /// </summary>
            public bool filled { get; set; }

            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public static IFillableShape operator +(IFillableShape shape, IntVector2 translation) => shape.Translate(translation);
            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public static IFillableShape operator +(IntVector2 translation, IFillableShape shape) => shape + translation;
            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public static IFillableShape operator -(IFillableShape shape, IntVector2 translation) => shape + (-translation);
            /// <summary>
            /// Reflects the shape through the origin.
            /// </summary>
            public static IFillableShape operator -(IFillableShape shape) => shape.Flip(FlipAxis.Vertical).Flip(FlipAxis.Horizontal);

            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public new IFillableShape Translate(IntVector2 translation);
            IShape IShape.Translate(IntVector2 translation) => Translate(translation);
            /// <summary>
            /// Reflects the shape across the given axis.
            /// </summary>
            public new IFillableShape Flip(FlipAxis axis);
            IShape IShape.Flip(FlipAxis axis) => Flip(axis);

            public new IFillableShape DeepCopy();
            IShape IShape.DeepCopy() => DeepCopy();
        }

        public interface I2DShape : IFillableShape, IRotatableShape
        {
            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public static I2DShape operator +(I2DShape shape, IntVector2 translation) => shape.Translate(translation);
            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public static I2DShape operator +(IntVector2 translation, I2DShape shape) => shape + translation;
            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public static I2DShape operator -(I2DShape shape, IntVector2 translation) => shape + (-translation);
            /// <summary>
            /// Reflects the shape through the origin.
            /// </summary>
            public static I2DShape operator -(I2DShape shape) => shape.Rotate(RotationAngle._180);

            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public new I2DShape Translate(IntVector2 translation);
            IShape IShape.Translate(IntVector2 translation) => Translate(translation);
            IFillableShape IFillableShape.Translate(IntVector2 translation) => Translate(translation);
            IRotatableShape IRotatableShape.Translate(IntVector2 translation) => Translate(translation);
            /// <summary>
            /// Reflects the shape across the given axis.
            /// </summary>
            public new I2DShape Flip(FlipAxis axis);
            IShape IShape.Flip(FlipAxis axis) => Flip(axis);
            IFillableShape IFillableShape.Flip(FlipAxis axis) => Flip(axis);
            IRotatableShape IRotatableShape.Flip(FlipAxis axis) => Flip(axis);
            /// <summary>
            /// Rotates the shape by the given angle.
            /// </summary>
            public new I2DShape Rotate(RotationAngle angle);
            IRotatableShape IRotatableShape.Rotate(RotationAngle angle) => Rotate(angle);

            public new I2DShape DeepCopy();
            IShape IShape.DeepCopy() => DeepCopy();
            IFillableShape IFillableShape.DeepCopy() => DeepCopy();
            IRotatableShape IRotatableShape.DeepCopy() => DeepCopy();
        }

        public class Rectangle : I2DShape
        {
            public IntVector2 bottomLeft
            {
                get => boundingRect.bottomLeft;
                set => boundingRect = new IntRect(value, topRight);
            }
            public IntVector2 topRight
            {
                get => boundingRect.topRight;
                set => boundingRect = new IntRect(value, bottomLeft);
            }
            public IntVector2 bottomRight
            {
                get => boundingRect.bottomRight;
                set => boundingRect = new IntRect(value, topLeft);
            }
            public IntVector2 topLeft
            {
                get => boundingRect.topLeft;
                set => boundingRect = new IntRect(value, bottomRight);
            }

            public bool filled { get; set; }

            public int width => boundingRect.width;
            public int height => boundingRect.height;

            /// <summary>True if the rectangle is a square.</summary>
            public bool isSquare => width == height;

            public IntRect boundingRect { get; private set; }

            public int Count
            {
                get
                {
                    // Filled
                    if (filled)
                    {
                        return width * height;
                    }
                    // Unfilled
                    if (width == 1)
                    {
                        return height;
                    }
                    if (height == 1)
                    {
                        return width;
                    }
                    return 2 * (width + height) - 4;
                }
            }

            public Rectangle(IntVector2 corner, IntVector2 oppositeCorner, bool filled)
            {
                boundingRect = new IntRect(corner, oppositeCorner);
                this.filled = filled;
            }

            public bool Contains(IntVector2 pixel)
            {
                if (filled)
                {
                    return boundingRect.Contains(pixel);
                }
                return boundingRect.Contains(pixel) && !(pixel > bottomLeft && pixel < topRight);
            }

            /// <summary>
            /// Translates the rectangle by the given vector.
            /// </summary>
            public static Rectangle operator +(Rectangle rectangle, IntVector2 translation) => rectangle.Translate(translation);
            /// <summary>
            /// Translates the rectangle by the given vector.
            /// </summary>
            public static Rectangle operator +(IntVector2 translation, Rectangle rectangle) => rectangle + translation;
            /// <summary>
            /// Translates the rectangle by the given vector.
            /// </summary>
            public static Rectangle operator -(Rectangle rectangle, IntVector2 translation) => rectangle + (-translation);
            /// <summary>
            /// Reflects the rectangle through the origin.
            /// </summary>
            public static Rectangle operator -(Rectangle rectangle) => new Rectangle(-rectangle.bottomLeft, -rectangle.topRight, rectangle.filled);

            I2DShape I2DShape.Translate(IntVector2 translation) => Translate(translation);
            /// <summary>
            /// Translates the rectangle by the given vector.
            /// </summary>
            public Rectangle Translate(IntVector2 translation) => new Rectangle(bottomLeft + translation, topRight + translation, filled);

            I2DShape I2DShape.Flip(FlipAxis axis) => Flip(axis);
            /// <summary>
            /// Reflects the rectangle across the given axis.
            /// </summary>
            public Rectangle Flip(FlipAxis axis) => new Rectangle(bottomLeft.Flip(axis), topRight.Flip(axis), filled);

            I2DShape I2DShape.Rotate(RotationAngle angle) => Rotate(angle);
            /// <summary>
            /// Rotates the rectangle by the given angle.
            /// </summary>
            public Rectangle Rotate(RotationAngle angle) => new Rectangle(bottomLeft.Rotate(angle), topRight.Rotate(angle), filled);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public IEnumerator<IntVector2> GetEnumerator()
            {
                // Filled - start with the bottom row, read left to right, then the next row, etc.
                if (filled)
                {
                    for (int y = bottomLeft.y; y <= topRight.y; y++)
                    {
                        for (int x = bottomLeft.x; x <= topRight.x; x++)
                        {
                            yield return new IntVector2(x, y);
                        }
                    }
                    yield break;
                }

                // Unfilled - go clockwise, starting at bottom-left

                // Up the left side
                for (int y = bottomLeft.y; y <= topRight.y; y++)
                {
                    yield return new IntVector2(bottomLeft.x, y);
                }

                // Rectangle has width 1
                if (bottomLeft.x == topRight.x)
                {
                    yield break;
                }

                // Along the top side (left to right)
                for (int x = bottomLeft.x + 1; x <= topRight.x; x++)
                {
                    yield return new IntVector2(x, topRight.y);
                }

                // Rectangle has height 1
                if (bottomLeft.y == topRight.y)
                {
                    yield break;
                }

                // Down the right side
                for (int y = topRight.y - 1; y >= bottomLeft.y; y--)
                {
                    yield return new IntVector2(topRight.x, y);
                }

                // Along the bottom side (right to left)
                for (int x = topRight.x - 1; x >= bottomLeft.x + 1; x--)
                {
                    yield return new IntVector2(x, bottomLeft.y);
                }
            }

            public static bool operator ==(Rectangle a, Rectangle b) => a.boundingRect == b.boundingRect && a.filled == b.filled;
            public static bool operator !=(Rectangle a, Rectangle b) => !(a == b);
            public override bool Equals(object obj)
            {
                if (obj == null || !GetType().Equals(obj.GetType()))
                {
                    return false;
                }
                else
                {
                    return this == (Rectangle)obj;
                }
            }

            public override int GetHashCode() => HashCode.Combine(bottomLeft, topRight, filled);

            public override string ToString() => "Rectangle(" + bottomLeft + ", " + topRight + ", " + (filled ? "filled" : "unfilled") + ")";

            I2DShape I2DShape.DeepCopy() => DeepCopy();
            public Rectangle DeepCopy() => new Rectangle(bottomLeft, topRight, filled);
        }

        public class Diamond : I2DShape
        {
            public IntVector2 bottomLeft
            {
                get => boundingRect.bottomLeft;
                set => boundingRect = new IntRect(value, topRight);
            }
            public IntVector2 topRight
            {
                get => boundingRect.topRight;
                set => boundingRect = new IntRect(value, bottomLeft);
            }
            public IntVector2 bottomRight
            {
                get => boundingRect.bottomRight;
                set => boundingRect = new IntRect(value, topLeft);
            }
            public IntVector2 topLeft
            {
                get => boundingRect.topLeft;
                set => boundingRect = new IntRect(value, bottomRight);
            }

            public bool filled { get; set; }

            public int width => boundingRect.width;
            public int height => boundingRect.height;

            /// <summary>True if the diamond is a square.</summary>
            public bool isSquare => width == height;

            public IntRect boundingRect { get; private set; }

            public int Count
            {
                get
                {
                    if (width == 1 || height == 1)
                    {
                        return Math.Max(width, height);
                    }

                    var edges = this.edges;
                    if (filled)
                    {
                        int count = 0;
                        for (int y = bottomLeft.y; y <= edges.bottomLeft.boundingRect.topRight.y; y++)
                        {
                            count += edges.bottomRight.MaxX(y) - edges.bottomLeft.MinX(y) + 1;
                        }
                        // The potential + 1 for the starting y is to avoid repeating pixels
                        for (int y = edges.bottomLeft.boundingRect.topRight.y + 1; y <= topRight.y; y++)
                        {
                            count += edges.topRight.MaxX(y) - edges.topLeft.MinX(y) + 1;
                        }
                        return count;
                    }
                    else
                    {
                        if (width <= 2 || height <= 2)
                        {
                            return width * height;
                        }

                        // We exploit the symmetry of the diamond across the vertical and horizontal axes
                        // However, it's not just as simple as multiplying the count of one edge by 4, since the edges can overlap

                        int count = edges.bottomLeft.Count;
                        // Remove left-most block of the line
                        count -= edges.bottomLeft.CountOnX(bottomLeft.x) * edges.bottomLeft.CountOnY(edges.bottomLeft.boundingRect.topRight.y);
                        // Remove bottom block of the line
                        count -= edges.bottomLeft.CountOnY(bottomLeft.y) * edges.bottomLeft.CountOnX(edges.bottomLeft.boundingRect.topRight.x);
                        // Count it for the other 3 edges
                        count *= 4;
                        // Count the left-most block of the diamond (doubled to also count the right-most block)
                        count += 2 * (edges.topLeft.MaxY(bottomLeft.x) - edges.bottomLeft.MinY(bottomLeft.x) + 1) * edges.bottomLeft.CountOnY(edges.bottomLeft.boundingRect.topRight.y);
                        // Count the bottom block of the diamond (doubled to also count the top block)
                        count += 2 * (edges.bottomRight.MaxX(bottomLeft.y) - edges.bottomLeft.MinX(bottomLeft.y) + 1) * edges.bottomLeft.CountOnX(edges.bottomLeft.boundingRect.topRight.x);

                        return count;
                    }
                }
            }

            private (Line bottomLeft, Line bottomRight, Line topRight, Line topLeft) edges
            {
                get
                {
                    Line[] lines =  new Line[] {
                        // Bottom-left edge
                        new Line(topLeft + height / 2 * IntVector2.down, bottomRight + width / 2 * IntVector2.left),
                        // Bottom-right edge
                        new Line(topRight + height / 2 * IntVector2.down, bottomLeft + width / 2 * IntVector2.right),
                        // Top-right edge
                        new Line(bottomRight + height / 2 * IntVector2.up, topLeft + width / 2 * IntVector2.right),
                        // Top-left edge
                        new Line(bottomLeft + height / 2 * IntVector2.up, topRight + width / 2 * IntVector2.left)
                    };

                    // This is to ensure rotating / reflecting doesn't change the shape (up to rotating / reflecting)
                    if (width > height)
                    {
                        for (int i = 0; i < lines.Length; i++)
                        {
                            lines[i] = new Line(lines[i].end, lines[i].start);
                        }
                    }

                    // This is to make the the diamonds more aesthetic by overlapping the edges as much as possible
                    // One such effect this has is making sure diamonds that can be drawn with perfect lines are drawn as such
                    if (width > height)
                    {
                        // Not particularly efficient, but does the job and really isn't slow even when drawing diamonds with a width of 1000
                        while (lines[0].boundingRect.bottomRight.x <= lines[1].MaxX(bottomLeft.y))
                        {
                            lines[0].start += IntVector2.right;
                            lines[1].start += IntVector2.left;
                            lines[2].start += IntVector2.left;
                            lines[3].start += IntVector2.right;
                        }

                        lines[0].start += IntVector2.left;
                        lines[1].start += IntVector2.right;
                        lines[2].start += IntVector2.right;
                        lines[3].start += IntVector2.left;
                    }
                    else if (width < height)
                    {
                        while (lines[0].boundingRect.topLeft.y <= lines[3].MaxY(bottomLeft.x))
                        {
                            lines[0].start += IntVector2.up;
                            lines[1].start += IntVector2.up;
                            lines[2].start += IntVector2.down;
                            lines[3].start += IntVector2.down;
                        }

                        lines[0].start += IntVector2.down;
                        lines[1].start += IntVector2.down;
                        lines[2].start += IntVector2.up;
                        lines[3].start += IntVector2.up;
                    }

                    return (lines[0], lines[1], lines[2], lines[3]);
                }
            }

            public Diamond(IntVector2 corner, IntVector2 oppositeCorner, bool filled)
            {
                boundingRect = new IntRect(corner, oppositeCorner);
                this.filled = filled;
            }

            public bool Contains(IntVector2 pixel)
            {
                if (filled)
                {
                    var edges = this.edges;
                    return (edges.bottomLeft.PointIsToRight(pixel) && edges.bottomRight.PointIsToLeft(pixel)) || (edges.topLeft.PointIsToRight(pixel) && edges.topRight.PointIsToLeft(pixel));
                }

                foreach (Line edge in edges.AsIEnumerable())
                {
                    if (edge.Contains(pixel))
                    {
                        return true;
                    }
                }
                return false;
            }

            /// <summary>
            /// Translates the diamond by the given vector.
            /// </summary>
            public static Diamond operator +(Diamond diamond, IntVector2 translation) => diamond.Translate(translation);
            /// <summary>
            /// Translates the diamond by the given vector.
            /// </summary>
            public static Diamond operator +(IntVector2 translation, Diamond diamond) => diamond + translation;
            /// <summary>
            /// Translates the diamond by the given vector.
            /// </summary>
            public static Diamond operator -(Diamond diamond, IntVector2 translation) => diamond + (-translation);
            /// <summary>
            /// Reflects the diamond through the origin.
            /// </summary>
            public static Diamond operator -(Diamond diamond) => new Diamond(-diamond.bottomLeft, -diamond.topRight, diamond.filled);

            I2DShape I2DShape.Translate(IntVector2 translation) => Translate(translation);
            /// <summary>
            /// Translates the diamond by the given vector.
            /// </summary>
            public Diamond Translate(IntVector2 translation) => new Diamond(bottomLeft + translation, topRight + translation, filled);

            I2DShape I2DShape.Flip(FlipAxis axis) => Flip(axis);
            /// <summary>
            /// Reflects the diamond across the given axis.
            /// </summary>
            public Diamond Flip(FlipAxis axis) => new Diamond(bottomLeft.Flip(axis), topRight.Flip(axis), filled);

            I2DShape I2DShape.Rotate(RotationAngle angle) => Rotate(angle);
            /// <summary>
            /// Rotates the diamond by the given angle.
            /// </summary>
            public Diamond Rotate(RotationAngle angle) => new Diamond(bottomLeft.Rotate(angle), topRight.Rotate(angle), filled);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public IEnumerator<IntVector2> GetEnumerator()
            {
                var edges = this.edges;
                if (filled)
                {
                    for (int y = bottomLeft.y; y <= edges.bottomLeft.boundingRect.topRight.y; y++)
                    {
                        for (int x = edges.bottomLeft.MinX(y); x <= edges.bottomRight.MaxX(y); x++)
                        {
                            yield return new IntVector2(x, y);
                        }
                    }
                    // The potential + 1 for the starting y is to avoid repeating pixels
                    for (int y = edges.bottomLeft.boundingRect.topRight.y + 1; y <= topRight.y; y++)
                    {
                        for (int x = edges.topLeft.MinX(y); x <= edges.topRight.MaxX(y); x++)
                        {
                            yield return new IntVector2(x, y);
                        }
                    }
                }
                else
                {
                    // Bottom-left edge
                    foreach (IntVector2 pixel in edges.bottomLeft)
                    {
                        yield return pixel;
                    }

                    // Bottom-right edge
                    foreach (IntVector2 pixel in edges.bottomRight)
                    {
                        // This check avoids repeating pixels
                        if (pixel.x > edges.bottomLeft.boundingRect.topRight.x)
                        {
                            yield return pixel;
                        }
                    }

                    // Top-right edge
                    foreach (IntVector2 pixel in edges.topRight)
                    {
                        // This check avoids repeating pixels
                        if (pixel.y > edges.bottomRight.boundingRect.topRight.y)
                        {
                            yield return pixel;
                        }
                    }

                    // Top-left edge
                    foreach (IntVector2 pixel in edges.topLeft)
                    {
                        // This check avoids repeating pixels
                        if (pixel.x < edges.topRight.boundingRect.bottomLeft.x && pixel.y > edges.bottomLeft.boundingRect.topRight.y)
                        {
                            yield return pixel;
                        }
                    }
                }
            }

            public static bool operator ==(Diamond a, Diamond b) => a.boundingRect == b.boundingRect && a.filled == b.filled;
            public static bool operator !=(Diamond a, Diamond b) => !(a == b);
            public override bool Equals(object obj)
            {
                if (obj == null || !GetType().Equals(obj.GetType()))
                {
                    return false;
                }
                else
                {
                    return this == (Diamond)obj;
                }
            }

            public override int GetHashCode() => HashCode.Combine(bottomLeft, topRight, filled);

            public override string ToString() => "Diamond(" + bottomLeft + ", " + topRight + ", " + (filled ? "filled" : "unfilled") + ")";

            I2DShape I2DShape.DeepCopy() => DeepCopy();
            public Diamond DeepCopy() => new Diamond(bottomLeft, topRight, filled);
        }

        public class Ellipse : I2DShape
        {
            // NOTE: For this shape, we work in a coordinate system where integer coordinates refer to the CENTRE of a pixel - e.g. the centre of pixel (0, 0) is (0, 0), not (0.5, 0.5).

            public IntVector2 bottomLeft
            {
                get => boundingRect.bottomLeft;
                set => SetValues(value, topRight);
            }
            public IntVector2 topRight
            {
                get => boundingRect.topRight;
                set => SetValues(value, bottomLeft);
            }
            public IntVector2 bottomRight
            {
                get => boundingRect.bottomRight;
                set => SetValues(value, topLeft);
            }
            public IntVector2 topLeft
            {
                get => boundingRect.topLeft;
                set => SetValues(value, bottomRight);
            }

            public bool filled { get; set; }

            // These values are calculated and cached when you set the size of the ellipse
            public int width { get; private set; }
            public int height { get; private set; }
            private float xRadius;
            private float yRadius;
            private Vector2 centre;

            /// <summary>True if the ellipse is a circle.</summary>
            public bool isCircle => width == height;

            public IntRect boundingRect { get; private set; }

            public int Count => ((IEnumerable<IntVector2>)this).Count();

            public Ellipse(IntVector2 corner, IntVector2 oppositeCorner, bool filled)
            {
                boundingRect = new IntRect(IntVector2.zero, IntVector2.zero);
                this.filled = filled;

                width = 0;
                height = 0;

                xRadius = 0f;
                yRadius = 0f;
                centre = Vector2.zero;

                SetValues(corner, oppositeCorner);
            }

            private void SetValues(IntVector2 corner, IntVector2 oppositeCorner)
            {
                boundingRect = new IntRect(corner, oppositeCorner);

                width = boundingRect.width;
                height = boundingRect.height;

                xRadius = width / 2f;
                yRadius = height / 2f;
                centre = ((Vector2)bottomLeft + topRight) / 2f;
            }

            /// <summary>
            /// Determines whether (x, y) is inside the filled ellipse (including the border).
            /// </summary>
            private bool IsInside(int x, int y) => IsInside(new IntVector2(x, y));
            /// <summary>
            /// Determines whether the pixel is inside the filled ellipse (including the border).
            /// </summary>
            private bool IsInside(IntVector2 pixel)
            {
                // Manually override 1xn and 2xn case (as otherwise the algorithm doesn't include the top/bottom row in the 2xn case - the 1xn case is just because we might as well include it)
                if (width <= 2)
                {
                    return boundingRect.Contains(pixel);
                }
                // Manually override nx1 and nx2 case (as otherwise the algorithm doesn't include the left/right column in the 2xn case - the 1xn case is just because we might as well include it)
                if (height <= 2)
                {
                    return boundingRect.Contains(pixel);
                }

                // Manually override 3x3 case (as otherwise the algorithm gives a 3x3 square instead of the more aesthetic 'plus sign')
                if (width == 3 && height == 3)
                {
                    IntVector2 centre = bottomLeft + IntVector2.one;
                    // This just gives the 'plus sign' shape we want
                    return IntVector2.L1Distance(pixel, centre) <= 1;
                }

                return (pixel.x - centre.x) * (pixel.x - centre.x) / (xRadius * xRadius) + (pixel.y - centre.y) * (pixel.y - centre.y) / (yRadius * yRadius) <= 1f;
            }

            public bool Contains(IntVector2 pixel)
            {
                // Filled
                if (filled)
                {
                    return IsInside(pixel);
                }

                // Unfilled

                if (!IsInside(pixel))
                {
                    return false;
                }
                if (!IsInside(pixel + IntVector2.up) || !IsInside(pixel + IntVector2.down) || !IsInside(pixel + IntVector2.left) || !IsInside(pixel + IntVector2.right))
                {
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Translates the ellipse by the given vector.
            /// </summary>
            public static Ellipse operator +(Ellipse ellipse, IntVector2 translation) => ellipse.Translate(translation);
            /// <summary>
            /// Translates the ellipse by the given vector.
            /// </summary>
            public static Ellipse operator +(IntVector2 translation, Ellipse ellipse) => ellipse + translation;
            /// <summary>
            /// Translates the ellipse by the given vector.
            /// </summary>
            public static Ellipse operator -(Ellipse ellipse, IntVector2 translation) => ellipse + (-translation);
            /// <summary>
            /// Reflects the ellipse through the origin.
            /// </summary>
            public static Ellipse operator -(Ellipse ellipse) => new Ellipse(-ellipse.bottomLeft, -ellipse.topRight, ellipse.filled);

            I2DShape I2DShape.Translate(IntVector2 translation) => Translate(translation);
            /// <summary>
            /// Translates the ellipse by the given vector.
            /// </summary>
            public Ellipse Translate(IntVector2 translation) => new Ellipse(bottomLeft + translation, topRight + translation, filled);

            I2DShape I2DShape.Flip(FlipAxis axis) => Flip(axis);
            /// <summary>
            /// Reflects the ellipse across the given axis.
            /// </summary>
            public Ellipse Flip(FlipAxis axis) => new Ellipse(bottomLeft.Flip(axis), topRight.Flip(axis), filled);

            I2DShape I2DShape.Rotate(RotationAngle angle) => Rotate(angle);
            /// <summary>
            /// Rotates the ellipse by the given angle.
            /// </summary>
            public Ellipse Rotate(RotationAngle angle) => new Ellipse(bottomLeft.Rotate(angle), topRight.Rotate(angle), filled);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public IEnumerator<IntVector2> GetEnumerator()
            {
                // Manually define horizontal/vertical lines to avoid repeating pixels
                if (width == 1)
                {
                    for (int y = bottomLeft.y; y <= topRight.y; y++)
                    {
                        yield return new IntVector2(bottomLeft.x, y);
                    }
                    yield break;
                }
                if (height == 1)
                {
                    for (int x = bottomLeft.x; x <= topRight.x; x++)
                    {
                        yield return new IntVector2(x, bottomLeft.y);
                    }
                    yield break;
                }

                // Filled
                if (filled)
                {
                    for (int y = bottomLeft.y; y <= topRight.y; y++)
                    {
                        int x = Mathf.FloorToInt(centre.x);
                        while (IsInside(x, y))
                        {
                            yield return new IntVector2(x, y);
                            x--;
                        }

                        x = Mathf.FloorToInt(centre.x) + 1;
                        while (IsInside(x, y))
                        {
                            yield return new IntVector2(x, y);
                            x++;
                        }
                    }
                    yield break;
                }

                // Unfilled

                IntVector2 primaryDirection = IntVector2.right;
                IntVector2 secondaryDirection = new IntVector2(1, -1);
                IntVector2 tertiaryDirection = IntVector2.down;
                IntVector2 start = new IntVector2(bottomLeft.x + width / 2, topRight.y);
                IntVector2 pixel = start;

                int iterations = 0;
                do
                {
                    iterations++;

                    if (IsInside(pixel + primaryDirection))
                    {
                        yield return pixel;
                        pixel += primaryDirection;
                    }
                    else if (IsInside(pixel + secondaryDirection))
                    {
                        yield return pixel;
                        pixel += secondaryDirection;
                    }
                    else if (IsInside(pixel + tertiaryDirection))
                    {
                        yield return pixel;
                        pixel += tertiaryDirection;
                    }
                    else
                    {
                        primaryDirection = primaryDirection.Rotate(RotationAngle._90);
                        secondaryDirection = secondaryDirection.Rotate(RotationAngle._90);
                        tertiaryDirection = tertiaryDirection.Rotate(RotationAngle._90);
                    }
                }
                while (pixel != start && iterations < 10_000);
            }

            public static bool operator ==(Ellipse a, Ellipse b) => a.boundingRect == b.boundingRect && a.filled == b.filled;
            public static bool operator !=(Ellipse a, Ellipse b) => !(a == b);
            public override bool Equals(object obj)
            {
                if (obj == null || !GetType().Equals(obj.GetType()))
                {
                    return false;
                }
                else
                {
                    return this == (Ellipse)obj;
                }
            }

            public override int GetHashCode() => HashCode.Combine(bottomLeft, topRight, filled);

            public override string ToString() => "Ellipse(" + bottomLeft + ", " + topRight + ", " + (filled ? "filled" : "unfilled") + ")";

            I2DShape I2DShape.DeepCopy() => DeepCopy();
            public Ellipse DeepCopy() => new Ellipse(bottomLeft, topRight, filled);
        }

        public class RightTriangle : I2DShape
        {
            public enum RightAngleLocation
            {
                Bottom = -1,
                Top = 1,
                Left = -2,
                Right = 2
            }

            private IntVector2 _bottomCorner;
            /// <summary>
            /// The lower of the two corners that don't contain the right angle. This should always be distinct from topCorner, unless the triangle is a single point.
            /// </summary>
            public IntVector2 bottomCorner
            {
                get => _bottomCorner;
                set
                {
                    if (value.y <= _topCorner.y)
                    {
                        _bottomCorner = value;
                    }
                    else
                    {
                        _bottomCorner = _topCorner;
                        _topCorner = value;
                    }
                }
            }
            private IntVector2 _topCorner;
            /// <summary>
            /// The higher of the two corners that don't contain the right angle. This should always be distinct from bottomCorner, unless the triangle is a single point.
            /// </summary>
            public IntVector2 topCorner
            {
                get => _topCorner;
                set
                {
                    if (value.y >= _bottomCorner.y)
                    {
                        _topCorner = value;
                    }
                    else
                    {
                        _topCorner = _bottomCorner;
                        _bottomCorner = value;
                    }
                }
            }
            /// <summary>
            /// The left-most of the two corners that don't contain the right angle. This should always be distinct from rightCorner, unless the triangle is a single point.
            /// </summary>
            public IntVector2 leftCorner
            {
                get => _bottomCorner.x <= _topCorner.x ? _bottomCorner : _topCorner;
                set
                {
                    if (leftCorner == _bottomCorner)
                    {
                        bottomCorner = value;
                    }
                    else
                    {
                        topCorner = value;
                    }
                }
            }
            /// <summary>
            /// The right-most of the two corners that don't contain the right angle. This should always be distinct from leftCorner, unless the triangle is a single point.
            /// </summary>
            public IntVector2 rightCorner
            {
                // We do > here and <= in leftCorner to ensure that if the triangle's corners have the same x coord then leftCorner and rightCorner are still different corners.
                get => _bottomCorner.x > _topCorner.x ? _bottomCorner : _topCorner;
                set
                {
                    if (rightCorner == _bottomCorner)
                    {
                        bottomCorner = value;
                    }
                    else
                    {
                        topCorner = value;
                    }
                }
            }
            /// <summary>
            /// The corner that contains the right angle.
            /// </summary>
            public IntVector2 rightAngleCorner
            {
                get
                {
                    switch (rightAngleLocation)
                    {
                        case RightAngleLocation.Bottom: return new IntVector2(topCorner.x, bottomCorner.y);
                        case RightAngleLocation.Top: return new IntVector2(bottomCorner.x, topCorner.y);
                        case RightAngleLocation.Left: return new IntVector2(leftCorner.x, rightCorner.y);
                        case RightAngleLocation.Right: return new IntVector2(rightCorner.x, leftCorner.y);
                        default: throw new NotImplementedException("Unknown / unimplemented RightAngleLocation: " + rightAngleLocation);
                    }
                }
            }

            public RightAngleLocation rightAngleLocation { get; set; }

            public bool filled { get; set; }

            public int width => boundingRect.width;
            public int height => boundingRect.height;

            /// <summary>True if the triangle is an isosceles triangle.</summary>
            public bool isIsosceles => width == height;

            public IntRect boundingRect => new IntRect(bottomCorner, topCorner);

            public int Count
            {
                get
                {
                    if (width == 1)
                    {
                        return height;
                    }
                    if (height == 1)
                    {
                        return width;
                    }

                    if (!filled || width == 2 || height == 2)
                    {
                        return border.Count;
                    }

                    if (width >= height)
                    {
                        // The vertical side of the triangle won't be counted in the loop
                        int count = height;
                        // Go along the hypotenuse
                        foreach (IntVector2 pixel in border.lines[0])
                        {
                            count += Math.Abs(pixel.y - rightAngleCorner.y) + 1;
                        }
                        return count;
                    }
                    else
                    {
                        // The horizontal side of the triangle won't be counted in the loop
                        int count = width;
                        // Go along the hypotenuse
                        foreach (IntVector2 pixel in border.lines[0])
                        {
                            count += Math.Abs(pixel.x - rightAngleCorner.x) + 1;
                        }
                        return count;
                    }
                }
            }

            private Path border
            {
                get
                {
                    // Single points / vertical lines / horizontal lines
                    if (bottomCorner.x == topCorner.x || bottomCorner.y == topCorner.y)
                    {
                        return new Path(new Line(bottomCorner, topCorner));
                    }

                    IntVector2 startCorner = bottomCorner;
                    IntVector2 endCorner = topCorner;
                    IntVector2 startAdjusted;
                    IntVector2 endAdjusted;

                    if (rightAngleLocation == RightAngleLocation.Bottom)
                    {
                        startAdjusted = bottomCorner + IntVector2.up;
                        endAdjusted = topCorner + (startCorner == leftCorner ? IntVector2.left : IntVector2.right);
                    }
                    else if (rightAngleLocation == RightAngleLocation.Top)
                    {
                        startAdjusted = bottomCorner + (endCorner == leftCorner ? IntVector2.left : IntVector2.right);
                        endAdjusted = topCorner + IntVector2.down;
                    }
                    else if (rightAngleLocation == RightAngleLocation.Left)
                    {
                        startAdjusted = bottomCorner + (startCorner == leftCorner ? IntVector2.right : IntVector2.up);
                        endAdjusted = topCorner + (startCorner == leftCorner ? IntVector2.down : IntVector2.right);
                    }
                    else if (rightAngleLocation == RightAngleLocation.Right)
                    {
                        startAdjusted = bottomCorner + (startCorner == leftCorner ? IntVector2.up : IntVector2.left);
                        endAdjusted = topCorner + (startCorner == leftCorner ? IntVector2.left : IntVector2.down);
                    }
                    else
                    {
                        throw new NotImplementedException("Unknown / unimplemented RightAngleLocation: " + rightAngleLocation);
                    }

                    // This is to ensure reflecting doesn't change the shape (up to reflecting)
                    // If width >= height, we draw the hypotenuse starting from the corner with the same y coord as the right angle corner
                    // If width < height, we draw the hypotenuse starting from the corner with the same x coord as the right angle corner
                    if ((width < height) != (startCorner.y != rightAngleCorner.y))
                    {
                        IntVector2 temp;

                        temp = startCorner;
                        startCorner = endCorner;
                        endCorner = temp;

                        temp = startAdjusted;
                        startAdjusted = endAdjusted;
                        endAdjusted = temp;
                    }

                    // Override shape of 2xn and nx2 triangles to be more aesthetic (otherwise they are just diamonds).
                    if (width == 2 && height == 2)
                    {
                        startAdjusted = startCorner;
                        endAdjusted = endCorner;
                    }
                    if (width == 2 || height == 2)
                    {
                        startAdjusted = endAdjusted + (startAdjusted - endAdjusted) / 2;
                    }

                    // The line order is start corner -> end corner -> right angle corner -> start corner
                    return new Path(new Line(startAdjusted, endAdjusted), new Line(endCorner, rightAngleCorner), new Line(rightAngleCorner, startCorner));
                }
            }

            public RightTriangle(IntVector2 corner, IntVector2 oppositeCorner, RightAngleLocation rightAngleLocation, bool filled)
            {
                if (corner.y <= oppositeCorner.y)
                {
                    _bottomCorner = corner;
                    _topCorner = oppositeCorner;
                }
                else
                {
                    _bottomCorner = oppositeCorner;
                    _topCorner = corner;
                }

                this.rightAngleLocation = rightAngleLocation;
                this.filled = filled;
            }

            public bool Contains(IntVector2 pixel)
            {
                if (!filled)
                {
                    return border.Contains(pixel);
                }

                // These cases are separate as the winding number is only defined for paths that are loops, but these cases don't give loops.
                // (Actually the 1x1, 1x2 and 2x1 cases don't need to be included in this, but it's easier to just include them.)
                if (width <= 2 || height <= 2)
                {
                    return border.Contains(pixel);
                }
                return border.Contains(pixel) || border.WindingNumber(pixel) != 0;
            }

            /// <summary>
            /// Translates the triangle by the given vector.
            /// </summary>
            public static RightTriangle operator +(RightTriangle triangle, IntVector2 translation) => triangle.Translate(translation);
            /// <summary>
            /// Translates the triangle by the given vector.
            /// </summary>
            public static RightTriangle operator +(IntVector2 translation, RightTriangle triangle) => triangle + translation;
            /// <summary>
            /// Translates the triangle by the given vector.
            /// </summary>
            public static RightTriangle operator -(RightTriangle triangle, IntVector2 translation) => triangle + (-translation);
            /// <summary>
            /// Reflects the triangle through the origin.
            /// </summary>
            public static RightTriangle operator -(RightTriangle triangle) => triangle.Rotate(RotationAngle._180);

            I2DShape I2DShape.Translate(IntVector2 translation) => Translate(translation);
            /// <summary>
            /// Translates the triangle by the given vector.
            /// </summary>
            public RightTriangle Translate(IntVector2 translation) => new RightTriangle(bottomCorner + translation, topCorner + translation, rightAngleLocation, filled);

            I2DShape I2DShape.Flip(FlipAxis axis) => Flip(axis);
            /// <summary>
            /// Reflects the triangle across the given axis.
            /// </summary>
            public RightTriangle Flip(FlipAxis axis)
            {
                RightAngleLocation FlipRightAngleLocation(RightAngleLocation rightAngleLocation, FlipAxis axis)
                {
                    switch (axis)
                    {
                        case FlipAxis.None: return rightAngleLocation;
                        case FlipAxis.Vertical:
                            switch (rightAngleLocation)
                            {
                                case RightAngleLocation.Top: return RightAngleLocation.Top;
                                case RightAngleLocation.Bottom: return RightAngleLocation.Bottom;
                                case RightAngleLocation.Left: return RightAngleLocation.Right;
                                case RightAngleLocation.Right: return RightAngleLocation.Left;
                                default: throw new NotImplementedException("Unknown / unimplemented RightAngleLocation: " + rightAngleLocation);
                            }
                        case FlipAxis.Horizontal:
                            switch (rightAngleLocation)
                            {
                                case RightAngleLocation.Top: return RightAngleLocation.Bottom;
                                case RightAngleLocation.Bottom: return RightAngleLocation.Top;
                                case RightAngleLocation.Left: return RightAngleLocation.Left;
                                case RightAngleLocation.Right: return RightAngleLocation.Right;
                                default: throw new NotImplementedException("Unknown / unimplemented RightAngleLocation: " + rightAngleLocation);
                            }
                        case FlipAxis._45Degrees:
                            switch (rightAngleLocation)
                            {
                                case RightAngleLocation.Top: return RightAngleLocation.Right;
                                case RightAngleLocation.Bottom: return RightAngleLocation.Left;
                                case RightAngleLocation.Left: return RightAngleLocation.Bottom;
                                case RightAngleLocation.Right: return RightAngleLocation.Top;
                                default: throw new NotImplementedException("Unknown / unimplemented RightAngleLocation: " + rightAngleLocation);
                            }
                        case FlipAxis.Minus45Degrees:
                            switch (rightAngleLocation)
                            {
                                case RightAngleLocation.Top: return RightAngleLocation.Left;
                                case RightAngleLocation.Bottom: return RightAngleLocation.Right;
                                case RightAngleLocation.Left: return RightAngleLocation.Top;
                                case RightAngleLocation.Right: return RightAngleLocation.Bottom;
                                default: throw new NotImplementedException("Unknown / unimplemented RightAngleLocation: " + rightAngleLocation);
                            }
                        default: throw new NotImplementedException("Unknown / unimplemented FlipAxis: " + axis);
                    }
                }

                return new RightTriangle(bottomCorner.Flip(axis), topCorner.Flip(axis), FlipRightAngleLocation(rightAngleLocation, axis), filled);
            }

            I2DShape I2DShape.Rotate(RotationAngle angle) => Rotate(angle);
            /// <summary>
            /// Rotates the triangle by the given angle.
            /// </summary>
            public RightTriangle Rotate(RotationAngle angle)
            {
                RightAngleLocation RotateRightAngleLocation(RightAngleLocation rightAngleLocation, RotationAngle angle)
                {
                    switch (angle)
                    {
                        case RotationAngle._0: return rightAngleLocation;
                        case RotationAngle._90:
                            switch (rightAngleLocation)
                            {
                                case RightAngleLocation.Top: return RightAngleLocation.Right;
                                case RightAngleLocation.Bottom: return RightAngleLocation.Left;
                                case RightAngleLocation.Left: return RightAngleLocation.Top;
                                case RightAngleLocation.Right: return RightAngleLocation.Bottom;
                                default: throw new NotImplementedException("Unknown / unimplemented RightAngleLocation: " + rightAngleLocation);
                            }
                        case RotationAngle._180:
                            switch (rightAngleLocation)
                            {
                                case RightAngleLocation.Top: return RightAngleLocation.Bottom;
                                case RightAngleLocation.Bottom: return RightAngleLocation.Top;
                                case RightAngleLocation.Left: return RightAngleLocation.Right;
                                case RightAngleLocation.Right: return RightAngleLocation.Left;
                                default: throw new NotImplementedException("Unknown / unimplemented RightAngleLocation: " + rightAngleLocation);
                            }
                        case RotationAngle.Minus90:
                            switch (rightAngleLocation)
                            {
                                case RightAngleLocation.Top: return RightAngleLocation.Left;
                                case RightAngleLocation.Bottom: return RightAngleLocation.Right;
                                case RightAngleLocation.Left: return RightAngleLocation.Bottom;
                                case RightAngleLocation.Right: return RightAngleLocation.Top;
                                default: throw new NotImplementedException("Unknown / unimplemented RightAngleLocation: " + rightAngleLocation);
                            }
                        default: throw new NotImplementedException("Unknown / unimplemented RotationAngle: " + angle);
                    }
                }

                return new RightTriangle(bottomCorner.Rotate(angle), topCorner.Rotate(angle), RotateRightAngleLocation(rightAngleLocation, angle), filled);
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public IEnumerator<IntVector2> GetEnumerator()
            {
                Path border = this.border;
                foreach (IntVector2 pixel in border)
                {
                    yield return pixel;
                }

                if (!filled || width == 2 || height == 2)
                {
                    yield break;
                }

                // either IntVector2.up, IntVector2.left or IntVector2.right
                IntVector2 directionToTopCorner = IntVector2.Simplify(topCorner - rightAngleCorner);
                // either IntVector2.down, IntVector2.left or IntVector2.right
                IntVector2 directionToBottomCorner = IntVector2.Simplify(bottomCorner - rightAngleCorner);

                for (IntVector2 rowStart = rightAngleCorner + directionToTopCorner + directionToBottomCorner; !border.Contains(rowStart); rowStart += directionToTopCorner)
                {
                    for (IntVector2 pixel = rowStart; !border.Contains(pixel); pixel += directionToBottomCorner)
                    {
                        yield return pixel;
                    }
                }
            }

            public static bool operator ==(RightTriangle a, RightTriangle b)
            {
                return a.bottomCorner == b.bottomCorner && a.topCorner == b.topCorner && a.rightAngleCorner == b.rightAngleCorner && a.filled == b.filled;
            }
            public static bool operator !=(RightTriangle a, RightTriangle b) => !(a == b);
            public override bool Equals(object obj)
            {
                if (obj == null || !GetType().Equals(obj.GetType()))
                {
                    return false;
                }
                else
                {
                    return this == (RightTriangle)obj;
                }
            }

            public override int GetHashCode() => HashCode.Combine(bottomCorner, topCorner, rightAngleLocation, filled);

            public override string ToString() => "RightTriangle(" + bottomCorner + ", " + topCorner + ", " + rightAngleLocation + ", " + (filled ? "filled" : "unfilled") + ")";

            I2DShape I2DShape.DeepCopy() => DeepCopy();
            public RightTriangle DeepCopy() => new RightTriangle(bottomCorner, topCorner, rightAngleLocation, filled);
        }

        /// <summary>
        /// Either changes the end coord's x or changes its y so that the rect it forms with the start coord is a square. Chooses the largest such square.
        /// </summary>
        public static IntVector2 SnapEndCoordToSquare(IntVector2 start, IntVector2 end)
        {
            int sideLength = Math.Max(Math.Abs(end.x - start.x), Mathf.Abs(end.y - start.y));
            return start + new IntVector2(sideLength * Math.Sign(end.x - start.x), sideLength * Math.Sign(end.y - start.y));
        }

        public interface IIsometricShape : IFillableShape
        {
            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public static IIsometricShape operator +(IIsometricShape shape, IntVector2 translation) => shape.Translate(translation);
            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public static IIsometricShape operator +(IntVector2 translation, IIsometricShape shape) => shape + translation;
            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public static IIsometricShape operator -(IIsometricShape shape, IntVector2 translation) => shape + (-translation);
            /// <summary>
            /// Reflects the shape through the origin.
            /// </summary>
            public static IIsometricShape operator -(IIsometricShape shape) => shape.Flip(FlipAxis.Vertical).Flip(FlipAxis.Horizontal);

            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public new IIsometricShape Translate(IntVector2 translation);
            IFillableShape IFillableShape.Translate(IntVector2 translation) => Translate(translation);
            /// <summary>
            /// Reflects the shape across the given axis.
            /// </summary>
            public new IIsometricShape Flip(FlipAxis axis);
            IFillableShape IFillableShape.Flip(FlipAxis axis) => Flip(axis);

            public new IIsometricShape DeepCopy();
            IFillableShape IFillableShape.DeepCopy() => DeepCopy();
        }

        public class IsometricRectangle : IIsometricShape
        {
            private IntVector2 startCorner;
            private IntVector2 endCorner;

            private IntVector2[] inferredCorners;

            public IntVector2 leftCorner
            {
                get => inferredCorners.ArgMin(p => p.x);
                set
                {
                    startCorner = rightCorner;
                    endCorner = leftCorner;
                    InferCornersAndBorder();
                }
            }
            public IntVector2 rightCorner
            {
                get => inferredCorners.ArgMax(p => p.x);
                set
                {
                    startCorner = leftCorner;
                    endCorner = rightCorner;
                    InferCornersAndBorder();
                }
            }
            public IntVector2 bottomCorner
            {
                get => inferredCorners.ArgMin(p => p.y);
                set
                {
                    startCorner = topCorner;
                    endCorner = bottomCorner;
                    InferCornersAndBorder();
                }
            }
            public IntVector2 topCorner
            {
                get => inferredCorners.ArgMax(p => p.y);
                set
                {
                    startCorner = bottomCorner;
                    endCorner = topCorner;
                    InferCornersAndBorder();
                }
            }

            public Path border => Path.Concat(lowerBorder, upperBorder);
            /// <summary>
            /// The lower edges of the border, i.e. (leftCorner, bottomCorner) and (rightCorner, bottomCorner).
            /// </summary>
            public Path lowerBorder { get; private set; }
            /// <summary>
            /// The upper edges of the border, i.e. (leftCorner, topCorner) and (rightCorner, topCorner).
            /// </summary>
            public Path upperBorder { get; private set; }

            public bool filled { get; set; }

            public int width => boundingRect.width;
            public int height => boundingRect.height;

            /// <summary>True if the isometric rectangle is an isometric square.</summary>
            public bool isIsometricSquare => leftCorner.y == rightCorner.y || topCorner.x == bottomCorner.x;

            public IntRect boundingRect => new IntRect(new IntVector2(leftCorner.x, bottomCorner.y), new IntVector2(rightCorner.x, topCorner.y));

            public int Count
            {
                get
                {
                    if (!filled)
                    {
                        return border.Count;
                    }
                    throw new NotImplementedException();
                }
            }

            public IsometricRectangle(IntVector2 startCorner, IntVector2 endCorner, bool filled)
            {
                this.startCorner = startCorner;
                this.endCorner = endCorner;
                this.filled = filled;

                InferCornersAndBorder();
            }

            /// <summary>
            /// Using just the start and end corners, deduces the other two corners and the shape of the border.
            /// </summary>
            private void InferCornersAndBorder()
            {
                // Specific cases

                if (startCorner == endCorner)
                {
                    inferredCorners = new IntVector2[] { startCorner };
                    lowerBorder = upperBorder = new Path(startCorner);
                    return;
                }
                if (Math.Abs(startCorner.y - endCorner.y) == 1)
                {
                    if (startCorner.x == endCorner.x)
                    {
                        IntRect rect = new IntRect(startCorner, endCorner);
                        IntVector2 leftCorner = rect.bottomLeft + IntVector2.left;
                        IntVector2 rightCorner = rect.topRight + IntVector2.right;
                        inferredCorners = new IntVector2[] { startCorner, endCorner, leftCorner, rightCorner };
                        lowerBorder = new Path(leftCorner, rightCorner + IntVector2.down);
                        upperBorder = new Path(rightCorner, leftCorner + IntVector2.up);
                        return;
                    }
                    else if (Math.Abs(startCorner.x - endCorner.x) == 1)
                    {
                        IntRect rect = new IntRect(startCorner, endCorner);
                        IntVector2 leftCorner = rect.bottomLeft;
                        IntVector2 rightCorner = rect.topRight;
                        inferredCorners = new IntVector2[] { startCorner, endCorner, leftCorner, rightCorner };
                        lowerBorder = new Path(leftCorner, rightCorner + IntVector2.down);
                        upperBorder = new Path(rightCorner, leftCorner + IntVector2.up);
                        return;
                    }
                }

                // General cases

                IntVector2 sign = IntVector2.Sign(endCorner - startCorner);
                IntVector2 diff = endCorner - startCorner;
                IntVector2 absDiff = IntVector2.Abs(diff);
                IntVector2 adjustedEndCorner = endCorner + ((absDiff.x + 1) % 2) * new IntVector2(sign.x, 0);
                int region = Math.Abs(adjustedEndCorner.x - startCorner.x) + 1 - 2 * (Math.Abs(adjustedEndCorner.y - startCorner.y) + 1);
                // If startCorner and endCorner are on a line with gradient +/- 1/2
                if (region == 0)
                {
                    inferredCorners = new IntVector2[] { startCorner, endCorner };
                    if ((absDiff.x + 1) % 2 == 0)
                    {
                        lowerBorder = new Path(startCorner, endCorner);
                        upperBorder = lowerBorder.reverse;
                    }
                    else
                    {
                        lowerBorder = new Path(new Line(startCorner, endCorner - sign), new Line(endCorner, endCorner));
                        upperBorder = lowerBorder.reverse;
                    }
                    return;
                }

                // In comments below, '= 2 horizontal' etc signifies the size of the block (in the border lines) that corner is in
                // E.g. 'start = 2 horizontal' indicates that startCorner will be in a horizontal block of 2 pixels in the border
                //
                // This value completely determines these block sizes / directions
                int shapeType = (absDiff.x + 2 * absDiff.y) % 4;

                // If {startCorner, endCorner} = {leftCorner, rightCorner}
                if (region > 0)
                {
                    if (shapeType == 0)
                    {
                        // start = 2 horizontal, end = 1, top / bottom = 2 horizontal

                        int numBlocks = Mathf.CeilToInt((absDiff.x + 2) / 4f) - Mathf.FloorToInt(diff.y / 2f);
                        IntVector2 offset = numBlocks * new IntVector2(2 * sign.x, -1) + new IntVector2(-sign.x, 1);
                        IntVector2 bottomCorner = startCorner + offset;
                        IntVector2 topCorner = adjustedEndCorner - offset;

                        inferredCorners = new IntVector2[] { startCorner, endCorner, bottomCorner, topCorner };
                        lowerBorder = new Path(
                            new Line(startCorner, bottomCorner),
                            new Line(bottomCorner + new IntVector2(sign.x, 1), endCorner - new IntVector2(sign.x, 1)),
                            new Line(endCorner, endCorner)
                            );
                        upperBorder = new Path(
                            new Line(endCorner, endCorner),
                            new Line(endCorner + new IntVector2(-sign.x, 1), topCorner),
                            new Line(topCorner - new IntVector2(sign.x, 1), startCorner)
                            );
                    }
                    else if (shapeType == 1)
                    {
                        // start = 2 horizontal, end = 2 horizontal, top / bottom = 2 horizontal

                        int numBlocks = Mathf.CeilToInt((absDiff.x + 1) / 4f) - Mathf.FloorToInt(diff.y / 2f);
                        IntVector2 offset = numBlocks * new IntVector2(2 * sign.x, -1) + new IntVector2(-sign.x, 1);
                        IntVector2 bottomCorner = startCorner + offset;
                        IntVector2 topCorner = endCorner - offset;

                        inferredCorners = new IntVector2[] { startCorner, endCorner, bottomCorner, topCorner };
                        lowerBorder = new Path(
                            new Line(startCorner, bottomCorner),
                            new Line(bottomCorner + new IntVector2(sign.x, 1), endCorner)
                            );
                        upperBorder = new Path(
                            new Line(endCorner, topCorner),
                            new Line(topCorner - new IntVector2(sign.x, 1), startCorner)
                            );
                    }
                    else if (shapeType == 2)
                    {
                        // start = 2 horizontal, end = 2 horizontal, top / bottom = 3 horizontal

                        int numBlocks = Mathf.CeilToInt(absDiff.x / 4f) - Mathf.FloorToInt(diff.y / 2f);
                        IntVector2 offset = numBlocks * new IntVector2(2 * sign.x, -1) + new IntVector2(-sign.x, 1);
                        IntVector2 bottomCorner = startCorner + offset;
                        IntVector2 topCorner = endCorner - offset;

                        inferredCorners = new IntVector2[] { startCorner, endCorner, bottomCorner, topCorner };
                        lowerBorder = new Path(
                            new Line(startCorner, bottomCorner),
                            new Line(bottomCorner, endCorner)
                            );
                        upperBorder = new Path(
                            new Line(endCorner, topCorner),
                            new Line(topCorner, startCorner)
                            );
                    }
                    else if (shapeType == 3)
                    {
                        // start = 1, end = 1, top / bottom = 2 horizontal

                        int numBlocks = Mathf.CeilToInt((absDiff.x - 1) / 4f) - Mathf.FloorToInt(diff.y / 2f);
                        IntVector2 offset = numBlocks * new IntVector2(2 * sign.x, -1);
                        IntVector2 bottomCorner = startCorner + offset;
                        IntVector2 topCorner = adjustedEndCorner - offset;

                        inferredCorners = new IntVector2[] { startCorner, endCorner, bottomCorner, topCorner };
                        lowerBorder = new Path(
                            new Line(startCorner, startCorner),
                            new Line(startCorner + new IntVector2(sign.x, -1), bottomCorner),
                            new Line(bottomCorner + new IntVector2(sign.x, 1), endCorner - new IntVector2(sign.x, 1)),
                            new Line(endCorner, endCorner)
                            );
                        upperBorder = new Path(
                            new Line(endCorner, endCorner),
                            new Line(endCorner + new IntVector2(-sign.x, 1), topCorner),
                            new Line(topCorner - new IntVector2(sign.x, 1), startCorner + new IntVector2(sign.x, 1)),
                            new Line(startCorner, startCorner)
                            );
                    }
                    else
                    {
                        throw new UnreachableException();
                    }
                }
                // If {startCorner, endCorner} = {topCorner, bottomCorner}
                else
                {
                    if (Math.Abs(adjustedEndCorner.x - startCorner.x) + 1 == 2 * Math.Abs(adjustedEndCorner.y - startCorner.y))
                    {
                        IntVector2 corner1 = startCorner + sign.y * IntVector2.up;
                        IntVector2 corner2 = endCorner + sign.y * IntVector2.down;
                        inferredCorners = new IntVector2[] { startCorner, endCorner, corner1, corner2 };
                        if (startCorner.y < endCorner.y)
                        {
                            if ((absDiff.x + 1) % 2 == 0)
                            {
                                lowerBorder = new Path(startCorner, corner2);
                                upperBorder = new Path(endCorner, corner1);
                            }
                            else
                            {
                                lowerBorder = new Path(new Line(startCorner, corner2 - sign), new Line(corner2, corner2));
                                upperBorder = new Path(new Line(endCorner, endCorner), new Line(endCorner - sign, corner1));
                            }
                        }
                        else
                        {
                            if ((absDiff.x + 1) % 2 == 0)
                            {
                                lowerBorder = new Path(new Line(endCorner, corner1));
                                upperBorder = new Path(new Line(startCorner, corner2));
                            }
                            else
                            {
                                lowerBorder = new Path(new Line(endCorner, endCorner), new Line(endCorner - sign, corner1));
                                upperBorder = new Path(new Line(startCorner, corner2 - sign), new Line(corner2, corner2));
                            }
                        }
                        return;
                    }

                    (IntVector2 topCorner, IntVector2 bottomCorner) = (endCorner.y - startCorner.y) switch
                    {
                        > 0 => (endCorner, startCorner),
                        < 0 => (startCorner, endCorner),
                        _ => throw new UnreachableException()
                    };
                    sign = IntVector2.Sign(topCorner - bottomCorner);

                    if (shapeType == 0)
                    {
                        if (bottomCorner.x == topCorner.x)
                        {
                            // start / end = 3 horizontal, left / right = 1

                            int numBlocks = absDiff.y / 2;
                            IntVector2 offset = numBlocks * new IntVector2(2, 1);
                            IntVector2 rightCorner = bottomCorner + offset;
                            IntVector2 leftCorner = topCorner - offset;

                            inferredCorners = new IntVector2[] { bottomCorner, topCorner, rightCorner, leftCorner };
                            lowerBorder = new Path(
                                new Line(leftCorner, leftCorner),
                                new Line(leftCorner + IntVector2.downRight, bottomCorner),
                                new Line(bottomCorner, rightCorner + IntVector2.downLeft),
                                new Line(rightCorner, rightCorner)
                                );
                            upperBorder = new Path(
                                new Line(rightCorner, rightCorner),
                                new Line(rightCorner + IntVector2.upLeft, topCorner),
                                new Line(topCorner, leftCorner + IntVector2.upRight),
                                new Line(leftCorner, leftCorner)
                                );
                        }
                        else
                        {
                            // start / end = 2 horizontal, left / right = 1

                            int numBlocks = absDiff.y / 2 + Mathf.CeilToInt(absDiff.x / 4f);
                            IntVector2 offset = numBlocks * new IntVector2(2 * sign.x, 1);
                            IntVector2 corner1 = bottomCorner + offset;
                            IntVector2 corner2 = topCorner - offset;

                            inferredCorners = new IntVector2[] { bottomCorner, topCorner, corner1, corner2 };
                            lowerBorder = new Path(
                                new Line(corner2, corner2),
                                new Line(corner2 + new IntVector2(sign.x, -1), bottomCorner),
                                new Line(bottomCorner, corner1 - new IntVector2(sign.x, 1)),
                                new Line(corner1, corner1)
                                );
                            upperBorder = new Path(
                                new Line(corner1, corner1),
                                new Line(corner1 - new IntVector2(sign.x, -1), topCorner),
                                new Line(topCorner, corner2 + new IntVector2(sign.x, 1)),
                                new Line(corner2, corner2)
                                );
                        }
                    }
                    else if (shapeType == 1)
                    {
                        // start / end = 2 horizontal, left / right = 1

                        int numBlocks = absDiff.y / 2 + Mathf.CeilToInt((absDiff.x - 1) / 4f);
                        IntVector2 offset = numBlocks * new IntVector2(2 * sign.x, 1);
                        IntVector2 corner1 = bottomCorner + offset;
                        IntVector2 corner2 = topCorner - offset;

                        inferredCorners = new IntVector2[] { bottomCorner, topCorner, corner1, corner2 };
                        lowerBorder = new Path(
                            new Line(corner2, corner2),
                            new Line(corner2 + new IntVector2(sign.x, -1), bottomCorner + new IntVector2(-sign.x, 1)),
                            new Line(bottomCorner, corner1 - new IntVector2(sign.x, 1)),
                            new Line(corner1, corner1)
                            );
                        upperBorder = new Path(
                            new Line(corner1, corner1),
                            new Line(corner1 - new IntVector2(sign.x, -1), topCorner - new IntVector2(-sign.x, 1)),
                            new Line(topCorner, corner2 + new IntVector2(sign.x, 1)),
                            new Line(corner2, corner2)
                            );
                    }
                    else if (shapeType == 2)
                    {
                        if (bottomCorner.x == topCorner.x)
                        {
                            // start / end = 3 horizontal, left / right = 2 vertical

                            int numBlocks = (absDiff.y - 1) / 2;
                            IntVector2 offset = numBlocks * new IntVector2(2, 1);
                            // On the y level closer to bottomCorner
                            IntVector2 rightCorner = bottomCorner + offset;
                            // On the y level closer to topCorner
                            IntVector2 leftCorner = topCorner - offset;

                            inferredCorners = new IntVector2[] { bottomCorner, topCorner, rightCorner, leftCorner };
                            lowerBorder = new Path(
                                new Line(leftCorner + IntVector2.down, leftCorner + IntVector2.down),
                                new Line(leftCorner + new IntVector2(1, -2 * 1), bottomCorner),
                                new Line(bottomCorner, rightCorner + IntVector2.downLeft),
                                new Line(rightCorner, rightCorner)
                                );
                            upperBorder = new Path(
                                new Line(rightCorner + IntVector2.up, rightCorner + IntVector2.up),
                                new Line(rightCorner - new IntVector2(1, -2 * 1), topCorner),
                                new Line(topCorner, leftCorner + IntVector2.upRight),
                                new Line(leftCorner, leftCorner)
                                );
                        }
                        else
                        {
                            // start / end = 2 horizontal, left / right = 2 vertical

                            int numBlocks = (absDiff.y - 1) / 2 + Mathf.CeilToInt(absDiff.x / 4f);
                            IntVector2 offset = numBlocks * new IntVector2(2 * sign.x, 1);
                            // On the y level closer to bottomCorner
                            IntVector2 corner1 = bottomCorner + offset;
                            // On the y level closer to topCorner
                            IntVector2 corner2 = topCorner - offset;

                            inferredCorners = new IntVector2[] { bottomCorner, topCorner, corner1, corner2 };
                            lowerBorder = new Path(
                                new Line(corner2 + IntVector2.down, corner2 + IntVector2.down),
                                new Line(corner2 + new IntVector2(sign.x, -2 * 1), bottomCorner),
                                new Line(bottomCorner, corner1 - new IntVector2(sign.x, 1)),
                                new Line(corner1, corner1)
                                );
                            upperBorder = new Path(
                                new Line(corner1 + IntVector2.up, corner1 + IntVector2.up),
                                new Line(corner1 - new IntVector2(sign.x, -2 * 1), topCorner),
                                new Line(topCorner, corner2 + new IntVector2(sign.x, 1)),
                                new Line(corner2, corner2)
                                );
                        }
                    }
                    else if (shapeType == 3)
                    {
                        // start / end = 2 horizontal, left / right = 2 vertical

                        int numBlocks = (absDiff.y - 1) / 2 + Mathf.CeilToInt((absDiff.x - 1) / 4f);
                        IntVector2 offset = numBlocks * new IntVector2(2 * sign.x, 1);
                        // On the y level closer to bottomCorner
                        IntVector2 corner1 = bottomCorner + offset;
                        // On the y level closer to topCorner
                        IntVector2 corner2 = topCorner - offset;

                        inferredCorners = new IntVector2[] { bottomCorner, topCorner, corner1, corner2 };
                        lowerBorder = new Path(
                            new Line(corner2 + IntVector2.down, corner2 + IntVector2.down),
                            new Line(corner2 + new IntVector2(sign.x, -2 * 1), bottomCorner + new IntVector2(-sign.x, 1)),
                            new Line(bottomCorner, corner1 - new IntVector2(sign.x, 1)),
                            new Line(corner1, corner1)
                            );
                        upperBorder = new Path(
                            new Line(corner1 + IntVector2.up, corner1 + IntVector2.up),
                            new Line(corner1 - new IntVector2(sign.x, -2 * 1), topCorner - new IntVector2(-sign.x, 1)),
                            new Line(topCorner, corner2 + new IntVector2(sign.x, 1)),
                            new Line(corner2, corner2)
                            );
                    }
                }
            }

            public bool Contains(IntVector2 pixel)
            {
                if (!filled)
                {
                    return border.Contains(pixel);
                }
                return border.Contains(pixel) || border.WindingNumber(pixel) != 0;
            }

            /// <summary>
            /// Translates the isometric rectangle by the given vector.
            /// </summary>
            public static IsometricRectangle operator +(IsometricRectangle isoRectangle, IntVector2 translation) => isoRectangle.Translate(translation);
            /// <summary>
            /// Translates the isometric rectangle by the given vector.
            /// </summary>
            public static IsometricRectangle operator +(IntVector2 translation, IsometricRectangle isoRectangle) => isoRectangle + translation;
            /// <summary>
            /// Translates the isometric rectangle by the given vector.
            /// </summary>
            public static IsometricRectangle operator -(IsometricRectangle isoRectangle, IntVector2 translation) => isoRectangle + (-translation);
            /// <summary>
            /// Reflects the isometric rectangle through the origin.
            /// </summary>
            public static IsometricRectangle operator -(IsometricRectangle isoRectangle) => new IsometricRectangle(-isoRectangle.startCorner, -isoRectangle.endCorner, isoRectangle.filled);

            IIsometricShape IIsometricShape.Translate(IntVector2 translation) => Translate(translation);
            /// <summary>
            /// Translates the isometric rectangle by the given vector.
            /// </summary>
            public IsometricRectangle Translate(IntVector2 translation) => new IsometricRectangle(startCorner + translation, endCorner + translation, filled);

            IIsometricShape IIsometricShape.Flip(FlipAxis axis) => Flip(axis);
            /// <summary>
            /// Reflects the isometric rectangle across the given axis.
            /// </summary>
            public IsometricRectangle Flip(FlipAxis axis) => new IsometricRectangle(startCorner.Flip(axis), endCorner.Flip(axis), filled);

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
            public IEnumerator<IntVector2> GetEnumerator()
            {
                if (!filled)
                {
                    foreach (IntVector2 pixel in border)
                    {
                        yield return pixel;
                    }
                    yield break;
                }

                for (int y = boundingRect.bottomLeft.y; y <= boundingRect.topRight.y; y++)
                {
                    for (int x = border.MinX(y); x <= border.MaxX(y); x++)
                    {
                        yield return new IntVector2(x, y);
                    }
                }
            }

            public static bool operator ==(IsometricRectangle a, IsometricRectangle b) => a.startCorner == b.startCorner && a.endCorner == b.endCorner && a.filled == b.filled;
            public static bool operator !=(IsometricRectangle a, IsometricRectangle b) => !(a == b);
            public override bool Equals(object obj)
            {
                if (obj == null || !GetType().Equals(obj.GetType()))
                {
                    return false;
                }
                else
                {
                    return this == (IsometricRectangle)obj;
                }
            }

            public override int GetHashCode() => HashCode.Combine(startCorner, endCorner, filled);

            public override string ToString() => "IsometricRectangle(" + startCorner + ", " + endCorner + ", " + (filled ? "filled" : "unfilled") + ")";

            IIsometricShape IIsometricShape.DeepCopy() => DeepCopy();
            public IsometricRectangle DeepCopy() => new IsometricRectangle(startCorner, endCorner, filled);
        }

        public static Texture2D IsoRectangle(int texWidth, int texHeight, IntVector2 start, IntVector2 end, Color colour, bool filled)
        {
            return IsoRectangleOnExistingTex(Tex2DSprite.BlankTexture(texWidth, texHeight), texWidth, texHeight, start, end, colour, filled, true);
        }
        /// <summary>
        /// Draws the isometric rectangle on the given texture.
        /// </summary>
        private static Texture2D IsoRectangleOnExistingTex(Texture2D texture, int texWidth, int texHeight, IntVector2 start, IntVector2 end, Color colour, bool filled, bool drawTopLines)
        {
            return IsoRectangleOnExistingTexReturnCorners(texture, texWidth, texHeight, start, end, colour, filled, drawTopLines).Item1;
        }
        /// <summary>
        /// Draws the isometric rectangle on the given texture and returns the corners in the given order: left, top, right, bottom.
        /// </summary>
        private static Tuple<Texture2D, IntVector2, IntVector2, IntVector2, IntVector2> IsoRectangleOnExistingTexReturnCorners(Texture2D texture, int texWidth, int texHeight, IntVector2 start, IntVector2 end, Color colour, bool filled, bool drawTopLines)
        {
            IntRect texRect = new IntRect(IntVector2.zero, new IntVector2(texWidth - 1, texHeight - 1));
            IsometricRectangle rectangle = new IsometricRectangle(start, end, filled);
            foreach (IntVector2 pixel in (drawTopLines ? (IEnumerable<IntVector2>)rectangle : rectangle.lowerBorder))
            {
                if (texRect.Contains(pixel))
                {
                    texture.SetPixel(pixel, colour);
                }
            }
            texture.Apply();
            return (texture, rectangle.leftCorner, rectangle.topCorner, rectangle.rightCorner, rectangle.bottomCorner).ToTuple();
        }

        public static Texture2D IsoBox(int texWidth, int texHeight, IntVector2 baseStart, IntVector2 baseEnd, IntVector2 heightEnd, Color colour, bool filled)
        {
            Texture2D tex = Tex2DSprite.BlankTexture(texWidth, texHeight);
            IntRect rect = new IntRect(IntVector2.zero, new IntVector2(texWidth - 1, texHeight - 1));

            IntVector2 offset = new IntVector2(0, heightEnd.y - baseEnd.y);
            Tuple<Texture2D, IntVector2, IntVector2, IntVector2, IntVector2> texAndCorners = IsoRectangleOnExistingTexReturnCorners(tex, texWidth, texHeight, baseStart, baseEnd, colour, false, !(filled && offset.y > 0f));
            tex = texAndCorners.Item1;

            IntVector2 left = texAndCorners.Item2;
            IntVector2 top = texAndCorners.Item3;
            IntVector2 right = texAndCorners.Item4;
            IntVector2 bottom = texAndCorners.Item5;

            tex = IsoRectangleOnExistingTex(tex, texWidth, texHeight, baseStart + offset, baseEnd + offset, colour, false, !(filled && offset.y < 0f));

            if (!filled)
            {
                foreach (IntVector2 corner in new IntVector2[] { left, top, right, bottom })
                {
                    for (int y = corner.y; y != corner.y + offset.y; y += MathF.Sign(offset.y))
                    {
                        if (rect.Contains(corner.x, y))
                        {
                            tex.SetPixel(corner.x, y, colour);
                        }
                    }
                }
            }
            else
            {
                foreach (IntVector2 corner in new IntVector2[] { left, right, bottom })
                {
                    for (int y = corner.y; y != corner.y + offset.y; y += MathF.Sign(offset.y))
                    {
                        if (rect.Contains(corner.x, y))
                        {
                            tex.SetPixel(corner.x, y, colour);
                        }
                    }
                }
            }

            tex.Apply();
            return tex;
        }

        public static Texture2D Gradient(int texWidth, int texHeight, IntVector2 start, IntVector2 end, Color startColour, Color endColour, GradientMode gradientMode)
        {
            switch (gradientMode)
            {
                case GradientMode.Linear: return GradientLinear(texWidth, texHeight, start, end, startColour, endColour);
                case GradientMode.Radial: return GradientRadial(texWidth, texHeight, start, end, startColour, endColour);
                default: throw new System.Exception("Unknown / unimplemented gradient mode: " + gradientMode);
            }
        }

        public static Texture2D GradientLinear(int texWidth, int texHeight, IntVector2 start, IntVector2 end, Color startColour, Color endColour)
        {
            Texture2D tex = Tex2DSprite.BlankTexture(texWidth, texHeight);

            if (start == end)
            {
                for (int x = 0; x < texWidth; x++)
                {
                    for (int y = 0; y < texHeight; y++)
                    {
                        tex.SetPixel(x, y, startColour);
                    }
                }

                tex.Apply();
                return tex;
            }

            Vector2 dir = end - start;
            float distance = dir.magnitude;

            for (int x = 0; x < texWidth; x++)
            {
                for (int y = 0; y < texHeight; y++)
                {
                    if (x == start.x && y == start.y)
                    {
                        tex.SetPixel(x, y, startColour);
                    }
                    else
                    {
                        Vector2 point = new Vector2(x, y);
                        float perpendicularDistance = Vector2.Distance(start, point) * Vector2.Dot(point - start, dir) /
                                                      (point - start).magnitude / dir.magnitude;

                        float scalar = Mathf.Clamp01(perpendicularDistance / distance);

                        tex.SetPixel(x, y, Color.Lerp(startColour, endColour, scalar));
                    }
                }
            }

            tex.Apply();
            return tex;
        }

        public static Texture2D GradientRadial(int texWidth, int texHeight, IntVector2 start, IntVector2 end, Color startColour, Color endColour)
        {
            Texture2D tex = Tex2DSprite.BlankTexture(texWidth, texHeight);

            if (start == end)
            {
                for (int x = 0; x < texWidth; x++)
                {
                    for (int y = 0; y < texHeight; y++)
                    {
                        tex.SetPixel(x, y, endColour);
                    }
                }

                tex.Apply();
                return tex;
            }

            float distance = (end - start).magnitude;

            for (int x = 0; x < texWidth; x++)
            {
                for (int y = 0; y < texHeight; y++)
                {
                    float distanceToPoint = Vector2.Distance(start, new Vector2(x, y));

                    float scalar = Mathf.Clamp01(distanceToPoint / distance);

                    tex.SetPixel(x, y, Color.Lerp(startColour, endColour, scalar));
                }
            }

            tex.Apply();
            return tex;
        }
    }
}