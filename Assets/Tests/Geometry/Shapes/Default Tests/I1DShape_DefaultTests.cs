using PAC.Geometry.Shapes.Interfaces;
using PAC.Tests.Geometry.Shapes.RequiredTests;

namespace PAC.Tests.Geometry.Shapes.DefaultTests
{
    /// <summary>
    /// Provides default implementations for the required tests in <see cref="I1DShape_RequiredTests"/>.
    /// </summary>
    public abstract class I1DShape_DefaultTests<T> : ITransformableShape_DefaultTests<T>, I1DShape_RequiredTests where T : I1DShape<T> { }
}
