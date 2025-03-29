using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using PAC.Extensions;
using PAC.Geometry;
using PAC.Geometry.Shapes;
using PAC.Tests.Geometry.Shapes.DefaultTests;
using PAC.Tests.Geometry.Shapes.RequiredTests;
using PAC.Tests.Geometry.Shapes.TestUtils;

namespace PAC.Tests.Geometry.Shapes
{
    /// <summary>
    /// Tests for <see cref="Path"/>.
    /// </summary>
    public class Path_Tests : I1DShape_DefaultTests<Path>, I1DShape_RequiredTests
    {
        protected override IEnumerable<Path> testCases => Enumerable.Concat(exampleTestCases, randomTestCases);
        private IEnumerable<Path> exampleTestCases
        {
            get
            {
                for (int x = 0; x <= 10; x++)
                {
                    yield return new Path((0, 0), (x, 0));
                }

                yield return new Path((0, 0), (1, 0), (1, 1));
                yield return new Path((0, 0), (1, 0), (0, 0));
                yield return new Path((0, 0), (3, 3), (-5, -5));
            }
        }
        private IEnumerable<Path> randomTestCases => RandomTestCases(1_000);
        private IEnumerable<Path> RandomTestCases(int numOfTestCases) => Enumerable.Concat(RandomTestCases(numOfTestCases, false), RandomTestCases(numOfTestCases, true));
        private IEnumerable<Path> RandomTestCases(int numOfTestCases, bool isLoop)
        {
            Random random = new Random(0);
            for (int length = 1; length <= 3; length++)
            {
                for (int iteration = 0; iteration < numOfTestCases; iteration++)
                {
                    yield return RandomPath(random, length, isLoop);
                }
            }
        }
        /// <summary>
        /// Generates a random <see cref="Path"/> with made up of <paramref name="length"/> many <see cref="Line"/>s.
        /// </summary>
        /// <remarks>
        /// If <paramref name="isLoop"/> is <see langword="true"/> it will add an extra <see cref="Line"/> from the end point to the start point to make sure the <see cref="Path"/> is a loop.
        /// If <paramref name="isLoop"/> is <see langword="false"/> the <see cref="Path"/> is guaranteed to not be a loop.
        /// </remarks>
        private Path RandomPath(Random random, int length, bool isLoop)
        {
            List<Line> lines = new List<Line>
            {
                new Line(new IntRect((-5, -5), (5, 5)).RandomPoint(random), new IntRect((-5, -5), (5, 5)).RandomPoint(random))
            };

            for (int i = 0; i < length - 1; i++)
            {
                IntVector2 start = lines[^1].end + new IntRect((-1, -1), (1, 1)).RandomPoint(random);
                lines.Add(new Line(start, start + new IntRect((-5, -5), (5, 5)).RandomPoint(random)));
            }

            if (isLoop)
            {
                lines.Add(new Line(
                    lines[^1].end + new IntRect((-1, -1), (1, 1)).RandomPoint(random),
                    lines[0].start + new IntRect((-1, -1), (1, 1)).RandomPoint(random)
                    ));

                return new Path(lines);
            }
            else
            {
                Path path = new Path(lines);
                while (path.isLoop)
                {
                    return RandomPath(random, length, false);
                }
                return path;
            }
        }

