using PAC.DataStructures;

namespace PAC.Drawing
{
    public static partial class Shapes
    {
        public interface I2DShape : IFillableShape, IRotatableShape
        {
            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public static I2DShape operator +(I2DShape shape, IntVector2 translation) => shape.Translate(translation);
            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public static I2DShape operator +(IntVector2 translation, I2DShape shape) => shape + translation;
            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public static I2DShape operator -(I2DShape shape, IntVector2 translation) => shape + (-translation);
            /// <summary>
            /// Reflects the shape through the origin.
            /// </summary>
            public static I2DShape operator -(I2DShape shape) => shape.Rotate(RotationAngle._180);

            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public new I2DShape Translate(IntVector2 translation);
            IShape IShape.Translate(IntVector2 translation) => Translate(translation);
            IFillableShape IFillableShape.Translate(IntVector2 translation) => Translate(translation);
            IRotatableShape IRotatableShape.Translate(IntVector2 translation) => Translate(translation);
            /// <summary>
            /// Reflects the shape across the given axis.
            /// </summary>
            public new I2DShape Flip(FlipAxis axis);
            IShape IShape.Flip(FlipAxis axis) => Flip(axis);
            IFillableShape IFillableShape.Flip(FlipAxis axis) => Flip(axis);
            IRotatableShape IRotatableShape.Flip(FlipAxis axis) => Flip(axis);
            /// <summary>
            /// Rotates the shape by the given angle.
            /// </summary>
            public new I2DShape Rotate(RotationAngle angle);
            IRotatableShape IRotatableShape.Rotate(RotationAngle angle) => Rotate(angle);

            public new I2DShape DeepCopy();
            IShape IShape.DeepCopy() => DeepCopy();
            IFillableShape IFillableShape.DeepCopy() => DeepCopy();
            IRotatableShape IRotatableShape.DeepCopy() => DeepCopy();
        }
    }
}