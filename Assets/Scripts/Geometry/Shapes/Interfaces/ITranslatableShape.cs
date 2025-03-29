namespace PAC.Geometry.Shapes.Interfaces
{
    /// <summary>
    /// An <see cref="IShape"/> that can be translated.
    /// </summary>
    /// <remarks>
    /// When implementing this interface on a concrete type, the type parameter <typeparamref name="T"/> should be the same as the implementing type.
    /// <example>
    /// For example,
    /// <code language="csharp">
    /// class Point : ITranslatableShape&lt;Point&gt;
    /// {
    ///     public IntVector2 point;
    ///     
    ///     public Point(IntVector2 point)
    ///     {
    ///         this.point = point;
    ///     }
    ///     
    ///     public Point Translate(IntVector2 translation) => new Point(point + translation);
    ///     
    ///     // Implementation of the rest of the interface...
    /// }
    /// </code>
    /// </example>
    /// This is done to ensure the specific return type of the <see cref="Translate(IntVector2)"/> method.
    /// Note though that <typeparamref name="T"/> is covariant (see <see langword="out"/>), so you can abstract over any shape that is translatable.
    /// <example>
    /// For example,
    /// <code language="csharp">
    /// List&lt;ITranslatableShape&lt;IShape&gt;&gt;
    /// </code>
    /// </example>
    /// This can store any shape that implements <see cref="ITranslatableShape{T}"/>. Note though that calling <see cref="Translate(IntVector2)"/> on an element of this list will return an
    /// <see cref="IShape"/> so cannot be translated again without downcasting. However you can do, say,
    /// <example>
    /// <code language="csharp">
    /// List&lt;ITranslatableShape&lt;ITranslatableShape&lt;IShape&gt;&gt;&gt;
    /// </code>
    /// </example>
    /// This allows you to translate twice before you get an <see cref="IShape"/> returned.
    /// </remarks>
    /// <typeparam name="T">
    /// The type of shape obtained from translating.
    /// When implementing this interface on a concrete type, this should be the same as the implementing type. See <see cref="ITranslatableShape{T}"/> for more detail on this design pattern.
    /// </typeparam>
    public interface ITranslatableShape<out T> : IDeepCopyableShape<T> where T : IShape
    {
        /// <summary>
        /// Returns a deep copy of the shape translated by the given vector.
        /// </summary>
        /// <seealso cref="operator +(ITranslatableShape{T}, IntVector2)"/>
        /// <seealso cref="operator +(IntVector2, ITranslatableShape{T})"/>
        /// <seealso cref="operator -(ITranslatableShape{T}, IntVector2)"/>
        public T Translate(IntVector2 translation);

        #region Default Implementations
        /// <summary>
        /// Returns a deep copy of the shape translated by the given vector.
        /// </summary>
        /// <seealso cref="Translate(IntVector2)"/>
        public static T operator +(ITranslatableShape<T> shape, IntVector2 translation) => shape.Translate(translation);
        /// <summary>
        /// Returns a deep copy of the shape translated by the given vector.
        /// </summary>
        /// <seealso cref="Translate(IntVector2)"/>
        public static T operator +(IntVector2 translation, ITranslatableShape<T> shape) => shape + translation;
        /// <summary>
        /// Returns a deep copy of the shape translated by the given vector.
        /// </summary>
        /// <seealso cref="Translate(IntVector2)"/>
        public static T operator -(ITranslatableShape<T> shape, IntVector2 translation) => shape + (-translation);

        T IDeepCopyableShape<T>.DeepCopy() => Translate(IntVector2.zero);
        #endregion
    }
}