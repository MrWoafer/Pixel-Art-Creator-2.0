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
            //Assert.Throws<ArgumentException>(() => new Shapes.Path()); // The empty constructor is now private
            Assert.Throws<ArgumentException>(() => new Shapes.Path(new Shapes.Line(IntVector2.zero, new IntVector2(1, 1)), new Shapes.Line(new IntVector2(2, 3), new IntVector2(5, 5))));
            Assert.Throws<ArgumentException>(() => new Shapes.Path(new Shapes.Line(IntVector2.zero, new IntVector2(1, 1)), new Shapes.Line(new IntVector2(2, 3), new IntVector2(1, 1))));

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
        public void Count()
        {
            (int, Shapes.Path)[] testCases =
            {
                (1, new Shapes.Path(IntVector2.zero)),
                (4, new Shapes.Path(IntVector2.zero, new IntVector2(2, 3))),
                (6, new Shapes.Path(IntVector2.zero, new IntVector2(2, 3), new IntVector2(4, 4))),
                // Loop
                (9, new Shapes.Path(IntVector2.zero, new IntVector2(2, 3), new IntVector2(4, 4), IntVector2.zero)),
                // Crossing previous values
                (12, new Shapes.Path(IntVector2.zero, new IntVector2(2, 3), new IntVector2(4, 4), new IntVector2(2, 4), new IntVector2(2, 0)))
            };
            
            foreach ((int expected, Shapes.Path path) in testCases)
            {
                Assert.AreEqual(expected, path.Count, "Failed with " + path);
                // Check that what we have implemented Count to count is the number of pixels in the IEnumerable
                Assert.AreEqual(((IEnumerable<IntVector2>)path).Count(), path.Count, "Failed with " + path);
            }
        }

        [Test]
        [Category("Shapes")]
        public void GetEnumerator()
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
                    new Shapes.Path(IntVector2.zero, new IntVector2(2, 3), new IntVector2(4, 4), new IntVector2(2, 4), new IntVector2(2, 0)))
            };

            foreach ((IEnumerable<IntVector2> expected, Shapes.Path path) in testCases)
            {
                Assert.True(expected.SequenceEqual(path), "Failed with " + path);
            }
        }
    }
}
