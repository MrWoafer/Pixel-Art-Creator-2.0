using System;
using System.Collections.Generic;

namespace PAC.Extensions
{
    public static class ValueTupleExtensions
    {
        public static IEnumerable<T> AsEnumerable<T>(this ValueTuple<T> valueTuple)
        {
            yield return valueTuple.Item1;
        }
        public static IEnumerable<T> AsEnumerable<T>(this ValueTuple<T, T> valueTuple)
        {
            yield return valueTuple.Item1;
            yield return valueTuple.Item2;
        }
        public static IEnumerable<T> AsEnumerable<T>(this ValueTuple<T, T, T> valueTuple)
        {
            yield return valueTuple.Item1;
            yield return valueTuple.Item2;
            yield return valueTuple.Item3;
        }
        public static IEnumerable<T> AsEnumerable<T>(this ValueTuple<T, T, T, T> valueTuple)
        {
            yield return valueTuple.Item1;
            yield return valueTuple.Item2;
            yield return valueTuple.Item3;
            yield return valueTuple.Item4;
        }
        public static IEnumerable<T> AsEnumerable<T>(this ValueTuple<T, T, T, T, T> valueTuple)
        {
            yield return valueTuple.Item1;
            yield return valueTuple.Item2;
            yield return valueTuple.Item3;
            yield return valueTuple.Item4;
            yield return valueTuple.Item5;
        }
        public static IEnumerable<T> AsEnumerable<T>(this ValueTuple<T, T, T, T, T, T> valueTuple)
        {
            yield return valueTuple.Item1;
            yield return valueTuple.Item2;
            yield return valueTuple.Item3;
            yield return valueTuple.Item4;
            yield return valueTuple.Item5;
            yield return valueTuple.Item6;
        }
        public static IEnumerable<T> AsEnumerable<T>(this ValueTuple<T, T, T, T, T, T, T> valueTuple)
        {
            yield return valueTuple.Item1;
            yield return valueTuple.Item2;
            yield return valueTuple.Item3;
            yield return valueTuple.Item4;
            yield return valueTuple.Item5;
            yield return valueTuple.Item6;
            yield return valueTuple.Item7;
        }

        /// <summary>
        /// Returns the element at the given index in the tuple.
        /// </summary>
        public static T Index<T>(this ValueTuple<T> valueTuple, int index) => index switch
        {
            0 => valueTuple.Item1,
            _ => throw new IndexOutOfRangeException($"The tuple has 1 element, but the given index was {index}.")
        };
        /// <summary>
        /// Returns the element at the given index in the tuple.
        /// </summary>
        public static T Index<T>(this ValueTuple<T, T> valueTuple, int index) => index switch
        {
            0 => valueTuple.Item1,
            1 => valueTuple.Item2,
            _ => throw new IndexOutOfRangeException($"The tuple has 2 elements, but the given index was {index}.")
        };
        /// <summary>
        /// Returns the element at the given index in the tuple.
        /// </summary>
        public static T Index<T>(this ValueTuple<T, T, T> valueTuple, int index) => index switch
        {
            0 => valueTuple.Item1,
            1 => valueTuple.Item2,
            2 => valueTuple.Item3,
            _ => throw new IndexOutOfRangeException($"The tuple has 3 elements, but the given index was {index}.")
        };
        /// <summary>
        /// Returns the element at the given index in the tuple.
        /// </summary>
        public static T Index<T>(this ValueTuple<T, T, T, T> valueTuple, int index) => index switch
        {
            0 => valueTuple.Item1,
            1 => valueTuple.Item2,
            2 => valueTuple.Item3,
            3 => valueTuple.Item4,
            _ => throw new IndexOutOfRangeException($"The tuple has 4 elements, but the given index was {index}.")
        };
        /// <summary>
        /// Returns the element at the given index in the tuple.
        /// </summary>
        public static T Index<T>(this ValueTuple<T, T, T, T, T> valueTuple, int index) => index switch
        {
            0 => valueTuple.Item1,
            1 => valueTuple.Item2,
            2 => valueTuple.Item3,
            3 => valueTuple.Item4,
            4 => valueTuple.Item5,
            _ => throw new IndexOutOfRangeException($"The tuple has 5 elements, but the given index was {index}.")
        };
        /// <summary>
        /// Returns the element at the given index in the tuple.
        /// </summary>
        public static T Index<T>(this ValueTuple<T, T, T, T, T, T> valueTuple, int index) => index switch
        {
            0 => valueTuple.Item1,
            1 => valueTuple.Item2,
            2 => valueTuple.Item3,
            3 => valueTuple.Item4,
            4 => valueTuple.Item5,
            5 => valueTuple.Item6,
            _ => throw new IndexOutOfRangeException($"The tuple has 6 elements, but the given index was {index}.")
        };
        /// <summary>
        /// Returns the element at the given index in the tuple.
        /// </summary>
        public static T Index<T>(this ValueTuple<T, T, T, T, T, T, T> valueTuple, int index) => index switch
        {
            0 => valueTuple.Item1,
            1 => valueTuple.Item2,
            2 => valueTuple.Item3,
            3 => valueTuple.Item4,
            4 => valueTuple.Item5,
            5 => valueTuple.Item6,
            6 => valueTuple.Item7,
            _ => throw new IndexOutOfRangeException($"The tuple has 7 elements, but the given index was {index}.")
        };
    }
}
