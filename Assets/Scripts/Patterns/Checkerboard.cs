using PAC.DataStructures;

namespace PAC.Patterns
{
    /// <summary>
    /// <para>
    /// A repeating pattern of two values where horizontally/vertically adjacent points do not have the same value (unless the two values of the checkerboard are the same).
    /// </para>
    /// <example>
    /// <para>
    /// For example, with r = red, b = blue:
    /// </para>
    /// <code>
    ///           .
    ///           .
    ///           .
    ///       r b r b r
    ///       b r b r b
    /// . . . r b r b r . . .
    ///       b r b r b
    ///       r b r b r
    ///           .
    ///           .
    ///           .
    /// </code>
    /// </example>
    /// </summary>
    public record Checkerboard<T> : IPattern2D<T>
    {
        /// <summary>
        /// The value of (0, 0) in the checkerboard.
        /// </summary>
        /// <seealso cref="otherValue"/>
        private readonly T valueOfOrigin;
        /// <summary>
        /// The value of (1, 0) in the checkerboard.
        /// </summary>
        /// <seealso cref="valueOfOrigin"/>
        private readonly T otherValue;

        /// <param name="valueOfOrigin"> The value of (0, 0) in the checkerboard.</param>
        /// <param name="otherValue"> The value of (1, 0) in the checkerboard.</param>
        public Checkerboard(T valueOfOrigin, T otherValue)
        {
            this.valueOfOrigin = valueOfOrigin;
            this.otherValue = otherValue;
        }

        public T this[IntVector2 point] => (point.x + point.y) % 2 == 0 ? valueOfOrigin : otherValue;
    }
}