namespace PAC.Tests.Geometry.Shapes.RequiredTests
{
    /// <summary>
    /// Describes tests that should be implemented for every implementation of <see cref="IFlippableShape{T}"/>.
    /// </summary>
    public interface IFlippableShape_RequiredTests : IDeepCopyableShape_RequiredTests
    {
        /// <summary>
        /// Tests <see cref="IFlippableShape{T}.Flip(CardinalOrdinalAxis)"/>.
        /// </summary>
        public void Flip();
    }
}