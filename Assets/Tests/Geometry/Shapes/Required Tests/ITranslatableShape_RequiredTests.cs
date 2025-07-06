using PAC.Geometry;
using PAC.Geometry.Shapes.Interfaces;

namespace PAC.Tests.Geometry.Shapes.RequiredTests
{
    /// <summary>
    /// Describes tests that should be implemented for every implementation of <see cref="ITranslatableShape{T}"/>.
    /// </summary>
    public interface ITranslatableShape_RequiredTests : IDeepCopyableShape_RequiredTests
    {
        /// <summary>
        /// Tests <see cref="ITranslatableShape{T}.Translated(IntVector2)"/>.
        /// </summary>
        public void Translated();

        /// <summary>
        /// Tests that <see cref="ITranslatableShape{T}.Translate(IntVector2)"/> and <see cref="ITranslatableShape{T}.Translated(IntVector2)"/> do the same transformation.
        /// </summary>
        public void TranslateMatchesTranslated();
    }
}