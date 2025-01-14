using PAC.Shapes.Interfaces;

namespace PAC.Tests.Shapes.RequiredTests
{
    /// <summary>
    /// Describes tests that should be implemented for every implementation of <see cref="IRotatableShape{T}"/>.
    /// </summary>
    public interface IRotatableShape_RequiredTests : IDeepCopyableShape_RequiredTests
    {
        /// <summary>
        /// Tests <see cref="IRotatableShape{T}.Rotate(RotationAngle)"/>.
        /// </summary>
        public void Rotate();
    }
}