using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using PAC.DataStructures;
using PAC.DataStructures.Extensions;

namespace PAC.Tests.DataStructures.Extensions
{
    /// <summary>
    /// Tests for <see cref="IntRangeExtensions"/>.
    /// </summary>
    public class IntRangeExtensions_Tests
    {
        [Test]
        [Category("Extensions")]
        public void GetRange()
        {
            List<int> list = new List<int> { 3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 2 };

            CollectionAssert.AreEqual(Enumerable.Empty<int>(), IntRangeExtensions.GetRange(list, IntRange.Empty));
            CollectionAssert.AreEqual(Enumerable.Empty<int>(), IntRangeExtensions.GetRange(list, IntRange.InclExcl(5, 5)));

            CollectionAssert.AreEqual(list, IntRangeExtensions.GetRange(list, IntRange.InclExcl(0, list.Count)));
            CollectionAssert.AreEqual(Enumerable.Reverse(list), IntRangeExtensions.GetRange(list, IntRange.ExclIncl(list.Count, 0)));

            CollectionAssert.AreEqual(new int[] { list[8] }, IntRangeExtensions.GetRange(list, IntRange.Singleton(8)));
            CollectionAssert.AreEqual(new int[] { list[8] }, IntRangeExtensions.GetRange(list, IntRange.InclExcl(8, 9)));
            CollectionAssert.AreEqual(new int[] { list[8] }, IntRangeExtensions.GetRange(list, IntRange.InclExcl(8, 7)));

            CollectionAssert.AreEqual(new int[] { list[2], list[3], list[4] }, IntRangeExtensions.GetRange(list, IntRange.InclIncl(2, 4)));
            CollectionAssert.AreEqual(new int[] { list[2], list[3], list[4] }, IntRangeExtensions.GetRange(list, IntRange.InclExcl(2, 5)));
            CollectionAssert.AreEqual(new int[] { list[2], list[3], list[4] }, IntRangeExtensions.GetRange(list, IntRange.ExclIncl(1, 4)));
            CollectionAssert.AreEqual(new int[] { list[2], list[3], list[4] }, IntRangeExtensions.GetRange(list, IntRange.ExclExcl(1, 5)));

            CollectionAssert.AreEqual(new int[] { list[5], list[4], list[3], list[2] }, IntRangeExtensions.GetRange(list, IntRange.InclIncl(5, 2)));
            CollectionAssert.AreEqual(new int[] { list[5], list[4], list[3], list[2] }, IntRangeExtensions.GetRange(list, IntRange.InclExcl(5, 1)));
            CollectionAssert.AreEqual(new int[] { list[5], list[4], list[3], list[2] }, IntRangeExtensions.GetRange(list, IntRange.ExclIncl(6, 2)));
            CollectionAssert.AreEqual(new int[] { list[5], list[4], list[3], list[2] }, IntRangeExtensions.GetRange(list, IntRange.ExclExcl(6, 1)));

            Assert.DoesNotThrow(() => IntRangeExtensions.GetRange(new List<int>(), IntRange.Empty).ToArray());
            Assert.Throws<ArgumentOutOfRangeException>(() => IntRangeExtensions.GetRange(new List<int>(), IntRange.Singleton(0)).ToArray());
            Assert.Throws<ArgumentOutOfRangeException>(() => IntRangeExtensions.GetRange(list, IntRange.InclIncl(0, list.Count)).ToArray());
            Assert.Throws<ArgumentOutOfRangeException>(() => IntRangeExtensions.GetRange(list, IntRange.InclIncl(0, -1)).ToArray());
        }

        /// <summary>
        /// Tests <see cref="IntRangeExtensions.Range(IEnumerable{int})"/>.
        /// </summary>
        [Test]
        [Category("Data Structures")]
        public void Range()
        {
            Random random = new Random(0);

            const int maxTestCaseLength = 20;
            const int numTestCasesPerLength = 1_000;

            List<int[]> testCases = new List<int[]>();
            for (int length = 1; length <= maxTestCaseLength; length++)
            {
                for (int i = 0; i < numTestCasesPerLength; i++)
                {
                    int[] testCase = new int[length];
                    for (int j = 0; j < length; j++)
                    {
                        testCase[j] = random.Next(-100, 100);
                    }
                    testCases.Add(testCase);
                }
            }

            /////

            Assert.True(IntRangeExtensions.Range(new int[] { }).isEmpty);
            Assert.True(IntRangeExtensions.Range(new int[] { }).isExclExcl);

            foreach (int[] testCase in testCases)
            {
                IntRange boundingRange = IntRangeExtensions.Range(testCase);

                foreach (int value in testCase)
                {
                    Assert.True(boundingRange.Contains(value), $"Failed with {testCase} and {value}.");
                }
                Assert.False(boundingRange.Contains(Enumerable.Min(boundingRange) - 1), $"Failed with {testCase}.");
                Assert.False(boundingRange.Contains(Enumerable.Max(boundingRange) + 1), $"Failed with {testCase}.");

                Assert.True(boundingRange.isInclIncl, $"Failed with {testCase}");
            }

            Assert.Throws<ArgumentNullException>(() => IntRangeExtensions.Range(null));
        }
    }
}
