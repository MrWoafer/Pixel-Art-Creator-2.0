using NUnit.Framework;

using PAC.DataStructures;
using PAC.Extensions;
using PAC.Shapes;
using PAC.Tests.Shapes.DefaultTests;
using PAC.Tests.Shapes.RequiredTests;
using PAC.Tests.Shapes.TestUtils;

using System;
using System.Collections.Generic;
using System.Linq;

namespace PAC.Tests.Shapes
{
    /// <summary>
    /// Tests for <see cref="IsometricCuboid"/>.
    /// </summary>
    public class IsometricCuboid_Tests : IIsometricShape_DefaultTests<IsometricCuboid>, IIsometricShape_RequiredTests
    {
        protected override IEnumerable<IsometricCuboid> testCases => exampleTestCases.Concat(randomTestCases);
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
                                foreach (bool showBackEdges in new bool[] { false, true })
                                {
                                    yield return new IsometricCuboid(IntVector2.zero, new IntVector2(x, y), height, filled, showBackEdges);
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
                Random rng = new Random(0);
                const int numTestCases = 1_000;
                for (int i = 0; i < numTestCases; i++)
                {
                    IntVector2 start = new IntVector2(rng.Next(-10, 11), rng.Next(-10, 11));
                    IntVector2 end = new IntVector2(rng.Next(-10, 11), rng.Next(-10, 11));
                    int height = rng.Next(-10, 11);
                    bool filled = rng.Next(0, 2) == 0;
                    bool showBackEdges = rng.Next(0, 2) == 0;

                    yield return new IsometricCuboid(start, end, height, filled, showBackEdges);
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public override void ShapeSinglePoint()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (bool showBackEdges in new bool[] { false, true })
                {
                    foreach (IntVector2 pixel in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
                    {
                        CollectionAssert.AreEquivalent(new IntVector2[] { pixel }, new IsometricCuboid(pixel, pixel, 0, filled, showBackEdges).ToHashSet(),
                            $"Failed with {pixel} {(filled ? "filled" : "unfilled")} {(showBackEdges ? "show back edges" : "don't show back edges")}.");
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public override void Contains()
        {
            foreach (IsometricCuboid cuboid in testCases)
            {
                IntRect boundingRect = cuboid.boundingRect;
                IntRect testRegion = new IntRect(boundingRect.bottomLeft + IntVector2.downLeft, boundingRect.topRight + IntVector2.upRight);

                HashSet<IntVector2> pixels = cuboid.ToHashSet();

                const int numTestPoints = 100;
                for (int i = 0; i < numTestPoints; i++)
                {
                    IntVector2 point = testRegion.RandomPoint();
                    Assert.True(pixels.Contains(point) == cuboid.Contains(point), $"Failed with {cuboid} and {point}. Expected {pixels.Contains(point)}.");
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void Height()
        {
            foreach (IsometricCuboid shape in testCases)
            {
                Assert.AreEqual(Math.Abs(shape.height), shape.topRectangle.boundingRect.minY - shape.bottomRectangle.boundingRect.minY, $"Failed with {shape}.");
            }
        }

        [Test]
        [Category("Shapes")]
        public override void UnfilledIsBorderOfFilled()
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
        /// Tests that the shape obtained with <see cref="IsometricCuboid.showBackEdges"/> = <see langword="false"/> is a subset of the shape with
        /// <see cref="IsometricCuboid.showBackEdges"/> = <see langword="true"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShowBackEdgesContainsDontShowBackEdges()
        {
            foreach (IsometricCuboid cuboid in testCases)
            {
                cuboid.showBackEdges = false;
                HashSet<IntVector2> dontShowBackEdges = cuboid.ToHashSet();

                cuboid.showBackEdges = true;
                Assert.True(dontShowBackEdges.IsSubsetOf(cuboid), $"Failed with {cuboid}.");
            }
        }

        [Test]
        [Category("Shapes")]
        public override void Flip()
        {
            foreach (IsometricCuboid shape in testCases)
            {
                foreach (FlipAxis axis in new FlipAxis[] { FlipAxis.None, FlipAxis.Vertical })
                {
                    HashSet<IntVector2> expected = shape.Select(p => p.Flip(axis)).ToHashSet();
                    HashSet<IntVector2> flipped = shape.Flip(axis).ToHashSet();
                    Assert.True(expected.SetEquals(flipped), $"Failed with {shape} and {axis}.");
                }
            }
        }
    }
}
