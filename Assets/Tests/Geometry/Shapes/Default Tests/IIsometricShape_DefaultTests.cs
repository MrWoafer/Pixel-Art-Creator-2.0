using NUnit.Framework;

using PAC.Geometry.Axes;
using PAC.Geometry.Shapes.Interfaces;
using PAC.Tests.Geometry.Shapes.RequiredTests;

namespace PAC.Tests.Geometry.Shapes.DefaultTests
{
    /// <summary>
    /// Provides default implementations for the required tests in <see cref="IIsometricShape_RequiredTests"/>.
    /// </summary>
    public abstract class IIsometricShape_DefaultTests<T> : IDeepCopyableShape_DefaultTests<T>, IIsometricShape_RequiredTests where T : IIsometricShape<T>
    {
        [Test]
        [Category("Shapes")]
        public virtual void Translated() => ITranslatableShape_DefaultTests<T>.Translated_Impl(testCases);

        [Test]
        [Category("Shapes")]
        public virtual void Flipped() => IFlippableShape_DefaultTests<T, VerticalAxis>.Flipped_Impl(testCases);

        [Test]
        [Category("Shapes")]
        public virtual void UnfilledIsBorderOfFilled() => IFillableShape_DefaultTests<T>.UnfilledIsBorderOfFilled_Impl(testCases);
    }
}
