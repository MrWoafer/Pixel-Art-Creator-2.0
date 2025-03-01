using System;
using System.Collections;
using System.Collections.Generic;

using PAC.DataStructures;
using PAC.Exceptions;
using PAC.Geometry.Shapes.Interfaces;

namespace PAC.Geometry.Shapes
{
    /// <summary>
    /// A pixel art right-angled triangle shape.
    /// <example>
    /// For example,
    /// <code>
    ///             # #
    ///         # #   #
    ///       #       #
    ///   # #         #
    /// #             #
    /// # # # # # # # #
    /// </code>
    /// </example>
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Shape:</b>
    /// </para>
    /// <para>
    /// The hypotenuse is drawn as a <see cref="Line"/>. We draw this <see cref="Line"/> starting from the longer side of the <see cref="RightTriangle"/> (or the horizontal side if both sides have
    /// the same length). This is done to ensure that rotating/reflecting the corners doesn't change the geometry (up to rotating/reflecting).
    /// </para>
    /// <para>
    /// Except in some special cases, this <see cref="Line"/> is not drawn from the corners. Instead, the endpoints are inset by 1 from the corners.
    /// <example>
    /// For example,
    /// <code>
    ///                      line end
    ///                          v
    ///                          # #
    ///                      # #   #
    ///                    #       #
    ///                # #         #
    /// line start > #             #
    ///              # # # # # # # #
    /// </code>
    /// </example>
    /// This is done to make it look more aesthetic.
    /// </para>
    /// <para>
    /// The special cases where the endpoints are not inset are:
    /// <list type="bullet">
    /// <item>
    /// Width or height 1 - the <see cref="RightTriangle"/> looks like a straight line
    /// </item>
    /// <item>
    /// Width or height 2 - the <see cref="RightTriangle"/> looks like, for example,
    /// <code>
    ///   #
    ///   #
    /// # #
    /// # #
    /// # #
    /// </code>
    /// where, if the width is 2, the height of the shorter vertical block is <c>ceil(height of triangle / 2)</c>, and analogously for when the height is 2.
    /// </item>
    /// </list>
    /// </para>
    /// <para>
    /// <b>Enumerator:</b>
    /// </para>
    /// <para>
    /// The enumerator does not repeat any points.
    /// </para>
    /// </remarks>
    public class RightTriangle : I2DShape<RightTriangle>, IDeepCopyableShape<RightTriangle>, IEquatable<RightTriangle>
    {
        /// <summary>
        /// The lower of the two corners that don't contain the right angle.
        /// </summary>
        /// <remarks>
        /// This will be different from <see cref="topCorner"/>, unless the <see cref="RightTriangle"/> is a single point.
        /// </remarks>
        /// <seealso cref="topCorner"/>
        /// <seealso cref="leftCorner"/>
        /// <seealso cref="rightCorner"/>
        /// <seealso cref="rightAngleCorner"/>
        public IntVector2 bottomCorner => rightAngleLocation switch
        {
            RightAngleLocation.BottomLeft => boundingRect.bottomRight,
            RightAngleLocation.BottomRight => boundingRect.bottomLeft,
            RightAngleLocation.TopLeft => boundingRect.bottomLeft,
            RightAngleLocation.TopRight => boundingRect.bottomRight,
            _ => throw new UnreachableException($"Unknown / unimplemented {nameof(RightAngleLocation)}: {rightAngleLocation}.")
        };
        /// <summary>
        /// The higher of the two corners that don't contain the right angle.
        /// </summary>
        /// <remarks>
        /// This will be different from <see cref="bottomCorner"/>, unless the <see cref="RightTriangle"/> is a single point.
        /// </remarks>
        /// <seealso cref="bottomCorner"/>
        /// <seealso cref="leftCorner"/>
        /// <seealso cref="rightCorner"/>
        /// <seealso cref="rightAngleCorner"/>
        public IntVector2 topCorner => rightAngleLocation switch
        {
            RightAngleLocation.BottomLeft => boundingRect.topLeft,
            RightAngleLocation.BottomRight => boundingRect.topRight,
            RightAngleLocation.TopLeft => boundingRect.topRight,
            RightAngleLocation.TopRight => boundingRect.topLeft,
            _ => throw new UnreachableException($"Unknown / unimplemented {nameof(RightAngleLocation)}: {rightAngleLocation}.")
        };
        /// <summary>
        /// The left-most of the two corners that don't contain the right angle.
        /// </summary>
        /// <remarks>
        /// This will be different from <see cref="rightCorner"/>, unless the <see cref="RightTriangle"/> is a single point.
        /// </remarks>
        /// <seealso cref="bottomCorner"/>
        /// <seealso cref="topCorner"/>
        /// <seealso cref="rightCorner"/>
        /// <seealso cref="rightAngleCorner"/>
        public IntVector2 leftCorner => rightAngleLocation switch
        {
            RightAngleLocation.BottomLeft => boundingRect.topLeft,
            RightAngleLocation.BottomRight => boundingRect.bottomLeft,
            RightAngleLocation.TopLeft => boundingRect.bottomLeft,
            RightAngleLocation.TopRight => boundingRect.topLeft,
            _ => throw new UnreachableException($"Unknown / unimplemented {nameof(RightAngleLocation)}: {rightAngleLocation}.")
        };
        /// <summary>
        /// The right-most of the two corners that don't contain the right angle.
        /// </summary>
        /// <remarks>
        /// This will be different from <see cref="leftCorner"/>, unless the <see cref="RightTriangle"/> is a single point.
        /// </remarks>
        /// <seealso cref="bottomCorner"/>
        /// <seealso cref="topCorner"/>
        /// <seealso cref="leftCorner"/>
        /// <seealso cref="rightAngleCorner"/>
        public IntVector2 rightCorner => rightAngleLocation switch
        {
            RightAngleLocation.BottomLeft => boundingRect.bottomRight,
            RightAngleLocation.BottomRight => boundingRect.topRight,
            RightAngleLocation.TopLeft => boundingRect.topRight,
            RightAngleLocation.TopRight => boundingRect.bottomRight,
            _ => throw new UnreachableException($"Unknown / unimplemented {nameof(RightAngleLocation)}: {rightAngleLocation}.")
        };
        /// <summary>
        /// The corner that contains the right angle.
        /// </summary>
        /// <seealso cref="bottomCorner"/>
        /// <seealso cref="topCorner"/>
        /// <seealso cref="leftCorner"/>
        /// <seealso cref="rightCorner"/>
        public IntVector2 rightAngleCorner => rightAngleLocation switch
        {
            RightAngleLocation.BottomLeft => boundingRect.bottomLeft,
            RightAngleLocation.BottomRight => boundingRect.bottomRight,
            RightAngleLocation.TopLeft => boundingRect.topLeft,
            RightAngleLocation.TopRight => boundingRect.topRight,
            _ => throw new UnreachableException($"Unknown / unimplemented {nameof(RightAngleLocation)}: {rightAngleLocation}.")
        };

