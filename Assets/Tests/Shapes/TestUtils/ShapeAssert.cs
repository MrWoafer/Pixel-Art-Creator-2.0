using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using PAC.DataStructures;
using PAC.Shapes.Interfaces;

namespace PAC.Tests.Shapes.TestUtils
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
        public static void SameGeometry(IEnumerable<IntVector2> expected, IEnumerable<IntVector2> actual, string failMessage) => Assert.True(SameGeometry_Impl(expected, actual), failMessage);
        public static bool SameGeometry_Impl(IEnumerable<IntVector2> expected, IEnumerable<IntVector2> actual) => expected.ToHashSet().SetEquals(actual);

        /// <summary>
        /// Asserts that no pixels are repeated at all in the shape's enumerator.
        /// </summary>
        public static void NoRepeats(IShape shape)
        {
            HashSet<IntVector2> visited = new HashSet<IntVector2>();
            foreach (IntVector2 pixel in shape)
            {
                Assert.False(visited.Contains(pixel), $"Failed with {shape} and {pixel}.");
                visited.Add(pixel);
            }
        }

        /// <summary>
        /// Asserts that every pixel has another pixel adjacent to it (including diagonally).
        /// </summary>
        /// <remarks>
        /// Note this is a strictly weaker assertion than <see cref="Connected(IShape)"/>.
        /// </remarks>
        public static void NoIsolatedPoints(IShape shape)
        {
            HashSet<IntVector2> pixels = shape.ToHashSet();
            foreach (IntVector2 pixel in shape)
            {
                bool hasAdjacent = false;
                foreach (IntVector2 check in (pixel + new IntRect(-IntVector2.one, IntVector2.one)).Where(p => p != pixel))
                {
                    if (pixels.Contains(check))
                    {
                        hasAdjacent = true;
                        break;
                    }
                }

                Assert.True(hasAdjacent, $"Failed with {shape} and {pixel}.");
            }
        }

        /// <summary>
        /// Asserts that the shape has exactly one connected component (defined it terms of pixels being adjacent, including diagonally).
        /// </summary>
        public static void Connected(IShape shape)
        {
            HashSet<IntVector2> pixels = shape.ToHashSet();
            HashSet<IntVector2> visited = new HashSet<IntVector2>();
            Queue<IntVector2> toVisit = new Queue<IntVector2>();

            IntVector2 startingPixel = pixels.First();
            toVisit.Enqueue(startingPixel);
            visited.Add(startingPixel);

            while (toVisit.Count > 0)
            {
                IntVector2 pixel = toVisit.Dequeue();
                foreach (IntVector2 offsetPixel in pixel + new IntRect(-IntVector2.one, IntVector2.one))
                {
                    if (pixels.Contains(offsetPixel) && !visited.Contains(offsetPixel))
                    {
                        toVisit.Enqueue(offsetPixel);
                        visited.Add(offsetPixel);

                        if (visited.Count == pixels.Count)
                        {
                            break;
                        }
                    }
                }
            }

            Assert.AreEqual(pixels.Count, visited.Count, $"Failed with {shape}.");
        }

        /// <summary>
        /// Asserts that the shape has reflective symmetry across the given axis.
        /// </summary>
        public static void ReflectiveSymmetry(IShape shape, FlipAxis axis)
        {
            CollectionAssert.AreEquivalent(shape.ToHashSet(), shape.Select(p => p.Flip(axis) + shape.boundingRect.bottomLeft - shape.boundingRect.Flip(axis).bottomLeft).ToHashSet(),
                $"Failed with {shape} and FlipAxis.{axis}.");
        }

        /// <summary>
        /// Asserts that the shape doesn't have reflective symmetry across the given axis.
        /// </summary>
        public static void ReflectiveAsymmetry(IShape shape, FlipAxis axis)
        {
            CollectionAssert.AreNotEquivalent(shape.ToHashSet(), shape.Select(p => p.Flip(axis) + shape.boundingRect.bottomLeft - shape.boundingRect.Flip(axis).bottomLeft).ToHashSet(),
                $"Failed with {shape} and FlipAxis.{axis}.");
        }

        /// <summary>
        /// Asserts that the shape has rotational symmetry by the given angle.
        /// </summary>
        public static void RotationalSymmetry(IRotatableShape<IShape> shape, RotationAngle angle)
        {
            CollectionAssert.AreEquivalent(shape.ToHashSet(), shape.Select(p => p.Rotate(angle) + shape.boundingRect.bottomLeft - shape.boundingRect.Rotate(angle).bottomLeft).ToHashSet(),
                $"Failed with {shape} and RotationAngle.{angle}.");
        }

        /// <summary>
        /// Asserts that the shape has 180-degree rotational symmetry.
        /// </summary>
        public static void RotationalSymmetry180(IIsometricShape<IShape> shape)
        {
            CollectionAssert.AreEquivalent(shape.ToHashSet(), shape.Select(p =>
                p.Flip(FlipAxis.Vertical).Flip(FlipAxis.Horizontal) + shape.boundingRect.bottomLeft - shape.boundingRect.Flip(FlipAxis.Vertical).Flip(FlipAxis.Horizontal).bottomLeft
                ).ToHashSet(),
                $"Failed with {shape}.");
        }

        /// <summary>
        /// Asserts that the unfilled version of the shape is precisely the border of the filled version.
        /// </summary>
        public static void UnfilledIsBorderOfFilled(IFillableShape shape)
        {
            shape.filled = true;
            HashSet<IntVector2> borderOfFilled = ShapeUtils.GetBorder(shape);

            shape.filled = false;
            Assert.True(borderOfFilled.SetEquals(shape), $"Failed with {shape}.");
        }
    }
}