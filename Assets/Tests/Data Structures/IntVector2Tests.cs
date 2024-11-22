using NUnit.Framework;
using PAC.DataStructures;

namespace PAC.Tests
{
    public class IntVector2Tests
    {
        [Test]
        [Category("Data Structures")]
        public void Simplify()
        {
            (IntVector2 input, IntVector2 expected)[] testCases =
            {
                (new IntVector2(3, 9), new IntVector2(1, 3)),
                (new IntVector2(4, 6), new IntVector2(2, 3)),
                (new IntVector2(12, 25), new IntVector2(12, 25)),
                (new IntVector2(18, 75), new IntVector2(6, 25))
            };

            foreach ((IntVector2 input, IntVector2 expected) in testCases)
            {
                Assert.AreEqual(expected, IntVector2.Simplify(input), "Failed with " + input);
                Assert.AreEqual(-expected, IntVector2.Simplify(-input), "Failed with " + input);
                Assert.AreEqual(expected.Flip(FlipAxis.Vertical), IntVector2.Simplify(input.Flip(FlipAxis.Vertical)), "Failed with " + input);
                Assert.AreEqual(expected.Flip(FlipAxis.Horizontal), IntVector2.Simplify(input.Flip(FlipAxis.Horizontal)), "Failed with " + input);

                Assert.AreEqual(1, Functions.Gcd(expected.x, expected.y), "Failed with " + input);
            }
        }
    }
}