        public bool filled { get; set; }

        /// <summary>
        /// Whether the <see cref="RightTriangle"/> is an isosceles triangle.
        /// </summary>
        /// <remarks>
        /// This is equivalent to its <see cref="boundingRect"/> being a square.
        /// </remarks>
        public bool isIsosceles => boundingRect.isSquare;

        /// <summary>
        /// Which corner of the <see cref="boundingRect"/> the right angle is in.
        /// </summary>
        public RightAngleLocation rightAngleLocation { get; set; }
        /// <summary>
        /// Which corner of the <see cref="boundingRect"/> the right angle is in.
        /// </summary>
        public enum RightAngleLocation
        {
            BottomLeft,
            BottomRight,
            TopLeft,
            TopRight,
        }
        /// <summary>
        /// Converts the <see cref="RightAngleLocation"/> into a 2-vector with +/- 1 in each component, representing the direction of that corner.
        /// </summary>
        /// <remarks>
        /// This is used with <see cref="FromDirection(IntVector2)"/> to simplify rotating/reflecting <see cref="RightAngleLocation"/>s.
        /// </remarks>
        private IntVector2 AsDirection(RightAngleLocation rightAngleLocation) => rightAngleLocation switch
        {
            RightAngleLocation.BottomLeft => (-1, -1),
            RightAngleLocation.BottomRight => (1, -1),
            RightAngleLocation.TopLeft => (-1, 1),
            RightAngleLocation.TopRight => (1, 1),
            _ => throw new UnreachableException($"Unknown / unimplemented {nameof(RightAngleLocation)}: {rightAngleLocation}.")
        };
        /// <summary>
        /// The inverse of <see cref="AsDirection"/>.
        /// </summary>
        /// <remarks>
        /// This is used with <see cref="AsDirection(RightAngleLocation)"/> to simplify rotating/reflecting <see cref="RightAngleLocation"/>s.
        /// </remarks>
        private RightAngleLocation FromDirection(IntVector2 direction) => direction switch
        {
            (-1, -1) => RightAngleLocation.BottomLeft,
            (1, -1) => RightAngleLocation.BottomRight,
            (-1, 1) => RightAngleLocation.TopLeft,
            (1, 1) => RightAngleLocation.TopRight,
            _ => throw new ArgumentException($"Invalid value for {nameof(direction)}: {direction}. It must be +/- 1 in each component.")
        };

        public IntRect boundingRect { get; set; }

