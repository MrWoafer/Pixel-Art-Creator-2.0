using System;
using System.Collections.Generic;

namespace PAC.Extensions.System
{
    /// <summary>
    /// Extension methods for <see cref="Random"/>.
    /// </summary>
    public static class RandomExtensions
    {
        /// <summary>
        /// Creates an infinite random sequence, lazily generated using <see cref="Random.Next()"/>.
        /// </summary>
        /// <remarks>
        /// This does not deep-copy the <see cref="Random"/>, so calling <see cref="Random.Next()"/> on this <see cref="Random"/> between the generation of two elements will
        /// affect the sequence.
        /// This also means that iterating over the sequence more than once will most likely give a different sequence each time; you may want to 'save' a portion of the sequence using
        /// <c>new Random().ToSequence().Take(10).ToArray()</c> or similar.
        /// </remarks>
        public static IEnumerable<int> ToSequence(this Random random)
        {
            if (random is null)
            {
                throw new ArgumentNullException(nameof(random), "The Random cannot be null.");
            }

            while (true)
            {
                yield return random.Next();
            }
        }
        /// <summary>
        /// Creates an infinite random sequence, lazily generated using <see cref="Random.Next(int, int)"/>.
        /// </summary>
        /// <remarks>
        /// This does not deep-copy the <see cref="Random"/>, so calling <see cref="Random.Next(int, int)"/> on this <see cref="Random"/> between the generation of two elements
        /// will affect the sequence.
        /// This also means that iterating over the sequence more than once will most likely give a different sequence each time; you may want to 'save' a portion of the sequence using
        /// <c>new Random().ToSequence(minValue, maxValue).Take(10).ToArray()</c> or similar.
        /// </remarks>
        /// <param name="minValue">The inclusive lower bound of the random numbers generated.</param>
        /// <param name="maxValue">The exclusive upper bound of the random numbers generated. <paramref name="maxValue"/> must be greater than or equal to <paramref name="minValue"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minValue"/> is greater than <paramref name="maxValue"/>.</exception>
        public static IEnumerable<int> ToSequence(this Random random, int minValue, int maxValue)
        {
            if (random is null)
            {
                throw new ArgumentNullException(nameof(random), "The Random cannot be null.");
            }
            if (maxValue < minValue)
            {
                throw new ArgumentOutOfRangeException(nameof(minValue), $"{nameof(minValue)} cannot be greater than {nameof(maxValue)}.");
            }

            while (true)
            {
                yield return random.Next(minValue, maxValue);
            }
        }

        /// <summary>
        /// Generates a uniformly random <see cref="bool"/> value.
        /// </summary>
        public static bool NextBool(this Random random)
        {
            return random.Next(2) == 0;
        }

        /// <summary>
        /// Returns a random <see cref="float"/> in the range <c>[0, 1)</c>.
        /// </summary>
        public static float NextFloat(this Random random) => (float)random.NextDouble();

        /// <summary>
        /// Selects a uniformly random element from <paramref name="elements"/>.
        /// </summary>
        /// <exception cref="ArgumentException"><paramref name="elements"/> is empty.</exception>
        public static T NextElement<T>(this Random random, IReadOnlyList<T> elements)
        {
            int Count = elements.Count;
            if (Count == 0)
            {
                throw new ArgumentException($"{nameof(elements)} is empty.", nameof(elements));
            }
            return elements[random.Next(Count)];
        }
    }
}
