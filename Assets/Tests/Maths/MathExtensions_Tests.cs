using System;

using NUnit.Framework;

using PAC.Maths;

namespace PAC.Tests
{
    /// <summary>
    /// Tests for <see cref="MathExtensions"/>.
    /// </summary>
    public class MathExtensions_Tests
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
                    Assert.AreEqual(expected, MathExtensions.Gcd(signA * a, signB * b));
                    Assert.AreEqual(expected, MathExtensions.Gcd(signB * b, signA * a));
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
                    Assert.AreEqual(expected, MathExtensions.Lcm(signA * a, signB * b));
                    Assert.AreEqual(expected, MathExtensions.Lcm(signB * b, signA * a));
                }
            }
        }

        [Test]
        [Category("Utils")]
        public void Pow()
        {
            Assert.AreEqual(1, MathExtensions.Pow(0, 0), "Failed with 0 ^ 0.");
            for (int exponent = 1; exponent <= 10; exponent++)
            {
                Assert.AreEqual(0, MathExtensions.Pow(0, exponent), $"Failed with 0 ^ {exponent}.");
            }

            for (int n = -10; n <= 10; n++)
            {
                string baseStr = (n < 0) ? "(" + n + ")" : n.ToString();

                Assert.AreEqual(1, MathExtensions.Pow(n, 0), $"Failed with {baseStr} ^ 0.");
                for (int exponent = 1; exponent <= 7; exponent++)
                {
                    int expected = 1;
                    for (int i = 0; i < exponent; i++)
                    {
                        expected *= n;
                    }

                    try
                    {
                        Assert.AreEqual(expected, MathExtensions.Pow(n, exponent), $"Failed with {baseStr} ^ {exponent}.");
                    }
                    catch (OverflowException)
                    {
                        Assert.Fail($"Overflow when computing {baseStr} ^ {exponent}.");
                    }
                }
            }

            Assert.Throws<OverflowException>(() => MathExtensions.Pow(10, 100));
            Assert.Throws<OverflowException>(() => MathExtensions.Pow(-10, 99));

            Assert.DoesNotThrow(() => MathExtensions.Pow(int.MinValue, 0));
            Assert.DoesNotThrow(() => MathExtensions.Pow(int.MaxValue, 0));
            Assert.DoesNotThrow(() => MathExtensions.Pow(int.MinValue, 1));
            Assert.DoesNotThrow(() => MathExtensions.Pow(int.MaxValue, 1));
            Assert.Throws<OverflowException>(() => MathExtensions.Pow(int.MinValue, 2));
            Assert.Throws<OverflowException>(() => MathExtensions.Pow(int.MaxValue, 2));
        }
    }
}
