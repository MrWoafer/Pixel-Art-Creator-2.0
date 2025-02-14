using System;
using System.Collections.Generic;

using NUnit.Framework;

using PAC.Maths;

namespace PAC.Tests.DataStructures
{
    /// <summary>
    /// Tests for <see cref="Rational"/>.
    /// </summary>
    public class Rational_Tests
    {
        /// <summary>
        /// Generates <paramref name="numTestCases"/> random <see cref="Rational"/>s.
        /// </summary>
        /// <remarks>
        /// None of the generated <see cref="Rational"/>s will be <see cref="Rational.Undefined"/>.
        /// </remarks>
        private IEnumerable<Rational> RandomTestCases(int numTestCases)
        {
            Random random = new Random(0);
            for (int i = 0; i < numTestCases; i++)
            {
                int numerator = random.Next(-100, 101);
                int denominator = random.Next(-100, 101);
                while (denominator == 0)
                {
                    denominator = random.Next(-100, 101);
                }

                yield return new Rational(numerator, denominator);
            }
        }

        [Test]
        [Category("Data Structures")]
        public void SignOfNumeratorAndDenominator()
        {
            Assert.AreEqual(3, new Rational(3, 2).numerator);
            Assert.AreEqual(2, new Rational(3, 2).denominator);

            Assert.AreEqual(-3, new Rational(-3, 2).numerator);
            Assert.AreEqual(2, new Rational(-3, 2).denominator);

            Assert.AreEqual(-3, new Rational(3, -2).numerator);
            Assert.AreEqual(2, new Rational(3, -2).denominator);

            Assert.AreEqual(3, new Rational(-3, -2).numerator);
            Assert.AreEqual(2, new Rational(-3, -2).denominator);

            Random random = new Random(0);
            for (int i = 0; i < 1_000; i++)
            {
                int numerator = random.Next(-100, 101);
                int denominator = random.Next(-100, 101);
                Rational rational = new Rational(numerator, denominator);

                Assert.True(rational.denominator >= 0, $"Failed with {numerator} and {denominator}.");
                Assert.AreEqual(Math.Sign(numerator) * Math.Sign(denominator), Math.Sign(rational.numerator), $"Failed with {numerator} and {denominator}.");
            }
        }

        [Test]
        [Category("Data Structures")]
        public void NumeratorAndDenominatorSpecialCases()
        {
            Random random = new Random(0);
            for (int i = 0; i < 1_000; i++)
            {
                int numerator = random.Next(-100, 101);
                Rational rational = new Rational(numerator, 0);

                Assert.AreEqual(0, rational.numerator, $"Failed with {numerator}.");
                Assert.AreEqual(0, rational.denominator, $"Failed with {numerator}.");
            }
            for (int i = 0; i < 1_000; i++)
            {
                int denominator = random.Next(-100, 101);
                Rational rational = new Rational(0, denominator);

                if (denominator == 0)
                {
                    Assert.AreEqual(0, rational.numerator, $"Failed with {denominator}.");
                    Assert.AreEqual(0, rational.denominator, $"Failed with {denominator}.");
                }
                else
                {
                    Assert.AreEqual(0, rational.numerator, $"Failed with {denominator}.");
                    Assert.AreEqual(1, rational.denominator, $"Failed with {denominator}.");
                }
            }
        }

        [Test]
        [Category("Data Structures")]
        public void KeptInSimplestForm()
        {
            Assert.AreEqual(new Rational(3, 2), new Rational(6, 4));

            Random random = new Random(0);
            for (int i = 0; i < 1_000; i++)
            {
                int numerator = random.Next(-100, 101);
                int denominator = random.Next(-100, 101);
                Rational rational = new Rational(numerator, denominator);
                
                if (denominator == 0)
                {
                    Assert.AreEqual(0, rational.numerator, $"Failed with {denominator}.");
                    Assert.AreEqual(0, rational.denominator, $"Failed with {denominator}.");
                }
                else if (numerator == 0)
                {
                    Assert.AreEqual(0, rational.numerator, $"Failed with {denominator}.");
                    Assert.AreEqual(1, rational.denominator, $"Failed with {denominator}.");
                }
                else
                {
                    Assert.AreEqual(1, MathExtensions.Gcd(rational.numerator, rational.denominator), $"Failed with {numerator} and {denominator}.");
                }
            }
        }

