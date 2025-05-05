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
    /// Tests for <see cref="Diamond"/>.
    /// </summary>
    public class Diamond_Tests : I2DShape_DefaultTests<Diamond>, I2DShape_RequiredTests
    {
        protected override IEnumerable<Diamond> testCases => Enumerable.Concat(exampleTestCases, randomTestCases);
        private IEnumerable<Diamond> exampleTestCases
        {
            get
            {
                foreach (bool filled in new bool[] { false, true })
                {
                    for (int x = 0; x <= 5; x++)
                    {
                        for (int y = 0; y <= 5; y++)
                        {
                            yield return new Diamond(new IntRect((0, 0), (x, y)), filled);
                        }
                    }
                }
            }
        }
        private IEnumerable<Diamond> randomTestCases
        {
            get
            {
                Random random = new Random(0);
                IntRect testRegion = new IntRect((-20, -20), (20, 20));
                for (int i = 0; i < 1_000; i++)
                {
                    yield return new Diamond(testRegion.RandomSubRect(random), random.NextBool());
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
                    ShapeAssert.SameGeometry(new IntVector2[] { point }, new Diamond(new IntRect(point, point), filled), $"Failed with {point} {(filled ? "filled" : "unfilled")}.");
                }
            }
        }

        /// <summary>
        /// Tests that 1xN <see cref="Diamond"/>s have the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void Shape1xN()
        {
            for (int height = 1; height <= 20; height++)
            {
                foreach (bool filled in new bool[] { false, true })
                {
                    Diamond diamond = new Diamond(new IntRect((0, 0), (0, height - 1)), filled);
                    IntRect expected = new IntRect((0, 0), (0, height - 1));
                    ShapeAssert.SameGeometry(expected, diamond, $"Failed with {diamond}.");
                }
            }
        }

        /// <summary>
        /// Tests that 2xN <see cref="Diamond"/>s have the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void Shape2xN()
        {
            for (int height = 1; height <= 20; height++)
            {
                foreach (bool filled in new bool[] { false, true })
                {
                    Diamond diamond = new Diamond(new IntRect((0, 0), (1, height - 1)), filled);
                    IntRect expected = new IntRect((0, 0), (1, height - 1));
                    ShapeAssert.SameGeometry(expected, diamond, $"Failed with {diamond}.");
                }
            }
        }

        /// <summary>
        /// Tests that <see cref="Diamond"/>s that can be drawn with perfect <see cref="Line"/>s are drawn as such. E.g. a 6x3 <see cref="Diamond"/> can be drawn with only blocks of size 2x1.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapePerfect()
        {
            foreach (IntVector2 start in new IntRect((-2, -2), (2, 2)))
            {
                for (int numBlocksPerEdge = 1; numBlocksPerEdge <= 10; numBlocksPerEdge++)
                {
                    for (int blockSize = 1; blockSize <= 10; blockSize++)
                    {
                        int width = blockSize * (2 * numBlocksPerEdge - 1);
                        int height = 2 * numBlocksPerEdge - 1;

                        Diamond diamond = new Diamond(new IntRect(start, start + (width - 1, height - 1)), false);

                        List<IntVector2> expected = new List<IntVector2>();
                        int numInBlock = 0;
                        int yOffset = 0;
                        int middleY = start.y + height / 2;
                        for (int xOffset = 0; start.x + xOffset <= start.x + width - 1 - xOffset; xOffset++)
                        {
                            numInBlock++;

                            expected.Add((start.x + xOffset, middleY + yOffset));
                            expected.Add((start.x + xOffset, middleY - yOffset));
                            expected.Add((start.x + width - 1 - xOffset, middleY + yOffset));
                            expected.Add((start.x + width - 1 - xOffset, middleY - yOffset));

                            if (numInBlock == blockSize)
                            {
                                numInBlock = 0;
                                yOffset++;
                            }
                        }

                        ShapeAssert.SameGeometry(expected, diamond, $"Failed with {diamond}.");
                    }
                }
            }
        }

        /// <summary>
        /// Tests that the <see cref="Diamond"/> enumerator doesn't repeat any points.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void NoRepeats()
        {
            foreach (Diamond diamond in testCases)
            {
                ShapeAssert.NoRepeats(diamond);
            }
        }

        /// <summary>
        /// Tests that <see cref="Diamond"/>s have reflective symmetry across the central vertical axis.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ReflectiveSymmetry_VerticalAxis()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 topRight in new IntRect((0, 0), (10, 10)))
                {
                    Diamond diamond = new Diamond(new IntRect((0, 0), topRight), filled);
                    ShapeAssert.ReflectiveSymmetry(diamond, Axes.Vertical);
                }
            }
        }
        /// <summary>
        /// Tests that <see cref="Diamond"/>s have reflective symmetry across the central horizontal axis.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ReflectiveSymmetry_HorizontalAxis()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 topRight in new IntRect((0, 0), (10, 10)))
                {
                    Diamond diamond = new Diamond(new IntRect((0, 0), topRight), filled);
                    ShapeAssert.ReflectiveSymmetry(diamond, Axes.Horizontal);
                }
            }
        }
    }
}
