using NUnit.Framework;

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
    /// Tests for <see cref="Ellipse"/>.
    /// </summary>
    public class Ellipse_Tests : I2DShape_DefaultTests<Ellipse>, I2DShape_RequiredTests
    {
        protected override IEnumerable<Ellipse> testCases => Enumerable.Concat(exampleTestCases, randomTestCases);
        private IEnumerable<Ellipse> exampleTestCases
        {
            get
            {
                foreach (bool filled in new bool[] { false, true })
                {
                    for (int x = 0; x <= 5; x++)
                    {
                        for (int y = 0; y <= 5; y++)
                        {
                            yield return new Ellipse(new IntRect((0, 0), (x, y)), filled);
                        }
                    }
                }
            }
        }
        private IEnumerable<Ellipse> randomTestCases
        {
            get
            {
                Random random = new Random(0);
                IntRect testRegion = new IntRect((-20, -20), (20, 20));
                for (int i = 0; i < 1_000; i++)
                {
                    yield return new Ellipse(testRegion.RandomSubRect(random), random.NextBool());
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public override void ShapeSinglePoint()
        {
            foreach (bool filled in new bool[] { false, true})
            {
                foreach (IntVector2 pixel in new IntRect((-5, -5), (5, 5)))
                {
                    ShapeAssert.SameGeometry(new IntVector2[] { pixel }, new Ellipse(new IntRect(pixel, pixel), filled), $"Failed with {pixel} {(filled ? "filled" : "unfilled")}.");
                }
            }
        }

        /// <summary>
        /// Tests that 1xN <see cref="Ellipse"/>s have the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void Shape1xN()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                for (int y = -10; y <= 10; y++)
                {
                    Ellipse ellipse = new Ellipse(new IntRect((0, 0), (0, y)), filled);
                    IntRect expected = new IntRect((0, 0), (0, y));
                    ShapeAssert.SameGeometry(expected, ellipse, $"Failed with {ellipse}.");
                }
            }
        }
        /// <summary>
        /// Tests that Nx1 <see cref="Ellipse"/>s have the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapeNx1()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                for (int x = -10; x <= 10; x++)
                {
                    Ellipse ellipse = new Ellipse(new IntRect((0, 0), (x, 0)), filled);
                    IntRect expected = new IntRect((0, 0), (x, 0));
                    ShapeAssert.SameGeometry(expected, ellipse, $"Failed with {ellipse}.");
                }
            }
        }

        /// <summary>
        /// Tests that 2xN <see cref="Ellipse"/>s have the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void Shape2xN()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (int x in new int[] { -1, 1 })
                {
                    for (int y = -10; y <= 10; y++)
                    {
                        Ellipse ellipse = new Ellipse(new IntRect((0, 0), (x, y)), filled);
                        IntRect expected = new IntRect((0, 0), (x, y));
                        ShapeAssert.SameGeometry(expected, ellipse, $"Failed with {ellipse}.");
                    }
                }
            }
        }
        /// <summary>
        /// Tests that Nx2 <see cref="Ellipse"/>s have the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapeNx2()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (int y in new int[] { -1, 1 })
                {
                    for (int x = -10; x <= 10; x++)
                    {
                        Ellipse ellipse = new Ellipse(new IntRect((0, 0), (x, y)), filled);
                        IntRect expected = new IntRect((0, 0), (x, y));
                        ShapeAssert.SameGeometry(expected, ellipse, $"Failed with {ellipse}.");
                    }
                }
            }
        }

        /// <summary>
        /// Tests that 3x3 <see cref="Ellipse"/>s have the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void Shape3x3()
        {
            foreach (IntVector2 centre in new IntRect((-5, -5), (5, 5)))
            {
                Ellipse ellipse = new Ellipse(new IntRect(centre - (1, 1), centre + (1, 1)), false);
                IEnumerable<IntVector2> expected = new IntVector2[]
                {
                    centre + IntVector2.left, centre + IntVector2.right, centre + IntVector2.up, centre + IntVector2.down
                };
                ShapeAssert.SameGeometry(expected, ellipse);

                ellipse.filled = true;
                expected = expected.Append(centre);
                ShapeAssert.SameGeometry(expected, ellipse);
            }
        }

        /// <summary>
        /// Tests that an example <see cref="Ellipse"/> has the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapeExample1()
        {
            Ellipse ellipse = new Ellipse(new IntRect((0, 0), (6, 10)), false);
            IntVector2[] expected =
            {
                (3, 10), (4, 10), (5, 9), (5, 8), (6, 7), (6, 6), (6, 5), (6, 4), (6, 3), (5, 2), (5, 1), (4, 0), (3, 0), (2, 0), (1, 1), (1, 2), (0, 3), (0, 4), (0, 5), (0, 6), (0, 7), (1, 8),
                (1, 9), (2, 10)
            };

            ShapeAssert.SameGeometry(expected, ellipse);
        }

        /// <summary>
        /// Tests that an example <see cref="Ellipse"/> has the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapeExample2()
        {
            Ellipse ellipse = new Ellipse(new IntRect((0, 0), (5, 10)), false);
            IntVector2[] expected =
            {
                (3, 10), (4, 9), (5, 8), (5, 7), (5, 6), (5, 5), (5, 4), (5, 3), (5, 2), (4, 1), (3, 0), (2, 0), (1, 1), (0, 2), (0, 3), (0, 4), (0, 5), (0, 6), (0, 7), (0, 8), (1, 9), (2, 10)
            };

            ShapeAssert.SameGeometry(expected, ellipse);
        }

        /// <summary>
        /// Tests that the <see cref="Ellipse"/> enumerator doesn't repeat any points.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void NoRepeats()
        {
            foreach (Ellipse ellipse in testCases)
            {
                ShapeAssert.NoRepeats(ellipse);
            }
        }

        /// <summary>
        /// Tests that <see cref="Ellipse"/>s has reflective symmetry across the vertical axis.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ReflectiveSymmetry_VerticalAxis()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 topRight in new IntRect((0, 0), (10, 10)))
                {
                    Ellipse ellipse = new Ellipse(new IntRect((0, 0), topRight), filled);
                    ShapeAssert.ReflectiveSymmetry(ellipse, Axes.Vertical);
                }
            }
        }

        /// <summary>
        /// Tests that <see cref="Ellipse"/>s has reflective symmetry across the horizontal axis.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ReflectiveSymmetry_HorizontalAxis()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 topRight in new IntRect((0, 0), (10, 10)))
                {
                    Ellipse ellipse = new Ellipse(new IntRect((0, 0), topRight), filled);
                    ShapeAssert.ReflectiveSymmetry(ellipse, Axes.Horizontal);
                }
            }
        }

        /// <summary>
        /// Tests that <see cref="Ellipse"/>s that are circles have 90-degree rotational symmetry (and hence 180-degree, 270-degree, etc).
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void CircleRotationalSymmetry()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                for (int diameter = 1; diameter <= 15; diameter++)
                {
                    Ellipse circle = new Ellipse(new IntRect((0, 0), (diameter - 1, diameter - 1)), filled);
                    ShapeAssert.RotationalSymmetry(circle, QuadrantalAngle.Clockwise90);
                }
            }
        }

        /// <summary>
        /// Tests that <see cref="Ellipse"/>s that are circles have reflective symmetry across a +/- 45-degree line through the centre.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void CircleReflectiveSymmetry_45Degrees()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                for (int diameter = 1; diameter <= 15; diameter++)
                {
                    Ellipse circle = new Ellipse(new IntRect((0, 0), (diameter - 1, diameter - 1)), filled);

                    ShapeAssert.ReflectiveSymmetry(circle, Axes.Diagonal45);
                    ShapeAssert.ReflectiveSymmetry(circle, Axes.Minus45);
                }
            }
        }
    }
}
