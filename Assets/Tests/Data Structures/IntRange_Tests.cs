using NUnit.Framework;

using PAC.DataStructures;
using PAC.Extensions.System.Collections;

using System;
using System.Collections.Generic;
using System.Linq;

namespace PAC.Tests.DataStructures
{
    /// <summary>
    /// Tests for <see cref="IntRange"/>.
    /// </summary>
    public class IntRange_Tests
    {
        public IEnumerable<IntRange> testCases
        {
            get
            {
                for (int startBoundary = -10; startBoundary <= 10; startBoundary++)
                {
                    for (int endBoundary = -10; endBoundary <= 10; endBoundary++)
                    {
                        foreach (bool startBoundaryInclusive in new bool[] { true, false })
                        {
                            foreach (bool endBoundaryInclusive in new bool[] { true, false })
                            {
                                yield return new IntRange(startBoundary, endBoundary, startBoundaryInclusive, endBoundaryInclusive);
                            }
                        }
                    }
                }
            }
        }

        #region Tests: Properties
        [Test]
        [Category("Data Structures")]
        public void startElement()
        {
            foreach (IntRange range in testCases)
            {
                if (range.None())
                {
                    Assert.Throws<InvalidOperationException>(() => { int _ = range.startElement; }, "Failed with " + range);
                }
                else
                {
                    Assert.AreEqual(range.First(), range.startElement, "Failed with " + range);
                }
            }
        }

        [Test]
        [Category("Data Structures")]
        public void endElement()
        {
            foreach (IntRange range in testCases)
            {
                if (range.None())
                {
                    Assert.Throws<InvalidOperationException>(() => { int _ = range.endElement; }, "Failed with " + range);
                }
                else
                {
                    Assert.AreEqual(range.Last(), range.endElement, "Failed with " + range);
                }
            }
        }

        [Test]
        [Category("Data Structures")]
        public void minElement()
        {
            foreach (IntRange range in testCases)
            {
                if (range.None())
                {
                    Assert.Throws<InvalidOperationException>(() => { int _ = range.minElement; }, "Failed with " + range);
                }
                else
                {
                    Assert.AreEqual(range.Min(), range.minElement, "Failed with " + range);
                }
            }
        }

        [Test]
        [Category("Data Structures")]
        public void maxElement()
        {
            foreach (IntRange range in testCases)
            {
                if (range.None())
                {
                    Assert.Throws<InvalidOperationException>(() => { int _ = range.maxElement; }, "Failed with " + range);
                }
                else
                {
                    Assert.AreEqual(range.Max(), range.maxElement, "Failed with " + range);
                }
            }
        }

        [Test]
        [Category("Data Structures")]
        public void reverse()
        {
            foreach (IntRange range in testCases)
            {
                CollectionAssert.AreEqual(range.Reverse(), range.reverse, "Failed with " + range);
            }
        }

        [Test]
        [Category("Data Structures")]
        public void asIncreasing()
        {
            foreach (IntRange range in testCases)
            {
                Assert.True(range.asIncreasing == range || range.asIncreasing == range.reverse, "Failed with " + range);
                CollectionAssert.AreEqual(range.OrderBy(x => x), range.asIncreasing, "Failed with " + range);
            }
        }

        [Test]
        [Category("Data Structures")]
        public void asDecreasing()
        {
            foreach (IntRange range in testCases)
            {
                Assert.True(range.asDecreasing == range || range.asDecreasing == range.reverse, "Failed with " + range);
                CollectionAssert.AreEqual(range.OrderByDescending(x => x), range.asDecreasing, "Failed with " + range);
            }
        }

        [Test]
        [Category("Data Structures")]
        public void asExclExcl()
        {
            foreach (IntRange range in testCases)
            {
                IntRange asExclExcl = range.asExclExcl;
                Assert.False(asExclExcl.startBoundaryInclusive, "Failed with " + range);
                Assert.False(asExclExcl.endBoundaryInclusive, "Failed with " + range);
                // Not using CollectionAssert.AreEqual() here since that seemed to check Equals() before SequenceEquals(), which would result in an incorrect test
                Assert.True(range.SequenceEqual(asExclExcl), "Failed with " + range);
            }
        }

