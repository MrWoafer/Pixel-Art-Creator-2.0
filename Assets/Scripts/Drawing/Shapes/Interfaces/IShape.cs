using PAC.DataStructures;
using PAC.Interfaces;

namespace PAC.Shapes
{
    public interface IShape : IReadOnlyContains<IntVector2>
    {
        /// <summary>
        /// The smallest IntRect containing the whole shape.
        /// </summary>
        public IntRect boundingRect { get; }

        /// <summary>
        /// Returns whether the pixel is in the shape.
        /// </summary>
        public new bool Contains(IntVector2 pixel);

        /// <summary>
        /// Translates the shape by the given vector.
        /// </summary>
        public static IShape operator +(IShape shape, IntVector2 translation) => shape.Translate(translation);
        /// <summary>
        /// Translates the shape by the given vector.
        /// </summary>
        public static IShape operator +(IntVector2 translation, IShape shape) => shape + translation;
        /// <summary>
        /// Translates the shape by the given vector.
        /// </summary>
        public static IShape operator -(IShape shape, IntVector2 translation) => shape + -translation;
        /// <summary>
        /// Reflects the shape through the origin.
        /// </summary>
        public static IShape operator -(IShape shape) => shape.Flip(FlipAxis.Vertical).Flip(FlipAxis.Horizontal);

        /// <summary>
        /// Translates the shape by the given vector.
        /// </summary>
        public IShape Translate(IntVector2 translation);
        /// <summary>
        /// Reflects the shape across the given axis.
        /// </summary>
        public IShape Flip(FlipAxis axis);

        public IShape DeepCopy();
    }
}