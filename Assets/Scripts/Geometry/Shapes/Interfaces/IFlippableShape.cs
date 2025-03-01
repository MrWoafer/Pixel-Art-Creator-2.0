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
    public interface IFlippableShape<out T> : IDeepCopyableShape<T> where T : IShape
    {
        /// <summary>
        /// Returns a deep copy of the shape reflected across the given axis.
        /// </summary>
        public T Flip(FlipAxis axis);

        #region Default Implementations
        T IDeepCopyableShape<T>.DeepCopy() => Flip(FlipAxis.None);
        #endregion
    }
}