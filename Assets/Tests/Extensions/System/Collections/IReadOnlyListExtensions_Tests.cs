using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using PAC.Extensions.System.Collections;

namespace PAC.Tests.Extensions
{
    /// <summary>
    /// Tests for <see cref="IReadOnlyListExtensions"/>.
    /// </summary>
    public class IReadOnlyListExtensions_Tests
    {
        /// <summary>
        /// Tests <see cref="IReadOnlyListExtensions.GetRange{T}(IReadOnlyList{T}, int, int)"/>
        /// </summary>
        [Test]
        [Category("Extensions")]
        public void GetRange_int_int()
        {
            List<int> list = new List<int> { 3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 2 };

            CollectionAssert.AreEqual(Enumerable.Empty<int>(), IReadOnlyListExtensions.GetRange(list, 0, 0));
            CollectionAssert.AreEqual(Enumerable.Empty<int>(), IReadOnlyListExtensions.GetRange(list, 5, 0));

            CollectionAssert.AreEqual(list, IReadOnlyListExtensions.GetRange(list, 0, list.Count));
            CollectionAssert.AreEqual(Enumerable.Reverse(list), IReadOnlyListExtensions.GetRange(list, list.Count - 1, -list.Count));

            CollectionAssert.AreEqual(new int[] { list[8] }, IReadOnlyListExtensions.GetRange(list, 8, 1));
            CollectionAssert.AreEqual(new int[] { list[8] }, IReadOnlyListExtensions.GetRange(list, 8, -1));
            CollectionAssert.AreEqual(new int[] { list[2], list[3], list[4] }, IReadOnlyListExtensions.GetRange(list, 2, 3));
            CollectionAssert.AreEqual(new int[] { list[5], list[4], list[3], list[2] }, IReadOnlyListExtensions.GetRange(list, 5, -4));

            Assert.DoesNotThrow(() => IReadOnlyListExtensions.GetRange(new List<int>(), 0, 0).ToArray());
            Assert.Throws<ArgumentOutOfRangeException>(() => IReadOnlyListExtensions.GetRange(new List<int>(), 0, 1).ToArray());
            Assert.Throws<ArgumentOutOfRangeException>(() => IReadOnlyListExtensions.GetRange(list, 1, list.Count).ToArray());
            Assert.Throws<ArgumentOutOfRangeException>(() => IReadOnlyListExtensions.GetRange(list, 0, -2).ToArray());
        }

        /// <summary>
        /// Tests <see cref="IReadOnlyListExtensions.GetRange{T}(IReadOnlyList{T}, Range)"/>
        /// </summary>
        [Test]
        [Category("Extensions")]
        public void GetRange_Range()
        {
            List<int> list = new List<int> { 3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 2 };

            CollectionAssert.AreEqual(Enumerable.Empty<int>(), IReadOnlyListExtensions.GetRange(list, new Range(new Index(0), new Index(0))));
            CollectionAssert.AreEqual(Enumerable.Empty<int>(), IReadOnlyListExtensions.GetRange(list, new Range(new Index(5), new Index(5))));

            CollectionAssert.AreEqual(list, IReadOnlyListExtensions.GetRange(list, new Range(new Index(0), new Index(0, true))));

            CollectionAssert.AreEqual(new int[] { list[8] }, IReadOnlyListExtensions.GetRange(list, new Range(new Index(8), new Index(9))));
            CollectionAssert.AreEqual(new int[] { list[2], list[3], list[4] }, IReadOnlyListExtensions.GetRange(list, new Range(new Index(2), new Index(5))));
            CollectionAssert.AreEqual(new int[] { list[5], list[4], list[3], list[2] }, IReadOnlyListExtensions.GetRange(list, new Range(new Index(5), new Index(1))));

            CollectionAssert.AreEqual(new int[] { list[^3], list[^2] }, IReadOnlyListExtensions.GetRange(list, new Range(new Index(3, true), new Index(1, true))));
            CollectionAssert.AreEqual(new int[] { list[^2], list[^3] }, IReadOnlyListExtensions.GetRange(list, new Range(new Index(2, true), new Index(4, true))));

            CollectionAssert.AreEqual(list.ToArray()[4..^2], IReadOnlyListExtensions.GetRange(list, new Range(new Index(4), new Index(2, true))));

            Assert.DoesNotThrow(() => IReadOnlyListExtensions.GetRange(new List<int>(), new Range(new Index(0), new Index(0))).ToArray());
            Assert.Throws<ArgumentOutOfRangeException>(() => IReadOnlyListExtensions.GetRange(new List<int>(), new Range(new Index(0), new Index(1))).ToArray());
        }
    }
}
