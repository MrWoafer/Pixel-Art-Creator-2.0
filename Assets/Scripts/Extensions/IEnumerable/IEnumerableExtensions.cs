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
            while (true)
            {
                foreach (T element in elements)
                {
                    yield return element;
                }
            }
        }

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
    }
}
