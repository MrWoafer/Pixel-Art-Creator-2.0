using PAC.DataStructures;

namespace PAC.Patterns
{
    /// <summary>
    /// Represents an algorithm to assign a value to each point in the 2D plane.
    /// </summary>
    /// <remarks>
    /// This should be stateless.
    /// </remarks>
    /// <typeparam name="T">The type of the values assigned to points in the 2D plane.</typeparam>
    public interface IPattern2D<out T>
    {
        /// <summary>
        /// Gets the value of the pattern at the given point.
        /// </summary>
        /// <remarks>
        /// This should be stateless.
        /// </remarks>
        public T this[IntVector2 point] { get; }
    }
}
