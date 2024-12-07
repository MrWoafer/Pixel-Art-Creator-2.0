using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PAC.Extensions
{
    public static class IEnumerableExtensions
    {
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
        /// Creates a new IEnumerable that iterates through all of this one, then all of second, etc.
        /// </summary>
        public static IEnumerable<T> Chain<T>(this IEnumerable<T> first, IEnumerable<T> second, params IEnumerable<T>[] subsequent)
        {
            foreach (T element in first)
            {
                yield return element;
            }
            foreach (T element in second)
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
        /// <para>
        /// Returns (elements[0], elements[1]), (elements[1], elements[2]), ..., (elements[^2], elements[^1]).
        /// </para>
        /// <para>
        /// Returns an empty IEnumerable if elements has length 0 or 1.
        /// </para>
        /// </summary>
        public static IEnumerable<(T, T)> PairCurrentAndNext<T>(this IEnumerable<T> elements) => elements.Zip(elements.Skip(1));

        /// <summary>
        /// Applies the function to each element and returns the element that gives the lowest output. If multiple elements give the lowest output, the first one will be returned.
        /// </summary>
        public static T1 ArgMin<T1, T2>(this IEnumerable<T1> elements, Func<T1, T2> function) where T2 : IComparable<T2>
        {
            if (elements is null)
            {
                throw new ArgumentException("The given IEnumerable<" + typeof(T1).Name + "> is null.", "elements");
            }
            if (elements.IsEmpty())
            {
                throw new ArgumentException("The given IEnumerable<" + typeof(T1).Name + "> is empty.", "elements");
            }

            (T1 element, T2 value) min = (elements.First(), function.Invoke(elements.First()));
            foreach (T1 element in elements)
            {
                T2 value = function.Invoke(element);
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
        public static T1 ArgMax<T1, T2>(this IEnumerable<T1> elements, Func<T1, T2> function) where T2 : IComparable<T2>
        {
            if (elements is null)
            {
                throw new ArgumentException("The given IEnumerable<" + typeof(T1).Name + "> is null.", "elements");
            }
            if (elements.IsEmpty())
            {
                throw new ArgumentException("The given IEnumerable<" + typeof(T1).Name + "> is empty.", "elements");
            }

            (T1 element, T2 value) max = (elements.First(), function.Invoke(elements.First()));
            foreach (T1 element in elements)
            {
                T2 value = function.Invoke(element);
                if (value.CompareTo(max.value) > 0)
                {
                    max = (element, value);
                }
            }

            return max.element;
        }

        /// <summary>
        /// Returns (first[0], second[0]), (first[1], second[1]), ..., until one of the IEnumerables ends.
        /// </summary>
        public static IEnumerable<(T1, T2)> Zip<T1, T2>(this IEnumerable<T1> first, IEnumerable<T2> second) => first.Zip(second, (x, y) => (x, y));
    }
}
