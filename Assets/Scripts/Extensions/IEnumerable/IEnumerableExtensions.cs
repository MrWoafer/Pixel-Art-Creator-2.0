using System;
using System.Collections;
using System.Collections.Generic;

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
        public static IEnumerable RepeatIEnumerable(IEnumerable elements)
        {
            if (elements is null)
            {
                throw new ArgumentException("Given IEnumerable is null.", "elements");
            }

            while (true)
            {
                foreach (object element in elements)
                {
                    yield return element;
                }
            }
        }
        /// <summary>
        /// Creates an IEnumerable that repeats the given IEnumerable indefinitely, looping back round to the beginning when it reaches the end.
        /// </summary>
        public static IEnumerable<T> RepeatIEnumerable<T>(IEnumerable<T> elements)
        {
            if (elements is null)
            {
                throw new ArgumentException("Given IEnumerable<" + typeof(T).Name + "> is null.", "elements");
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
        /// Returns whether the given IEnumerable has no elements.
        /// </summary>
        public static bool IsEmpty(this IEnumerable elements)
        {
            if (elements is null)
            {
                throw new ArgumentException("Given IEnumerable is null.", "elements");
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
                throw new ArgumentException("Given IEnumerable<" + typeof(T).Name + "> is null.", "elements");
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
                throw new ArgumentException("Given IEnumerable<" + typeof(T).Name + "> is null.", "elements");
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
                throw new ArgumentException("Given IEnumerable<" + typeof(T).Name + "> is null.", "elements");
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
                throw new ArgumentException("Given IEnumerable<" + typeof(T).Name + "> is null.", "elements");
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
        public static IEnumerable<(T, T)> PairCurrentAndNext<T>(this IEnumerable<T> elements)
        {
            if (elements is null)
            {
                throw new ArgumentException("Given IEnumerable<" + typeof(T).Name + "> is null.", "elements");
            }

            T previousElement = default;
            bool passedFirstElement = false;
            foreach(T element in elements)
            {
                if (passedFirstElement)
                {
                    yield return (previousElement, element);
                }
                else
                {
                    passedFirstElement = true;
                }

                previousElement = element;
            }
        }
    }
}
