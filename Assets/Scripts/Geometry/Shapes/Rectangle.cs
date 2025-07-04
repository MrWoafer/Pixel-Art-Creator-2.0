using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using PAC.Geometry.Axes;
using PAC.Geometry.Shapes.Interfaces;

namespace PAC.Geometry.Shapes
{
    /// <summary>
    /// A pixel art rectangle shape.
    /// <example>
    /// For example,
    /// <code>
    /// # # # #
    /// #     #
    /// # # # #
    /// </code>
    /// </example>
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Enumerator:</b>
    /// </para>
    /// <para>
    /// The enumerator does not repeat any points.
    /// </para>
    /// </remarks>
    public class Rectangle : I2DShape<Rectangle>, IDeepCopyableShape<Rectangle>, IEquatable<Rectangle>
    {
        public bool filled { get; set; }

        public IntRect boundingRect { get; set; }

        /// <summary>
        /// Whether the <see cref="Rectangle"/> is a square.
        /// </summary>
        /// <remarks>
        /// This is equivalent to its <see cref="boundingRect"/> being a square.
        /// </remarks>
        public bool isSquare => boundingRect.isSquare;

        public int Count
        {
            get
            {
                // Filled
                if (filled)
                {
                    return boundingRect.width * boundingRect.height;
                }
                // Unfilled
                if (boundingRect.width == 1)
                {
                    return boundingRect.height;
                }
                if (boundingRect.height == 1)
                {
                    return boundingRect.width;
                }
                // Add the length of the left side, right side, top side and bottom side, then subtract 4 as we've double-counted each corner
                return 2 * (boundingRect.width + boundingRect.height) - 4;
            }
        }

        /// <summary>
        /// Creates a <see cref="Rectangle"/> that takes up the region of the given <see cref="IntRect"/>.
        /// </summary>
        /// <param name="boundingRect">See <see cref="boundingRect"/>.</param>
        /// <param name="filled">See <see cref="filled"/>.</param>
        public Rectangle(IntRect boundingRect, bool filled)
        {
            this.boundingRect = boundingRect;
            this.filled = filled;
        }

        public bool Contains(IntVector2 point)
        {
            if (filled)
            {
                return boundingRect.Contains(point);
            }
            return boundingRect.Contains(point) && !(boundingRect.bottomLeft < point && point < boundingRect.topRight);
        }

        /// <summary>
        /// Returns a deep copy of the <see cref="Rectangle"/> translated by the given vector.
        /// </summary>
        /// <seealso cref="Translated(IntVector2)"/>
        public static Rectangle operator +(Rectangle rectangle, IntVector2 translation) => rectangle.Translated(translation);
        /// <summary>
        /// Returns a deep copy of the <see cref="Rectangle"/> translated by the given vector.
        /// </summary>
        /// <seealso cref="Translated(IntVector2)"/>
        public static Rectangle operator +(IntVector2 translation, Rectangle rectangle) => rectangle + translation;
        /// <summary>
        /// Returns a deep copy of the <see cref="Rectangle"/> translated by the given vector.
        /// </summary>
        /// <seealso cref="Translated(IntVector2)"/>
        public static Rectangle operator -(Rectangle rectangle, IntVector2 translation) => rectangle + (-translation);
        /// <summary>
        /// Returns a deep copy of the <see cref="Rectangle"/> rotated 180 degrees about the origin (equivalently, reflected through the origin).
        /// </summary>
        /// <seealso cref="Rotated(QuadrantalAngle)"/>
        /// <seealso cref="Flip(CardinalOrdinalAxis)"/>
        public static Rectangle operator -(Rectangle rectangle) => new Rectangle(-rectangle.boundingRect, rectangle.filled);

        public Rectangle Translated(IntVector2 translation) => new Rectangle(boundingRect + translation, filled);
        public Rectangle Flip(CardinalOrdinalAxis axis) => new Rectangle(boundingRect.Flip(axis), filled);
        public Rectangle Rotated(QuadrantalAngle angle) => new Rectangle(boundingRect.Rotate(angle), filled);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<IntVector2> GetEnumerator()
        {
            if (filled)
            {
                foreach (IntVector2 point in boundingRect)
                {
                    yield return point;
                }
                yield break;
            }

            // Unfilled - go clockwise, starting at bottom-left

            // Up the left side
            foreach (int y in boundingRect.yRange)
            {
                yield return new IntVector2(boundingRect.minX, y);
            }

            // Avoid repeating points
            if (boundingRect.width == 1)
            {
                yield break;
            }

            // Along the top side (left to right), skipping the point we've already seen
            foreach (int x in boundingRect.xRange.Skip(1))
            {
                yield return new IntVector2(x, boundingRect.maxY);
            }

            // Avoid repeating points
            if (boundingRect.height == 1)
            {
                yield break;
            }

            // Down the right side, skipping the point we've already seen
            foreach (int y in boundingRect.yRange.reverse.Skip(1))
            {
                yield return new IntVector2(boundingRect.maxX, y);
            }

            // Along the bottom side (right to left), skipping the two endpoints as we've already seen them
            foreach (int x in boundingRect.xRange.reverse.Skip(1).SkipLast(1))
            {
                yield return new IntVector2(x, boundingRect.minY);
            }
        }

        /// <summary>
        /// Whether the two <see cref="Rectangle"/>s have the same shape, and the same value for <see cref="filled"/>.
        /// </summary>
        public static bool operator ==(Rectangle a, Rectangle b) => a.boundingRect == b.boundingRect && a.filled == b.filled;
        /// <summary>
        /// See <see cref="operator ==(Rectangle, Rectangle)"/>.
        /// </summary>
        public static bool operator !=(Rectangle a, Rectangle b) => !(a == b);
        /// <summary>
        /// See <see cref="operator ==(Rectangle, Rectangle)"/>.
        /// </summary>
        public bool Equals(Rectangle other) => this == other;
        /// <summary>
        /// See <see cref="Equals(Rectangle)"/>.
        /// </summary>
        public override bool Equals(object obj) => obj is Rectangle other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(boundingRect, filled);

        public override string ToString() => $"{nameof(Rectangle)}({boundingRect}, {(filled ? "filled" : "unfilled")})";

        public Rectangle DeepCopy() => new Rectangle(boundingRect, filled);
    }
}