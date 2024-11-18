using System;
using System.Linq;
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
        public void AsIEnumerable()
        {
            Assert.True(new Range(4, 4).AsIEnumerable(10).SequenceEqual(new int[0]));
            Assert.True(new Range(4, 5).AsIEnumerable(10).SequenceEqual(new int[] { 4 }));
            Assert.True(new Range(4, 3).AsIEnumerable(10).SequenceEqual(new int[] { 4 }));
            Assert.True(new Range(2, 5).AsIEnumerable(10).SequenceEqual(new int[] { 2, 3, 4 }));
            Assert.True(new Range(6, 2).AsIEnumerable(10).SequenceEqual(new int[] { 6, 5, 4, 3 }));

            Assert.True(new RangeIndexingTest(5)[..^1].AsIEnumerable(5).SequenceEqual(new int[] { 0, 1, 2, 3 }));
            Assert.True(new RangeIndexingTest(5)[..^2].AsIEnumerable(5).SequenceEqual(new int[] { 0, 1, 2 }));
            Assert.True(new RangeIndexingTest(5)[^2..1].AsIEnumerable(5).SequenceEqual(new int[] { 3, 2 }));
        }
    }
}
