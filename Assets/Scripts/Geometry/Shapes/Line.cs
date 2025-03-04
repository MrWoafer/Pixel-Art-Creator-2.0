using System;
using System.Collections;
using System.Collections.Generic;

using PAC.DataStructures;
using PAC.Extensions;
using PAC.Geometry.Axes;
using PAC.Geometry.Shapes.Interfaces;

using UnityEngine;

namespace PAC.Geometry.Shapes
{
    /// <summary>
    /// A pixel art straight line.
    /// <example>
    /// For example,
    /// <code>
    ///             #
    ///         # #
    ///       #
    ///   # #
    /// #
    /// </code>
    /// </example>
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Shape:</b>
    /// </para>
    /// The <see cref="Line"/> will never contain 'right-angles':
    /// <code>
    ///   #
    /// # #
    /// </code>
    /// <para>
    /// The <see cref="Line"/> will always be 'perfect' if possible. By 'perfect', we mean it is drawn using blocks of a constant size.
    /// <example>
    /// For example,
    /// <code>
    ///             # #
    ///         # #
    ///     # #
    /// # #
    /// </code>
    /// </example>
    /// </para>
    /// <para>
    /// The <see cref="Line"/> will always have 180-degree rotational symmetry, except potentially the midpoint in some odd-length <see cref="Line"/>s.
    /// </para>
    /// <para>
    /// Rotating the end point by a quadrantal angle about the start point, or reflecting it across a vertical/horizontal axis through the start point, will apply the same transformation to the
    /// shape of the <see cref="Line"/>.
    /// <example>
    /// For example, with
    /// <code>
    ///         # &lt; end
    ///       #
    ///   # #
    /// #
    /// ^start
    /// </code>
    /// rotating the end point 180 degrees about the start point gives the <see cref="Line"/>
    /// <code>
    ///         # &lt; start
    ///     # #
    ///   #
    /// #
    /// ^end
    /// </code>
    /// Even though the x-distance and y-distance between the start and end haven't changed between the two <see cref="Line"/>s, it matters which endpoint is the start and which is the end.
    /// </example>
    /// </para>
    /// <para>
    /// <b>Enumerator:</b>
    /// </para>
    /// <para>
    /// The enumerator starts at <see cref="start"/> and ends at <see cref="end"/>.
    /// </para>
    /// <para>
    /// The enumerator does not repeat any points.
    /// </para>
    /// </remarks>
    public class Line : I1DShape<Line>, IDeepCopyableShape<Line>, IReadOnlyList<IntVector2>, IEquatable<Line>
    {
        /* How this shape works:
         * 
         * For this explanation we assume the line is between +/- 45 degrees - the other case is achieved by just swapping the role of the x and y coords.
         * 
         * (Follow along on Desmos: https://www.desmos.com/calculator/ipmv1jntbh)
         * 
         *           y
         *           ^        end
         *         6 |         v
         *         5 |         #
         *         4 |
         *         3 |
         *         2 | #
         *         1 | ^start
         *       — 0 + — — — — — — > x
         *           0 1 2 3 4 5 6
         *           |
         * 
         * We imagine a real line between the start/end pixels:
         * 
         *      - If the start and end pixels have a common x or y coord (i.e. the pixel art line should be horizontal/vertical), we define the imaginary line to go through their centres.
         *      
         *      - Otherwise, We don't define the line between the centres of the start/end pixels. Instead, we define it to start at the corner of the start pixel furthest from the end pixel, and
         *        to end at the corner of the end pixel furthest from the start pixel. (This definition is made so that the algorithm draws perfect lines whenever possible.)
         *        
         * So in our example the imaginary line goes from the bottom-left corner of the start pixel to the top-right corner of the end pixel.
         * 
         * For each integer x coord the line spans, the line will pass through either 1 or 2 grid squares with this x coord. We plot a pixel at the one it spends more time in.
         * (It's easy to find which grid square it spends more time in - you just calculate the line y coord for the grid square's middle x coord and see which grid square that y lies in.)
         * 
         *           y
         *           ^        end
         *         6 |         v
         *         5 |         #
         *         4 |       #
         *         3 |   #
         *         2 | #
         *         1 | ^start
         *       — 0 + — — — — — — > x
         *           0 1 2 3 4 5 6
         *           |
         * 
         * Here we have an issue where the line spends an equal amount of time in (3, 3) and (3, 4) (i.e. the calculated y coord is on the border of the two pixels). In this case we always choose
         * the y coord closer to the endpoint x is closer to. (This ensures rotational symmetry except potentially at the midpoint of the line). If x is the exactly halfway between the start and
         * end, we choose the y coord closer to the start point. (This ensures that rotating the end point by a quadrantal angle about the start point, or reflecting it across a vertical/horizontal
         * axis through the start point, will apply the same transformation to the shape.)
         * 
         *           y 
         *           ^        end
         *         6 |         v
         *         5 |         #
         *         4 |       #
         *         3 |   # #
         *         2 | #
         *         1 | ^start
         *       — 0 + — — — — — — > x
         *           0 1 2 3 4 5 6
         *           |
         * 
         * NOTE: In order to simplify calculations, we work in a coordinate system where integer coordinates refer to the CENTRE of the grid square. This means that a y coord being on the border
         * of two pixels is equivalent to it having decimal part .5.
         */

