using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PAC.DataStructures;
using PAC.Drawing;

namespace PAC.Tests
{
    public class LineTests : IShapeTests
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
                Assert.True(new Shapes.Line(pixel, pixel).SequenceEqual(new IntVector2[] { pixel }), "Failed with " + pixel);
            }
        }

        /// <summary>
        /// Tests that lines that can be drawn with constant-size blocks are drawn as such. E.g. a 12x4 line can be drawn as 4 horizontal blocks of 3 pixels.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapePerfect()
        {
            IEnumerable<IntVector2> Expected(bool isMoreHorizontal, int blockSize, int numBlocks)
            {
                if (isMoreHorizontal)
                {
                    for (int i = 0; i < numBlocks * blockSize; i++)
                    {
                        yield return new IntVector2(i, i / blockSize);
                    }
                    yield break;
                }

                for (int i = 0; i < numBlocks * blockSize; i++)
                {
                    yield return new IntVector2(i / blockSize, i);
                }
            }

            for (int numBlocks = 1; numBlocks <= 10; numBlocks++)
            {
                for (int blockSize = 1; blockSize <= 10; blockSize++)
                {
                    // More horizontal than vertical
                    Shapes.Line line = new Shapes.Line(IntVector2.zero, new IntVector2(blockSize * numBlocks - 1, numBlocks - 1));
                    Assert.True(line.SequenceEqual(Expected(true, blockSize, numBlocks)), "Failed with " + line);
                    Assert.True(line.isPerfect, "Failed with " + line);

                    // More vertical than horizontal
                    line = new Shapes.Line(IntVector2.zero, new IntVector2(numBlocks - 1, blockSize * numBlocks - 1));
                    Assert.True(line.SequenceEqual(Expected(false, blockSize, numBlocks)), "Failed with " + line);
                    Assert.True(line.isPerfect, "Failed with " + line);
                }
            }
        }

        /// <summary>
        /// Tests that an example line has the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapeExample1()
        {
            Shapes.Line line = new Shapes.Line(IntVector2.zero, new IntVector2(2, 4));
            IntVector2[] expected =
            {
                new IntVector2(0, 0), new IntVector2(0, 1), new IntVector2(1, 2), new IntVector2(2, 3), new IntVector2(2, 4)
            };

            Assert.True(expected.SequenceEqual(line));
        }

        /// <summary>
        /// Tests that an example line has the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapeExample2()
        {
            Shapes.Line line = new Shapes.Line(IntVector2.zero, new IntVector2(4, 1));
            IntVector2[] expected =
            {
                new IntVector2(0, 0), new IntVector2(1, 0), new IntVector2(2, 0), new IntVector2(3, 1), new IntVector2(4, 1)
            };

            Assert.True(expected.SequenceEqual(line));
        }

        [Test]
        [Category("Shapes")]
        public void BoundingRect()
        {
            foreach (IntVector2 start in new IntRect(new IntVector2(-1, -1), new IntVector2(1, 1)))
            {
                foreach (IntVector2 end in start + new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
                {
                    IShapeTestHelper.BoundingRect(new Shapes.Line(start, end));
                }
            }
        }

        /// <summary>
        /// Tests that lines are oriented from start to end.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void Orientation()
        {
            foreach (IntVector2 start in new IntRect(new IntVector2(-1, -1), new IntVector2(1, 1)))
            {
                foreach (IntVector2 end in start + new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
                {
                    Shapes.Line line = new Shapes.Line(start, end);
                    Assert.True(line.First() == line.start, "Failed with " + line);
                    Assert.True(line.Last() == line.end, "Failed with " + line);
                }
            }
        }

        /// <summary>
        /// Tests that indexing lines works correctly.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void Indexing()
        {
            foreach (IntVector2 start in new IntRect(new IntVector2(-1, -1), new IntVector2(1, 1)))
            {
                foreach (IntVector2 end in start + new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
                {
                    Shapes.Line line = new Shapes.Line(start, end);

                    Assert.Throws<IndexOutOfRangeException>(() => { IntVector2 x = line[-1]; }, "Failed with " + line);

                    int index = 0;
                    foreach (IntVector2 pixel in line)
                    {
                        Assert.AreEqual(pixel, line[index], "Failed with " + line + " at index " + index);
                        index++;
                    }

                    Assert.Throws<IndexOutOfRangeException>(() => { IntVector2 x = line[index]; }, "Failed with " + line);

                    // Test hat syntax
                    Assert.AreEqual(line.end, line[^1], "Failed with " + line);
                }
            }
        }

        /// <summary>
        /// Tests that indexing with Range lines works correctly.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void IndexingRange()
        {
            Shapes.Line line = new Shapes.Line(IntVector2.zero, new IntVector2(4, 1));
            Assert.True(line[..].SequenceEqual(line));
            Assert.True(line[1..].SequenceEqual(new IntVector2[] { new IntVector2(1, 0), new IntVector2(2, 0), new IntVector2(3, 1), new IntVector2(4, 1) }));
            Assert.True(line[1..5].SequenceEqual(new IntVector2[] { new IntVector2(1, 0), new IntVector2(2, 0), new IntVector2(3, 1), new IntVector2(4, 1) }));
            Assert.True(line[..^1].SequenceEqual(new IntVector2[] { new IntVector2(0, 0), new IntVector2(1, 0), new IntVector2(2, 0), new IntVector2(3, 1) }));
            Assert.True(line[1..4].SequenceEqual(new IntVector2[] { new IntVector2(1, 0), new IntVector2(2, 0), new IntVector2(3, 1) }));
        }

        [Test]
        [Category("Shapes")]
        public void Count()
        {
            foreach (IntVector2 end in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
            {
                IShapeTestHelper.Count(new Shapes.Line(IntVector2.zero, end));
            }
        }

        [Test]
        [Category("Shapes")]
        public void Contains()
        {
            foreach (IntVector2 end in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
            {
                IShapeTestHelper.Contains(new Shapes.Line(IntVector2.zero, end));
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
                IShapeTestHelper.NoRepeatsAtAll(new Shapes.Line(IntVector2.zero, end));
            }
        }

        /// <summary>
        /// Tests that the shape of a line is only determined by the width and height, not by the position.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void TranslationalInvariance()
        {
            foreach (IntVector2 start in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
            {
                foreach (IntVector2 end in start + new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
                {
                    Shapes.Line line = new Shapes.Line(start, end);
                    Shapes.Line expected = new Shapes.Line(IntVector2.zero, line.end - line.start);
                    Assert.True(expected.SequenceEqual(line.Select(p => p - line.start)), "Failed with " + line);
                }
            }
        }

        /// <summary>
        /// Tests that rotating a line 90 degrees gives the same shape as creating one with the width/height swapped.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void RotationalInvariance()
        {
            foreach (IntVector2 end in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
            {
                Shapes.Line line = new Shapes.Line(IntVector2.zero, end);
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
                foreach (IntVector2 end in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
                {
                    Shapes.Line line = new Shapes.Line(IntVector2.zero, end);
                    Shapes.Line expected = new Shapes.Line(line.start.Flip(axis), line.end.Flip(axis));
                    Assert.True(expected.SequenceEqual(line.Select(p => p.Flip(axis))), "Failed with " + line + " and FlipAxis." + axis);
                }
            }
        }
    }
}
