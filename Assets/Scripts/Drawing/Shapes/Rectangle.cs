using System;
using System.Collections;
using System.Collections.Generic;

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
        public static Rectangle operator -(Rectangle rectangle) => new Rectangle(-rectangle.boundingRect.bottomLeft, -rectangle.boundingRect.topRight, rectangle.filled);

        /// <summary>
        /// Translates the rectangle by the given vector.
        /// </summary>
        public Rectangle Translate(IntVector2 translation) => new Rectangle(boundingRect.bottomLeft + translation, boundingRect.topRight + translation, filled);

        /// <summary>
        /// Reflects the rectangle across the given axis.
        /// </summary>
        public Rectangle Flip(FlipAxis axis) => new Rectangle(boundingRect.bottomLeft.Flip(axis), boundingRect.topRight.Flip(axis), filled);

        /// <summary>
        /// Rotates the rectangle by the given angle.
        /// </summary>
        public Rectangle Rotate(RotationAngle angle) => new Rectangle(boundingRect.bottomLeft.Rotate(angle), boundingRect.topRight.Rotate(angle), filled);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<IntVector2> GetEnumerator()
        {
            // Filled - start with the bottom row, read left to right, then the next row, etc.
            if (filled)
            {
                for (int y = boundingRect.bottomLeft.y; y <= boundingRect.topRight.y; y++)
                {
                    for (int x = boundingRect.bottomLeft.x; x <= boundingRect.topRight.x; x++)
                    {
                        yield return new IntVector2(x, y);
                    }
                }
                yield break;
            }

            // Unfilled - go clockwise, starting at bottom-left

            // Up the left side
            for (int y = boundingRect.bottomLeft.y; y <= boundingRect.topRight.y; y++)
            {
                yield return new IntVector2(boundingRect.bottomLeft.x, y);
            }

            // Rectangle has width 1
            if (boundingRect.bottomLeft.x == boundingRect.topRight.x)
            {
                yield break;
            }

            // Along the top side (left to right)
            for (int x = boundingRect.bottomLeft.x + 1; x <= boundingRect.topRight.x; x++)
            {
                yield return new IntVector2(x, boundingRect.topRight.y);
            }

            // Rectangle has height 1
            if (boundingRect.bottomLeft.y == boundingRect.topRight.y)
            {
                yield break;
            }

            // Down the right side
            for (int y = boundingRect.topRight.y - 1; y >= boundingRect.bottomLeft.y; y--)
            {
                yield return new IntVector2(boundingRect.topRight.x, y);
            }

            // Along the bottom side (right to left)
            for (int x = boundingRect.topRight.x - 1; x >= boundingRect.bottomLeft.x + 1; x--)
            {
                yield return new IntVector2(x, boundingRect.bottomLeft.y);
            }
        }

        public static bool operator ==(Rectangle a, Rectangle b) => a.boundingRect == b.boundingRect && a.filled == b.filled;
        public static bool operator !=(Rectangle a, Rectangle b) => !(a == b);
        public bool Equals(Rectangle other) => this == other;
        public override bool Equals(object obj) => obj is Rectangle other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(boundingRect.bottomLeft, boundingRect.topRight, filled);

        public override string ToString() => "Rectangle(" + boundingRect.bottomLeft + ", " + boundingRect.topRight + ", " + (filled ? "filled" : "unfilled") + ")";

        public Rectangle DeepCopy() => new Rectangle(boundingRect.bottomLeft, boundingRect.topRight, filled);
    }
}