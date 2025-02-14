using PAC.Shapes.Interfaces;

namespace PAC.Tests.Shapes.RequiredTests
{
    /// <summary>
    /// Describes tests that should be implemented for every implementation of <see cref="IDeepCopyableShape{T}"/>.
    /// </summary>
    public interface IDeepCopyableShape_RequiredTests : IShape_RequiredTests
    {
        /// <summary>
        /// Tests <see cref="IDeepCopyableShape{T}.DeepCopy"/>.
        /// </summary>
        public void DeepCopy();
    }
}