using NUnit.Framework;
using PAC.DataStructures;
using PAC.Drawing;
using System.Collections.Generic;

namespace PAC.Tests
{
    public class RightTriangleTests : I2DShapeTests<RightTriangle>
    {
        public override IEnumerable<RightTriangle> testCases
        {
            get
            {
                foreach (RightTriangle.RightAngleLocation rightAngleLocation in new RightTriangle.RightAngleLocation[]
                {
                RightTriangle.RightAngleLocation.Top, RightTriangle.RightAngleLocation.Bottom,
                RightTriangle.RightAngleLocation.Right, RightTriangle.RightAngleLocation.Left
                })
                {
                    foreach (bool filled in new bool[] { false, true })
                    {
                        foreach (IntVector2 bottomLeft in new IntRect(new IntVector2(-2, -2), new IntVector2(2, 2)))
                        {
                            foreach (IntVector2 topRight in bottomLeft + new IntRect(new IntVector2(-5, 0), new IntVector2(5, 5)))
                            {
                                yield return new RightTriangle(bottomLeft, topRight, rightAngleLocation, filled);
                            }
                        }
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public override void ShapeSinglePoint()
        {
            foreach (RightTriangle.RightAngleLocation rightAngleLocation in new RightTriangle.RightAngleLocation[]
            {
                RightTriangle.RightAngleLocation.Top, RightTriangle.RightAngleLocation.Bottom,
                RightTriangle.RightAngleLocation.Right, RightTriangle.RightAngleLocation.Left
            })
            {
                foreach (bool filled in new bool[] { false, true })
                {
                    foreach (IntVector2 pixel in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
                    {
                        CollectionAssert.AreEqual(new IntVector2[] { pixel }, new RightTriangle(pixel, pixel, rightAngleLocation, filled), "Failed with " + pixel + " " +
                            (filled ? "filled" : "unfilled"));
                    }
                }
            }
        }
    }
}
