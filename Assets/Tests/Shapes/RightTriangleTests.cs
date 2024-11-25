using NUnit.Framework;
using PAC.DataStructures;
using PAC.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PAC.Tests
{
    public class RightTriangleTests : IShapeTests
    {
        [Test]
        [Category("Shapes")]
        public void ShapeSinglePoint()
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
                        Assert.True(new Shapes.RightTriangle(pixel, pixel, rightAngleLocation, filled).SequenceEqual(new IntVector2[] { pixel }), "Failed with " + pixel);
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void BoundingRect()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 bottomLeft in new IntRect(new IntVector2(-2, -2), new IntVector2(2, 2)))
                {
                    foreach (IntVector2 topRight in bottomLeft + new IntRect(IntVector2.zero, new IntVector2(5, 5)))
                    {
                        IShapeTestHelper.BoundingRect(new Shapes.RightTriangle(bottomLeft, topRight, Shapes.RightTriangle.RightAngleLocation.Bottom, filled));
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void Count()
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
                            IShapeTestHelper.CountDistinct(new Shapes.RightTriangle(bottomLeft, topRight, rightAngleLocation, filled));
                        }
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void Contains()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 bottomLeft in new IntRect(new IntVector2(-2, -2), new IntVector2(2, 2)))
                {
                    foreach (IntVector2 topRight in bottomLeft + new IntRect(IntVector2.zero, new IntVector2(5, 5)))
                    {
                        IShapeTestHelper.Contains(new Shapes.RightTriangle(bottomLeft, topRight, Shapes.RightTriangle.RightAngleLocation.Bottom, filled));
                    }
                }
            }
        }

        /// <summary>
        /// Tests that the enumerator doesn't repeat any pixels.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void NoRepeats()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 topRight in new IntRect(IntVector2.zero, new IntVector2(10, 10)))
                {
                    IShapeTestHelper.NoRepeatsAtAll(new Shapes.RightTriangle(IntVector2.zero, topRight, Shapes.RightTriangle.RightAngleLocation.Bottom, filled));
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void Translate()
        {
            foreach (Shapes.RightTriangle.RightAngleLocation rightAngleLocation in new Shapes.RightTriangle.RightAngleLocation[]
                {
                Shapes.RightTriangle.RightAngleLocation.Top, Shapes.RightTriangle.RightAngleLocation.Bottom,
                Shapes.RightTriangle.RightAngleLocation.Right, Shapes.RightTriangle.RightAngleLocation.Left
                })
            {
                foreach (bool filled in new bool[] { false, true })
                {
                    foreach (IntVector2 bottomCorner in new IntRect(new IntVector2(-2, -2), new IntVector2(2, 2)))
                    {
                        foreach (IntVector2 topCorner in bottomCorner + new IntRect(new IntVector2(-5, 0), new IntVector2(5, 5)))
                        {
                            IShapeTestHelper.Translate(new Shapes.RightTriangle(bottomCorner, topCorner, rightAngleLocation, filled));
                        }
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void Rotate()
        {
            foreach (Shapes.RightTriangle.RightAngleLocation rightAngleLocation in new Shapes.RightTriangle.RightAngleLocation[]
                {
                Shapes.RightTriangle.RightAngleLocation.Top, Shapes.RightTriangle.RightAngleLocation.Bottom,
                Shapes.RightTriangle.RightAngleLocation.Right, Shapes.RightTriangle.RightAngleLocation.Left
                })
            {
                foreach (bool filled in new bool[] { false, true })
                {
                    foreach (IntVector2 bottomCorner in new IntRect(new IntVector2(-2, -2), new IntVector2(2, 2)))
                    {
                        foreach (IntVector2 topCorner in bottomCorner + new IntRect(new IntVector2(-5, 0), new IntVector2(5, 5)))
                        {
                            IShapeTestHelper.Rotate(new Shapes.RightTriangle(bottomCorner, topCorner, rightAngleLocation, filled));
                        }
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void Flip()
        {
            foreach (Shapes.RightTriangle.RightAngleLocation rightAngleLocation in new Shapes.RightTriangle.RightAngleLocation[]
                {
                Shapes.RightTriangle.RightAngleLocation.Top, Shapes.RightTriangle.RightAngleLocation.Bottom,
                Shapes.RightTriangle.RightAngleLocation.Right, Shapes.RightTriangle.RightAngleLocation.Left
                })
            {
                foreach (bool filled in new bool[] { false, true })
                {
                    foreach (IntVector2 bottomCorner in new IntRect(new IntVector2(-2, -2), new IntVector2(2, 2)))
                    {
                        foreach (IntVector2 topCorner in bottomCorner + new IntRect(new IntVector2(-5, 0), new IntVector2(5, 5)))
                        {
                            IShapeTestHelper.Flip(new Shapes.RightTriangle(bottomCorner, topCorner, rightAngleLocation, filled));
                        }
                    }
                }
            }
        }
    }
}