        [Test]
        [Category("Data Structures")]
        public void asExclIncl()
        {
            foreach (IntRange range in testCases)
            {
                IntRange asExclIncl = range.asExclIncl;
                Assert.False(asExclIncl.startBoundaryInclusive, "Failed with " + range);
                Assert.True(asExclIncl.endBoundaryInclusive, "Failed with " + range);
                // Not using CollectionAssert.AreEqual() here since that seemed to check Equals() before SequenceEquals(), which would result in an incorrect test
                Assert.True(range.SequenceEqual(asExclIncl), "Failed with " + range);
            }
        }

        [Test]
        [Category("Data Structures")]
        public void asInclExcl()
        {
            foreach (IntRange range in testCases)
            {
                IntRange asInclExcl = range.asInclExcl;
                Assert.True(asInclExcl.startBoundaryInclusive, "Failed with " + range);
                Assert.False(asInclExcl.endBoundaryInclusive, "Failed with " + range);
                // Not using CollectionAssert.AreEqual() here since that seemed to check Equals() before SequenceEquals(), which would result in an incorrect test
                Assert.True(range.SequenceEqual(asInclExcl), "Failed with " + range);
            }
        }

        [Test]
        [Category("Data Structures")]
        public void asInclIncl()
        {
            foreach (IntRange range in testCases)
            {
                if (range.None())
                {
                    Assert.Throws<InvalidOperationException>(() => { IntRange _ = range.asInclIncl; }, "Failed with " + range);
                }
                else
                {
                    IntRange asInclIncl = range.asInclIncl;
                    Assert.True(asInclIncl.startBoundaryInclusive, "Failed with " + range);
                    Assert.True(asInclIncl.endBoundaryInclusive, "Failed with " + range);
                    // Not using CollectionAssert.AreEqual() here since that seemed to check Equals() before SequenceEquals(), which would result in an incorrect test
                    Assert.True(range.SequenceEqual(asInclIncl), "Failed with " + range);
                }
            }
        }

        [Test]
        [Category("Data Structures")]
        public void isEmpty()
        {
            foreach (IntRange range in testCases)
            {
                Assert.AreEqual(range.None(), range.isEmpty, "Failed with " + range);
            }
        }

        [Test]
        [Category("Data Structures")]
        public void Count()
        {
            foreach (IntRange range in testCases)
            {
                Assert.AreEqual(range.Count(), range.Count, "Failed with " + range);
            }
        }

        [Test]
        [Category("Data Structures")]
        public void LongCount()
        {
            foreach (IntRange range in testCases)
            {
                Assert.AreEqual(range.LongCount(), range.LongCount, "Failed with " + range);
            }
        }
        #endregion

        #region Tests: Comparison
        [Test]
        [Category("Data Structures")]
        public void Contains()
        {
            foreach (IntRange range in testCases)
            {
                for (int x = -11; x <= 11; x++)
                {
                    Assert.AreEqual(((IEnumerable<int>)range).Contains(x), range.Contains(x), "Failed with " + range + " and " + x);
                }
            }
        }

        [Test]
        [Category("Data Structures")]
        public void Intersects()
        {
            foreach (IntRange range1 in testCases)
            {
                foreach (IntRange range2 in testCases)
                {
                    // Using Assert.True(x == y) is much faster than Assert.AreEqual(x, y)
                    Assert.True(((IEnumerable<int>)range1).Intersect((IEnumerable<int>)range2).Any() == range1.Intersects(range2), $"Failed with {range1} and {range2}.");
                }
            }
        }
        #endregion

        #region Tests: Operations
        [Test]
        [Category("Data Structures")]
        public void Add()
        {
            foreach (IntRange range in testCases)
            {
                for (int add = -3; add <= 3; add++)
                {
                    Assert.AreEqual(range.startBoundary + add, (range + add).startBoundary, "Failed with " + range + " and " + add);
                    Assert.AreEqual(range.endBoundary + add, (range + add).endBoundary, "Failed with " + range + " and " + add);
                    Assert.AreEqual(range.startBoundaryInclusive, (range + add).startBoundaryInclusive, "Failed with " + range + " and " + add);
                    Assert.AreEqual(range.endBoundaryInclusive, (range + add).endBoundaryInclusive, "Failed with " + range + " and " + add);
                    CollectionAssert.AreEqual(range.Select(x => x + add), range + add, "Failed with " + range + " and " + add);

                    Assert.AreEqual(add + range.startBoundary, (add + range).startBoundary, "Failed with " + range + " and " + add);
                    Assert.AreEqual(add + range.endBoundary, (add + range).endBoundary, "Failed with " + range + " and " + add);
                    Assert.AreEqual(range.startBoundaryInclusive, (add + range).startBoundaryInclusive, "Failed with " + range + " and " + add);
                    Assert.AreEqual(range.endBoundaryInclusive, (add + range).endBoundaryInclusive, "Failed with " + range + " and " + add);
                    CollectionAssert.AreEqual(range.Select(x => add + x), add + range, "Failed with " + range + " and " + add);
                }
            }
        }