        /// <summary>
        /// Tests <see cref="Path(IntVector2[])"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void Constructor_Points()
        {
            Assert.Throws<ArgumentException>(() => new Path(new IntVector2[0]));

            Assert.AreEqual(
                new Path(new Line((0, 0), (0, 0))),
                new Path((0, 0))
                );
            Assert.AreEqual(
                new Path(new Line((0, 0), (1, 1))),
                new Path((0, 0), (1, 1))
                );
            Assert.AreEqual(
                new Path(new Line((0, 0), (1, 1)), new Line((1, 1), (2, 4))),
                new Path((0, 0), (1, 1), (2, 4))
                );
        }
        /// <summary>
        /// Tests <see cref="Path(Line[])"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void Constructor_Lines()
        {
            Assert.Throws<ArgumentException>(() => new Path(new Line[0]));
            Assert.Throws<ArgumentException>(() => new Path(new Line((0, 0), (1, 1)), new Line((2, 3), (5, 5))));
            Assert.Throws<ArgumentException>(() => new Path(new Line((0, 0), (1, 1)), new Line((2, 3), (1, 1))));

            Assert.DoesNotThrow(() => new Path(new Line((0, 0), (1, 1))));
            Assert.DoesNotThrow(() => new Path(new Line((0, 0), (1, 1)), new Line((1, 1), (5, 5))));
            Assert.DoesNotThrow(() => new Path(new Line((0, 0), (1, 1)), new Line((1, 2), (5, 5))));
            Assert.DoesNotThrow(() => new Path(new Line((0, 0), (1, 1)), new Line((2, 2), (5, 5))));
        }

        [Test]
        [Category("Shapes")]
        public override void ShapeSinglePoint()
        {
            foreach (IntVector2 point in new IntRect((-5, -5), (5, 5)))
            {
                for (int repetitions = 1; repetitions <= 5; repetitions++)
                {
                    Path path = new Path(Enumerable.Repeat(point, repetitions));
                    ShapeAssert.SameGeometry(new IntVector2[] { point }, path, $"Failed with {path}.");
                }
            }
        }

        /// <summary>
        /// Tests <see cref="Path.start"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void start()
        {
            foreach (Path path in testCases)
            {
                Assert.AreEqual(Enumerable.First(path), path.start, $"Failed with {path}.");
            }
        }
        /// <summary>
        /// Tests <see cref="Path.end"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void end()
        {
            foreach (Path path in testCases)
            {
                if (path.end == path.start && Enumerable.Count(path) > 1)
                {
                    Assert.AreNotEqual(Enumerable.Last(path), path.end, $"Failed with {path}.");
                }
                else
                {
                    Assert.AreEqual(Enumerable.Last(path), path.end, $"Failed with {path}.");
                }
            }
        }

        /// <summary>
        /// Tests that some example <see cref="Path"/>s have the correct enumerator.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void EnumeratorExamples()
        {
            (IEnumerable<IntVector2> expected, Path path)[] testCases =
            {
                (new IntVector2[] { (0, 0) },
                    new Path((0, 0))),
                (new Line((0, 0), (2, 3)),
                    new Path((0, 0), (2, 3))),
                (Enumerable.Concat(new Line((0, 0), (2, 3)), new Line((2, 3), (4, 4))[1..]),
                    new Path((0, 0), (2, 3), (4, 4))),
                // Loop
                (IEnumerableExtensions.Concat(new Line((0, 0), (2, 3)), new Line((2, 3), (4, 4))[1..], new Line((4, 4), (0, 0))[1..^1]),
                    new Path((0, 0), (2, 3), (4, 4), (0, 0))),
                // Crossing previous values
                (IEnumerableExtensions.Concat(new Line((0, 0), (2, 3)), new Line((2, 3), (4, 4))[1..], new Line((4, 4), (2, 4))[1..], new Line((2, 4), (2, 0))[1..]),
                    new Path((0, 0), (2, 3), (4, 4), (2, 4), (2, 0))),
                (new IntVector2[] { (0, 0), (1, 0), (1, 1) },
                    new Path((0, 0), (1, 0), (1, 1), (0, 0))),
                (new IntVector2[] { (4, 2), (5, 1) },
                    new Path((4, 2), (5, 1))),
                (new IntVector2[] { (0, 0) },
                    new Path((0, 0), (0, 0), (0, 0), (0, 0))),
                (Enumerable.Concat(new Line((0, 0), (2, 5)), new Line((2, 5), (3, 2))[1..]),
                    new Path((0, 0), (2, 5), (3, 2), (3, 2))),
                (IEnumerableExtensions.Concat(new Line((0, 0),(2, 5)), new Line((2, 5),(3, 2))[1 ..], new Line((3, 2), (0, 0))[1..^1]),
                    new Path((0, 0), (2, 5), (3, 2), (3, 2), (0, 0)))
            };

            foreach ((IEnumerable<IntVector2> expected, Path path) in testCases)
            {
                CollectionAssert.AreEqual(expected, path, $"Failed with {path}.");
            }
        }

