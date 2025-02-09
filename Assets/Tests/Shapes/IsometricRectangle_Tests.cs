using NUnit.Framework;

using PAC.DataStructures;
using PAC.Shapes;
using PAC.Tests.Shapes.DefaultTests;
using PAC.Tests.Shapes.RequiredTests;
using PAC.Tests.Shapes.TestUtils;

using System;
using System.Collections.Generic;
using System.Linq;

namespace PAC.Tests.Shapes
{
    public class IsometricRectangle_Tests : IIsometricShape_DefaultTests<IsometricRectangle>, IIsometricShape_RequiredTests
    {
        protected override IEnumerable<IsometricRectangle> testCases => exampleTestCases.Concat(randomTestCases);
        private IEnumerable<IsometricRectangle> exampleTestCases => new IsometricRectangle[]
        {
            new IsometricRectangle(IntVector2.zero, IntVector2.zero, false),
            new IsometricRectangle(IntVector2.zero, IntVector2.zero, true),
            new IsometricRectangle(IntVector2.zero, IntVector2.right, false),
            new IsometricRectangle(IntVector2.zero, IntVector2.right, true),
            new IsometricRectangle(IntVector2.zero, IntVector2.left, false),
            new IsometricRectangle(IntVector2.zero, IntVector2.left, true),
            new IsometricRectangle(IntVector2.zero, IntVector2.up, false),
            new IsometricRectangle(IntVector2.zero, IntVector2.up, true),
            new IsometricRectangle(IntVector2.zero, IntVector2.down, false),
            new IsometricRectangle(IntVector2.zero, IntVector2.down, true)
        };
        private IEnumerable<IsometricRectangle> randomTestCases
        {
            get
            {
                Random random = new Random(0);
                const int numTestCases = 1_000;
                for (int i = 0; i < numTestCases; i++)
                {
                    IntVector2 start = new IntVector2(random.Next(-20, 21), random.Next(-20, 21));
                    IntVector2 end = start + new IntVector2(random.Next(-20, 21), random.Next(-20, 21));
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
                foreach (IntVector2 pixel in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
                {
                    CollectionAssert.AreEquivalent(new IntVector2[] { pixel }, new IsometricRectangle(pixel, pixel, filled), $"Failed with {pixel} {(filled ? "filled" : "unfilled")}.");
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void ShapeExample()
        {
            IsometricRectangle rectangle = new IsometricRectangle((0, 0), (11, -1), false);
            IntVector2[] expected = new IntVector2[]
            {
                (0, 0), (1, 0), (2, -1), (3, -1), (4, -2), (5, -2), (6, -3), (7, -3), (8, -2), (9, -2), (10, -1), (11, -1),
                (10, -1), (9, 0), (8, 0), (7, 1), (6, 1), (5, 2), (4, 2), (3, 1), (2, 1), (1, 0), (0, 0)
            };

            ShapeAssert.SameGeometry(expected, rectangle);
        }

        /// <summary>
        /// Tests that the topCorner, bottomCorner, leftCorner and rightCorner properties are indeed corners of the shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void CornersAreCorners()
        {
            foreach (IsometricRectangle rectangle in testCases)
            {
                Assert.AreEqual(rectangle.boundingRect.minX, rectangle.leftCorner.x, $"Failed with {rectangle}.");
                Assert.AreEqual(rectangle.boundingRect.maxX, rectangle.rightCorner.x, $"Failed with {rectangle}.");
                Assert.AreEqual(rectangle.boundingRect.minY, rectangle.bottomCorner.y, $"Failed with {rectangle}.");
                Assert.AreEqual(rectangle.boundingRect.maxY, rectangle.topCorner.y, $"Failed with {rectangle}.");
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

                Assert.True(leftRightTopBottom.Contains(rectangle.startCorner), $"Failed with {rectangle}.");
                Assert.True(leftRightTopBottom.Contains(rectangle.endCorner), $"Failed with {rectangle}.");
            }
        }

        /// <summary>
        /// Tests that the shape of the border is just the combination of the lower border and upper border.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void BorderIsCombinationOfLowerAndUpperBorder()
        {
            foreach (IsometricRectangle rectangle in testCases)
            {
                CollectionAssert.AreEquivalent(Path.Concat(rectangle.lowerBorder, rectangle.upperBorder).ToHashSet(), rectangle.border.ToHashSet(), $"Failed with {rectangle}.");
            }
        }

        /// <summary>
        /// Tests that the lowerBorder property is indeed the lower edges of the border.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void LowerBorderIsLowerPartOfBorder()
        {
            foreach (IsometricRectangle rectangle in testCases)
            {
                CollectionAssert.AreEquivalent(rectangle.Where(p => p.y == rectangle.border.MinY(p.x)).ToHashSet(), rectangle.lowerBorder.ToHashSet(), $"Failed with {rectangle}.");
            }
        }

        /// <summary>
        /// Tests that the upperBorder property is indeed the upper edges of the border.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void UpperBorderIsUpperPartOfBorder()
        {
            foreach (IsometricRectangle rectangle in testCases)
            {
                CollectionAssert.AreEquivalent(rectangle.Where(p => p.y == rectangle.border.MaxY(p.x)).ToHashSet(), rectangle.upperBorder.ToHashSet(), $"Failed with {rectangle}.");
            }
        }

        [Test]
        [Category("Shapes")]
        public void IsIsometricSquare()
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
