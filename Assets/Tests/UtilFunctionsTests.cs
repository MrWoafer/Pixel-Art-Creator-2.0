using System.Collections;
using NUnit.Framework;

namespace PAC.Tests
{
    public class UtilFunctionsTests
    {
        [Test]
        [Category("Utils")]
        public void Gcd()
        {
            // (a, b, gcd) - make sure all are non-negative
            (int, int, int)[] testCases =
            {
                (3, 4, 1),
                (5, 11, 1),
                (4, 6, 2),
                (42, 54, 6),
                (0, 0, 0),
                (0, 3, 3),
                (5, 0, 5)
            };

            foreach ((int a, int b, int expected) in testCases)
            {
                foreach ((int signA, int signB) in new (int, int)[] { (1, 1), (-1, 1), (1, -1), (-1, -1) })
                {
                    Assert.AreEqual(expected, Functions.Gcd(signA * a, signB * b));
                    Assert.AreEqual(expected, Functions.Gcd(signB * b, signA * a));
                }
            }
        }

        [Test]
        [Category("Utils")]
        public void Lcm()
        {
            // (a, b, lcm) - make sure all are non-negative
            (int, int, int)[] testCases =
            {
                (1, 3, 3),
                (4, 2, 4),
                (7, 7, 7),
                (3, 4, 12),
                (5, 11, 55),
                (4, 6, 12),
                (42, 54, 378),
                (0, 0, 0),
                (0, 3, 0),
                (5, 0, 0)
            };

            foreach ((int a, int b, int expected) in testCases)
            {
                foreach ((int signA, int signB) in new (int, int)[] { (1, 1), (-1, 1), (1, -1), (-1, -1) })
                {
                    Assert.AreEqual(expected, Functions.Lcm(signA * a, signB * b));
                    Assert.AreEqual(expected, Functions.Lcm(signB * b, signA * a));
                }
            }
        }
    }
}
