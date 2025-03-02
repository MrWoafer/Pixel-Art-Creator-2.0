using System.Collections;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;
using System;
using PAC.Extensions;
using PAC.Interfaces;
using PAC.Geometry;

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
    /// The enumerator iterates over the points in the rect, starting with the bottom row, read left to right, then the next row, etc.
    /// This ordering is useful since it matches the expected order of pixels in Unity's <see cref="Texture2D.SetPixels(Color[])"/>.
    /// </remarks>
    /// <seealso cref="IntVector2"/>
    /// <seealso cref="IntRange"/>
    public readonly struct IntRect : IReadOnlyContains<IntVector2>, IEquatable<IntRect>, ISetComparable<IntRect>
    {
        #region Fields
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
        #endregion

        #region Properties
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
        /// The minimum x coordinate of points in the rect.
        /// </summary>
        /// <seealso cref="maxX"/>
        /// <seealso cref="minY"/>
        /// <seealso cref="maxY"/>
        public int minX => bottomLeft.x;
        /// <summary>
        /// The maximum x coordinate of points in the rect.
        /// </summary>
        /// <seealso cref="minX"/>
        /// <seealso cref="minY"/>
        /// <seealso cref="maxY"/>
        public int maxX => topRight.x;
        /// <summary>
        /// The minimum y coordinate of points in the rect.
        /// </summary>
        /// <seealso cref="minX"/>
        /// <seealso cref="maxX"/>
        /// <seealso cref="maxY"/>
        public int minY => bottomLeft.y;
        /// <summary>
        /// The maximum y coordinate of points in the rect.
        /// </summary>
        /// <seealso cref="minX"/>
        /// <seealso cref="maxX"/>
        /// <seealso cref="minY"/>
        public int maxY => topRight.y;

        /// <summary>
        /// The number of integer points the rect covers horizontally.
        /// </summary>
        /// <seealso cref="height"/>
        /// <seealso cref="size"/>
        public int width => maxX - minX + 1;
        /// <summary>
        /// The number of integer points the rect covers vertically.
        /// </summary>
        /// <seealso cref="width"/>
        /// <seealso cref="size"/>
        public int height => maxY - minY + 1;
        /// <summary>
        /// The vector <c>(<see cref="width"/>, <see cref="height"/>)</c>.
        /// </summary>
        /// <seealso cref="width"/>
        /// <seealso cref="height"/>
        public IntVector2 size => new IntVector2(width, height);

        /// <summary>
        /// The inclusive range of x coordinates in the rect, in increasing order.
        /// </summary>
        /// <seealso cref="yRange"/>
        public IntRange xRange => IntRange.InclIncl(minX, maxX);
        /// <summary>
        /// The inclusive range of y coordinates in the rect, in increasing order.
        /// </summary>
        /// <see cref="xRange"/>
        public IntRange yRange => IntRange.InclIncl(minY, maxY);

        /// <summary>
        /// The number of points in the rect.
        /// </summary>
        public int Count => width * height;

        /// <summary>
        /// Whether the rect is a square - i.e. the <see cref="width"/> and <see cref="height"/> are equal.
        /// </summary>
        /// <seealso cref="IsSquare(IntRect)"/>
        public bool isSquare => IsSquare(this);
        #endregion

        #region Constructors
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
        #endregion

        #region Conversion
        /// <summary>
        /// Casts to Unity's <see cref="RectInt"/>.
        /// </summary>
        public static implicit operator RectInt(IntRect rect) => new RectInt(rect.topLeft, rect.size);
        /// <summary>
        /// Casts from Unity's <see cref="RectInt"/>.
        /// </summary>
        public static implicit operator IntRect(RectInt rect) => new IntRect(new IntVector2(rect.xMin, rect.yMin), new IntVector2(rect.xMax, rect.yMax));

        // I haven't added a cast to Unity's Rect because of the lack of a convention as to where an IntVector2 refers to in a pixel (the centre of the pixel? the bottom-left of the pixel?),
        // which affects which coordinates to use for the corners of the Rect.

        // I haven't added a cast from Unity's Rect since it's more complicated than just 'new IntRect(new IntVector2(rect.xMin, rect.yMin), new IntVector2(rect.xMax, rect.yMax))'.
        // Doing that would mean casting to Rect and from Rect aren't inverses. To make that the case I also need to decide how Rects that don't lie on integer coords get rounded.
        // It would also mean that the way the top-right corners gets rounded is different from the bottom-left.
        #endregion

        #region Comparison
        /// <summary>
        /// Whether the two rects describe the same set of points.
        /// </summary>
        /// <remarks>
        /// This is just the default <see langword="struct"/> comparison (comparing fields).
        /// </remarks>
        /// <seealso cref="Equals(IntRect)"/>
        /// <seealso cref="SetEquals(IntRect)"/>
        public static bool operator ==(IntRect a, IntRect b) => a.bottomLeft == b.bottomLeft && a.topRight == b.topRight;
        /// <summary>
        /// Whether the two rects describe different sets of points.
        /// </summary>
        /// <remarks>
        /// This is just the default <see langword="struct"/> comparison (comparing fields).
        /// </remarks>
        /// <seealso cref="Equals(IntRect)"/>
        /// <seealso cref="SetEquals(IntRect)"/>
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
        /// Whether the two rects describe the same set of points.
        /// </summary>
        /// <remarks>
        /// The is the same as <see cref="operator ==(IntRect, IntRect)"/>.
        /// </remarks>
        public bool SetEquals(IntRect other) => this == other;
        /// <summary>
        /// Whether this rect is a subset of <paramref name="other"/>.
        /// </summary>
        /// <seealso cref="IsSupersetOf(IntRect)"/>
        public bool IsSubsetOf(IntRect other) => other.IsSupersetOf(this);
        /// <summary>
        /// Whether this rect is a superset of <paramref name="other"/>.
        /// </summary>
        /// <seealso cref="IsSubsetOf(IntRect)"/>
        public bool IsSupersetOf(IntRect other) => bottomLeft <= other.bottomLeft && other.topRight <= topRight;

        /// <summary>
        /// Whether the point is in the rect.
        /// </summary>
        /// <seealso cref="Contains(int, int)"/>
        public bool Contains(IntVector2 point) => Contains(point.x, point.y);
        /// <summary>
        /// Whether the point <c>(<paramref name="x"/>, <paramref name="y"/>)</c> is in the rect.
        /// </summary>
        /// <seealso cref="Contains(IntVector2)"/>
        public bool Contains(int x, int y) => minX <= x && minY <= y && x <= maxX && y <= maxY;

        /// <summary>
        /// Returns whether the rect contains any points with the given x coord.
        /// </summary>
        /// <seealso cref="ContainsY(int)"/>
        public bool ContainsX(int x) => minX <= x && x <= maxX;
        /// <summary>
        /// Returns whether the rect contains any points with the given y coord.
        /// </summary>
        /// <seealso cref="ContainsX(int)"/>
        public bool ContainsY(int y) => minY <= y && y <= maxY;

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
        #endregion

        #region Operations
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
        /// <summary>
        /// Negates each point in the rect.
        /// </summary>
        public static IntRect operator -(IntRect rect) => new IntRect(-rect.bottomLeft, -rect.topRight);

        /// <summary>
        /// Whether the rect is a square - i.e. the <see cref="width"/> and <see cref="height"/> are equal.
        /// </summary>
        /// <seealso cref="isSquare"/>
        public static bool IsSquare(IntRect rect) => rect.width == rect.height;

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
        public IntVector2 Clamp(IntVector2 vector) => new IntVector2(Math.Clamp(vector.x, minX, maxX), Math.Clamp(vector.y, minY, maxY));

        /// <summary>
        /// Translates <paramref name="toClamp"/> so it is a subset of this rect. Chooses the smallest such translation.
        /// </summary>
        /// <remarks>
        /// Note this is only possible when <paramref name="toClamp"/> is at most as wide and at most as tall as this rect.
        /// </remarks>
        /// <exception cref="ArgumentException"><paramref name="toClamp"/> is wider and/or taller than this rect.</exception>
        public IntRect TranslateClamp(IntRect toClamp)
        {
            if (toClamp.width > width)
            {
                throw new ArgumentException($"Cannot translate-clamp a wider rect. Width of this: {width}. Width of {nameof(toClamp)}: {toClamp.width}.", nameof(toClamp));
            }
            if (toClamp.height > height)
            {
                throw new ArgumentException($"Cannot translate-clamp a taller rect. Height of this: {height}. Height of {nameof(toClamp)}: {toClamp.height}.", nameof(toClamp));
            }

            if (toClamp.minX < minX)
            {
                toClamp += new IntVector2(minX - toClamp.minX, 0);
            }
            else if (toClamp.maxX > maxX)
            {
                toClamp -= new IntVector2(toClamp.maxX - maxX, 0);
            }
            if (toClamp.minY < minY)
            {
                toClamp += new IntVector2(0, minY - toClamp.minY);
            }
            else if (toClamp.maxY > maxY)
            {
                toClamp -= new IntVector2(0, toClamp.maxY - maxY);
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
                throw new ArgumentNullException(nameof(rects), "The given collection of rects is null.");
            }
            if (rects.None())
            {
                throw new ArgumentException("The given collection of rects is empty.", nameof(rects));
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
        #endregion

        #region Random
        /// <summary>
        /// Generates a uniformly random point within the rect.
        /// </summary>
        public IntVector2 RandomPoint(System.Random random) => new IntVector2(random.Next(minX, maxX + 1), random.Next(minY, maxY + 1));
        /// <summary>
        /// Returns the rect defined by two independently uniformly randomly generated points within the rect.
        /// </summary>
        public IntRect RandomSubRect(System.Random random) => new IntRect(RandomPoint(random), RandomPoint(random));
        #endregion

        #region Enumerator
        /// <summary>
        /// Iterates over the points in the rect, starting with the bottom row, read left to right, then the next row, etc.
        /// </summary>
        /// <remarks>
        /// The ordering of this enumerator is useful since it matches the expected order of pixels in Unity's <see cref="Texture2D.SetPixels(Color[])"/>.
        /// </remarks>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        /// <summary>
        /// Iterates over the points in the rect, starting with the bottom row, read left to right, then the next row, etc.
        /// </summary>
        /// <remarks>
        /// The ordering of this enumerator is useful since it matches the expected order of pixels in Unity's <see cref="Texture2D.SetPixels(Color[])"/>.
        /// </remarks>
        public IEnumerator<IntVector2> GetEnumerator()
        {
            for (int y = minY; y <= maxY; y++)
            {
                for (int x = minX; x <= maxX; x++)
                {
                    yield return new IntVector2(x, y);
                }
            }
        }
        #endregion

        /// <summary>
        /// Represents the rect as a string in the form <c>"((bottom-left x coord, bottom-left y coord), (top-right x coord, top-right y coord))"</c>.
        /// </summary>
        public override string ToString() => $"({bottomLeft}, {topRight})";
    }
}
