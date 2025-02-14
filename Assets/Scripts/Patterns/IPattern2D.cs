using PAC.DataStructures;

namespace PAC.Patterns
{
    /// <summary>
    /// Represents an algorithm to assign a value to each point in the 2D plane.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This value should be completely determined by the state of the object, and computing it should not change the state in a way that will affect any subsequent computations. This is so the
    /// value is invariant across multiple calls, but allows 'settings' of the pattern to be changed.
    /// </para>
    /// <example>
    /// <para>
    /// <b>Intended Usage:</b>
    /// </para>
    /// <code language="csharp">
    /// class Pattern : IPattern2D&lt;int&gt;
    /// {
    ///     public int valueOfZero;
    /// 
    ///     public Pattern(int valueOfZero)
    ///     {
    ///         this.valueOfZero = valueOfZero;
    ///     }
    /// 
    ///     // Assigns valueOfZero to (0, 0) and assigns 0 to every other point
    ///     public int this[IntVector2 point] =&gt; point == IntVector2.zero ? valueOfZero : 0;
    /// }
    /// </code>
    /// <para>
    /// Here the value of a point is completely determined by <c>valueOfZero</c> and computing a value does not change anything about the object.
    /// The user is allowed to change <c>valueOfZero</c>, which acts as a 'setting' for the pattern.
    /// </para>
    /// <para>
    /// <b>Not Intended Usage:</b>
    /// </para>
    /// <code language="csharp">
    /// class Pattern : IPattern2D&lt;int&gt;
    /// {
    ///     private int value = 0;
    ///     
    ///     public int this[IntVector2 point]
    ///     {
    ///         value += 1; // Changes the object's state
    ///         return value;
    ///     }
    /// }
    /// </code>
    /// <para>
    /// This is not intended usage because computing a value modifies the state of the object so that computing the value of, say, <c>(0, 0)</c> twice will give two different values without having
    /// changed any 'settings' of the pattern.
    /// </para>
    /// </example>
    /// <para>
    /// <b>Note:</b>
    /// </para>
    /// <para>
    /// Changes to state that don't affect the values are allowed. So, for example, caching values to avoid recomputing them is allowed.
    /// </para>
    /// </remarks>
    /// <typeparam name="T">The type of the values assigned to points in the 2D plane.</typeparam>
    public interface IPattern2D<out T>
    {
        /// <summary>
        /// Gets the value of the pattern at the given point.
        /// </summary>
        /// <remarks>
        /// This value should be completely determined by the state of the object, and computing it should not change the state in a way that will affect any subsequent computations.
        /// See <see cref="IPattern2D{T}"/> for more details.
        /// </remarks>
        public T this[IntVector2 point] { get; }
    }
}
