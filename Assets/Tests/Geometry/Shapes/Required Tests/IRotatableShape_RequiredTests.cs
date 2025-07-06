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

        /// <summary>
        /// Tests that <see cref="IRotatableShape{T}.Rotate(QuadrantalAngle)"/> and <see cref="IRotatableShape{T}.Rotated(QuadrantalAngle)"/> do the same transformation.
        /// </summary>
        public void RotateMatchesRotated();
    }
}