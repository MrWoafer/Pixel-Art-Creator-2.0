using NUnit.Framework;

using PAC.DataStructures;
using PAC.Extensions;
using PAC.Shapes;
using PAC.Tests.Shapes.DefaultTests;
using PAC.Tests.Shapes.RequiredTests;
using PAC.Tests.Shapes.TestUtils;

using System;
using System.Collections.Generic;
using System.Linq;

namespace PAC.Tests.Shapes
{
    /// <summary>
    /// Tests for <see cref="Rectangle"/>.
    /// </summary>
    public class Rectangle_Tests : I2DShape_DefaultTests<Rectangle>, I2DShape_RequiredTests
    {
        protected override IEnumerable<Rectangle> testCases => Enumerable.Concat(exampleTestCases, randomTestCases);
        private IEnumerable<Rectangle> exampleTestCases
        {
            get
            {
                foreach (bool filled in new bool[] { false, true })
                {
                    for (int x = 0; x <= 5; x++)
                    {
                        for (int y = 0; y <= 5; y++)
                        {
                            yield return new Rectangle(new IntRect((0, 0), (x, y)), filled);
                        }
                    }
                }
            }
        }
        private IEnumerable<Rectangle> randomTestCases
        {
            get
            {
                Random random = new Random(0);
                IntRect testRegion = new IntRect((-20, -20), (20, 20));
                for (int i = 0; i < 1_000; i++)
                {
                    yield return new Rectangle(testRegion.RandomSubRect(random), random.NextBool());
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
                    ShapeAssert.SameGeometry(new IntVector2[] { point }, new Rectangle(new IntRect(point, point), filled), $"Failed with {point} {(filled ? "filled" : "unfilled")}.");
                }
            }
        }

        /// <summary>
        /// Tests that some example filled <see cref="Rectangle"/>s have the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapeExamples_Filled()
        {
            foreach (IntVector2 bottomLeft in new IntRect((-5, -5), (5, 5)))
            {
                foreach (IntVector2 topRight in bottomLeft + new IntRect((0, 0), (5, 5)))
                {
                    Rectangle rectangle = new Rectangle(new IntRect(bottomLeft, topRight), true);
                    IntRect expected = new IntRect(bottomLeft, topRight);
                    ShapeAssert.SameGeometry(expected, rectangle, $"Failed with {rectangle}.");
                }
            }
        }

        /// <summary>
        /// Tests that the <see cref="Rectangle"/> enumerator doesn't repeat any points.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void NoRepeats()
        {
            foreach (Rectangle rectangle in testCases)
            {
                ShapeAssert.NoRepeats(rectangle);
            }
        }
    }
}
