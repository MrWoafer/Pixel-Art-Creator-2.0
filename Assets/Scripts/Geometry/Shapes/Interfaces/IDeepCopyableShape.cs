namespace PAC.Geometry.Shapes.Interfaces
{
    /// <summary>
    /// An <see cref="IShape"/> that can be deep-copied.
    /// </summary>
    /// <remarks>
    /// See <see cref="ITranslatableShape{T}"/> for details of the design pattern regarding the generic type parameter.
    /// </remarks>
    /// <typeparam name="T">
    /// The type of shape obtained from the deep copy.
    /// When implementing this interface on a concrete type, this should be the same as the implementing type. See <see cref="ITranslatableShape{T}"/> for more detail on this design pattern.
    /// </typeparam>
    public interface IDeepCopyableShape<out T> : IShape where T : IShape
    {
        /// <summary>
        /// Returns a deep copy of the shape.
        /// </summary>
        public T DeepCopy();
    }
}