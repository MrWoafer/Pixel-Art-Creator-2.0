using NUnit.Framework;

using PAC.DataStructures;
using PAC.Shapes;
using PAC.Tests.Shapes.DefaultTests;
using PAC.Tests.Shapes.RequiredTests;
using PAC.Tests.Shapes.TestUtils;

using System.Collections.Generic;

namespace PAC.Tests.Shapes
{
    public class Rectangle_Tests : I2DShape_DefaultTests<Rectangle>, I2DShape_RequiredTests
    {
        protected override IEnumerable<Rectangle> testCases
        {
            get
            {
                foreach (bool filled in new bool[] { false, true })
                {
                    foreach (IntVector2 bottomLeft in new IntRect(new IntVector2(-2, -2), new IntVector2(2, 2)))
                    {
                        foreach (IntVector2 topRight in bottomLeft + new IntRect(IntVector2.zero, new IntVector2(5, 5)))
                        {
                            yield return new Rectangle(new IntRect(bottomLeft, topRight), filled);
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
                    CollectionAssert.AreEquivalent(new IntVector2[] { pixel }, new Rectangle(new IntRect(pixel, pixel), filled), $"Failed with {pixel} {(filled ? "filled" : "unfilled")}.");
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
                    Rectangle rectangle = new Rectangle(new IntRect(bottomLeft, topRight), true);
                    CollectionAssert.AreEquivalent(rectangle.boundingRect, rectangle, $"Failed with {rectangle}.");
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void NoRepeats()
        {
            foreach (Rectangle rectangle in testCases)
            {
                ShapeAssert.NoRepeats(rectangle);
            }
        }
    }
}
