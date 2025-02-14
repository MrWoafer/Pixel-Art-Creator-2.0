using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using PAC.DataStructures;
using PAC.Shapes.Interfaces;

namespace PAC.Shapes
{
    /// <summary>
    /// A pixel art diamond shape.
    /// <example>
    /// For example,
    /// <code>
    ///     #
    ///     #
    ///   #   #
    ///   #   #
    /// #       #
    /// #       #
    ///   #   #
    ///   #   #
    ///     #
    ///     #
    /// </code>
    /// </example>
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Shape:</b>
    /// </para>
    /// <para>
    /// If the border can be drawn using perfect <see cref="Line"/>s, it will be. (See <see cref="Line"/> for a definition of 'perfect'.)
    /// </para>
    /// <para>
    /// If the width or height of the <see cref="boundingRect"/> is &lt;= 2, it will look like a rectangle.
    /// </para>
    /// <para>
    /// <b>Enumerator:</b>
    /// </para>
    /// <para>
    /// The enumerator does not repeat any points.
    /// </para>
    /// </remarks>
    public class Diamond : I2DShape<Diamond>, IDeepCopyableShape<Diamond>, IEquatable<Diamond>
    {
        public bool filled { get; set; }

        public IntRect boundingRect { get; set; }

        /// <summary>
        /// Whether the <see cref="Diamond"/> is a square (at a 45-degree angle).
        /// </summary>
        /// <remarks>
        /// This is equivalent to its <see cref="boundingRect"/> being a square.
        /// </remarks>
        public bool isSquare => boundingRect.isSquare;

        public int Count
        {
            get
            {
                if (boundingRect.width == 1)
                {
                    return boundingRect.height;
                }
                if (boundingRect.height == 1)
                {
                    return boundingRect.width;
                }

                var edges = this.edges;
                if (filled)
                {
                    int count = 0;
                    for (int y = boundingRect.minY; y <= edges.bottomLeft.boundingRect.maxY; y++)
                    {
                        count += edges.bottomRight.MaxX(y) - edges.bottomLeft.MinX(y) + 1;
                    }
                    // The + 1 for the starting y is to avoid repeating points
                    for (int y = edges.bottomLeft.boundingRect.maxY + 1; y <= boundingRect.maxY; y++)
                    {
                        count += edges.topRight.MaxX(y) - edges.topLeft.MinX(y) + 1;
                    }
                    return count;
                }
                else
                {
                    if (boundingRect.width <= 2 || boundingRect.height <= 2)
                    {
                        return boundingRect.width * boundingRect.height;
                    }

                    // We exploit the symmetry of the diamond across the vertical and horizontal axes
                    // However, it's not just as simple as multiplying the count of one edge by 4, since the edges can overlap

                    int count = edges.bottomLeft.Count;
                    // Remove left-most block of the line
                    count -= edges.bottomLeft.CountOnX(boundingRect.minX) * edges.bottomLeft.CountOnY(edges.bottomLeft.boundingRect.maxY);
                    // Remove bottom block of the line
                    count -= edges.bottomLeft.CountOnY(boundingRect.minY) * edges.bottomLeft.CountOnX(edges.bottomLeft.boundingRect.maxX);
                    // Count it for the other 3 edges
                    count *= 4;
                    // Count the left-most block of the diamond (doubled to also count the right-most block)
                    count += 2 * (edges.topLeft.MaxY(boundingRect.minX) - edges.bottomLeft.MinY(boundingRect.minX) + 1) * edges.bottomLeft.CountOnY(edges.bottomLeft.boundingRect.maxY);
                    // Count the bottom block of the diamond (doubled to also count the top block)
                    count += 2 * (edges.bottomRight.MaxX(boundingRect.minY) - edges.bottomLeft.MinX(boundingRect.minY) + 1) * edges.bottomLeft.CountOnX(edges.bottomLeft.boundingRect.maxX);

                    return count;
                }
            }
        }

