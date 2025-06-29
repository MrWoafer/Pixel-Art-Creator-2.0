using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using PAC.Extensions.System.Collections;
using PAC.Geometry;

namespace PAC.Tests.Geometry
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
                Random random = new Random(0);
                const int minInclusive = -10;
                const int maxInclusive = 10;
                while (true)
                {
                    IntVector2 bottomLeft = new IntVector2(random.Next(minInclusive, maxInclusive + 1), random.Next(minInclusive, maxInclusive + 1));
                    IntVector2 topRight = new IntVector2(random.Next(minInclusive, maxInclusive + 1), random.Next(minInclusive, maxInclusive + 1));
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

        /// <summary>
        /// Tests <see cref="IntRect.XCentreIs(int)"/>.
        /// </summary>
        [Test]
        [Category("Data Structures")]
        public void XCentreIs()
        {
            (IntRect rect, int[] expected)[] testCases =
            {
                (new IntRect((0, 0), (0, 0)), new int[] { 0 }),
                (new IntRect((0, 0), (1, 0)), new int[] { 0, 1 }),
                (new IntRect((3, -2), (10, 7)), new int[] { 6, 7 }),
                (new IntRect((-3, -14), (7, -99)), new int[] { 2 }),
                (new IntRect((-3, 4), (-8, 11)), new int[] { -5, -6 }),
                (new IntRect((-2, 64), (-8, 1000)), new int[] { -5 }),
            };

            foreach ((IntRect rect, int[] expected) in testCases)
            {
                for (int x = rect.minX - 5; x <= rect.maxX + 5; x++)
                {
                    Assert.AreEqual(expected.Contains(x), rect.XCentreIs(x), $"Failed with {rect} and {x}.");
                }
            }
        }

        /// <summary>
        /// Tests <see cref="IntRect.YCentreIs(int)"/>.
        /// </summary>
        [Test]
        [Category("Data Structures")]
        public void YCentreIs()
        {
            (IntRect rect, int[] expected)[] testCases =
            {
                (new IntRect((0, 0), (0, 0)), new int[] { 0 }),
                (new IntRect((0, 0), (0, 1)), new int[] { 0, 1 }),
                (new IntRect((13, 5), (3, 12)), new int[] { 8, 9 }),
                (new IntRect((-5, 7), (-5, 19)), new int[] { 13 }),
                (new IntRect((0, -12), (6, -19)), new int[] { -15, -16 }),
                (new IntRect((-1, -11), (1, -15)), new int[] { -13 }),
            };

            foreach ((IntRect rect, int[] expected) in testCases)
            {
                for (int y = rect.minY - 5; y <= rect.maxY + 5; y++)
                {
                    Assert.AreEqual(expected.Contains(y), rect.YCentreIs(y), $"Failed with {rect} and {y}.");
                }
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

                Assert.AreEqual(expected, boundingRect, "Failed with " + inputs.ToPrettyString());

                foreach (IntVector2 point in inputs)
                {
                    Assert.True(boundingRect.Contains(point), "Failed with " + point + " in " + inputs.ToPrettyString());
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

                Assert.AreEqual(expected, boundingRect, "Failed with " + inputs.ToPrettyString());

                foreach (IntRect rect in inputs)
                {
                    Assert.True(rect.ToHashSet().IsSubsetOf(boundingRect), "Failed with " + rect + " in " + inputs.ToPrettyString());
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

        /// <summary>
        /// Tests <see cref="IntRect.RandomPoint(Random)"/>.
        /// </summary>
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
                        foreach (IntVector2 point in rect)
                        {
                            counts[point] = 0;
                        }

                        const int numIterations = 10_000;
                        for (int i = 0; i < numIterations; i++)
                        {
                            IntVector2 randomPoint = rect.RandomPoint(random);
                            Assert.True(rect.Contains(randomPoint), $"Failed with {rect} and {randomPoint}.");

                            counts[randomPoint]++;
                        }

                        float expected = 1f / rect.Count;
                        float tolerance = expected / 5f;
                        foreach (IntVector2 point in rect)
                        {
                            Assert.AreEqual(expected, counts[point] / (float)numIterations, tolerance, $"Failed with {rect} and {point}.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tests <see cref="IntRect.RandomSubRect(Random)"/> is indeed a subrect of the rect.
        /// </summary>
        [Test]
        [Category("Data Structures"), Category("Random")]
        public void RandomSubRect_IsSubRect()
        {
            for (int seed = 0; seed <= 2; seed++)
            {
                Random random = new Random(seed);

                foreach (IntVector2 bottomLeft in new IntRect(new IntVector2(-1, -1), IntVector2.zero))
                {
                    foreach (IntVector2 topRight in bottomLeft + new IntRect(IntVector2.zero, new IntVector2(2, 3)))
                    {
                        IntRect rect = new IntRect(bottomLeft, topRight);

                        for (int i = 0; i < 10_000; i++)
                        {
                            IntRect randomSubRect = rect.RandomSubRect(random);
                            Assert.True(randomSubRect.ToHashSet().IsSubsetOf(rect), $"Failed with {rect} and {randomSubRect}.");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tests that <see cref="IntRect.RandomSubRect(Random)"/> is not always a single point.
        /// </summary>
        [Test]
        [Category("Data Structures"), Category("Random")]
        public void RandomSubRect_IsNotAlwaysSinglePoint()
        {
            for (int seed = 0; seed <= 2; seed++)
            {
                Random random = new Random(seed);

                foreach (IntVector2 bottomLeft in new IntRect(new IntVector2(-1, -1), IntVector2.zero))
                {
                    foreach (IntVector2 topRight in bottomLeft + new IntRect(IntVector2.zero, new IntVector2(2, 3)))
                    {
                        IntRect rect = new IntRect(bottomLeft, topRight);
                        if (rect.Count == 1)
                        {
                            continue;
                        }

                        bool passed = false;
                        for (int i = 0; i < 10_000; i++)
                        {
                            IntRect randomSubRect = rect.RandomSubRect(random);
                            if (randomSubRect.Count > 1)
                            {
                                passed = true;
                            }
                        }

                        if (!passed)
                        {
                            Assert.Fail($"Failed with {rect}.");
                        }
                    }
                }
            }
        }
    }
}
