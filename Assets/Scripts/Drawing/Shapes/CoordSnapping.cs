using System;

using PAC.DataStructures;

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
    }
}