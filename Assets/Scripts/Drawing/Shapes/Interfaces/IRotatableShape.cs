using PAC.DataStructures;

namespace PAC.Shapes
{
    /// <summary>
    /// An <see cref="IShape"/> that can be rotated.
    /// </summary>
    public interface IRotatableShape : IShape
    {
        #region Contract
        #region New Contract
        /// <summary>
        /// Rotates the shape by the given angle.
        /// </summary>
        /// <remarks>
        /// This should return the same type as the object it was called from.
        /// </remarks>
        /// <returns>
        /// A deep copy of the shape rotated by the given angle.
        /// </returns>
        public IRotatableShape Rotate(RotationAngle angle);
        #endregion

        #region Parent Contracts With More Derived Types
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
        public new IRotatableShape Translate(IntVector2 translation);

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
        public new IRotatableShape Flip(FlipAxis axis);

        public new IRotatableShape DeepCopy();
        #endregion
        #endregion

        #region Default Implementations
        /// <summary>
        /// Translates the shape by the given vector.
        /// </summary>
        /// <returns>
        /// A deep copy of the shape translated by the given vector.
        /// </returns>
        /// <seealso cref="Translate(IntVector2)"/>
        public static IRotatableShape operator +(IRotatableShape shape, IntVector2 translation) => shape.Translate(translation);
        /// <summary>
        /// Translates the shape by the given vector.
        /// </summary>
        /// <returns>
        /// A deep copy of the shape translated by the given vector.
        /// </returns>
        /// <seealso cref="Translate(IntVector2)"/>
        public static IRotatableShape operator +(IntVector2 translation, IRotatableShape shape) => shape + translation;
        /// <summary>
        /// Translates the shape by the given vector.
        /// </summary>
        /// <returns>
        /// A deep copy of the shape translated by the given vector.
        /// </returns>
        /// <seealso cref="Translate(IntVector2)"/>
        public static IRotatableShape operator -(IRotatableShape shape, IntVector2 translation) => shape + (-translation);
        /// <summary>
        /// Reflects the shape through the origin.
        /// </summary>
        /// <returns>
        /// A deep copy of the shape reflected through the origin.
        /// </returns>
        /// <seealso cref="Flip(FlipAxis)"/>
        public static IRotatableShape operator -(IRotatableShape shape) => shape.Flip(FlipAxis.Vertical).Flip(FlipAxis.Horizontal);
        #endregion
    }
}