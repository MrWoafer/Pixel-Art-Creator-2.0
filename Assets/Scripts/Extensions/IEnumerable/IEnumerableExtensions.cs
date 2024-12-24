using System;
using System.Collections.Generic;
using System.Linq;

namespace PAC.Extensions
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
        /// Creates an infinite random sequence.
        /// </summary>
        public static IEnumerable<int> Random(Random rng)
        {
            while (true)
            {
                yield return rng.Next();
            }
        }

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
        /// Returns true iff the given <see cref="IEnumerable{T}"/> has no elements.
        /// </summary>
        public static bool IsEmpty<T>(this IEnumerable<T> elements)
        {
            if (elements is null)
            {
                throw NullIEnumerableException<T>(nameof(elements));
            }

            foreach (T element in elements)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Returns true iff the given <see cref="IEnumerable{T}"/> has at least one element.
        /// </summary>
        public static bool IsNotEmpty<T>(this IEnumerable<T> elements) => !elements.IsEmpty();

        /// <summary>
        /// Returns whether the given <see cref="IEnumerable{T}"/> has exactly n elements.
        /// </summary>
        public static bool CountExactly<T>(this IEnumerable<T> elements, int n)
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
        /// Returns whether the given <see cref="IEnumerable{T}"/> has &gt;= n elements.
        /// </summary>
        public static bool CountAtLeast<T>(this IEnumerable<T> elements, int n)
        {
            if (elements is null)
            {
                throw NullIEnumerableException<T>(nameof(elements));
            }

            if (n < 0)
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
            // We check n == 0 instead of just returning false to deal with the case when elements is empty.
            return n == 0;
        }
        /// <summary>
        /// Returns whether the given <see cref="IEnumerable{T}"/> has &lt;= n elements.
        /// </summary>
        public static bool CountAtMost<T>(this IEnumerable<T> elements, int n)
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
        /// Returns the number of distinct elements in the <see cref="IEnumerable{T}"/>.
        /// </summary>
        public static int CountDistinct<T>(this IEnumerable<T> elements) => elements.ToHashSet().Count;

        /// <summary>
        /// Returns true iff none of the elements satisfy the predicate.
        /// </summary>
        public static bool None<T>(this IEnumerable<T> elements, Func<T, bool> predicate) => !elements.Any(predicate);

        /// <summary>
        /// Applies the function to each element and returns the element that gives the lowest output. If multiple elements give the lowest output, the first one will be returned.
        /// </summary>
        public static TElement ArgMin<TElement, TCompare>(this IEnumerable<TElement> elements, Func<TElement, TCompare> function) where TCompare : IComparable<TCompare>
        {
            if (elements is null)
            {
                throw NullIEnumerableException<TElement>(nameof(elements));
            }
            if (elements.IsEmpty())
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
            if (elements.IsEmpty())
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
            if (elements.IsEmpty())
            {
                throw EmptyIEnumerableException<T>(nameof(elements));
            }

            Comparer<T> comparer;
            try
            {
                comparer = Comparer<T>.Default;
            }
            catch (ArgumentException)
            {
                throw new ArgumentException($"Type {typeof(T).Name}> has no default comparer.", typeof(T).Name);
            }

            T min = elements.First();
            T max = min;
            foreach (T element in elements.Skip(1))
            {
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

        /// <summary>
        /// Returns (<paramref name="elements"/>[0], 0), (<paramref name="elements"/>[1], 1), ..., (<paramref name="elements"/>[^1], <paramref name="elements"/>.Count - 1).
        /// </summary>
        public static IEnumerable<(T element, int index)> Enumerate<T>(this IEnumerable<T> elements) => elements.Select((x, i) => (x, i));
        /// <summary>
        /// Returns (<paramref name="left"/>[0], <paramref name="right"/>[0]), (<paramref name="left"/>[1], <paramref name="right"/>[1]), ..., until either <paramref name="left"/> or
        /// <paramref name="right"/> ends.
        /// </summary>
        public static IEnumerable<(T1 left, T2 right)> Zip<T1, T2>(this IEnumerable<T1> left, IEnumerable<T2> right) => left.Zip(right, (x, y) => (x, y));
        /// <summary>
        /// <para>
        /// Returns (<paramref name="elements"/>[0], <paramref name="elements"/>[1]), (<paramref name="elements"/>[1], <paramref name="elements"/>[2]), ...,
        /// (<paramref name="elements"/>[^2], <paramref name="elements"/>[^1]).
        /// </para>
        /// <para>
        /// Returns an empty <see cref="IEnumerable{T}"/> if <paramref name="elements"/> has length 0 or 1.
        /// </para>
        /// </summary>
        public static IEnumerable<(T current, T next)> ZipCurrentAndNext<T>(this IEnumerable<T> elements) => elements.Zip(elements.Skip(1));

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
    }
}
