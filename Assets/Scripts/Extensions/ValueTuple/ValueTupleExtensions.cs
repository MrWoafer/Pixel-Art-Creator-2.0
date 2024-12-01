using System;
using System.Collections.Generic;

namespace PAC
{
    public static class ValueTupleExtensions
    {
        public static IEnumerable<T> AsIEnumerable<T>(this ValueTuple<T> valueTuple)
        {
            yield return valueTuple.Item1;
        }
        public static IEnumerable<T> AsIEnumerable<T>(this ValueTuple<T, T> valueTuple)
        {
            yield return valueTuple.Item1;
            yield return valueTuple.Item2;
        }
        public static IEnumerable<T> AsIEnumerable<T>(this ValueTuple<T, T, T> valueTuple)
        {
            yield return valueTuple.Item1;
            yield return valueTuple.Item2;
            yield return valueTuple.Item3;
        }
        public static IEnumerable<T> AsIEnumerable<T>(this ValueTuple<T, T, T, T> valueTuple)
        {
            yield return valueTuple.Item1;
            yield return valueTuple.Item2;
            yield return valueTuple.Item3;
            yield return valueTuple.Item4;
        }
        public static IEnumerable<T> AsIEnumerable<T>(this ValueTuple<T, T, T, T, T> valueTuple)
        {
            yield return valueTuple.Item1;
            yield return valueTuple.Item2;
            yield return valueTuple.Item3;
            yield return valueTuple.Item4;
            yield return valueTuple.Item5;
        }
        public static IEnumerable<T> AsIEnumerable<T>(this ValueTuple<T, T, T, T, T, T> valueTuple)
        {
            yield return valueTuple.Item1;
            yield return valueTuple.Item2;
            yield return valueTuple.Item3;
            yield return valueTuple.Item4;
            yield return valueTuple.Item5;
            yield return valueTuple.Item6;
        }
        public static IEnumerable<T> AsIEnumerable<T>(this ValueTuple<T, T, T, T, T, T, T> valueTuple)
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
        public static T Index<T>(this ValueTuple<T> valueTuple, int index)
        {
            switch (index)
            {
                case 0: return valueTuple.Item1;
                default: throw new IndexOutOfRangeException("Tuple has 1 element, but the given index was " + index + ".");
            }
        }
        /// <summary>
        /// Returns the element at the given index in the tuple.
        /// </summary>
        public static T Index<T>(this ValueTuple<T, T> valueTuple, int index)
        {
            switch (index)
            {
                case 0: return valueTuple.Item1;
                case 1: return valueTuple.Item2;
                default: throw new IndexOutOfRangeException("Tuple has 2 elements, but the given index was " + index + ".");
            }
        }
        /// <summary>
        /// Returns the element at the given index in the tuple.
        /// </summary>
        public static T Index<T>(this ValueTuple<T, T, T> valueTuple, int index)
        {
            switch (index)
            {
                case 0: return valueTuple.Item1;
                case 1: return valueTuple.Item2;
                case 2: return valueTuple.Item3;
                default: throw new IndexOutOfRangeException("Tuple has 3 elements, but the given index was " + index + ".");
            }
        }
        /// <summary>
        /// Returns the element at the given index in the tuple.
        /// </summary>
        public static T Index<T>(this ValueTuple<T, T, T, T> valueTuple, int index)
        {
            switch (index)
            {
                case 0: return valueTuple.Item1;
                case 1: return valueTuple.Item2;
                case 2: return valueTuple.Item3;
                case 3: return valueTuple.Item4;
                default: throw new IndexOutOfRangeException("Tuple has 4 elements, but the given index was " + index + ".");
            }
        }
        /// <summary>
        /// Returns the element at the given index in the tuple.
        /// </summary>
        public static T Index<T>(this ValueTuple<T, T, T, T, T> valueTuple, int index)
        {
            switch (index)
            {
                case 0: return valueTuple.Item1;
                case 1: return valueTuple.Item2;
                case 2: return valueTuple.Item3;
                case 3: return valueTuple.Item4;
                case 4: return valueTuple.Item5;
                default: throw new IndexOutOfRangeException("Tuple has 5 elements, but the given index was " + index + ".");
            }
        }
        /// <summary>
        /// Returns the element at the given index in the tuple.
        /// </summary>
        public static T Index<T>(this ValueTuple<T, T, T, T, T, T> valueTuple, int index)
        {
            switch (index)
            {
                case 0: return valueTuple.Item1;
                case 1: return valueTuple.Item2;
                case 2: return valueTuple.Item3;
                case 3: return valueTuple.Item4;
                case 4: return valueTuple.Item5;
                case 5: return valueTuple.Item6;
                default: throw new IndexOutOfRangeException("Tuple has 6 elements, but the given index was " + index + ".");
            }
        }
        /// <summary>
        /// Returns the element at the given index in the tuple.
        /// </summary>
        public static T Index<T>(this ValueTuple<T, T, T, T, T, T, T> valueTuple, int index)
        {
            switch (index)
            {
                case 0: return valueTuple.Item1;
                case 1: return valueTuple.Item2;
                case 2: return valueTuple.Item3;
                case 3: return valueTuple.Item4;
                case 4: return valueTuple.Item5;
                case 5: return valueTuple.Item6;
                case 6: return valueTuple.Item7;
                default: throw new IndexOutOfRangeException("Tuple has 7 elements, but the given index was " + index + ".");
            }
        }
    }
}
