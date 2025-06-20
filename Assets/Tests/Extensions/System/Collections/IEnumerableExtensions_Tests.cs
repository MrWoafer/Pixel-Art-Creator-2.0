using NUnit.Framework;

using PAC.Extensions.System;
using PAC.Extensions.System.Collections;

using System;
using System.Collections.Generic;
using System.Linq;

namespace PAC.Tests.Extensions.System.Collections
{
    public class IEnumerableExtensions_Tests
    {
        [Test]
        [Category("Extensions")]
        public void Replace()
        {
            (int toReplace, int replaceWith, IEnumerable<int> sequence, IEnumerable<int> expected)[] testCases =
            {
                (0, 1, new int[] { }, new int[] { }),
                (0, 1, new int[] { 0 }, new int[] { 1 }),
                (1, 1, new int[] { 0 }, new int[] { 0 }),
                (0, 1, new int[] { 0, 1, 0, 2 }, new int[] { 1, 1, 1, 2 }),
                (2, 0, new int[] { 0, 1, 0, 2 }, new int[] { 0, 1, 0, 0 }),
                (1, 3, new int[] { 0, 1, 0, 2 }, new int[] { 0, 3, 0, 2 }),
                (5, 1, new int[] { 0, 1, 0, 2 }, new int[] { 0, 1, 0, 2 }),
            };

            foreach ((int toReplace, int replaceWith, IEnumerable<int> sequence, IEnumerable<int> expected) in testCases)
            {
                CollectionAssert.AreEqual(expected, sequence.Replace(toReplace, replaceWith), $"Failed with {{ {string.Join(", ", sequence)} }}.");
            }
        }

        [Test]
        [Category("Extensions")]
        public void None()
        {
            for (int inputLength = 0; inputLength <= 10; inputLength++)
            {
                int[] input = new int[inputLength];
                Assert.AreEqual(input.Length == 0, input.None(), $"Failed with length {inputLength}");
            }
        }

        [Test]
        [Category("Extensions")]
        public void CountIsExactly()
        {
            for (int inputLength = 0; inputLength <= 10; inputLength++)
            {
                int[] input = new int[inputLength];
                for (int testLength = -10; testLength <= 20; testLength++)
                {
                    Assert.AreEqual(input.Length == testLength, input.CountIsExactly(testLength), "Failed with input length " + inputLength + " and test length " + testLength);
                }
            }
        }

        [Test]
        [Category("Extensions")]
        public void CountIsAtLeast()
        {
            for (int inputLength = 0; inputLength <= 10; inputLength++)
            {
                int[] input = new int[inputLength];
                for (int testLength = -10; testLength <= 20; testLength++)
                {
                    Assert.AreEqual(input.Length >= testLength, input.CountIsAtLeast(testLength), "Failed with input length " + inputLength + " and test length " + testLength);
                }
            }
        }

        [Test]
        [Category("Extensions")]
        public void CountIsAtMost()
        {
            for (int inputLength = 0; inputLength <= 10; inputLength++)
            {
                int[] input = new int[inputLength];
                for (int testLength = -10; testLength <= 20; testLength++)
                {
                    Assert.AreEqual(input.Length <= testLength, input.CountIsAtMost(testLength), "Failed with input length " + inputLength + " and test length " + testLength);
                }
            }
        }

        [Test]
        [Category("Extensions")]
        public void AreAllDistinct()
        {
            IEnumerable<int> randomSequence = new Random(0).ToSequence(-10, 11);

            const int maxTestCaseLength = 10;
            const int numTestCasesPerLength = 100;

            List<IEnumerable<int>> testCases = new List<IEnumerable<int>>
            {
                Enumerable.Empty<int>()
            };
            for (int length = 1; length <= maxTestCaseLength; length++)
            {
                for (int i = 0; i < numTestCasesPerLength; i++)
                {
                    // We do the ToArray() to 'save' the sequence, as iterating over a ToSequence() more than once can give a different sequence.
                    testCases.Add(randomSequence.Take(length).ToArray());
                }
            }

            /////
            
            foreach (IEnumerable<int> testCase in testCases)
            {
                Assert.AreEqual(testCase.Distinct().Count() == testCase.Count(), testCase.AreAllDistinct(), $"Failed with {{ {string.Join(", ", testCase)} }}");
            }
        }

