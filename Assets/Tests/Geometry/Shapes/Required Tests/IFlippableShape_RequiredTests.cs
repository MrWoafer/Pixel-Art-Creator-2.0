using PAC.Geometry.Shapes.Interfaces;

namespace PAC.Tests.Geometry.Shapes.RequiredTests
{
    /// <summary>
    /// Describes tests that should be implemented for every implementation of <see cref="IFlippableShape{T}"/>.
    /// </summary>
    public interface IFlippableShape_RequiredTests : IDeepCopyableShape_RequiredTests
    {
        /// <summary>
        /// Tests <see cref="IFlippableShape{T, A}.Flipped(A)"/>.
        /// </summary>
        public void Flipped();

        /// <summary>
        /// Tests that <see cref="IFlippableShape{T, A}.Flip(A)"/> and <see cref="IFlippableShape{T, A}.Flipped(A)"/> do the same transformation.
        /// </summary>
        public void FlipMatchesFlipped();
    }
}