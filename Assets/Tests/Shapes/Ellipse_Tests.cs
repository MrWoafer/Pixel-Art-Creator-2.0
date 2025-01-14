using NUnit.Framework;

using PAC.DataStructures;
using PAC.Shapes;
using PAC.Tests.Shapes.DefaultTests;
using PAC.Tests.Shapes.RequiredTests;
using PAC.Tests.Shapes.TestUtils;

using System.Collections.Generic;
using System.Linq;

namespace PAC.Tests.Shapes
{
    public class Ellipse_Tests : I2DShape_DefaultTests<Ellipse>, I2DShape_RequiredTests
    {
        protected override IEnumerable<Ellipse> testCases
        {
            get
            {
                foreach (bool filled in new bool[] { false, true })
                {
                    foreach (IntVector2 bottomLeft in new IntRect(new IntVector2(-2, -2), new IntVector2(2, 2)))
                    {
                        foreach (IntVector2 topRight in bottomLeft + new IntRect(IntVector2.zero, new IntVector2(5, 5)))
                        {
                            yield return new Ellipse(bottomLeft, topRight, filled);
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
                    CollectionAssert.AreEqual(new IntVector2[] { pixel }, new Ellipse(pixel, pixel, filled), "Failed with " + pixel + " " + (filled ? "filled" : "unfilled"));
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
                        Ellipse ellipse = new Ellipse(IntVector2.zero, new IntVector2(x, y), filled);
                        CollectionAssert.AreEquivalent(ellipse.boundingRect, ellipse, "Failed with " + ellipse);
                    }
                }
            }

            foreach (bool filled in new bool[] { false, true })
            {
                foreach (int y in new int[] { -1, 1 })
                {
                    for (int x = -5; x <= 5; x++)
                    {
                        Ellipse ellipse = new Ellipse(IntVector2.zero, new IntVector2(x, y), filled);
                        CollectionAssert.AreEquivalent(ellipse.boundingRect, ellipse, "Failed with " + ellipse);
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
                Ellipse ellipse = new Ellipse(centre - IntVector2.one, centre + IntVector2.one, false);
                CollectionAssert.AreEquivalent(new IntVector2[] { IntVector2.up, IntVector2.right, IntVector2.down, IntVector2.left }.Select(p => p + centre), ellipse);

                ellipse = new Ellipse(centre - IntVector2.one, centre + IntVector2.one, true);
                CollectionAssert.AreEquivalent(new IntVector2[] { IntVector2.zero, IntVector2.up, IntVector2.right, IntVector2.down, IntVector2.left }.Select(p => p + centre), ellipse);
            }
        }

        /// <summary>
        /// Tests that an example ellipse has the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapeExample1()
        {
            Ellipse ellipse = new Ellipse(IntVector2.zero, new IntVector2(6, 10), false);
            IntVector2[] expected =
            {
                new IntVector2(3, 10), new IntVector2(4, 10), new IntVector2(5, 9), new IntVector2(5, 8), new IntVector2(6, 7), new IntVector2(6, 6), new IntVector2(6, 5), new IntVector2(6, 4),
                new IntVector2(6, 3), new IntVector2(5, 2), new IntVector2(5, 1), new IntVector2(4, 0), new IntVector2(3, 0), new IntVector2(2, 0), new IntVector2(1, 1), new IntVector2(1, 2),
                new IntVector2(0, 3), new IntVector2(0, 4), new IntVector2(0, 5), new IntVector2(0, 6), new IntVector2(0, 7), new IntVector2(1, 8), new IntVector2(1, 9), new IntVector2(2, 10)
            };

            CollectionAssert.AreEqual(expected, ellipse);
        }

        /// <summary>
        /// Tests that an example ellipse has the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapeExample2()
        {
            Ellipse ellipse = new Ellipse(IntVector2.zero, new IntVector2(5, 10), false);
            IntVector2[] expected =
            {
                new IntVector2(3, 10), new IntVector2(4, 9), new IntVector2(5, 8), new IntVector2(5, 7), new IntVector2(5, 6), new IntVector2(5, 5), new IntVector2(5, 4), new IntVector2(5, 3),
                new IntVector2(5, 2), new IntVector2(4, 1), new IntVector2(3, 0), new IntVector2(2, 0), new IntVector2(1, 1), new IntVector2(0, 2), new IntVector2(0, 3), new IntVector2(0, 4),
                new IntVector2(0, 5), new IntVector2(0, 6), new IntVector2(0, 7), new IntVector2(0, 8), new IntVector2(1, 9), new IntVector2(2, 10)
            };

            CollectionAssert.AreEqual(expected, ellipse);
        }

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
                    Ellipse ellipse = new Ellipse(IntVector2.zero, topRight, filled);

                    ShapeAssert.ReflectiveSymmetry(ellipse, FlipAxis.Vertical);
                    ShapeAssert.ReflectiveSymmetry(ellipse, FlipAxis.Horizontal);
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
                    Ellipse circle = new Ellipse(IntVector2.zero, new IntVector2(diameter - 1, diameter - 1), filled);
                    ShapeAssert.RotationalSymmetry(circle, RotationAngle._90);
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
                    Ellipse circle = new Ellipse(IntVector2.zero, new IntVector2(diameter - 1, diameter - 1), filled);

                    ShapeAssert.ReflectiveSymmetry(circle, FlipAxis.Vertical);
                    ShapeAssert.ReflectiveSymmetry(circle, FlipAxis.Horizontal);
                    ShapeAssert.ReflectiveSymmetry(circle, FlipAxis._45Degrees);
                    ShapeAssert.ReflectiveSymmetry(circle, FlipAxis.Minus45Degrees);
                }
            }
        }
    }
}
