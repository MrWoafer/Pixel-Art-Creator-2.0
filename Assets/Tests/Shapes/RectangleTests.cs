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
                foreach (IntVector2 pixel in new IntRect(new IntVector2(-5, 5), new IntVector2(5, 5)))
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
                    Shapes.Rectangle rectangle = new Shapes.Rectangle(IntVector2.zero, topRight, filled);

                    int count = 0;
                    HashSet<IntVector2> visited = new HashSet<IntVector2>();
                    foreach (IntVector2 pixel in rectangle)
                    {
                        if (!visited.Contains(pixel))
                        {
                            count++;
                            visited.Add(pixel);
                        }
                    }

                    Assert.AreEqual(count, rectangle.Count, "Failed with " + rectangle);
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
                    Shapes.Rectangle rectangle = new Shapes.Rectangle(IntVector2.zero, topRight, filled);

                    IntRect testRegion = rectangle.boundingRect;
                    testRegion.bottomLeft -= IntVector2.one;
                    testRegion.topRight += IntVector2.one;

                    HashSet<IntVector2> rectanglePixels = rectangle.ToHashSet();

                    foreach (IntVector2 pixel in testRegion)
                    {
                        Assert.AreEqual(rectanglePixels.Contains(pixel), rectangle.Contains(pixel), "Failed with " + rectangle + " and " + pixel);
                    }
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
                    Shapes.Rectangle rectangle = new Shapes.Rectangle(IntVector2.zero, topRight, filled);

                    HashSet<IntVector2> visited = new HashSet<IntVector2>();
                    foreach (IntVector2 pixel in rectangle)
                    {
                        Assert.False(visited.Contains(pixel), "Failed with " + rectangle + " and " + pixel);
                        visited.Add(pixel);
                    }
                }
            }
        }
    }
}