        private IntVector2 _start;
        /// <summary>
        /// The start point of the <see cref="Line"/>.
        /// </summary>
        /// <seealso cref="end"/>
        public IntVector2 start
        {
            get => _start;
            set => SetEndpoints(value, end);
        }

        private IntVector2 _end;
        /// <summary>
        /// The end point of the <see cref="Line"/>.
        /// </summary>
        /// <seealso cref="start"/>
        public IntVector2 end
        {
            get => _end;
            set => SetEndpoints(start, value);
        }

        /// <summary>
        /// The imaginary line described in the 'How this shape works' comment.
        /// </summary>
        /// <remarks>
        /// This is cached when you set the endpoints of the <see cref="Line"/>.
        /// </remarks>
        private ImaginaryLine imaginaryLine;
        /// <summary>
        /// Encapsulates the information about the imaginary line described in the 'How this shape works' comment.
        /// </summary>
        private readonly struct ImaginaryLine
        {
            private readonly (Rational x, Rational y) start;
            private readonly Rational gradient;

            /// <summary>
            /// Creates the <see cref="ImaginaryLine"/> from the start/end pixels.
            /// </summary>
            public ImaginaryLine(IntVector2 startPixel, IntVector2 endPixel)
            {
                IntVector2 sign = (endPixel - startPixel).sign;

                start = (startPixel.x - Rational.Half * sign.x, startPixel.y - Rational.Half * sign.y);
                (Rational x, Rational y) end = (endPixel.x + Rational.Half * sign.x, endPixel.y + Rational.Half * sign.y);

                gradient = (end.y - start.y) / (end.x - start.x);
            }

            /// <summary>
            /// Determines the corresponding y coord for the given x coord.
            /// </summary>
            public Rational CalculateY(Rational x) => (x - start.x) * gradient + start.y;
            /// <summary>
            /// Determines the corresponding x coord for the given y coord.
            /// </summary>
            public Rational CalculateX(Rational y) => (y - start.y) / gradient + start.x;
        }

        /// <summary>
        /// Whether the <see cref="Line"/> is more horizontal than vertical. If it's at +/- 45 degrees then it is <see langword="true"/>.
        /// </summary>
        /// <remarks>
        /// This is cached when you set the endpoints of the <see cref="Line"/>.
        /// </remarks>
        private bool isMoreHorizontal;

        /// <summary>
        /// Whether this <see cref="Line"/> is divided into blocks of a constant size. E.g. a 12x4 <see cref="Line"/> is perfect as it is drawn as 4 horizontal blocks of 3 pixels.
        /// </summary>
        public bool isPerfect => isMoreHorizontal ? (boundingRect.width % boundingRect.height == 0) : (boundingRect.height % boundingRect.width == 0);
        /// <summary>
        /// True iff <see cref="start"/> and <see cref="end"/> are equal.
        /// </summary>
        /// <seealso cref="isVertical"/>
        /// <seealso cref="isHorizontal"/>
        public bool isPoint => start == end;
        /// <summary>
        /// True iff the <see cref="Line"/> has a constant x coord.
        /// </summary>
        /// <seealso cref="isPoint"/>
        /// <seealso cref="isHorizontal"/>
        public bool isVertical => start.x == end.x;
        /// <summary>
        /// True iff the <see cref="Line"/> has a constant y coord.
        /// </summary>
        /// <seealso cref="isPoint"/>
        /// <seealso cref="isVertical"/>
        public bool isHorizontal => start.y == end.y;

        /// <summary>
        /// <para>
        /// Returns a new <see cref="Line"/> with <see cref="start"/> and <see cref="end"/> swapped.
        /// </para>
        /// <para>
        /// This will look like the original <see cref="Line"/> rotated 180 degrees, which will look the same except potentially at the midpoint of some odd-length <see cref="Line"/>s.
        /// </para>
        /// </summary>
        public Line reverse => new Line(end, start);

