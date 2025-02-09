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
    public class Path_Tests : I1DShape_DefaultTests<Path>, I1DShape_RequiredTests
    {
        protected override IEnumerable<Path> testCases => RandomTestCases(1_000);
        private IEnumerable<Path> RandomTestCases(int numOfTestCases) => RandomTestCases(numOfTestCases, false).Concat(RandomTestCases(numOfTestCases, true));
        private IEnumerable<Path> RandomTestCases(int numOfTestCases, bool isLoop)
        {
            for (int length = 1; length <= 3; length++)
            {
                for (int iteration = 0; iteration < numOfTestCases; iteration++)
                {
                    yield return RandomPath(length, isLoop);
                }
            }
        }
        private Path RandomPath(int length, bool isLoop)
        {
            List<Line> lines = new List<Line>
            {
                new Line(new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)).RandomPoint(), new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)).RandomPoint())
            };

            for (int i = 0; i < length - 1; i++)
            {
                IntVector2 start = lines[^1].end + new IntRect(new IntVector2(-1, -1), new IntVector2(1, 1)).RandomPoint();
                lines.Add(new Line(start, start + new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)).RandomPoint()));
            }

            if (isLoop)
            {
                lines.Add(new Line(
                    lines[^1].end + new IntRect(new IntVector2(-1, -1), new IntVector2(1, 1)).RandomPoint(),
                    lines[0].start + new IntRect(new IntVector2(-1, -1), new IntVector2(1, 1)).RandomPoint()
                    ));

                return new Path(lines);
            }
            else
            {
                Path path = new Path(lines);
                while (path.isLoop)
                {
                    return RandomPath(length, false);
                }
                return path;
            }
        }

        [Test]
        [Category("Shapes")]
        public void Constructor()
        {
            Assert.Throws<ArgumentException>(() => new Path(new Line[0]));
            Assert.Throws<ArgumentException>(() => new Path(new Line(IntVector2.zero, new IntVector2(1, 1)), new Line(new IntVector2(2, 3), new IntVector2(5, 5))));
            Assert.Throws<ArgumentException>(() => new Path(new Line(IntVector2.zero, new IntVector2(1, 1)), new Line(new IntVector2(2, 3), new IntVector2(1, 1))));

            Assert.Throws<ArgumentException>(() => new Path(new IntVector2[0]));
            Assert.DoesNotThrow(() => new Path(new Line(IntVector2.zero, new IntVector2(1, 1))));
            Assert.DoesNotThrow(() => new Path(new Line(IntVector2.zero, new IntVector2(1, 1)), new Line(new IntVector2(1, 1), new IntVector2(5, 5))));
            Assert.DoesNotThrow(() => new Path(new Line(IntVector2.zero, new IntVector2(1, 1)), new Line(new IntVector2(1, 2), new IntVector2(5, 5))));
            Assert.DoesNotThrow(() => new Path(new Line(IntVector2.zero, new IntVector2(1, 1)), new Line(new IntVector2(2, 2), new IntVector2(5, 5))));

            Assert.AreEqual(new Path(new Line(IntVector2.zero, IntVector2.zero)), new Path(IntVector2.zero));
            Assert.AreEqual(new Path(new Line(IntVector2.zero, IntVector2.one)), new Path(IntVector2.zero, IntVector2.one));
            Assert.AreEqual(new Path(new Line(IntVector2.zero, IntVector2.one), new Line(IntVector2.one, new IntVector2(2, 4))),
                new Path(IntVector2.zero, IntVector2.one, new IntVector2(2, 4)));
        }

        [Test]
        [Category("Shapes")]
        public override void ShapeSinglePoint()
        {
            foreach (IntVector2 pixel in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
            {
                for (int repetitions = 1; repetitions <= 5; repetitions++)
                {
                    Path path = new Path(Enumerable.Repeat(pixel, repetitions));
                    CollectionAssert.AreEqual(new IntVector2[] { pixel }, path, $"Failed with {path}.");
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void ShapeExamples()
        {
            (IEnumerable<IntVector2> expected, Path path)[] testCases =
            {
                (new IntVector2[] { IntVector2.zero },
                    new Path(IntVector2.zero)),
                (new Line(IntVector2.zero, new IntVector2(2, 3)),
                    new Path(IntVector2.zero, new IntVector2(2, 3))),
                (new Line(IntVector2.zero, new IntVector2(2, 3)).Concat(new Line(new IntVector2(2, 3), new IntVector2(4, 4))[1..]),
                    new Path(IntVector2.zero, new IntVector2(2, 3), new IntVector2(4, 4))),
                // Loop
                (new Line(IntVector2.zero, new IntVector2(2, 3)).Concat(new Line(new IntVector2(2, 3), new IntVector2(4, 4))[1..])
                    .Concat(new Line(new IntVector2(4, 4), IntVector2.zero)[1..^1]),
                    new Path(IntVector2.zero, new IntVector2(2, 3), new IntVector2(4, 4), IntVector2.zero)),
                // Crossing previous values
                (new Line(IntVector2.zero, new IntVector2(2, 3)).Concat(new Line(new IntVector2(2, 3), new IntVector2(4, 4))[1..])
                    .Concat(new Line(new IntVector2(4, 4), new IntVector2(2, 4))[1..]).Concat(new Line(new IntVector2(2, 4), new IntVector2(2, 0))[1..]),
                    new Path(IntVector2.zero, new IntVector2(2, 3), new IntVector2(4, 4), new IntVector2(2, 4), new IntVector2(2, 0))),
                (new IntVector2[] { IntVector2.zero, IntVector2.right, IntVector2.upRight },
                    new Path(IntVector2.zero, IntVector2.right, IntVector2.upRight, IntVector2.zero)),
                (new IntVector2[] { new IntVector2(4, 2), new IntVector2(5, 1) },
                    new Path(new IntVector2(4, 2), new IntVector2(5, 1))),
                (new IntVector2[] { IntVector2.zero },
                    new Path(IntVector2.zero, IntVector2.zero, IntVector2.zero, IntVector2.zero)),
                (new Line(IntVector2.zero, new IntVector2(2, 5)).Concat(new Line(new IntVector2(2, 5), new IntVector2(3, 2))[1..]),
                    new Path(IntVector2.zero, new IntVector2(2, 5), new IntVector2(3, 2), new IntVector2(3, 2))),
                (new Line(IntVector2.zero, new IntVector2(2, 5)).Concat(new Line(new IntVector2(2, 5), new IntVector2(3, 2))[1..])
                    .Concat(new Line(new IntVector2(3, 2), IntVector2.zero)[1..^1]),
                    new Path(IntVector2.zero, new IntVector2(2, 5), new IntVector2(3, 2), new IntVector2(3, 2), IntVector2.zero))
            };

            foreach ((IEnumerable<IntVector2> expected, Path path) in testCases)
            {
                CollectionAssert.AreEqual(expected, path, $"Failed with {path}.");
            }
        }

        [Test]
        [Category("Shapes")]
        public void IsLoop()
        {
            Assert.AreEqual(false, new Path(IntVector2.zero, new IntVector2(2, 1)).isLoop);
            Assert.AreEqual(false, new Path(IntVector2.zero, new IntVector2(1, 1), new IntVector2(2, 2), new IntVector2(5, 5)).isLoop);
            Assert.AreEqual(false, new Path(IntVector2.zero, new IntVector2(1, 1), new IntVector2(2, 2), new IntVector2(2, 1)).isLoop);

            Assert.AreEqual(true, new Path(IntVector2.zero).isLoop);
            Assert.AreEqual(true, new Path(IntVector2.zero, new IntVector2(1, 1)).isLoop);
            Assert.AreEqual(true, new Path(IntVector2.zero, new IntVector2(1, 1), new IntVector2(2, 2), IntVector2.zero).isLoop);
            Assert.AreEqual(true, new Path(IntVector2.zero, new IntVector2(1, 1), new IntVector2(2, 2), new IntVector2(0, 1)).isLoop);
            Assert.AreEqual(true, new Path(IntVector2.zero, new IntVector2(1, 1), new IntVector2(2, 2), new IntVector2(1, 1)).isLoop);
        }

        [Test]
        [Category("Shapes")]
        public override void BoundingRect()
        {
            foreach (Path path in RandomTestCases(2_000))
            {
                Assert.AreEqual(IntRect.BoundingRect(path), path.boundingRect, $"Failed with {path}.");
            }
        }

        [Test]
        [Category("Shapes")]
        public override void Count()
        {
            // Pre-defined example-based tests

            (int, Path)[] testCases =
            {
                (1, new Path(IntVector2.zero)),
                (4, new Path(IntVector2.zero, new IntVector2(2, 3))),
                (6, new Path(IntVector2.zero, new IntVector2(2, 3), new IntVector2(4, 4))),
                // Loop
                (9, new Path(IntVector2.zero, new IntVector2(2, 3), new IntVector2(4, 4), IntVector2.zero)),
                // Crossing previous values
                (12, new Path(IntVector2.zero, new IntVector2(2, 3), new IntVector2(4, 4), new IntVector2(2, 4), new IntVector2(2, 0))),
                (3, new Path(IntVector2.zero, IntVector2.right, IntVector2.upRight, IntVector2.zero)),
                (3, new Path(IntVector2.zero, IntVector2.upRight, IntVector2.right, IntVector2.zero)),
                (12, new Path(
                    new Line(new IntVector2(-1, 5), new IntVector2(0, 4)), new Line(new IntVector2(1, 3), new IntVector2(-4, 7)),
                    new Line(new IntVector2(-4, 6), new IntVector2(-1, 5)), new Line(new IntVector2(-1, 5), new IntVector2(-1, 5))
                ))
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
        /// Tests that pixels never appear twice in a row in a path.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void NoConsecutiveRepeats()
        {
            for (int length = 1; length <= 3; length++)
            {
                foreach (bool isLoop in new bool[] { false, true })
                {
                    for (int iteration = 0; iteration < 1_000; iteration++)
                    {
                        Path path = RandomPath(length, isLoop);
                        IntVector2[] pixels = path.ToArray();

                        if (pixels.Length == 1)
                        {
                            continue;
                        }

                        for (int i = 0; i < pixels.Length; i++)
                        {
                            Assert.AreNotEqual(pixels[(i + 1) % pixels.Length], pixels[i], $"Failed with {path} at index {i}.");
                        }
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void MinX()
        {
            foreach (Path path in testCases)
            {
                for (int y = path.boundingRect.bottomLeft.y; y <= path.boundingRect.topRight.y; y++)
                {
                    Assert.AreEqual(path.Where(p => p.y == y).Min(p => p.x), path.MinX(y), $"Failed with {path} and y = {y}.");
                }

                Assert.Throws<ArgumentOutOfRangeException>(() => path.MinX(path.boundingRect.bottomLeft.y - 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => path.MinX(path.boundingRect.topRight.y + 1));
            }
        }

        [Test]
        [Category("Shapes")]
        public void MaxX()
        {
            foreach (Path path in testCases)
            {
                for (int y = path.boundingRect.bottomLeft.y; y <= path.boundingRect.topRight.y; y++)
                {
                    Assert.AreEqual(path.Where(p => p.y == y).Max(p => p.x), path.MaxX(y), $"Failed with {path} and y = {y}.");
                }

                Assert.Throws<ArgumentOutOfRangeException>(() => path.MaxX(path.boundingRect.bottomLeft.y - 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => path.MaxX(path.boundingRect.topRight.y + 1));
            }
        }

        [Test]
        [Category("Shapes")]
        public void MinY()
        {
            foreach (Path path in testCases)
            {
                for (int x = path.boundingRect.bottomLeft.x; x <= path.boundingRect.topRight.x; x++)
                {
                    Assert.AreEqual(path.Where(p => p.x == x).Min(p => p.y), path.MinY(x), $"Failed with {path} and x = {x}.");
                }

                Assert.Throws<ArgumentOutOfRangeException>(() => path.MinY(path.boundingRect.bottomLeft.x - 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => path.MinY(path.boundingRect.topRight.x + 1));
            }
        }

        [Test]
        [Category("Shapes")]
        public void MaxY()
        {
            foreach (Path path in testCases)
            {
                for (int x = path.boundingRect.bottomLeft.x; x <= path.boundingRect.topRight.x; x++)
                {
                    Assert.AreEqual(path.Where(p => p.x == x).Max(p => p.y), path.MaxY(x), $"Failed with {path} and x = {x}.");
                }

                Assert.Throws<ArgumentOutOfRangeException>(() => path.MaxY(path.boundingRect.bottomLeft.x - 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => path.MaxY(path.boundingRect.topRight.x + 1));
            }
        }

        [Test]
        [Category("Shapes")]
        public void CountOnX()
        {
            foreach (Path path in testCases)
            {
                for (int x = path.boundingRect.bottomLeft.x - 2; x <= path.boundingRect.topRight.x + 2; x++)
                {
                    Assert.AreEqual(path.Count(p => p.x == x), path.CountOnX(x), $"Failed with {path} and x = {x}.");
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void CountOnY()
        {
            foreach (Path path in testCases)
            {
                for (int y = path.boundingRect.bottomLeft.y - 2; y <= path.boundingRect.topRight.y + 2; y++)
                {
                    Assert.AreEqual(path.Count(p => p.y == y), path.CountOnY(y), $"Failed with {path} and y = {y}.");
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void SelfIntersects()
        {
            foreach (Path path in testCases)
            {
                if (!path.selfIntersects)
                {
                    ShapeAssert.NoRepeats(path);
                }
                else
                {
                    bool hasDuplicate = false;
                    HashSet<IntVector2> visited = new HashSet<IntVector2>();
                    foreach (IntVector2 pixel in path)
                    {
                        if (visited.Contains(pixel))
                        {
                            hasDuplicate = true;
                            break;
                        }
                        visited.Add(pixel);
                    }
                    Assert.True(hasDuplicate, $"Failed with {path}.");
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void IsSimplePolygon()
        {
            (bool isSimplePolygon, Path path)[] testCases =
            {
                (true, new Path(IntVector2.zero, IntVector2.up, IntVector2.upRight, IntVector2.right, IntVector2.zero)),
                (true, new Path(IntVector2.downLeft, IntVector2.upLeft, IntVector2.upRight, IntVector2.downRight, IntVector2.downLeft)),
                (true, new Path(IntVector2.zero, IntVector2.upRight, IntVector2.right, IntVector2.zero)),
                (true, new Path(IntVector2.zero, IntVector2.zero)),
                (true, new Path(IntVector2.zero, IntVector2.zero, IntVector2.zero)),
                (true, new Path(IntVector2.zero, IntVector2.right, IntVector2.zero)),
                (true, new Path(IntVector2.zero, new IntVector2(5, 0), IntVector2.zero)),
                (true, new Path(IntVector2.zero, new IntVector2(5, 0), new IntVector2(0, 7), new IntVector2(0, -4), IntVector2.zero)),
                (true, new Path(IntVector2.zero, new IntVector2(5, 0), new IntVector2(0, 7), new IntVector2(0, -4), new IntVector2(4, -4), IntVector2.zero)),
                (true, new Path(IntVector2.zero, new IntVector2(5, 0), new IntVector2(-1, 7), new IntVector2(-1, -4), new IntVector2(4, -4), new IntVector2(3, 0), IntVector2.zero)),
                (true, new Path(IntVector2.zero, new IntVector2(5, 0), new IntVector2(0, 7), new IntVector2(0, -4), new IntVector2(4, -4), new IntVector2(3, 0), IntVector2.zero)),
                (false, new Path(new IntVector2(5, 0), new IntVector2(0, 7), new IntVector2(0, -4), new IntVector2(-4, 0), new IntVector2(5, 0))),
                (false, new Path(IntVector2.zero, new IntVector2(5, 0), new IntVector2(0, 7), new IntVector2(0, -4), new IntVector2(-4, 0), IntVector2.zero)),
                (false, new Path(IntVector2.zero, new IntVector2(5, 0), new IntVector2(0, 7), new IntVector2(0, -4), new IntVector2(-4, -4), IntVector2.zero)),
                (true, new Path(IntVector2.zero, new IntVector2(2, 0), new IntVector2(1, 0), new IntVector2(1, 1), new IntVector2(1, 0))),
                (true, new Path(IntVector2.zero, new IntVector2(2, 0), new IntVector2(1, 0), new IntVector2(1, 1), new IntVector2(1, -1))),
                (false, new Path(IntVector2.zero, new IntVector2(2, 0), new IntVector2(1, 1), new IntVector2(1, -1))),
            };

            foreach ((bool isSimplePolygon, Path path) in testCases)
            {
                Assert.AreEqual(isSimplePolygon, path.isSimplePolygon, $"Failed with {path}.");
            }

            // Check that simple polygons must be loops
            foreach (Path path in RandomTestCases(1_000, false))
            {
                Assert.False(path.isSimplePolygon, $"Failed with {path}.");
            }

            foreach (Path path in RandomTestCases(1_000, true))
            {
                // Check the logical implication 'not self-intersecting => simple polygon'
                if (!path.selfIntersects)
                {
                    Assert.True(path.isSimplePolygon, $"Failed with {path}.");
                }
            }
        }

        /// <summary>
        /// Tests <see cref="Path.WindingNumber(IntVector2)"/> for some handmade examples.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void WindingNumberExamples()
        {
            (Path path, (int windingNumber, IntVector2 point)[] pathTestCases)[] testCases =
            {
                (new Path(IntVector2.downRight, IntVector2.upRight, IntVector2.upLeft, IntVector2.downLeft, IntVector2.downRight), new (int, IntVector2)[] {
                    (1, IntVector2.zero),
                    (0, new IntVector2(2, 0)),
                    (0, new IntVector2(-2, 1)),
                    (0, new IntVector2(2, -1)),
                    (0, new IntVector2(0, 2))
                }),
                (new Path(IntVector2.zero, new IntVector2(3, 0), new IntVector2(3, 3), IntVector2.zero), new (int, IntVector2)[] {
                    (1, new IntVector2(2, 1)),
                    (0, new IntVector2(2, 3)),
                    (0, new IntVector2(-1, 0)),
                    (0, new IntVector2(4, 0)),
                    (0, new IntVector2(4, 1)),
                    (0, new IntVector2(2, 4))
                }),
                (new Path(IntVector2.zero), new (int, IntVector2)[] {
                    (0, IntVector2.up),
                    (0, IntVector2.right),
                    (0, IntVector2.downLeft),
                    (0, new IntVector2(4, -3))
                }),
                (new Path(
                    new Line(IntVector2.zero, new IntVector2(3, 0)),
                    new Line(new IntVector2(4, 1), new IntVector2(6, 1)),
                    new Line(new IntVector2(7, 2), new IntVector2(3, 10)),
                    new Line(new IntVector2(3, 10), IntVector2.zero)
                    ),
                    new (int, IntVector2)[] {
                        (1, new IntVector2(3, 1)),
                        (1, new IntVector2(2, 2)),
                        (1, new IntVector2(5, 2)),
                        (1, new IntVector2(3, 7)),
                        (0, new IntVector2(0, 6)),
                        (0, new IntVector2(-1, 0)),
                        (0, new IntVector2(-3, 1)),
                        (0, new IntVector2(-1, 2))
                    }),
                (new Path(
                    new Line(IntVector2.zero, IntVector2.zero),
                    new Line(IntVector2.zero, IntVector2.one)
                    ),
                    new (int, IntVector2)[] {
                        (0, new IntVector2(0, 1)),
                        (0, new IntVector2(-1, 0)),
                        (0, new IntVector2(-1, 1))
                    }),
                (new Path(
                    new Line(IntVector2.zero, new IntVector2(5, 0)),
                    new Line(new IntVector2(5, 0), IntVector2.zero)
                    ),
                    new (int, IntVector2)[] {
                        (0, new IntVector2(0, 1)),
                        (0, new IntVector2(0, -1)),
                        (0, new IntVector2(-1, 0)),
                        (0, new IntVector2(6, 0))
                    }),
                (new Path((0, 0), (0, -1), (1, -2), (2, -1), (1, 0), (0, 1), (-1, 2), (-2, 1), (-1, 0)),
                    new (int, IntVector2)[] {
                        (1, new IntVector2(1, -1)),
                        (1, new IntVector2(-1, 1)),
                        (0, new IntVector2(2, 0)),
                        (0, new IntVector2(-2, 0)),
                        (0, new IntVector2(0, -2)),
                        (0, new IntVector2(-3, 1)),
                    }),
                (new Path(
                    new Line((2, 2), (1, 2)),
                    new Line((2, 1), (0, 3)),
                    new Line((0, 3), (3, 2))
                    ),
                    new (int, IntVector2)[] {
                        (0, new IntVector2(0, 2))
                    })
            };

            foreach ((Path path, (int, IntVector2)[] pathTestCases) in testCases)
            {
                foreach ((int windingNumber, IntVector2 point) in pathTestCases)
                {
                    Assert.AreEqual(windingNumber, path.WindingNumber(point), $"Failed with {path} and {point}.");
                }
            }

            // Path must be a loop
            Assert.Throws<ArgumentException>(() => new Path(IntVector2.zero, IntVector2.right, IntVector2.upRight).WindingNumber(IntVector2.zero));

            // Winding number of point on path is undefined
            Assert.Throws<ArgumentException>(() => new Path(IntVector2.zero).WindingNumber(IntVector2.zero));
            Assert.Throws<ArgumentException>(() => new Path(IntVector2.zero, IntVector2.right, new IntVector2(1, 2), IntVector2.zero).WindingNumber(IntVector2.one));
        }

        private void TestWindingNumberZeroOrNonZero(Path path)
        {
            if (path.lines.Count > 3)
                //throw new ArgumentException("This test is only guaranteed to work correctly for convex shapes. For this reason, we currently only allow test cases made of <= 3 lines.", nameof(path));
            }

            HashSet<IntVector2> points = path.ToHashSet();
            IntRect boundingRect = path.boundingRect;
            IntRect testRegion = new IntRect(boundingRect.bottomLeft + IntVector2.downLeft, boundingRect.topRight + IntVector2.upRight);

            for (int iteration = 0; iteration < 10; iteration++)
            {
                IntVector2 point = testRegion.RandomPoint();

                if (points.Contains(point))
                {
                    continue;
                }

                bool outside = false;
                foreach (IntVector2 direction in IntVector2.upDownLeftRight)
                {
                    bool hitLine = false;
                    for (IntVector2 check = point; testRegion.Contains(check); check += direction)
                    {
                        if (points.Contains(check))
                        {
                            hitLine = true;
                            break;
                        }
                    }

                    if (!hitLine)
                    {
                        outside = true;
                        break;
                    }
                }

                Assert.AreEqual(outside, path.WindingNumber(point) == 0, $"Failed with {path} and {point}.");
            }
        }

        /// <summary>
        /// Tests that <see cref="Path.WindingNumber(IntVector2)"/> is correctly zero / non-zero for simple polygons.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void WindingNumberSimplePolygons()
        {
            foreach (Path path in RandomTestCases(100, true))
            {
                if (path.isSimplePolygon)
                {
                    TestWindingNumberZeroOrNonZero(path);
                }
            }
        }

        /// <summary>
        /// Tests that <see cref="Path.WindingNumber(IntVector2)"/> is correctly zero / non-zero for <see cref="Path"/>s that don't self-intersect.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void WindingNumberNonSelfIntersecting()
        {
            TestWindingNumberZeroOrNonZero(new Path(new Line((5, 5), (4, 5)), new Line((4, 5), (5, 4))));

            foreach (Path path in RandomTestCases(100, true))
            {
                if (!path.selfIntersects)
                {
                    TestWindingNumberZeroOrNonZero(path);
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void Equals()
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
