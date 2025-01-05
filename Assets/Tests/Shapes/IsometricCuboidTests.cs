using NUnit.Framework;
using PAC.DataStructures;
using PAC.Drawing;
using PAC.Extensions;

using System;
using System.Collections.Generic;
using System.Linq;

namespace PAC.Tests
{
    /// <summary>
    /// Tests for <see cref="Shapes.IsometricCuboid"/>.
    /// </summary>
    public class IsometricCuboidTests : IIsometricShapeTests<Shapes.IsometricCuboid>
    {
        public override IEnumerable<Shapes.IsometricCuboid> testCases => exampleTestCases.Concat(randomTestCases);
        public IEnumerable<Shapes.IsometricCuboid> exampleTestCases
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
                                    yield return new Shapes.IsometricCuboid(IntVector2.zero, new IntVector2(x, y), height, filled, showBackEdges);
                                }
                            }
                        }
                    }
                }
            }
        }
        public IEnumerable<Shapes.IsometricCuboid> randomTestCases
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

                    yield return new Shapes.IsometricCuboid(start, end, height, filled, showBackEdges);
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
                        CollectionAssert.AreEquivalent(new IntVector2[] { pixel }, new Shapes.IsometricCuboid(pixel, pixel, 0, filled, showBackEdges).ToHashSet(),
                            $"Failed with {pixel} {(filled ? "filled" : "unfilled")} {(showBackEdges ? "show back edges" : "don't show back edges")}");
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public override void Contains()
        {
            foreach (Shapes.IsometricCuboid cuboid in testCases)
            {
                IntRect boundingRect = cuboid.boundingRect;
                IntRect testRegion = new IntRect(boundingRect.bottomLeft + IntVector2.downLeft, boundingRect.topRight + IntVector2.upRight);

                HashSet<IntVector2> pixels = cuboid.ToHashSet();

                const int numTestPoints = 100;
                for (int i = 0; i < numTestPoints; i++)
                {
                    IntVector2 point = testRegion.RandomPoint();
                    Assert.True(pixels.Contains(point) == cuboid.Contains(point), $"Failed with {cuboid} and {point}. Expected {pixels.Contains(point)}");
                }
            }
        }

        /// <summary>
        /// Don't currently care about repeated pixels in <see cref="Shapes.IsometricCuboid"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public override void NoRepeats() => Assert.Pass();

        [Test]
        [Category("Shapes")]
        public void Height()
        {
            foreach (Shapes.IsometricCuboid shape in testCases)
            {
                Assert.AreEqual(Math.Abs(shape.height), shape.topRectangle.boundingRect.minY - shape.bottomRectangle.boundingRect.minY, $"Failed with {shape}");
            }
        }

        [Test]
        [Category("Shapes")]
        public override void UnfilledIsBorderOfFilled()
        {
            foreach (Shapes.IsometricCuboid cuboid in testCases)
            {
                cuboid.filled = true;
                HashSet<IntVector2> borderOfFilled = IFillableShapeTestHelper.GetBorder(cuboid);

                cuboid.filled = false;
                // We check subset instead of set-equal since the unfilled shape will have vertical lines inside the shape
                Assert.True(borderOfFilled.IsSubsetOf(cuboid), $"Failed with {cuboid}");
            }
        }

        /// <summary>
        /// Tests that the shape obtained with <see cref="Shapes.IsometricCuboid.showBackEdges"/> = <see langword="false"/> is a subset of the shape with
        /// <see cref="Shapes.IsometricCuboid.showBackEdges"/> = <see langword="true"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShowBackEdgesContainsDontShowBackEdges()
        {
            foreach (Shapes.IsometricCuboid cuboid in testCases)
            {
                cuboid.showBackEdges = false;
                HashSet<IntVector2> dontShowBackEdges = cuboid.ToHashSet();

                cuboid.showBackEdges = true;
                Assert.True(dontShowBackEdges.IsSubsetOf(cuboid), $"Failed with {cuboid}");
            }
        }

        [Test]
        [Category("Shapes")]
        public override void Flip()
        {
            foreach (Shapes.IsometricCuboid shape in testCases)
            {
                foreach (FlipAxis axis in new FlipAxis[] { FlipAxis.None, FlipAxis.Vertical })
                {
                    IShapeTestHelper.Flip(shape, axis);
                }
            }
        }
    }
}
