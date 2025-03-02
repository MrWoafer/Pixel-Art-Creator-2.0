using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using PAC.DataStructures;
using PAC.Geometry;
using PAC.Geometry.Shapes;
using PAC.Tests.Geometry.Shapes.DefaultTests;
using PAC.Tests.Geometry.Shapes.RequiredTests;
using PAC.Tests.Geometry.Shapes.TestUtils;

namespace PAC.Tests.Geometry.Shapes
{
    /// <summary>
    /// Tests <see cref="Line"/>.
    /// </summary>
    public class Line_Tests : I1DShape_DefaultTests<Line>, I1DShape_RequiredTests
    {
        protected override IEnumerable<Line> testCases => Enumerable.Concat(exampleTestCases, randomTestCases);
        private IEnumerable<Line> exampleTestCases
        {
            get
            {
                foreach (IntVector2 end in new IntRect((-5, -5), (5, 5)))
                {
                    yield return new Line((0, 0), end);
                }
            }
        }
        private IEnumerable<Line> randomTestCases
        {
            get
            {
                Random random = new Random(0);
                IntRect testRegion = new IntRect((-20, -20), (20, 20));
                for (int i = 0; i < 1_000; i++)
                {
                    yield return new Line(testRegion.RandomPoint(random), testRegion.RandomPoint(random));
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public override void ShapeSinglePoint()
        {
            foreach (IntVector2 point in new IntRect((-5, -5), (5, 5)))
            {
                ShapeAssert.SameGeometry(new IntVector2[] { point }, new Line(point), $"Failed with {point}.");
                Assert.AreEqual(new Line(point, point), new Line(point), $"Failed with {point}.");
                Assert.True(new Line(point).isPoint, $"Failed wiht {point}.");
            }
        }

        /// <summary>
        /// Tests that <see cref="Line"/>s that can be drawn with constant-size blocks are drawn as such. E.g. a 12x4 <see cref="Line"/> can be drawn as 4 horizontal blocks of 3 pixels.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapePerfect()
        {
            IEnumerable<IntVector2> Expected(int blockSize, int numBlocks)
            {
                for (int i = 0; i < numBlocks * blockSize; i++)
                {
                    yield return (i, i / blockSize);
                }
            }

            foreach (IntVector2 start in new IntRect((-2, -2), (2, 2)))
            {
                for (int numBlocks = 1; numBlocks <= 10; numBlocks++)
                {
                    for (int blockSize = 1; blockSize <= 10; blockSize++)
                    {
                        foreach (QuadrantalAngle angle in new QuadrantalAngle[] { QuadrantalAngle._0, QuadrantalAngle._90, QuadrantalAngle._180, QuadrantalAngle.Minus90 })
                        {
                            foreach (FlipAxis axis in new FlipAxis[] { FlipAxis.None, FlipAxis._45Degrees })
                            {
                                Line line = new Line(start, start + new IntVector2(blockSize * numBlocks - 1, numBlocks - 1).Flip(axis).Rotate(angle));
                                IEnumerable<IntVector2> expected = Expected(blockSize, numBlocks).Select(p => start + p.Flip(axis).Rotate(angle));
                                ShapeAssert.SameGeometry(expected, line, $"Failed with {line}.");
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Tests <see cref="Line.isPerfect"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void isPerfect()
        {
            static bool Expected(Line line)
            {
                IntRect boundingRect = IntRect.BoundingRect(line);
                if (boundingRect.width >= boundingRect.height)
                {
                    return Enumerable.GroupBy(line, p => p.y).Select(block => block.Count()).Distinct().Count() == 1;
                }
                return Enumerable.GroupBy(line, p => p.x).Select(block => block.Count()).Distinct().Count() == 1;
            }

            foreach (Line line in testCases)
            {
                Assert.AreEqual(Expected(line), line.isPerfect, $"Failed with {line}.");
            }
        }

        /// <summary>
        /// Tests that an example <see cref="Line"/> has the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapeExample1()
        {
            Line line = new Line((0, 0), (2, 4));
            IntVector2[] expected =
            {
                (0, 0), (0, 1), (1, 2), (2, 3), (2, 4)
            };
            ShapeAssert.SameGeometry(expected, line);
        }

        /// <summary>
        /// Tests that an example <see cref="Line"/> has the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapeExample2()
        {
            Line line = new Line((0, 0), (4, 1));
            IntVector2[] expected =
            {
                (0, 0), (1, 0), (2, 0), (3, 1), (4, 1)
            };
            ShapeAssert.SameGeometry(expected, line);
        }

        /// <summary>
        /// Tests that an example <see cref="Line"/> has the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapeExample3()
        {
            Line line = new Line((0, 0), (5, 3));
            IntVector2[] expected =
            {
                (0, 0), (1, 0), (2, 1), (3, 2), (4, 3), (5, 3)
            };
            ShapeAssert.SameGeometry(expected, line);
        }

        /// <summary>
        /// Tests that an example <see cref="Line"/> has the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void ShapeExample4()
        {
            Line line = new Line((1, 2), (5, 5));
            IntVector2[] expected = {
                (1, 2), (2, 3), (3, 3), (4, 4), (5, 5)
            };
            ShapeAssert.SameGeometry(expected, line);
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
        /// Tests that the <see cref="Line"/> enumerator starts at <see cref="Line.start"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void EnumeratorStartsAtStart()
        {
            foreach (Line line in testCases)
            {
                Assert.AreEqual(Enumerable.First(line), line.start, $"Failed with {line}.");
            }
        }
        /// <summary>
        /// Tests that the <see cref="Line"/> enumerator ends at <see cref="Line.end"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void EnumeratorEndsAtEnd()
        {
            foreach (Line line in testCases)
            {
                Assert.AreEqual(Enumerable.Last(line), line.end, $"Failed with {line}.");
            }
        }

        /// <summary>
        /// Tests that each point in the <see cref="Line"/> enumerator is closer to <see cref="Line.end"/> than the previous point was.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void PointsGetCloserToEnd()
        {
            foreach (Line line in testCases)
            {
                int previousDistance = int.MaxValue;
                foreach (IntVector2 point in line)
                {
                    int distance = IntVector2.L1Distance(point, line.end);
                    Assert.True(distance < previousDistance, $"Failed with {line} and {point}.");
                    previousDistance = distance;
                }
            }
        }

        /// <summary>
        /// Tests that <see cref="Line"/>s have no three pixels that form a right angle, such as:
        /// <code>
        ///   #
        /// # #
        /// </code>
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void NoRightAngles()
        {
            foreach (Line line in testCases)
            {
                HashSet<IntVector2> points = Enumerable.ToHashSet(line);
                foreach (IntVector2 point in line)
                {
                    foreach (IntVector2 direction in IntVector2.upDownLeftRight)
                    {
                        Assert.False(
                            points.Contains(point + direction) && points.Contains(point + direction.Rotate(QuadrantalAngle._90)),
                            $"Failed with {line} and {point}."
                            );
                    }
                }
            }
        }

        /// <summary>
        /// Tests <see cref="Line.this[int]"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void Indexer_int()
        {
            foreach (Line line in testCases)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => { var x = line[-1]; }, $"Failed with {line}.");

                int index = 0;
                foreach (IntVector2 point in line)
                {
                    Assert.AreEqual(point, line[index], $"Failed with {line} at index {index}.");
                    index++;
                }

                Assert.Throws<ArgumentOutOfRangeException>(() => { var x = line[index]; }, $"Failed with {line}.");

                // Test hat syntax
                Assert.AreEqual(line.end, line[^1], $"Failed with {line}.");
            }
        }
        /// <summary>
        /// Tests <see cref="Line.this[Range]"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void Indexer_Range()
        {
            Line line = new Line((0, 0), (4, 1));
            IntVector2[] points = Enumerable.ToArray(line);

            CollectionAssert.AreEqual(line, line[..]);
            CollectionAssert.AreEqual(points[1..], line[1..]);
            CollectionAssert.AreEqual(points[1..5], line[1..5]);
            CollectionAssert.AreEqual(points[..^1], line[..^1]);
            CollectionAssert.AreEqual(points[2..4], line[2..4]);
        }

        /// <summary>
        /// Tests <see cref="Line.PointIsToLeft(IntVector2)"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void PointIsToLeft()
        {
            foreach (Line line in testCases)
            {
                IntRect boundingRect = IntRect.BoundingRect(line);

                foreach (IntVector2 point in line)
                {
                    foreach (int x in boundingRect.xRange.Extend(-2, 2))
                    {
                        IntVector2 testPoint = (x, point.y);
                        bool expected = testPoint.x <= point.x || Enumerable.Contains(line, testPoint);
                        Assert.True(expected == line.PointIsToLeft(testPoint), $"Failed with {line} and {testPoint}.");
                    }
                }

                foreach (int x in boundingRect.xRange.Extend(-2, 2))
                {
                    foreach (int y in (boundingRect.minY - IntRange.InclIncl(1, 2)).Concat(boundingRect.maxY + IntRange.InclIncl(1, 2)))
                    {
                        IntVector2 testPoint = (x, y);
                        Assert.False(line.PointIsToLeft(testPoint), $"Failed with {line} and {testPoint}.");
                    }
                }
            }
        }
        /// <summary>
        /// Tests <see cref="Line.PointIsToRight(IntVector2)"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void PointIsToRight()
        {
            foreach (Line line in testCases)
            {
                IntRect boundingRect = IntRect.BoundingRect(line);

                foreach (IntVector2 point in line)
                {
                    foreach (int x in boundingRect.xRange.Extend(-2, 2))
                    {
                        IntVector2 testPoint = (x, point.y);
                        bool expected = testPoint.x >= point.x || Enumerable.Contains(line, testPoint);
                        Assert.True(expected == line.PointIsToRight(testPoint), $"Failed with {line} and {testPoint}.");
                    }
                }

                foreach (int x in boundingRect.xRange.Extend(-2, 2))
                {
                    foreach (int y in (boundingRect.minY - IntRange.InclIncl(1, 2)).Concat(boundingRect.maxY + IntRange.InclIncl(1, 2)))
                    {
                        IntVector2 testPoint = (x, y);
                        Assert.False(line.PointIsToRight(testPoint), $"Failed with {line} and {testPoint}.");
                    }
                }
            }
        }
        /// <summary>
        /// Tests <see cref="Line.PointIsBelow(IntVector2)"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void PointIsBelow()
        {
            foreach (Line line in testCases)
            {
                IntRect boundingRect = IntRect.BoundingRect(line);

                foreach (IntVector2 point in line)
                {
                    foreach (int y in boundingRect.xRange.Extend(-2, 2))
                    {
                        IntVector2 testPoint = (point.x, y);
                        bool expected = testPoint.y <= point.y || Enumerable.Contains(line, testPoint);
                        Assert.True(expected == line.PointIsBelow(testPoint), $"Failed with {line} and {testPoint}.");
                    }
                }

                foreach (int y in boundingRect.yRange.Extend(-2, 2))
                {
                    foreach (int x in (boundingRect.minX - IntRange.InclIncl(1, 2)).Concat(boundingRect.maxX + IntRange.InclIncl(1, 2)))
                    {
                        IntVector2 testPoint = (x, y);
                        Assert.False(line.PointIsBelow(testPoint), $"Failed with {line} and {testPoint}.");
                    }
                }
            }
        }
        /// <summary>
        /// Tests <see cref="Line.PointIsAbove(IntVector2)"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void PointIsAbove()
        {
            foreach (Line line in testCases)
            {
                IntRect boundingRect = IntRect.BoundingRect(line);

                foreach (IntVector2 point in line)
                {
                    foreach (int y in boundingRect.xRange.Extend(-2, 2))
                    {
                        IntVector2 testPoint = (point.x, y);
                        bool expected = testPoint.y >= point.y || Enumerable.Contains(line, testPoint);
                        Assert.True(expected == line.PointIsAbove(testPoint), $"Failed with {line} and {testPoint}.");
                    }
                }

                foreach (int y in boundingRect.yRange.Extend(-2, 2))
                {
                    foreach (int x in (boundingRect.minX - IntRange.InclIncl(1, 2)).Concat(boundingRect.maxX + IntRange.InclIncl(1, 2)))
                    {
                        IntVector2 testPoint = (x, y);
                        Assert.False(line.PointIsAbove(testPoint), $"Failed with {line} and {testPoint}.");
                    }
                }
            }
        }

        /// <summary>
        /// Tests <see cref="Line.MinX(int)"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void MinX()
        {
            foreach (Line line in testCases)
            {
                IntRect boundingRect = IntRect.BoundingRect(line);

                for (int y = boundingRect.minY; y <= boundingRect.maxY; y++)
                {
                    Assert.AreEqual(line.Where(p => p.y == y).Min(p => p.x), line.MinX(y), $"Failed with {line} and y = {y}.");
                }

                Assert.Throws<ArgumentOutOfRangeException>(() => line.MinX(boundingRect.minY - 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => line.MinX(boundingRect.maxY + 1));
            }
        }
        /// <summary>
        /// Tests <see cref="Line.MaxX(int)"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void MaxX()
        {
            foreach (Line line in testCases)
            {
                IntRect boundingRect = IntRect.BoundingRect(line);

                for (int y = boundingRect.minY; y <= boundingRect.maxY; y++)
                {
                    Assert.AreEqual(line.Where(p => p.y == y).Max(p => p.x), line.MaxX(y), $"Failed with {line} and y = {y}.");
                }

                Assert.Throws<ArgumentOutOfRangeException>(() => line.MaxX(boundingRect.minY - 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => line.MaxX(boundingRect.maxY + 1));
            }
        }
        /// <summary>
        /// Tests <see cref="Line.MinY(int)"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void MinY()
        {
            foreach (Line line in testCases)
            {
                IntRect boundingRect = IntRect.BoundingRect(line);

                for (int x = boundingRect.minX; x <= boundingRect.maxX; x++)
                {
                    Assert.AreEqual(line.Where(p => p.x == x).Min(p => p.y), line.MinY(x), $"Failed with {line} and x = {x}.");
                }

                Assert.Throws<ArgumentOutOfRangeException>(() => line.MinY(boundingRect.minX - 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => line.MinY(boundingRect.maxX + 1));
            }
        }
        /// <summary>
        /// Tests <see cref="Line.MaxY(int)"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void MaxY()
        {
            foreach (Line line in testCases)
            {
                IntRect boundingRect = IntRect.BoundingRect(line);

                for (int x = boundingRect.minX; x <= boundingRect.maxX; x++)
                {
                    Assert.AreEqual(line.Where(p => p.x == x).Max(p => p.y), line.MaxY(x), $"Failed with {line} and x = {x}.");
                }

                Assert.Throws<ArgumentOutOfRangeException>(() => line.MaxY(boundingRect.minX - 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => line.MaxY(boundingRect.maxX + 1));
            }
        }

        /// <summary>
        /// Tests <see cref="Line.CountOnX(int)"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void CountOnX()
        {
            foreach (Line line in testCases)
            {
                IntRect boundingRect = IntRect.BoundingRect(line);
                for (int x = boundingRect.minX - 2; x <= boundingRect.maxX + 2; x++)
                {
                    Assert.AreEqual(Enumerable.Count(line, p => p.x == x), line.CountOnX(x), $"Failed with {line} and x = {x}.");
                }
            }
        }
        /// <summary>
        /// Tests <see cref="Line.CountOnY(int)"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void CountOnY()
        {
            foreach (Line line in testCases)
            {
                IntRect boundingRect = IntRect.BoundingRect(line);
                for (int y = boundingRect.minY - 2; y <= boundingRect.maxY + 2; y++)
                {
                    Assert.AreEqual(Enumerable.Count(line, p => p.y == y), line.CountOnY(y), $"Failed with {line} and y = {y}.");
                }
            }
        }

        /// <summary>
        /// Tests that <see cref="Line"/>s have 180-degree rotational symmetry, except potentially at the midpoint.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void RotationalSymmetry_180Degrees()
        {
            foreach (Line line in testCases)
            {
                int Count = Enumerable.Count(line);
                if (Count % 2 == 0)
                {
                    ShapeAssert.RotationalSymmetry(line, QuadrantalAngle._180);
                }
                else
                {
                    List<IntVector2> points = Enumerable.ToList(line);
                    points.Remove(line[Count / 2]); // Ignore midpoint
                    ShapeAssert.RotationalSymmetry(points, QuadrantalAngle._180);
                }
            }
        }

        /// <summary>
        /// Tests <see cref="Line.reverse"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void reverse()
        {
            foreach (Line line in testCases)
            {
                Line reverse = line.reverse;

                Assert.AreEqual(line.end, reverse.start, $"Failed with {line}.");
                Assert.AreEqual(line.start, reverse.end, $"Failed with {line}.");

                // Check that the reverse looks like the line rotated 180 degrees
                ShapeAssert.SameGeometry(line.Select(p => line.end - (p - line.start)), reverse, $"Failed with {line}.");
            }
        }
    }
}
