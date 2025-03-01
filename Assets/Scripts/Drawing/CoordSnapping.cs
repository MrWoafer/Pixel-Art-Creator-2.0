using System;
using System.Collections.Generic;

using PAC.DataStructures;
using PAC.Exceptions;
using PAC.Maths;
using PAC.Geometry.Shapes;
using PAC.Geometry;

namespace PAC.Drawing
{
    /// <summary>
    /// Provides methods to snap coordinates so they fit certain properties, such as forming a square.
    /// </summary>
    public static class CoordSnapping
    {
        /// <summary>
        /// Either changes <paramref name="movablePoint"/>'s x coord <i>or</i> changes its y coord so that the rectangle it forms with <paramref name="fixedPoint"/> is a square.
        /// </summary>
        /// <remarks>
        /// Chooses the largest such square that preserves the sign of each component of <c><paramref name="movablePoint"/> - <paramref name="fixedPoint"/></c>.
        /// If <paramref name="fixedPoint"/> and <paramref name="movablePoint"/> have the same x coord (but different y), the square will be made by increasing the x coord;
        /// if <paramref name="fixedPoint"/> and <paramref name="movablePoint"/> have the same y coord (but different x), the square will be made by increasing the y coord.
        /// </remarks>
        public static IntVector2 SnapToSquare(IntVector2 fixedPoint, IntVector2 movablePoint)
        {
            IntVector2 sign = IntVector2.Sign(movablePoint - fixedPoint);
            sign = new IntVector2(
                sign.x == 0 ? 1 : sign.x,
                sign.y == 0 ? 1 : sign.y
                );

            int xDifference = Math.Abs(movablePoint.x - fixedPoint.x);
            int yDifference = Math.Abs(movablePoint.y - fixedPoint.y);
            // Actually this is the side length - 1. But if we do + 1 here to get the actual side length, we'll just have to do - 1 in the next line.
            int squareSideLength = Math.Max(xDifference, yDifference);

            return fixedPoint + squareSideLength * sign;
        }

        private enum RotationDirection
        {
            Anticlockwise = -1,
            Clockwise = 1
        }
        private enum DiamondCorner
        {
            Bottom,
            Top,
            Left,
            Right
        }
        /// <summary>
        /// Enumerates all points at an l1 distance of <paramref name="radius"/> from <paramref name="centre"/> (note this is a diamond shape), starting from <paramref name="startCorner"/> of the
        /// diamond, going round in the <paramref name="rotationDirection"/> direction.
        /// </summary>
        /// <param name="centre"></param>
        /// <param name="radius"></param>
        /// <param name="rotationDirection"></param>
        /// <param name="startCorner"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NotImplementedException"></exception>
        private static IEnumerable<IntVector2> EnumerateL1Circle(IntVector2 centre, int radius, RotationDirection rotationDirection, DiamondCorner startCorner)
        {
            if (radius < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(radius), $"{nameof(radius)} must be non-negative: {radius}.");
            }

            if (radius == 0)
            {
                yield return centre;
                yield break;
            }

            IntVector2 direction = (startCorner, rotationDirection) switch
            {
                (DiamondCorner.Bottom, RotationDirection.Clockwise) => (-1, 1),
                (DiamondCorner.Top, RotationDirection.Clockwise) => (1, -1),
                (DiamondCorner.Left, RotationDirection.Clockwise) => (1, 1),
                (DiamondCorner.Right, RotationDirection.Clockwise) => (-1, -1),
                (DiamondCorner.Bottom, RotationDirection.Anticlockwise) => (1, 1),
                (DiamondCorner.Top, RotationDirection.Anticlockwise) => (-1, -1),
                (DiamondCorner.Left, RotationDirection.Anticlockwise) => (1, -1),
                (DiamondCorner.Right, RotationDirection.Anticlockwise) => (-1, 1),
                _ => throw new NotImplementedException()
            };
            RotationAngle rotation = rotationDirection switch
            {
                RotationDirection.Clockwise => RotationAngle._90,
                RotationDirection.Anticlockwise => RotationAngle.Minus90,
                _ => throw new NotImplementedException()
            };

            IntVector2 start = startCorner switch
            {
                DiamondCorner.Bottom => centre + (0, -radius),
                DiamondCorner.Top => centre + (0, radius),
                DiamondCorner.Left => centre + (-radius, 0),
                DiamondCorner.Right => centre + (radius, 0),
                _ => throw new NotImplementedException()
            };

            IntVector2 point = start;
            do
            {
                yield return point;

                if (IntVector2.L1Distance(point + direction, centre) == radius)
                {
                    point += direction;
                }
                else
                {
                    direction = direction.Rotate(rotation);
                    point += direction;
                }
            } while (point != start);
        }

