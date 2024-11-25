using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using PAC.DataStructures;
using PAC.Drawing;

namespace PAC.Tests
{
    public class PathTests
    {
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
        public void ShapeSinglePoint()
        {
            foreach (IntVector2 pixel in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
            {
                for (int repetitions = 1; repetitions <= 5; repetitions++)
                {
                    Shapes.Path path = new Shapes.Path(Enumerable.Repeat(pixel, repetitions));
                    Assert.True(path.SequenceEqual(new IntVector2[] { pixel }), "Failed with " + path);
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void ShapeExamples()
        {
            (IEnumerable<IntVector2>, Shapes.Path)[] testCases =
            {
                (new IntVector2[] { IntVector2.zero }, new Shapes.Path(IntVector2.zero)),
                (new Shapes.Line(IntVector2.zero, new IntVector2(2, 3)), new Shapes.Path(IntVector2.zero, new IntVector2(2, 3))),
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
                (new IntVector2[] { IntVector2.zero, IntVector2.right, IntVector2.upRight }, new Shapes.Path(IntVector2.zero, IntVector2.right, IntVector2.upRight, IntVector2.zero)),
            };

            foreach ((IEnumerable<IntVector2> expected, Shapes.Path path) in testCases)
            {
                Assert.True(expected.SequenceEqual(path), "Failed with " + path);
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
            }

            return new Shapes.Path(lines);
        }

        [Test]
        [Category("Shapes")]
        public void BoundingRect()
        {
            for (int length = 1; length < 3; length++)
            {
                foreach (bool isLoop in new bool[] { false, true })
                {
                    for (int iteration = 0; iteration < 2_000; iteration++)
                    {
                        IShapeTestHelper.BoundingRect(RandomPath(length, isLoop));
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void Count()
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
            };
            
            foreach ((int expected, Shapes.Path path) in testCases)
            {
                Assert.AreEqual(expected, path.Count, "Failed with " + path);
                // Check that what we have implemented Count to count is the number of pixels in the IEnumerable
                Assert.AreEqual(((IEnumerable<IntVector2>)path).Count(), path.Count, "Failed with " + path);
            }

            // Random tests

            for (int length = 1; length < 3; length++)
            {
                foreach (bool isLoop in new bool[] { false, true })
                {
                    for (int iteration = 0; iteration < 2_000; iteration++)
                    {
                        Shapes.Path path = RandomPath(length, isLoop);
                        Assert.AreEqual(((IEnumerable<IntVector2>)path).Count(), path.Count, "Failed with " + path);
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void Contains()
        {
            for (int length = 1; length < 3; length++)
            {
                foreach (bool isLoop in new bool[] { false, true })
                {
                    for (int iteration = 0; iteration < 1_000; iteration++)
                    {
                        IShapeTestHelper.Contains(RandomPath(length, isLoop));
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void Translate()
        {
            for (int length = 1; length <= 3; length++)
            {
                foreach (bool isLoop in new bool[] { false, true })
                {
                    for (int iteration = 0; iteration < 1_000; iteration++)
                    {
                        IShapeTestHelper.Translate(RandomPath(length, isLoop));
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void Rotate()
        {
            for (int length = 1; length <= 3; length++)
            {
                foreach (bool isLoop in new bool[] { false, true })
                {
                    for (int iteration = 0; iteration < 1_000; iteration++)
                    {
                        IShapeTestHelper.Rotate(RandomPath(length, isLoop));
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void Flip()
        {
            for (int length = 1; length <= 3; length++)
            {
                foreach (bool isLoop in new bool[] { false, true })
                {
                    for (int iteration = 0; iteration < 1_000; iteration++)
                    {
                        IShapeTestHelper.Flip(RandomPath(length, isLoop));
                    }
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
