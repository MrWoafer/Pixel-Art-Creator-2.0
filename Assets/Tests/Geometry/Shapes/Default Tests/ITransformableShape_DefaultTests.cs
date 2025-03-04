using NUnit.Framework;

using PAC.Geometry.Axes;
using PAC.Geometry.Shapes.Interfaces;
using PAC.Tests.Geometry.Shapes.RequiredTests;

namespace PAC.Tests.Geometry.Shapes.DefaultTests
{
    /// <summary>
    /// Provides default implementations for the required tests in <see cref="ITransformableShape_RequiredTests"/>.
    /// </summary>
    public abstract class ITransformableShape_DefaultTests<T, A> : ITranslatableShape_DefaultTests<T>, ITransformableShape_RequiredTests
        where T : ITransformableShape<T, A>
        where A : CardinalOrdinalAxis
    {
        [Test]
        [Category("Shapes")]
        public virtual void Flip() => IReflectableShape_DefaultTests<T, A>.Flip_Impl(testCases);

        [Test]
        [Category("Shapes")]
        public virtual void Rotate() => IRotatableShape_DefaultTests<T>.Rotate_Impl(testCases);
    }
}
