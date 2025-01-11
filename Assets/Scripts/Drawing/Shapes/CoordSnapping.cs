using System;

using PAC.DataStructures;

using UnityEngine;

namespace PAC.Drawing
{
    public static partial class Shapes
    {
        /// <summary>
        /// Either changes the end coord's x or changes its y so that the rect it forms with the start coord is a square. Chooses the largest such square.
        /// </summary>
        public static IntVector2 SnapEndCoordToSquare(IntVector2 start, IntVector2 end)
        {
            int sideLength = Math.Max(Math.Abs(end.x - start.x), Mathf.Abs(end.y - start.y));
            return start + new IntVector2(sideLength * Math.Sign(end.x - start.x), sideLength * Math.Sign(end.y - start.y));
        }
    }
}