using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using PAC.DataStructures;
using PAC.Geometry;
using PAC.Geometry.Shapes.Interfaces;

namespace PAC.Tests.Geometry.Shapes.TestUtils
{
    /// <summary>
    /// Provides assertions for <see cref="IShape"/>s.
    /// </summary>
    public static class ShapeAssert
    {
        /// <summary>
        /// Asserts that the two shapes look the same, i.e. they are set-equal.
        /// </summary>
        public static void SameGeometry(IEnumerable<IntVector2> expected, IEnumerable<IntVector2> actual) => Assert.True(SameGeometry_Impl(expected, actual));
        /// <summary>
        /// Asserts that the two shapes look the same, i.e. they are set-equal.
        /// </summary>
        public static void SameGeometry(IEnumerable<IntVector2> expected, IEnumerable<IntVector2> actual, string failMessage) => Assert.True(SameGeometry_Impl(expected, actual), failMessage);
        private static bool SameGeometry_Impl(IEnumerable<IntVector2> expected, IEnumerable<IntVector2> actual) => Enumerable.ToHashSet(expected).SetEquals(actual);

        /// <summary>
        /// Asserts that the two shapes do not look the same, i.e. they are not set-equal.
        /// </summary>
        public static void NotSameGeometry(IEnumerable<IntVector2> expected, IEnumerable<IntVector2> actual) => Assert.False(SameGeometry_Impl(expected, actual));
        /// <summary>
        /// Asserts that the two shapes do not look the same, i.e. they are not set-equal.
        /// </summary>
        public static void NotSameGeometry(IEnumerable<IntVector2> expected, IEnumerable<IntVector2> actual, string failMessage) => Assert.False(SameGeometry_Impl(expected, actual), failMessage);

        /// <summary>
        /// Asserts that no points are repeated at all in the shape's enumerator.
        /// </summary>
        public static void NoRepeats(IEnumerable<IntVector2> shape)
        {
            HashSet<IntVector2> visited = new HashSet<IntVector2>();
            foreach (IntVector2 point in shape)
            {
                Assert.False(visited.Contains(point), $"Failed with {shape} and {point}.");
                visited.Add(point);
            }
        }

        /// <summary>
        /// Asserts that every point has another point adjacent to it (including diagonally).
        /// </summary>
        /// <remarks>
        /// Note this is a strictly weaker assertion than <see cref="Connected(IEnumerable{IntVector2})"/>.
        /// </remarks>
        public static void NoIsolatedPoints(IEnumerable<IntVector2> shape)
        {
            HashSet<IntVector2> points = Enumerable.ToHashSet(shape);
            foreach (IntVector2 point in shape)
            {
                bool hasAdjacent = false;
                foreach (IntVector2 check in (point + new IntRect((-1, -1), (1, 1))).Where(p => p != point))
                {
                    if (points.Contains(check))
                    {
                        hasAdjacent = true;
                        break;
                    }
                }

                Assert.True(hasAdjacent, $"Failed with {shape} and {point}.");
            }
        }

        /// <summary>
        /// Asserts that the shape has exactly one connected component (defined in terms of points being adjacent, including diagonally).
        /// </summary>
        public static void Connected(IEnumerable<IntVector2> shape)
        {
            HashSet<IntVector2> points = Enumerable.ToHashSet(shape);
            HashSet<IntVector2> visited = new HashSet<IntVector2>();
            Queue<IntVector2> toVisit = new Queue<IntVector2>();

            IntVector2 startingPoint = points.First();
            toVisit.Enqueue(startingPoint);
            visited.Add(startingPoint);

            while (toVisit.Count > 0)
            {
                IntVector2 point = toVisit.Dequeue();
                foreach (IntVector2 adjacentPoint in point + new IntRect((-1, -1), (1, 1)))
                {
                    if (points.Contains(adjacentPoint) && !visited.Contains(adjacentPoint))
                    {
                        toVisit.Enqueue(adjacentPoint);
                        visited.Add(adjacentPoint);

                        if (visited.Count == points.Count)
                        {
                            break;
                        }
                    }
                }
            }

            Assert.AreEqual(points.Count, visited.Count, $"Failed with {shape}.");
        }

        /// <summary>
        /// Asserts that the shape has reflective symmetry across the given axis.
        /// </summary>
        public static void ReflectiveSymmetry(IEnumerable<IntVector2> shape, FlipAxis axis)
        {
            if (!shape.Any())
            {
                return;
            }

            IntRect boundingRect = IntRect.BoundingRect(shape);
            ShapeAssert.SameGeometry(
                shape,
                shape.Select(p => p.Flip(axis) + boundingRect.bottomLeft - boundingRect.Flip(axis).bottomLeft),
                $"Failed with {shape} and FlipAxis.{axis}."
                );
        }

        /// <summary>
        /// Asserts that the shape doesn't have reflective symmetry across the given axis.
        /// </summary>
        public static void ReflectiveAsymmetry(IEnumerable<IntVector2> shape, FlipAxis axis)
        {
            if (!shape.Any())
            {
                return;
            }

            IntRect boundingRect = IntRect.BoundingRect(shape);
            ShapeAssert.NotSameGeometry(
                shape,
                shape.Select(p => p.Flip(axis) + boundingRect.bottomLeft - boundingRect.Flip(axis).bottomLeft),
                $"Failed with {shape} and FlipAxis.{axis}."
                );
        }

        /// <summary>
        /// Asserts that the shape has rotational symmetry by the given angle.
        /// </summary>
        public static void RotationalSymmetry(IEnumerable<IntVector2> shape, QuadrantalAngle angle)
        {
            if (!shape.Any())
            {
                return;
            }

            IntRect boundingRect = IntRect.BoundingRect(shape);
            ShapeAssert.SameGeometry(
                shape,
                shape.Select(p => p.Rotate(angle) + boundingRect.bottomLeft - boundingRect.Rotate(angle).bottomLeft),
                $"Failed with {shape} and RotationAngle.{angle}."
                );
            
        }

        /// <summary>
        /// Asserts that the unfilled version of the shape is precisely the border of the filled version.
        /// </summary>
        public static void UnfilledIsBorderOfFilled(IFillableShape shape)
        {
            shape.filled = true;
            HashSet<IntVector2> borderOfFilled = ShapeUtils.GetBorder(shape);

            shape.filled = false;
            ShapeAssert.SameGeometry(borderOfFilled, shape, $"Failed with {shape}.");
        }
    }
}