        /// <summary>
        /// Returns the closest (in l1 distance) point to <paramref name="movablePoint"/> that forms a perfect <see cref="Line"/> to <paramref name="fixedPoint"/>.
        /// </summary>
        /// <remarks>
        /// Note such a point may not be unique. This method breaks ties in such a way that applying an isometry to <paramref name="fixedPoint"/> and <paramref name="movablePoint"/> (the same
        /// one to both) applies the isometry to the snapped point.
        /// </remarks>
        /// <param name="maxDistance">The largest l1 distance from <paramref name="movablePoint"/> to search for a point to snap to.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="maxDistance"/> is negative.</exception>
        /// <exception cref="InvalidOperationException">There is no point to snap to within <paramref name="maxDistance"/> of <paramref name="movablePoint"/>.</exception>
        public static IntVector2 SnapToPerfectLine(IntVector2 fixedPoint, IntVector2 movablePoint, int maxDistance = 100)
        {
            /* How this method works:
             * 
             * If fixedPoint and movablePoint form a perfect Line, we are done. If not, we check the points at an l1 distance of 1 from movablePoint. If none of those work, we check the points
             * at an l1 distance of 2 from movablePoint. Etc. Note that { p : l1Distance(p, movablePoint) = r } is a diamond. E.g. for r = 2:
             * 
             *                 #
             *               #   #
             *             #   m   #      m = movablePoint
             *               #   #
             *                 #
             *   f                        f = fixedPoint
             * 
             * Once we find a point that works, we return it. There may be multiple points on a diamond that form a perfect Line, so the order we iterate over the diamond matters. In order to
             * have the desired isometry property, we iterate over the diamond starting at the point closest to a horizontal/vertical axis through fixedPoint (there is a unique such point because
             * if there weren't then movablePoint and fixedPoint are already on a horizontal / vertical / +/- 45 degree line, which we've already dealt with), going clockwise/anticlockwise so
             * that we start by going towards the other horizontal/vertical axis through fixedPoint. For example:
             *   
             *   |             4
             *   |           3   5
             *   |         2   m   6      m = movablePoint
             *   |           1   7
             *   |             0
             * --f---------------------   f = fixedPoint
             *   |
             */

            if (maxDistance < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(maxDistance), $"{nameof(maxDistance)} must be non-negative: {maxDistance}.");
            }

            Line line = new Line(fixedPoint, movablePoint); // we edit this instead of reallocating
            if (line.isPerfect)
            {
                return movablePoint;
            }

            (DiamondCorner startCorner, RotationDirection rotationDirection) = Plane2D.GetOctant(movablePoint - fixedPoint) switch
            {
                Plane2D.Octant.NorthNortheast => (DiamondCorner.Left, RotationDirection.Anticlockwise),
                Plane2D.Octant.EastNortheast => (DiamondCorner.Bottom, RotationDirection.Clockwise),
                Plane2D.Octant.EastSoutheast => (DiamondCorner.Top, RotationDirection.Anticlockwise),
                Plane2D.Octant.SouthSoutheast => (DiamondCorner.Left, RotationDirection.Clockwise),
                Plane2D.Octant.SouthSouthwest => (DiamondCorner.Right, RotationDirection.Anticlockwise),
                Plane2D.Octant.WestSouthwest => (DiamondCorner.Top, RotationDirection.Clockwise),
                Plane2D.Octant.WestNorthwest => (DiamondCorner.Bottom, RotationDirection.Anticlockwise),
                Plane2D.Octant.NorthNorthwest => (DiamondCorner.Right, RotationDirection.Clockwise),
                _ => throw new UnreachableException("We should have already dealt with the cases where it's in more than one octant.")
            };

            // The last two values in the min are because the diamonds with these radii will contain a point with the same x or y coord as fixedPoint, which will form a perfect Line
            int maxRadius = MathExtensions.Min(maxDistance, Math.Abs(fixedPoint.x - movablePoint.x), Math.Abs(fixedPoint.y - movablePoint.y));

            for (int radius = 1; radius <= maxRadius; radius++)
            {
                foreach (IntVector2 endPoint in EnumerateL1Circle(movablePoint, radius, rotationDirection, startCorner))
                {
                    line.end = endPoint;

                    if (line.isPerfect)
                    {
                        return endPoint;
                    }
                }
            }

            throw new InvalidOperationException($"Could not find a point to snap to within {nameof(maxDistance)} = {maxDistance}.");
        }
    }
}