        /// <summary>
        /// The four <see cref="Line"/>s that make up the border of the <see cref="Diamond"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Note this is in general not a <see cref="Path"/>. This is because we overlap the end blocks of the <see cref="Line"/>s to make the <see cref="Diamond"/> more aesthetic (without it, the
        /// middle blocks on each side are longer than all the other blocks) and to ensure that <see cref="Diamond"/>s that can be drawn with perfect <see cref="Line"/>s are drawn as such.
        /// </para>
        /// <example>
        /// For example, without overlapping:
        /// <code>
        ///     # &lt;- other endpoint of top-right edge
        ///     #
        ///   #   #
        /// #       #
        /// #       # &lt;- one endpoint of top-right edge
        /// #       # &lt;- one endpoint of bottom-right edge
        /// #       #
        ///   #   #
        ///     #
        ///     # &lt;- other endpoint of bottom-right edge
        /// </code>
        /// With overlapping:
        /// <code>
        ///     # &lt;- other endpoint of top-right edge
        ///     #
        ///   #   #
        ///   #   #
        /// #       # &lt;- one endpoint of bottom-right edge
        /// #       # &lt;- one endpoint of top-right edge
        ///   #   #
        ///   #   #
        ///     #
        ///     # &lt;- other endpoint of bottom-right edge
        /// </code>
        /// </example>
        /// </remarks>
        private (Line bottomLeft, Line bottomRight, Line topRight, Line topLeft) edges
        {
            get
            {
                Line[] lines = new Line[] {
                        // Bottom-left edge
                        new Line(boundingRect.topLeft + boundingRect.height / 2 * IntVector2.down, boundingRect.bottomRight + boundingRect.width / 2 * IntVector2.left),
                        // Bottom-right edge
                        new Line(boundingRect.topRight + boundingRect.height / 2 * IntVector2.down, boundingRect.bottomLeft + boundingRect.width / 2 * IntVector2.right),
                        // Top-right edge
                        new Line(boundingRect.bottomRight + boundingRect.height / 2 * IntVector2.up, boundingRect.topLeft + boundingRect.width / 2 * IntVector2.right),
                        // Top-left edge
                        new Line(boundingRect.bottomLeft + boundingRect.height / 2 * IntVector2.up, boundingRect.topRight + boundingRect.width / 2 * IntVector2.left)
                    };

                // This is to ensure rotating / reflecting doesn't change the geometry (up to rotating / reflecting)
                if (boundingRect.width > boundingRect.height)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        lines[i] = lines[i].reverse;
                    }
                }

                // Overlap the end blocks of the lines

                if (boundingRect.width > boundingRect.height)
                {
                    // Not particularly efficient, but does the job and really isn't slow even when drawing diamonds with a width of 1000
                    while (lines[0].boundingRect.maxX <= lines[1].MaxX(boundingRect.minY))
                    {
                        lines[0].start += IntVector2.right;
                        lines[1].start += IntVector2.left;
                        lines[2].start += IntVector2.left;
                        lines[3].start += IntVector2.right;
                    }

                    // Undo the last step, so that the loop condition holds again (this process is done to obtain the boundary case for that condition)
                    lines[0].start -= IntVector2.right;
                    lines[1].start -= IntVector2.left;
                    lines[2].start -= IntVector2.left;
                    lines[3].start -= IntVector2.right;
                }
                else if (boundingRect.width < boundingRect.height)
                {
                    while (lines[0].boundingRect.maxY <= lines[3].MaxY(boundingRect.minX))
                    {
                        lines[0].start += IntVector2.up;
                        lines[1].start += IntVector2.up;
                        lines[2].start += IntVector2.down;
                        lines[3].start += IntVector2.down;
                    }

                    lines[0].start -= IntVector2.up;
                    lines[1].start -= IntVector2.up;
                    lines[2].start -= IntVector2.down;
                    lines[3].start -= IntVector2.down;
                }

