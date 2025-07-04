using PAC.Geometry.Axes;

namespace PAC.Geometry.Shapes.Interfaces
{
    /// <summary>
    /// An <see cref="IShape"/> that can be flipped.
    /// </summary>
    /// <remarks>
    /// See <see cref="ITranslatableShape{T}"/> for details of the design pattern regarding the generic type parameter.
    /// </remarks>
    /// <typeparam name="T">
    /// The type of shape obtained from flipping.
    /// When implementing this interface on a concrete type, this should be the same as the implementing type. See <see cref="ITranslatableShape{T}"/> for more detail on this design pattern.
    /// </typeparam>
    /// <typeparam name="A">
    /// The axis/axes that the shape can be flipped over.
    /// </typeparam>
    public interface IFlippableShape<out T, in A> : IDeepCopyableShape<T> where T : IShape where A : CardinalOrdinalAxis
    {
        /// <summary>
        /// Returns a deep copy of the shape reflected across the given axis.
        /// </summary>
        public T Flipped(A axis);
    }
}