using PAC.Geometry.Shapes.Interfaces;

namespace PAC.Tests.Geometry.Shapes.RequiredTests
{
    /// <summary>
    /// Describes tests that should be implemented for every implementation of <see cref="IIsometricShape{T}"/>.
    /// </summary>
    public interface IIsometricShape_RequiredTests : IFillableShape_RequiredTests, ITranslatableShape_RequiredTests, IReflectableShape_RequiredTests { }
}