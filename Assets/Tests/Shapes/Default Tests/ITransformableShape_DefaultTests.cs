using NUnit.Framework;

using PAC.Shapes.Interfaces;
using PAC.Tests.Shapes.RequiredTests;

namespace PAC.Tests.Shapes.DefaultTests
{
    /// <summary>
    /// Provides default implementations for the required tests in <see cref="ITransformableShape_RequiredTests"/>.
    /// </summary>
    public abstract class ITransformableShape_DefaultTests<T> : ITranslatableShape_DefaultTests<T>, ITransformableShape_RequiredTests where T : ITransformableShape<T>
    {
        [Test]
        [Category("Shapes")]
        public virtual void Flip() => IFlippableShape_DefaultTests<T>.Flip_Impl(testCases);

        [Test]
        [Category("Shapes")]
        public virtual void Rotate() => IRotatableShape_DefaultTests<T>.Rotate_Impl(testCases);
    }
}
