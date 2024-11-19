using NUnit.Framework;
using PAC.DataStructures;
using System;

namespace PAC.Tests
{
    public class IntRectTests
    {
        /// <summary>
        /// Tests that changing the corners of an IntRect works correctly.
        /// </summary>
        [Test]
        [Category("Data Structures")]
        public void Resize()
        {
            IntRect rect = new IntRect(IntVector2.zero, new IntVector2(2, 3));
            Assert.AreEqual(new IntVector2(0, 0), rect.bottomLeft);
            Assert.AreEqual(new IntVector2(2, 3), rect.topRight);
            Assert.AreEqual(new IntVector2(0, 3), rect.topLeft);
            Assert.AreEqual(new IntVector2(2, 0), rect.bottomRight);

            // Move bottom-left

            // Position moved to is new bottom-left
            rect.bottomLeft = new IntVector2(1, 1);
            Assert.AreEqual(new IntVector2(1, 1), rect.bottomLeft);
            Assert.AreEqual(new IntVector2(2, 3), rect.topRight);
            Assert.AreEqual(new IntVector2(1, 3), rect.topLeft);
            Assert.AreEqual(new IntVector2(2, 1), rect.bottomRight);

            // Position moved to is new bottom-right
            rect.bottomLeft = new IntVector2(4, 1);
            Assert.AreEqual(new IntVector2(2, 1), rect.bottomLeft);
            Assert.AreEqual(new IntVector2(4, 3), rect.topRight);
            Assert.AreEqual(new IntVector2(2, 3), rect.topLeft);
            Assert.AreEqual(new IntVector2(4, 1), rect.bottomRight);

            // Position moved to is new top-left
            rect.bottomLeft = new IntVector2(2, 6);
            Assert.AreEqual(new IntVector2(2, 3), rect.bottomLeft);
            Assert.AreEqual(new IntVector2(4, 6), rect.topRight);
            Assert.AreEqual(new IntVector2(2, 6), rect.topLeft);
            Assert.AreEqual(new IntVector2(4, 3), rect.bottomRight);

            // Position moved to is new top-right
            rect.bottomLeft = new IntVector2(7, 7);
            Assert.AreEqual(new IntVector2(4, 6), rect.bottomLeft);
            Assert.AreEqual(new IntVector2(7, 7), rect.topRight);
            Assert.AreEqual(new IntVector2(4, 7), rect.topLeft);
            Assert.AreEqual(new IntVector2(7, 6), rect.bottomRight);

            // Move top-right

            // Position moved to is new top-right
            rect.topRight = new IntVector2(6, 7);
            Assert.AreEqual(new IntVector2(4, 6), rect.bottomLeft);
            Assert.AreEqual(new IntVector2(6, 7), rect.topRight);
            Assert.AreEqual(new IntVector2(4, 7), rect.topLeft);
            Assert.AreEqual(new IntVector2(6, 6), rect.bottomRight);

            // Position moved to is new top-left
            rect.topRight = new IntVector2(2, 8);
            Assert.AreEqual(new IntVector2(2, 6), rect.bottomLeft);
            Assert.AreEqual(new IntVector2(4, 8), rect.topRight);
            Assert.AreEqual(new IntVector2(2, 8), rect.topLeft);
            Assert.AreEqual(new IntVector2(4, 6), rect.bottomRight);

            // Position moved to is new bottom-right
            rect.topRight = new IntVector2(4, 1);
            Assert.AreEqual(new IntVector2(2, 1), rect.bottomLeft);
            Assert.AreEqual(new IntVector2(4, 6), rect.topRight);
            Assert.AreEqual(new IntVector2(2, 6), rect.topLeft);
            Assert.AreEqual(new IntVector2(4, 1), rect.bottomRight);

            // Position moved to is new bottom-left
            rect.topRight = new IntVector2(0, 0);
            Assert.AreEqual(new IntVector2(0, 0), rect.bottomLeft);
            Assert.AreEqual(new IntVector2(2, 1), rect.topRight);
            Assert.AreEqual(new IntVector2(0, 1), rect.topLeft);
            Assert.AreEqual(new IntVector2(2, 0), rect.bottomRight);

            // Move bottom-right

            // Position moved to is new bottom-right
            rect.bottomRight = new IntVector2(3, -1);
            Assert.AreEqual(new IntVector2(0, -1), rect.bottomLeft);
            Assert.AreEqual(new IntVector2(3, 1), rect.topRight);
            Assert.AreEqual(new IntVector2(0, 1), rect.topLeft);
            Assert.AreEqual(new IntVector2(3, -1), rect.bottomRight);

            // Position moved to is new bottom-left
            rect.bottomRight = new IntVector2(-2, 0);
            Assert.AreEqual(new IntVector2(-2, 0), rect.bottomLeft);
            Assert.AreEqual(new IntVector2(0, 1), rect.topRight);
            Assert.AreEqual(new IntVector2(-2, 1), rect.topLeft);
            Assert.AreEqual(new IntVector2(0, 0), rect.bottomRight);

            // Position moved to is new top-right
            rect.bottomRight = new IntVector2(3, 3);
            Assert.AreEqual(new IntVector2(-2, 1), rect.bottomLeft);
            Assert.AreEqual(new IntVector2(3, 3), rect.topRight);
            Assert.AreEqual(new IntVector2(-2, 3), rect.topLeft);
            Assert.AreEqual(new IntVector2(3, 1), rect.bottomRight);

            // Position moved to is new top-left
            rect.bottomRight = new IntVector2(-3, 4);
            Assert.AreEqual(new IntVector2(-3, 3), rect.bottomLeft);
            Assert.AreEqual(new IntVector2(-2, 4), rect.topRight);
            Assert.AreEqual(new IntVector2(-3, 4), rect.topLeft);
            Assert.AreEqual(new IntVector2(-2, 3), rect.bottomRight);

            // Move top-left

            // Position moved to is new top-left
            rect.topLeft = new IntVector2(-4, 5);
            Assert.AreEqual(new IntVector2(-4, 3), rect.bottomLeft);
            Assert.AreEqual(new IntVector2(-2, 5), rect.topRight);
            Assert.AreEqual(new IntVector2(-4, 5), rect.topLeft);
            Assert.AreEqual(new IntVector2(-2, 3), rect.bottomRight);

            // Position moved to is new top-right
            rect.topLeft = new IntVector2(0, 6);
            Assert.AreEqual(new IntVector2(-2, 3), rect.bottomLeft);
            Assert.AreEqual(new IntVector2(0, 6), rect.topRight);
            Assert.AreEqual(new IntVector2(-2, 6), rect.topLeft);
            Assert.AreEqual(new IntVector2(0, 3), rect.bottomRight);

            // Position moved to is new bottom-left
            rect.topLeft = new IntVector2(-3, 2);
            Assert.AreEqual(new IntVector2(-3, 2), rect.bottomLeft);
            Assert.AreEqual(new IntVector2(0, 3), rect.topRight);
            Assert.AreEqual(new IntVector2(-3, 3), rect.topLeft);
            Assert.AreEqual(new IntVector2(0, 2), rect.bottomRight);

            // Position moved to is new bottom-right
            rect.topLeft = new IntVector2(2, 0);
            Assert.AreEqual(new IntVector2(0, 0), rect.bottomLeft);
            Assert.AreEqual(new IntVector2(2, 2), rect.topRight);
            Assert.AreEqual(new IntVector2(0, 2), rect.topLeft);
            Assert.AreEqual(new IntVector2(2, 0), rect.bottomRight);
        }