        [Test]
        [Category("Data Structures")]
        public void Subtract()
        {
            foreach (IntRange range in testCases)
            {
                for (int subtract = -3; subtract <= 3; subtract++)
                {
                    Assert.AreEqual(range.startBoundary - subtract, (range - subtract).startBoundary, "Failed with " + range + " and " + subtract);
                    Assert.AreEqual(range.endBoundary - subtract, (range - subtract).endBoundary, "Failed with " + range + " and " + subtract);
                    Assert.AreEqual(range.startBoundaryInclusive, (range - subtract).startBoundaryInclusive, "Failed with " + range + " and " + subtract);
                    Assert.AreEqual(range.endBoundaryInclusive, (range - subtract).endBoundaryInclusive, "Failed with " + range + " and " + subtract);
                    CollectionAssert.AreEqual(range.Select(x => x - subtract), range - subtract, "Failed with " + range + " and " + subtract);

                    Assert.AreEqual(subtract - range.startBoundary, (subtract - range).startBoundary, "Failed with " + range + " and " + subtract);
                    Assert.AreEqual(subtract - range.endBoundary, (subtract - range).endBoundary, "Failed with " + range + " and " + subtract);
                    Assert.AreEqual(range.startBoundaryInclusive, (subtract - range).startBoundaryInclusive, "Failed with " + range + " and " + subtract);
                    Assert.AreEqual(range.endBoundaryInclusive, (subtract - range).endBoundaryInclusive, "Failed with " + range + " and " + subtract);
                    CollectionAssert.AreEqual(range.Select(x => subtract - x), subtract - range, "Failed with " + range + " and " + subtract);
                }
            }
        }

        [Test]
        [Category("Data Structures")]
        public void Negate()
        {
            foreach (IntRange range in testCases)
            {
                Assert.AreEqual(-(range.startBoundary), (-range).startBoundary, "Failed with " + range);
                Assert.AreEqual(-(range.endBoundary), (-range).endBoundary, "Failed with " + range);
                Assert.AreEqual(range.startBoundaryInclusive, (-range).startBoundaryInclusive, "Failed with " + range);
                Assert.AreEqual(range.endBoundaryInclusive, (-range).endBoundaryInclusive, "Failed with " + range);
                CollectionAssert.AreEqual(range.Select(x => -x), -range, "Failed with " + range);
            }
        }

        /// <summary>
        /// Tests <see cref="IntRange.Extend(int, int)"/>.
        /// </summary>
        [Test]
        [Category("Data Structures")]
        public void Extend()
        {
            foreach (IntRange range in testCases)
            {
                for (int startOffset = -2; startOffset <= 2; startOffset++)
                {
                    for (int endOffset = -2; endOffset <= 2; endOffset++)
                    {
                        IntRange extended = range.Extend(startOffset, endOffset);
                        Assert.AreEqual(range.startBoundary + startOffset, extended.startBoundary, $"Failed with {range} and {startOffset}, {endOffset}.");
                        Assert.AreEqual(range.endBoundary + endOffset, extended.endBoundary, $"Failed with {range} and {startOffset}, {endOffset}.");
                        Assert.AreEqual(range.startBoundaryInclusive, extended.startBoundaryInclusive, $"Failed with {range} and {startOffset}, {endOffset}.");
                        Assert.AreEqual(range.endBoundaryInclusive, extended.endBoundaryInclusive, $"Failed with {range} and {startOffset}, {endOffset}.");
                    }
                }
            }
        }