        /// <summary>
        /// Tests <see cref="Rational.isInteger"/>.
        /// </summary>
        [Test]
        [Category("Data Structures")]
        public void isInteger()
        {
            Assert.False(Rational.Undefined.isInteger);

            for (int n = -10; n <= 10; n++)
            {
                Rational cast = n;
                Assert.True(cast.isInteger, $"Failed with {n}.");
            }

            foreach (Rational a in RandomTestCases(1_000))
            {
                Assert.AreEqual(a.denominator == 1, a.isInteger, $"Failed with {a}.");
                Assert.AreEqual((int)a == a, a.isInteger, $"Failed with {a}.");
            }
        }

        /// <summary>
        /// Tests <see cref="Rational.isUndefined"/>.
        /// </summary>
        [Test]
        [Category("Data Structures")]
        public void isUndefined()
        {
            Assert.True(Rational.Undefined.isUndefined);

            foreach (Rational a in RandomTestCases(1_000))
            {
                Assert.False(a.isUndefined, $"Failed with {a}.");
            }
        }

        /// <summary>
        /// Tests <see cref="Rational.sign"/>.
        /// </summary>
        [Test]
        [Category("Data Structures")]
        public void sign()
        {
            Assert.AreEqual(0, Rational.Undefined.sign);

            foreach (Rational a in RandomTestCases(1_000))
            {
                Assert.AreEqual(Math.Sign(a.numerator), a.sign, $"Failed with {a}.");
                Assert.AreEqual(Math.Sign(a.numerator) * Math.Sign(a.denominator), a.sign, $"Failed with {a}.");

                if (a == 0)
                {
                    Assert.AreEqual(0, a.sign, $"Failed with {a}.");
                }
                else if (a > 0)
                {
                    Assert.AreEqual(1, a.sign, $"Failed with {a}.");
                }
                else
                {
                    Assert.AreEqual(-1, a.sign, $"Failed with {a}.");
                }
            }
        }

        /// <summary>
        /// Tests <see cref="Rational.implicit operator Rational(int)"/>.
        /// </summary>
        [Test]
        [Category("Data Structures")]
        public void CastFromInt()
        {
            for (int n = -10; n <= 10; n++)
            {
                Rational cast = n;

                Assert.AreEqual(n, cast.numerator, $"Failed with {n}.");
                Assert.AreEqual(1, cast.denominator, $"Failed with {n}.");
                Assert.AreEqual(n, cast, $"Failed with {n}.");
            }
        }

        /// <summary>
        /// Tests <see cref="Rational.explicit operator int(Rational)"/>.
        /// </summary>
        [Test]
        [Category("Data Structures")]
        public void CastToInt()
        {
            Random random = new Random(0);
            for (int i = 0; i < 1_000; i++)
            {
                int numerator = random.Next(-100, 101);
                int denominator = random.Next(-100, 101);
                Rational rational = new Rational(numerator, denominator);

                if (rational.isUndefined)
                {
                    Assert.Throws<DivideByZeroException>(() => { var x = (int)rational; }, $"Failed with {numerator} and {denominator}.");
                }
                else
                {
                    Assert.AreEqual(numerator / denominator, (int)rational, $"Failed with {numerator} and {denominator}.");
                }
            }
        }

        /// <summary>
        /// Tests <see cref="Rational.implicit operator float(Rational)"/>.
        /// </summary>
        [Test]
        [Category("Data Structures")]
        public void CastToFloat()
        {
            Assert.AreEqual(-5f, (float)new Rational(-5, 1));
            Assert.AreEqual(0.5f, (float)new Rational(1, 2));
            Assert.AreEqual(1.75f, (float)new Rational(7, 4));

            Random random = new Random(0);
            for (int i = 0; i < 1_000; i++)
            {
                int numerator = random.Next(-100, 101);
                int denominator = random.Next(-100, 101);
                Rational rational = new Rational(numerator, denominator);

                if (rational.isUndefined)
                {
                    Assert.AreEqual(float.NaN, (float)rational, $"Failed with {numerator} and {denominator}.");
                }
                else
                {
                    Assert.AreEqual(numerator / (float)denominator, (float)rational, 0.0001f, $"Failed with {numerator} and {denominator}.");
                }
            }
        }

