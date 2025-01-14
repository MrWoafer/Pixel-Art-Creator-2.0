using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using PAC.Extensions;
using PAC.Maths;

namespace PAC.Tests
{
    public class Combinatorics_Tests
    {
        [Test]
        [Category("Maths")]
        public void Factorial()
        {
            Assert.AreEqual(1, Combinatorics.Factorial(0));
            Assert.AreEqual(1, Combinatorics.Factorial(1));
            Assert.AreEqual(2, Combinatorics.Factorial(2));
            Assert.AreEqual(6, Combinatorics.Factorial(3));
            Assert.AreEqual(24, Combinatorics.Factorial(4));
            Assert.AreEqual(120, Combinatorics.Factorial(5));
            Assert.AreEqual(720, Combinatorics.Factorial(6));
            Assert.AreEqual(5040, Combinatorics.Factorial(7));

            for (int i = -5; i < 0; i++)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => Combinatorics.Factorial(i));
            }

            Assert.Throws<OverflowException>(() => Combinatorics.Factorial(int.MaxValue));
        }

        [Test]
        [Category("Maths")]
        public void FallingFactorial()
        {
            for (int n = -10; n <= 10; n++)
            {
                Assert.AreEqual(1, Combinatorics.FallingFactorial(n, 0));
                Assert.AreEqual(n, Combinatorics.FallingFactorial(n, 1));
                Assert.AreEqual(n * (n - 1), Combinatorics.FallingFactorial(n, 2));
                Assert.AreEqual(n * (n - 1) * (n - 2), Combinatorics.FallingFactorial(n, 3));
                Assert.AreEqual(n * (n - 1) * (n - 2) * (n - 3), Combinatorics.FallingFactorial(n, 4));
            }
        }

        [Test]
        [Category("Maths")]
        public void RisingFactorial()
        {
            for (int n = -10; n <= 10; n++)
            {
                Assert.AreEqual(1, Combinatorics.RisingFactorial(n, 0));
                Assert.AreEqual(n, Combinatorics.RisingFactorial(n, 1));
                Assert.AreEqual(n * (n + 1), Combinatorics.RisingFactorial(n, 2));
                Assert.AreEqual(n * (n + 1) * (n + 2), Combinatorics.RisingFactorial(n, 3));
                Assert.AreEqual(n * (n + 1) * (n + 2) * (n + 3), Combinatorics.RisingFactorial(n, 4));
            }
        }

        [Test]
        [Category("Maths")]
        public void NumberOfTuples()
        {
            for (int lengthOfTuple = 0; lengthOfTuple <= 10; lengthOfTuple++)
            {
                Assert.AreEqual(0, Combinatorics.NumberOfTuples(0, lengthOfTuple));
            }

            for (int lengthOfTuple = 1; lengthOfTuple <= 10; lengthOfTuple++)
            {
                Assert.AreEqual(1, Combinatorics.NumberOfTuples(1, lengthOfTuple));
            }

            for (int n = 0; n <= 10; n++)
            {
                Assert.AreEqual(0, Combinatorics.NumberOfTuples(n, 0), $"Failed with {n}.");
                Assert.AreEqual(n, Combinatorics.NumberOfTuples(n, 1), $"Failed with {n}.");
                Assert.AreEqual(n * n, Combinatorics.NumberOfTuples(n, 2), $"Failed with {n}.");
                Assert.AreEqual(n * n * n, Combinatorics.NumberOfTuples(n, 3), $"Failed with {n}.");
                Assert.AreEqual(n * n * n * n, Combinatorics.NumberOfTuples(n, 4), $"Failed with {n}.");
            }

            // Check it's defined precisely when 0 <= numElementsToChooseFrom, lengthOfTuple
            for (int numElementsToChooseFrom = -5; numElementsToChooseFrom <= 5; numElementsToChooseFrom++)
            {
                for (int lengthOfTuple = -5; lengthOfTuple <= 5; lengthOfTuple++)
                {
                    if (0 <= numElementsToChooseFrom && 0 <= lengthOfTuple)
                    {
                        Assert.DoesNotThrow(() => Combinatorics.NumberOfTuples(numElementsToChooseFrom, lengthOfTuple));
                    }
                    else
                    {
                        Assert.Throws<ArgumentOutOfRangeException>(() => Combinatorics.NumberOfTuples(numElementsToChooseFrom, lengthOfTuple));
                    }
                }
            }
            // Check you can't generate tuples of negative length
            for (int numElementsToChooseFrom = 0; numElementsToChooseFrom <= 5; numElementsToChooseFrom++)
            {
                for (int lengthOfTuple = -5; lengthOfTuple < 0; lengthOfTuple++)
                {
                    Assert.Throws<ArgumentOutOfRangeException>(() => Combinatorics.NumberOfTuples(numElementsToChooseFrom, lengthOfTuple));
                }
            }

            Assert.DoesNotThrow(() => Combinatorics.NumberOfTuples(1, int.MaxValue));
            Assert.DoesNotThrow(() => Combinatorics.NumberOfTuples(int.MaxValue, 1));
            Assert.Throws<OverflowException>(() => Combinatorics.NumberOfTuples(2, int.MaxValue));
            Assert.Throws<OverflowException>(() => Combinatorics.NumberOfTuples(int.MaxValue, 2));
        }

        [Test]
        [Category("Maths")]
        public void NumberOfPermutations()
        {
            for (int n = 0; n <= 10; n++)
            {
                Assert.AreEqual(0, Combinatorics.NumberOfPermutations(n, 0), $"Failed with {n}.");
            }
            for (int n = 1; n <= 10; n++)
            {
                Assert.AreEqual(n, Combinatorics.NumberOfPermutations(n, 1), $"Failed with {n}.");
            }
            for (int n = 2; n <= 10; n++)
            {
                Assert.AreEqual(n * (n - 1), Combinatorics.NumberOfPermutations(n, 2), $"Failed with {n}.");
            }
            for (int n = 3; n <= 10; n++)
            {
                Assert.AreEqual(n * (n - 1) * (n - 2), Combinatorics.NumberOfPermutations(n, 3), $"Failed with {n}.");
            }
            for (int n = 4; n <= 10; n++)
            {
                Assert.AreEqual(n * (n - 1) * (n - 2) * (n - 3), Combinatorics.NumberOfPermutations(n, 4), $"Failed with {n}.");
            }

            // Check it's defined precisely when 0 <= lengthOfTuple <= numElementsToChooseFrom
            for (int numElementsToChooseFrom = -5; numElementsToChooseFrom <= 5; numElementsToChooseFrom++)
            {
                for (int lengthOfPermutation = -10; lengthOfPermutation <= numElementsToChooseFrom + 5; lengthOfPermutation++)
                {
                    if (0 <= lengthOfPermutation && lengthOfPermutation <= numElementsToChooseFrom)
                    {
                        Assert.DoesNotThrow(() => Combinatorics.NumberOfPermutations(numElementsToChooseFrom, lengthOfPermutation));
                    }
                    else
                    {
                        Assert.Throws<ArgumentOutOfRangeException>(() => Combinatorics.NumberOfPermutations(numElementsToChooseFrom, lengthOfPermutation));
                    }
                }
            }

            Assert.DoesNotThrow(() => Combinatorics.NumberOfPermutations(int.MaxValue, 1));
            Assert.Throws<OverflowException>(() => Combinatorics.NumberOfPermutations(int.MaxValue, 2));
        }

        [Test]
        [Category("Maths")]
        public void Choose()
        {
            for (int n = 0; n <= 10; n++)
            {
                Assert.AreEqual(1, Combinatorics.Choose(n, 0));
            }
            for (int n = 1; n <= 10; n++)
            {
                Assert.AreEqual(n, Combinatorics.Choose(n, 1));
            }
            for (int n = 2; n <= 10; n++)
            {
                Assert.AreEqual((n * (n - 1)) / 2, Combinatorics.Choose(n, 2));
            }
            for (int n = 3; n <= 10; n++)
            {
                Assert.AreEqual((n * (n - 1) * (n - 2)) / 6, Combinatorics.Choose(n, 3));
            }
            for (int n = 4; n <= 10; n++)
            {
                Assert.AreEqual((n * (n - 1) * (n - 2) * (n - 3)) / 24, Combinatorics.Choose(n, 4));
            }
            for (int n = 5; n <= 10; n++)
            {
                Assert.AreEqual((n * (n - 1) * (n - 2) * (n - 3) * (n - 4)) / 120, Combinatorics.Choose(n, 5));
            }

            // Test symmetry property
            for (int n = 0; n <= 10; n++)
            {
                for (int k = 0; k <= n; k++)
                {
                    Assert.AreEqual(Combinatorics.Choose(n, k), Combinatorics.Choose(n, n - k), $"Failed with n = {n}, k = {k}");
                }
            }

            // Test sum property
            for (int n = 0; n <= 10; n++)
            {
                int sumOfChooseFunctions = 0;
                for (int k = 0; k <= n; k++)
                {
                    sumOfChooseFunctions += Combinatorics.Choose(n, k);
                }

                // expected = 2^n
                int expected = 1;
                for (int _ = 0; _ < n; _++)
                {
                    expected *= 2;
                }

                Assert.AreEqual(expected, sumOfChooseFunctions, $"Failed with n = {n}");
            }

            // Check it's defined precisely when 0 <= k <= n
            for (int n = -5; n <= 10; n++)
            {
                for (int k = -10; k < 0; k++)
                {
                    if (0 <= k && k <= n)
                    {
                        Assert.DoesNotThrow(() => Combinatorics.Choose(n, k));
                    }
                    else
                    {
                        Assert.Throws<ArgumentOutOfRangeException>(() => Combinatorics.Choose(n, k));
                    }
                }
            }
        }

        [Test]
        [Category("Maths")]
        public void Pairs()
        {
            (IEnumerable<int> set, IEnumerable<(int, int)> expected)[] testCases =
            {
                (new int[] { }, new (int, int)[] { }),
                (new int[] { 0 }, new (int, int)[] { (0, 0) }),
                (new int[] { 5, 2 }, new (int, int)[] { (5, 5), (5, 2), (2, 5), (2, 2) }),
                (new int[] { -1, 19, 4 }, new (int, int)[] { (-1, -1), (-1, 19), (-1, 4), (19, -1), (19, 19), (19, 4), (4, -1), (4, 19), (4, 4) }),
            };

            /////
            
            foreach ((IEnumerable<int> set, IEnumerable<(int, int)> expected) in testCases)
            {
                CollectionAssert.AreEqual(expected, set.Pairs());
                CollectionAssert.AreEqual(set.Tuples(2).Select(pair => (pair.ElementAt(0), pair.ElementAt(1))), set.Pairs());
            }
        }

        [Test]
        [Category("Maths")]
        public void Triples()
        {
            List<(IEnumerable<int> set, IEnumerable<(int, int, int)> expected)> testCases = new List<(IEnumerable<int>, IEnumerable<(int, int, int)>)>
            {
                (new int[] { }, new (int, int, int)[] { }),
                (new int[] { 0 }, new (int, int, int)[] { (0, 0, 0) }),
                (new int[] { 5, 2 }, new (int, int, int)[] { (5, 5, 5), (5, 5, 2), (5, 2, 5), (5, 2, 2), (2, 5, 5), (2, 5, 2), (2, 2, 5), (2, 2, 2) }),
            };

            int[] exampleSet = new int[] { 17, 3, -2, 6, -100, -44 };
            List<(int, int, int)> exampleExpected = new List<(int, int, int)>();
            foreach ((int first, int second) in exampleSet.Pairs())
            {
                foreach (int third in exampleSet)
                {
                    exampleExpected.Add((first, second, third));
                }
            }
            testCases.Add((exampleSet, exampleExpected));

            /////

            foreach ((IEnumerable<int> set, IEnumerable<(int, int, int)> expected) in testCases)
            {
                CollectionAssert.AreEqual(expected, set.Triples());
                CollectionAssert.AreEqual(set.Tuples(3).Select(pair => (pair.ElementAt(0), pair.ElementAt(1), pair.ElementAt(2))), set.Triples());
            }
        }

        [Test]
        [Category("Maths")]
        public void Tuples()
        {
            List<(int length, IEnumerable<int> set, IEnumerable<IEnumerable<int>> expected)> testCases = new List<(int, IEnumerable<int>, IEnumerable<IEnumerable<int>>)>
            {
                (0, new int[] { }, new int[][] { }),
                (1, new int[] { }, new int[][] { }),
                (2, new int[] { }, new int[][] { }),
                /////
                (2, new int[] { 0, 1 }, new int[][] { new int[] { 0, 0 }, new int[] { 0, 1 }, new int[] { 1, 0 }, new int[] { 1, 1 } }),
                /////
                (0, new int[] { 5, 1, 3 }, new int[][] { }),
                (1, new int[] { 5, 1, 3 }, new int[][] { new int[] { 5 }, new int[] { 1 }, new int[] { 3 } }),
                (2, new int[] { 5, 1, 3 }, new int[][] {
                    new int[] { 5, 5 }, new int[] { 5, 1 }, new int[] { 5, 3 }, new int[] { 1, 5 }, new int[] { 1, 1 }, new int[] { 1, 3 }, new int[] { 3, 5 }, new int[] { 3, 1 }, new int[] { 3, 3 }
                }),
            };
            
            int[] exampleSet = new int[] { 2, 3, 5, 7, 2 };
            // Key = n; Value = expected sequence of n tuples
            Dictionary<int, List<IEnumerable<int>>> expectedNTuples = new Dictionary<int, List<IEnumerable<int>>>()
            {
                { 0, new List<IEnumerable<int>>() },
                { 1, new List<IEnumerable<int>>(exampleSet.Select(x => IEnumerableExtensions.Singleton(x))) }
            };

            for (int n = 2; n <= 8; n++)
            {
                expectedNTuples[n] = new List<IEnumerable<int>>();
                foreach (IEnumerable<int> tuple in expectedNTuples[n - 1])
                {
                    foreach (int element in exampleSet)
                    {
                        expectedNTuples[n].Add(tuple.Append(element));
                    }
                }
            }

            foreach (int n in expectedNTuples.Keys)
            {
                testCases.Add((n, exampleSet, expectedNTuples[n]));
            }

            /////

            foreach ((int length, IEnumerable<int> set, IEnumerable<IEnumerable<int>> expected) in testCases)
            {
                CollectionAssert.AreEqual(expected, set.Tuples(length));

                Assert.AreEqual(Combinatorics.NumberOfTuples(set.Count(), length), set.Tuples(length).Count());
                foreach (IEnumerable<int> tuple in set.Tuples(length))
                {
                    Assert.AreEqual(length, tuple.Count());
                }
            }

            for (int n = -5; n < 0; n++)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => exampleSet.Tuples(n).ToList(), $"Failed with n = {n}");
            }
        }

        [Test]
        [Category("Maths")]
        public void Permutations()
        {
            List<(int length, IEnumerable<int> set, IEnumerable<IEnumerable<int>> expected)> testCases = new List<(int, IEnumerable<int>, IEnumerable<IEnumerable<int>>)>
            {
                (0, new int[] { }, new int[][] { }),
                /////
                (0, new int[] { 5, 1, 3 }, new int[][] { }),
                (1, new int[] { 5, 1, 3 }, new int[][] { new int[] { 5 }, new int[] { 1 }, new int[] { 3 } }),
                (2, new int[] { 5, 1, 3 }, new int[][] { new int[] { 5, 1 }, new int[] { 5, 3 }, new int[] { 1, 5 }, new int[] { 1, 3 }, new int[] { 3, 5 }, new int[] { 3, 1 } }),
                (3, new int[] { 5, 1, 3 }, new int[][] { new int[] { 5, 1, 3 }, new int[] { 5, 3, 1 }, new int[] { 1, 5, 3 }, new int[] { 1, 3, 5 }, new int[] { 3, 5, 1 }, new int[] { 3, 1, 5 } }),
            };

            int[] exampleSet = new int[] { 2, 3, 5, 7, 2 };
            for (int n = 0; n <= exampleSet.Length; n++)
            {
                // The EnumerateOccurrences() and subsequent Select(pair => pair.element) is to ensure we treat duplicate elements as distinct elements
                IEnumerable<IEnumerable<int>> expectedPermutations = exampleSet.EnumerateOccurrences().Tuples(n).Where(x => x.AreAllDistinct()).Select(p => p.Select(pair => pair.element));
                testCases.Add((n, exampleSet, expectedPermutations));
            }

            /////

            foreach ((int length, IEnumerable<int> set, IEnumerable<IEnumerable<int>> expected) in testCases)
            {
                CollectionAssert.AreEquivalent(expected, set.Permutations(length));

                Assert.AreEqual(Combinatorics.NumberOfPermutations(set.Count(), length), set.Permutations(length).Count());
                foreach (IEnumerable<int> permutation in set.Permutations(length))
                {
                    Assert.AreEqual(length, permutation.Count());
                }
            }

            CollectionAssert.AreEquivalent(Enumerable.Empty<IEnumerable<int>>(), Enumerable.Empty<int>().Permutations());

            Assert.AreEqual(Combinatorics.NumberOfPermutations(exampleSet.Length), exampleSet.Permutations().Count());
            CollectionAssert.AreEquivalent(exampleSet.Permutations(exampleSet.Length), exampleSet.Permutations());

            for (int n = -5; n < 0; n++)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => exampleSet.Permutations(n).ToList(), $"Failed with n = {n}");
            }
            for (int n = exampleSet.Length + 1; n <= exampleSet.Length + 5; n++)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => exampleSet.Permutations(n).ToList(), $"Failed with n = {n}");
            }
        }
    }
}
