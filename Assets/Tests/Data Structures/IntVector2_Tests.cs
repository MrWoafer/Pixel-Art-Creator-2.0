using NUnit.Framework;

using PAC.DataStructures;
using PAC.Geometry.Axes;
using PAC.Maths;

namespace PAC.Tests.DataStructures
{
    /// <summary>
    /// Tests for <see cref="IntVector2"/>.
    /// </summary>
    public class IntVector2_Tests
    {
        [Test]
        [Category("Data Structures")]
        public void Simplify()
        {
            (IntVector2 input, IntVector2 expected)[] testCases =
            {
                ((3, 9), (1, 3)),
                ((4, 6), (2, 3)),
                ((12, 25), (12, 25)),
                ((18, 75), (6, 25))
            };

            foreach ((IntVector2 input, IntVector2 expected) in testCases)
            {
                Assert.AreEqual(expected, IntVector2.Simplify(input), "Failed with " + input);
                Assert.AreEqual(-expected, IntVector2.Simplify(-input), "Failed with " + input);
                Assert.AreEqual(expected.Reflect(Axes.Vertical), IntVector2.Simplify(input.Reflect(Axes.Vertical)), "Failed with " + input);
                Assert.AreEqual(expected.Reflect(Axes.Horizontal), IntVector2.Simplify(input.Reflect(Axes.Horizontal)), "Failed with " + input);

                Assert.AreEqual(1, MathExtensions.Gcd(expected.x, expected.y), "Failed with " + input);
            }
        }
    }
}
