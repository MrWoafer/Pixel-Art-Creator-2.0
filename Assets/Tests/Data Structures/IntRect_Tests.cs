using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using PAC.DataStructures;

namespace PAC.Tests.DataStructures
{
    /// <summary>
    /// Tests for <see cref="IntRect"/>.
    /// </summary>
    public class IntRect_Tests
    {
        /// <summary>
        /// An infinite sequence of random <see cref="IntRect"/>s for tests.
        /// </summary>
        private IEnumerable<IntRect> randomTestCases
        {
            get
            {
                Random rng = new Random(0);
                const int minInclusive = -10;
                const int maxInclusive = 10;
                while (true)
                {
                    IntVector2 bottomLeft = new IntVector2(rng.Next(minInclusive, maxInclusive + 1), rng.Next(minInclusive, maxInclusive + 1));
                    IntVector2 topRight = new IntVector2(rng.Next(minInclusive, maxInclusive + 1), rng.Next(minInclusive, maxInclusive + 1));
                    yield return new IntRect(bottomLeft, topRight);
                }
            }
        }

        [Test]
        [Category("Data Structures")]
        public void minX()
        {
            foreach (IntRect rect in randomTestCases.Take(10_000))
            {
                Assert.AreEqual(rect.Min(p => p.x), rect.minX, $"Failed with {rect}");
            }
        }
        [Test]
        [Category("Data Structures")]
        public void maxX()
        {
            foreach (IntRect rect in randomTestCases.Take(10_000))
            {
                Assert.AreEqual(rect.Max(p => p.x), rect.maxX, $"Failed with {rect}");
            }
        }
        [Test]
        [Category("Data Structures")]
        public void minY()
        {
            foreach (IntRect rect in randomTestCases.Take(10_000))
            {
                Assert.AreEqual(rect.Min(p => p.y), rect.minY, $"Failed with {rect}");
            }
        }
        [Test]
        [Category("Data Structures")]
        public void maxY()
        {
            foreach (IntRect rect in randomTestCases.Take(10_000))
            {
                Assert.AreEqual(rect.Max(p => p.y), rect.maxY, $"Failed with {rect}");
            }
        }

        [Test]
        [Category("Data Structures")]
        public void BoundingRect()
        {
            // Bounding rect of IntVector2s

            (IntRect expected, IntVector2[] input)[] testCases =
            {
                (new IntRect(new IntVector2(1, -4), new IntVector2(1, -4)), new IntVector2[] {
                    new IntVector2(1, -4)
                }),
                (new IntRect(new IntVector2(3, 4), new IntVector2(5, 8)), new IntVector2[] {
                    new IntVector2(3, 4), new IntVector2(5, 8)
                }),
                (new IntRect(new IntVector2(2, 4), new IntVector2(9, 7)), new IntVector2[] {
                    new IntVector2(3, 4), new IntVector2(2, 6), new IntVector2(9, 7)
                }),
                (new IntRect(new IntVector2(2, 4), new IntVector2(9, 7)), new IntVector2[] {
                    new IntVector2(3, 4), new IntVector2(3, 4), new IntVector2(2, 6), new IntVector2(2, 6), new IntVector2(9, 7), new IntVector2(9, 7)
                }),
                (new IntRect(new IntVector2(-3, -2), new IntVector2(10, 7)), new IntVector2[] {
                    new IntVector2(-3, -1), new IntVector2(2, 6), new IntVector2(9, 7), new IntVector2(5, -2), new IntVector2(5, 5), new IntVector2(10, 1)
                })
            };

            foreach ((IntRect expected, IntVector2[] inputs) in testCases)
            {
                IntRect boundingRect = IntRect.BoundingRect(inputs);

                Assert.AreEqual(expected, boundingRect, "Failed with " + Functions.ArrayToString(inputs));

                foreach (IntVector2 point in inputs)
                {
                    Assert.True(boundingRect.Contains(point), "Failed with " + point + " in " + Functions.ArrayToString(inputs));
                }
            }

            // Bounding rect of IntRects

            (IntRect expected, IntRect[] inputs)[] testCases2 =
            {
                (new IntRect(new IntVector2(3, 4), new IntVector2(5, 8)), new IntRect[] {
                    new IntRect(new IntVector2(3, 4), new IntVector2(5, 8))
                }),
                (new IntRect(new IntVector2(2, 4), new IntVector2(9, 8)), new IntRect[] {
                    new IntRect(new IntVector2(3, 4), new IntVector2(5, 8)),
                    new IntRect(new IntVector2(2, 6), new IntVector2(9, 7))
                }),
                (new IntRect(new IntVector2(0, 0), new IntVector2(9, 8)), new IntRect[] {
                    new IntRect(new IntVector2(3, 4), new IntVector2(5, 8)),
                    new IntRect(new IntVector2(2, 6), new IntVector2(9, 7)),
                    new IntRect(new IntVector2(0, 0), new IntVector2(0, 2))
                })
            };

            foreach ((IntRect expected, IntRect[] inputs) in testCases2)
            {
                IntRect boundingRect = IntRect.BoundingRect(inputs);

                Assert.AreEqual(expected, boundingRect, "Failed with " + Functions.ArrayToString(inputs));

                foreach (IntRect rect in inputs)
                {
                    Assert.True(rect.ToHashSet().IsSubsetOf(boundingRect), "Failed with " + rect + " in " + Functions.ArrayToString(inputs));
                }
            }

            // Cannot get bounding rect of 0 IntRects
            //Assert.Throws<ArgumentException>(() => IntRect.BoundingRect());   // The call is now ambiguous
        }

        [Test]
        [Category("Data Structures")]
        public void Intersection()
        {
            const int iterations = 100;
            foreach (IntRect rect1 in randomTestCases.Take(iterations))
            {
                foreach (IntRect rect2 in randomTestCases.Take(iterations))
                {
                    if (Enumerable.Intersect(rect1, rect2).Any())
                    {
                        CollectionAssert.AreEquivalent(Enumerable.Intersect(rect1, rect2), rect1.Intersection(rect2), $"Failed with {rect1} and {rect2}");
                    }
                    else
                    {
                        Assert.Throws<InvalidOperationException>(() => rect1.Intersection(rect2), $"Failed with {rect1} and {rect2}");
                    }
                }
            }
        }

        [Test]
        [Category("Data Structures"), Category("Random")]
        public void RandomPoint()
        {
            for (int seed = 0; seed <= 2; seed++)
            {
                Random random = new Random(seed);

                foreach (IntVector2 bottomLeft in new IntRect(new IntVector2(-1, -1), IntVector2.zero))
                {
                    foreach (IntVector2 topRight in bottomLeft + new IntRect(IntVector2.zero, new IntVector2(2, 3)))
                    {
                        IntRect rect = new IntRect(bottomLeft, topRight);
                        Dictionary<IntVector2, int> counts = new Dictionary<IntVector2, int>();
                        foreach (IntVector2 pixel in rect)
                        {
                            counts[pixel] = 0;
                        }

                        const int iterations = 10_000;
                        for (int i = 0; i < iterations; i++)
                        {
                            IntVector2 randomPoint = rect.RandomPoint(random);
                            Assert.True(rect.Contains(randomPoint), "Failed with " + rect + " and " + randomPoint);

                            counts[randomPoint]++;
                        }

                        float expected = 1f / rect.Count;
                        float tolerance = expected / 5f;
                        foreach (IntVector2 pixel in rect)
                        {
                            Assert.That((float)counts[pixel] / iterations, Is.EqualTo(expected).Within(tolerance), "Failed with " + rect + " and " + pixel);
                        }
                    }
                }
            }
        }
    }
}
