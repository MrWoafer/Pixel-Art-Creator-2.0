using PAC.Geometry.Axes;
using PAC.Geometry.Shapes.Interfaces;

namespace PAC.Tests.Geometry.Shapes.RequiredTests
{
    /// <summary>
    /// Describes tests that should be implemented for every implementation of <see cref="IReflectableShape{T, A}"/>.
    /// </summary>
    public interface IReflectableShape_RequiredTests : IDeepCopyableShape_RequiredTests
    {
        /// <summary>
        /// Tests <see cref="IReflectableShape{T, A}.Reflect(A)"/>.
        /// </summary>
        public void Flip();
    }
}