using System;
using NUnit.Framework;
using PAC.Extensions;

namespace PAC.Tests
{
    public class RangeExtensionsTests
    {
        private class RangeIndexingTest
        {
            public int Length { get; set; }

            public RangeIndexingTest(int length)
            {
                Length = length;
            }

            public Range this[Range range]
            {
                get => range;
            }
        }

        [Test]
        [Category("Extensions")]
        public void AsEnumerable()
        {
            CollectionAssert.AreEqual(new int[0], new Range(4, 4).AsEnumerable(10));
            CollectionAssert.AreEqual(new int[] { 4 }, new Range(4, 5).AsEnumerable(10));
            CollectionAssert.AreEqual(new int[] { 4 }, new Range(4, 3).AsEnumerable(10));
            CollectionAssert.AreEqual(new int[] { 2, 3, 4 }, new Range(2, 5).AsEnumerable(10));
            CollectionAssert.AreEqual(new int[] { 6, 5, 4, 3 }, new Range(6, 2).AsEnumerable(10));

            CollectionAssert.AreEqual(new int[] { 1, 2 }, new RangeIndexingTest(5)[1..^2].AsEnumerable(5));
            CollectionAssert.AreEqual(new int[] { 0, 1, 2, 3 }, new RangeIndexingTest(5)[..^1].AsEnumerable(5));
            CollectionAssert.AreEqual(new int[] { 0, 1, 2 }, new RangeIndexingTest(5)[..^2].AsEnumerable(5));
            CollectionAssert.AreEqual(new int[] { 3, 2 }, new RangeIndexingTest(5)[^2..1].AsEnumerable(5));
        }
    }
}
