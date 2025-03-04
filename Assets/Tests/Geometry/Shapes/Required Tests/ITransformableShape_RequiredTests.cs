using PAC.Geometry.Shapes.Interfaces;

namespace PAC.Tests.Geometry.Shapes.RequiredTests
{
    /// <summary>
    /// Describes tests that should be implemented for every implementation of <see cref="ITransformableShape{T}"/>.
    /// </summary>
    public interface ITransformableShape_RequiredTests : ITranslatableShape_RequiredTests, IReflectableShape_RequiredTests, IRotatableShape_RequiredTests { }
}