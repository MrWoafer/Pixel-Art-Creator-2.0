namespace PAC.Interfaces
{
    /// <summary>
    /// Defines how to determine whether two sets are equal / one is a subset of the other.
    /// </summary>
    /// <remarks>
    /// If <c>T1</c> implements <c>ISetComparable&lt;T2&gt;</c> and <c>T2</c> implements <c>ISetComparable&lt;T1&gt;</c>, it is up to implementers to ensure the logical symmetries of the methods.
    /// See the documentation of each method for more detail on these symmetries.
    /// </remarks>
    /// <typeparam name="T">The type to define set comparison against.</typeparam>
    public interface ISetComparable<T>
    {
        /// <summary>
        /// Whether <see langword="this"/> as a set is equal to <paramref name="other"/> as a set.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Recall that set equality means they have precisely the same elements, ignoring order and duplicate elements.
        /// </para>
        /// <para>
        /// If <c>T1</c> implements <c>ISetComparable&lt;T2&gt;</c> and <c>T2</c> implements <c>ISetComparable&lt;T1&gt;</c>, then <c>T1.SetEquals(T2)</c> should be logically equivalent to
        /// <c>T2.SetEquals(T1)</c>.
        /// </para>
        /// </remarks>
        public bool SetEquals(T other);
        /// <summary>
        /// Whether <see langword="this"/> as a set is a subset of <paramref name="other"/> as a set.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Recall that '<see langword="this"/> is subset of <paramref name="other"/>' means that every element of <see langword="this"/> is an element of <paramref name="other"/>, ignoring order
        /// and duplicate elements.
        /// </para>
        /// <para>
        /// If <c>T1</c> implements <c>ISetComparable&lt;T2&gt;</c> and <c>T2</c> implements <c>ISetComparable&lt;T1&gt;</c>, then <c>T1.IsSubsetOf(T2)</c> should be logically equivalent to
        /// <c>T2.IsSupersetOf(T1)</c>.
        /// </para>
        /// </remarks>
        /// <seealso cref="IsSupersetOf(T)"/>
        public bool IsSubsetOf(T other);
        /// <summary>
        /// Whether <see langword="this"/> as a set is a superset of <paramref name="other"/> as a set.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Recall that '<see langword="this"/> is superset of <paramref name="other"/>' means that every element of <paramref name="other"/> is an element of <see langword="this"/>, ignoring order
        /// and duplicate elements.
        /// </para>
        /// <para>
        /// If <c>T1</c> implements <c>ISetComparable&lt;T2&gt;</c> and <c>T2</c> implements <c>ISetComparable&lt;T1&gt;</c>, then <c>T1.IsSupersetOf(T2)</c> should be logically equivalent to
        /// <c>T2.IsSubsetOf(T1)</c>.
        /// </para>
        /// </remarks>
        /// <seealso cref="IsSubsetOf(T)"/>
        public bool IsSupersetOf(T other);
    }
}
