using NUnit.Framework;
using PAC.DataStructures;
using PAC.Exceptions;
using PAC.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PAC.Tests
{
    /// <summary>
    /// Tests for <see cref="IntRange"/>.
    /// </summary>
    public class IntRangeTests
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
                if (range.IsEmpty())
                {
                    Assert.Throws<UndefinedOperationException>(() => { int _ = range.startElement; }, "Failed with " + range);
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
                if (range.IsEmpty())
                {
                    Assert.Throws<UndefinedOperationException>(() => { int _ = range.endElement; }, "Failed with " + range);
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
                if (range.IsEmpty())
                {
                    Assert.Throws<UndefinedOperationException>(() => { int _ = range.minElement; }, "Failed with " + range);
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
                if (range.IsEmpty())
                {
                    Assert.Throws<UndefinedOperationException>(() => { int _ = range.maxElement; }, "Failed with " + range);
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
                if (range.IsEmpty())
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
                Assert.AreEqual(range.IsEmpty(), range.isEmpty, "Failed with " + range);
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
        public void CountLong()
        {
            foreach (IntRange range in testCases)
            {
                Assert.AreEqual(range.LongCount(), range.CountLong, "Failed with " + range);
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
                    Assert.True(((IEnumerable<int>)range1).Intersect((IEnumerable<int>)range2).IsNotEmpty() == range1.Intersects(range2), $"Failed with {range1} and {range2}.");
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
                    Assert.Throws<IndexOutOfRangeException>(() => { int _ = range[i]; }, "Failed with " + range + " and " + i);
                }
                for (int i = range.Count(); i <= range.Count() + 3; i++)
                {
                    Assert.Throws<IndexOutOfRangeException>(() => { int _ = range[i]; }, "Failed with " + range + " and " + i);
                }

                int[] expected = range.ToArray();
                for (int i = 0; i < range.Count(); i++)
                {
                    Assert.AreEqual(expected[i], range[i], "Failed with " + range + " and " + i);
                    Assert.AreEqual(expected[i], range[(long)i], "Failed with " + range + " and " + i);
                }

                Assert.Throws<IndexOutOfRangeException>(() => { int _ = range[long.MinValue]; }, "Failed with " + range);
                Assert.Throws<IndexOutOfRangeException>(() => { int _ = range[long.MaxValue]; }, "Failed with " + range);
            }
        }
        #endregion
    }
}
