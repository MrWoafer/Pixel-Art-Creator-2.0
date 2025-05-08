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
        [Category("Maths")]
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
        [Category("Maths")]
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
        [Category("Maths")]
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

        /// <summary>
        /// Tests <see cref="MathExtensions.FractionalPart(float)"/>.
        /// </summary>
        [Test]
        [Category("Maths")]
        public void FractionalPart()
        {
            Assert.AreEqual(0f, MathExtensions.FractionalPart(0f));
            Assert.AreEqual(0.3f, MathExtensions.FractionalPart(0.3f), 0.00001f);
            Assert.AreEqual(0.62f, MathExtensions.FractionalPart(5.62f), 0.00001f);

            Assert.AreEqual(-0.14f, MathExtensions.FractionalPart(-0.14f), 0.00001f);
            Assert.AreEqual(-0.167f, MathExtensions.FractionalPart(-12.167f), 0.00001f);

            for (int n = -10; n <= 10; n++)
            {
                Assert.AreEqual(0f, MathExtensions.FractionalPart(n));
            }
        }

        /// <summary>
        /// Tests <see cref="MathExtensions.RoundHalfUp(float)"/>.
        /// </summary>
        [Test]
        [Category("Maths")]
        public void RoundHalfUp()
        {
            Assert.AreEqual(0f, MathExtensions.RoundHalfUp(0.1f));
            Assert.AreEqual(0f, MathExtensions.RoundHalfUp(0.49f));
            Assert.AreEqual(1f, MathExtensions.RoundHalfUp(0.51f));

            Assert.AreEqual(4f, MathExtensions.RoundHalfUp(3.64f));
            Assert.AreEqual(10f, MathExtensions.RoundHalfUp(10.3f));
            Assert.AreEqual(-5f, MathExtensions.RoundHalfUp(-5.3f));
            Assert.AreEqual(-8f, MathExtensions.RoundHalfUp(-7.8f));

            Assert.AreEqual(-3f, MathExtensions.RoundHalfUp(-3.5f));
            Assert.AreEqual(-2f, MathExtensions.RoundHalfUp(-2.5f));
            Assert.AreEqual(-1f, MathExtensions.RoundHalfUp(-1.5f));
            Assert.AreEqual(0f, MathExtensions.RoundHalfUp(-0.5f));
            Assert.AreEqual(1f, MathExtensions.RoundHalfUp(0.5f));
            Assert.AreEqual(2f, MathExtensions.RoundHalfUp(1.5f));
            Assert.AreEqual(3f, MathExtensions.RoundHalfUp(2.5f));
            Assert.AreEqual(4f, MathExtensions.RoundHalfUp(3.5f));

            for (int n = -10; n <= 10; n++)
            {
                Assert.AreEqual(n, MathExtensions.RoundHalfUp(n));
            }
        }

        /// <summary>
        /// Tests <see cref="MathExtensions.RoundHalfUp(float, float)"/>.
        /// </summary>
        [Test]
        [Category("Maths")]
        public void RoundHalfUp_ToMultiple()
        {
            Assert.AreEqual(0f, MathExtensions.RoundHalfUp(0.3f, 2f));
            Assert.AreEqual(0f, MathExtensions.RoundHalfUp(0.6f, 2f));
            Assert.AreEqual(2f, MathExtensions.RoundHalfUp(1f, 2f));
            Assert.AreEqual(2f, MathExtensions.RoundHalfUp(1.7f, 2f));

            Assert.AreEqual(5f, MathExtensions.RoundHalfUp(4.2f, -5f));
            Assert.AreEqual(-5f, MathExtensions.RoundHalfUp(-4.2f, -5f));
            Assert.AreEqual(0f, MathExtensions.RoundHalfUp(-2.5f, -5f));
            Assert.AreEqual(5f, MathExtensions.RoundHalfUp(2.5f, -5f));
        }
    }
}
