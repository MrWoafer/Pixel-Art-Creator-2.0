using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PAC.DataStructures;
using PAC.Drawing;

namespace PAC.Tests
{
    public class LineTests
    {
        /// <summary>
        /// Tests that lines that are single points have the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapeSinglePoint()
        {
            foreach (IntVector2 pixel in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
            {
                Assert.True(new Shapes.Line(pixel, pixel).SequenceEqual(new IntVector2[] { pixel }));
            }
        }

        [Test]
        [Category("Shapes")]
        public void Count()
        {
            foreach (IntVector2 end in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
            {
                Shapes.Line line = new Shapes.Line(IntVector2.zero, end);

                int count = 0;
                HashSet<IntVector2> visited = new HashSet<IntVector2>();
                foreach (IntVector2 pixel in line)
                {
                    if (!visited.Contains(pixel))
                    {
                        count++;
                        visited.Add(pixel);
                    }
                }
                Assert.AreEqual(count, line.Count, "Failed with " + line);
            }
        }

        [Test]
        [Category("Shapes")]
        public void Contains()
        {
            foreach (IntVector2 end in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
            {
                Shapes.Line line = new Shapes.Line(IntVector2.zero, end);

                IntRect testRegion = line.boundingRect;
                testRegion.bottomLeft -= IntVector2.one;
                testRegion.topRight += IntVector2.one;

                HashSet<IntVector2> linePixels = line.ToHashSet();

                foreach (IntVector2 pixel in testRegion)
                {
                    Assert.AreEqual(linePixels.Contains(pixel), line.Contains(pixel), "Failed with " + line + " and " + pixel);
                }
            }
        }

        /// <summary>
        /// Tests that the line enumerator doesn't repeat any pixels.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void NoRepeats()
        {
            foreach (IntVector2 end in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
            {
                Shapes.Line line = new Shapes.Line(IntVector2.zero, end);

                HashSet<IntVector2> visited = new HashSet<IntVector2>();
                foreach (IntVector2 pixel in line)
                {
                    Assert.False(visited.Contains(pixel), "Failed with " + line + " and " + pixel);
                    visited.Add(pixel);
                }
            }
        }

        private IEnumerable<Shapes.Line> Datapoints()
        {
            foreach (IntVector2 start in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
            {
                foreach (IntVector2 end in start + new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
                {
                    yield return new Shapes.Line(start, end);
                }
            }
        }

        /// <summary>
        /// Tests that the shape of a line is only determined by the width and height, not by the position.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void TranslationalInvariance()
        {
            foreach (Shapes.Line translated in Datapoints())
            {
                Shapes.Line expected = new Shapes.Line(IntVector2.zero, translated.end - translated.start);
                Assert.True(expected.SequenceEqual(translated.Select(p => p - translated.start)), "Failed with " + translated);
            }
        }

        /// <summary>
        /// Tests that rotating a line 90 degrees gives the same shape as creating one with the width/height swapped.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void RotationalInvariance()
        {
            foreach (Shapes.Line line in Datapoints())
            {
                Shapes.Line expected = new Shapes.Line(IntVector2.zero, (line.end - line.start).Rotate(RotationAngle._90));
                Assert.True(expected.SequenceEqual(line.Select(p => (p - line.start).Rotate(RotationAngle._90))), "Failed with " + line);
            }
        }

        /// <summary>
        /// Tests that reflecting a line gives the same shape as creating one with the start/end reflected.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ReflectiveInvariance()
        {
            foreach (FlipAxis axis in new FlipAxis[] { FlipAxis.Vertical, FlipAxis.Horizontal, FlipAxis._45Degrees, FlipAxis.Minus45Degrees })
            {
                foreach (Shapes.Line line in Datapoints())
                {
                    Shapes.Line expected = new Shapes.Line(line.start.Flip(axis), line.end.Flip(axis));
                    Assert.True(expected.SequenceEqual(line.Select(p => p.Flip(axis))), "Failed with " + line + " and FlipAxis." + axis);
                }
            }
        }
    }
}
