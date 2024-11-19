using NUnit.Framework;
using PAC.DataStructures;
using PAC.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace PAC.Tests
{
    public class EllipseTests : IShapeTests
    {
        /// <summary>
        /// Tests that ellipses that are single points have the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapeSinglePoint()
        {
            foreach (bool filled in new bool[] { false, true})
            {
                foreach (IntVector2 pixel in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
                {
                    Assert.True(new Shapes.Ellipse(pixel, pixel, filled).SequenceEqual(new IntVector2[] { pixel }), "Failed with " + pixel);
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

        [Test]
        [Category("Shapes")]
        public void BoundingRect()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 bottomLeft in new IntRect(new IntVector2(-5, 5), IntVector2.zero))
                {
                    foreach (IntVector2 topRight in new IntRect(IntVector2.zero, new IntVector2(5, 5)))
                    {
                        IShapeTestHelper.BoundingRect(new Shapes.Ellipse(bottomLeft, topRight, filled));
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void Count()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 topRight in new IntRect(IntVector2.zero, new IntVector2(10, 10)))
                {
                    IShapeTestHelper.Count(new Shapes.Ellipse(IntVector2.zero, topRight, filled));
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void Contains()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 topRight in new IntRect(IntVector2.zero, new IntVector2(10, 10)))
                {
                    IShapeTestHelper.Contains(new Shapes.Ellipse(IntVector2.zero, topRight, filled));
                }
            }
        }

        /// <summary>
        /// Tests that the ellipse enumerator doesn't repeat any pixels.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void NoRepeats()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 topRight in new IntRect(IntVector2.zero, new IntVector2(10, 10)))
                {
                    IShapeTestHelper.NoRepeatsAtAll(new Shapes.Ellipse(IntVector2.zero, topRight, filled));
                }
            }
        }

        /// <summary>
        /// Tests that the shape of an ellipse is only determined by the width and height, not by the position.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void TranslationalInvariance()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 bottomLeft in  new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
                {
                    foreach (IntVector2 topRight in bottomLeft + new IntRect(IntVector2.zero, new IntVector2(10, 10)))
                    {
                        Shapes.Ellipse ellipse = new Shapes.Ellipse(bottomLeft, topRight, filled);

                        Shapes.Ellipse expected = new Shapes.Ellipse(IntVector2.zero, topRight - bottomLeft, filled);
                        IEnumerable<IntVector2> translated = ellipse.Select(p => p - bottomLeft);

                        Assert.True(expected.SequenceEqual(translated), "Failed with " + ellipse);
                    }
                }
            }
        }

        /// <summary>
        /// Tests that rotating an ellipse 90 degrees gives the same shape as creating one with the width/height swapped.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void RotationalInvariance()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 topRight in new IntRect(IntVector2.zero, new IntVector2(10, 10)))
                {
                    Shapes.Ellipse ellipse = new Shapes.Ellipse(IntVector2.zero, topRight, filled);

                    Shapes.Ellipse expected = new Shapes.Ellipse(IntVector2.zero, new IntVector2(ellipse.height - 1, ellipse.width - 1), ellipse.filled);
                    IEnumerable<IntVector2> rotated = ellipse.Select(p => (p - ellipse.bottomLeft).Rotate(RotationAngle._90) + new IntVector2(0, ellipse.width - 1));

                    Assert.True(expected.ToHashSet().SetEquals(rotated), "Failed with " + ellipse);
                }
            }
        }

        /// <summary>
        /// Tests that reflecting an ellipse gives the same shape as creating one with the corners reflected.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ReflectiveInvariance()
        {
            foreach (FlipAxis axis in new FlipAxis[] { FlipAxis.Vertical, FlipAxis.Horizontal, FlipAxis._45Degrees, FlipAxis.Minus45Degrees })
            {
                foreach (bool filled in new bool[] { false, true })
                {
                    foreach (IntVector2 topRight in new IntRect(IntVector2.zero, new IntVector2(10, 10)))
                    {
                        Shapes.Ellipse ellipse = new Shapes.Ellipse(IntVector2.zero, topRight, filled);

                        Shapes.Ellipse expected = new Shapes.Ellipse(ellipse.bottomLeft.Flip(axis), ellipse.topRight.Flip(axis), filled);
                        IEnumerable<IntVector2> reflected = ellipse.Select(p => p.Flip(axis));

                        Assert.True(expected.ToHashSet().SetEquals(reflected), "Failed with " + ellipse + " and FlipAxis." + axis);
                    }
                }
            }
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

                    IShapeTestHelper.ReflectiveSymmetry(ellipse, FlipAxis.Vertical);
                    IShapeTestHelper.ReflectiveSymmetry(ellipse, FlipAxis.Horizontal);
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
                    IShapeTestHelper.RotationalSymmetry(circle, RotationAngle._90);
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

                    IShapeTestHelper.ReflectiveSymmetry(circle, FlipAxis.Vertical);
                    IShapeTestHelper.ReflectiveSymmetry(circle, FlipAxis.Horizontal);
                    IShapeTestHelper.ReflectiveSymmetry(circle, FlipAxis._45Degrees);
                    IShapeTestHelper.ReflectiveSymmetry(circle, FlipAxis.Minus45Degrees);
                }
            }
        }
    }
}
