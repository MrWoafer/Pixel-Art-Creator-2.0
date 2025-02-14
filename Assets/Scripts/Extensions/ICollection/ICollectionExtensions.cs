using System.Collections.Generic;
using System.Linq;

namespace PAC.Extensions
{
    public static class ICollectionExtensions
    {
        /// <summary>
        /// Whether all the given elements are in the <see cref="ICollection{T}"/>.
        /// </summary>
        public static bool ContainsAll<T>(this ICollection<T> iCollection, params T[] elements) => ContainsAll(iCollection, (IEnumerable<T>)elements);
        /// <summary>
        /// Whether all the given elements are in the <see cref="ICollection{T}"/>.
        /// </summary>
        public static bool ContainsAll<T>(this ICollection<T> iCollection, IEnumerable<T> elements) => elements.All(x => iCollection.Contains(x));

        /// <summary>
        /// Whether at least one of the given elements is not in the <see cref="ICollection{T}"/>.
        /// </summary>
        public static bool ContainsNotAll<T>(this ICollection<T> iCollection, params T[] elements) => ContainsNotAll(iCollection, (IEnumerable<T>)elements);
        /// <summary>
        /// Whether at least one of the given elements is not in the <see cref="ICollection{T}"/>.
        /// </summary>
        public static bool ContainsNotAll<T>(this ICollection<T> iCollection, IEnumerable<T> elements) => !elements.All(x => iCollection.Contains(x));

        /// <summary>
        /// Whether any of the given elements are in the <see cref="ICollection{T}"/>.
        /// </summary>
        public static bool ContainsAny<T>(this ICollection<T> iCollection, params T[] elements) => ContainsAny(iCollection, (IEnumerable<T>)elements);
        /// <summary>
        /// Whether any of the given elements are in the <see cref="ICollection{T}"/>.
        /// </summary>
        public static bool ContainsAny<T>(this ICollection<T> iCollection, IEnumerable<T> elements) => elements.Any(x => iCollection.Contains(x));

        /// <summary>
        /// Whether none of the given elements are in the <see cref="ICollection{T}"/>.
        /// </summary>
        public static bool ContainsNone<T>(this ICollection<T> iCollection, params T[] elements) => ContainsNone(iCollection, (IEnumerable<T>)elements);
        /// <summary>
        /// Whether none of the given elements are in the <see cref="ICollection{T}"/>.
        /// </summary>
        public static bool ContainsNone<T>(this ICollection<T> iCollection, IEnumerable<T> elements) => !elements.Any(x => iCollection.Contains(x));
    }
}
