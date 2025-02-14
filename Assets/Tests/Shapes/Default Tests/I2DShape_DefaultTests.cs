using NUnit.Framework;

using PAC.Shapes.Interfaces;
using PAC.Tests.Shapes.RequiredTests;

namespace PAC.Tests.Shapes.DefaultTests
{
    /// <summary>
    /// Provides default implementations for the required tests in <see cref="I2DShape_RequiredTests"/>.
    /// </summary>
    public abstract class I2DShape_DefaultTests<T> : ITransformableShape_DefaultTests<T>, I2DShape_RequiredTests where T : I2DShape<T>
    {
        [Test]
        [Category("Shapes")]
        public virtual void UnfilledIsBorderOfFilled() => IFillableShape_DefaultTests<T>.UnfilledIsBorderOfFilled_Impl(testCases);
    }
}
