using System;
using System.Collections.Generic;
using System.Linq;

using PAC.Extensions;

namespace PAC.Maths
{
    /// <summary>
    /// A collection of common combinatorial functions, such as computing factorials or generating all permutations of a set.
    /// </summary>
    public static class Combinatorics
    {
        /// <summary>
        /// Computes <c><paramref name="n"/> * (<paramref name="n"/> - 1) * ... * 2 * 1</c>.
        /// Returns 1 if <paramref name="n"/> is 0.
        /// </summary>
        public static int Factorial(int n)
        {
            if (n < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(n), $"Factorial is only defined for non-negative integers: {n}.");
            }

            int product = 1;
            for (int i = 2; i <= n; i++)
            {
                product = checked(product * i);
            }
            return product;
        }

        /// <summary>
        /// Computes <c><paramref name="n"/> * (<paramref name="n"/> - 1) * ... * (<paramref name="n"/> - (<paramref name="k"/> - 1))</c>.
        /// Returns 1 if <paramref name="k"/> is 0.
        /// </summary>
        public static int FallingFactorial(int n, int k)
        {
            if (k < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(k), $"Falling factorial is only defined for non-negative {nameof(k)}: {k}.");
            }

            int product = 1;
            for (int i = 0; i < k; i++)
            {
                product = checked(product * (n - i));
            }
            return product;
        }

        /// <summary>
        /// Computes <c><paramref name="n"/> * (<paramref name="n"/> + 1) * ... * (<paramref name="n"/> + (<paramref name="k"/> - 1))</c>.
        /// Returns 1 if <paramref name="k"/> is 0.
        /// </summary>
        public static int RisingFactorial(int n, int k)
        {
            if (k < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(k), $"Rising factorial is only defined for non-negative {nameof(k)}: {k}.");
            }

            int product = 1;
            for (int i = 0; i < k; i++)
            {
                product = checked(product * (n + i));
            }
            return product;
        }

        /// <summary>
        /// The number of possible tuples of the given length with elements in a set of the given size. (Tuples allow repeated elements.)
        /// </summary>
        public static int NumberOfTuples(int numElementsToChooseFrom, int lengthOfTuple)
        {
            if (numElementsToChooseFrom < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numElementsToChooseFrom), $"Sets cannot have a negative size: {numElementsToChooseFrom}.");
            }
            if (lengthOfTuple < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lengthOfTuple), $"Tuples cannot have a negative length: {lengthOfTuple}.");
            }

            return lengthOfTuple == 0 ? 0 : numElementsToChooseFrom.Pow(lengthOfTuple);
        }

        /// <summary>
        /// The number of possible permutations of a set of the given size. (Permutations do not allow repeated elements.)
        /// </summary>
        public static int NumberOfPermutations(int sizeOfSet) => NumberOfPermutations(sizeOfSet, sizeOfSet);
        /// <summary>
        /// The number of possible permutations of the given length with elements in a set of the given size. (Permutations do not allow repeated elements.)
        /// </summary>
        public static int NumberOfPermutations(int numElementsToChooseFrom, int lengthOfPermutation)
        {
            if (numElementsToChooseFrom < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numElementsToChooseFrom), $"Sets cannot have a negative size: {numElementsToChooseFrom}.");
            }
            if (lengthOfPermutation < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lengthOfPermutation), $"Permutations cannot have a negative length: {lengthOfPermutation}.");
            }
            if (lengthOfPermutation > numElementsToChooseFrom)
            {
                throw new ArgumentOutOfRangeException($"Must have {nameof(numElementsToChooseFrom)} >= {nameof(lengthOfPermutation)}. {nameof(numElementsToChooseFrom)}: {numElementsToChooseFrom} .{nameof(lengthOfPermutation)}: {lengthOfPermutation}.");
            }

            return lengthOfPermutation == 0 ? 0 : FallingFactorial(numElementsToChooseFrom, lengthOfPermutation);
        }

        /// <summary>
        /// The choose function. An alias for <see cref="BinomialCoefficient(int, int)(int, int)"/>.
        /// </summary>
        /// <seealso cref="BinomialCoefficient(int, int)"/>
        public static int Choose(int n, int k)
        {
            if (n < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(n), $"{nameof(n)} must be non-negative: {n}.");
            }
            if (k < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(k), $"{nameof(k)} must be non-negative: {k}.");
            }
            if (k > n)
            {
                throw new ArgumentOutOfRangeException(nameof(k), $"Must have {nameof(k)} <= {nameof(n)}. {nameof(k)}: {k}. {nameof(n)}: {n}.");
            }

            return FallingFactorial(n, k) / Factorial(k);
        }
        /// <summary>
        /// The coefficient of <c>x^k</c> in <c>(x + 1)^n</c>. An alias for <see cref="Choose(int, int)"/>.
        /// </summary>
        /// <seealso cref="Choose(int, int)"/>
        public static int BinomialCoefficient(int n, int k) => Choose(n, k);

        public static int NumberOfCombinations(int sizeOfSet) => NumberOfCombinations(sizeOfSet, sizeOfSet);
        public static int NumberOfCombinations(int numElementsToChooseFrom, int lengthOfCombination)
        {
            if (numElementsToChooseFrom < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numElementsToChooseFrom), $"{nameof(numElementsToChooseFrom)} must be non-negative: {numElementsToChooseFrom}.");
            }
            if (lengthOfCombination < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(lengthOfCombination), $"{nameof(lengthOfCombination)} must be non-negative: {lengthOfCombination}.");
            }
            if (lengthOfCombination > numElementsToChooseFrom)
            {
                throw new ArgumentOutOfRangeException(nameof(lengthOfCombination), $"Must have {nameof(lengthOfCombination)} <= {nameof(numElementsToChooseFrom)}. {nameof(lengthOfCombination)}: " +
                    $"{lengthOfCombination}. {nameof(numElementsToChooseFrom)}: {numElementsToChooseFrom}.");
            }

            return lengthOfCombination == 0 ? 0 : Choose(numElementsToChooseFrom, lengthOfCombination);
        }

        public static int MultiChoose(int n, int k) => Choose(n + k - 1, k);

        public static int NumberOfMultisets(int sizeOfSet) => NumberOfMultisets(sizeOfSet, sizeOfSet);
        public static int NumberOfMultisets(int numElementsToChooseFrom, int sizeOfMultiset)
        {
            if (numElementsToChooseFrom < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numElementsToChooseFrom), $"{nameof(numElementsToChooseFrom)} must be non-negative: {numElementsToChooseFrom}.");
            }
            if (sizeOfMultiset < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sizeOfMultiset), $"{nameof(sizeOfMultiset)} must be non-negative: {sizeOfMultiset}.");
            }

            return sizeOfMultiset == 0 ? 0 : MultiChoose(numElementsToChooseFrom, sizeOfMultiset);
        }

        /// <summary>
        /// Lazily generates all possible pairs <c>(x, y)</c> with <c>x</c> and <c>y</c> in the given <see cref="IEnumerable{T}"/>.
        /// <c>x</c> and <c>y</c> don't have to be distinct.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the given <see cref="IEnumerable{T}"/> has duplicate elements, those elements will be treated as though they were distinct.
        /// </para>
        /// <para>
        /// For a description of the order in which the pairs are yielded, see <see cref="Tuples{T}(IEnumerable{T}, int)"/>.
        /// </para>
        /// </remarks>
        public static IEnumerable<(T first, T second)> Pairs<T>(this IEnumerable<T> set) => Pairs(set, set);
        /// <summary>
        /// Lazily generates all possible pairs <c>(x, y)</c> with <c>x</c> in <paramref name="first"/> and <c>y</c> in <paramref name="second"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If a given <see cref="IEnumerable{T}"/> has duplicate elements, those elements will be treated as though they were distinct.
        /// </para>
        /// <para>
        /// For a description of the order in which the pairs are yielded, see <see cref="Tuples{T}(IEnumerable{T}, int)"/>.
        /// </para>
        /// </remarks>
        public static IEnumerable<(TFirst first, TSecond second)> Pairs<TFirst, TSecond>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second)
        {
            if (first is null)
            {
                throw new ArgumentNullException(nameof(first), $"The first given set of elements is null.");
            }
            if (second is null)
            {
                throw new ArgumentNullException(nameof(second), $"The second given set of elements is null.");
            }

            // Optimisation - otherwise, if first is non-empty but second is empty, then we would still have to iterate over all of first
            if (second.IsEmpty())
            {
                yield break;
            }

            foreach (TFirst firstElement in first)
            {
                foreach (TSecond secondElement in second)
                {
                    yield return (firstElement, secondElement);
                }
            }
        }

        /// <summary>
        /// Lazily generates all possible triples <c>(x, y, z)</c> with <c>x</c>, <c>y</c> and <c>z</c> in the given <see cref="IEnumerable{T}"/>.
        /// <c>x</c>, <c>y</c> and <c>z</c> don't have to all be distinct.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the given <see cref="IEnumerable{T}"/> has duplicate elements, those elements will be treated as though they were distinct.
        /// </para>
        /// <para>
        /// For a description of the order in which the triples are yielded, see <see cref="Tuples{T}(IEnumerable{T}, int)"/>.
        /// </para>
        /// </remarks>
        public static IEnumerable<(T first, T second, T third)> Triples<T>(this IEnumerable<T> set) => Triples(set, set, set);
        /// <summary>
        /// Lazily generates all possible triples <c>(x, y, z)</c> with <c>x</c> in <paramref name="first"/>, <c>y</c> in <paramref name="second"/> and <c>z</c> in <paramref name="third"/>.
        /// </summary>
        /// <remarks>
        /// <para>
        /// If a given <see cref="IEnumerable{T}"/> has duplicate elements, those elements will be treated as though they were distinct.
        /// </para>
        /// <para>
        /// For a description of the order in which the triples are yielded, see <see cref="Tuples{T}(IEnumerable{T}, int)"/>.
        /// </para>
        /// </remarks>
        public static IEnumerable<(TFirst first, TSecond second, TThird third)> Triples<TFirst, TSecond, TThird>(this IEnumerable<TFirst> first, IEnumerable<TSecond> second, IEnumerable<TThird> third)
        {
            if (first is null)
            {
                throw new ArgumentNullException(nameof(first), $"The first given set of elements is null.");
            }
            if (second is null)
            {
                throw new ArgumentNullException(nameof(second), $"The second given set of elements is null.");
            }
            if (third is null)
            {
                throw new ArgumentNullException(nameof(third), $"The third given set of elements is null.");
            }

            // Optimisation - otherwise, if first is non-empty but second and/or third are empty, then we would still have to iterate over all of first and potentially second
            if (second.IsEmpty() || third.IsEmpty())
            {
                yield break;
            }

            foreach (TFirst firstElement in first)
            {
                foreach (TSecond secondElement in second)
                {
                    foreach (TThird thirdElement in third)
                    {
                        yield return (firstElement, secondElement, thirdElement);
                    }
                }
            }
        }

        /// <summary>
        /// Lazily generates all possible tuples of the given length with elements in the given <see cref="IEnumerable{T}"/>. (Tuples allow repeated elements.)
        /// </summary>
        /// <remarks>
        /// <para>
        /// If the given <see cref="IEnumerable{T}"/> has duplicate elements, they will be treated as though they were distinct.
        /// </para>
        /// <para>
        /// The order of the tuples will be lexicographic based on the order in which the elements appear in the input, with the left-most coordinate being the most significant. This is equivalent
        /// to using nested <see langword="foreach"/> loops, with a loop being less nested corresponding to a further-left coordinate.
        /// </para>
        /// <example>
        /// For example, the tuples of length 2 with elements in <c>{ 0, 1 }</c> will be in the order <c>{ { 0, 0 }, { 0, 1 }, { 1, 0 }, { 1, 1 } }.</c> This is equivalent to
        /// <code language="csharp">
        /// foreach (int element1 in set)
        /// {
        ///     foreach (int element2 in set)
        ///     {
        ///         yield return new int[] { element1, element2 };
        ///     }
        /// }
        /// </code>
        /// </example>
        /// </remarks>
        public static IEnumerable<IEnumerable<T>> Tuples<T>(this IEnumerable<T> elements, int length)
        {
            if (elements is null)
            {
                throw new ArgumentNullException(nameof(elements), $"The given set of elements is null.");
            }
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), $"Cannot generate tuples of negative length: {length}.");
            }
            if (elements.IsEmpty())
            {
                yield break;
            }
            if (length == 0)
            {
                yield break;
            }
            if (length == 1)
            {
                foreach (T element in elements)
                {
                    yield return IEnumerableExtensions.Singleton(element);
                }
                yield break;
            }

            foreach (T element in elements)
            {
                foreach (IEnumerable<T> tuple in Tuples(elements, length - 1))
                {
                    yield return tuple.Prepend(element);
                }
            }
        }

        /// <summary>
        /// Lazily generates all possible permutations of the elements in the given <see cref="IEnumerable{T}"/>. (Permutations do not allow repeated elements.)
        /// </summary>
        /// <remarks>
        /// If the given <see cref="IEnumerable{T}"/> has duplicate elements, they will be treated as though they were distinct.
        /// </remarks>
        public static IEnumerable<IEnumerable<T>> Permutations<T>(this IEnumerable<T> elements)
        {
            static IEnumerable<IEnumerable<T>> PermutationsNonEmptyInput(IEnumerable<T> elements)
            {
                if (elements.IsEmpty())
                {
                    yield return Enumerable.Empty<T>();
                    yield break;
                }

                foreach ((T element, int index) in elements.Enumerate())
                {
                    foreach (IEnumerable<T> permutation in Permutations(elements.SkipIndex(index)))
                    {
                        yield return permutation.Prepend(element);
                    }
                }
            }

            if (elements is null)
            {
                throw new ArgumentNullException(nameof(elements), $"The given set of elements is null.");
            }

            if (elements.IsEmpty())
            {
                yield break;
            }

            foreach ((T element, int index) in elements.Enumerate())
            {
                foreach (IEnumerable<T> permutation in PermutationsNonEmptyInput(elements.SkipIndex(index)))
                {
                    yield return permutation.Prepend(element);
                }
            }
        }
        /// <summary>
        /// Lazily generates all possible permutations of the given length with elements in the given <see cref="IEnumerable{T}"/>. (Permutations do not allow repeated elements.)
        /// </summary>
        /// <remarks>
        /// If the given <see cref="IEnumerable{T}"/> has duplicate elements, they will be treated as though they were distinct.
        /// </remarks>
        public static IEnumerable<IEnumerable<T>> Permutations<T>(this IEnumerable<T> elements, int length)
        {
            if (elements is null)
            {
                throw new ArgumentNullException(nameof(elements), $"The given set of elements is null.");
            }
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), $"Cannot generate permutations of negative length: {length}.");
            }
            if (length == 0)
            {
                yield break;
            }
            if (elements.IsEmpty())
            {
                throw new ArgumentOutOfRangeException(nameof(length), $"Ran out of elements to pick from. Still have {length} left to choose.");
            }
            if (length == 1)
            {
                foreach (T element in elements)
                {
                    yield return IEnumerableExtensions.Singleton(element);
                }
                yield break;
            }

            foreach ((T element, int index) in elements.Enumerate())
            {
                foreach (IEnumerable<T> permutation in Permutations(elements.SkipIndex(index), length - 1))
                {
                    yield return permutation.Prepend(element);
                }
            }
        }
    }
}
