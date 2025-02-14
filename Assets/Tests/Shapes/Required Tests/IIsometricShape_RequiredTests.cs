using PAC.Shapes.Interfaces;

namespace PAC.Tests.Shapes.RequiredTests
{
    /// <summary>
    /// Describes tests that should be implemented for every implementation of <see cref="IIsometricShape{T}"/>.
    /// </summary>
    public interface IIsometricShape_RequiredTests : IFillableShape_RequiredTests, ITranslatableShape_RequiredTests, IFlippableShape_RequiredTests { }
}