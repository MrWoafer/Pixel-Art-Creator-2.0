using System;
using System.Collections;
using System.Collections.Generic;

using PAC.DataStructures;
using PAC.Shapes.Interfaces;

namespace PAC.Shapes
{
    public class Diamond : I2DShape<Diamond>, IDeepCopyableShape<Diamond>, IEquatable<Diamond>
    {
        public bool filled { get; set; }

        /// <summary>True if the diamond is a square.</summary>
        public bool isSquare => boundingRect.width == boundingRect.height;

        public IntRect boundingRect { get; private set; }

        public int Count
        {
            get
            {
                if (boundingRect.width == 1 || boundingRect.height == 1)
                {
                    return Math.Max(boundingRect.width, boundingRect.height);
                }

                var edges = this.edges;
                if (filled)
                {
                    int count = 0;
                    for (int y = boundingRect.minY; y <= edges.bottomLeft.boundingRect.topRight.y; y++)
                    {
                        count += edges.bottomRight.MaxX(y) - edges.bottomLeft.MinX(y) + 1;
                    }
                    // The potential + 1 for the starting y is to avoid repeating pixels
                    for (int y = edges.bottomLeft.boundingRect.topRight.y + 1; y <= boundingRect.maxY; y++)
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
                    count -= edges.bottomLeft.CountOnX(boundingRect.minX) * edges.bottomLeft.CountOnY(edges.bottomLeft.boundingRect.topRight.y);
                    // Remove bottom block of the line
                    count -= edges.bottomLeft.CountOnY(boundingRect.minY) * edges.bottomLeft.CountOnX(edges.bottomLeft.boundingRect.topRight.x);
                    // Count it for the other 3 edges
                    count *= 4;
                    // Count the left-most block of the diamond (doubled to also count the right-most block)
                    count += 2 * (edges.topLeft.MaxY(boundingRect.minX) - edges.bottomLeft.MinY(boundingRect.minX) + 1) * edges.bottomLeft.CountOnY(edges.bottomLeft.boundingRect.topRight.y);
                    // Count the bottom block of the diamond (doubled to also count the top block)
                    count += 2 * (edges.bottomRight.MaxX(boundingRect.minY) - edges.bottomLeft.MinX(boundingRect.minY) + 1) * edges.bottomLeft.CountOnX(edges.bottomLeft.boundingRect.topRight.x);

                    return count;
                }
            }
        }

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

                // This is to ensure rotating / reflecting doesn't change the shape (up to rotating / reflecting)
                if (boundingRect.width > boundingRect.height)
                {
                    for (int i = 0; i < lines.Length; i++)
                    {
                        lines[i] = new Line(lines[i].end, lines[i].start);
                    }
                }

                // This is to make the the diamonds more aesthetic by overlapping the edges as much as possible
                // One such effect this has is making sure diamonds that can be drawn with perfect lines are drawn as such
                if (boundingRect.width > boundingRect.height)
                {
                    // Not particularly efficient, but does the job and really isn't slow even when drawing diamonds with a width of 1000
                    while (lines[0].boundingRect.bottomRight.x <= lines[1].MaxX(boundingRect.minY))
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
                else if (boundingRect.width < boundingRect.height)
                {
                    while (lines[0].boundingRect.topLeft.y <= lines[3].MaxY(boundingRect.minX))
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
                return edges.bottomLeft.PointIsToRight(pixel) && edges.bottomRight.PointIsToLeft(pixel) || edges.topLeft.PointIsToRight(pixel) && edges.topRight.PointIsToLeft(pixel);
            }

            foreach (Line edge in edges.AsEnumerable())
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
        public static Diamond operator -(Diamond diamond, IntVector2 translation) => diamond + -translation;
        /// <summary>
        /// Reflects the diamond through the origin.
        /// </summary>
        public static Diamond operator -(Diamond diamond) => new Diamond(-diamond.boundingRect.bottomLeft, -diamond.boundingRect.topRight, diamond.filled);

        /// <summary>
        /// Translates the diamond by the given vector.
        /// </summary>
        public Diamond Translate(IntVector2 translation) => new Diamond(boundingRect.bottomLeft + translation, boundingRect.topRight + translation, filled);

        /// <summary>
        /// Reflects the diamond across the given axis.
        /// </summary>
        public Diamond Flip(FlipAxis axis) => new Diamond(boundingRect.bottomLeft.Flip(axis), boundingRect.topRight.Flip(axis), filled);

        /// <summary>
        /// Rotates the diamond by the given angle.
        /// </summary>
        public Diamond Rotate(RotationAngle angle) => new Diamond(boundingRect.bottomLeft.Rotate(angle), boundingRect.topRight.Rotate(angle), filled);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<IntVector2> GetEnumerator()
        {
            var edges = this.edges;
            if (filled)
            {
                for (int y = boundingRect.minY; y <= edges.bottomLeft.boundingRect.topRight.y; y++)
                {
                    for (int x = edges.bottomLeft.MinX(y); x <= edges.bottomRight.MaxX(y); x++)
                    {
                        yield return new IntVector2(x, y);
                    }
                }
                // The potential + 1 for the starting y is to avoid repeating pixels
                for (int y = edges.bottomLeft.boundingRect.topRight.y + 1; y <= boundingRect.maxY; y++)
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
        public bool Equals(Diamond other) => this == other;
        public override bool Equals(object obj) => obj is Diamond other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(boundingRect.bottomLeft, boundingRect.topRight, filled);

        public override string ToString() => "Diamond(" + boundingRect.bottomLeft + ", " + boundingRect.topRight + ", " + (filled ? "filled" : "unfilled") + ")";

        public Diamond DeepCopy() => new Diamond(boundingRect.bottomLeft, boundingRect.topRight, filled);
    }
}