        [Test]
        [Category("Data Structures")]
        public void BoundingRect()
        {
            // Bounding rect of IntVector2s

            (IntRect, IntVector2[])[] testCases =
            {
                (new IntRect(new IntVector2(1, -4), new IntVector2(1, -4)), new IntVector2[] {
                    new IntVector2(1, -4)
                }),
                (new IntRect(new IntVector2(3, 4), new IntVector2(5, 8)), new IntVector2[] {
                    new IntVector2(3, 4), new IntVector2(5, 8)
                }),
                (new IntRect(new IntVector2(2, 4), new IntVector2(9, 7)), new IntVector2[] {
                    new IntVector2(3, 4), new IntVector2(2, 6), new IntVector2(9, 7)
                }),
                (new IntRect(new IntVector2(-3, -2), new IntVector2(10, 9)), new IntVector2[] {
                    new IntVector2(-3, -1), new IntVector2(2, 6), new IntVector2(9, 7), new IntVector2(5, -2), new IntVector2(5, 5), new IntVector2(-3, -1), new IntVector2(10, 1)
                })
            };

            foreach ((IntRect expected, IntVector2[] points) in testCases)
            {
                IntRect boundingRect = IntRect.BoundingRect(points);

                Assert.AreEqual(expected, boundingRect);

                foreach (IntVector2 point in points)
                {
                    Assert.True(boundingRect.Contains(point));
                }
            }

            // Bounding rect of IntRects

            (IntRect, IntRect[])[] testCases2 =
            {
                (new IntRect(new IntVector2(3, 4), new IntVector2(5, 8)), new IntRect[] {
                    new IntRect(new IntVector2(3, 4), new IntVector2(5, 8))
                }),
                (new IntRect(new IntVector2(2, 4), new IntVector2(9, 8)), new IntRect[] {
                    new IntRect(new IntVector2(3, 4), new IntVector2(5, 8)),
                    new IntRect(new IntVector2(2, 6), new IntVector2(9, 7))
                }),
                (new IntRect(new IntVector2(0, 0), new IntVector2(9, 8)), new IntRect[] {
                    new IntRect(new IntVector2(3, 4), new IntVector2(5, 8)),
                    new IntRect(new IntVector2(2, 6), new IntVector2(9, 7)),
                    new IntRect(new IntVector2(0, 0), new IntVector2(0, 2))
                })
            };

            foreach ((IntRect expected, IntRect[] rects) in testCases2)
            {
                IntRect boundingRect = IntRect.BoundingRect(rects);

                Assert.AreEqual(expected, boundingRect);

                foreach (IntRect rect in rects)
                {
                    Assert.True(boundingRect.Contains(rect));
                }
            }

            // Cannot get bounding rect of 0 IntRects
            //Assert.Throws<ArgumentException>(() => IntRect.BoundingRect());   // The call is now ambiguous
        }
    }
}
