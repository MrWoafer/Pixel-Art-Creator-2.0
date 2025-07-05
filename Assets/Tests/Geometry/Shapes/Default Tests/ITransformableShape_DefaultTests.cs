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
        public virtual void Flipped() => IFlippableShape_DefaultTests<T, A>.Flipped_Impl(testCases);

        [Test]
        [Category("Shapes")]
        public virtual void Rotated() => IRotatableShape_DefaultTests<T>.Rotated_Impl(testCases);
        [Test]
        [Category("Shapes")]
        public virtual void RotateMatchesRotated() => IRotatableShape_DefaultTests<T>.RotateMatchesRotated_Impl(testCases);
    }
}
