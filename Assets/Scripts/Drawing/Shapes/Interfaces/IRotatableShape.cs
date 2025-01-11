using PAC.DataStructures;

namespace PAC.Drawing
{
    public static partial class Shapes
    {
        public interface IRotatableShape : IShape
        {
            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public static IRotatableShape operator +(IRotatableShape shape, IntVector2 translation) => shape.Translate(translation);
            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public static IRotatableShape operator +(IntVector2 translation, IRotatableShape shape) => shape + translation;
            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public static IRotatableShape operator -(IRotatableShape shape, IntVector2 translation) => shape + (-translation);
            /// <summary>
            /// Reflects the shape through the origin.
            /// </summary>
            public static IRotatableShape operator -(IRotatableShape shape) => shape.Rotate(RotationAngle._180);

            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public new IRotatableShape Translate(IntVector2 translation);
            IShape IShape.Translate(IntVector2 translation) => Translate(translation);
            /// <summary>
            /// Reflects the shape across the given axis.
            /// </summary>
            public new IRotatableShape Flip(FlipAxis axis);
            IShape IShape.Flip(FlipAxis axis) => Flip(axis);
            /// <summary>
            /// Rotates the shape by the given angle.
            /// </summary>
            public IRotatableShape Rotate(RotationAngle angle);

            public new IRotatableShape DeepCopy();
            IShape IShape.DeepCopy() => DeepCopy();
        }
    }
}