        [Test]
        [Category("Data Structures")]
        public void Intersection()
        {
            foreach (IntRange range1 in testCases)
            {
                foreach (IntRange range2 in testCases)
                {
                    CollectionAssert.AreEquivalent(((IEnumerable<int>)range1).Intersect((IEnumerable<int>)range2), range1.Intersection(range2), $"Failed with {range1} and {range2}.");
                }
            }
        }

        [Test]
        [Category("Data Structures")]
        public void Clamp()
        {
            foreach (IntRange range in testCases)
            {
                if (range.None())
                {
                    for (int n = -5; n <= 5; n++)
                    {
                        Assert.Throws<InvalidOperationException>(() => range.Clamp(n), $"Failed with {range} and {n}.");
                    }
                    continue;
                }

                int min = range.Min();
                int max = range.Max();
                for (int n = min - 5; n <= max + 5; n++)
                {
                    if (n < min)
                    {
                        Assert.AreEqual(min, range.Clamp(n), $"Failed with {range} and {n}.");
                    }
                    else if (min <= n && n <= max)
                    {
                        Assert.AreEqual(n, range.Clamp(n), $"Failed with {range} and {n}.");
                    }
                    else if (n > max)
                    {
                        Assert.AreEqual(max, range.Clamp(n), $"Failed with {range} and {n}.");
                    }
                }
            }
        }
        #endregion

        #region Random
        /// <summary>
        /// Tests <see cref="IntRange.RandomDistinctOrderedPair(Random)"/>.
        /// </summary>
        [Test]
        [Category("Data Structures"), Category("Random")]
        public void RandomDistinctOrderedPair()
        {
            for (int seed = 0; seed <= 2; seed++)
            {
                Random random = new Random(seed);

                foreach (IntRange range in testCases.Take(100))
                {
                    if (range.Count <= 1)
                    {
                        Assert.Throws<InvalidOperationException>(() => range.RandomDistinctOrderedPair(random), $"Failed with {range}.");
                        continue;
                    }

                    Dictionary<(int, int), int> counts = new Dictionary<(int, int), int>();

                    const int numIterations = 10_000;
                    for (int i = 0; i < numIterations; i++)
                    {
                        (int x, int y) randomOrderedPair = range.RandomDistinctOrderedPair(random);
                        Assert.True(range.Contains(randomOrderedPair.x), $"Failed with {range} and {randomOrderedPair}.");
                        Assert.True(range.Contains(randomOrderedPair.y), $"Failed with {range} and {randomOrderedPair}.");

                        counts[randomOrderedPair] = counts.GetValueOrDefault(randomOrderedPair, 0) + 1;
                    }

                    int numDistinctOrderedPairs = range.Count * (range.Count - 1);
                    float expected = 1f / numDistinctOrderedPairs;
                    float tolerance = 0.02f;
                    foreach ((int, int) orderedPair in counts.Keys)
                    {
                        Assert.AreEqual(expected, counts.GetValueOrDefault(orderedPair, 0) / (float)numIterations, tolerance, $"Failed with {range} and {orderedPair}.");
                    }
                }
            }
        }
        #endregion

        #region Tests: Enumerator
        [Test]
        [Category("Data Structures")]
        public void Indexer()
        {
            foreach (IntRange range in testCases)
            {
                for (int i = -3; i < 0; i++)
                {
                    Assert.Throws<ArgumentOutOfRangeException>(() => { int _ = range[i]; }, "Failed with " + range + " and " + i);
                }
                for (int i = range.Count(); i <= range.Count() + 3; i++)
                {
                    Assert.Throws<ArgumentOutOfRangeException>(() => { int _ = range[i]; }, "Failed with " + range + " and " + i);
                }

                int[] expected = range.ToArray();
                for (int i = 0; i < range.Count(); i++)
                {
                    Assert.AreEqual(expected[i], range[i], "Failed with " + range + " and " + i);
                    Assert.AreEqual(expected[i], range[(long)i], "Failed with " + range + " and " + i);
                }

                Assert.Throws<ArgumentOutOfRangeException>(() => { int _ = range[long.MinValue]; }, "Failed with " + range);
                Assert.Throws<ArgumentOutOfRangeException>(() => { int _ = range[long.MaxValue]; }, "Failed with " + range);
            }
        }
        #endregion
    }
}
