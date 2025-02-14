using System.Collections.Generic;
using System.Linq;
using PAC.Interfaces;

namespace PAC.Extensions
{
    public static class IReadOnlyContainsExtensions
    {
        /// <summary>
        /// Whether all the given elements are in the <see cref="IReadOnlyContains{T}"/>.
        /// </summary>
        public static bool ContainsAll<T>(this IReadOnlyContains<T> iReadOnlyContains, params T[] elements) => ContainsAll(iReadOnlyContains, (IEnumerable<T>)elements);
        /// <summary>
        /// Whether all the given elements are in the <see cref="IReadOnlyContains{T}"/>.
        /// </summary>
        public static bool ContainsAll<T>(this IReadOnlyContains<T> iReadOnlyContains, IEnumerable<T> elements) => elements.All(x => iReadOnlyContains.Contains(x));

        /// <summary>
        /// Whether at least one of the given elements is not in the <see cref="IReadOnlyContains{T}"/>.
        /// </summary>
        public static bool ContainsNotAll<T>(this IReadOnlyContains<T> iReadOnlyContains, params T[] elements) => ContainsNotAll(iReadOnlyContains, (IEnumerable<T>)elements);
        /// <summary>
        /// Whether at least one of the given elements is not in the <see cref="IReadOnlyContains{T}"/>.
        /// </summary>
        public static bool ContainsNotAll<T>(this IReadOnlyContains<T> iReadOnlyContains, IEnumerable<T> elements) => !elements.All(x => iReadOnlyContains.Contains(x));

        /// <summary>
        /// Whether any of the given elements are in the <see cref="IReadOnlyContains{T}"/>.
        /// </summary>
        public static bool ContainsAny<T>(this IReadOnlyContains<T> iReadOnlyContains, params T[] elements) => ContainsAny(iReadOnlyContains, (IEnumerable<T>)elements);
        /// <summary>
        /// Whether any of the given elements are in the <see cref="IReadOnlyContains{T}"/>.
        /// </summary>
        public static bool ContainsAny<T>(this IReadOnlyContains<T> iReadOnlyContains, IEnumerable<T> elements) => elements.Any(x => iReadOnlyContains.Contains(x));

        /// <summary>
        /// Whether none of the given elements are in the <see cref="IReadOnlyContains{T}"/>.
        /// </summary>
        public static bool ContainsNone<T>(this IReadOnlyContains<T> iReadOnlyContains, params T[] elements) => ContainsNone(iReadOnlyContains, (IEnumerable<T>)elements);
        /// <summary>
        /// Whether none of the given elements are in the <see cref="IReadOnlyContains{T}"/>.
        /// </summary>
        public static bool ContainsNone<T>(this IReadOnlyContains<T> iReadOnlyContains, IEnumerable<T> elements) => !elements.Any(x => iReadOnlyContains.Contains(x));
    }
}
