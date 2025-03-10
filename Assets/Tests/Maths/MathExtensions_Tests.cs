using System;

using NUnit.Framework;

using PAC.Maths;

namespace PAC.Tests.Maths
{
    /// <summary>
    /// Tests for <see cref="MathExtensions"/>.
    /// </summary>
    public class MathExtensions_Tests
    {
        [Test]
        [Category("Utils")]
        public void Mod()
        {
            for (int a = -50; a <= 50; a++)
            {
                if (a != 0)
                {
                    Assert.AreEqual(0, MathExtensions.Mod(a, a), $"Failed with {a}.");
                }

                for (int b = -50; b <= 50; b++)
                {
                    if (b == 0)
                    {
                        Assert.Throws<DivideByZeroException>(() => MathExtensions.Mod(a, b), $"Failed with {a}, {b}.");
                        continue;
                    }

                    int mod = MathExtensions.Mod(a, b);
                    Assert.True(mod >= 0, $"Failed with {a}, {b}. mod = {mod}.");
                    Assert.True(mod < Math.Abs(b), $"Failed with {a}, {b}. mod = {mod}.");
                    Assert.AreEqual(0, (a - mod) % b, $"Failed with {a}, {b}. mod = {mod}.");
                }
            }
        }

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
