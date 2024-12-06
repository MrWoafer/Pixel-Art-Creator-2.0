using NUnit.Framework;
using PAC.DataStructures;
using PAC.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace PAC.Tests
{
    public class RectangleTests : I2DShapeTests<Shapes.Rectangle>
    {
        protected override IEnumerable<Shapes.Rectangle> testCases
        {
            get
            {
                foreach (bool filled in new bool[] { false, true })
                {
                    foreach (IntVector2 bottomLeft in new IntRect(new IntVector2(-2, -2), new IntVector2(2, 2)))
                    {
                        foreach (IntVector2 topRight in bottomLeft + new IntRect(IntVector2.zero, new IntVector2(5, 5)))
                        {
                            yield return new Shapes.Rectangle(bottomLeft, topRight, filled);
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
                    Assert.True(new Shapes.Rectangle(pixel, pixel, filled).SequenceEqual(new IntVector2[] { pixel }), "Failed with " + pixel + " " + (filled ? "filled" : "unfilled"));
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void Shape()
        {
            foreach (IntVector2 bottomLeft in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
            {
                foreach (IntVector2 topRight in bottomLeft + new IntRect(IntVector2.zero, new IntVector2(5, 5)))
                {
                    Shapes.Rectangle rectangle = new Shapes.Rectangle(bottomLeft, topRight, true);
                    Assert.True(rectangle.boundingRect.ToHashSet().SetEquals(rectangle), "Failed with " + rectangle);
                }
            }
        }
    }
}
