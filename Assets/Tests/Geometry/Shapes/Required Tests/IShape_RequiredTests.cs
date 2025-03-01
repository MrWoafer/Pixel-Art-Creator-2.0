using System.Collections.Generic;

using PAC.Interfaces;
using PAC.Geometry.Shapes.Interfaces;

namespace PAC.Tests.Geometry.Shapes.RequiredTests
{
    /// <summary>
    /// Describes tests that should be implemented for every implementation of <see cref="IShape"/>.
    /// </summary>
    public interface IShape_RequiredTests
    {
        /// <summary>
        /// Tests that the shape has at least one point.
        /// </summary>
        public void NotEmpty();

        /// <summary>
        /// Tests that the <see cref="IReadOnlyCollection{T}.Count"/> property of the <see cref="IShape"/> matches the length of the enumerator.
        /// </summary>
        public void Count();

        /// <summary>
        /// Tests <see cref="IReadOnlyContains{T}.Contains(T)"/> method of the <see cref="IShape"/>.
        /// </summary>
        public void Contains();

        /// <summary>
        /// Tests <see cref="IShape.boundingRect"/>.
        /// </summary>
        public void BoundingRect();

        /// <summary>
        /// Tests that instances of the shape that are single points have the correct shape.
        /// </summary>
        public void ShapeSinglePoint();

        /// <summary>
        /// Tests that the shape has exactly one connected component (defined in terms of pixels being adjacent, including diagonally).
        /// </summary>
        public void Connected();
    }
}