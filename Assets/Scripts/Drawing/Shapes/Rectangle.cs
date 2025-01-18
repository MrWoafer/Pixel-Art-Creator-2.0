using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using PAC.DataStructures;
using PAC.Shapes.Interfaces;

namespace PAC.Shapes
{
    public class Rectangle : I2DShape<Rectangle>, IDeepCopyableShape<Rectangle>, IEquatable<Rectangle>
    {
        public bool filled { get; set; }

        /// <summary>True if the rectangle is a square.</summary>
        public bool isSquare => boundingRect.width == boundingRect.height;

        public IntRect boundingRect { get; set; }

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
                return 2 * (boundingRect.width + boundingRect.height) - 4;
            }
        }

        public Rectangle(IntVector2 corner, IntVector2 oppositeCorner, bool filled) : this(new IntRect(corner, oppositeCorner), filled) { }
        public Rectangle(IntRect boundingRect, bool filled)
        {
            this.boundingRect = boundingRect;
            this.filled = filled;
        }

        public bool Contains(IntVector2 pixel)
        {
            if (filled)
            {
                return boundingRect.Contains(pixel);
            }
            return boundingRect.Contains(pixel) && !(pixel > boundingRect.bottomLeft && pixel < boundingRect.topRight);
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
        public static Rectangle operator -(Rectangle rectangle, IntVector2 translation) => rectangle + -translation;
        /// <summary>
        /// Reflects the rectangle through the origin.
        /// </summary>
        public static Rectangle operator -(Rectangle rectangle) => new Rectangle(-rectangle.boundingRect, rectangle.filled);

        /// <summary>
        /// Translates the rectangle by the given vector.
        /// </summary>
        public Rectangle Translate(IntVector2 translation) => new Rectangle(boundingRect + translation, filled);

        /// <summary>
        /// Reflects the rectangle across the given axis.
        /// </summary>
        public Rectangle Flip(FlipAxis axis) => new Rectangle(boundingRect.Flip(axis), filled);

        /// <summary>
        /// Rotates the rectangle by the given angle.
        /// </summary>
        public Rectangle Rotate(RotationAngle angle) => new Rectangle(boundingRect.Rotate(angle), filled);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<IntVector2> GetEnumerator()
        {
            // Filled - start with the bottom row, read left to right, then the next row, etc.
            if (filled)
            {
                foreach (int y in boundingRect.yRange)
                {
                    foreach (int x in boundingRect.xRange)
                    {
                        yield return new IntVector2(x, y);
                    }
                }
                yield break;
            }

            // Unfilled - go clockwise, starting at bottom-left

            // Up the left side
            foreach (int y in boundingRect.yRange)
            {
                yield return new IntVector2(boundingRect.minX, y);
            }

            if (boundingRect.width == 1)
            {
                yield break;
            }

            // Along the top side (left to right), skipping the point we've already seen
            foreach (int x in boundingRect.xRange.Skip(1))
            {
                yield return new IntVector2(x, boundingRect.maxY);
            }

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
            for (int x = boundingRect.maxX - 1; x >= boundingRect.minX + 1; x--)
            {
                yield return new IntVector2(x, boundingRect.minY);
            }
        }

        public static bool operator ==(Rectangle a, Rectangle b) => a.boundingRect == b.boundingRect && a.filled == b.filled;
        public static bool operator !=(Rectangle a, Rectangle b) => !(a == b);
        public bool Equals(Rectangle other) => this == other;
        public override bool Equals(object obj) => obj is Rectangle other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(boundingRect, filled);

        public override string ToString() => "Rectangle(" + boundingRect.bottomLeft + ", " + boundingRect.topRight + ", " + (filled ? "filled" : "unfilled") + ")";

        public Rectangle DeepCopy() => new Rectangle(boundingRect, filled);
    }
}