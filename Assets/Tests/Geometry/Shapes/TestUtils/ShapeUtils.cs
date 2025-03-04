using System.Collections.Generic;
using System.Linq;

using PAC.DataStructures;
using PAC.Geometry.Shapes.Interfaces;

namespace PAC.Tests.Geometry.Shapes.TestUtils
{
    public static class ShapeUtils
    {
        /// <summary>
        /// Returns the set of points in the shape such that at least one of their 4 directly-adjacent points (up / down / left / right) is not in the shape.
        /// </summary>
        public static HashSet<IntVector2> GetBorder(IEnumerable<IntVector2> shape)
        {
            HashSet<IntVector2> points = Enumerable.ToHashSet(shape);
            HashSet<IntVector2> border = new HashSet<IntVector2>();
            foreach (IntVector2 point in points)
            {
                foreach (IntVector2 offset in IntVector2.upDownLeftRight)
                {
                    if (!points.Contains(point + offset))
                    {
                        border.Add(point);
                        break;
                    }
                }
            }
            return border;
        }
    }
}