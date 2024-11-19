using PAC.Drawing;
using NUnit.Framework;
using PAC.DataStructures;
using System.Collections.Generic;
using System.Linq;

namespace PAC.Tests
{
    // I guess this should technically be IIShapeTests
    /// <summary>
    /// The tests that should be implemented for every shape.
    /// </summary>
    public interface IShapeTests
    {
        /// <summary>
        /// Tests that instances of the shape that are single points have the correct shape.
        /// </summary>
        public void ShapeSinglePoint();

        public void BoundingRect();

        public void Count();
        public void Contains();

        /// <summary>
        /// Tests that the shape's enumerator doesn't repeat any pixels it doesn't need to.
        /// </summary>
        public void NoRepeats();

        /// <summary>
        /// Tests that the shape of the shape is only determined by the width and height, not by the position.
        /// </summary>
        public void TranslationalInvariance();
        /// <summary>
        /// Tests that rotating the shape 90 degrees gives the same shape as creating one with the width/height swapped. (Note that this implies the same holds for 180 and 270 degrees.)
        /// </summary>
        public void RotationalInvariance();
        /// <summary>
        /// Tests that reflecting the shape gives the same shape as creating one with the corners reflected.
        /// </summary>
        public void ReflectiveInvariance();
    }
}
