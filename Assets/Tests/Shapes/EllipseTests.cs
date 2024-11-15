using NUnit.Framework;
using PAC.DataStructures;
using PAC.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace PAC.Tests
{
    public class EllipseTests
    {
        /// <summary>
        /// Tests that ellipses that are single points have the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapeSinglePoint()
        {
            foreach (bool filled in new bool[] { false, true})
            {
                foreach (IntVector2 pixel in new IntRect(new IntVector2(-5, 5), new IntVector2(5, 5)))
                {
                    Assert.True(new Shapes.Ellipse(pixel, pixel, filled).SequenceEqual(new IntVector2[] { pixel }));
                }
            }
        }

        /// <summary>
        /// Tests that 2xn and nx2 ellipses have the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void Shape2xNAndNx2()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (int x in new int[] { -1, 1 })
                {
                    for (int y = -5; y <= 5; y++)
                    {
                        Shapes.Ellipse ellipse = new Shapes.Ellipse(IntVector2.zero, new IntVector2(x, y), filled);
                        Assert.True(ellipse.ToHashSet().SetEquals(ellipse.boundingRect), "Failed with " + ellipse);
                    }
                }
            }

            foreach (bool filled in new bool[] { false, true })
            {
                foreach (int y in new int[] { -1, 1 })
                {
                    for (int x = -5; x <= 5; x++)
                    {
                        Shapes.Ellipse ellipse = new Shapes.Ellipse(IntVector2.zero, new IntVector2(x, y), filled);
                        Assert.True(ellipse.ToHashSet().SetEquals(ellipse.boundingRect), "Failed with " + ellipse);
                    }
                }
            }
        }

        /// <summary>
        /// Tests that 3x3 ellipses have the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void Shape3x3()
        {
            foreach (IntVector2 centre in new IntRect(new IntVector2(-5, 5), new IntVector2(5, 5)))
            {
                Shapes.Ellipse ellipse = new Shapes.Ellipse(centre - IntVector2.one, centre + IntVector2.one, false);
                Assert.True(ellipse.ToHashSet().SetEquals(centre + new IntVector2[] { IntVector2.up, IntVector2.right, IntVector2.down, IntVector2.left }));

                ellipse = new Shapes.Ellipse(centre - IntVector2.one, centre + IntVector2.one, true);
                Assert.True(ellipse.ToHashSet().SetEquals(centre + new IntVector2[] { IntVector2.zero, IntVector2.up, IntVector2.right, IntVector2.down, IntVector2.left }));
            }
        }

        /// <summary>
        /// Tests that an example ellips has the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapeExample1()
        {
            Shapes.Ellipse ellipse = new Shapes.Ellipse(IntVector2.zero, new IntVector2(6, 10), false);
            IntVector2[] expected =
            {
                new IntVector2(3, 10), new IntVector2(4, 10), new IntVector2(5, 9), new IntVector2(5, 8), new IntVector2(6, 7), new IntVector2(6, 6), new IntVector2(6, 5), new IntVector2(6, 4),
                new IntVector2(6, 3), new IntVector2(5, 2), new IntVector2(5, 1), new IntVector2(4, 0), new IntVector2(3, 0), new IntVector2(2, 0), new IntVector2(1, 1), new IntVector2(1, 2),
                new IntVector2(0, 3), new IntVector2(0, 4), new IntVector2(0, 5), new IntVector2(0, 6), new IntVector2(0, 7), new IntVector2(1, 8), new IntVector2(1, 9), new IntVector2(2, 10)
            };

            Assert.True(expected.SequenceEqual(ellipse));
        }

        /// <summary>
        /// Tests that an example ellips has the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapeExample2()
        {
            Shapes.Ellipse ellipse = new Shapes.Ellipse(IntVector2.zero, new IntVector2(5, 10), false);
            IntVector2[] expected =
            {
                new IntVector2(3, 10), new IntVector2(4, 9), new IntVector2(5, 8), new IntVector2(5, 7), new IntVector2(5, 6), new IntVector2(5, 5), new IntVector2(5, 4), new IntVector2(5, 3),
                new IntVector2(5, 2), new IntVector2(4, 1), new IntVector2(3, 0), new IntVector2(2, 0), new IntVector2(1, 1), new IntVector2(0, 2), new IntVector2(0, 3), new IntVector2(0, 4),
                new IntVector2(0, 5), new IntVector2(0, 6), new IntVector2(0, 7), new IntVector2(0, 8), new IntVector2(1, 9), new IntVector2(2, 10)
            };

            Assert.True(expected.SequenceEqual(ellipse));
        }

        private IEnumerable<Shapes.Ellipse> Datapoints(bool includeFilled, bool includeUnfilled)
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
                            yield return new Shapes.Ellipse(bottomLeft, bottomLeft + new IntVector2(width - 1, height - 1), filled);
                        }
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void Count()
        {
            foreach(Shapes.Ellipse ellipse in Datapoints(true, true))
            {
                int count = 0;
                HashSet<IntVector2> visited = new HashSet<IntVector2>();
                foreach (IntVector2 pixel in ellipse)
                {
                    if (!visited.Contains(pixel))
                    {
                        count++;
                        visited.Add(pixel);
                    }
                }
                Assert.AreEqual(count, ellipse.Count, "Failed with " + ellipse);
            }
        }

        [Test]
        [Category("Shapes")]
        public void Contains()
        {
            foreach (Shapes.Ellipse ellipse in Datapoints(true, true))
            {
                IntRect testRegion = ellipse.boundingRect;
                testRegion.bottomLeft -= IntVector2.one;
                testRegion.topRight += IntVector2.one;

                HashSet<IntVector2> ellipsePixels = ellipse.ToHashSet();

                foreach (IntVector2 pixel in testRegion)
                {
                    Assert.AreEqual(ellipsePixels.Contains(pixel), ellipse.Contains(pixel), "Failed with " + ellipse + " and " + pixel);
                }
            }
        }

        /// <summary>
        /// Tests that the Ellipse enumerator doesn't repeat any pixels.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void NoRepeats()
        {
            foreach (Shapes.Ellipse ellipse in Datapoints(true, true))
            {
                HashSet<IntVector2> visited = new HashSet<IntVector2>();
                foreach (IntVector2 pixel in ellipse)
                {
                    Assert.False(visited.Contains(pixel), "Failed with " + ellipse + " and " + pixel);
                    visited.Add(pixel);
                }
            }
        }

        /// <summary>
        /// Tests that the shape of the Ellipse is only determined by the width and height, not by the position.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void TranslationalInvariance()
        {
            foreach (Shapes.Ellipse translated in Datapoints(true, true))
            {
                Shapes.Ellipse expected = new Shapes.Ellipse(IntVector2.zero, translated.topRight - translated.bottomLeft, translated.filled);
                Assert.True(expected.SequenceEqual(translated.Select(x => x - translated.bottomLeft)), "Failed with " + translated);
            }
        }

        /// <summary>
        /// Tests that rotating an Ellipse 90 degrees gives the same shape as creating one with the width/height swapped.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void RotationalInvariance()
        {
            foreach (Shapes.Ellipse translated in Datapoints(true, true))
            {
                Shapes.Ellipse expected = new Shapes.Ellipse(IntVector2.zero, new IntVector2(translated.height - 1, translated.width - 1), translated.filled);
                Assert.True(expected.ToHashSet().SetEquals(translated.Select(x => (x - translated.bottomLeft).Rotate(RotationAngle._90) + new IntVector2(0, translated.width - 1))),
                    "Failed with " + translated);
            }
        }
    }
}
