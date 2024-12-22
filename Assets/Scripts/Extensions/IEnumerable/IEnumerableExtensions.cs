using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PAC.Extensions
{
    /// <summary>
    /// Extension methods for IEnumerable&lt;T&gt;.
    /// </summary>
    public static class IEnumerableExtensions
    {
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
        /// Creates an IEnumerable that repeats the given element indefinitely.
        /// </summary>
        public static IEnumerable<T> Repeat<T>(T element)
        {
            while (true)
            {
                yield return element;
            }
        }

        /// <summary>
        /// Creates an IEnumerable that repeats the given IEnumerable indefinitely, looping back round to the beginning when it reaches the end.
        /// </summary>
        public static IEnumerable<T> RepeatIEnumerable<T>(IEnumerable<T> elements)
        {
            if (elements is null)
            {
                throw new ArgumentException("The given IEnumerable<" + typeof(T).Name + "> is null.", "elements");
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
        /// Creates a new IEnumerable that iterates through all of this one, then all of second, etc. Extends the LINQ Concat() method to concat multiple IEnumerables.
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
        /// Returns whether the given IEnumerable has no elements.
        /// </summary>
        public static bool IsEmpty(this IEnumerable elements)
        {
            if (elements is null)
            {
                throw new ArgumentException("The given IEnumerable is null.", "elements");
            }

            foreach (object element in elements)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// Returns whether the given IEnumerable has no elements.
        /// </summary>
        public static bool IsEmpty<T>(this IEnumerable<T> elements)
        {
            if (elements is null)
            {
                throw new ArgumentException("The given IEnumerable<" + typeof(T).Name + "> is null.", "elements");
            }

            foreach (T element in elements)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Returns whether the given IEnumerable has exactly n elements.
        /// </summary>
        public static bool CountExactly<T>(this IEnumerable<T> elements, int n)
        {
            if (elements is null)
            {
                throw new ArgumentException("The given IEnumerable<" + typeof(T).Name + "> is null.", "elements");
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
        /// Returns whether the given IEnumerable has &gt;= n elements.
        /// </summary>
        public static bool CountAtLeast<T>(this IEnumerable<T> elements, int n)
        {
            if (elements is null)
            {
                throw new ArgumentException("The given IEnumerable<" + typeof(T).Name + "> is null.", "elements");
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
        /// Returns whether the given IEnumerable has &lt;= n elements.
        /// </summary>
        public static bool CountAtMost<T>(this IEnumerable<T> elements, int n)
        {
            if (elements is null)
            {
                throw new ArgumentException("The given IEnumerable<" + typeof(T).Name + "> is null.", "elements");
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
        /// Returns the number of distinct elements in the IEnumerable.
        /// </summary>
        public static int CountDistinct<T>(this IEnumerable<T> elements) => elements.ToHashSet().Count;

        /// <summary>
        /// Determines whether none of the elements satisfy the predicate.
        /// </summary>
        public static bool None<T>(this IEnumerable<T> elements, Func<T, bool> predicate) => !elements.Any(predicate);

        /// <summary>
        /// Applies the function to each element and returns the element that gives the lowest output. If multiple elements give the lowest output, the first one will be returned.
        /// </summary>
        public static TElement ArgMin<TElement, TCompare>(this IEnumerable<TElement> elements, Func<TElement, TCompare> function) where TCompare : IComparable<TCompare>
        {
            if (elements is null)
            {
                throw new ArgumentNullException($"The given IEnumerable<{typeof(TElement).Name}> is null.", nameof(elements));
            }
            if (elements.IsEmpty())
            {
                throw new ArgumentException($"The given IEnumerable<{typeof(TElement).Name}> is empty.", nameof(elements));
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
                throw new ArgumentNullException($"The given IEnumerable<{typeof(TElement).Name}> is null.", nameof(elements));
            }
            if (elements.IsEmpty())
            {
                throw new ArgumentException($"The given IEnumerable<{typeof(TElement).Name}> is empty.", nameof(elements));
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
        /// Returns the minimum and maximum element of the IEnumerable. Can be more efficient that calling Min() and Max() as this only iterates through the IEnumerable once.
        /// </para>
        /// <para>
        /// Throws an exception if T doesn't have a default comparer.
        /// </para>
        /// </summary>
        public static (T min, T max) MinAndMax<T>(this IEnumerable<T> elements)
        {
            if (elements is null)
            {
                throw new ArgumentNullException($"The given IEnumerable<{typeof(T).Name}> is null.", nameof(elements));
            }
            if (elements.IsEmpty())
            {
                throw new ArgumentException($"The given IEnumerable<{typeof(T).Name}> is empty.", nameof(elements));
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
        /// Returns (elements[0], 0), (elements[1], 1), ..., (elements[^1], elements.Count - 1)
        /// </summary>
        public static IEnumerable<(T element, int index)> Enumerate<T>(this IEnumerable<T> elements) => elements.Select((x, i) => (x, i));
        /// <summary>
        /// Returns (left[0], right[0]), (left[1], right[1]), ..., until one of the IEnumerables ends.
        /// </summary>
        public static IEnumerable<(T1 left, T2 right)> Zip<T1, T2>(this IEnumerable<T1> left, IEnumerable<T2> right) => left.Zip(right, (x, y) => (x, y));
        /// <summary>
        /// <para>
        /// Returns (elements[0], elements[1]), (elements[1], elements[2]), ..., (elements[^2], elements[^1]).
        /// </para>
        /// <para>
        /// Returns an empty IEnumerable if elements has length 0 or 1.
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
