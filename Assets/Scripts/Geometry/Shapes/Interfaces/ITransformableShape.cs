using PAC.Geometry.Axes;

namespace PAC.Geometry.Shapes.Interfaces
{
    /// <summary>
    /// An <see cref="IShape"/> that can be translated, flipped and rotated.
    /// </summary>
    /// <remarks>
    /// See <see cref="ITranslatableShape{T}"/> for details of the design pattern regarding the generic type parameter.
    /// </remarks>
    /// <typeparam name="T">
    /// The type of shape obtained from the transformations.
    /// When implementing this interface on a concrete type, this should be the same as the implementing type. See <see cref="ITranslatableShape{T}"/> for more detail on this design pattern.
    /// </typeparam>
    /// <typeparam name="A">
    /// The axis/axes that the shape can be flipped over.
    /// </typeparam>
    public interface ITransformableShape<out T, in A> : ITranslatableShape<T>, IReflectableShape<T, A>, IRotatableShape<T> where T : IShape where A : CardinalOrdinalAxis { }
}