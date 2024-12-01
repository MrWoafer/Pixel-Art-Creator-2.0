using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PAC.DataStructures;
using PAC.Drawing;

namespace PAC.Tests
{
    public class LineTests : IShapeTests<Shapes.Line>
    {
        protected override IEnumerable<Shapes.Line> testCases
        {
            get
            {
                foreach (IntVector2 start in new IntRect(new IntVector2(-2, -2), new IntVector2(2, 2)))
                {
                    foreach (IntVector2 end in start + new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
                    {
                        yield return new Shapes.Line(start, end);
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public override void ShapeSinglePoint()
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
            IEnumerable<IntVector2> Expected(int blockSize, int numBlocks)
            {
                for (int i = 0; i < numBlocks * blockSize; i++)
                {
                    yield return new IntVector2(i, i / blockSize);
                }
            }

            foreach (IntVector2 start in new IntRect(new IntVector2(-2, -2), new IntVector2(2, 2)))
            {
                for (int numBlocks = 1; numBlocks <= 10; numBlocks++)
                {
                    for (int blockSize = 1; blockSize <= 10; blockSize++)
                    {
                        foreach (RotationAngle angle in new RotationAngle[] { RotationAngle._0, RotationAngle._90, RotationAngle._180, RotationAngle.Minus90 })
                        {
                            foreach (FlipAxis axis in new FlipAxis[] { FlipAxis.None, FlipAxis._45Degrees })
                            {
                                Shapes.Line line = new Shapes.Line(start, start + new IntVector2(blockSize * numBlocks - 1, numBlocks - 1).Flip(axis).Rotate(angle));
                                IEnumerable<IntVector2> expected = Expected(blockSize, numBlocks).Select(p => start + p.Flip(axis).Rotate(angle));
                                Assert.True(expected.SequenceEqual(line), "Failed with " + line);
                                Assert.True(line.isPerfect, "Failed with " + line);
                            }
                        }
                    }
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

        /// <summary>
        /// Tests that lines are oriented from start to end.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void Orientation()
        {
            foreach (Shapes.Line line in testCases)
            {
                Assert.True(line.First() == line.start, "Failed with " + line);
                Assert.True(line.Last() == line.end, "Failed with " + line);
            }
        }

        /// <summary>
        /// Tests that indexing lines works correctly.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void Indexing()
        {
            foreach (Shapes.Line line in testCases)
            {
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
        public void PointIsToLeft()
        {
            foreach (Shapes.Line line in testCases)
            {
                foreach (IntVector2 pixel in line)
                {
                    for (int x = pixel.x; x >= line.boundingRect.bottomLeft.x - 2; x--)
                    {
                        IntVector2 test = new IntVector2(x, pixel.y);
                        Assert.True(line.PointIsToLeft(test), "Failed with " + line + " and " + test);
                    }

                    for (int x = pixel.x + 1; x <= line.boundingRect.topRight.x + 2; x++)
                    {
                        IntVector2 test = new IntVector2(x, pixel.y);
                        if (!line.Contains(pixel))
                        {
                            Assert.False(line.PointIsToLeft(test), "Failed with " + line + " and " + test);
                        }
                    }
                }

                for (int x = line.boundingRect.bottomLeft.x - 2; x <= line.boundingRect.topRight.x + 2; x++)
                {
                    for (int y = line.boundingRect.topRight.y + 1; y <= line.boundingRect.topRight.y + 2; y++)
                    {
                        IntVector2 test = new IntVector2(x, y);
                        Assert.False(line.PointIsToLeft(test), "Failed with " + line + " and " + test);
                    }

                    for (int y = line.boundingRect.bottomLeft.y - 1; y >= line.boundingRect.topRight.y - 2; y--)
                    {
                        IntVector2 test = new IntVector2(x, y);
                        Assert.False(line.PointIsToLeft(test), "Failed with " + line + " and " + test);
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void PointIsToRight()
        {
            foreach (Shapes.Line line in testCases)
            {
                foreach (IntVector2 pixel in line)
                {
                    for (int x = pixel.x; x <= line.boundingRect.topRight.x + 2; x++)
                    {
                        IntVector2 test = new IntVector2(x, pixel.y);
                        Assert.True(line.PointIsToRight(test), "Failed with " + line + " and " + test);
                    }

                    for (int x = pixel.x - 1; x >= line.boundingRect.bottomLeft.x - 2; x--)
                    {
                        IntVector2 test = new IntVector2(x, pixel.y);
                        if (!line.Contains(pixel))
                        {
                            Assert.False(line.PointIsToRight(test), "Failed with " + line + " and " + test);
                        }
                    }
                }

                for (int x = line.boundingRect.bottomLeft.x - 2; x <= line.boundingRect.topRight.x + 2; x++)
                {
                    for (int y = line.boundingRect.topRight.y + 1; y <= line.boundingRect.topRight.y + 2; y++)
                    {
                        IntVector2 test = new IntVector2(x, y);
                        Assert.False(line.PointIsToRight(test), "Failed with " + line + " and " + test);
                    }

                    for (int y = line.boundingRect.bottomLeft.y - 1; y >= line.boundingRect.topRight.y - 2; y--)
                    {
                        IntVector2 test = new IntVector2(x, y);
                        Assert.False(line.PointIsToRight(test), "Failed with " + line + " and " + test);
                    }
                }
            }
        }
    }
}
