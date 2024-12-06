using NUnit.Framework;
using PAC.DataStructures;
using PAC.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace PAC.Tests
{
    public class EllipseTests : I2DShapeTests<Shapes.Ellipse>
    {
        protected override IEnumerable<Shapes.Ellipse> testCases
        {
            get
            {
                foreach (bool filled in new bool[] { false, true })
                {
                    foreach (IntVector2 bottomLeft in new IntRect(new IntVector2(-2, -2), new IntVector2(2, 2)))
                    {
                        foreach (IntVector2 topRight in bottomLeft + new IntRect(IntVector2.zero, new IntVector2(5, 5)))
                        {
                            yield return new Shapes.Ellipse(bottomLeft, topRight, filled);
                        }
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public override void ShapeSinglePoint()
        {
            foreach (bool filled in new bool[] { false, true})
            {
                foreach (IntVector2 pixel in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
                {
                    Assert.True(new Shapes.Ellipse(pixel, pixel, filled).SequenceEqual(new IntVector2[] { pixel }), "Failed with " + pixel + " " + (filled ? "filled" : "unfilled"));
                }
            }
        }

        /// <summary>
        /// Tests that 2xn and nx2 ellipses have the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void Shape2xNAndNx2()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (int x in new int[] { -1, 1 })
                {
                    for (int y = -5; y <= 5; y++)
                    {
                        Shapes.Ellipse ellipse = new Shapes.Ellipse(IntVector2.zero, new IntVector2(x, y), filled);
                        Assert.True(ellipse.ToHashSet().SetEquals(ellipse.boundingRect), "Failed with " + ellipse);
                    }
                }
            }

            foreach (bool filled in new bool[] { false, true })
            {
                foreach (int y in new int[] { -1, 1 })
                {
                    for (int x = -5; x <= 5; x++)
                    {
                        Shapes.Ellipse ellipse = new Shapes.Ellipse(IntVector2.zero, new IntVector2(x, y), filled);
                        Assert.True(ellipse.ToHashSet().SetEquals(ellipse.boundingRect), "Failed with " + ellipse);
                    }
                }
            }
        }

        /// <summary>
        /// Tests that 3x3 ellipses have the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void Shape3x3()
        {
            foreach (IntVector2 centre in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
            {
                Shapes.Ellipse ellipse = new Shapes.Ellipse(centre - IntVector2.one, centre + IntVector2.one, false);
                Assert.True(ellipse.ToHashSet().SetEquals(centre + new IntVector2[] { IntVector2.up, IntVector2.right, IntVector2.down, IntVector2.left }));

                ellipse = new Shapes.Ellipse(centre - IntVector2.one, centre + IntVector2.one, true);
                Assert.True(ellipse.ToHashSet().SetEquals(centre + new IntVector2[] { IntVector2.zero, IntVector2.up, IntVector2.right, IntVector2.down, IntVector2.left }));
            }
        }

        /// <summary>
        /// Tests that an example ellipse has the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapeExample1()
        {
            Shapes.Ellipse ellipse = new Shapes.Ellipse(IntVector2.zero, new IntVector2(6, 10), false);
            IntVector2[] expected =
            {
                new IntVector2(3, 10), new IntVector2(4, 10), new IntVector2(5, 9), new IntVector2(5, 8), new IntVector2(6, 7), new IntVector2(6, 6), new IntVector2(6, 5), new IntVector2(6, 4),
                new IntVector2(6, 3), new IntVector2(5, 2), new IntVector2(5, 1), new IntVector2(4, 0), new IntVector2(3, 0), new IntVector2(2, 0), new IntVector2(1, 1), new IntVector2(1, 2),
                new IntVector2(0, 3), new IntVector2(0, 4), new IntVector2(0, 5), new IntVector2(0, 6), new IntVector2(0, 7), new IntVector2(1, 8), new IntVector2(1, 9), new IntVector2(2, 10)
            };

            Assert.True(expected.SequenceEqual(ellipse));
        }

        /// <summary>
        /// Tests that an example ellipse has the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapeExample2()
        {
            Shapes.Ellipse ellipse = new Shapes.Ellipse(IntVector2.zero, new IntVector2(5, 10), false);
            IntVector2[] expected =
            {
                new IntVector2(3, 10), new IntVector2(4, 9), new IntVector2(5, 8), new IntVector2(5, 7), new IntVector2(5, 6), new IntVector2(5, 5), new IntVector2(5, 4), new IntVector2(5, 3),
                new IntVector2(5, 2), new IntVector2(4, 1), new IntVector2(3, 0), new IntVector2(2, 0), new IntVector2(1, 1), new IntVector2(0, 2), new IntVector2(0, 3), new IntVector2(0, 4),
                new IntVector2(0, 5), new IntVector2(0, 6), new IntVector2(0, 7), new IntVector2(0, 8), new IntVector2(1, 9), new IntVector2(2, 10)
            };

            Assert.True(expected.SequenceEqual(ellipse));
        }

        /// <summary>
        /// Tests that ellipses has reflective symmetry across the vertical axis and across the horizontal axis. Note that together these also imply 180-degree rotational symmetry.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ReflectiveSymmetry()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 topRight in new IntRect(IntVector2.zero, new IntVector2(10, 10)))
                {
                    Shapes.Ellipse ellipse = new Shapes.Ellipse(IntVector2.zero, topRight, filled);

                    I2DShapeTestHelper.ReflectiveSymmetry(ellipse, FlipAxis.Vertical);
                    I2DShapeTestHelper.ReflectiveSymmetry(ellipse, FlipAxis.Horizontal);
                }
            }
        }

        /// <summary>
        /// Tests that circles have 90-degree rotational symmetry (and hence 180-degree, 270-degree, etc).
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void CircleRotationalSymmetry()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                for (int diameter = 1; diameter <= 10; diameter++)
                {
                    Shapes.Ellipse circle = new Shapes.Ellipse(IntVector2.zero, new IntVector2(diameter - 1, diameter - 1), filled);
                    I2DShapeTestHelper.RotationalSymmetry(circle, RotationAngle._90);
                }
            }
        }

        /// <summary>
        /// Tests that circles have reflective symmetry across a +/- 45-degree line through the centre.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void CircleReflectiveSymmetry()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                for (int diameter = 1; diameter <= 10; diameter++)
                {
                    Shapes.Ellipse circle = new Shapes.Ellipse(IntVector2.zero, new IntVector2(diameter - 1, diameter - 1), filled);

                    I2DShapeTestHelper.ReflectiveSymmetry(circle, FlipAxis.Vertical);
                    I2DShapeTestHelper.ReflectiveSymmetry(circle, FlipAxis.Horizontal);
                    I2DShapeTestHelper.ReflectiveSymmetry(circle, FlipAxis._45Degrees);
                    I2DShapeTestHelper.ReflectiveSymmetry(circle, FlipAxis.Minus45Degrees);
                }
            }
        }
    }
}
