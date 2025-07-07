using NUnit.Framework;

using PAC.Extensions.System;
using PAC.Geometry;
using PAC.Geometry.Axes;
using PAC.Geometry.Shapes;
using PAC.Tests.Geometry.Shapes.DefaultTests;
using PAC.Tests.Geometry.Shapes.RequiredTests;
using PAC.Tests.Geometry.Shapes.TestUtils;

using System;
using System.Collections.Generic;
using System.Linq;

namespace PAC.Tests.Geometry.Shapes
{
    /// <summary>
    /// Tests for <see cref="IsometricHexagon"/>.
    /// </summary>
    public class IsometricHexagon_Tests : IIsometricShape_DefaultTests<IsometricHexagon>, IIsometricShape_RequiredTests
    {
        protected override IEnumerable<IsometricHexagon> testCases => Enumerable.Concat(exampleTestCases, randomTestCases);
        private IEnumerable<IsometricHexagon> exampleTestCases => new bool[] { false, true }.SelectMany(filled => new IsometricHexagon[]
        {
            new IsometricHexagon(new IntRect((0, 0), (0, 0)), filled),
            new IsometricHexagon(new IntRect((0, 0), (0, 8)), filled),
            new IsometricHexagon(new IntRect((0, 0), (10, 0)), filled),
            new IsometricHexagon(new IntRect((0, 0), (1, 7)), filled),
            new IsometricHexagon(new IntRect((0, 0), (9, 1)), filled),
            new IsometricHexagon(new IntRect((0, 0), (7, 6)), filled),
            new IsometricHexagon(new IntRect((0, 0), (0, 1)), filled),
            new IsometricHexagon(new IntRect((0, 0), (1, 0)), filled),
            new IsometricHexagon(new IntRect((0, 0), (0, 2)), filled),
            new IsometricHexagon(new IntRect((0, 0), (1, 1)), filled),
            new IsometricHexagon(new IntRect((0, 0), (2, 0)), filled),
            new IsometricHexagon(new IntRect((0, 0), (0, 3)), filled),
            new IsometricHexagon(new IntRect((0, 0), (1, 2)), filled),
            new IsometricHexagon(new IntRect((0, 0), (2, 1)), filled),
            new IsometricHexagon(new IntRect((0, 0), (3, 0)), filled),
        });
        private IEnumerable<IsometricHexagon> randomTestCases
        {
            get
            {
                Random random = new Random(0);
                IntRect testRegion = new IntRect((-20, -20), (20, 20));
                for (int i = 0; i < 1_000; i++)
                {
                    IntRect rect = testRegion.RandomSubRect(random);
                    foreach (bool filled in new bool[] { false, true })
                    {
                        yield return new IsometricHexagon(rect, filled);
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public override void ShapeSinglePoint()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 point in new IntRect((-5, -5), (5, 5)))
                {
                    ShapeAssert.SameGeometry(new IntVector2[] { point }, new IsometricHexagon(new IntRect(point, point), filled), $"Failed with {point} {(filled ? "filled" : "unfilled")}.");
                }
            }
        }

        /// <summary>
        /// Tests that an example <see cref="IsometricHexagon"/> has the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapeExample1()
        {
            IsometricHexagon hexagon = new IsometricHexagon(new IntRect((0, 0), (7, 6)), false);
            IntVector2[] expected = new IntVector2[]
            {
                (3, 0), (4, 0), (5, 1), (6, 1), (7, 2), (7, 3), (7, 4), (6, 5), (5, 5), (4, 6), (3, 6), (2, 5), (1, 5), (0, 4), (0, 3), (0, 2), (1, 1), (2, 1)
            };

            ShapeAssert.SameGeometry(expected, hexagon);
        }

        /// <summary>
        /// Tests that an example <see cref="IsometricHexagon"/> has the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapeExample2()
        {
            IsometricHexagon hexagon = new IsometricHexagon(new IntRect((0, 0), (11, 10)), false);
            IntVector2[] expected = new IntVector2[]
            {
                (5, 0), (6, 0), (7, 1), (8, 1), (9, 2), (10, 2), (11, 3), (11, 4), (11, 5), (11, 6), (11, 7), (10, 8), (9, 8), (8, 9), (7, 9), (6, 10), (5, 10), (4, 9), (3, 9), (2, 8), (1, 8),
                (0, 7), (0, 6), (0, 5), (0, 4), (0, 3), (1, 2), (2, 2), (3, 1), (4, 1)
            };

            ShapeAssert.SameGeometry(expected, hexagon);
        }

        /// <summary>
        /// Tests that the diagonal edges of <see cref="IsometricHexagon"/> are perfect <see cref="Line"/>s of block size 2.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void BorderBlockSize()
        {
            foreach (IsometricHexagon hexagon in testCases)
            {
                if (hexagon.filled)
                {
                    continue;
                }

                IntRect boundingRect = IntRect.BoundingRect(hexagon);
                HashSet<IntVector2> points = Enumerable.ToHashSet(hexagon);
                foreach (IntVector2 point in points)
                {
                    int numHorizontalAdjacents = (points.Contains(point + IntVector2.left) ? 1 : 0) + (points.Contains(point + IntVector2.right) ? 1 : 0);
                    if (boundingRect.XCentreIs(point.x))
                    {
                        // Top/bottom corner - block size 1 or 2
                        Assert.LessOrEqual(numHorizontalAdjacents, 1, $"Failed with {hexagon} and {point + IntVector2.left}, {point}, {point + IntVector2.right}.");
                    }
                    else if (boundingRect.BorderContainsX(point.x))
                    {
                        if (point.y == points.Where(p => p.x == point.x).Max(p => p.y) || point.y == points.Where(p => p.x == point.x).Min(p => p.y))
                        {
                            // Corner that's not top/bottom - block size 1 or 2
                            Assert.LessOrEqual(numHorizontalAdjacents, 1, $"Failed with {hexagon} and {point + IntVector2.left}, {point}, {point + IntVector2.right}.");
                        }
                        else
                        {
                            // Vertical edge - block size 1
                            Assert.AreEqual(0, numHorizontalAdjacents, $"Failed with {hexagon} and {point + IntVector2.left}, {point}, {point + IntVector2.right}.");
                        }
                    }
                    else
                    {
                        // Diagonal edge - block size 2
                        Assert.AreEqual(1, numHorizontalAdjacents, $"Failed with {hexagon} and {point + IntVector2.left}, {point}, {point + IntVector2.right}.");
                    }
                }
            }
        }

        /// <summary>
        /// Tests that the diagonal edges of <see cref="IsometricHexagon"/> have no right angles - i.e. no
        /// <code>
        ///   #
        /// # #
        /// </code>
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void BorderHasNoRightAngles()
        {
            foreach (IsometricHexagon hexagon in testCases)
            {
                if (hexagon.filled)
                {
                    continue;
                }

                IntRect boundingRect = IntRect.BoundingRect(hexagon);
                HashSet<IntVector2> points = Enumerable.ToHashSet(hexagon);
                foreach (IntVector2 point in points)
                {
                    if (!boundingRect.BorderContainsX(point.x) &&
                        !(
                            points.Where(p => p.x == boundingRect.maxX).Count() == 2
                            && (point.x == boundingRect.maxX - 1 || point.x == boundingRect.minX + 1)
                        )
                    )
                    {
                        Assert.False(points.Contains(point + IntVector2.up), $"Failed with {hexagon} and {point}.");
                        Assert.False(points.Contains(point + IntVector2.down), $"Failed with {hexagon} and {point}.");
                    }
                }
            }
        }

        /// <summary>
        /// Tests that <see cref="IsometricHexagon.border"/> is indeed the border of the <see cref="IsometricHexagon"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void borderIsBorder()
        {
            foreach (IsometricHexagon hexagon in testCases)
            {
                ShapeAssert.SameGeometry(ShapeUtils.GetBorder(hexagon), hexagon.border, $"Failed with {hexagon}.");
            }
        }

        /// <summary>
        /// Tests that when you create an <see cref="IsometricHexagon"/> with a given rect, the bounding rect is a subset of that rect.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void boundingRectIsSubsetOfGivenRect()
        {
            Random random = new Random(0);
            IntRect testRegion = new IntRect((-20, -20), (20, 20));
            for (int i = 0; i < 1_000; i++)
            {
                IntRect initialRect = testRegion.RandomSubRect(random);
                IsometricHexagon hexagon = new IsometricHexagon(initialRect, false);
                Assert.True(hexagon.boundingRect.IsSubsetOf(initialRect), $"Failed with {initialRect}.");
                Assert.AreEqual(initialRect.height, hexagon.boundingRect.height, $"Failed with {initialRect}.");
                Assert.AreEqual(hexagon.boundingRect.maxX - initialRect.maxX, initialRect.minX - hexagon.boundingRect.minX, $"Failed with {initialRect}.");
            }
        }

        /// <summary>
        /// Tests that if you create an <see cref="IsometricHexagon"/> with a rect that is the bounding rect of another <see cref="IsometricHexagon"/>, the new <see cref="IsometricHexagon"/>
        /// is the same.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void boundingRectIsIdempotent()
        {
            Random random = new Random(0);
            IntRect testRegion = new IntRect((-20, -20), (20, 20));
            for (int i = 0; i < 1_000; i++)
            {
                IntRect initialRect = testRegion.RandomSubRect(random);
                IsometricHexagon firstHexagon = new IsometricHexagon(initialRect, false);
                IsometricHexagon secondHexagon = new IsometricHexagon(firstHexagon.boundingRect, false);
                Assert.AreEqual(firstHexagon, secondHexagon, $"Failed with {initialRect}.");
            }
        }

        /// <summary>
        /// Tests that the <see cref="IsometricHexagon"/> enumerator doesn't repeat any points.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void NoRepeats()
        {
            foreach (IsometricHexagon hexagon in testCases)
            {
                ShapeAssert.NoRepeats(hexagon);
            }
        }

        [Test]
        [Category("Shapes")]
        public override void Flipped() => IFlippableShape_DefaultTests<IsometricHexagon, CardinalAxis>.Flipped_Impl(testCases);
        [Test]
        [Category("Shapes")]
        public override void FlipMatchesFlipped() => IFlippableShape_DefaultTests<IsometricHexagon, CardinalAxis>.FlipMatchesFlipped_Impl(testCases);

        /// <summary>
        /// Tests that <see cref="IsometricHexagon"/>s has reflective symmetry across the vertical axis.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ReflectiveSymmetry_VerticalAxis()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 topRight in new IntRect((0, 0), (10, 10)))
                {
                    IsometricHexagon hexagon = new IsometricHexagon(new IntRect((0, 0), topRight), filled);
                    ShapeAssert.ReflectiveSymmetry(hexagon, Axes.Vertical);
                }
            }
        }

