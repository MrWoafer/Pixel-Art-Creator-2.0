using PAC.DataStructures;

namespace PAC.Shapes
{
    public interface IFillableShape : IShape
    {
        /// <summary>
        /// Whether the shape has its inside filled-in, or whether it's just the border.
        /// </summary>
        public bool filled { get; set; }

        /// <summary>
        /// Translates the shape by the given vector.
        /// </summary>
        public static IFillableShape operator +(IFillableShape shape, IntVector2 translation) => shape.Translate(translation);
        /// <summary>
        /// Translates the shape by the given vector.
        /// </summary>
        public static IFillableShape operator +(IntVector2 translation, IFillableShape shape) => shape + translation;
        /// <summary>
        /// Translates the shape by the given vector.
        /// </summary>
        public static IFillableShape operator -(IFillableShape shape, IntVector2 translation) => shape + -translation;
        /// <summary>
        /// Reflects the shape through the origin.
        /// </summary>
        public static IFillableShape operator -(IFillableShape shape) => shape.Flip(FlipAxis.Vertical).Flip(FlipAxis.Horizontal);

        /// <summary>
        /// Translates the shape by the given vector.
        /// </summary>
        public new IFillableShape Translate(IntVector2 translation);
        IShape IShape.Translate(IntVector2 translation) => Translate(translation);
        /// <summary>
        /// Reflects the shape across the given axis.
        /// </summary>
        public new IFillableShape Flip(FlipAxis axis);
        IShape IShape.Flip(FlipAxis axis) => Flip(axis);

        public new IFillableShape DeepCopy();
        IShape IShape.DeepCopy() => DeepCopy();
    }
}