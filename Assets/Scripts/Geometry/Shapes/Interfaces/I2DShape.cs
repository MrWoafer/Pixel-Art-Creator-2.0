﻿using PAC.Geometry.Axes;

namespace PAC.Geometry.Shapes.Interfaces
{
    /// <summary>
    /// A pixel art representation of a 2D shape (such as a rectangle or a circle).
    /// </summary>
    /// <remarks>
    /// See <see cref="ITranslatableShape{T}"/> for details of the design pattern regarding the generic type parameter.
    /// </remarks>
    /// <typeparam name="T">
    /// The type of shape obtained from transformations on the shape.
    /// When implementing this interface on a concrete type, this should be the same as the implementing type. See <see cref="ITranslatableShape{T}"/> for more detail on this design pattern.
    /// </typeparam>
    public interface I2DShape<out T> : IFillableShape, ITransformableShape<T, CardinalOrdinalAxis> where T : IShape { }
}