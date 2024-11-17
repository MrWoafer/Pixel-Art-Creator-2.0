using System;
using System.Linq;
using NUnit.Framework;
using PAC.Extensions;

namespace PAC.Tests
{
    public class RangeExtensionsTests
    {
        [Test]
        [Category("Extensions")]
        public void AsIEnumerable()
        {
            Assert.True(new Range(4, 4).AsIEnumerable().SequenceEqual(new int[0]));
            Assert.True(new Range(4, 5).AsIEnumerable().SequenceEqual(new int[] { 4 }));
            Assert.True(new Range(4, 3).AsIEnumerable().SequenceEqual(new int[] { 4 }));
            Assert.True(new Range(2, 5).AsIEnumerable().SequenceEqual(new int[] { 2, 3, 4 }));
            Assert.True(new Range(6, 2).AsIEnumerable().SequenceEqual(new int[] { 6, 5, 4, 3 }));
        }
    }
}