        /// <summary>
        /// Tests <see cref="Path.isLoop"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void isLoop()
        {
            Assert.AreEqual(false, new Path((0, 0), (2, 1)).isLoop);
            Assert.AreEqual(false, new Path((0, 0), (1, 1), (2, 2), (5, 5)).isLoop);
            Assert.AreEqual(false, new Path((0, 0), (1, 1), (2, 2), (2, 1)).isLoop);

            Assert.AreEqual(true, new Path((0, 0)).isLoop);
            Assert.AreEqual(true, new Path((0, 0), (1, 1)).isLoop);
            Assert.AreEqual(true, new Path((0, 0), (1, 1), (2, 2), (0, 0)).isLoop);
            Assert.AreEqual(true, new Path((0, 0), (1, 1), (2, 2), (0, 1)).isLoop);
            Assert.AreEqual(true, new Path((0, 0), (1, 1), (2, 2), (1, 1)).isLoop);
        }

        [Test]
        [Category("Shapes")]
        public override void Count()
        {
            // Pre-defined example-based tests

            (int, Path)[] testCases =
            {
                (1, new Path((0, 0))),
                (4, new Path((0, 0), (2, 3))),
                (6, new Path((0, 0), (2, 3), (4, 4))),
                // Loop
                (9, new Path((0, 0), (2, 3), (4, 4), (0, 0))),
                // Crossing previous values
                (12, new Path((0, 0), (2, 3), (4, 4), (2, 4), (2, 0))),
                (3, new Path((0, 0), (1, 0), (1, 1), (0, 0))),
                (3, new Path((0, 0), (1, 1), (1, 0), (0, 0))),
                (12, new Path(new Line((-1, 5), (0, 4)), new Line((1, 3), (-4, 7)), new Line((-4, 6), (-1, 5)), new Line((-1, 5), (-1, 5))))
            };
            
            foreach ((int expected, Path path) in testCases)
            {
                Assert.AreEqual(expected, path.Count, $"Failed with {path}.");
                Assert.AreEqual(Enumerable.Count(path), path.Count, $"Failed with {path}.");
            }

            // Random tests

            foreach (Path path in RandomTestCases(2_000))
            {
                Assert.AreEqual(Enumerable.Count(path), path.Count, $"Failed with {path}.");
            }
        }

        /// <summary>
        /// Tests that points never appear twice in a row in the <see cref="Path"/> enumerator.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void NoConsecutiveRepeats()
        {
            foreach (Path path in testCases)
            {
                IntVector2[] points = Enumerable.ToArray(path);

                if (points.Length == 1)
                {
                    continue;
                }

                for (int i = 0; i < points.Length; i++)
                {
                    Assert.AreNotEqual(points[(i + 1) % points.Length], points[i], $"Failed with {path} at index {i}.");
                }
            }
        }

