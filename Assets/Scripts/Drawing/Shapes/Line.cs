using System;
using System.Collections;
using System.Collections.Generic;

using PAC.DataStructures;
using PAC.Extensions;
using PAC.Maths;
using PAC.Shapes.Interfaces;

using UnityEngine;

namespace PAC.Shapes
{
    /// <summary>
    /// A pixel-perfect line between two points, ordered from start to end.
    /// </summary>
    public class Line : I1DShape<Line>, IDeepCopyableShape<Line>, IReadOnlyList<IntVector2>, IEquatable<Line>
    {
        // NOTE: For this shape, we work in a coordinate system where integer coordinates refer to the CENTRE of a pixel - e.g. the centre of pixel (0, 0) is (0, 0), not (0.5, 0.5).

        private IntVector2 _start;
        public IntVector2 start
        {
            get => _start;
            set => SetEndpoints(value, end);
        }

        private IntVector2 _end;
        public IntVector2 end
        {
            get => _end;
            set => SetEndpoints(start, value);
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
        public bool isPerfect => isMoreHorizontal ? (boundingRect.width % boundingRect.height == 0) : (boundingRect.height % boundingRect.width == 0);
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

        /// <summary>
        /// end - start.
        /// </summary>
        public IntVector2 vector => end - start;

        public IntRect boundingRect => new IntRect(start, end);

        public int Count => IntVector2.SupDistance(start, end) + 1;

        /// <summary>
        /// Creates a <see cref="Line"/> that's just a single point.
        /// </summary>
        public Line(IntVector2 point) : this(point, point) { }
        public Line(IntVector2 start, IntVector2 end) => SetEndpoints(start, end);

        private void SetEndpoints(IntVector2 start, IntVector2 end)
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
                if (!boundingRect.ContainsY(pixel.y) || pixel.x > boundingRect.maxX)
                {
                    return false;
                }
                if (pixel.x < boundingRect.minX)
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
        public bool PointIsToRight(IntVector2 pixel) => (-this).PointIsToLeft(-pixel);
        /// <summary>
        /// Whether the point below / on the line (and lies within the x range of the line).
        /// </summary>
        public bool PointIsBelow(IntVector2 pixel) => Rotate(RotationAngle._90).PointIsToLeft(pixel.Rotate(RotationAngle._90));
        /// <summary>
        /// Whether the point above / on the line (and lies within the x range of the line).
        /// </summary>
        public bool PointIsAbove(IntVector2 pixel) => Rotate(RotationAngle.Minus90).PointIsToLeft(pixel.Rotate(RotationAngle.Minus90));

        /// <summary>
        /// Returns the minimum x coord of the pixels on the line that have the given y coord.
        /// </summary>
        public int MinX(int y) => MinX_Impl(y, 'y', boundingRect.yRange);
        /// <summary>
        /// Returns the maximum x coord of the pixels on the line that have the given y coord.
        /// </summary>
        public int MaxX(int y) => -(-this).MinX_Impl(-y, 'y', boundingRect.yRange);
        /// <summary>
        /// Returns the minimum y coord of the pixels on the line that have the given x coord.
        /// </summary>
        public int MinY(int x) => Rotate(RotationAngle._90).MinX_Impl(-x, 'x', boundingRect.xRange);
        /// <summary>
        /// Returns the maximum y coord of the pixels on the line that have the given x coord.
        /// </summary>
        public int MaxY(int x) => -Rotate(RotationAngle.Minus90).MinX_Impl(x, 'x', boundingRect.xRange);
        /// <summary>
        /// Computes the value of <see cref="MinX(int)"/>, but takes in extra information to allow accurate exceptions when e.g. <see cref="MinY(int)"/> delegates its logic to
        /// <see cref="MinX(int)"/>.
        /// </summary>
        public int MinX_Impl(int y, char coordName, IntRange coordRange)
        {
            if (!boundingRect.ContainsY(y))
            {
                throw new ArgumentOutOfRangeException(coordName.ToString(), $"{coordName} must be within the {coordName} range of the line. {coordName}: {y}; line {coordName} range: {coordRange}");
            }

            if (!isMoreHorizontal)
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
            if (((y - 0.5f * Math.Sign(end.y - start.y) * Math.Sign(end.x - start.x) - imaginaryStart.y) * (imaginaryEnd.x - imaginaryStart.x)).Mod(imaginaryEnd.y - imaginaryStart.y)
                == ((imaginaryEnd.y - imaginaryStart.y) / 2f).Mod(imaginaryEnd.y - imaginaryStart.y))
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
        /// Returns the number of pixels on the line that have the given x coord.
        /// </summary>
        public int CountOnX(int x) => boundingRect.ContainsX(x) ? MaxY(x) - MinY(x) + 1 : 0;
        /// <summary>
        /// Returns the number of pixels on the line that have the given y coord.
        /// </summary>
        public int CountOnY(int y) => boundingRect.ContainsY(y) ? MaxX(y) - MinX(y) + 1 : 0;

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
        public static Line operator -(Line line, IntVector2 translation) => line + -translation;
        /// <summary>
        /// Reflects the line through the origin.
        /// </summary>
        public static Line operator -(Line line) => new Line(-line.start, -line.end);

        /// <summary>
        /// Translates the line by the given vector.
        /// </summary>
        public Line Translate(IntVector2 translation) => new Line(start + translation, end + translation);

        /// <summary>
        /// Reflects the line across the given axis.
        /// </summary>
        public Line Flip(FlipAxis axis) => new Line(start.Flip(axis), end.Flip(axis));

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
                    throw new ArgumentOutOfRangeException(nameof(index), $"{nameof(index)} cannot be negative. {nameof(index)}: {index}.");
                }
                if (index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"{nameof(index)} cannot be >= {nameof(Count)}. {nameof(index)}: {index}. {nameof(Count)}: {Count}.");
                }

                return IndexUnchecked(index);
            }
        }
        private IntVector2 IndexUnchecked(int index)
        {
            // Delegate the more-vertical case to the more-horizontal case by considering a 90-degree rotation
            if (!isMoreHorizontal)
            {
                // This may put a lot of pressure on the garbage collector due to how often this is called when the user draws a line
                return Rotate(RotationAngle._90).IndexUnchecked(index).Rotate(RotationAngle.Minus90);
            }

            if (index == 0)
            {
                return start;
            }
            if (index == Count - 1)
            {
                return end;
            }

            // Index is now in the segment from start (exclusive) to end (exclusive).

            int x = start.x + index * Math.Sign(end.x - start.x);

            if (isHorizontal)
            {
                return new IntVector2(x, start.y);
            }

            // Line equation is:
            //      imaginaryGradient = (y - imaginaryStart.y) / (x - imaginaryStart.x)
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
        public IEnumerable<IntVector2> this[Range range] => this.GetRange(range);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<IntVector2> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return IndexUnchecked(i);
            }
        }

        public static bool operator ==(Line a, Line b) => a.start == b.start && a.end == b.end;
        public static bool operator !=(Line a, Line b) => !(a == b);
        public bool Equals(Line other) => this == other;
        public override bool Equals(object obj) => obj is Line other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(start, end);

        public override string ToString() => "Line(" + start + ", " + end + ")";

        public Line DeepCopy() => new Line(start, end);
    }
}