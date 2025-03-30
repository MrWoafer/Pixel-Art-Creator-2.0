using System;
using System.Collections.Generic;

namespace PAC.Extensions.System.Collections
{
    /// <summary>
    /// Extension methods for <see cref="IReadOnlyList{T}"/>.
    /// </summary>
    public static class IReadOnlyListExtensions
    {
        /// <summary>
        /// Lazily iterates over the <see cref="IReadOnlyList{T}"/>'s elements from position <paramref name="index"/> (inclusive) to <paramref name="index"/> + <paramref name="count"/> (exclusive).
        /// This is defined even if <paramref name="count"/> is negative.
        /// </summary>
        /// <param name="index">The index to start iterating from.</param>
        /// <param name="count">The number of elements to iterate over, starting from <paramref name="index"/>. If negative, it will iterate from <paramref name="index"/> backwards.</param>
        /// <exception cref="ArgumentNullException"><paramref name="list"/> is empty.</exception>
        public static IEnumerable<T> GetRange<T>(this IReadOnlyList<T> list, int index, int count)
        {
            if (list is null)
            {
                throw new ArgumentNullException(nameof(list), $"The given IReadOnlyList<{typeof(T).Name}> is null.");
            }

            if (count >= 0)
            {
                for (int i = 0; i < count; i++)
                {
                    yield return list[index + i];
                }
            }
            else
            {
                for (int i = 0; i < -count; i++)
                {
                    yield return list[index - i];
                }
            }
        }

        /// <summary>
        /// Lazily iterates over the indices in the <see cref="Range"/> and yields the <see cref="IReadOnlyList{T}"/>'s element at that position.
        /// </summary>
        /// <param name="indices">The range of indices to iterate over.</param>
        /// <exception cref="ArgumentNullException"><paramref name="list"/> is empty.</exception>
        public static IEnumerable<T> GetRange<T>(this IReadOnlyList<T> list, Range indices)
        {
            int start = indices.Start.IsFromEnd ? list.Count - indices.Start.Value : indices.Start.Value;
            int end = indices.End.IsFromEnd ? list.Count - indices.End.Value : indices.End.Value;
            return list.GetRange(start, end - start);
        }
    }
}
