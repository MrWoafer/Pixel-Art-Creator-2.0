using PAC.DataStructures;

namespace PAC.Drawing
{
    public static partial class Shapes
    {
        public interface IIsometricShape : IFillableShape
        {
            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public static IIsometricShape operator +(IIsometricShape shape, IntVector2 translation) => shape.Translate(translation);
            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public static IIsometricShape operator +(IntVector2 translation, IIsometricShape shape) => shape + translation;
            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public static IIsometricShape operator -(IIsometricShape shape, IntVector2 translation) => shape + (-translation);
            /// <summary>
            /// Reflects the shape through the origin.
            /// </summary>
            public static IIsometricShape operator -(IIsometricShape shape) => shape.Flip(FlipAxis.Vertical).Flip(FlipAxis.Horizontal);

            /// <summary>
            /// Translates the shape by the given vector.
            /// </summary>
            public new IIsometricShape Translate(IntVector2 translation);
            IFillableShape IFillableShape.Translate(IntVector2 translation) => Translate(translation);
            /// <summary>
            /// Reflects the shape across the given axis.
            /// </summary>
            public new IIsometricShape Flip(FlipAxis axis);
            IFillableShape IFillableShape.Flip(FlipAxis axis) => Flip(axis);

            public new IIsometricShape DeepCopy();
            IFillableShape IFillableShape.DeepCopy() => DeepCopy();
        }
    }
}