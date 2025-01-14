using NUnit.Framework;

using PAC.Shapes.Interfaces;
using PAC.Tests.Shapes.RequiredTests;

namespace PAC.Tests.Shapes.DefaultTests
{
    /// <summary>
    /// Provides default implementations for the required tests in <see cref="IIsometricShape_RequiredTests"/>.
    /// </summary>
    public abstract class IIsometricShape_DefaultTests<T> : IDeepCopyableShape_DefaultTests<T>, IIsometricShape_RequiredTests where T : IIsometricShape<T>
    {
        [Test]
        [Category("Shapes")]
        public virtual void Translate() => ITranslatableShape_DefaultTests<T>.Translate_Impl(testCases);

        [Test]
        [Category("Shapes")]
        public virtual void Flip() => IFlippableShape_DefaultTests<T>.Flip_Impl(testCases);

        [Test]
        [Category("Shapes")]
        public virtual void UnfilledIsBorderOfFilled() => IFillableShape_DefaultTests<T>.UnfilledIsBorderOfFilled_Impl(testCases);
    }
}