        /// <summary>
        /// Tests that <see cref="IsometricHexagon"/>s has reflective symmetry across the entral vertical axis of the given rect.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ReflectiveSymmetryInGivenRect_VerticalAxis()
        {
            Random random = new Random(0);
            IntRect testRegion = new IntRect((-20, -20), (20, 20));
            for (int i = 0; i < 1_000; i++)
            {
                IntRect initialRect = testRegion.RandomSubRect(random);
                IsometricHexagon hexagon = new IsometricHexagon(initialRect, random.NextBool());
                HashSet<IntVector2> points = Enumerable.ToHashSet(hexagon);
                foreach (IntVector2 point in points)
                {
                    Assert.True(points.Contains(new IntVector2(initialRect.maxX - (point.x - initialRect.minX), point.y)), $"Failed with {hexagon} and {point}.");
                }
            }
        }

        /// <summary>
        /// Tests that <see cref="IsometricHexagon"/>s has reflective symmetry across the horizontal axis.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ReflectiveSymmetry_HorizontalAxis()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 topRight in new IntRect((0, 0), (10, 10)))
                {
                    IsometricHexagon hexagon = new IsometricHexagon(new IntRect((0, 0), topRight), filled);
                    ShapeAssert.ReflectiveSymmetry(hexagon, Axes.Horizontal);
                }
            }
        }

        /// <summary>
        /// Tests that <see cref="IsometricHexagon"/>s has reflective symmetry across the central vertical axis of the given rect.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ReflectiveSymmetryInGivenRect_HorizontalAxis()
        {
            Random random = new Random(0);
            IntRect testRegion = new IntRect((-20, -20), (20, 20));
            for (int i = 0; i < 1_000; i++)
            {
                IntRect initialRect = testRegion.RandomSubRect(random);
                IsometricHexagon hexagon = new IsometricHexagon(initialRect, random.NextBool());
                HashSet<IntVector2> points = Enumerable.ToHashSet(hexagon);
                foreach (IntVector2 point in points)
                {
                    Assert.True(points.Contains(new IntVector2(point.x, initialRect.maxY - (point.y - initialRect.minY))), $"Failed with {hexagon} and {point}.");
                }
            }
        }
    }
}
