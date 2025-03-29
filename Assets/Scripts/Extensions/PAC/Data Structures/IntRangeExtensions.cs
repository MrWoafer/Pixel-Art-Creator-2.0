using System;
using System.Collections.Generic;

using PAC.DataStructures;

namespace PAC.Extensions
{
    /// <summary>
    /// Extension methods involving <see cref="IntRange"/>.
    /// </summary>
    public static class IntRangeExtensions
    {
        /// <summary>
        /// Returns a random integer within the given range.
        /// </summary>
        public static int Next(this Random random, IntRange range) =>
            range.isEmpty ? throw new ArgumentException("The given range is empty.", nameof(range)) : random.Next(range.minElement, range.maxElement + 1);

        /// <summary>
        /// Creates an infinite random sequence.
        /// </summary>
        public static IEnumerable<int> Random(Random random, IntRange range)
        {
            while (true)
            {
                yield return random.Next(range);
            }
        }

        /// <summary>
        /// Lazily iterates over the indices in the <see cref="IntRange"/> and yields the <see cref="IReadOnlyList{T}"/>'s element at that position.
        /// </summary>
        /// <param name="indices">The range of indices to iterate over.</param>
        /// <exception cref="ArgumentNullException"><paramref name="list"/> is empty.</exception>
        public static IEnumerable<T> GetRange<T>(this IReadOnlyList<T> list, IntRange indices)
        {
            if (list is null)
            {
                throw new ArgumentNullException(nameof(list), $"The given IReadOnlyList<{typeof(T).Name}> is null.");
            }

            foreach (int index in indices)
            {
                yield return list[index];
            }
        }
    }
}
