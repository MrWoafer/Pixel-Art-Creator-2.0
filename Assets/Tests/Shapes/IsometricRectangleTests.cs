using NUnit.Framework;

using PAC.DataStructures;
using PAC.Shapes;

using System;
using System.Collections.Generic;
using System.Linq;

namespace PAC.Tests
{
    public class IsometricRectangleTests : IIsometricShapeTests<IsometricRectangle>
    {
        public override IEnumerable<IsometricRectangle> testCases => exampleTestCases.Concat(randomTestCases);
        public IEnumerable<IsometricRectangle> exampleTestCases => new IsometricRectangle[]
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
        public IEnumerable<IsometricRectangle> randomTestCases
        {
            get
            {
                Random rng = new Random(0);
                const int numTestCases = 1_000;
                for (int i = 0; i < numTestCases; i++)
                {
                    IntVector2 start = new IntVector2(rng.Next(-20, 21), rng.Next(-20, 21));
                    IntVector2 end = start + new IntVector2(rng.Next(-20, 21), rng.Next(-20, 21));
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
                    CollectionAssert.AreEquivalent(new IntVector2[] { pixel }, new IsometricRectangle(pixel, pixel, filled), "Failed with " + pixel + " " + (filled ? "filled" : "unfilled"));
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
            foreach (IsometricRectangle rectangle in testCases)
            {
                Assert.AreEqual(rectangle.boundingRect.bottomLeft.x, rectangle.leftCorner.x, "Failed with " + rectangle);
                Assert.AreEqual(rectangle.boundingRect.topRight.x, rectangle.rightCorner.x, "Failed with " + rectangle);
                Assert.AreEqual(rectangle.boundingRect.bottomLeft.y, rectangle.bottomCorner.y, "Failed with " + rectangle);
                Assert.AreEqual(rectangle.boundingRect.topRight.y, rectangle.topCorner.y, "Failed with " + rectangle);
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
                CollectionAssert.AreEquivalent(Path.Concat(rectangle.lowerBorder, rectangle.upperBorder).ToHashSet(), rectangle.border.ToHashSet(), "Failed with " + rectangle);
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
                CollectionAssert.AreEquivalent(rectangle.Where(p => p.y == rectangle.border.MinY(p.x)).ToHashSet(), rectangle.lowerBorder.ToHashSet(), "Failed with " + rectangle);
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
                CollectionAssert.AreEquivalent(rectangle.Where(p => p.y == rectangle.border.MaxY(p.x)).ToHashSet(), rectangle.upperBorder.ToHashSet(), "Failed with " + rectangle);
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