        /// <summary>
        /// Tests <see cref="Path.MinX(int)"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void MinX()
        {
            foreach (Path path in testCases)
            {
                IntRect boundingRect = IntRect.BoundingRect(path);

                for (int y = boundingRect.minY; y <= boundingRect.maxY; y++)
                {
                    Assert.AreEqual(path.Where(p => p.y == y).Min(p => p.x), path.MinX(y), $"Failed with {path} and y = {y}.");
                }

                Assert.Throws<ArgumentOutOfRangeException>(() => path.MinX(boundingRect.minY - 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => path.MinX(boundingRect.maxY + 1));
            }
        }
        /// <summary>
        /// Tests <see cref="Path.MaxX(int)"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void MaxX()
        {
            foreach (Path path in testCases)
            {
                IntRect boundingRect = IntRect.BoundingRect(path);

                for (int y = boundingRect.minY; y <= boundingRect.maxY; y++)
                {
                    Assert.AreEqual(path.Where(p => p.y == y).Max(p => p.x), path.MaxX(y), $"Failed with {path} and y = {y}.");
                }

                Assert.Throws<ArgumentOutOfRangeException>(() => path.MaxX(boundingRect.minY - 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => path.MaxX(boundingRect.maxY + 1));
            }
        }
        /// <summary>
        /// Tests <see cref="Path.MinY(int)"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void MinY()
        {
            foreach (Path path in testCases)
            {
                IntRect boundingRect = IntRect.BoundingRect(path);

                for (int x = boundingRect.minX; x <= boundingRect.maxX; x++)
                {
                    Assert.AreEqual(path.Where(p => p.x == x).Min(p => p.y), path.MinY(x), $"Failed with {path} and x = {x}.");
                }

                Assert.Throws<ArgumentOutOfRangeException>(() => path.MinY(boundingRect.minX - 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => path.MinY(boundingRect.maxX + 1));
            }
        }
        /// <summary>
        /// Tests <see cref="Path.CountOnX(int)"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void MaxY()
        {
            foreach (Path path in testCases)
            {
                IntRect boundingRect = IntRect.BoundingRect(path);

                for (int x = boundingRect.minX; x <= boundingRect.maxX; x++)
                {
                    Assert.AreEqual(path.Where(p => p.x == x).Max(p => p.y), path.MaxY(x), $"Failed with {path} and x = {x}.");
                }

                Assert.Throws<ArgumentOutOfRangeException>(() => path.MaxY(boundingRect.minX - 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => path.MaxY(boundingRect.maxX + 1));
            }
        }

        /// <summary>
        /// Tests <see cref="Path.CountOnX(int)"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void CountOnX()
        {
            foreach (Path path in testCases)
            {
                IntRect boundingRect = IntRect.BoundingRect(path);
                for (int x = boundingRect.minX - 2; x <= boundingRect.maxX + 2; x++)
                {
                    Assert.AreEqual(Enumerable.Count(path, p => p.x == x), path.CountOnX(x), $"Failed with {path} and x = {x}.");
                }
            }
        }
        /// <summary>
        /// Tests <see cref="Path.CountOnY(int)"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void CountOnY()
        {
            foreach (Path path in testCases)
            {
                IntRect boundingRect = IntRect.BoundingRect(path);
                for (int y = boundingRect.minY - 2; y <= boundingRect.maxY + 2; y++)
                {
                    Assert.AreEqual(Enumerable.Count(path, p => p.y == y), path.CountOnY(y), $"Failed with {path} and y = {y}.");
                }
            }
        }

        /// <summary>
        /// Tests <see cref="Path.selfIntersects"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void selfIntersects()
        {
            foreach (Path path in testCases)
            {
                Assert.AreEqual(
                    Enumerable.Distinct(path).Count() != Enumerable.Count(path),
                    path.selfIntersects,
                    $"Failed with {path}."
                    );
            }
        }

        /// <summary>
        /// Tests <see cref="Path.Equals(Path)"/>.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void Equals_Path()
        {
            static bool Expected(Path a, Path b)
            {
                if (a.lines.Count != b.lines.Count)
                {
                    return false;
                }

                for (int i = 0; i < a.lines.Count; i++)
                {
                    if (a.lines[i] != b.lines[i])
                    {
                        return false;
                    }
                }
                return true;
            }

            foreach (Path a in RandomTestCases(100))
            {
                Assert.True(a.Equals(a), $"Failed with {a}.");
                Assert.True(a.Equals(a.DeepCopy()), $"Failed with {a}.");

                foreach (Path b in RandomTestCases(100))
                {
                    Assert.True(Expected(a, b) == a.Equals(b), $"Failed with {a} and {b}.");
                }
            }
        }
    }
}
