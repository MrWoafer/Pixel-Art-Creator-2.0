namespace PAC.Geometry.Shapes.Interfaces
{
    /// <summary>
    /// An <see cref="IShape"/> that can be rotated.
    /// </summary>
    /// <remarks>
    /// See <see cref="ITranslatableShape{T}"/> for details of the design pattern regarding the generic type parameter.
    /// </remarks>
    /// <typeparam name="T">
    /// The type of shape obtained from rotating.
    /// When implementing this interface on a concrete type, this should be the same as the implementing type. See <see cref="ITranslatableShape{T}"/> for more detail on this design pattern.
    /// </typeparam>
    public interface IRotatableShape<out T> : IDeepCopyableShape<T> where T : IShape
    {
        /// <summary>
        /// Returns a deep copy of the shape rotated by the given angle.
        /// </summary>
        /// <seealso cref="operator -(IRotatableShape{T})"/>
        public T Rotate(QuadrantalAngle angle);

        #region Default Implementations
        /// <summary>
        /// Returns a deep copy of the shape rotated 180 degrees about the origin (equivalently, reflected through the origin).
        /// </summary>
        /// <seealso cref="Rotate(QuadrantalAngle)"/>
        public static T operator -(IRotatableShape<T> shape) => shape.Rotate(QuadrantalAngle._180);

        T IDeepCopyableShape<T>.DeepCopy() => Rotate(QuadrantalAngle._0);
        #endregion
    }
}