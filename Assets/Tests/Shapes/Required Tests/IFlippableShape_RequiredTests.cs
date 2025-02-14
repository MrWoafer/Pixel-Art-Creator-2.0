using PAC.Shapes.Interfaces;

namespace PAC.Tests.Shapes.RequiredTests
{
    /// <summary>
    /// Describes tests that should be implemented for every implementation of <see cref="IFlippableShape{T}"/>.
    /// </summary>
    public interface IFlippableShape_RequiredTests : IDeepCopyableShape_RequiredTests
    {
        /// <summary>
        /// Tests <see cref="IFlippableShape{T}.Flip(FlipAxis)"/>.
        /// </summary>
        public void Flip();
    }
}