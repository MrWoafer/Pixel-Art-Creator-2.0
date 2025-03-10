using PAC.Geometry.Shapes.Interfaces;

namespace PAC.Tests.Geometry.Shapes.RequiredTests
{
    /// <summary>
    /// Describes tests that should be implemented for every implementation of <see cref="I2DShape{T}"/>.
    /// </summary>
    public interface I2DShape_RequiredTests : IFillableShape_RequiredTests, ITransformableShape_RequiredTests { }
}