        /// <summary>
        /// Tests <see cref="IEnumerableExtensions.IsSubsequenceOf{T}(IEnumerable{T}, IEnumerable{T})"/> and <see cref="IEnumerableExtensions.IsSupersequenceOf{T}(IEnumerable{T}, IEnumerable{T})"/>.
        /// </summary>
        [Test]
        [Category("Extensions")]
        public void IsSubsequenceOf()
        {
            // Example test cases

            (bool expected, IEnumerable<int> sequence, IEnumerable<int> otherSequence)[] exampleTestCases =
            {
                (true, new int[] { }, new int[] { }),
                (true, new int[] { }, new int[] { 0 }),
                (true, new int[] { }, new int[] { 0, 1 }),
                (false, new int[] { 0 }, new int[] { }),
                /////
                (true, new int[] { 0 }, new int[] { 0 }),
                (false, new int[] { 1 }, new int[] { 0 }),
                (true, new int[] { 0 }, new int[] { 0, 1 }),
                (true, new int[] { 1 }, new int[] { 0, 1 }),
                (false, new int[] { 2 }, new int[] { 0, 1 }),
                (true, new int[] { 0 }, new int[] { 0, 1, 2 }),
                (true, new int[] { 0 }, new int[] { 0, 1, 2 }),
                (true, new int[] { 1 }, new int[] { 0, 1, 2 }),
                (true, new int[] { 2 }, new int[] { 0, 1, 2 }),
                (false, new int[] { 3 }, new int[] { 0, 1, 2 }),
                /////
                (true, new int[] { 3, 1, 4, 1, 5 }, new int[] { 3, 1, 4, 1, 5 }),
                (true, new int[] { 3, 1, 4, 1, 5 }, new int[] { 3, 1, 2, 4, 1, 5 }),
                (true, new int[] { 3, 1, 4, 1, 5 }, new int[] { 3, 1, 3, 4, 1, 5 }),
                (true, new int[] { 3, 1, 4, 1, 5 }, new int[] { 3, 1, 1, 4, 1, 5 }),
                (true, new int[] { 3, 1, 4, 1, 5 }, new int[] { 0, 0, 3, 1, 4, 1, 5 }),
                (true, new int[] { 3, 1, 4, 1, 5 }, new int[] { 3, 3, 1, 1, 4, 4, 1, 1, 5, 5 }),
                (true, new int[] { 3, 1, 4, 1, 5 }, new int[] { 3, 1, 3, 1, 4, 1, 5 }),
                (false, new int[] { 3, 1, 4, 1, 5 }, new int[] { 3, 1, 4, 2, 5 }),
                (false, new int[] { 3, 1, 4, 1, 5 }, new int[] { 3, 1, 5, 1, 4 }),
                (false, new int[] { 3, 1, 4, 1, 5 }, new int[] { 3, 1, 1, 4, 5 }),
                (false, new int[] { 3, 1, 4, 1, 5 }, new int[] { 3, 1, 4, 5 }),
                ///
                (true, new int[] { 3, 3 }, new int[] { 3, 3 }),
                (true, new int[] { 3, 3 }, new int[] { 2, 3, 1, 3 }),
                (false, new int[] { 3, 3 }, new int[] { 3 }),
                (false, new int[] { 3, 3 }, new int[] { 2, 3, 1 })
            };

            foreach ((bool expected, IEnumerable<int> sequence, IEnumerable<int> otherSequence) in exampleTestCases)
            {
                // Every sequence is a subsequence of itself
                Assert.True(sequence.IsSubsequenceOf(sequence), $"Failed with {{ {string.Join(", ", sequence)} }}.");

                bool isSubsequence = sequence.IsSubsequenceOf(otherSequence);
                Assert.AreEqual(expected, isSubsequence, $"Failed with {{ {string.Join(", ", sequence)} }} and {{ {string.Join(", ", expected)} }}.");
                Assert.AreEqual(isSubsequence, otherSequence.IsSupersequenceOf(sequence), $"Failed with {{ {string.Join(", ", sequence)} }} and {{ {string.Join(", ", expected)} }}.");
            }

            // Random test cases

            Random random = new Random(0);

            const int maxTestCaseLength = 10;
            const int numTestCasesPerLength = 100;

            List<int[]> randomTestCases = new List<int[]>
            {
                new int[] { }
            };
            for (int length = 1; length <= maxTestCaseLength; length++)
            {
                for (int i = 0; i < numTestCasesPerLength; i++)
                {
                    int[] testCase = new int[length];
                    for (int j = 0; j < length; j++)
                    {
                        testCase[j] = random.Next(-10, 11);
                    }
                    randomTestCases.Add(testCase);
                }
            }

            /////

            foreach (int[] testCase1 in randomTestCases)
            {
                // Every sequence is a subsequence of itself
                Assert.True(testCase1.IsSubsequenceOf(testCase1), $"Failed with {{ {string.Join(", ", testCase1)} }}.");

                // Adding more elements to a sequence results in a supersequence
                List<int> supersequence = new List<int>(testCase1);
                for (int extra = 0; extra < 5; extra++)
                {
                    supersequence.Insert(random.Next(0, supersequence.Count + 1), random.Next(-10, 11));
                    Assert.True(testCase1.IsSubsequenceOf(supersequence), $"Failed with {{ {string.Join(", ", testCase1)} }}.");
                    Assert.True(supersequence.IsSupersequenceOf(testCase1), $"Failed with {{ {string.Join(", ", testCase1)} }}.");
                }

                foreach (int[] testCase2 in randomTestCases)
                {
                    bool isSubsequence = testCase1.IsSubsequenceOf(testCase2);

                    Assert.AreEqual(isSubsequence, testCase2.IsSupersequenceOf(testCase1), $"Failed with {{ {string.Join(", ", testCase1)} }} and {{ {string.Join(", ", testCase2)} }}.");

                    // The empty sequence is a subsequence of every sequence
                    if (testCase1.Length == 0)
                    {
                        Assert.True(isSubsequence, $"Failed with {{ {string.Join(", ", testCase1)} }} and {{ {string.Join(", ", testCase2)} }}.");
                    }
                    // A sequence cannot be a subsequence of a shorter sequence
                    if (testCase1.Length > testCase2.Length)
                    {
                        Assert.False(isSubsequence, $"Failed with {{ {string.Join(", ", testCase1)} }} and {{ {string.Join(", ", testCase2)} }}.");
                    }
                    // A subsequence must be a subset
                    if (!testCase1.ToHashSet().IsSubsetOf(testCase2))
                    {
                        Assert.False(isSubsequence, $"Failed with {{ {string.Join(", ", testCase1)} }} and {{ {string.Join(", ", testCase2)} }}.");
                    }

                    bool expectedIsSubsequence = true;
                    int indexOfPreviousTerm = -1;
                    foreach (int term in testCase1)
                    {
                        if (indexOfPreviousTerm + 1 >= testCase2.Length)
                        {
                            expectedIsSubsequence = false;
                            break;
                        }

                        indexOfPreviousTerm = Array.FindIndex(testCase2, indexOfPreviousTerm + 1, x => x == term);

                        if (indexOfPreviousTerm == -1)
                        {
                            expectedIsSubsequence = false;
                            break;
                        }
                    }
                    Assert.AreEqual(expectedIsSubsequence, isSubsequence, $"Failed with {{ {string.Join(", ", testCase1)} }} and {{ {string.Join(", ", testCase2)} }}.");
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

        private class NoComparer { }

        [Test]
        [Category("Extensions")]
        public void MinAndMax()
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

            foreach (int[] testCase in testCases)
            {
                var output = testCase.MinAndMax();
                Assert.AreEqual(((IEnumerable<int>)testCase).Min(), output.min);
                Assert.AreEqual(((IEnumerable<int>)testCase).Max(), output.max);
            }

            // Empty check
            Assert.Throws<ArgumentException>(() => new int[] { }.MinAndMax());
            // Null check
            Assert.Throws<ArgumentNullException>(() => IEnumerableExtensions.MinAndMax<int>(null));
            // Test that it throws an exception if the element type doesn't have a default comparer
            Assert.Throws<ArgumentException>(() => new NoComparer[] {new NoComparer(), new NoComparer(), new NoComparer() }.MinAndMax());
        }

        [Test]
        [Category("Extensions")]
        public void SkipIndex()
        {
            (int indexToSkip, IEnumerable<char> input, IEnumerable<char> expected)[] testCases =
            {
                (0, new char[] { }, new char[] { }),
                (0, new char[] { 'a' }, new char[] { }),
                (0, new char[] { 'a', 'e' }, new char[] { 'e' }),
                (1, new char[] { 'a', 'e' }, new char[] { 'a' }),
                (0, new char[] { 'a', 'e', 'i' }, new char[] { 'e', 'i' }),
                (1, new char[] { 'a', 'e', 'i' }, new char[] { 'a', 'i' }),
                (2, new char[] { 'a', 'e', 'i' }, new char[] { 'a', 'e' }),
            };

            foreach ((int indexToSkip, IEnumerable<char> input, IEnumerable<char> expected) in testCases)
            {
                CollectionAssert.AreEqual(expected, input.SkipIndex(indexToSkip));

                for (int index = -5; index < 0; index++)
                {
                    CollectionAssert.AreEqual(input, input.SkipIndex(index));
                }
                for (int index = input.Count() + 1; index <= input.Count() + 5; index++)
                {
                    CollectionAssert.AreEqual(input, input.SkipIndex(index));
                }
            }
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
        public void EnumerateOccurrences()
        {
            (IEnumerable<char> input, IEnumerable<(char, int)> expected)[] testCases =
            {
                (new char[] { }, new (char, int)[] { }),
                (new char[] { 'a', 'b', 'c' }, new (char, int)[] { ('a', 0), ('b', 0), ('c', 0) }),
                (new char[] { 'a', 'a', 'a' }, new (char, int)[] { ('a', 0), ('a', 1), ('a', 2) }),
                (new char[] { 'a', 'b', 'a', 'c', 'b', 'a' }, new (char, int)[] { ('a', 0), ('b', 0), ('a', 1), ('c', 0), ('b', 1), ('a', 2) })
            };

            foreach ((IEnumerable<char> input, IEnumerable<(char, int)> expected) in testCases)
            {
                CollectionAssert.AreEqual(expected, input.EnumerateOccurrences());
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

        [Test]
        [Category("Extensions")]
        public void Flatten()
        {
            (IEnumerable<IEnumerable<int>> input, IEnumerable<int> expected)[] testCases =
            {
                (new int[][] { }, new int[] { }),
                (new int[][] { new int[] { } }, new int[] { }),
                (new int[][] { new int[] { }, new int[] { } }, new int[] { }),
                /////
                (new int[][] { new int[] { 7 } }, new int[] { 7 }),
                (new int[][] { new int[] { 7 }, new int[] { 0 } }, new int[] { 7, 0 }),
                (new int[][] { new int[] { 2, -1 } }, new int[] { 2, -1 }),
                (new int[][] { new int[] { 0, 1, 2 }, new int[] { 3, 4 } }, new int[] { 0, 1, 2, 3, 4 }),
                (new int[][] { new int[] { 1, 2, 3 }, new int[] { 4, 5 }, new int[] { 6, 7, 8, 9, 10 } }, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }),
                (new int[][] { new int[] { 1, 2, 3 }, new int[] { 4, 5 }, new int[] { }, new int[] { 6, 7, 8, 9, 10 } }, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }),
                (new int[][] { new int[] { 3, 1, 4 }, new int[] { 1, 5 }, new int[] { 9, 2, 6, 5 } }, new int[] { 3, 1, 4, 1, 5, 9, 2, 6, 5 }),
                /////
                (new int[][] { new int[] { }, new int[] { 3, -2 } }, new int[] { 3, -2 }),
                (new int[][] { new int[] { 3, -2 }, new int[] { } }, new int[] { 3, -2 }),
                (new int[][] { new int[] { }, new int[] { 5, 4 }, new int[] { 3, 2, 1 } }, new int[] { 5, 4, 3, 2, 1 }),
                (new int[][] { new int[] { 5, 4 }, new int[] { }, new int[] { 3, 2, 1 } }, new int[] { 5, 4, 3, 2, 1 }),
                (new int[][] { new int[] { 5, 4 }, new int[] { 3, 2, 1 }, new int[] { } }, new int[] { 5, 4, 3, 2, 1 }),
                (new int[][] { new int[] { 5, 4 }, new int[] { }, new int[] { 3, 2, 1 }, new int[] { } }, new int[] { 5, 4, 3, 2, 1 }),
                (new int[][] { new int[] { 5, 4 }, new int[] { }, new int[] { }, new int[] { 3, 2, 1 } }, new int[] { 5, 4, 3, 2, 1 }),
            };
            
            /////

            foreach ((IEnumerable<IEnumerable<int>> input, IEnumerable<int> expected) in testCases)
            {
                CollectionAssert.AreEqual(expected, input.Flatten(), $"Failed with {{ {string.Join(", ", input.Select(s => $"{{ {string.Join(", ", s)} }}"))} }}");
            }

            Assert.Throws<ArgumentNullException>(() => IEnumerableExtensions.Flatten<int>(null).ToArray());
            Assert.Throws<ArgumentException>(() => new List<List<int>> { null }.Flatten().ToArray());
            Assert.Throws<ArgumentException>(() => new List<List<int>> { new List<int> { 0, 1 }, null }.Flatten().ToArray());
        }
    }
}
