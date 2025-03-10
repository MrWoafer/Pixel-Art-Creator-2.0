using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using PAC.DataStructures;
using PAC.Geometry.Shapes;
using PAC.Tests.Geometry.Shapes.DefaultTests;
using PAC.Tests.Geometry.Shapes.RequiredTests;
using PAC.Tests.Geometry.Shapes.TestUtils;

namespace PAC.Tests.Geometry.Shapes
{
    /// <summary>
    /// Tests <see cref="QuadraticBezier"/>.
    /// </summary>
    public class QuadraticBezier_Tests : I1DShape_DefaultTests<QuadraticBezier>, I1DShape_RequiredTests
    {
        protected override IEnumerable<QuadraticBezier> testCases
        {
            get
            {
                Random random = new Random(0);
                IntRect testRegion = new IntRect((-20, -20), (20, 20));
                for (int i = 0; i < 1_000; i++)
                {
                    yield return new QuadraticBezier(testRegion.RandomPoint(random), testRegion.RandomPoint(random), testRegion.RandomPoint(random));
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public override void ShapeSinglePoint()
        {
            foreach (IntVector2 point in new IntRect((-5, -5), (5, 5)))
            {
                ShapeAssert.SameGeometry(new IntVector2[] { point }, new QuadraticBezier(point, point, point), $"Failed with {point}.");
            }
        }

        [Test]
        [Category("Shapes")]
        public void NoRepeats()
        {
            foreach (QuadraticBezier line in testCases)
            {
                ShapeAssert.NoRepeats(line);
            }
        }

        /// <summary>
        /// Tests that the <see cref="QuadraticBezier"/> enumerator starts at <see cref="QuadraticBezier.start"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void EnumeratorStartsAtStart()
        {
            foreach (QuadraticBezier line in testCases)
            {
                Assert.AreEqual(Enumerable.First(line), line.start, $"Failed with {line}.");
            }
        }
        /// <summary>
        /// Tests that the <see cref="QuadraticBezier"/> enumerator ends at <see cref="QuadraticBezier.end"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void EnumeratorEndsAtEnd()
        {
            foreach (QuadraticBezier line in testCases)
            {
                Assert.AreEqual(Enumerable.Last(line), line.end, $"Failed with {line}.");
            }
        }

        /// <summary>
        /// Tests <see cref="QuadraticBezier.reverse"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void reverse()
        {
            foreach (QuadraticBezier line in testCases)
            {
                ShapeAssert.SameGeometry(Enumerable.Reverse(line), line.reverse, $"Failed with {line}.");
            }
        }
    }
}
