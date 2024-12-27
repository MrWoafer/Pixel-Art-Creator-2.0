using System;
using System.Collections.Generic;

namespace PAC.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="System.Random"/>.
    /// </summary>
    public static class RandomExtensions
    {
        /// <summary>
        /// Creates an infinite random sequence, lazily generated using <see cref="System.Random.Next()"/>.
        /// </summary>
        /// <remarks>
        /// This does not deep-copy the <see cref="System.Random"/>, so calling <see cref="System.Random.Next()"/> on this <see cref="System.Random"/> between the generation of two elements will
        /// affect the sequence.
        /// This also means that iterating over the sequence more than once will most likely give a different sequence each time; you may want to 'save' a portion of the sequence using
        /// <c>new Random().ToSequence().Take(10).ToArray()</c> or similar.
        /// </remarks>
        public static IEnumerable<int> ToSequence(this Random rand)
        {
            if (rand is null)
            {
                throw new ArgumentNullException(nameof(rand), "The Random cannot be null.");
            }

            while (true)
            {
                yield return rand.Next();
            }
        }
        /// <summary>
        /// Creates an infinite random sequence, lazily generated using <see cref="System.Random.Next(int, int)"/>.
        /// </summary>
        /// <remarks>
        /// This does not deep-copy the <see cref="System.Random"/>, so calling <see cref="System.Random.Next(int, int)"/> on this <see cref="System.Random"/> between the generation of two elements
        /// will affect the sequence.
        /// This also means that iterating over the sequence more than once will most likely give a different sequence each time; you may want to 'save' a portion of the sequence using
        /// <c>new Random().ToSequence(minValue, maxValue).Take(10).ToArray()</c> or similar.
        /// </remarks>
        /// <param name="minValue">The inclusive lower bound of the random numbers generated.</param>
        /// <param name="maxValue">The exclusive upper bound of the random numbers generated. <paramref name="maxValue"/> must be greater than or equal to <paramref name="minValue"/>.</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="minValue"/> is greater than <paramref name="maxValue"/>.</exception>
        public static IEnumerable<int> ToSequence(this Random rand, int minValue, int maxValue)
        {
            if (rand is null)
            {
                throw new ArgumentNullException(nameof(rand), "The Random cannot be null.");
            }
            if (maxValue < minValue)
            {
                throw new ArgumentOutOfRangeException(nameof(minValue), $"{nameof(minValue)} cannot be greater than {nameof(maxValue)}.");
            }

            while (true)
            {
                yield return rand.Next(minValue, maxValue);
            }
        }
    }
}
