using System;
using System.Collections.Generic;
using System.Linq;

using PAC.Extensions.System.Collections;

namespace PAC.Extensions.System.Collections
{
    /// <summary>
    /// Extension methods for <see cref="IEnumerable{T}"/>.
    /// </summary>
    public static class IEnumerableExtensions
    {
        private static ArgumentNullException NullIEnumerableException<T>(string paramName) => new ArgumentNullException(paramName, $"The given IEnumerable<{typeof(T).Name}> is null.");
        private static ArgumentException EmptyIEnumerableException<T>(string paramName) => new ArgumentException($"The given IEnumerable<{typeof(T).Name}> is empty.", paramName);

        /// <summary>
        /// Creates an <see cref="IEnumerable{T}"/> with only one element.
        /// </summary>
        public static IEnumerable<T> Singleton<T>(T element) { yield return element; }

        /// <summary>
        /// Creates an <see cref="IEnumerable{T}"/> that repeats the given element indefinitely.
        /// </summary>
        public static IEnumerable<T> Repeat<T>(T element)
        {
            while (true)
            {
                yield return element;
            }
        }

        /// <summary>
        /// Creates an <see cref="IEnumerable{T}"/> that repeats the given <see cref="IEnumerable{T}"/> indefinitely, looping back round to the beginning when it reaches the end.
        /// </summary>
        public static IEnumerable<T> RepeatIEnumerable<T>(IEnumerable<T> elements)
        {
            if (elements is null)
            {
                throw NullIEnumerableException<T>(nameof(elements));
            }

            while (true)
            {
                foreach (T element in elements)
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// Creates a new <see cref="IEnumerable{T}"/> that iterates through all of this one, then all of second, etc. Extends <see cref="Enumerable.Concat{T}(IEnumerable{T}, IEnumerable{T})"/> to
        /// allow concating multiple <see cref="IEnumerable{T}"/>s.
        /// </summary>
        public static IEnumerable<T> Concat<T>(this IEnumerable<T> first, IEnumerable<T> second, IEnumerable<T> third, params IEnumerable<T>[] subsequent)
        {
            foreach (T element in first)
            {
                yield return element;
            }
            foreach (T element in second)
            {
                yield return element;
            }
            foreach (T element in third)
            {
                yield return element;
            }
            foreach (IEnumerable<T> ienumerable in subsequent)
            {
                foreach (T element in ienumerable)
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// Replaces all occurrences of <paramref name="toReplace"/> with <paramref name="replaceWith"/>. Uses the default equality comparer for type <typeparamref name="T"/>.
        /// </summary>
        public static IEnumerable<T> Replace<T>(this IEnumerable<T> elements, T toReplace, T replaceWith)
        {
            EqualityComparer<T> comparer = EqualityComparer<T>.Default;
            return elements.Select(x => comparer.Equals(x, toReplace) ? replaceWith : x);
        }

        /// <summary>
        /// Returns whether the given <see cref="IEnumerable{T}"/> has exactly <paramref name="n"/> elements.
        /// </summary>
        /// <remarks>
        /// Iterates through at most <paramref name="n"/> + 1 elements of the <see cref="IEnumerable{T}"/>.
        /// </remarks>
        public static bool CountIsExactly<T>(this IEnumerable<T> elements, int n)
        {
            if (elements is null)
            {
                throw NullIEnumerableException<T>(nameof(elements));
            }

            if (n < 0)
            {
                return false;
            }

            int count = 0;
            foreach (T element in elements)
            {
                count++;
                if (count > n)
                {
                    return false;
                }
            }
            return count == n;
        }
        /// <summary>
        /// Returns whether the given <see cref="IEnumerable{T}"/> has &gt;= <paramref name="n"/> elements.
        /// </summary>
        /// <remarks>
        /// Iterates through at most <paramref name="n"/> elements of the <see cref="IEnumerable{T}"/>.
        /// </remarks>
        public static bool CountIsAtLeast<T>(this IEnumerable<T> elements, int n)
        {
            if (elements is null)
            {
                throw NullIEnumerableException<T>(nameof(elements));
            }

            if (n <= 0)
            {
                return true;
            }

            int count = 0;
            foreach (T element in elements)
            {
                count++;
                if (count >= n)
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// Returns whether the given <see cref="IEnumerable{T}"/> has &lt;= <paramref name="n"/> elements.
        /// </summary>
        /// <remarks>
        /// Iterates through at most <paramref name="n"/> + 1 elements of the <see cref="IEnumerable{T}"/>.
        /// </remarks>
        public static bool CountIsAtMost<T>(this IEnumerable<T> elements, int n)
        {
            if (elements is null)
            {
                throw NullIEnumerableException<T>(nameof(elements));
            }

            if (n < 0)
            {
                return false;
            }

            int count = 0;
            foreach (T element in elements)
            {
                count++;
                if (count > n)
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns whether all the elements in the <see cref="IEnumerable{T}"/> are distinct. Uses the default equality comparer for type <typeparamref name="T"/>.
        /// </summary>
        public static bool AreAllDistinct<T>(this IEnumerable<T> elements)
        {
            HashSet<T> visited = new HashSet<T>();
            foreach (T element in elements)
            {
                if (!visited.Add(element))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Returns true iff at least one of the elements doesn't satisfy the predicate.
        /// </summary>
        public static bool NotAll<T>(this IEnumerable<T> elements, Func<T, bool> predicate) => !elements.All(predicate);
        /// <summary>
        /// Returns true iff the given sequence is empty.
        /// </summary>
        public static bool None<T>(this IEnumerable<T> elements) => !elements.Any();
        /// <summary>
        /// Returns true iff none of the elements satisfy the predicate.
        /// </summary>
        public static bool None<T>(this IEnumerable<T> elements, Func<T, bool> predicate) => !elements.Any(predicate);

        /// <summary>
        /// Whether all the given elements are in the <see cref="IEnumerable{T}"/>.
        /// </summary>
        public static bool ContainsAll<T>(this IEnumerable<T> iEnumerable, params T[] elements) => iEnumerable.ContainsAll((IEnumerable<T>)elements);
        /// <summary>
        /// Whether all the given elements are in the <see cref="ICollection{T}"/>.
        /// </summary>
        public static bool ContainsAll<T>(this IEnumerable<T> iEnumerable, IEnumerable<T> elements) => elements.All(x => iEnumerable.Contains(x));

        /// <summary>
        /// Whether at least one of the given elements is not in the <see cref="IEnumerable{T}"/>.
        /// </summary>
        public static bool ContainsNotAll<T>(this IEnumerable<T> iEnumerable, params T[] elements) => iEnumerable.ContainsNotAll((IEnumerable<T>)elements);
        /// <summary>
        /// Whether at least one of the given elements is not in the <see cref="ICollection{T}"/>.
        /// </summary>
        public static bool ContainsNotAll<T>(this IEnumerable<T> iEnumerable, IEnumerable<T> elements) => !elements.All(x => iEnumerable.Contains(x));

        /// <summary>
        /// Whether any of the given elements are in the <see cref="IEnumerable{T}"/>.
        /// </summary>
        public static bool ContainsAny<T>(this IEnumerable<T> iEnumerable, params T[] elements) => iEnumerable.ContainsAny((IEnumerable<T>)elements);
        /// <summary>
        /// Whether any of the given elements are in the <see cref="IEnumerable{T}"/>.
        /// </summary>
        public static bool ContainsAny<T>(this IEnumerable<T> iEnumerable, IEnumerable<T> elements) => elements.Any(x => iEnumerable.Contains(x));

        /// <summary>
        /// Whether none of the given elements are in the <see cref="IEnumerable{T}"/>.
        /// </summary>
        public static bool ContainsNone<T>(this IEnumerable<T> iEnumerable, params T[] elements) => iEnumerable.ContainsNone((IEnumerable<T>)elements);
        /// <summary>
        /// Whether none of the given elements are in the <see cref="IEnumerable{T}"/>.
        /// </summary>
        public static bool ContainsNone<T>(this IEnumerable<T> iEnumerable, IEnumerable<T> elements) => !elements.Any(x => iEnumerable.Contains(x));

        /// <summary>
        /// Returns whether this <see cref="IEnumerable{T}"/> is a subsequence of <paramref name="otherSequence"/>; that is, <paramref name="otherSequence"/> is this <see cref="IEnumerable{T}"/>
        /// with potentially extra elements added anywhere in the sequence.
        /// </summary>
        /// <seealso cref="IsSupersequenceOf{T}(IEnumerable{T}, IEnumerable{T})"/>
        public static bool IsSubsequenceOf<T>(this IEnumerable<T> sequence, IEnumerable<T> otherSequence)
        {
            if (sequence is null)
            {
                throw NullIEnumerableException<T>(nameof(sequence));
            }
            if (otherSequence is null)
            {
                throw NullIEnumerableException<T>(nameof(otherSequence));
            }

            using (IEnumerator<T> enumerator = sequence.GetEnumerator())
            {
                // Check if sequence is empty
                if (!enumerator.MoveNext())
                {
                    return true;
                }

                EqualityComparer<T> comparer = EqualityComparer<T>.Default;

                foreach (T otherElement in otherSequence)
                {
                    if (comparer.Equals(enumerator.Current, otherElement))
                    {
                        // Check if sequence has ended
                        if (!enumerator.MoveNext())
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
        }
        /// <summary>
        /// Returns whether this <see cref="IEnumerable{T}"/> is a supersequence of <paramref name="otherSequence"/>; that is, this <see cref="IEnumerable{T}"/> is <paramref name="otherSequence"/> 
        /// with potentially extra elements added anywhere in the sequence.
        /// </summary>
        /// <seealso cref="IsSubsequenceOf{T}(IEnumerable{T}, IEnumerable{T})"/>
        public static bool IsSupersequenceOf<T>(this IEnumerable<T> sequence, IEnumerable<T> otherSequence) => otherSequence.IsSubsequenceOf(sequence);

        /// <summary>
        /// Applies the function to each element and returns the element that gives the lowest output. If multiple elements give the lowest output, the first one will be returned.
        /// </summary>
        public static TElement ArgMin<TElement, TCompare>(this IEnumerable<TElement> elements, Func<TElement, TCompare> function) where TCompare : IComparable<TCompare>
        {
            if (elements is null)
            {
                throw NullIEnumerableException<TElement>(nameof(elements));
            }
            if (elements.None())
            {
                throw EmptyIEnumerableException<TElement>(nameof(elements));
            }

            (TElement element, TCompare value) min = (elements.First(), function.Invoke(elements.First()));
            foreach (TElement element in elements.Skip(1))
            {
                TCompare value = function.Invoke(element);
                if (value.CompareTo(min.value) < 0)
                {
                    min = (element, value);
                }
            }

            return min.element;
        }
        /// <summary>
        /// Applies the function to each element and returns the element that gives the highest output. If multiple elements give the highest output, the first one will be returned.
        /// </summary>
        public static TElement ArgMax<TElement, TCompare>(this IEnumerable<TElement> elements, Func<TElement, TCompare> function) where TCompare : IComparable<TCompare>
        {
            if (elements is null)
            {
                throw NullIEnumerableException<TElement>(nameof(elements));
            }
            if (elements.None())
            {
                throw EmptyIEnumerableException<TElement>(nameof(elements));
            }

            (TElement element, TCompare value) max = (elements.First(), function.Invoke(elements.First()));
            foreach (TElement element in elements.Skip(1))
            {
                TCompare value = function.Invoke(element);
                if (value.CompareTo(max.value) > 0)
                {
                    max = (element, value);
                }
            }

            return max.element;
        }

        /// <summary>
        /// <para>
        /// Returns the minimum and maximum element of the <see cref="IEnumerable{T}"/>. Can be more efficient than calling <see cref="Enumerable.Min{T}(IEnumerable{T})"/> and
        /// <see cref="Enumerable.Max{T}(IEnumerable{T})"/> as this only iterates through the <see cref="IEnumerable{T}"/> once.
        /// </para>
        /// <para>
        /// Throws an exception if <typeparamref name="T"/> doesn't have a default comparer.
        /// </para>
        /// </summary>
        public static (T min, T max) MinAndMax<T>(this IEnumerable<T> elements)
        {
            if (elements is null)
            {
                throw NullIEnumerableException<T>(nameof(elements));
            }

            Comparer<T> comparer = Comparer<T>.Default ?? throw new ArgumentException($"Type {typeof(T).Name}> has no default comparer.", typeof(T).Name);

            using (IEnumerator<T> enumerator = elements.GetEnumerator())
            {
                if (!enumerator.MoveNext())
                {
                    throw EmptyIEnumerableException<T>(nameof(elements));
                }

                T min = enumerator.Current;
                T max = min;

                while (enumerator.MoveNext())
                {
                    T element = enumerator.Current;
                    if (comparer.Compare(element, min) < 0)
                    {
                        min = element;
                    }
                    if (comparer.Compare(element, max) > 0)
                    {
                        max = element;
                    }
                }

                return (min, max);
            }
        }

        /// <summary>
        /// Returns the given sequence but without the element at the given index. If the index is outside the range of the sequence, the sequence will be unaffected.
        /// </summary>
        public static IEnumerable<T> SkipIndex<T>(this IEnumerable<T> elements, int index) => elements.Where((x, i) => i != index);

        /// <summary>
        /// Returns (<paramref name="elements"/>[0], 0), (<paramref name="elements"/>[1], 1), ..., (<paramref name="elements"/>[^1], <paramref name="elements"/>.Count - 1).
        /// </summary>
        public static IEnumerable<(T element, int index)> Enumerate<T>(this IEnumerable<T> elements) => elements.Select((x, i) => (x, i));
        /// <summary>
        /// Pairs each element with the number of times that element has already occurred.
        /// <example>
        /// <code language="csharp">
        /// char[] actual = new char[] { 'a', 'b', 'a', 'c', 'b', 'a' }.EnumerateOccurrences();
        /// char[] expected = new (char, int)[] { ('a', 0), ('b', 0), ('a', 1), ('c', 0), ('b', 1), ('a', 2) };
        /// expected.SequenceEqual(actual);  // true
        /// </code>
        /// </example>
        /// </summary>
        /// <remarks>
        /// Uses the default equality comparer for type <typeparamref name="T"/>.
        /// </remarks>
        public static IEnumerable<(T element, int occurrence)> EnumerateOccurrences<T>(this IEnumerable<T> elements)
        {
            if (elements is null)
            {
                throw NullIEnumerableException<T>(nameof(elements));
            }

            Dictionary<T, int> occurrenceCounts = new Dictionary<T, int>();
            foreach (T element in elements)
            {
                int occurrenceCount = occurrenceCounts.GetValueOrDefault(element, 0);
                yield return (element, occurrenceCount);
                occurrenceCounts[element] = occurrenceCount + 1;
            }
        }

        /// <summary>
        /// Returns <c>(<paramref name="left"/>[0], <paramref name="right"/>[0]), (<paramref name="left"/>[1], <paramref name="right"/>[1]), ...,</c> until either <paramref name="left"/> or
        /// <paramref name="right"/> ends.
        /// </summary>
        public static IEnumerable<(T1 left, T2 right)> Zip<T1, T2>(this IEnumerable<T1> left, IEnumerable<T2> right) => left.Zip(right, (x, y) => (x, y));
        /// <summary>
        /// <para>
        /// Returns <c>(<paramref name="elements"/>[0], <paramref name="elements"/>[1]), (<paramref name="elements"/>[1], <paramref name="elements"/>[2]), ...,
        /// (<paramref name="elements"/>[^2], <paramref name="elements"/>[^1])</c>.
        /// </para>
        /// <para>
        /// Returns an empty <see cref="IEnumerable{T}"/> if <paramref name="elements"/> has length 0 or 1.
        /// </para>
        /// </summary>
        public static IEnumerable<(T current, T next)> ZipCurrentAndNext<T>(this IEnumerable<T> elements) => elements.Zip(elements.Skip(1));

        /// <summary>
        /// Flattens a sequence of sequences into one sequence, by first iterating over the first sequence, then the second sequence, etc.
        /// <example>
        /// For example, <c>{ { 1, 2, 3 }, { 4, 5 }, { }, { 6, 7, 8, 9, 10 } }</c> flattens to <c>{ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }</c>.
        /// </example>
        /// </summary>
        /// <exception cref="ArgumentNullException"><paramref name="sequences"/> is null.</exception>
        /// <exception cref="ArgumentException">One of the sequences in <paramref name="sequences"/> is null.</exception>
        public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> sequences)
        {
            if (sequences is null)
            {
                throw new ArgumentNullException(nameof(sequences), "The outer sequence is null.");
            }

            foreach (IEnumerable<T> sequence in sequences)
            {
                if (sequence is null)
                {
                    throw new ArgumentException("One of the inner sequences is null.", nameof(sequences));
                }

                foreach (T element in sequence)
                {
                    yield return element;
                }
            }
        }

        /// <summary>
        /// Returns the product of all elements in the sequence, or 1 if the sequence is empty.
        /// </summary>
        public static int Product(this IEnumerable<int> elements) => elements.Aggregate(1, (accumulator, x) => accumulator * x);
        /// <summary>
        /// Returns the product of all elements in the sequence, or 1 if the sequence is empty.
        /// </summary>
        public static long Product(this IEnumerable<long> elements) => elements.Aggregate(1L, (accumulator, x) => accumulator * x);
        /// <summary>
        /// Returns the product of all elements in the sequence, or 1 if the sequence is empty.
        /// </summary>
        public static float Product(this IEnumerable<float> elements) => elements.Aggregate(1f, (accumulator, x) => accumulator * x);
        /// <summary>
        /// Returns the product of all elements in the sequence, or 1 if the sequence is empty.
        /// </summary>
        public static double Product(this IEnumerable<double> elements) => elements.Aggregate(1d, (accumulator, x) => accumulator * x);
        /// <summary>
        /// Returns the product of all elements in the sequence, or 1 if the sequence is empty.
        /// </summary>
        public static decimal Product(this IEnumerable<decimal> elements) => elements.Aggregate(1m, (accumulator, x) => accumulator * x);

        /// <summary>
        /// Formats <paramref name="elements"/> into a string of the form <c>"{ <paramref name="elements"/>[0], ..., <paramref name="elements"/>[^1] }"</c> (with the '...' replaced by elements).
        /// </summary>
        public static string ToPrettyString<T>(this IEnumerable<T> elements) => elements.Any() ? $"{{ {string.Join(", ", elements)} }}" : "{ }";
    }
}
