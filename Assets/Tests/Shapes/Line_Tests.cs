using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using PAC.DataStructures;
using PAC.Shapes;
using PAC.Tests.Shapes.DefaultTests;
using PAC.Tests.Shapes.RequiredTests;
using PAC.Tests.Shapes.TestUtils;

namespace PAC.Tests.Shapes
{
    public class Line_Tests : I1DShape_DefaultTests<Line>, I1DShape_RequiredTests
    {
        protected override IEnumerable<Line> testCases
        {
            get
            {
                foreach (IntVector2 start in new IntRect(new IntVector2(-2, -2), new IntVector2(2, 2)))
                {
                    foreach (IntVector2 end in start + new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
                    {
                        yield return new Line(start, end);
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public override void ShapeSinglePoint()
        {
            foreach (IntVector2 point in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
            {
                CollectionAssert.AreEqual(new IntVector2[] { point }, new Line(point), $"Failed with {point}.");
                CollectionAssert.AreEqual(new Line(point), new Line(point, point), $"Failed with {point}.");
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
                                Line line = new Line(start, start + new IntVector2(blockSize * numBlocks - 1, numBlocks - 1).Flip(axis).Rotate(angle));
                                IEnumerable<IntVector2> expected = Expected(blockSize, numBlocks).Select(p => start + p.Flip(axis).Rotate(angle));
                                CollectionAssert.AreEqual(expected, line, $"Failed with {line}.");
                                Assert.True(line.isPerfect, $"Failed with {line}.");
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
            Line line = new Line(IntVector2.zero, new IntVector2(2, 4));
            IntVector2[] expected =
            {
                new IntVector2(0, 0), new IntVector2(0, 1), new IntVector2(1, 2), new IntVector2(2, 3), new IntVector2(2, 4)
            };

            CollectionAssert.AreEqual(expected, line);
        }

        /// <summary>
        /// Tests that an example line has the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapeExample2()
        {
            Line line = new Line(IntVector2.zero, new IntVector2(4, 1));
            IntVector2[] expected =
            {
                new IntVector2(0, 0), new IntVector2(1, 0), new IntVector2(2, 0), new IntVector2(3, 1), new IntVector2(4, 1)
            };

            CollectionAssert.AreEqual(expected, line);
        }

        /// <summary>
        /// Tests that an example line has the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapeExample3()
        {
            Line line = new Line(IntVector2.zero, new IntVector2(5, 3));
            IntVector2[] expected =
            {
                new IntVector2(0, 0), new IntVector2(1, 0), new IntVector2(2, 1), new IntVector2(3, 2), new IntVector2(4, 3), new IntVector2(5, 3)
            };

            CollectionAssert.AreEqual(expected, line);
        }

        [Test]
        [Category("Shapes")]
        public void NoRepeats()
        {
            foreach (Line line in testCases)
            {
                ShapeAssert.NoRepeats(line);
            }
        }

        /// <summary>
        /// Tests that lines are oriented from start to end.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void Orientation()
        {
            foreach (Line line in testCases)
            {
                Assert.AreEqual(line.First(),  line.start, $"Failed with {line}.");
                Assert.AreEqual(line.Last(), line.end, $"Failed with {line}.");
            }
        }

        /// <summary>
        /// Tests that indexing lines works correctly.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void Indexing()
        {
            foreach (Line line in testCases)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => { IntVector2 x = line[-1]; }, $"Failed with {line}.");

                int index = 0;
                foreach (IntVector2 pixel in line)
                {
                    Assert.AreEqual(pixel, line[index], $"Failed with {line} at index {index}.");
                    index++;
                }

                Assert.Throws<ArgumentOutOfRangeException>(() => { IntVector2 x = line[index]; }, $"Failed with {line}.");

                // Test hat syntax
                Assert.AreEqual(line.end, line[^1], $"Failed with {line}.");
            }
        }

        /// <summary>
        /// Tests that indexing lines with <see cref="Range"/> works correctly.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void IndexingRange()
        {
            Line line = new Line(IntVector2.zero, new IntVector2(4, 1));
            CollectionAssert.AreEqual(line, line[..]);
            CollectionAssert.AreEqual(new IntVector2[] { new IntVector2(1, 0), new IntVector2(2, 0), new IntVector2(3, 1), new IntVector2(4, 1) }, line[1..]);
            CollectionAssert.AreEqual(new IntVector2[] { new IntVector2(1, 0), new IntVector2(2, 0), new IntVector2(3, 1), new IntVector2(4, 1) }, line[1..5]);
            CollectionAssert.AreEqual(new IntVector2[] { new IntVector2(0, 0), new IntVector2(1, 0), new IntVector2(2, 0), new IntVector2(3, 1) }, line[..^1]);
            CollectionAssert.AreEqual(new IntVector2[] { new IntVector2(1, 0), new IntVector2(2, 0), new IntVector2(3, 1) }, line[1..4]);
        }

        [Test]
        [Category("Shapes")]
        public void PointIsToLeft()
        {
            foreach (Line line in testCases)
            {
                foreach (IntVector2 pixel in line)
                {
                    foreach (int x in line.boundingRect.xRange.Extend(-2, 2))
                    {
                        IntVector2 test = new IntVector2(x, pixel.y);
                        bool expected = test.x <= pixel.x || Enumerable.Contains(line, test);
                        Assert.True(expected == line.PointIsToLeft(test), $"Failed with {line} and {test}.");
                    }
                }

                foreach (int x in line.boundingRect.xRange.Extend(-2, 2))
                {
                    foreach (int y in (line.boundingRect.minY - IntRange.InclIncl(1, 2)).Concat(line.boundingRect.maxY + IntRange.InclIncl(1, 2)))
                    {
                        IntVector2 test = new IntVector2(x, y);
                        Assert.False(line.PointIsToLeft(test), $"Failed with {line} and {test}.");
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void PointIsToRight()
        {
            foreach (Line line in testCases)
            {
                foreach (IntVector2 pixel in line)
                {
                    foreach (int x in line.boundingRect.xRange.Extend(-2, 2))
                    {
                        IntVector2 test = new IntVector2(x, pixel.y);
                        bool expected = test.x >= pixel.x || Enumerable.Contains(line, test);
                        Assert.True(expected == line.PointIsToRight(test), $"Failed with {line} and {test}.");
                    }
                }

                foreach (int x in line.boundingRect.xRange.Extend(-2, 2))
                {
                    foreach (int y in (line.boundingRect.minY - IntRange.InclIncl(1, 2)).Concat(line.boundingRect.maxY + IntRange.InclIncl(1, 2)))
                    {
                        IntVector2 test = new IntVector2(x, y);
                        Assert.False(line.PointIsToRight(test), $"Failed with {line} and {test}.");
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void PointIsBelow()
        {
            foreach (Line line in testCases)
            {
                foreach (IntVector2 pixel in line)
                {
                    foreach (int y in line.boundingRect.xRange.Extend(-2, 2))
                    {
                        IntVector2 test = new IntVector2(pixel.x, y);
                        bool expected = test.y <= pixel.y || Enumerable.Contains(line, test);
                        Assert.True(expected == line.PointIsBelow(test), $"Failed with {line} and {test}.");
                    }
                }

                foreach (int y in line.boundingRect.yRange.Extend(-2, 2))
                {
                    foreach (int x in (line.boundingRect.minX - IntRange.InclIncl(1, 2)).Concat(line.boundingRect.maxX + IntRange.InclIncl(1, 2)))
                    {
                        IntVector2 test = new IntVector2(x, y);
                        Assert.False(line.PointIsBelow(test), $"Failed with {line} and {test}.");
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void PointIsAbove()
        {
            foreach (Line line in testCases)
            {
                foreach (IntVector2 pixel in line)
                {
                    foreach (int y in line.boundingRect.xRange.Extend(-2, 2))
                    {
                        IntVector2 test = new IntVector2(pixel.x, y);
                        bool expected = test.y >= pixel.y || Enumerable.Contains(line, test);
                        Assert.True(expected == line.PointIsAbove(test), $"Failed with {line} and {test}.");
                    }
                }

                foreach (int y in line.boundingRect.yRange.Extend(-2, 2))
                {
                    foreach (int x in (line.boundingRect.minX - IntRange.InclIncl(1, 2)).Concat(line.boundingRect.maxX + IntRange.InclIncl(1, 2)))
                    {
                        IntVector2 test = new IntVector2(x, y);
                        Assert.False(line.PointIsAbove(test), $"Failed with {line} and {test}.");
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void MinX()
        {
            foreach (Line line in testCases)
            {
                for (int y = line.boundingRect.bottomLeft.y; y <= line.boundingRect.topRight.y; y++)
                {
                    Assert.AreEqual(line.Where(p => p.y == y).Min(p => p.x), line.MinX(y), $"Failed with {line} and y = {y}.");
                }

                Assert.Throws<ArgumentOutOfRangeException>(() => line.MinX(line.boundingRect.bottomLeft.y - 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => line.MinX(line.boundingRect.topRight.y + 1));
            }
        }

        [Test]
        [Category("Shapes")]
        public void MaxX()
        {
            foreach (Line line in testCases)
            {
                for (int y = line.boundingRect.bottomLeft.y; y <= line.boundingRect.topRight.y; y++)
                {
                    Assert.AreEqual(line.Where(p => p.y == y).Max(p => p.x), line.MaxX(y), $"Failed with {line} and y = {y}.");
                }

                Assert.Throws<ArgumentOutOfRangeException>(() => line.MaxX(line.boundingRect.bottomLeft.y - 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => line.MaxX(line.boundingRect.topRight.y + 1));
            }
        }

        [Test]
        [Category("Shapes")]
        public void MinY()
        {
            foreach (Line line in testCases)
            {
                for (int x = line.boundingRect.bottomLeft.x; x <= line.boundingRect.topRight.x; x++)
                {
                    Assert.AreEqual(line.Where(p => p.x == x).Min(p => p.y), line.MinY(x), $"Failed with {line} and x = {x}.");
                }

                Assert.Throws<ArgumentOutOfRangeException>(() => line.MinY(line.boundingRect.bottomLeft.x - 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => line.MinY(line.boundingRect.topRight.x + 1));
            }
        }

        [Test]
        [Category("Shapes")]
        public void MaxY()
        {
            foreach (Line line in testCases)
            {
                for (int x = line.boundingRect.bottomLeft.x; x <= line.boundingRect.topRight.x; x++)
                {
                    Assert.AreEqual(line.Where(p => p.x == x).Max(p => p.y), line.MaxY(x), $"Failed with {line} and x = {x}.");
                }

                Assert.Throws<ArgumentOutOfRangeException>(() => line.MaxY(line.boundingRect.bottomLeft.x - 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => line.MaxY(line.boundingRect.topRight.x + 1));
            }
        }

        [Test]
        [Category("Shapes")]
        public void CountOnX()
        {
            foreach (Line line in testCases)
            {
                for (int x = line.boundingRect.bottomLeft.x - 2; x <= line.boundingRect.topRight.x + 2; x++)
                {
                    Assert.AreEqual(line.Count(p => p.x == x), line.CountOnX(x), $"Failed with {line} and x = {x}.");
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void CountOnY()
        {
            foreach (Line line in testCases)
            {
                for (int y = line.boundingRect.bottomLeft.y - 2; y <= line.boundingRect.topRight.y + 2; y++)
                {
                    Assert.AreEqual(line.Count(p => p.y == y), line.CountOnY(y), $"Failed with {line} and y = {y}.");
                }
            }
        }
    }
}
