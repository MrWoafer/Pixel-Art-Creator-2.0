using NUnit.Framework;
using PAC.DataStructures;
using PAC.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace PAC.Tests
{
    public class DiamondTests : IShapeTests<Shapes.Diamond>
    {
        protected override IEnumerable<Shapes.Diamond> testCases
        {
            get
            {
                foreach (bool filled in new bool[] { false, true })
                {
                    foreach (IntVector2 bottomLeft in new IntRect(new IntVector2(-2, -2), new IntVector2(2, 2)))
                    {
                        foreach (IntVector2 topRight in bottomLeft + new IntRect(IntVector2.zero, new IntVector2(5, 5)))
                        {
                            yield return new Shapes.Diamond(bottomLeft, topRight, filled);
                        }
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
                foreach (IntVector2 pixel in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
                {
                    Assert.True(new Shapes.Diamond(pixel, pixel, filled).SequenceEqual(new IntVector2[] { pixel }), "Failed with " + pixel + " " + (filled ? "filled" : "unfilled"));
                }
            }
        }

        /// <summary>
        /// Tests that diamonds has reflective symmetry across the vertical axis and across the horizontal axis. Note that together these also imply 180-degree rotational symmetry.
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
    }
}
