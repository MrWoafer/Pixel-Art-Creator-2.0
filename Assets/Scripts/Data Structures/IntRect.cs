using System.Collections;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System;
using PAC.Extensions;
using PAC.Interfaces;

namespace PAC.DataStructures
{
    /// <summary>
    /// A non-empty rectangular region of integer coordinates.
    /// </summary>
    /// <remarks>
    /// When we say an <see cref="IntVector2"/> is in the rect, we mean that whole grid square is in the rect.
    /// <example>
    /// For example, the rect with corners <c>(0, 0), (0, 4), (3, 0), (3, 4)</c> is a 4x5 rectangle:
    /// <code>
    /// (0, 4) -&gt; # # # # &lt;- (3, 4)
    ///           # # # #
    ///           # # # #
    ///           # # # #
    /// (0, 0) -&gt; # # # # &lt;- (3, 0)
    /// </code>
    /// </example>
    /// </remarks>
    /// <seealso cref="IntVector2"/>
    /// <seealso cref="IntRange"/>
    public readonly struct IntRect : IReadOnlyContains<IntVector2>, IEquatable<IntRect>
    {
        /// <summary>
        /// The coordinates of the bottom-left point in the rect (inclusive).
        /// </summary>
        /// <seealso cref="topRight"/>
        /// <seealso cref="bottomRight"/>
        /// <seealso cref="topLeft"/>
        public readonly IntVector2 bottomLeft;
        /// <summary>
        /// The coordinates of the top-right point in the rect (inclusive).
        /// </summary>
        /// <seealso cref="bottomLeft"/>
        /// <seealso cref="bottomRight"/>
        /// <seealso cref="topLeft"/>
        public readonly IntVector2 topRight;

        /// <summary>
        /// The coordinates of the bottom-right point in the rect (inclusive).
        /// </summary>
        /// <seealso cref="bottomLeft"/>
        /// <seealso cref="topRight"/>
        /// <seealso cref="topLeft"/>
        public IntVector2 bottomRight => new IntVector2(topRight.x, bottomLeft.y);
        /// <summary>
        /// The coordinates of the top-left point in the rect (inclusive).
        /// </summary>
        /// <seealso cref="bottomLeft"/>
        /// <seealso cref="topRight"/>
        /// <seealso cref="bottomRight"/>
        public IntVector2 topLeft => new IntVector2(bottomLeft.x, topRight.y);

        /// <summary>
        /// The number of integer points the rect covers horizontally.
        /// </summary>
        /// <seealso cref="height"/>
        public int width => topRight.x - bottomLeft.x + 1;
        /// <summary>
        /// The number of integer points the rect covers vertically.
        /// </summary>
        /// <seealso cref="width"/>
        public int height => topRight.y - bottomLeft.y + 1;

        /// <summary>
        /// The inclusive range of x coordinates in the rect.
        /// </summary>
        /// <seealso cref="yRange"/>
        public IntRange xRange => IntRange.InclIncl(bottomLeft.x, topRight.x);
        /// <summary>
        /// The inclusive range of y coordinates in the rect.
        /// </summary>
        /// <see cref="xRange"/>
        public IntRange yRange => IntRange.InclIncl(bottomLeft.y, topRight.y);

        /// <summary>
        /// The number of points in the rect.
        /// </summary>
        public int Count => width * height;

        /// <summary>
        /// Whether the rect is a square - i.e. the <see cref="width"/> and <see cref="height"/> are equal.
        /// </summary>
        /// <seealso cref="IsSquare(IntRect)"/>
        public bool isSquare => IsSquare(this);

        /// <summary>
        /// Creates an <see cref="IntRect"/> with the two given points as opposite corners (inclusive) of the rect.
        /// </summary>
        /// <remarks>
        /// <example>
        /// For example, with <c><paramref name="corner"/> = (3, 0)</c> and <c><paramref name="oppositeCorner"/> = (0, 4)</c>, the rect will have corners <c>(0, 0), (0, 4), (3, 0), (3, 4)</c>:
        /// <code>
        /// (0, 4) -&gt; # # # # &lt;- (3, 4)
        ///           # # # #
        ///           # # # #
        ///           # # # #
        /// (0, 0) -&gt; # # # # &lt;- (3, 0)
        /// </code>
        /// </example>
        /// </remarks>
        /// <param name="corner">The corner diagonally opposite <paramref name="oppositeCorner"/>.</param>
        /// <param name="oppositeCorner">The corner diagonally opposite <paramref name="corner"/>.</param>
        public IntRect(IntVector2 corner, IntVector2 oppositeCorner)
        {
            bottomLeft = new IntVector2(Math.Min(corner.x, oppositeCorner.x), Math.Min(corner.y, oppositeCorner.y));
            topRight = new IntVector2(Math.Max(corner.x, oppositeCorner.x), Math.Max(corner.y, oppositeCorner.y));
        }

        /// <summary>
        /// Creates a rect with the given ranges of x and y coordinates.
        /// </summary>
        /// <remarks>
        /// The x and y ranges must be non-empty, as <see cref="IntRect"/>s are non-empty.
        /// </remarks>
        /// <param name="xRange">The range of x coordinates.</param>
        /// <param name="yRange">The range of y coordinates.</param>
        /// <exception cref="ArgumentException"><paramref name="xRange"/> is empty and/or <paramref name="yRange"/> is empty.</exception>
        public IntRect(IntRange xRange, IntRange yRange)
        {
            switch ((xRange.isEmpty, yRange.isEmpty))
            {
                case (false, false):
                    bottomLeft = new IntVector2(xRange.minElement, yRange.minElement);
                    topRight = new IntVector2(xRange.maxElement, yRange.maxElement);
                    return;
                case (true, false): throw new ArgumentException("The given x range is empty.", nameof(xRange));
                case (false, true): throw new ArgumentException("The given y range is empty.", nameof(yRange));
                case (true, true): throw new ArgumentException("The given x range and y range are both empty.");
            }
        }

        /// <summary>
        /// Whether the two rects describe the same set of points.
        /// </summary>
        /// <remarks>
        /// This is just the default <see langword="struct"/> comparison (comparing fields).
        /// </remarks>
        /// <seealso cref="Equals(IntRect)"/>
        public static bool operator ==(IntRect a, IntRect b) => a.bottomLeft == b.bottomLeft && a.topRight == b.topRight;
        /// <summary>
        /// Whether the two rects describe different sets of points.
        /// </summary>
        /// <remarks>
        /// This is just the default <see langword="struct"/> comparison (comparing fields).
        /// </remarks>
        /// <seealso cref="Equals(IntRect)"/>
        public static bool operator !=(IntRect a, IntRect b) => !(a == b);
        /// <summary>
        /// The same as <see cref="operator ==(IntRect, IntRect)"/>.
        /// </summary>
        public bool Equals(IntRect other) => this == other;
        /// <summary>
        /// The same as <see cref="Equals(IntRect)"/>.
        /// </summary>
        public override bool Equals(object obj) => obj is IntRect other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(bottomLeft, topRight);

        /// <summary>
        /// Represents the rect as a string in the form <c>"((bottom-left x coord, bottom-left y coord), (top-right x coord, top-right y coord))"</c>.
        /// </summary>
        public override string ToString() => $"({bottomLeft}, {topRight})";

        /// <summary>
        /// Translates the rect by the given vector.
        /// </summary>
        public static IntRect operator +(IntVector2 vector, IntRect rect) => rect + vector;
        /// <summary>
        /// Translates the rect by the given vector.
        /// </summary>
        public static IntRect operator +(IntRect rect, IntVector2 vector) => new IntRect(rect.bottomLeft + vector, rect.topRight + vector);

        /// <summary>
        /// Translates the rect by the given vector.
        /// </summary>
        public static IntRect operator -(IntRect rect, IntVector2 vector) => rect + (-vector);

        // I haven't added a cast to Unity's Rect because of the lack of a convention as to where an IntVector2 refers to in a pixel (the centre of the pixel? the bottom-left of the pixel?),
        // which affects which coordinates to use for the corners of the Rect.

        // I haven't added a cast from Unity's Rect since it's more complicated than just 'new IntRect(new IntVector2(rect.xMin, rect.yMin), new IntVector2(rect.xMax, rect.yMax))'.
        // Doing that would mean casting to Rect and from Rect aren't inverses. To make that the case I also need to decide how Rects that don't lie on integer coords get rounded.
        // It would also mean that the way the top-right corners gets rounded is different from the bottom-left.

        /// <summary>
        /// Whether the rect is a square - i.e. the <see cref="width"/> and <see cref="height"/> are equal.
        /// </summary>
        /// <seealso cref="isSquare"/>
        public static bool IsSquare(IntRect rect) => rect.width == rect.height;

        /// <summary>
        /// Whether the point is in the rect.
        /// </summary>
        /// <seealso cref="Contains(int, int)"/>
        public bool Contains(IntVector2 point) => Contains(point.x, point.y);
        /// <summary>
        /// Whether the point <c>(<paramref name="x"/>, <paramref name="y"/>)</c> is in the rect.
        /// </summary>
        /// <seealso cref="Contains(IntVector2)"/>
        public bool Contains(int x, int y) => x >= bottomLeft.x && y >= bottomLeft.y && x <= topRight.x && y <= topRight.y;

        /// <summary>
        /// Whether <paramref name="other"/> is a subset of this rect.
        /// </summary>
        /// <seealso cref="IsContainedIn(IntRect)"/>
        public bool Contains(IntRect other) => bottomLeft <= other.bottomLeft && topRight >= other.topRight;
        /// <summary>
        /// Whether this rect is a subset of <paramref name="other"/>.
        /// </summary>
        /// <seealso cref="Contains(IntRect)"/>
        public bool IsContainedIn(IntRect other) => other.Contains(this);

        /// <summary>
        /// Returns whether the rect contains any points with the given x coord.
        /// </summary>
        /// <seealso cref="ContainsY(int)"/>
        public bool ContainsX(int x) => bottomLeft.x <= x && x <= topRight.x;
        /// <summary>
        /// Returns whether the rect contains any points with the given y coord.
        /// </summary>
        /// <seealso cref="ContainsX(int)"/>
        public bool ContainsY(int y) => bottomLeft.y <= y && y <= topRight.y;

        /// <summary>
        /// Whether the two rects have any points in common.
        /// </summary>
        /// <seealso cref="Overlap(IntRect, IntRect)"/>
        public bool Overlaps(IntRect rect) => Overlap(this, rect);
        /// <summary>
        /// Whether the two rects have any points in common.
        /// </summary>
        /// <seealso cref="Overlaps(IntRect)"/>
        public static bool Overlap(IntRect a, IntRect b) => a.xRange.Intersects(b.xRange) && a.yRange.Intersects(b.yRange);

        /// <summary>
        /// Returns the set intersection of the two rects.
        /// </summary>
        /// <exception cref="InvalidOperationException">This rect and <paramref name="other"/> do not intersect.</exception>
        /// <seealso cref="Intersection(IntRect, IntRect)"/>
        public IntRect Intersection(IntRect other) => Intersection(this, other);
        /// <summary>
        /// Returns the set intersection of the two rects.
        /// </summary>
        /// <exception cref="InvalidOperationException"><paramref name="a"/> and <paramref name="b"/> do not intersect.</exception>
        /// <seealso cref="Intersection(IntRect)"/>
        public static IntRect Intersection(IntRect a, IntRect b)
        {
            if (!a.Overlaps(b))
            {
                throw new InvalidOperationException($"The given rects do not intersect. (This is an issue because the intersection would be empty, but {nameof(IntRect)}s cannot be empty.)");
            }

            return new IntRect(a.xRange.Intersection(b.xRange), a.yRange.Intersection(b.yRange));
        }

        /// <summary>
        /// Returns the point in the rect that is closest to the given vector.
        /// Equivalently, it clamps the vector's x coord to be in the rect's range of x coords, and clamps the vector's y coord to be in the rect's range of y coords.
        /// </summary>
        /// <seealso cref="Math.Clamp(int, int, int)"/>
        public IntVector2 Clamp(IntVector2 vector) => new IntVector2(Math.Clamp(vector.x, bottomLeft.x, topRight.x), Math.Clamp(vector.y, bottomLeft.y, topRight.y));

        /// <summary>
        /// Translates <paramref name="toClamp"/> so it is a subset of this rect. Chooses the smallest such translation.
        /// </summary>
        /// <remarks>
        /// Note this is only possible when <paramref name="toClamp"/> is at most as wide and at most as tall as this rect.
        /// </remarks>
        /// <exception cref="ArgumentException"><paramref name="toClamp"/> is wider and/or taller than this rect.</exception>
        public IntRect ShiftClamp(IntRect toClamp)
        {
            if (toClamp.width > width)
            {
                throw new ArgumentException("Cannot shift-clamp a wider rect. Width: " + width + " < " + toClamp.width, "toClamp");
            }
            if (toClamp.height > height)
            {
                throw new ArgumentException("Cannot shift-clamp a taller rect. Height: " + height + " < " + toClamp.height, "toClamp");
            }

            if (toClamp.bottomLeft.x < bottomLeft.x)
            {
                toClamp += new IntVector2(bottomLeft.x - toClamp.bottomLeft.x, 0);
            }
            else if (toClamp.topRight.x > topRight.x)
            {
                toClamp -= new IntVector2(toClamp.topRight.x - topRight.x, 0);
            }
            if (toClamp.bottomLeft.y < bottomLeft.y)
            {
                toClamp += new IntVector2(0, bottomLeft.y - toClamp.bottomLeft.y);
            }
            else if (toClamp.topRight.y > topRight.y)
            {
                toClamp -= new IntVector2(0, toClamp.topRight.y - topRight.y);
            }
            return toClamp;
        }

        /// <summary>
        /// Returns the elements of the sequence that are in this rect.
        /// </summary>
        /// <returns>
        /// A lazily-generated sequence containing the elements of <paramref name="vectors"/> that are in this rect.
        /// </returns>
        /// <remarks>
        /// Preserves the order of the sequence.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="vectors"/> is null.</exception>
        /// <seealso cref="FilterPointsOutside(IEnumerable{IntVector2})"/>
        public IEnumerable<IntVector2> FilterPointsInside(IEnumerable<IntVector2> vectors)
        {
            // C# can't access 'this' inside lambda expressions.
            IntRect rect = this;
            return vectors.Where(x => rect.Contains(x));
        }
        /// <summary>
        /// Returns the elements of the sequence that are outside this rect.
        /// </summary>
        /// <returns>
        /// A lazily-generated sequence containing the elements of <paramref name="vectors"/> that are not in this rect.
        /// </returns>
        /// <remarks>
        /// Preserves the order of the sequence.
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="vectors"/> is null.</exception>
        /// <seealso cref="FilterPointsInside(IEnumerable{IntVector2})"/>
        public IEnumerable<IntVector2> FilterPointsOutside(IEnumerable<IntVector2> vectors)
        {
            // C# can't access 'this' inside lambda expressions.
            IntRect rect = this;
            return vectors.Where(x => !rect.Contains(x));
        }

        /// <summary>
        /// Returns the smallest rect containing all the given vectors.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="vectors"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="vectors"/> is empty.</exception>
        public static IntRect BoundingRect(params IntVector2[] vectors) => BoundingRect((IEnumerable<IntVector2>)vectors);
        /// <summary>
        /// Returns the smallest rect containing all the given vectors.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="vectors"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="vectors"/> is empty.</exception>
        public static IntRect BoundingRect(IEnumerable<IntVector2> vectors) => new IntRect(IntVector2.Min(vectors), IntVector2.Max(vectors));
        /// <summary>
        /// Returns the smallest rect containing both the given rects.
        /// </summary>
        public static IntRect BoundingRect(IntRect a, IntRect b) => new IntRect(IntVector2.Min(a.bottomLeft, b.bottomLeft), IntVector2.Max(a.topRight, b.topRight));
        /// <summary>
        /// Returns the smallest rect containing all the given rects.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="rects"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="rects"/> is empty.</exception>
        public static IntRect BoundingRect(params IntRect[] rects) => BoundingRect((IEnumerable<IntRect>)rects);
        /// <summary>
        /// Returns the smallest rect containing all the given rects.
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="rects"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="rects"/> is empty.</exception>
        public static IntRect BoundingRect(IEnumerable<IntRect> rects)
        {
            if (rects is null)
            {
                throw new ArgumentNullException($"Cannot perform {nameof(BoundingRect)}() on null.", nameof(rects));
            }
            if (rects.IsEmpty())
            {
                throw new ArgumentException($"Cannot perform {nameof(BoundingRect)}() on an empty collection of {nameof(IntRect)}s.", nameof(rects));
            }

            return new IntRect(IntVector2.Min(rects.Select(rect => rect.bottomLeft)), IntVector2.Max(rects.Select(rect => rect.topRight)));
        }

        /// <summary>
        /// Returns the rect rotated clockwise by the given angle.
        /// </summary>
        /// <seealso cref="IntVector2.Rotate(RotationAngle)"/>
        public IntRect Rotate(RotationAngle angle) => new IntRect(bottomLeft.Rotate(angle), topRight.Rotate(angle));

        /// <summary>
        /// Returns the rect flipped across the given axis.
        /// </summary>
        /// <seealso cref="IntVector2.Flip(FlipAxis)"/>
        public IntRect Flip(FlipAxis axis) => new IntRect(bottomLeft.Flip(axis), topRight.Flip(axis));

        /// <summary>
        /// Generates a uniformly random point within the rect.
        /// </summary>
        public IntVector2 RandomPoint() => new IntVector2(UnityEngine.Random.Range(bottomLeft.x, topRight.x + 1), UnityEngine.Random.Range(bottomLeft.y, topRight.y + 1));

        /// <summary>
        /// Iterates over the points in the rect, starting with the bottom row, read left to right, then the next row, etc.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        /// <summary>
        /// Iterates over the points in the rect, starting with the bottom row, read left to right, then the next row, etc.
        /// </summary>
        public IEnumerator<IntVector2> GetEnumerator()
        {
            for (int y = bottomLeft.y; y <= topRight.y; y++)
            {
                for (int x = bottomLeft.x; x <= topRight.x; x++)
                {
                    yield return new IntVector2(x, y);
                }
            }
        }
    }
}