        /// <summary>
        /// Tests <see cref="Rational.operator ==(Rational, Rational)"/>.
        /// </summary>
        [Test]
        [Category("Data Structures")]
        public void Equals()
        {
            Assert.AreNotEqual(Rational.Undefined, Rational.Undefined);

            foreach (Rational a in RandomTestCases(100))
            {
                Assert.AreEqual(a, a, $"Failed with {a}.");

                Assert.AreNotEqual(a, Rational.Undefined, $"Failed with {a}.");
                Assert.AreNotEqual(Rational.Undefined, a, $"Failed with {a}.");

                foreach (Rational b in RandomTestCases(100))
                {
                    Assert.AreEqual(a == b, b == a, $"Failed with {a} and {b}.");
                }
            }
        }

        /// <summary>
        /// Tests <see cref="Rational.operator {(Rational, Rational)"/>.
        /// </summary>
        [Test]
        [Category("Data Structures")]
        public void LessThan()
        {
            Assert.False(Rational.Undefined < Rational.Undefined);

            foreach (Rational a in RandomTestCases(100))
            {
                Assert.False(a < a, $"Failed with {a}.");

                Assert.False(a < Rational.Undefined, $"Failed with {a}.");
                Assert.False(Rational.Undefined < a, $"Failed with {a}.");

                foreach (Rational b in RandomTestCases(100))
                {
                    Assert.AreEqual((float)a < (float)b, a < b, $"Failed with {a} and {b}.");

                    Assert.AreEqual(b > a, a < b, $"Failed with {a} and {b}.");
                    Assert.AreEqual(a <= b && a != b, a < b, $"Failed with {a} and {b}.");
                }
            }
        }

        /// <summary>
        /// Tests <see cref="Rational.operator {=(Rational, Rational)"/>.
        /// </summary>
        [Test]
        [Category("Data Structures")]
        public void LessThanOrEqual()
        {
            Assert.False(Rational.Undefined <= Rational.Undefined);

            foreach (Rational a in RandomTestCases(100))
            {
                Assert.True(a <= a, $"Failed with {a}.");

                Assert.False(a <= Rational.Undefined, $"Failed with {a}.");
                Assert.False(Rational.Undefined <= a, $"Failed with {a}.");

                foreach (Rational b in RandomTestCases(100))
                {
                    Assert.AreEqual((float)a <= (float)b, a <= b, $"Failed with {a} and {b}.");

                    Assert.AreEqual(b >= a, a <= b, $"Failed with {a} and {b}.");
                    Assert.AreEqual(a < b || a == b, a <= b, $"Failed with {a} and {b}.");
                }
            }
        }

        /// <summary>
        /// Tests <see cref="Rational.operator }(Rational, Rational)"/>.
        /// </summary>
        [Test]
        [Category("Data Structures")]
        public void MoreThan()
        {
            Assert.False(Rational.Undefined > Rational.Undefined);

            foreach (Rational a in RandomTestCases(100))
            {
                Assert.False(a > a, $"Failed with {a}.");

                Assert.False(a > Rational.Undefined, $"Failed with {a}.");
                Assert.False(Rational.Undefined > a, $"Failed with {a}.");

                foreach (Rational b in RandomTestCases(100))
                {
                    Assert.AreEqual((float)a > (float)b, a > b, $"Failed with {a} and {b}.");

                    Assert.AreEqual(b < a, a > b, $"Failed with {a} and {b}.");
                    Assert.AreEqual(a >= b && a != b, a > b, $"Failed with {a} and {b}.");
                }
            }
        }