        /// <summary>
        /// <c><see cref="end"/> - <see cref="start"/></c>
        /// </summary>
        public IntVector2 vector => end - start;

        public IntRect boundingRect => new IntRect(start, end);

        public int Count => IntVector2.SupDistance(start, end) + 1;

        /// <summary>
        /// Creates a <see cref="Line"/> that's just a single point.
        /// </summary>
        public Line(IntVector2 point) : this(point, point) { }
        /// <summary>
        /// Creates a <see cref="Line"/> that starts at <paramref name="start"/> and ends at <paramref name="end"/>.
        /// </summary>
        /// <param name="start">See <see cref="start"/>.</param>
        /// <param name="end">See <see cref="end"/>.</param>
        public Line(IntVector2 start, IntVector2 end) => SetEndpoints(start, end);

        /// <summary>
        /// Sets <see cref="start"/> to <paramref name="start"/> and <see cref="end"/> to <paramref name="end"/>, and calculates <see cref="imaginaryLine"/>.
        /// </summary>
        private void SetEndpoints(IntVector2 start, IntVector2 end)
        {
            _start = start;
            _end = end;

            imaginaryLine = new ImaginaryLine(start, end);
            isMoreHorizontal = Math.Abs(end.y - start.y) <= Math.Abs(end.x - start.x);
        }

        /// <remarks>
        /// This is O(1).
        /// </remarks>
        public bool Contains(IntVector2 point)
        {
            int index = isMoreHorizontal ? (point.x - start.x) * Math.Sign(end.x - start.x) : (point.y - start.y) * Math.Sign(end.y - start.y);
            if (index < 0 || index >= Count)
            {
                return false;
            }
            return this[index] == point;
        }

        /// <summary>
        /// Whether the point lies to the left of / on the <see cref="Line"/> (and lies within the y range of the <see cref="Line"/>).
        /// </summary>
        /// <remarks>
        /// This is O(1).
        /// </remarks>
        /// <seealso cref="PointIsToRight(IntVector2)"/>
        /// <seealso cref="PointIsBelow(IntVector2)(IntVector2)"/>
        /// <seealso cref="PointIsAbove(IntVector2)(IntVector2)"/>
        public bool PointIsToLeft(IntVector2 point)
        {
            if (!isMoreHorizontal)
            {
                int index = (point.y - start.y) * Math.Sign(end.y - start.y);
                if (index < 0 || index >= Count)
                {
                    return false;
                }
                return point.x <= this[index].x;
            }
            else
            {
                if (!boundingRect.ContainsY(point.y) || point.x > boundingRect.maxX)
                {
                    return false;
                }
                if (point.x < boundingRect.minX)
                {
                    return true;
                }

                // pixel now lies within bounding rect

                int index = (point.x - start.x) * Math.Sign(end.x - start.x);
                if (index < 0 || index >= Count)
                {
                    return false;
                }
                bool gradientIsPositive = Math.Sign(end.x - start.x) == Math.Sign(end.y - start.y);
                return gradientIsPositive ? point.y >= this[index].y : point.y <= this[index].y;
            }
        }
        /// <summary>
        /// Whether the point lies to the right of / on the <see cref="Line"/> (and lies within the y range of the <see cref="Line"/>).
        /// </summary>
        /// <remarks>
        /// This is O(1).
        /// </remarks>
        /// <seealso cref="PointIsToLeft(IntVector2)"/>
        /// <seealso cref="PointIsBelow(IntVector2)(IntVector2)"/>
        /// <seealso cref="PointIsAbove(IntVector2)(IntVector2)"/>
        public bool PointIsToRight(IntVector2 point) => (-this).PointIsToLeft(-point);
        /// <summary>
        /// Whether the point below / on the <see cref="Line"/> (and lies within the x range of the <see cref="Line"/>).
        /// </summary>
        /// <remarks>
        /// This is O(1).
        /// </remarks>
        /// <seealso cref="PointIsToLeft(IntVector2)(IntVector2)"/>
        /// <seealso cref="PointIsToRight(IntVector2)"/>
        /// <seealso cref="PointIsAbove(IntVector2)(IntVector2)"/>
        public bool PointIsBelow(IntVector2 point) => Rotate(QuadrantalAngle.Clockwise90).PointIsToLeft(point.Rotate(QuadrantalAngle.Clockwise90));
        /// <summary>
        /// Whether the point above / on the <see cref="Line"/> (and lies within the x range of the <see cref="Line"/>).
        /// </summary>
        /// <remarks>
        /// This is O(1).
        /// </remarks>
        /// <seealso cref="PointIsToLeft(IntVector2)(IntVector2)"/>
        /// <seealso cref="PointIsToRight(IntVector2)"/>
        /// <seealso cref="PointIsBelow(IntVector2)(IntVector2)"/>
        public bool PointIsAbove(IntVector2 point) => Rotate(QuadrantalAngle.Anticlockwise90).PointIsToLeft(point.Rotate(QuadrantalAngle.Anticlockwise90));

