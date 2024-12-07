using NUnit.Framework;
using PAC.DataStructures;
using PAC.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace PAC.Tests
{
    public class IsometricRectangleTests : IIsometricShapeTests<Shapes.IsometricRectangle>
    {
        public override IEnumerable<Shapes.IsometricRectangle> testCases
        {
            get
            {
                foreach (bool filled in new bool[] { false, true })
                {
                    foreach (IntVector2 bottomLeft in new IntRect(new IntVector2(-2, -2), new IntVector2(2, 2)))
                    {
                        foreach (IntVector2 topRight in bottomLeft + new IntRect(IntVector2.zero, new IntVector2(5, 5)))
                        {
                            yield return new Shapes.IsometricRectangle(bottomLeft, topRight, filled);
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
                    Assert.True(new Shapes.IsometricRectangle(pixel, pixel, filled).SequenceEqual(new IntVector2[] { pixel }), "Failed with " + pixel + " " + (filled ? "filled" : "unfilled"));
                }
            }
        }

        /// <summary>
        /// Tests that the topCorner, bottomCorner, leftCorner and rightCorner properties are indeed corners of the shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void CornersAreCorners()
        {
            foreach (Shapes.IsometricRectangle rectangle in testCases)
            {
                Assert.AreEqual(rectangle.boundingRect.bottomLeft.x, rectangle.leftCorner.x, "Failed with " + rectangle);
                Assert.AreEqual(rectangle.boundingRect.topRight.x, rectangle.rightCorner.x, "Failed with " + rectangle);
                Assert.AreEqual(rectangle.boundingRect.bottomLeft.y, rectangle.bottomCorner.y, "Failed with " + rectangle);
                Assert.AreEqual(rectangle.boundingRect.topRight.y, rectangle.topCorner.y, "Failed with " + rectangle);
            }
        }

        [Test]
        [Category("Shapes")]
        public void IsIsometricSquare()
        {
            foreach (Shapes.IsometricRectangle rectangle in testCases)
            {
                if (rectangle.isIsometricSquare)
                {
                    IShapeTestHelper.ReflectiveSymmetry(rectangle, FlipAxis.Horizontal);
                }
                else
                {
                    IShapeTestHelper.ReflectiveAsymmetry(rectangle, FlipAxis.Horizontal);
                }
            }
        }
    }
}
