using PAC.DataStructures;
using System;
using System.Collections.Generic;

namespace PAC.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="IntRange"/>.
    /// </summary>
    public static class IntRangeExtensions
    {
        /// <summary>
        /// Returns a random integer within the given range.
        /// </summary>
        public static int Next(this Random rng, IntRange range) =>
            range.isEmpty ? throw new ArgumentException("The given range is empty.", nameof(range)) : rng.Next(range.minElement, range.maxElement + 1);
    }
}
