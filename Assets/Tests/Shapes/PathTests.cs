using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PAC.DataStructures;
using PAC.Drawing;

namespace PAC.Tests
{
    public class PathTests : I1DShapeTests<Shapes.Path>
    {
        public override IEnumerable<Shapes.Path> testCases => RandomTestCases(1_000);
        private IEnumerable<Shapes.Path> RandomTestCases(int numOfTestCases) => RandomTestCases(numOfTestCases, false).Concat(RandomTestCases(numOfTestCases, true));
        private IEnumerable<Shapes.Path> RandomTestCases(int numOfTestCases, bool isLoop)
        {
            for (int length = 1; length <= 3; length++)
            {
                for (int iteration = 0; iteration < numOfTestCases; iteration++)
                {
                    yield return RandomPath(length, isLoop);
                }
            }
        }
        private Shapes.Path RandomPath(int length, bool isLoop)
        {
            List<Shapes.Line> lines = new List<Shapes.Line>
            {
                new Shapes.Line(new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)).RandomPoint(), new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)).RandomPoint())
            };

            for (int i = 0; i < length - 1; i++)
            {
                IntVector2 start = lines[^1].end + new IntRect(new IntVector2(-1, -1), new IntVector2(1, 1)).RandomPoint();
                lines.Add(new Shapes.Line(start, start + new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)).RandomPoint()));
            }

            if (isLoop)
            {
                lines.Add(new Shapes.Line(
                    lines[^1].end + new IntRect(new IntVector2(-1, -1), new IntVector2(1, 1)).RandomPoint(),
                    lines[0].start + new IntRect(new IntVector2(-1, -1), new IntVector2(1, 1)).RandomPoint()
                    ));

                return new Shapes.Path(lines);
            }
            else
            {
                Shapes.Path path = new Shapes.Path(lines);
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
            Assert.Throws<ArgumentException>(() => new Shapes.Path(new Shapes.Line[0]));
            Assert.Throws<ArgumentException>(() => new Shapes.Path(new Shapes.Line(IntVector2.zero, new IntVector2(1, 1)), new Shapes.Line(new IntVector2(2, 3), new IntVector2(5, 5))));
            Assert.Throws<ArgumentException>(() => new Shapes.Path(new Shapes.Line(IntVector2.zero, new IntVector2(1, 1)), new Shapes.Line(new IntVector2(2, 3), new IntVector2(1, 1))));

            Assert.Throws<ArgumentException>(() => new Shapes.Path(new IntVector2[0]));
            Assert.DoesNotThrow(() => new Shapes.Path(new Shapes.Line(IntVector2.zero, new IntVector2(1, 1))));
            Assert.DoesNotThrow(() => new Shapes.Path(new Shapes.Line(IntVector2.zero, new IntVector2(1, 1)), new Shapes.Line(new IntVector2(1, 1), new IntVector2(5, 5))));
            Assert.DoesNotThrow(() => new Shapes.Path(new Shapes.Line(IntVector2.zero, new IntVector2(1, 1)), new Shapes.Line(new IntVector2(1, 2), new IntVector2(5, 5))));
            Assert.DoesNotThrow(() => new Shapes.Path(new Shapes.Line(IntVector2.zero, new IntVector2(1, 1)), new Shapes.Line(new IntVector2(2, 2), new IntVector2(5, 5))));

            Assert.AreEqual(new Shapes.Path(new Shapes.Line(IntVector2.zero, IntVector2.zero)), new Shapes.Path(IntVector2.zero));
            Assert.AreEqual(new Shapes.Path(new Shapes.Line(IntVector2.zero, IntVector2.one)), new Shapes.Path(IntVector2.zero, IntVector2.one));
            Assert.AreEqual(new Shapes.Path(new Shapes.Line(IntVector2.zero, IntVector2.one), new Shapes.Line(IntVector2.one, new IntVector2(2, 4))),
                new Shapes.Path(IntVector2.zero, IntVector2.one, new IntVector2(2, 4)));
        }

        [Test]
        [Category("Shapes")]
        public override void ShapeSinglePoint()
        {
            foreach (IntVector2 pixel in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
            {
                for (int repetitions = 1; repetitions <= 5; repetitions++)
                {
                    Shapes.Path path = new Shapes.Path(Enumerable.Repeat(pixel, repetitions));
                    CollectionAssert.AreEqual(new IntVector2[] { pixel }, path, "Failed with " + path);
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void ShapeExamples()
        {
            (IEnumerable<IntVector2> expected, Shapes.Path path)[] testCases =
            {
                (new IntVector2[] { IntVector2.zero },
                    new Shapes.Path(IntVector2.zero)),
                (new Shapes.Line(IntVector2.zero, new IntVector2(2, 3)),
                    new Shapes.Path(IntVector2.zero, new IntVector2(2, 3))),
                (new Shapes.Line(IntVector2.zero, new IntVector2(2, 3)).Concat(new Shapes.Line(new IntVector2(2, 3), new IntVector2(4, 4))[1..]),
                    new Shapes.Path(IntVector2.zero, new IntVector2(2, 3), new IntVector2(4, 4))),
                // Loop
                (new Shapes.Line(IntVector2.zero, new IntVector2(2, 3)).Concat(new Shapes.Line(new IntVector2(2, 3), new IntVector2(4, 4))[1..])
                    .Concat(new Shapes.Line(new IntVector2(4, 4), IntVector2.zero)[1..^1]),
                    new Shapes.Path(IntVector2.zero, new IntVector2(2, 3), new IntVector2(4, 4), IntVector2.zero)),
                // Crossing previous values
                (new Shapes.Line(IntVector2.zero, new IntVector2(2, 3)).Concat(new Shapes.Line(new IntVector2(2, 3), new IntVector2(4, 4))[1..])
                    .Concat(new Shapes.Line(new IntVector2(4, 4), new IntVector2(2, 4))[1..]).Concat(new Shapes.Line(new IntVector2(2, 4), new IntVector2(2, 0))[1..]),
                    new Shapes.Path(IntVector2.zero, new IntVector2(2, 3), new IntVector2(4, 4), new IntVector2(2, 4), new IntVector2(2, 0))),
                (new IntVector2[] { IntVector2.zero, IntVector2.right, IntVector2.upRight },
                    new Shapes.Path(IntVector2.zero, IntVector2.right, IntVector2.upRight, IntVector2.zero)),
                (new IntVector2[] { new IntVector2(4, 2), new IntVector2(5, 1) },
                    new Shapes.Path(new IntVector2(4, 2), new IntVector2(5, 1))),
                (new IntVector2[] { IntVector2.zero },
                    new Shapes.Path(IntVector2.zero, IntVector2.zero, IntVector2.zero, IntVector2.zero)),
                (new Shapes.Line(IntVector2.zero, new IntVector2(2, 5)).Concat(new Shapes.Line(new IntVector2(2, 5), new IntVector2(3, 2))[1..]),
                    new Shapes.Path(IntVector2.zero, new IntVector2(2, 5), new IntVector2(3, 2), new IntVector2(3, 2))),
                (new Shapes.Line(IntVector2.zero, new IntVector2(2, 5)).Concat(new Shapes.Line(new IntVector2(2, 5), new IntVector2(3, 2))[1..])
                    .Concat(new Shapes.Line(new IntVector2(3, 2), IntVector2.zero)[1..^1]),
                    new Shapes.Path(IntVector2.zero, new IntVector2(2, 5), new IntVector2(3, 2), new IntVector2(3, 2), IntVector2.zero))
            };

            foreach ((IEnumerable<IntVector2> expected, Shapes.Path path) in testCases)
            {
                CollectionAssert.AreEqual(expected, path, "Failed with " + path);
            }
        }

        [Test]
        [Category("Shapes")]
        public void IsLoop()
        {
            Assert.AreEqual(false, new Shapes.Path(IntVector2.zero, new IntVector2(2, 1)).isLoop);
            Assert.AreEqual(false, new Shapes.Path(IntVector2.zero, new IntVector2(1, 1), new IntVector2(2, 2), new IntVector2(5, 5)).isLoop);
            Assert.AreEqual(false, new Shapes.Path(IntVector2.zero, new IntVector2(1, 1), new IntVector2(2, 2), new IntVector2(2, 1)).isLoop);

            Assert.AreEqual(true, new Shapes.Path(IntVector2.zero).isLoop);
            Assert.AreEqual(true, new Shapes.Path(IntVector2.zero, new IntVector2(1, 1)).isLoop);
            Assert.AreEqual(true, new Shapes.Path(IntVector2.zero, new IntVector2(1, 1), new IntVector2(2, 2), IntVector2.zero).isLoop);
            Assert.AreEqual(true, new Shapes.Path(IntVector2.zero, new IntVector2(1, 1), new IntVector2(2, 2), new IntVector2(0, 1)).isLoop);
            Assert.AreEqual(true, new Shapes.Path(IntVector2.zero, new IntVector2(1, 1), new IntVector2(2, 2), new IntVector2(1, 1)).isLoop);
        }

        [Test]
        [Category("Shapes")]
        public override void BoundingRect()
        {
            foreach (Shapes.Path path in RandomTestCases(2_000))
            {
                IShapeTestHelper.BoundingRect(path);
            }
        }

        [Test]
        [Category("Shapes")]
        public override void Count()
        {
            // Pre-defined example-based tests

            (int, Shapes.Path)[] testCases =
            {
                (1, new Shapes.Path(IntVector2.zero)),
                (4, new Shapes.Path(IntVector2.zero, new IntVector2(2, 3))),
                (6, new Shapes.Path(IntVector2.zero, new IntVector2(2, 3), new IntVector2(4, 4))),
                // Loop
                (9, new Shapes.Path(IntVector2.zero, new IntVector2(2, 3), new IntVector2(4, 4), IntVector2.zero)),
                // Crossing previous values
                (12, new Shapes.Path(IntVector2.zero, new IntVector2(2, 3), new IntVector2(4, 4), new IntVector2(2, 4), new IntVector2(2, 0))),
                (3, new Shapes.Path(IntVector2.zero, IntVector2.right, IntVector2.upRight, IntVector2.zero)),
                (3, new Shapes.Path(IntVector2.zero, IntVector2.upRight, IntVector2.right, IntVector2.zero)),
                (12, new Shapes.Path(
                    new Shapes.Line(new IntVector2(-1, 5), new IntVector2(0, 4)), new Shapes.Line(new IntVector2(1, 3), new IntVector2(-4, 7)),
                    new Shapes.Line(new IntVector2(-4, 6), new IntVector2(-1, 5)), new Shapes.Line(new IntVector2(-1, 5), new IntVector2(-1, 5))
                ))
            };
            
            foreach ((int expected, Shapes.Path path) in testCases)
            {
                Assert.AreEqual(expected, path.Count, "Failed with " + path);
                // Check that what we have implemented Count to count is the number of pixels in the IEnumerable
                Assert.AreEqual(((IEnumerable<IntVector2>)path).Count(), path.Count, "Failed with " + path);
            }

            // Random tests

            foreach (Shapes.Path path in RandomTestCases(2_000))
            {
                Assert.AreEqual(((IEnumerable<IntVector2>)path).Count(), path.Count, "Failed with " + path);
            }
        }

        /// <summary>
        /// Tests that pixels never appear twice in a row in a path.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public override void NoRepeats()
        {
            for (int length = 1; length <= 3; length++)
            {
                foreach (bool isLoop in new bool[] { false, true })
                {
                    for (int iteration = 0; iteration < 1_000; iteration++)
                    {
                        Shapes.Path path = RandomPath(length, isLoop);
                        IntVector2[] pixels = path.ToArray();

                        if (pixels.Length == 1)
                        {
                            continue;
                        }

                        for (int i = 0; i < pixels.Length; i++)
                        {
                            Assert.AreNotEqual(pixels[(i + 1) % pixels.Length], pixels[i], "Failed with " + path + " at index " + i);
                        }
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void MinX()
        {
            foreach (Shapes.Path path in testCases)
            {
                for (int y = path.boundingRect.bottomLeft.y; y <= path.boundingRect.topRight.y; y++)
                {
                    Assert.AreEqual(path.Where(p => p.y == y).Min(p => p.x), path.MinX(y), "Failed with " + path + " and y = " + y);
                }

                Assert.Throws<ArgumentOutOfRangeException>(() => path.MinX(path.boundingRect.bottomLeft.y - 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => path.MinX(path.boundingRect.topRight.y + 1));
            }
        }

        [Test]
        [Category("Shapes")]
        public void MaxX()
        {
            foreach (Shapes.Path path in testCases)
            {
                for (int y = path.boundingRect.bottomLeft.y; y <= path.boundingRect.topRight.y; y++)
                {
                    Assert.AreEqual(path.Where(p => p.y == y).Max(p => p.x), path.MaxX(y), "Failed with " + path + " and y = " + y);
                }

                Assert.Throws<ArgumentOutOfRangeException>(() => path.MaxX(path.boundingRect.bottomLeft.y - 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => path.MaxX(path.boundingRect.topRight.y + 1));
            }
        }

        [Test]
        [Category("Shapes")]
        public void MinY()
        {
            foreach (Shapes.Path path in testCases)
            {
                for (int x = path.boundingRect.bottomLeft.x; x <= path.boundingRect.topRight.x; x++)
                {
                    Assert.AreEqual(path.Where(p => p.x == x).Min(p => p.y), path.MinY(x), "Failed with " + path + " and x = " + x);
                }

                Assert.Throws<ArgumentOutOfRangeException>(() => path.MinY(path.boundingRect.bottomLeft.x - 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => path.MinY(path.boundingRect.topRight.x + 1));
            }
        }

        [Test]
        [Category("Shapes")]
        public void MaxY()
        {
            foreach (Shapes.Path path in testCases)
            {
                for (int x = path.boundingRect.bottomLeft.x; x <= path.boundingRect.topRight.x; x++)
                {
                    Assert.AreEqual(path.Where(p => p.x == x).Max(p => p.y), path.MaxY(x), "Failed with " + path + " and x = " + x);
                }

                Assert.Throws<ArgumentOutOfRangeException>(() => path.MaxY(path.boundingRect.bottomLeft.x - 1));
                Assert.Throws<ArgumentOutOfRangeException>(() => path.MaxY(path.boundingRect.topRight.x + 1));
            }
        }

        [Test]
        [Category("Shapes")]
        public void SelfIntersects()
        {
            foreach (Shapes.Path path in testCases)
            {
                if (!path.selfIntersects)
                {
                    IShapeTestHelper.NoRepeatsAtAll(path);
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
                    Assert.True(hasDuplicate, "Failed with " + path);
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void IsSimplePolygon()
        {
            (bool isSimplePolygon, Shapes.Path path)[] testCases =
            {
                (true, new Shapes.Path(IntVector2.zero, IntVector2.up, IntVector2.upRight, IntVector2.right, IntVector2.zero)),
                (true, new Shapes.Path(IntVector2.downLeft, IntVector2.upLeft, IntVector2.upRight, IntVector2.downRight, IntVector2.downLeft)),
                (true, new Shapes.Path(IntVector2.zero, IntVector2.upRight, IntVector2.right, IntVector2.zero)),
                (true, new Shapes.Path(IntVector2.zero, IntVector2.zero)),
                (true, new Shapes.Path(IntVector2.zero, IntVector2.zero, IntVector2.zero)),
                (true, new Shapes.Path(IntVector2.zero, IntVector2.right, IntVector2.zero)),
                (true, new Shapes.Path(IntVector2.zero, new IntVector2(5, 0), IntVector2.zero)),
                (true, new Shapes.Path(IntVector2.zero, new IntVector2(5, 0), new IntVector2(0, 7), new IntVector2(0, -4), IntVector2.zero)),
                (true, new Shapes.Path(IntVector2.zero, new IntVector2(5, 0), new IntVector2(0, 7), new IntVector2(0, -4), new IntVector2(4, -4), IntVector2.zero)),
                (true, new Shapes.Path(IntVector2.zero, new IntVector2(5, 0), new IntVector2(-1, 7), new IntVector2(-1, -4), new IntVector2(4, -4), new IntVector2(3, 0), IntVector2.zero)),
                (true, new Shapes.Path(IntVector2.zero, new IntVector2(5, 0), new IntVector2(0, 7), new IntVector2(0, -4), new IntVector2(4, -4), new IntVector2(3, 0), IntVector2.zero)),
                (false, new Shapes.Path(new IntVector2(5, 0), new IntVector2(0, 7), new IntVector2(0, -4), new IntVector2(-4, 0), new IntVector2(5, 0))),
                (false, new Shapes.Path(IntVector2.zero, new IntVector2(5, 0), new IntVector2(0, 7), new IntVector2(0, -4), new IntVector2(-4, 0), IntVector2.zero)),
                (false, new Shapes.Path(IntVector2.zero, new IntVector2(5, 0), new IntVector2(0, 7), new IntVector2(0, -4), new IntVector2(-4, -4), IntVector2.zero)),
                (true, new Shapes.Path(IntVector2.zero, new IntVector2(2, 0), new IntVector2(1, 0), new IntVector2(1, 1), new IntVector2(1, 0))),
                (true, new Shapes.Path(IntVector2.zero, new IntVector2(2, 0), new IntVector2(1, 0), new IntVector2(1, 1), new IntVector2(1, -1))),
                (false, new Shapes.Path(IntVector2.zero, new IntVector2(2, 0), new IntVector2(1, 1), new IntVector2(1, -1))),
            };

            foreach ((bool isSimplePolygon, Shapes.Path path) in testCases)
            {
                Assert.AreEqual(isSimplePolygon, path.isSimplePolygon, "Failed with " + path);
            }

            // Check that simple polygons must be loops
            foreach (Shapes.Path path in RandomTestCases(1_000, false))
            {
                Assert.False(path.isSimplePolygon, "Failed with " + path);
            }

            foreach (Shapes.Path path in RandomTestCases(1_000, true))
            {
                // Check the logical implication 'not self-intersecting => simple polygon'
                if (!path.selfIntersects)
                {
                    Assert.True(path.isSimplePolygon, "Failed with " + path);
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void WindingNumber()
        {
            (Shapes.Path path, (int windingNumber, IntVector2 point)[])[] testCases =
            {
                (new Shapes.Path(IntVector2.downRight, IntVector2.upRight, IntVector2.upLeft, IntVector2.downLeft, IntVector2.downRight), new (int, IntVector2)[] {
                    (1, IntVector2.zero),
                    (0, new IntVector2(2, 0)),
                    (0, new IntVector2(-2, 1)),
                    (0, new IntVector2(2, -1)),
                    (0, new IntVector2(0, 2))
                }),
                (new Shapes.Path(IntVector2.zero, new IntVector2(3, 0), new IntVector2(3, 3), IntVector2.zero), new (int, IntVector2)[] {
                    (1, new IntVector2(2, 1)),
                    (0, new IntVector2(2, 3)),
                    (0, new IntVector2(-1, 0)),
                    (0, new IntVector2(4, 0)),
                    (0, new IntVector2(4, 1)),
                    (0, new IntVector2(2, 4))
                }),
                (new Shapes.Path(IntVector2.zero), new (int, IntVector2)[] {
                    (0, IntVector2.up),
                    (0, IntVector2.right),
                    (0, IntVector2.downLeft),
                    (0, new IntVector2(4, -3))
                }),
                (new Shapes.Path(
                    new Shapes.Line(IntVector2.zero, new IntVector2(3, 0)),
                    new Shapes.Line(new IntVector2(4, 1), new IntVector2(6, 1)),
                    new Shapes.Line(new IntVector2(7, 2), new IntVector2(3, 10)),
                    new Shapes.Line(new IntVector2(3, 10), IntVector2.zero)
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
            };

            foreach ((Shapes.Path path, (int, IntVector2)[] pathTestCases) in testCases)
            {
                foreach ((int windingNumber, IntVector2 point) in pathTestCases)
                {
                    Assert.AreEqual(windingNumber, path.WindingNumber(point), "Failed with " + path + " and " + point);
                }
            }

            // Path must be a loop
            Assert.Throws<ArgumentException>(() => new Shapes.Path(IntVector2.zero, IntVector2.right, IntVector2.upRight).WindingNumber(IntVector2.zero));

            // Winding number of point on path is undefined
            Assert.Throws<ArgumentException>(() => new Shapes.Path(IntVector2.zero).WindingNumber(IntVector2.zero));
            Assert.Throws<ArgumentException>(() => new Shapes.Path(IntVector2.zero, IntVector2.right, new IntVector2(1, 2), IntVector2.zero).WindingNumber(IntVector2.one));
        }
    }
}
