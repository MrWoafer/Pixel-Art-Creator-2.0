using PAC.DataStructures;
using PAC.Interfaces;

namespace PAC.Geometry.Shapes.Interfaces
{
    /// <summary>
    /// Defines a non-empty collection of integer points that make up a pixel art shape. The shape must be connected (including diagonally).
    /// </summary>
    /// <remarks>
    /// Though uncommon among the shapes that implement this interface, points <i>can</i> be repeated in the collection. Whether this can happen or not should be documented for each
    /// implementing shape. Note that <see cref="System.Collections.Generic.IReadOnlyCollection{T}.Count"/> should include multiplicities.
    /// </remarks>
    public interface IShape : IReadOnlyContains<IntVector2>
    {
        /// <summary>
        /// The smallest rect containing the whole shape.
        /// </summary>
        public IntRect boundingRect { get; }
    }
}