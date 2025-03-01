using NUnit.Framework;

using PAC.DataStructures;
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
    /// Tests for <see cref="IsometricRectangle"/>.
    /// </summary>
    public class IsometricRectangle_Tests : IIsometricShape_DefaultTests<IsometricRectangle>, IIsometricShape_RequiredTests
    {
        protected override IEnumerable<IsometricRectangle> testCases => Enumerable.Concat(exampleTestCases, randomTestCases);
        private IEnumerable<IsometricRectangle> exampleTestCases => new IsometricRectangle[]
        {
            new IsometricRectangle((0, 0), (0, 0), false),
            new IsometricRectangle((0, 0), (0, 0), true),
            new IsometricRectangle((0, 0), (1, 0), false),
            new IsometricRectangle((0, 0), (1, 0), true),
            new IsometricRectangle((0, 0), (-1, 0), false),
            new IsometricRectangle((0, 0), (-1, 0), true),
            new IsometricRectangle((0, 0), (0, 1), false),
            new IsometricRectangle((0, 0), (0, 1), true),
            new IsometricRectangle((0, 0), (0, -1), false),
            new IsometricRectangle((0, 0), (0, -1), true)
        };
        private IEnumerable<IsometricRectangle> randomTestCases
        {
            get
            {
                Random random = new Random(0);
                IntRect testRegion = new IntRect((-20, -20), (20, 20));
                for (int i = 0; i < 1_000; i++)
                {
                    IntVector2 start = testRegion.RandomPoint(random);
                    IntVector2 end = start + testRegion.RandomPoint(random);
                    foreach (bool filled in new bool[] { false, true })
                    {
                        yield return new IsometricRectangle(start, end, filled);
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
                foreach (IntVector2 point in new IntRect((-5, -5), (5, 5)))
                {
                    ShapeAssert.SameGeometry(new IntVector2[] { point }, new IsometricRectangle(point, point, filled), $"Failed with {point} {(filled ? "filled" : "unfilled")}.");
                }
            }
        }

        /// <summary>
        /// Tests that an example <see cref="IsometricRectangle"/> has the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapeExample()
        {
            IsometricRectangle rectangle = new IsometricRectangle((0, 0), (11, -1), false);
            IntVector2[] expected = new IntVector2[]
            {
                (0, 0), (1, 0), (2, -1), (3, -1), (4, -2), (5, -2), (6, -3), (7, -3), (8, -2), (9, -2), (10, -1), (11, -1), (10, -1), (9, 0), (8, 0), (7, 1), (6, 1), (5, 2), (4, 2), (3, 1),
                (2, 1), (1, 0), (0, 0)
            };

            ShapeAssert.SameGeometry(expected, rectangle);
        }

        /// <summary>
        /// Tests that <see cref="IsometricRectangle.topCorner"/>, <see cref="IsometricRectangle.bottomCorner"/>, <see cref="IsometricRectangle.leftCorner"/> and
        /// <see cref="IsometricRectangle.rightCorner"/> are indeed corners of the shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void CornersAreCorners()
        {
            foreach (IsometricRectangle rectangle in testCases)
            {
                IntRect boundingRect = IntRect.BoundingRect(rectangle);

                Assert.AreEqual(boundingRect.minX, rectangle.leftCorner.x, $"Failed with {rectangle}.");
                Assert.AreEqual(boundingRect.maxX, rectangle.rightCorner.x, $"Failed with {rectangle}.");
                Assert.AreEqual(boundingRect.minY, rectangle.bottomCorner.y, $"Failed with {rectangle}.");
                Assert.AreEqual(boundingRect.maxY, rectangle.topCorner.y, $"Failed with {rectangle}.");
            }
        }

        /// <summary>
        /// Tests that <see cref="IsometricRectangle.startCorner"/> and <see cref="IsometricRectangle.endCorner"/> are among <see cref="IsometricRectangle.leftCorner"/>,
        /// <see cref="IsometricRectangle.rightCorner"/>, <see cref="IsometricRectangle.topCorner"/> and <see cref="IsometricRectangle.bottomCorner"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void StartEndCornersAreAmongLeftRightTopBottomCorners()
        {
            foreach (IsometricRectangle rectangle in testCases)
            {
                IntVector2[] leftRightTopBottom = new IntVector2[] { rectangle.leftCorner, rectangle.rightCorner, rectangle.topCorner, rectangle.bottomCorner };

                CollectionAssert.Contains(leftRightTopBottom, rectangle.startCorner, $"Failed with {rectangle}.");
                CollectionAssert.Contains(leftRightTopBottom, rectangle.endCorner, $"Failed with {rectangle}.");
            }
        }

        /// <summary>
        /// Tests that <see cref="IsometricRectangle.border"/> is indeed the border of the <see cref="IsometricRectangle"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void borderIsBorder()
        {
            foreach (IsometricRectangle rectangle in testCases)
            {
                ShapeAssert.SameGeometry(ShapeUtils.GetBorder(rectangle), rectangle.border, $"Failed with {rectangle}.");
            }
        }

        /// <summary>
        /// Tests that the shape of <see cref="IsometricRectangle.border"/> is just the combination of <see cref="IsometricRectangle.lowerBorder"/> and
        /// <see cref="IsometricRectangle.upperBorder"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void borderIsConcatenationOfLowerAndUpperBorder()
        {
            foreach (IsometricRectangle rectangle in testCases)
            {
                ShapeAssert.SameGeometry(Path.Concat(rectangle.lowerBorder, rectangle.upperBorder), rectangle.border, $"Failed with {rectangle}.");
            }
        }

        /// <summary>
        /// Tests that <see cref="IsometricRectangle.lowerBorder"/> is indeed the lower edges of <see cref="IsometricRectangle.border"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void lowerBorderIsLowerPartOfBorder()
        {
            foreach (IsometricRectangle rectangle in testCases)
            {
                IEnumerable<IntVector2> expected = rectangle.Where(p => p.y == rectangle.border.Where(q => q.x == p.x).Min(q => q.y));
                ShapeAssert.SameGeometry(expected, rectangle.lowerBorder, $"Failed with {rectangle}.");
            }
        }

        /// <summary>
        /// Tests that <see cref="IsometricRectangle.upperBorder"/> is indeed the upper edges of <see cref="IsometricRectangle.border"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void upperBorderIsUpperPartOfBorder()
        {
            foreach (IsometricRectangle rectangle in testCases)
            {
                IEnumerable<IntVector2> expected = rectangle.Where(p => p.y == rectangle.border.Where(q => q.x == p.x).Max(q => q.y));
                ShapeAssert.SameGeometry(expected, rectangle.upperBorder, $"Failed with {rectangle}.");
            }
        }

        /// <summary>
        /// Tests <see cref="IsometricRectangle.isIsometricSquare"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void isIsometricSquare()
        {
            foreach (IsometricRectangle rectangle in testCases)
            {
                if (rectangle.isIsometricSquare)
                {
                    ShapeAssert.ReflectiveSymmetry(rectangle, FlipAxis.Horizontal);
                    ShapeAssert.ReflectiveSymmetry(rectangle, FlipAxis.Vertical);
                }
                else
                {
                    ShapeAssert.ReflectiveAsymmetry(rectangle, FlipAxis.Vertical);
                    // An isometric rectangle can have symmetry across the horizontal axis without being an isometric square
                }
            }
        }

        /// <summary>
        /// Tests that the <see cref="IsometricRectangle"/> enumerator doesn't repeat any points.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void NoRepeats()
        {
            foreach (IsometricRectangle rectangle in testCases)
            {
                ShapeAssert.NoRepeats(rectangle);
            }
        }
    }
}
