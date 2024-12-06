using NUnit.Framework;
using PAC.DataStructures;
using PAC.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PAC.Tests
{
    public class RightTriangleTests : I2DShapeTests<Shapes.RightTriangle>
    {
        protected override IEnumerable<Shapes.RightTriangle> testCases
        {
            get
            {
                foreach (Shapes.RightTriangle.RightAngleLocation rightAngleLocation in new Shapes.RightTriangle.RightAngleLocation[]
                {
                Shapes.RightTriangle.RightAngleLocation.Top, Shapes.RightTriangle.RightAngleLocation.Bottom,
                Shapes.RightTriangle.RightAngleLocation.Right, Shapes.RightTriangle.RightAngleLocation.Left
                })
                {
                    foreach (bool filled in new bool[] { false, true })
                    {
                        foreach (IntVector2 bottomLeft in new IntRect(new IntVector2(-2, -2), new IntVector2(2, 2)))
                        {
                            foreach (IntVector2 topRight in bottomLeft + new IntRect(new IntVector2(-5, 0), new IntVector2(5, 5)))
                            {
                                yield return new Shapes.RightTriangle(bottomLeft, topRight, rightAngleLocation, filled);
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
            foreach (Shapes.RightTriangle.RightAngleLocation rightAngleLocation in new Shapes.RightTriangle.RightAngleLocation[]
            {
                Shapes.RightTriangle.RightAngleLocation.Top, Shapes.RightTriangle.RightAngleLocation.Bottom,
                Shapes.RightTriangle.RightAngleLocation.Right, Shapes.RightTriangle.RightAngleLocation.Left
            })
            {
                foreach (bool filled in new bool[] { false, true })
                {
                    foreach (IntVector2 pixel in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
                    {
                        Assert.True(new Shapes.RightTriangle(pixel, pixel, rightAngleLocation, filled).SequenceEqual(new IntVector2[] { pixel }), "Failed with " + pixel + " " +
                            (filled ? "filled" : "unfilled"));
                    }
                }
            }
        }
    }
}