                return (lines[0], lines[1], lines[2], lines[3]);
            }
        }

        /// <summary>
        /// Creates the largest <see cref="Diamond"/> that can fit in the given rect.
        /// </summary>
        /// <param name="boundingRect">See <see cref="boundingRect"/>.</param>
        /// <param name="filled">See <see cref="filled"/>.</param>
        public Diamond(IntRect boundingRect, bool filled)
        {
            this.boundingRect = boundingRect;
            this.filled = filled;
        }

        public bool Contains(IntVector2 point)
        {
            if (filled)
            {
                var edges = this.edges;
                return edges.bottomLeft.PointIsToRight(point) && edges.bottomRight.PointIsToLeft(point) || edges.topLeft.PointIsToRight(point) && edges.topRight.PointIsToLeft(point);
            }
            return edges.AsEnumerable().Any(edge => edge.Contains(point));
        }

        /// <summary>
        /// Returns a deep copy of the <see cref="Diamond"/> translated by the given vector.
        /// </summary>
        /// <seealso cref="Translate(IntVector2)"/>
        public static Diamond operator +(Diamond diamond, IntVector2 translation) => diamond.Translate(translation);
        /// <summary>
        /// Returns a deep copy of the <see cref="Diamond"/> translated by the given vector.
        /// </summary>
        /// <seealso cref="Translate(IntVector2)"/>
        public static Diamond operator +(IntVector2 translation, Diamond diamond) => diamond + translation;
        /// <summary>
        /// Returns a deep copy of the <see cref="Diamond"/> translated by the given vector.
        /// </summary>
        /// <seealso cref="Translate(IntVector2)"/>
        public static Diamond operator -(Diamond diamond, IntVector2 translation) => diamond + (-translation);
        /// <summary>
        /// Returns a deep copy of the <see cref="Diamond"/> rotated 180 degrees about the origin (equivalently, reflected through the origin).
        /// </summary>
        /// <seealso cref="Rotate(RotationAngle)"/>
        /// <seealso cref="Flip(FlipAxis)"/>
        public static Diamond operator -(Diamond diamond) => new Diamond(-diamond.boundingRect, diamond.filled);

        public Diamond Translate(IntVector2 translation) => new Diamond(boundingRect + translation, filled);
        public Diamond Flip(FlipAxis axis) => new Diamond(boundingRect.Flip(axis), filled);
        public Diamond Rotate(RotationAngle angle) => new Diamond(boundingRect.Rotate(angle), filled);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<IntVector2> GetEnumerator()
        {
            var edges = this.edges;
            if (filled)
            {
                for (int y = boundingRect.minY; y <= edges.bottomLeft.boundingRect.maxY; y++)
                {
                    for (int x = edges.bottomLeft.MinX(y); x <= edges.bottomRight.MaxX(y); x++)
                    {
                        yield return new IntVector2(x, y);
                    }
                }
                // The + 1 for the starting y is to avoid repeating points
                for (int y = edges.bottomLeft.boundingRect.maxY + 1; y <= boundingRect.maxY; y++)
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
                foreach (IntVector2 point in edges.bottomLeft)
                {
                    yield return point;
                }

                // Bottom-right edge
                foreach (IntVector2 point in edges.bottomRight)
                {
                    // This check avoids repeating points from previous line
                    if (point.x > edges.bottomLeft.boundingRect.maxX)
                    {
                        yield return point;
                    }
                }

                // Top-right edge
                foreach (IntVector2 point in edges.topRight)
                {
                    // This check avoids repeating points from previous lines
                    if (point.y > edges.bottomRight.boundingRect.maxY)
                    {
                        yield return point;
                    }
                }

                // Top-left edge
                foreach (IntVector2 point in edges.topLeft)
                {
                    // This check avoids repeating points from previous lines
                    if (point.x < edges.topRight.boundingRect.minX && point.y > edges.bottomLeft.boundingRect.maxY)
                    {
                        yield return point;
                    }
                }
            }
        }

        /// <summary>
        /// Whether the two <see cref="Diamond"/>s have the same shape, and the same value for <see cref="filled"/>.
        /// </summary>
        public static bool operator ==(Diamond a, Diamond b) => a.boundingRect == b.boundingRect && a.filled == b.filled;
        /// <summary>
        /// See <see cref="operator ==(Diamond, Diamond)"/>.
        /// </summary>
        public static bool operator !=(Diamond a, Diamond b) => !(a == b);
        /// <summary>
        /// See <see cref="operator ==(Diamond, Diamond)"/>.
        /// </summary>
        public bool Equals(Diamond other) => this == other;
        /// <summary>
        /// See <see cref="Equals(Diamond)"/>.
        /// </summary>
        public override bool Equals(object obj) => obj is Diamond other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(boundingRect, filled);

        public override string ToString() => $"{nameof(Diamond)}({boundingRect}, {(filled ? "filled" : "unfilled")})";

        public Diamond DeepCopy() => new Diamond(boundingRect, filled);
    }
}