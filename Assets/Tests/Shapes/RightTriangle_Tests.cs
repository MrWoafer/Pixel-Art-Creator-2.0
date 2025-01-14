using NUnit.Framework;

using PAC.DataStructures;
using PAC.Shapes;
using PAC.Tests.Shapes.DefaultTests;
using PAC.Tests.Shapes.RequiredTests;
using PAC.Tests.Shapes.TestUtils;

using System.Collections.Generic;

namespace PAC.Tests.Shapes
{
    public class RightTriangle_Tests : I2DShape_DefaultTests<RightTriangle>, I2DShape_RequiredTests
    {
        protected override IEnumerable<RightTriangle> testCases
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

        [Test]
        [Category("Shapes")]
        public void NoRepeats()
        {
            foreach (RightTriangle triangle in testCases)
            {
                ShapeAssert.NoRepeats(triangle);
            }
        }
    }
}
