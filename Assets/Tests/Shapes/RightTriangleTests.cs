using NUnit.Framework;
using PAC.DataStructures;
using PAC.Drawing;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PAC.Tests
{
    public class RightTriangleTests
    {
        /// <summary>
        /// Tests that right triangles that are single points have the correct shape.
        /// </summary>
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
                            IShapeTestHelper.Count(new Shapes.RightTriangle(bottomLeft, topRight, rightAngleLocation, filled));
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
                    IShapeTestHelper.NoRepeatsAtAll(new Shapes.RightTriangle(IntVector2.zero, topRight, Shapes.RightTriangle.RightAngleLocation.Bottom, filled));
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
                            Shapes.RightTriangle triangle = new Shapes.RightTriangle(bottomCorner, topCorner, rightAngleLocation, filled);

                            Shapes.RightTriangle expected = new Shapes.RightTriangle(IntVector2.zero, topCorner - bottomCorner, rightAngleLocation, filled);
                            IEnumerable<IntVector2> translated = triangle.Select(p => p - bottomCorner);

                            Assert.True(expected.SequenceEqual(translated), "Failed with " + triangle);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tests that reflecting an ellipse gives the same shape as creating one with the corners reflected.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void RotationalInvariance()
        {
            Shapes.RightTriangle.RightAngleLocation RotateRightAngleLocation(Shapes.RightTriangle.RightAngleLocation rightAngleLocation)
            {
                switch (rightAngleLocation)
                {
                    case Shapes.RightTriangle.RightAngleLocation.Top: return Shapes.RightTriangle.RightAngleLocation.Top;
                    case Shapes.RightTriangle.RightAngleLocation.Bottom: return Shapes.RightTriangle.RightAngleLocation.Bottom;
                    case Shapes.RightTriangle.RightAngleLocation.Left: return Shapes.RightTriangle.RightAngleLocation.Right;
                    case Shapes.RightTriangle.RightAngleLocation.Right: return Shapes.RightTriangle.RightAngleLocation.Left;
                    default: throw new NotImplementedException("Unknown / unimplemented RightAngleLocation: " + rightAngleLocation);
                }
            }

            foreach (Shapes.RightTriangle.RightAngleLocation rightAngleLocation in new Shapes.RightTriangle.RightAngleLocation[]
                {
                Shapes.RightTriangle.RightAngleLocation.Top, Shapes.RightTriangle.RightAngleLocation.Bottom,
                Shapes.RightTriangle.RightAngleLocation.Right, Shapes.RightTriangle.RightAngleLocation.Left
                })
            {
                foreach (bool filled in new bool[] { false, true })
                {
                    foreach (IntVector2 topRight in new IntRect(IntVector2.zero, new IntVector2(10, 10)))
                    {
                        Shapes.RightTriangle triangle = new Shapes.RightTriangle(IntVector2.zero, topRight, rightAngleLocation, filled);

                        Shapes.RightTriangle expected = new Shapes.RightTriangle(triangle.bottomCorner.Rotate(RotationAngle._90), triangle.topCorner.Rotate(RotationAngle._90),
                            RotateRightAngleLocation(rightAngleLocation), filled);
                        IEnumerable<IntVector2> rotated = triangle.Select(p => p.Rotate(RotationAngle._90));

                        Assert.True(expected.ToHashSet().SetEquals(rotated), "Failed with " + triangle);
                    }
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
            Shapes.RightTriangle.RightAngleLocation FlipRightAngleLocation(Shapes.RightTriangle.RightAngleLocation rightAngleLocation, FlipAxis axis)
            {
                switch (axis)
                {
                    case FlipAxis.None: return rightAngleLocation;
                    case FlipAxis.Vertical:
                        {
                            switch (rightAngleLocation)
                            {
                                case Shapes.RightTriangle.RightAngleLocation.Top: return Shapes.RightTriangle.RightAngleLocation.Top;
                                case Shapes.RightTriangle.RightAngleLocation.Bottom: return Shapes.RightTriangle.RightAngleLocation.Bottom;
                                case Shapes.RightTriangle.RightAngleLocation.Left: return Shapes.RightTriangle.RightAngleLocation.Right;
                                case Shapes.RightTriangle.RightAngleLocation.Right: return Shapes.RightTriangle.RightAngleLocation.Left;
                                default: throw new NotImplementedException("Unknown / unimplemented RightAngleLocation: " + rightAngleLocation);
                            }
                        }
                    case FlipAxis.Horizontal:
                        {
                            switch (rightAngleLocation)
                            {
                                case Shapes.RightTriangle.RightAngleLocation.Top: return Shapes.RightTriangle.RightAngleLocation.Bottom;
                                case Shapes.RightTriangle.RightAngleLocation.Bottom: return Shapes.RightTriangle.RightAngleLocation.Top;
                                case Shapes.RightTriangle.RightAngleLocation.Left: return Shapes.RightTriangle.RightAngleLocation.Left;
                                case Shapes.RightTriangle.RightAngleLocation.Right: return Shapes.RightTriangle.RightAngleLocation.Right;
                                default: throw new NotImplementedException("Unknown / unimplemented RightAngleLocation: " + rightAngleLocation);
                            }
                        }
                    case FlipAxis._45Degrees:
                        {
                            switch (rightAngleLocation)
                            {
                                case Shapes.RightTriangle.RightAngleLocation.Top: return Shapes.RightTriangle.RightAngleLocation.Right;
                                case Shapes.RightTriangle.RightAngleLocation.Bottom: return Shapes.RightTriangle.RightAngleLocation.Left;
                                case Shapes.RightTriangle.RightAngleLocation.Left: return Shapes.RightTriangle.RightAngleLocation.Bottom;
                                case Shapes.RightTriangle.RightAngleLocation.Right: return Shapes.RightTriangle.RightAngleLocation.Top;
                                default: throw new NotImplementedException("Unknown / unimplemented RightAngleLocation: " + rightAngleLocation);
                            }
                        }
                    case FlipAxis.Minus45Degrees:
                        {
                            switch (rightAngleLocation)
                            {
                                case Shapes.RightTriangle.RightAngleLocation.Top: return Shapes.RightTriangle.RightAngleLocation.Left;
                                case Shapes.RightTriangle.RightAngleLocation.Bottom: return Shapes.RightTriangle.RightAngleLocation.Right;
                                case Shapes.RightTriangle.RightAngleLocation.Left: return Shapes.RightTriangle.RightAngleLocation.Top;
                                case Shapes.RightTriangle.RightAngleLocation.Right: return Shapes.RightTriangle.RightAngleLocation.Bottom;
                                default: throw new NotImplementedException("Unknown / unimplemented RightAngleLocation: " + rightAngleLocation);
                            }
                        }
                    default: throw new NotImplementedException("Unknown / unimplemented FlipAxis: " + axis);
                }
            }

            foreach (FlipAxis axis in new FlipAxis[] { FlipAxis.Vertical, FlipAxis.Horizontal, FlipAxis._45Degrees, FlipAxis.Minus45Degrees })
            {
                foreach (Shapes.RightTriangle.RightAngleLocation rightAngleLocation in new Shapes.RightTriangle.RightAngleLocation[]
                {
                Shapes.RightTriangle.RightAngleLocation.Top, Shapes.RightTriangle.RightAngleLocation.Bottom,
                Shapes.RightTriangle.RightAngleLocation.Right, Shapes.RightTriangle.RightAngleLocation.Left
                })
                {
                    foreach (bool filled in new bool[] { false, true })
                    {
                        foreach (IntVector2 topRight in new IntRect(IntVector2.zero, new IntVector2(10, 10)))
                        {
                            Shapes.RightTriangle triangle = new Shapes.RightTriangle(IntVector2.zero, topRight, rightAngleLocation, filled);

                            Shapes.RightTriangle expected = new Shapes.RightTriangle(triangle.bottomCorner.Flip(axis), triangle.topCorner.Flip(axis),
                                FlipRightAngleLocation(rightAngleLocation, axis), filled);
                            IEnumerable<IntVector2> reflected = triangle.Select(p => p.Flip(axis));

                            Assert.True(expected.ToHashSet().SetEquals(reflected), "Failed with " + triangle + " and FlipAxis." + axis);
                        }
                    }
                }
            }
        }
    }
}
