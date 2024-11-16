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
                    Assert.True(new Shapes.Rectangle(pixel, pixel, filled).SequenceEqual(new IntVector2[] { pixel }));
                }
            }
        }

        private IEnumerable<Shapes.Rectangle> Datapoints(bool includeFilled, bool includeUnfilled)
        {
            List<bool> filledDatapoints = new List<bool>();
            if (includeFilled)
            {
                filledDatapoints.Add(true);
            }
            if (includeUnfilled)
            {
                filledDatapoints.Add(false);
            }

            foreach (bool filled in filledDatapoints)
            {
                for (int width = 1; width <= 5; width++)
                {
                    for (int height = 1; height <= 5; height++)
                    {
                        foreach (IntVector2 bottomLeft in new IntRect(new IntVector2(-5, 5), new IntVector2(5, 5)))
                        {
                            yield return new Shapes.Rectangle(bottomLeft, bottomLeft + new IntVector2(width - 1, height - 1), filled);
                        }
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void Count()
        {
            foreach (Shapes.Rectangle rectangle in Datapoints(true, true))
            {
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

        [Test]
        [Category("Shapes")]
        public void Contains()
        {
            foreach (Shapes.Rectangle rectangle in Datapoints(true, true))
            {
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

        /// <summary>
        /// Tests that the rectangle enumerator doesn't repeat any pixels.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void NoRepeats()
        {
            foreach (Shapes.Rectangle rectangle in Datapoints(true, true))
            {
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
