using PAC.Shapes.Interfaces;

namespace PAC.Tests.Shapes.RequiredTests
{
    /// <summary>
    /// Describes tests that should be implemented for every implementation of <see cref="ITranslatableShape{T}"/>.
    /// </summary>
    public interface ITranslatableShape_RequiredTests : IDeepCopyableShape_RequiredTests
    {
        /// <summary>
        /// Tests <see cref="ITranslatableShape{T}.Translate(DataStructures.IntVector2)"/>.
        /// </summary>
        public void Translate();
    }
}