        public int Count
        {
            get
            {
                if (boundingRect.width == 1 || boundingRect.height == 1)
                {
                    return boundingRect.Count;
                }

                if (!filled || boundingRect.width == 2 || boundingRect.height == 2)
                {
                    return border.Count;
                }

                IntVector2 rightAngleCorner = this.rightAngleCorner;
                if (boundingRect.width >= boundingRect.height)
                {
                    int count = boundingRect.height; // The vertical edge of the triangle won't be counted in the loop
                    foreach (IntVector2 point in border.lines[0])
                    {
                        count += Math.Abs(point.y - rightAngleCorner.y) + 1;
                    }
                    return count;
                }
                else
                {
                    int count = boundingRect.width; // The horizontal edge of the triangle won't be counted in the loop
                    foreach (IntVector2 point in border.lines[0])
                    {
                        count += Math.Abs(point.x - rightAngleCorner.x) + 1;
                    }
                    return count;
                }
            }
        }

        /// <summary>
        /// The outline of the <see cref="RightTriangle"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// This will not self-intersect.
        /// </para>
        /// <para>
        /// This will be a loop, except in the 2xn and nx2 cases for n >= 4.
        /// </para>
        /// </remarks>
        private Path border
        {
            get
            {
                // Single points / vertical lines / horizontal lines
                if (boundingRect.width == 1 || boundingRect.height == 1)
                {
                    return new Line(bottomCorner, topCorner);
                }

                (IntVector2 longerCorner, IntVector2 shorterCorner, IntVector2 longerCornerOffset, IntVector2 shorterCornerOffset) = rightAngleLocation switch
                {
                    RightAngleLocation.BottomLeft => (boundingRect.bottomRight, boundingRect.topLeft, IntVector2.up, IntVector2.right),
                    RightAngleLocation.BottomRight => (boundingRect.bottomLeft, boundingRect.topRight, IntVector2.up, IntVector2.left),
                    RightAngleLocation.TopLeft => (boundingRect.topRight, boundingRect.bottomLeft, IntVector2.down, IntVector2.right),
                    RightAngleLocation.TopRight => (boundingRect.topLeft, boundingRect.bottomRight, IntVector2.down, IntVector2.left),
                    _ => throw new UnreachableException($"Unknown / unimplemented {nameof(RightAngleLocation)}: {rightAngleLocation}.")
                };
                if (boundingRect.width < boundingRect.height)
                {
                    (longerCorner, shorterCorner) = (shorterCorner, longerCorner);
                    (longerCornerOffset, shorterCornerOffset) = (shorterCornerOffset, longerCornerOffset);
                }

                // Override shape of 2xn and nx2 triangles
                if (boundingRect.width == 2 || boundingRect.height == 2)
                {
                    // In this case the border doesn't necessarily form a loop
                    return new Path(
                        new Line(shorterCorner + (longerCorner + longerCornerOffset - shorterCorner) / 2, shorterCorner),
                        new Line(rightAngleCorner, longerCorner)
                        );
                }

                return new Path(
                    new Line(longerCorner + longerCornerOffset, shorterCorner + shorterCornerOffset),
                    new Line(shorterCorner, rightAngleCorner),
                    new Line(rightAngleCorner, longerCorner)
                    );
            }
        }

        /// <summary>
        /// Creates the largest <see cref="RightTriangle"/> that can fit in the given rect, and that has the right angle in the specified corner of the rect.
        /// </summary>
        /// <param name="boundingRect">See <see cref="boundingRect"/>.</param>
        /// <param name="rightAngleLocation">See <see cref="rightAngleLocation"/>.</param>
        /// <param name="filled">See <see cref="filled"/>.</param>
        public RightTriangle(IntRect boundingRect, RightAngleLocation rightAngleLocation, bool filled)
        {
            this.boundingRect = boundingRect;
            this.rightAngleLocation = rightAngleLocation;
            this.filled = filled;
        }
        /// <summary>
        /// Creates a <see cref="RightTriangle"/> with the two given non-right-angle corners.
        /// </summary>
        /// <param name="corner">One of the two non-right-angle corners of the <see cref="RightTriangle"/>.</param>
        /// <param name="oppositeCorner">The non-right-angle corner opposite <paramref name="corner"/>.</param>
        /// <param name="horizontalEdgeIsBottomEdge">Whether the horizontal edge of the <see cref="RightTriangle"/> is the bottom edge (as opposed to the top edge).</param>
        /// <param name="filled">See <see cref="filled"/>.</param>
        public RightTriangle(IntVector2 corner, IntVector2 oppositeCorner, bool horizontalEdgeIsBottomEdge, bool filled)
        {
            boundingRect = new IntRect(corner, oppositeCorner);
            this.filled = filled;

            rightAngleLocation = (corner.x >= oppositeCorner.x == corner.y >= oppositeCorner.y, horizontalEdgeIsBottomEdge) switch
            {
                (false, false) => RightAngleLocation.TopRight,
                (false, true) => RightAngleLocation.BottomLeft,
                (true, false) => RightAngleLocation.TopLeft,
                (true, true) => RightAngleLocation.BottomRight
            };
        }

