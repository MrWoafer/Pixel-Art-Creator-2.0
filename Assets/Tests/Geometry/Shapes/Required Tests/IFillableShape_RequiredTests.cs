using PAC.Geometry.Shapes.Interfaces;

namespace PAC.Tests.Geometry.Shapes.RequiredTests
{
    /// <summary>
    /// Describes tests that should be implemented for every implementation of <see cref="IFillableShape"/>.
    /// </summary>
    public interface IFillableShape_RequiredTests : IShape_RequiredTests
    {
        /// <summary>
        /// Tests that the unfilled version of a shape is precisely the border of the filled version.
        /// </summary>
        public void UnfilledIsBorderOfFilled();
    }
}