        /// <summary>
        /// Tests <see cref="Rational.operator }=(Rational, Rational)"/>.
        /// </summary>
        [Test]
        [Category("Data Structures")]
        public void MoreThanOrEqual()
        {
            Assert.False(Rational.Undefined >= Rational.Undefined);

            foreach (Rational a in RandomTestCases(100))
            {
                Assert.True(a >= a, $"Failed with {a}.");

                Assert.False(a >= Rational.Undefined, $"Failed with {a}.");
                Assert.False(Rational.Undefined >= a, $"Failed with {a}.");

                foreach (Rational b in RandomTestCases(100))
                {
                    Assert.AreEqual((float)a >= (float)b, a >= b, $"Failed with {a} and {b}.");

                    Assert.AreEqual(b <= a, a >= b, $"Failed with {a} and {b}.");
                    Assert.AreEqual(a > b || a == b, a >= b, $"Failed with {a} and {b}.");
                }
            }
        }

        /// <summary>
        /// Tests <see cref="Rational.operator +(Rational, Rational)"/>.
        /// </summary>
        [Test]
        [Category("Data Structures")]
        public void Add()
        {
            Assert.AreEqual(new Rational(-7, 12), new Rational(2, 3) + new Rational(-5, 4));

            Assert.True((Rational.Undefined + Rational.Undefined).isUndefined);

            foreach (Rational a in RandomTestCases(100))
            {
                Assert.AreEqual(a, a + 0, $"Failed with {a}.");
                Assert.AreEqual(a, 0 + a, $"Failed with {a}.");

                Assert.True((a + Rational.Undefined).isUndefined, $"Failed with {a}.");
                Assert.True((Rational.Undefined + a).isUndefined, $"Failed with {a}.");

                foreach (Rational b in RandomTestCases(100))
                {
                    Assert.AreEqual((float)a + (float)b, (float)(a + b), 0.0001f, $"Failed with {a} and {b}.");

                    Assert.AreEqual(a + b, b + a, $"Failed with {a} and {b}.");
                }
            }
        }

        /// <summary>
        /// Tests <see cref="Rational.operator -(Rational, Rational)"/>.
        /// </summary>
        [Test]
        [Category("Data Structures")]
        public void Subtract()
        {
            Assert.AreEqual(new Rational(23, 12), new Rational(2, 3) - new Rational(-5, 4));

            Assert.True((Rational.Undefined - Rational.Undefined).isUndefined);

            foreach (Rational a in RandomTestCases(100))
            {
                Assert.AreEqual(a, a - 0, $"Failed with {a}.");
                Assert.AreEqual(-a, 0 - a, $"Failed with {a}.");

                Assert.True((a - Rational.Undefined).isUndefined, $"Failed with {a}.");
                Assert.True((Rational.Undefined - a).isUndefined, $"Failed with {a}.");

                foreach (Rational b in RandomTestCases(100))
                {
                    Assert.AreEqual((float)a - (float)b, (float)(a - b), 0.0001f, $"Failed with {a} and {b}.");

                    Assert.AreEqual(-(a - b), b - a, $"Failed with {a} and {b}.");
                }
            }
        }

        /// <summary>
        /// Tests <see cref="Rational.operator -(Rational)"/>.
        /// </summary>
        [Test]
        [Category("Data Structures")]
        public void Negate()
        {
            Assert.AreEqual(new Rational(-2, 3), -new Rational(2, 3));

            Assert.True((-Rational.Undefined).isUndefined);

            foreach (Rational a in RandomTestCases(100))
            {
                Assert.AreEqual(-(a.numerator), (-a).numerator, $"Failed with {a}.");
                Assert.AreEqual(a.denominator, (-a).denominator, $"Failed with {a}.");

                Assert.AreEqual(a, -(-a), $"Failed with {a}.");
            }
        }

