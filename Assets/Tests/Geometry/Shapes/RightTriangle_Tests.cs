using NUnit.Framework;

using PAC.DataStructures;
using PAC.Extensions;
using PAC.Geometry.Shapes;
using PAC.Tests.Geometry.Shapes.DefaultTests;
using PAC.Tests.Geometry.Shapes.RequiredTests;
using PAC.Tests.Geometry.Shapes.TestUtils;

using System;
using System.Collections.Generic;
using System.Linq;

namespace PAC.Tests.Geometry.Shapes
{
    /// <summary>
    /// Tests for <see cref="RightTriangle"/>.
    /// </summary>
    public class RightTriangle_Tests : I2DShape_DefaultTests<RightTriangle>, I2DShape_RequiredTests
    {
        protected override IEnumerable<RightTriangle> testCases => Enumerable.Concat(exampleTestCases, randomTestCases);
        private IEnumerable<RightTriangle> exampleTestCases
        {
            get
            {
                foreach (RightTriangle.RightAngleLocation rightAngleLocation in TypeExtensions.GetValues<RightTriangle.RightAngleLocation>())
                {
                    foreach (bool filled in new bool[] { false, true })
                    {
                        for (int x = 0; x <= 2; x++)
                        {
                            for (int y = 0; y <= 2; y++)
                            {
                                yield return new RightTriangle(new IntRect((0, 0), (x, y)), rightAngleLocation, filled);
                            }
                        }
                    }
                }
            }
        }
        private IEnumerable<RightTriangle> randomTestCases
        {
            get
            {
                Random random = new Random(0);
                IntRect testRegion = new IntRect((-20, -20), (20, 20));
                for (int i = 0; i < 1_000; i++)
                {
                    yield return new RightTriangle(testRegion.RandomSubRect(random), random.NextElement(TypeExtensions.GetValues<RightTriangle.RightAngleLocation>()), random.NextBool());
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public override void ShapeSinglePoint()
        {
            foreach (RightTriangle.RightAngleLocation rightAngleLocation in TypeExtensions.GetValues<RightTriangle.RightAngleLocation>())
            {
                foreach (bool filled in new bool[] { false, true })
                {
                    foreach (IntVector2 point in new IntRect((-5, -5), (5, 5)))
                    {
                        ShapeAssert.SameGeometry(
                            new IntVector2[] { point },
                            new RightTriangle(new IntRect(point, point), rightAngleLocation, filled),
                            $"Failed with {point} {(filled ? "filled" : "unfilled")}."
                            );
                    }
                }
            }
        }

        /// <summary>
        /// Tests that 2x2 <see cref="RightTriangle"/>s have the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void Shape2x2()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                (IntVector2[] expected, RightTriangle triangle)[] testCases =
                {
                    (new IntVector2[] { (0, 0), (0, 1), (1, 0) }, new RightTriangle(new IntRect((0, 0), (1, 1)), RightTriangle.RightAngleLocation.BottomLeft, filled)),
                    (new IntVector2[] { (0, 0), (1, 0), (1, 1) }, new RightTriangle(new IntRect((0, 0), (1, 1)), RightTriangle.RightAngleLocation.BottomRight, filled)),
                    (new IntVector2[] { (0, 0), (0, 1), (1, 1) }, new RightTriangle(new IntRect((0, 0), (1, 1)), RightTriangle.RightAngleLocation.TopLeft, filled)),
                    (new IntVector2[] { (0, 1), (1, 0), (1, 1) }, new RightTriangle(new IntRect((0, 0), (1, 1)), RightTriangle.RightAngleLocation.TopRight, filled))
                };

                foreach ((IntVector2[] expected, RightTriangle triangle) in testCases)
                {
                    ShapeAssert.SameGeometry(expected, triangle, $"Failed with {triangle}.");
                }
            }
        }

        /// <summary>
        /// Tests that NxN <see cref="RightTriangle"/>s have the correct shape for N > 2.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapeNxN_NGreaterThan2()
        {
            for (int n = 2; n < 10; n++)
            {
                foreach (bool filled in new bool[] { false, true })
                {
                    RightTriangle triangle = new RightTriangle(new IntRect((0, 0), (n, n)), RightTriangle.RightAngleLocation.BottomRight, filled);

                    List<IntVector2> expected = new List<IntVector2>();

                    if (filled)
                    {
                        for (int x = 0; x < n; x++)
                        {
                            for (int y = 0; y <= x + 1; y++)
                            {
                                expected.Add((x, y));
                            }
                        }
                    }
                    else
                    {
                        for (int x = 0; x < n; x++)
                        {
                            expected.Add((x, 0));
                            expected.Add((x, x + 1));
                        }
                    }

                    for (int y = 0; y <= n; y++)
                    {
                        expected.Add((n, y));
                    }

                    ShapeAssert.SameGeometry(expected, triangle, $"Failed with n = {n}.");
                }
            }
        }

        /// <summary>
        /// Tests that 1xN <see cref="RightTriangle"/>s have the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void Shape1xN()
        {
            foreach (RightTriangle.RightAngleLocation rightAngleLocation in TypeExtensions.GetValues<RightTriangle.RightAngleLocation>())
            {
                foreach (bool filled in new bool[] { false, true })
                {
                    for (int height = 1; height <= 10; height++)
                    {
                        RightTriangle triangle = new RightTriangle(new IntRect((0, 0), (0, height - 1)), rightAngleLocation, filled);
                        IntRect expected = new IntRect((0, 0), (0, height - 1));
                        ShapeAssert.SameGeometry(expected, triangle, $"Failed with height {height}.");
                    }
                }
            }
        }

        /// <summary>
        /// Tests that 2xN <see cref="RightTriangle"/>s have the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void Shape2xN()
        {
            for (int height = 1; height <= 10; height++)
            {
                foreach (bool filled in new bool[] { false, true })
                {
                    RightTriangle triangle = new RightTriangle(new IntRect((0, 0), (1, height - 1)), RightTriangle.RightAngleLocation.BottomLeft, filled);

                    List<IntVector2> expected = new List<IntVector2>();
                    for (int y = 0; y <= height - 1; y++)
                    {
                        expected.Add((0, y));
                    }
                    for (int y = 0; y <= UnityEngine.Mathf.Ceil(height / 2f) - 1; y++)
                    {
                        expected.Add((1, y));
                    }

                    ShapeAssert.SameGeometry(expected, triangle, $"Failed with height {height}.");
                }
            }
        }

        /// <summary>
        /// Tests that an example <see cref="RightTriangle"/> has the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapeExample1()
        {
            RightTriangle triangle = new RightTriangle(new IntRect((0, 0), (1, 4)), RightTriangle.RightAngleLocation.BottomRight, false);
            IntVector2[] expected = new IntVector2[] { (0, 0), (0, 1), (0, 2), (1, 0), (1, 1), (1, 2), (1, 3), (1, 4) };
            ShapeAssert.SameGeometry(expected, triangle);
        }

        /// <summary>
        /// Tests that an example <see cref="RightTriangle"/> has the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapeExample2()
        {
            RightTriangle triangle = new RightTriangle(new IntRect((1, 2), (10, 19)), RightTriangle.RightAngleLocation.BottomRight, false);

            List<IntVector2> expected = new List<IntVector2>();
            for (int x = 1; x <= 10; x++)
            {
                expected.Add((x, 2));
            }
            for (int y = 3; y <= 19; y++)
            {
                expected.Add((10, y));
            }
            expected.AddRange(new Line((9, 19), (1, 3)));

            ShapeAssert.SameGeometry(expected, triangle);
        }

        /// <summary>
        /// Tests that <see cref="RightTriangle.rightAngleCorner"/>, <see cref="RightTriangle.leftCorner"/>, <see cref="RightTriangle.rightCorner"/>, <see cref="RightTriangle.topCorner"/> and
        /// <see cref="RightTriangle.bottomCorner"/> are the correct corners of the <see cref="RightTriangle"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void Corners()
        {
            foreach (RightTriangle triangle in testCases)
            {
                foreach (IntVector2 corner in new IntVector2[] { triangle.rightAngleCorner, triangle.topCorner, triangle.bottomCorner, triangle.leftCorner, triangle.rightCorner })
                {
                    CollectionAssert.Contains(triangle, corner, $"Failed with {triangle} and {corner}.");
                }

                IntRect boundingRect = IntRect.BoundingRect(triangle);

                CollectionAssert.Contains(
                    new IntVector2[] { boundingRect.bottomLeft, boundingRect.bottomRight, boundingRect.topLeft, boundingRect.topRight },
                    triangle.rightAngleCorner,
                    $"Failed with {triangle} and {triangle.rightAngleCorner}."
                    );
                Assert.True(triangle.leftCorner == boundingRect.bottomLeft || triangle.leftCorner == boundingRect.topLeft, $"Failed with {triangle}.");
                Assert.True(triangle.rightCorner == boundingRect.bottomRight || triangle.rightCorner == boundingRect.topRight, $"Failed with {triangle}.");
                Assert.True(triangle.topCorner == boundingRect.topLeft || triangle.topCorner == boundingRect.topRight, $"Failed with {triangle}.");
                Assert.True(triangle.bottomCorner == boundingRect.bottomLeft || triangle.bottomCorner == boundingRect.bottomRight, $"Failed with {triangle}.");

                if (boundingRect.Count > 1)
                {
                    Assert.AreNotEqual(triangle.topCorner, triangle.bottomCorner, $"Failed with {triangle}.");
                    Assert.AreNotEqual(triangle.leftCorner, triangle.rightCorner, $"Failed with {triangle}.");
                }

                if (boundingRect.width >= 2 && boundingRect.height >= 2)
                {
                    Assert.True(IntVector2.upDownLeftRight.Any(
                        direction => 
                            triangle.Contains(triangle.rightAngleCorner + direction) &&
                            triangle.Contains(triangle.rightAngleCorner + direction.Rotate(RotationAngle._90)) &&
                            !triangle.Contains(triangle.rightAngleCorner + direction.Rotate(RotationAngle._180)) &&
                            !triangle.Contains(triangle.rightAngleCorner + direction.Rotate(RotationAngle.Minus90))
                        ),
                        $"Failed with {triangle}."); // Test it is actually a right angle at that corner

                    if (triangle.rightAngleCorner == boundingRect.bottomLeft)
                    {
                        Assert.AreEqual(boundingRect.bottomRight, triangle.bottomCorner, $"Failed with {triangle}.");
                        Assert.AreEqual(boundingRect.topLeft, triangle.topCorner, $"Failed with {triangle}.");
                        Assert.AreEqual(boundingRect.topLeft, triangle.leftCorner, $"Failed with {triangle}.");
                        Assert.AreEqual(boundingRect.bottomRight, triangle.rightCorner, $"Failed with {triangle}.");
                    }
                    else if (triangle.rightAngleCorner == boundingRect.bottomRight)
                    {
                        Assert.AreEqual(boundingRect.bottomLeft, triangle.bottomCorner, $"Failed with {triangle}.");
                        Assert.AreEqual(boundingRect.topRight, triangle.topCorner, $"Failed with {triangle}.");
                        Assert.AreEqual(boundingRect.bottomLeft, triangle.leftCorner, $"Failed with {triangle}.");
                        Assert.AreEqual(boundingRect.topRight, triangle.rightCorner, $"Failed with {triangle}.");
                    }
                    else if (triangle.rightAngleCorner == boundingRect.topLeft)
                    {
                        Assert.AreEqual(boundingRect.bottomLeft, triangle.bottomCorner, $"Failed with {triangle}.");
                        Assert.AreEqual(boundingRect.topRight, triangle.topCorner, $"Failed with {triangle}.");
                        Assert.AreEqual(boundingRect.bottomLeft, triangle.leftCorner, $"Failed with {triangle}.");
                        Assert.AreEqual(boundingRect.topRight, triangle.rightCorner, $"Failed with {triangle}.");
                    }
                    else if (triangle.rightAngleCorner == boundingRect.topRight)
                    {
                        Assert.AreEqual(boundingRect.bottomRight, triangle.bottomCorner, $"Failed with {triangle}.");
                        Assert.AreEqual(boundingRect.topLeft, triangle.topCorner, $"Failed with {triangle}.");
                        Assert.AreEqual(boundingRect.topLeft, triangle.leftCorner, $"Failed with {triangle}.");
                        Assert.AreEqual(boundingRect.bottomRight, triangle.rightCorner, $"Failed with {triangle}.");
                    }
                    else
                    {
                        Assert.Fail($"Failed with {triangle}.");
                    }
                }
            }
        }

        /// <summary>
        /// Tests that <see cref="RightTriangle"/>s has at least one horizontal edge that's on the border of the <see cref="RightTriangle"/>'s bounding rect.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void HasAHorizontalEdgeAlongBorderOfBoundingRect()
        {
            foreach (RightTriangle triangle in testCases)
            {
                IntRect boundingRect = IntRect.BoundingRect(triangle);
                Assert.True(
                    new IntRect(boundingRect.bottomLeft, boundingRect.bottomRight).ToHashSet().IsSubsetOf(triangle) ||
                    new IntRect(boundingRect.topLeft, boundingRect.topRight).ToHashSet().IsSubsetOf(triangle),
                    $"Failed with {triangle}."
                    );
            }
        }
        /// <summary>
        /// Tests that <see cref="RightTriangle"/>s has at least one vertical edge that's on the border of the <see cref="RightTriangle"/>'s bounding rect.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void HasAVerticalEdgeAlongBorderOfBoundingRect()
        {
            foreach (RightTriangle triangle in testCases)
            {
                IntRect boundingRect = IntRect.BoundingRect(triangle);
                Assert.True(
                    new IntRect(boundingRect.bottomLeft, boundingRect.topLeft).ToHashSet().IsSubsetOf(triangle) ||
                    new IntRect(boundingRect.bottomRight, boundingRect.topRight).ToHashSet().IsSubsetOf(triangle),
                    $"Failed with {triangle}."
                    );
            }
        }

        /// <summary>
        /// Tests that the <see cref="RightTriangle"/> enumerator doesn't repeat any points.
        /// </summary>
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
