using System;
using System.Collections;
using System.Collections.Generic;

using PAC.DataStructures;
using PAC.Shapes.Interfaces;

namespace PAC.Shapes
{
    public class RightTriangle : I2DShape<RightTriangle>, IDeepCopyableShape<RightTriangle>, IEquatable<RightTriangle>
    {
        public enum RightAngleLocation
        {
            Bottom = -1,
            Top = 1,
            Left = -2,
            Right = 2
        }

        private IntVector2 _bottomCorner;
        /// <summary>
        /// The lower of the two corners that don't contain the right angle. This should always be distinct from topCorner, unless the triangle is a single point.
        /// </summary>
        public IntVector2 bottomCorner
        {
            get => _bottomCorner;
            set
            {
                if (value.y <= _topCorner.y)
                {
                    _bottomCorner = value;
                }
                else
                {
                    _bottomCorner = _topCorner;
                    _topCorner = value;
                }
            }
        }
        private IntVector2 _topCorner;
        /// <summary>
        /// The higher of the two corners that don't contain the right angle. This should always be distinct from bottomCorner, unless the triangle is a single point.
        /// </summary>
        public IntVector2 topCorner
        {
            get => _topCorner;
            set
            {
                if (value.y >= _bottomCorner.y)
                {
                    _topCorner = value;
                }
                else
                {
                    _topCorner = _bottomCorner;
                    _bottomCorner = value;
                }
            }
        }
        /// <summary>
        /// The left-most of the two corners that don't contain the right angle. This should always be distinct from rightCorner, unless the triangle is a single point.
        /// </summary>
        public IntVector2 leftCorner
        {
            get => _bottomCorner.x <= _topCorner.x ? _bottomCorner : _topCorner;
            set
            {
                if (leftCorner == _bottomCorner)
                {
                    bottomCorner = value;
                }
                else
                {
                    topCorner = value;
                }
            }
        }
        /// <summary>
        /// The right-most of the two corners that don't contain the right angle. This should always be distinct from leftCorner, unless the triangle is a single point.
        /// </summary>
        public IntVector2 rightCorner
        {
            // We do > here and <= in leftCorner to ensure that if the triangle's corners have the same x coord then leftCorner and rightCorner are still different corners.
            get => _bottomCorner.x > _topCorner.x ? _bottomCorner : _topCorner;
            set
            {
                if (rightCorner == _bottomCorner)
                {
                    bottomCorner = value;
                }
                else
                {
                    topCorner = value;
                }
            }
        }
        /// <summary>
        /// The corner that contains the right angle.
        /// </summary>
        public IntVector2 rightAngleCorner
        {
            get
            {
                switch (rightAngleLocation)
                {
                    case RightAngleLocation.Bottom: return new IntVector2(topCorner.x, bottomCorner.y);
                    case RightAngleLocation.Top: return new IntVector2(bottomCorner.x, topCorner.y);
                    case RightAngleLocation.Left: return new IntVector2(leftCorner.x, rightCorner.y);
                    case RightAngleLocation.Right: return new IntVector2(rightCorner.x, leftCorner.y);
                    default: throw new NotImplementedException("Unknown / unimplemented RightAngleLocation: " + rightAngleLocation);
                }
            }
        }

        public RightAngleLocation rightAngleLocation { get; set; }

        public bool filled { get; set; }

        /// <summary>True if the triangle is an isosceles triangle.</summary>
        public bool isIsosceles => boundingRect.width == boundingRect.height;

        public IntRect boundingRect => new IntRect(bottomCorner, topCorner);

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

                if (!filled || boundingRect.width == 2 || boundingRect.height == 2)
                {
                    return border.Count;
                }

