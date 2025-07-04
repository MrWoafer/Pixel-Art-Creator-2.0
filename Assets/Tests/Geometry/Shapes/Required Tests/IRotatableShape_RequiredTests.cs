using PAC.Geometry;
using PAC.Geometry.Shapes.Interfaces;

namespace PAC.Tests.Geometry.Shapes.RequiredTests
{
    /// <summary>
    /// Describes tests that should be implemented for every implementation of <see cref="IRotatableShape{T}"/>.
    /// </summary>
    public interface IRotatableShape_RequiredTests : IDeepCopyableShape_RequiredTests
    {
        /// <summary>
        /// Tests <see cref="IRotatableShape{T}.Rotated(QuadrantalAngle)"/>.
        /// </summary>
        public void Rotated();
    }
}