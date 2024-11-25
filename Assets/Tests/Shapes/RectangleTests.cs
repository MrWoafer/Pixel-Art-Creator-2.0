using NUnit.Framework;
using PAC.DataStructures;
using PAC.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace PAC.Tests
{
    public class RectangleTests : IShapeTests
    {
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
        public void Shape()
        {
            foreach (IntVector2 bottomLeft in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
            {
                foreach (IntVector2 topRight in bottomLeft + new IntRect(IntVector2.zero, new IntVector2(5, 5)))
                {
                    Shapes.Rectangle rectangle = new Shapes.Rectangle(bottomLeft, topRight, true);
                    Assert.True(rectangle.boundingRect.ToHashSet().SetEquals(rectangle), "Failed with " + rectangle);
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void BoundingRect()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 bottomLeft in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
                {
                    foreach (IntVector2 topRight in bottomLeft + new IntRect(IntVector2.zero, new IntVector2(5, 5)))
                    {
                        IShapeTestHelper.BoundingRect(new Shapes.Rectangle(bottomLeft, topRight, filled));
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void Count()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 bottomLeft in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
                {
                    foreach (IntVector2 topRight in bottomLeft + new IntRect(IntVector2.zero, new IntVector2(5, 5)))
                    {
                        IShapeTestHelper.CountDistinct(new Shapes.Rectangle(bottomLeft, topRight, filled));
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void Contains()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 bottomLeft in new IntRect(new IntVector2(-2, -2), new IntVector2(2, 2)))
                {
                    foreach (IntVector2 topRight in bottomLeft + new IntRect(IntVector2.zero, new IntVector2(5, 5)))
                    {
                        IShapeTestHelper.Contains(new Shapes.Rectangle(bottomLeft, topRight, filled));
                    }
                }
            }
        }

        /// <summary>
        /// Tests that the enumerator doesn't repeat any pixels.
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

        [Test]
        [Category("Shapes")]
        public void Translate()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 bottomLeft in new IntRect(new IntVector2(-2, -2), new IntVector2(2, 2)))
                {
                    foreach (IntVector2 topRight in bottomLeft + new IntRect(IntVector2.zero, new IntVector2(5, 5)))
                    {
                        IShapeTestHelper.Translate(new Shapes.Rectangle(bottomLeft, topRight, filled));
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void Rotate()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 bottomLeft in new IntRect(new IntVector2(-2, -2), new IntVector2(2, 2)))
                {
                    foreach (IntVector2 topRight in bottomLeft + new IntRect(IntVector2.zero, new IntVector2(5, 5)))
                    {
                        IShapeTestHelper.Rotate(new Shapes.Rectangle(bottomLeft, topRight, filled));
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void Flip()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 bottomLeft in new IntRect(new IntVector2(-2, -2), new IntVector2(2, 2)))
                {
                    foreach (IntVector2 topRight in bottomLeft + new IntRect(IntVector2.zero, new IntVector2(5, 5)))
                    {
                        IShapeTestHelper.Flip(new Shapes.Rectangle(bottomLeft, topRight, filled));
                    }
                }
            }
        }
    }
}