                if (boundingRect.width >= boundingRect.height)
                {
                    // The vertical side of the triangle won't be counted in the loop
                    int count = boundingRect.height;
                    // Go along the hypotenuse
                    foreach (IntVector2 pixel in border.lines[0])
                    {
                        count += Math.Abs(pixel.y - rightAngleCorner.y) + 1;
                    }
                    return count;
                }
                else
                {
                    // The horizontal side of the triangle won't be counted in the loop
                    int count = boundingRect.width;
                    // Go along the hypotenuse
                    foreach (IntVector2 pixel in border.lines[0])
                    {
                        count += Math.Abs(pixel.x - rightAngleCorner.x) + 1;
                    }
                    return count;
                }
            }
        }

        private Path border
        {
            get
            {
                // Single points / vertical lines / horizontal lines
                if (bottomCorner.x == topCorner.x || bottomCorner.y == topCorner.y)
                {
                    return new Path(new Line(bottomCorner, topCorner));
                }

                IntVector2 startCorner = bottomCorner;
                IntVector2 endCorner = topCorner;
                IntVector2 startAdjusted;
                IntVector2 endAdjusted;

                if (rightAngleLocation == RightAngleLocation.Bottom)
                {
                    startAdjusted = bottomCorner + IntVector2.up;
                    endAdjusted = topCorner + (startCorner == leftCorner ? IntVector2.left : IntVector2.right);
                }
                else if (rightAngleLocation == RightAngleLocation.Top)
                {
                    startAdjusted = bottomCorner + (endCorner == leftCorner ? IntVector2.left : IntVector2.right);
                    endAdjusted = topCorner + IntVector2.down;
                }
                else if (rightAngleLocation == RightAngleLocation.Left)
                {
                    startAdjusted = bottomCorner + (startCorner == leftCorner ? IntVector2.right : IntVector2.up);
                    endAdjusted = topCorner + (startCorner == leftCorner ? IntVector2.down : IntVector2.right);
                }
                else if (rightAngleLocation == RightAngleLocation.Right)
                {
                    startAdjusted = bottomCorner + (startCorner == leftCorner ? IntVector2.up : IntVector2.left);
                    endAdjusted = topCorner + (startCorner == leftCorner ? IntVector2.left : IntVector2.down);
                }
                else
                {
                    throw new NotImplementedException("Unknown / unimplemented RightAngleLocation: " + rightAngleLocation);
                }

                // This is to ensure reflecting doesn't change the shape (up to reflecting)
                // If width >= height, we draw the hypotenuse starting from the corner with the same y coord as the right angle corner
                // If width < height, we draw the hypotenuse starting from the corner with the same x coord as the right angle corner
                if (boundingRect.width < boundingRect.height != (startCorner.y != rightAngleCorner.y))
                {
                    IntVector2 temp;

                    temp = startCorner;
                    startCorner = endCorner;
                    endCorner = temp;

                    temp = startAdjusted;
                    startAdjusted = endAdjusted;
                    endAdjusted = temp;
                }

                // Override shape of 2xn and nx2 triangles to be more aesthetic (otherwise they are just diamonds).
                if (boundingRect.width == 2 && boundingRect.height == 2)
                {
                    startAdjusted = startCorner;
                    endAdjusted = endCorner;
                }
                if (boundingRect.width == 2 || boundingRect.height == 2)
                {
                    startAdjusted = endAdjusted + (startAdjusted - endAdjusted) / 2;
                }

                // The line order is start corner -> end corner -> right angle corner -> start corner
                return new Path(new Line(startAdjusted, endAdjusted), new Line(endCorner, rightAngleCorner), new Line(rightAngleCorner, startCorner));
            }
        }

        public RightTriangle(IntVector2 corner, IntVector2 oppositeCorner, RightAngleLocation rightAngleLocation, bool filled)
        {
            if (corner.y <= oppositeCorner.y)
            {
                _bottomCorner = corner;
                _topCorner = oppositeCorner;
            }
            else
            {
                _bottomCorner = oppositeCorner;
                _topCorner = corner;
            }

            this.rightAngleLocation = rightAngleLocation;
            this.filled = filled;
        }

        public bool Contains(IntVector2 pixel)
        {
            if (!filled)
            {
                return border.Contains(pixel);
            }

            // These cases are separate as the winding number is only defined for paths that are loops, but these cases don't give loops.
            // (Actually the 1x1, 1x2 and 2x1 cases don't need to be included in this, but it's easier to just include them.)
            if (boundingRect.width <= 2 || boundingRect.height <= 2)
            {
                return border.Contains(pixel);
            }
            return border.Contains(pixel) || border.WindingNumber(pixel) != 0;
        }

        /// <summary>
        /// Translates the triangle by the given vector.
        /// </summary>
        public static RightTriangle operator +(RightTriangle triangle, IntVector2 translation) => triangle.Translate(translation);
        /// <summary>
        /// Translates the triangle by the given vector.
        /// </summary>
        public static RightTriangle operator +(IntVector2 translation, RightTriangle triangle) => triangle + translation;
        /// <summary>
        /// Translates the triangle by the given vector.
        /// </summary>
        public static RightTriangle operator -(RightTriangle triangle, IntVector2 translation) => triangle + -translation;
        /// <summary>
        /// Reflects the triangle through the origin.
        /// </summary>
        public static RightTriangle operator -(RightTriangle triangle) => triangle.Rotate(RotationAngle._180);

        /// <summary>
        /// Translates the triangle by the given vector.
        /// </summary>
        public RightTriangle Translate(IntVector2 translation) => new RightTriangle(bottomCorner + translation, topCorner + translation, rightAngleLocation, filled);

        /// <summary>
        /// Reflects the triangle across the given axis.
        /// </summary>
        public RightTriangle Flip(FlipAxis axis)
        {
            RightAngleLocation FlipRightAngleLocation(RightAngleLocation rightAngleLocation, FlipAxis axis) => axis switch
            {
                FlipAxis.None => rightAngleLocation,
                FlipAxis.Vertical => rightAngleLocation switch
                {
                    RightAngleLocation.Top => RightAngleLocation.Top,
                    RightAngleLocation.Bottom => RightAngleLocation.Bottom,
                    RightAngleLocation.Left => RightAngleLocation.Right,
                    RightAngleLocation.Right => RightAngleLocation.Left,
                    _ => throw new NotImplementedException("Unknown / unimplemented RightAngleLocation: " + rightAngleLocation),
                },
                FlipAxis.Horizontal => rightAngleLocation switch
                {
                    RightAngleLocation.Top => RightAngleLocation.Bottom,
                    RightAngleLocation.Bottom => RightAngleLocation.Top,
                    RightAngleLocation.Left => RightAngleLocation.Left,
                    RightAngleLocation.Right => RightAngleLocation.Right,
                    _ => throw new NotImplementedException("Unknown / unimplemented RightAngleLocation: " + rightAngleLocation),
                },
                FlipAxis._45Degrees => rightAngleLocation switch
                {
                    RightAngleLocation.Top => RightAngleLocation.Right,
                    RightAngleLocation.Bottom => RightAngleLocation.Left,
                    RightAngleLocation.Left => RightAngleLocation.Bottom,
                    RightAngleLocation.Right => RightAngleLocation.Top,
                    _ => throw new NotImplementedException("Unknown / unimplemented RightAngleLocation: " + rightAngleLocation),
                },
                FlipAxis.Minus45Degrees => rightAngleLocation switch
                {
                    RightAngleLocation.Top => RightAngleLocation.Left,
                    RightAngleLocation.Bottom => RightAngleLocation.Right,
                    RightAngleLocation.Left => RightAngleLocation.Top,
                    RightAngleLocation.Right => RightAngleLocation.Bottom,
                    _ => throw new NotImplementedException("Unknown / unimplemented RightAngleLocation: " + rightAngleLocation),
                },
                _ => throw new NotImplementedException("Unknown / unimplemented FlipAxis: " + axis),
            };

            return new RightTriangle(bottomCorner.Flip(axis), topCorner.Flip(axis), FlipRightAngleLocation(rightAngleLocation, axis), filled);
        }

        /// <summary>
        /// Rotates the triangle by the given angle.
        /// </summary>
        public RightTriangle Rotate(RotationAngle angle)
        {
            RightAngleLocation RotateRightAngleLocation(RightAngleLocation rightAngleLocation, RotationAngle angle) => angle switch
            {
                RotationAngle._0 => rightAngleLocation,
                RotationAngle._90 => rightAngleLocation switch
                {
                    RightAngleLocation.Top => RightAngleLocation.Right,
                    RightAngleLocation.Bottom => RightAngleLocation.Left,
                    RightAngleLocation.Left => RightAngleLocation.Top,
                    RightAngleLocation.Right => RightAngleLocation.Bottom,
                    _ => throw new NotImplementedException("Unknown / unimplemented RightAngleLocation: " + rightAngleLocation),
                },
                RotationAngle._180 => rightAngleLocation switch
                {
                    RightAngleLocation.Top => RightAngleLocation.Bottom,
                    RightAngleLocation.Bottom => RightAngleLocation.Top,
                    RightAngleLocation.Left => RightAngleLocation.Right,
                    RightAngleLocation.Right => RightAngleLocation.Left,
                    _ => throw new NotImplementedException("Unknown / unimplemented RightAngleLocation: " + rightAngleLocation),
                },
                RotationAngle.Minus90 => rightAngleLocation switch
                {
                    RightAngleLocation.Top => RightAngleLocation.Left,
                    RightAngleLocation.Bottom => RightAngleLocation.Right,
                    RightAngleLocation.Left => RightAngleLocation.Bottom,
                    RightAngleLocation.Right => RightAngleLocation.Top,
                    _ => throw new NotImplementedException("Unknown / unimplemented RightAngleLocation: " + rightAngleLocation),
                },
                _ => throw new NotImplementedException("Unknown / unimplemented RotationAngle: " + angle),
            };

            return new RightTriangle(bottomCorner.Rotate(angle), topCorner.Rotate(angle), RotateRightAngleLocation(rightAngleLocation, angle), filled);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        public IEnumerator<IntVector2> GetEnumerator()
        {
            Path border = this.border;
            foreach (IntVector2 pixel in border)
            {
                yield return pixel;
            }

            if (!filled || boundingRect.width == 2 || boundingRect.height == 2)
            {
                yield break;
            }

            // either IntVector2.up, IntVector2.left or IntVector2.right
            IntVector2 directionToTopCorner = IntVector2.Simplify(topCorner - rightAngleCorner);
            // either IntVector2.down, IntVector2.left or IntVector2.right
            IntVector2 directionToBottomCorner = IntVector2.Simplify(bottomCorner - rightAngleCorner);

            for (IntVector2 rowStart = rightAngleCorner + directionToTopCorner + directionToBottomCorner; !border.Contains(rowStart); rowStart += directionToTopCorner)
            {
                for (IntVector2 pixel = rowStart; !border.Contains(pixel); pixel += directionToBottomCorner)
                {
                    yield return pixel;
                }
            }
        }

        public static bool operator ==(RightTriangle a, RightTriangle b)
            => a.bottomCorner == b.bottomCorner && a.topCorner == b.topCorner && a.rightAngleCorner == b.rightAngleCorner && a.filled == b.filled;
        public static bool operator !=(RightTriangle a, RightTriangle b) => !(a == b);
        public bool Equals(RightTriangle other) => this == other;
        public override bool Equals(object obj) => obj is RightTriangle other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(bottomCorner, topCorner, rightAngleLocation, filled);

        public override string ToString() => "RightTriangle(" + bottomCorner + ", " + topCorner + ", " + rightAngleLocation + ", " + (filled ? "filled" : "unfilled") + ")";

        public RightTriangle DeepCopy() => new RightTriangle(bottomCorner, topCorner, rightAngleLocation, filled);
    }
}