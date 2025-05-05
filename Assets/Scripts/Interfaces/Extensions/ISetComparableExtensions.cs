namespace PAC.Interfaces.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="ISetComparable{T}"/>.
    /// </summary>
    public static class ISetComparableExtensions
    {
        /// <summary>
        /// Whether <paramref name="set"/> as a set is a proper subset of <paramref name="other"/> as a set.
        /// </summary>
        /// <remarks>
        /// Recall that '<paramref name="set"/> is proper subset of <paramref name="other"/>' means that <paramref name="set"/> is a subset of <paramref name="other"/>, but is not set-equal to
        /// <paramref name="other"/>. In more detail, every element of <paramref name="set"/> is an element of <paramref name="other"/>, ignoring order and duplicate elements, but there is at
        /// least one element of <paramref name="other"/> that is not in <paramref name="set"/>.
        /// </remarks>
        /// <seealso cref="IsProperSupersetOf{T1, T2}(T1, T2)"/>
        /// <seealso cref="ISetComparable{T}.IsSubsetOf(T)"/>
        /// <seealso cref="ISetComparable{T}.SetEquals(T)"/>
        public static bool IsProperSubsetOf<T1, T2>(this T1 set, T2 other) where T1 : ISetComparable<T2>
            => set.IsSubsetOf(other) && !set.SetEquals(other);

        /// <summary>
        /// Whether <paramref name="set"/> as a set is a proper superset of <paramref name="other"/> as a set.
        /// </summary>
        /// <remarks>
        /// Recall that '<paramref name="set"/> is proper superset of <paramref name="other"/>' means that <paramref name="set"/> is a superset of <paramref name="other"/>, but is not set-equal to
        /// <paramref name="other"/>. In more detail, every element of <paramref name="other"/> is an element of <paramref name="set"/>, ignoring order and duplicate elements, but there is at
        /// least one element of <paramref name="set"/> that is not in <paramref name="other"/>.
        /// </remarks>
        /// <seealso cref="IsProperSupersetOf{T1, T2}(T1, T2)"/>
        /// <seealso cref="ISetComparable{T}.IsSubsetOf(T)"/>
        /// <seealso cref="ISetComparable{T}.SetEquals(T)"/>
        public static bool IsProperSupersetOf<T1, T2>(this T1 set, T2 other) where T1 : ISetComparable<T2>
            => set.IsSupersetOf(other) && !set.SetEquals(other);
    }
}
