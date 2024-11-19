using NUnit.Framework;
using PAC.DataStructures;
using PAC.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace PAC.Tests
{
    public class RectangleTests
    {
        /// <summary>
        /// Tests that rectangles that are single points have the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapeSinglePoint()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 pixel in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
                {
                    Assert.True(new Shapes.Rectangle(pixel, pixel, filled).SequenceEqual(new IntVector2[] { pixel }), "Failed with " + pixel);
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void Count()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 topRight in new IntRect(IntVector2.zero, new IntVector2(10, 10)))
                {
                    IShapeTestHelper.Count(new Shapes.Rectangle(IntVector2.zero, topRight, filled));
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void Contains()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 topRight in new IntRect(IntVector2.zero, new IntVector2(10, 10)))
                {
                    IShapeTestHelper.Contains(new Shapes.Rectangle(IntVector2.zero, topRight, filled));
                }
            }
        }

        /// <summary>
        /// Tests that the rectangle enumerator doesn't repeat any pixels.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void NoRepeats()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 topRight in new IntRect(IntVector2.zero, new IntVector2(10, 10)))
                {
                    IShapeTestHelper.NoRepeatsAtAll(new Shapes.Rectangle(IntVector2.zero, topRight, filled));
                }
            }
        }
    }
}
