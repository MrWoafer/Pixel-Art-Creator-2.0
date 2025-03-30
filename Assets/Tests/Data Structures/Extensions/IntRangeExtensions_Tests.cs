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
    }
}