        /// <summary>
        /// Tests <see cref="Rational.operator *(Rational, Rational)"/>.
        /// </summary>
        [Test]
        [Category("Data Structures")]
        public void Multiply()
        {
            Assert.AreEqual(new Rational(-5, 6), new Rational(2, 3) * new Rational(-5, 4));

            Assert.True((Rational.Undefined * Rational.Undefined).isUndefined);

            foreach (Rational a in RandomTestCases(100))
            {
                Assert.AreEqual(0, a * 0, $"Failed with {a}.");
                Assert.AreEqual(0, 0 * a, $"Failed with {a}.");
                Assert.AreEqual(a, a * 1, $"Failed with {a}.");
                Assert.AreEqual(a, 1 * a, $"Failed with {a}.");

                Assert.True((a * Rational.Undefined).isUndefined, $"Failed with {a}.");
                Assert.True((Rational.Undefined * a).isUndefined, $"Failed with {a}.");

                foreach (Rational b in RandomTestCases(100))
                {
                    Assert.AreEqual((float)a * (float)b, (float)(a * b), 0.0001f, $"Failed with {a} and {b}.");

                    Assert.AreEqual(a * b, b * a, $"Failed with {a} and {b}.");
                }
            }
        }

        /// <summary>
        /// Tests <see cref="Rational.operator /(Rational, Rational)"/>.
        /// </summary>
        [Test]
        [Category("Data Structures")]
        public void Divide()
        {
            Assert.AreEqual(new Rational(-8, 15), new Rational(2, 3) / new Rational(-5, 4));

            Assert.True((Rational.Undefined / Rational.Undefined).isUndefined);

            foreach (Rational a in RandomTestCases(100))
            {
                Assert.AreEqual(a, a / 1, $"Failed with {a}.");
                Assert.AreEqual(Math.Sign(a.numerator) * a.denominator, (1 / a).numerator, $"Failed with {a}.");
                Assert.AreEqual(Math.Abs(a.numerator), (1 / a).denominator, $"Failed with {a}.");

                Assert.AreEqual(0, 0 / a, $"Failed with {a}.");
                Assert.True((a / 0).isUndefined);

                Assert.True((a / Rational.Undefined).isUndefined, $"Failed with {a}.");
                Assert.True((Rational.Undefined / a).isUndefined, $"Failed with {a}.");

                foreach (Rational b in RandomTestCases(100))
                {
                    if (b == 0)
                    {
                        continue;
                    }

                    Assert.AreEqual((float)a / (float)b, (float)(a / b), 0.0001f, $"Failed with {a} and {b}.");

                    Assert.AreEqual(1 / (a / b), b / a, $"Failed with {a} and {b}.");
                }
            }
        }

        /// <summary>
        /// Tests <see cref="Rational.operator %(Rational, Rational)"/>.
        /// </summary>
        [Test]
        [Category("Data Structures")]
        public void Mod()
        {
            Assert.True((Rational.Undefined % Rational.Undefined).isUndefined);

            foreach (Rational a in RandomTestCases(100))
            {
                Assert.True((a % Rational.Undefined).isUndefined, $"Failed with {a}.");
                if (a != 0)
                {
                    Assert.True((Rational.Undefined % a).isUndefined, $"Failed with {a}.");
                }
                else
                {
                    Assert.Throws<DivideByZeroException>(() => { var r = Rational.Undefined % a; }, $"Failed with {a}.");
                }

                foreach (Rational b in RandomTestCases(100))
                {
                    if (b == 0)
                    {
                        Assert.Throws<DivideByZeroException>(() => { var r = a % b; }, $"Failed with {a} and {b}.");
                        continue;
                    }

                    Rational r = a % b;
                    Assert.True(r >= 0, $"Failed with {a} and {b}. r = {r}.");
                    Assert.True(r < Rational.Abs(b), $"Failed with {a} and {b}. r = {r}.");
                    Assert.AreEqual(0, (a - r) % b, $"Failed with {a} and {b}. r = {r}.");
                }
            }
        }

        /// <summary>
        /// Tests <see cref="Rational.Abs(Rational)"/>.
        /// </summary>
        [Test]
        [Category("Data Structures")]
        public void Abs()
        {
            Assert.True(Rational.Abs(Rational.Undefined).isUndefined);

            foreach (Rational a in RandomTestCases(100))
            {
                Rational abs = Rational.Abs(a);
                Assert.True(abs > 0, $"Failed with {a}. abs = {abs}.");

                if (a >= 0)
                {
                    Assert.AreEqual(a, abs, $"Failed with {a}. abs = {abs}.");
                }
                else
                {
                    Assert.AreEqual(-a, abs, $"Failed with {a}. abs = {abs}.");
                }
            }
        }
    }
}