        /// <summary>
        /// Returns the minimum x coord of the pixels on the <see cref="Line"/> that have the given y coord.
        /// </summary>
        /// <remarks>
        /// This is O(1).
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="y"/> is not in the range of y coords spanned by the <see cref="Line"/>.</exception>
        /// <seealso cref="MaxX(int)"/>
        /// <seealso cref="MinY(int)"/>
        /// <seealso cref="MaxY(int)"/>
        public int MinX(int y) => MinX_Impl(y, 'y', boundingRect.yRange);
        /// <summary>
        /// Returns the maximum x coord of the pixels on the <see cref="Line"/> that have the given y coord.
        /// </summary>
        /// <remarks>
        /// This is O(1).
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="y"/> is not in the range of y coords spanned by the <see cref="Line"/>.</exception>
        /// <seealso cref="MinX(int)"/>
        /// <seealso cref="MaxY(int)"/>
        /// <seealso cref="MinY(int)"/>
        public int MaxX(int y) => -(-this).MinX_Impl(-y, 'y', boundingRect.yRange);
        /// <summary>
        /// Returns the minimum y coord of the pixels on the <see cref="Line"/> that have the given x coord.
        /// </summary>
        /// <remarks>
        /// This is O(1).
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="x"/> is not in the range of x coords spanned by the <see cref="Line"/>.</exception>
        /// <seealso cref="MaxY(int)"/>
        /// <seealso cref="MinX(int)"/>
        /// <seealso cref="MaxX(int)"/>
        public int MinY(int x) => Rotate(QuadrantalAngle.Clockwise90).MinX_Impl(-x, 'x', boundingRect.xRange);
        /// <summary>
        /// Returns the maximum y coord of the pixels on the <see cref="Line"/> that have the given x coord.
        /// </summary>
        /// <remarks>
        /// This is O(1).
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="x"/> is not in the range of x coords spanned by the <see cref="Line"/>.</exception>
        /// <seealso cref="MinY(int)"/>
        /// <seealso cref="MaxX(int)"/>
        /// <seealso cref="MinX(int)"/>
        public int MaxY(int x) => -Rotate(QuadrantalAngle.Anticlockwise90).MinX_Impl(x, 'x', boundingRect.xRange);
        /// <summary>
        /// Computes the value of <see cref="MinX(int)"/>, but takes in extra information to allow accurate exceptions when e.g. <see cref="MinY(int)"/> delegates its logic to
        /// <see cref="MinX(int)"/>.
        /// </summary>
        /// <remarks>
        /// This is O(1).
        /// </remarks>
        private int MinX_Impl(int y, char coordName, IntRange coordRange)
        {
            if (!boundingRect.ContainsY(y))
            {
                throw new ArgumentOutOfRangeException(coordName.ToString(), $"{coordName} must be within the {coordName} range of the line. {coordName}: {y}; line {coordName} range: {coordRange}");
            }

            /* This method is just reverse-engineering the logic in the indexer */

            if (!isMoreHorizontal)
            {
                return this[(y - start.y) * Math.Sign(end.y - start.y)].x;
            }
            if (start.y == end.y)
            {
                return Math.Min(start.x, end.x);
            }

            Rational yToInverseMap = y - Rational.Half * Math.Sign(end.y - start.y) * Math.Sign(end.x - start.x);
            // The minimum value of x such that plugging it into the the line and rounding gives you the method parameter y
            Rational minXWhereYOnLineRoundsToGivenY = imaginaryLine.CalculateX(yToInverseMap);

            if (minXWhereYOnLineRoundsToGivenY.isInteger)
            {
                bool minXWhereYOnLineRoundsToGivenY_isInFirstHalfOfLine = 2 * Math.Abs(Mathf.RoundToInt(minXWhereYOnLineRoundsToGivenY) - start.x) <= Math.Abs(end.x - start.x);
                if (minXWhereYOnLineRoundsToGivenY_isInFirstHalfOfLine)
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
        /// Returns the number of pixels on the <see cref="Line"/> that have the given x coord.
        /// </summary>
        /// <remarks>
        /// This is O(1).
        /// </remarks>
        /// <seealso cref="CountOnY(int)"/>
        public int CountOnX(int x) => boundingRect.ContainsX(x) ? MaxY(x) - MinY(x) + 1 : 0;
        /// <summary>
        /// Returns the number of pixels on the <see cref="Line"/> that have the given y coord.
        /// </summary>
        /// <remarks>
        /// This is O(1).
        /// </remarks>
        /// <seealso cref="CountOnX(int)"/>
        public int CountOnY(int y) => boundingRect.ContainsY(y) ? MaxX(y) - MinX(y) + 1 : 0;

        /// <summary>
        /// Returns a deep copy of the <see cref="Line"/> translated by the given vector.
        /// </summary>
        /// <seealso cref="Translate(IntVector2)"/>
        public static Line operator +(Line line, IntVector2 translation) => line.Translate(translation);
        /// <summary>
        /// Returns a deep copy of the <see cref="Line"/> translated by the given vector.
        /// </summary>
        /// <seealso cref="Translate(IntVector2)"/>
        public static Line operator +(IntVector2 translation, Line line) => line + translation;
        /// <summary>
        /// Returns a deep copy of the <see cref="Line"/> translated by the given vector.
        /// </summary>
        /// <seealso cref="Translate(IntVector2)"/>
        public static Line operator -(Line line, IntVector2 translation) => line + (-translation);
        /// <summary>
        /// Returns a deep copy of the <see cref="Line"/> rotated 180 degrees about the origin (equivalently, reflected through the origin).
        /// </summary>
        /// <seealso cref="Rotate(QuadrantalAngle)"/>
        /// <seealso cref="Reflect(CardinalOrdinalAxis)"/>
        public static Line operator -(Line line) => new Line(-line.start, -line.end);

        public Line Translate(IntVector2 translation) => new Line(start + translation, end + translation);
        public Line Reflect(CardinalOrdinalAxis axis) => new Line(start.Reflect(axis), end.Reflect(axis));
        public Line Rotate(QuadrantalAngle angle) => new Line(start.Rotate(angle), end.Rotate(angle));

        /// <summary>
        /// Returns the point on the <see cref="Line"/> at the given index, starting at <see cref="start"/>.
        /// </summary>
        /// <remarks>
        /// This is O(1).
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is &lt; 0 or &gt;= <see cref="Count"/>.</exception>
        public IntVector2 this[int index]
        {
            get
            {
                // Delegate the more-vertical case to the more-horizontal case by considering a 90-degree rotation
                if (!isMoreHorizontal)
                {
                    // This may put a lot of pressure on the garbage collector due to how often this is called when the user draws a line
                    return Rotate(QuadrantalAngle.Clockwise90)[index].Rotate(QuadrantalAngle.Anticlockwise90);
                }

                if (index < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"{nameof(index)} cannot be negative. {nameof(index)}: {index}.");
                }
                if (index >= Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(index), $"{nameof(index)} cannot be >= {nameof(Count)}. {nameof(index)}: {index}. {nameof(Count)}: {Count}.");
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
                Rational y = imaginaryLine.CalculateY(x);

                // Deal with edge case of y being exactly on the border of two pixels
                if (y % 1 == Rational.Half)
                {
                    // We always pull the y so it's closer to the endpoint x is closer to (e.g. if x is closer to start.x than end.x then we round y up/down to whichever is closer to start.y)
                    bool xIsInFirstHalfOfLine = 2 * Math.Abs(x - start.x) <= Math.Abs(end.x - start.x);
                    if (xIsInFirstHalfOfLine)
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
        }
        public IEnumerable<IntVector2> this[Range range] => this.GetRange(range);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<IntVector2> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return this[i];
            }
        }

        /// <summary>
        /// Whether the two <see cref="Line"/>s have the same <see cref="start"/> and <see cref="end"/>.
        /// </summary>
        public static bool operator ==(Line a, Line b) => a.start == b.start && a.end == b.end;
        /// <summary>
        /// See <see cref="operator ==(Line, Line)"/>.
        /// </summary>
        public static bool operator !=(Line a, Line b) => !(a == b);
        /// <summary>
        /// See <see cref="operator ==(Line, Line)"/>.
        /// </summary>
        public bool Equals(Line other) => this == other;
        /// <summary>
        /// See <see cref="Equals(Line)"/>
        /// </summary>
        public override bool Equals(object obj) => obj is Line other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(start, end);

        public override string ToString() => $"{nameof(Line)}({start}, {end})";

        public Line DeepCopy() => new Line(start, end);
    }
}