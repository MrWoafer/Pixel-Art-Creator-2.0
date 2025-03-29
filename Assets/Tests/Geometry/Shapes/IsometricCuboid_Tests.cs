using NUnit.Framework;

using PAC.DataStructures;
using PAC.Extensions;
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
    /// Tests for <see cref="IsometricCuboid"/>.
    /// </summary>
    public class IsometricCuboid_Tests : IIsometricShape_DefaultTests<IsometricCuboid>, IIsometricShape_RequiredTests
    {
        protected override IEnumerable<IsometricCuboid> testCases => Enumerable.Concat(exampleTestCases, randomTestCases);
        private IEnumerable<IsometricCuboid> exampleTestCases
        {
            get
            {
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        for (int height = -5; height <= 5; height++)
                        {
                            foreach (bool filled in new bool[] { false, true })
                            {
                                foreach (bool includeBackEdges in new bool[] { false, true })
                                {
                                    yield return new IsometricCuboid(new IsometricRectangle((0, 0), (x, y), false), height, filled, includeBackEdges);
                                }
                            }
                        }
                    }
                }
            }
        }
        private IEnumerable<IsometricCuboid> randomTestCases
        {
            get
            {
                Random random = new Random(0);
                IntRect testRegion = new IntRect((-10, -10), (10, 10));
                for (int i = 0; i < 1_000; i++)
                {
                    IntVector2 start = testRegion.RandomPoint(random);
                    IntVector2 end = testRegion.RandomPoint(random);
                    int height = random.Next(-10, 11);
                    bool filled = random.NextBool();
                    bool includeBackEdges = random.NextBool();

                    yield return new IsometricCuboid(new IsometricRectangle(start, end, false), height, filled, includeBackEdges);
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public override void ShapeSinglePoint()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (bool includeBackEdges in new bool[] { false, true })
                {
                    foreach (IntVector2 point in new IntRect((-5, -5), (5, 5)))
                    {
                        ShapeAssert.SameGeometry(
                            new IntVector2[] { point },
                            new IsometricCuboid(new IsometricRectangle(point, point, false), 0, filled, includeBackEdges),
                            $"Failed with {point} {(filled ? "filled" : "unfilled")} {(includeBackEdges ? "show back edges" : "don't show back edges")}."
                            );
                    }
                }
            }
        }

        /// <summary>
        /// Tests that if an <see cref="IsometricCuboid"/> has <see cref="IsometricCuboid.height"/> == 0, then it looks the same as an <see cref="IsometricRectangle"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void HeightZeroIsIsometricRectangle()
        {
            foreach (IsometricCuboid cuboid in testCases)
            {
                if (cuboid.height == 0)
                {
                    IsometricRectangle bottomFace = cuboid.bottomFace;
                    bottomFace.filled = cuboid.filled;
                    ShapeAssert.SameGeometry(bottomFace, cuboid, $"Failed with {cuboid}.");
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public override void Contains() // overridden to not do as many test points due to how slow it is
        {
            Random random = new Random(0);
            foreach (IsometricCuboid cuboid in testCases)
            {
                IntRect boundingRect = cuboid.boundingRect;
                IntRect testRegion = new IntRect(boundingRect.bottomLeft + IntVector2.downLeft, boundingRect.topRight + IntVector2.upRight);

                HashSet<IntVector2> points = Enumerable.ToHashSet(cuboid);
                for (int i = 0; i < 100; i++)
                {
                    IntVector2 point = testRegion.RandomPoint(random);
                    Assert.True(points.Contains(point) == cuboid.Contains(point), $"Failed with {cuboid} and {point}. Expected {points.Contains(point)}.");
                }
            }
        }

        /// <summary>
        /// Tests <see cref="IsometricCuboid.height"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void height()
        {
            foreach (IsometricCuboid cuboid in testCases)
            {
                Assert.AreEqual(cuboid.topFace.boundingRect.minY - cuboid.bottomFace.boundingRect.minY, Math.Abs(cuboid.height), $"Failed with {cuboid}.");
            }
        }

        /// <summary>
        /// Tests that <see cref="IsometricCuboid.bottomFace"/> and <see cref="IsometricCuboid.topFace"/> are the same up to vertical translation.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void bottomFaceIsVerticalTranslationOfTopFace()
        {
            foreach (IsometricCuboid cuboid in testCases)
            {
                Assert.AreEqual(
                    cuboid.topFace,
                    cuboid.bottomFace + IntVector2.up * (IntRect.BoundingRect(cuboid.topFace).maxY - IntRect.BoundingRect(cuboid.bottomFace).maxY),
                    $"Failed with {cuboid}."
                    );
            }
        }

        /// <summary>
        /// Tests that <see cref="IsometricCuboid.bottomFace"/> is below (or on the same y level as) <see cref="IsometricCuboid.topFace"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void bottomFaceIsBelowTopFace()
        {
            foreach (IsometricCuboid cuboid in testCases)
            {
                Assert.True(cuboid.bottomFace.Any(p => cuboid.topFace.All(q => p.y <= q.y)), $"Failed with {cuboid}.");
            }
        }

        [Test]
        [Category("Shapes")]
        public override void UnfilledIsBorderOfFilled() // overridden because the default implementation won't work since the unfilled shape will have vertical lines inside the shape
        {
            foreach (IsometricCuboid cuboid in testCases)
            {
                cuboid.filled = true;
                HashSet<IntVector2> borderOfFilled = ShapeUtils.GetBorder(cuboid);

                cuboid.filled = false;
                // We check subset instead of set-equal since the unfilled shape will have vertical lines inside the shape
                Assert.True(borderOfFilled.IsSubsetOf(cuboid), $"Failed with {cuboid}.");
            }
        }

        /// <summary>
        /// Tests that the shape obtained with <see cref="IsometricCuboid.includeBackEdges"/> = <see langword="false"/> is a subset of the shape with
        /// <see cref="IsometricCuboid.includeBackEdges"/> = <see langword="true"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void includeBackEdgesContainsDontIncludeBackEdges()
        {
            foreach (IsometricCuboid cuboid in testCases)
            {
                cuboid.includeBackEdges = false;
                HashSet<IntVector2> dontincludeBackEdges = Enumerable.ToHashSet(cuboid);

                cuboid.includeBackEdges = true;
                Assert.True(dontincludeBackEdges.IsSubsetOf(cuboid), $"Failed with {cuboid}.");
            }
        }

        [Test]
        [Category("Shapes")]
        public override void Flip() // overridden to not test flipping across certain axes
        {
            foreach (IsometricCuboid cuboid in testCases)
            {
                IEnumerable<IntVector2> expected = cuboid.Select(p => p.Flip(Axes.Vertical));
                IsometricCuboid flipped = cuboid.Flip(Axes.Vertical);
                ShapeAssert.SameGeometry(expected, flipped, $"Failed with {cuboid}.");
            }
        }
    }
}
