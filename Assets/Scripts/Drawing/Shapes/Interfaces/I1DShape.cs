using PAC.DataStructures;

namespace PAC.Shapes
{
    public interface I1DShape : IRotatableShape
    {
        /// <summary>
        /// Translates the shape by the given vector.
        /// </summary>
        public static I1DShape operator +(I1DShape shape, IntVector2 translation) => shape.Translate(translation);
        /// <summary>
        /// Translates the shape by the given vector.
        /// </summary>
        public static I1DShape operator +(IntVector2 translation, I1DShape shape) => shape + translation;
        /// <summary>
        /// Translates the shape by the given vector.
        /// </summary>
        public static I1DShape operator -(I1DShape shape, IntVector2 translation) => shape + -translation;
        /// <summary>
        /// Reflects the shape through the origin.
        /// </summary>
        public static I1DShape operator -(I1DShape shape) => shape.Rotate(RotationAngle._180);

        /// <summary>
        /// Translates the shape by the given vector.
        /// </summary>
        public new I1DShape Translate(IntVector2 translation);
        IRotatableShape IRotatableShape.Translate(IntVector2 translation) => Translate(translation);
        /// <summary>
        /// Reflects the shape across the given axis.
        /// </summary>
        public new I1DShape Flip(FlipAxis axis);
        IRotatableShape IRotatableShape.Flip(FlipAxis axis) => Flip(axis);
        /// <summary>
        /// Rotates the shape by the given angle.
        /// </summary>
        public new I1DShape Rotate(RotationAngle angle);
        IRotatableShape IRotatableShape.Rotate(RotationAngle angle) => Rotate(angle);

        public new I1DShape DeepCopy();
        IRotatableShape IRotatableShape.DeepCopy() => DeepCopy();
    }
}