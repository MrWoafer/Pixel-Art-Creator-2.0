using NUnit.Framework;
using PAC.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PAC.Tests
{
    public class IEnumerableExtensionsTests
    {
        [Test]
        [Category("Extensions")]
        public void IsEmpty()
        {
            for (int inputLength = 0; inputLength <= 10; inputLength++)
            {
                int[] input = new int[inputLength];
                Assert.AreEqual(input.Length == 0, input.IsEmpty(), "Failed with length " + inputLength);
            }
        }

        [Test]
        [Category("Extensions")]
        public void CountExactly()
        {
            for (int inputLength = 0; inputLength <= 10; inputLength++)
            {
                int[] input = new int[inputLength];
                for (int testLength = -10; testLength <= 20; testLength++)
                {
                    Assert.AreEqual(input.Length == testLength, input.CountExactly(testLength), "Failed with input length " + inputLength + " and test length " + testLength);
                }
            }
        }

        [Test]
        [Category("Extensions")]
        public void CountAtLeast()
        {
            for (int inputLength = 0; inputLength <= 10; inputLength++)
            {
                int[] input = new int[inputLength];
                for (int testLength = -10; testLength <= 20; testLength++)
                {
                    Assert.AreEqual(input.Length >= testLength, input.CountAtLeast(testLength), "Failed with input length " + inputLength + " and test length " + testLength);
                }
            }
        }

        [Test]
        [Category("Extensions")]
        public void CountAtMost()
        {
            for (int inputLength = 0; inputLength <= 10; inputLength++)
            {
                int[] input = new int[inputLength];
                for (int testLength = -10; testLength <= 20; testLength++)
                {
                    Assert.AreEqual(input.Length <= testLength, input.CountAtMost(testLength), "Failed with input length " + inputLength + " and test length " + testLength);
                }
            }
        }

        [Test]
        [Category("Extensions")]
        public void ArgMin()
        {
            (string expected, IEnumerable<string> elements, Func<string, int> function)[] testCases =
            {
                ("h", new[] { "h", "el", "lo!" }, s => s.Length),
                ("e", new[] { "he", "e", "lo!" }, s => s.Length),
                ("ij", new[] { "abcde", "fgh", "ij", "klmnop", "qr", "tuv", "wxyz" }, s => s.Length)
            };

            foreach ((string expected, IEnumerable<string> elements, Func<string, int> function) in testCases)
            {
                Assert.AreEqual(expected, elements.ArgMin(function));
            }

            Assert.Throws<ArgumentException>(() => new string[] { }.ArgMin(s => s.Length));
        }

        [Test]
        [Category("Extensions")]
        public void ArgMax()
        {
            (string expected, IEnumerable<string> elements, Func<string, int> function)[] testCases =
            {
                ("lo!", new[] { "h", "el", "lo!" }, s => s.Length),
                ("ell", new[] { "h", "ell", "o!" }, s => s.Length),
                ("abcde", new[] { "abcde", "fgh", "ij", "klmno", "pqr", "tuv", "wxyz" }, s => s.Length)
            };

            foreach ((string expected, IEnumerable<string> elements, Func<string, int> function) in testCases)
            {
                Assert.AreEqual(expected, elements.ArgMax(function));
            }

            Assert.Throws<ArgumentException>(() => new string[] { }.ArgMin(s => s.Length));
        }

        [Test]
        [Category("Extensions")]
        public void Enumerate()
        {
            (IEnumerable<string> input, IEnumerable<(string, int)> expected)[] testCases =
            {
                (new string[] { }, new (string, int)[] { }),
                (new string[] { "a" }, new (string, int)[] { ("a", 0) }),
                (new string[] { "a", "z" }, new (string, int)[] { ("a", 0), ("z", 1) }),
                (new string[] { "a", "z", "hello" }, new (string, int)[] { ("a", 0), ("z", 1), ("hello", 2) }),
            };

            foreach ((IEnumerable<string> input, IEnumerable<(string, int)> expected) in testCases)
            {
                CollectionAssert.AreEqual(expected, input.Enumerate());
            }
        }

        [Test]
        [Category("Extensions")]
        public void Zip()
        {
            (IEnumerable<int> input1, IEnumerable<string> input2, IEnumerable<(int, string)> expected)[] testCases =
            {
                (new int[] { }, new string[] { }, new (int, string)[] { }),
                (new int[] { 1 }, new string[] { }, new (int, string)[] { }),
                (new int[] { }, new string[] { "a" }, new (int, string)[] { }),
                (new int[] { 2 }, new string[] { "b" }, new (int, string)[] { (2, "b") }),
                (new int[] { 1, 2, 3 }, new string[] { "a", "b", "c" }, new (int, string)[] { (1, "a"), (2, "b"), (3, "c") }),
                (new int[] { 1, 2 }, new string[] { "a", "b", "c", "d" }, new (int, string)[] { (1, "a"), (2, "b") }),
                (new int[] { 1, 2, 3, 4, 5 }, new string[] { "a", "b", "c" }, new (int, string)[] { (1, "a"), (2, "b"), (3, "c") })
            };

            foreach ((IEnumerable<int> input1, IEnumerable<string> input2, IEnumerable<(int, string)> expected) in testCases)
            {
                CollectionAssert.AreEqual(expected, input1.Zip(input2));
            }
        }

        [Test]
        [Category("Extensions")]
        public void ZipCurrentAndNext()
        {
            (IEnumerable<int> input, IEnumerable<(int, int)> expected)[] testCases =
            {
                (new int[] { }, new (int, int)[] { }),
                (new int[] { 7 }, new (int, int)[] { }),
                (new int[] { 10, 12 }, new (int, int)[] { (10, 12) }),
                (new int[] { 3, -2, 13 }, new (int, int)[] { (3, -2), (-2, 13) }),
                (new int[] { 3, 1, 4, 2 }, new (int, int)[] { (3, 1), (1, 4), (4, 2) }),
            };

            foreach ((IEnumerable<int> input, IEnumerable<(int, int)> expected) in testCases)
            {
                CollectionAssert.AreEqual(expected, input.ZipCurrentAndNext());
            }
        }
    }
}
