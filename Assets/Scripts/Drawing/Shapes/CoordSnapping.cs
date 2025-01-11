using System;

using PAC.DataStructures;

using UnityEngine;

namespace PAC.Drawing
{
    /// <summary>
    /// Provides methods to snap coordinates so they fit certain properties, such as forming a square.
    /// </summary>
    public static class CoordSnapping
    {
        /// <summary>
        /// Either changes the end coord's x or changes its y so that the rect it forms with the start coord is a square. Chooses the largest such square.
        /// </summary>
        public static IntVector2 SnapToSquare(IntVector2 start, IntVector2 end)
        {
            int sideLength = Math.Max(Math.Abs(end.x - start.x), Mathf.Abs(end.y - start.y));
            return start + new IntVector2(sideLength * Math.Sign(end.x - start.x), sideLength * Math.Sign(end.y - start.y));
        }
    }
}