        public bool Contains(IntVector2 point)
        {
            if (!filled)
            {
                return border.Contains(point);
            }

            // These cases are separate as the winding number is only defined for paths that are loops, but these cases don't necessarily give loops
            if (boundingRect.width <= 2 || boundingRect.height <= 2)
            {
                return border.Contains(point);
            }

            return boundingRect.ContainsX(point.x) && border.MinY(point.x) <= point.y && point.y <= border.MaxY(point.x);
        }

        /// <summary>
        /// Returns a deep copy of the <see cref="RightTriangle"/> translated by the given vector.
        /// </summary>
        /// <seealso cref="Translate(IntVector2)"/>
        public static RightTriangle operator +(RightTriangle triangle, IntVector2 translation) => triangle.Translate(translation);
        /// <summary>
        /// Returns a deep copy of the <see cref="RightTriangle"/> translated by the given vector.
        /// </summary>
        /// <seealso cref="Translate(IntVector2)"/>
        public static RightTriangle operator +(IntVector2 translation, RightTriangle triangle) => triangle + translation;
        /// <summary>
        /// Returns a deep copy of the <see cref="RightTriangle"/> translated by the given vector.
        /// </summary>
        /// <seealso cref="Translate(IntVector2)"/>
        public static RightTriangle operator -(RightTriangle triangle, IntVector2 translation) => triangle + (-translation);
        /// <summary>
        /// Returns a deep copy of the <see cref="RightTriangle"/> rotated 180 degrees about the origin (equivalently, reflected through the origin).
        /// </summary>
        /// <seealso cref="Rotate(RotationAngle)"/>
        /// <seealso cref="Flip(FlipAxis)"/>
        public static RightTriangle operator -(RightTriangle triangle) => triangle.Rotate(RotationAngle._180);

        public RightTriangle Translate(IntVector2 translation) => new RightTriangle(boundingRect + translation, rightAngleLocation, filled);
        public RightTriangle Flip(FlipAxis axis) => new RightTriangle(boundingRect.Flip(axis), FromDirection(AsDirection(rightAngleLocation).Flip(axis)), filled);
        public RightTriangle Rotate(RotationAngle angle) => new RightTriangle(boundingRect.Rotate(angle), FromDirection(AsDirection(rightAngleLocation).Rotate(angle)), filled);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<IntVector2> GetEnumerator()
        {
            Path border = this.border;
            foreach (IntVector2 point in border)
            {
                yield return point;
            }

            if (!filled || boundingRect.width <= 2 || boundingRect.height <= 2)
            {
                yield break;
            }

            IntVector2 directionToTopCorner = IntVector2.Simplify(topCorner - rightAngleCorner); // either IntVector2.up, IntVector2.left or IntVector2.right
            IntVector2 directionToBottomCorner = IntVector2.Simplify(bottomCorner - rightAngleCorner); // either IntVector2.down, IntVector2.left or IntVector2.right

            for (IntVector2 scanLineStart = rightAngleCorner + directionToTopCorner + directionToBottomCorner; !border.Contains(scanLineStart); scanLineStart += directionToTopCorner)
            {
                for (IntVector2 point = scanLineStart; !border.Contains(point); point += directionToBottomCorner)
                {
                    yield return point;
                }
            }
        }

        /// <summary>
        /// Whether the two <see cref="RightTriangle"/>s have the same shape, and the same <see cref="rightAngleLocation"/>.
        /// </summary>
        public static bool operator ==(RightTriangle a, RightTriangle b) => a.boundingRect == b.boundingRect && a.rightAngleCorner == b.rightAngleCorner && a.filled == b.filled;
        /// <summary>
        /// See <see cref="operator ==(RightTriangle, RightTriangle)"/>.
        /// </summary>
        public static bool operator !=(RightTriangle a, RightTriangle b) => !(a == b);
        /// <summary>
        /// See <see cref="operator ==(RightTriangle, RightTriangle)"/>.
        /// </summary>
        public bool Equals(RightTriangle other) => this == other;
        /// <summary>
        /// See <see cref="Equals(RightTriangle)"/>.
        /// </summary>
        public override bool Equals(object obj) => obj is RightTriangle other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(boundingRect, rightAngleLocation, filled);

        public override string ToString() => $"{nameof(RightTriangle)}({boundingRect}, {rightAngleLocation}, {(filled ? "filled" : "unfilled")})";

        public RightTriangle DeepCopy() => new RightTriangle(boundingRect, rightAngleLocation, filled);
    }
}