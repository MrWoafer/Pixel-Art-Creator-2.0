using PAC.DataStructures;
using PAC.Interfaces;

namespace PAC.Shapes
{
    /// <summary>
    /// Defines a non-empty collection of integer points that make up a pixel art shape.
    /// </summary>
    /// <remarks>
    /// Though uncommon among the shapes that implement this interface, points <i>can</i> be repeated in the collection. Whether this can happen or not should be documented for each
    /// implementing shape. Note that <see cref="System.Collections.Generic.IReadOnlyCollection{T}.Count"/> should include multiplicities.
    /// </remarks>
    public interface IShape : IReadOnlyContains<IntVector2>
    {
        #region Contract
        /// <summary>
        /// The smallest rect containing the whole shape.
        /// </summary>
        public IntRect boundingRect { get; }

        /// <summary>
        /// Translates the shape by the given vector.
        /// </summary>
        /// <remarks>
        /// This should return the same type as the object it was called from.
        /// </remarks>
        /// <returns>
        /// A deep copy of the shape translated by the given vector.
        /// </returns>
        /// <seealso cref="operator +(IShape, IntVector2)"/>
        /// <seealso cref="operator +(IntVector2, IShape)"/>
        /// <seealso cref="operator -(IShape, IntVector2)"/>
        public IShape Translate(IntVector2 translation);

        /// <summary>
        /// Reflects the shape across the given axis.
        /// </summary>
        /// <remarks>
        /// This should return the same type as the object it was called from.
        /// </remarks>
        /// <returns>
        /// A deep copy of the shape reflected across the given axis.
        /// </returns>
        /// <see cref="operator -(IShape)"/>
        public IShape Flip(FlipAxis axis);

        public IShape DeepCopy();
        #endregion

        #region Default Implementations
        /// <summary>
        /// Translates the shape by the given vector.
        /// </summary>
        /// <returns>
        /// A deep copy of the shape translated by the given vector.
        /// </returns>
        /// <seealso cref="Translate(IntVector2)"/>
        public static IShape operator +(IShape shape, IntVector2 translation) => shape.Translate(translation);
        /// <summary>
        /// Translates the shape by the given vector.
        /// </summary>
        /// <returns>
        /// A deep copy of the shape translated by the given vector.
        /// </returns>
        /// <seealso cref="Translate(IntVector2)"/>
        public static IShape operator +(IntVector2 translation, IShape shape) => shape + translation;
        /// <summary>
        /// Translates the shape by the given vector.
        /// </summary>
        /// <returns>
        /// A deep copy of the shape translated by the given vector.
        /// </returns>
        /// <seealso cref="Translate(IntVector2)"/>
        public static IShape operator -(IShape shape, IntVector2 translation) => shape + (-translation);
        /// <summary>
        /// Reflects the shape through the origin.
        /// </summary>
        /// <returns>
        /// A deep copy of the shape reflected through the origin.
        /// </returns>
        /// <seealso cref="Flip(FlipAxis)"/>
        public static IShape operator -(IShape shape) => shape.Flip(FlipAxis.Vertical).Flip(FlipAxis.Horizontal);
        #endregion
    }
}