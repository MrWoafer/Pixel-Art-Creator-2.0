using PAC.Shapes.Interfaces;

namespace PAC.Tests.Shapes.RequiredTests
{
    /// <summary>
    /// Describes tests that should be implemented for every implementation of <see cref="ITransformableShape{T}"/>.
    /// </summary>
    public interface ITransformableShape_RequiredTests : ITranslatableShape_RequiredTests, IFlippableShape_RequiredTests, IRotatableShape_RequiredTests { }
}