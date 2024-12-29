using NUnit.Framework;
using PAC.DataStructures;
using System.Collections.Generic;

namespace PAC.Tests
{
    public class IntRectTests
    {
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
                (new IntRect(new IntVector2(2, 4), new IntVector2(9, 7)), new IntVector2[] {
                    new IntVector2(3, 4), new IntVector2(3, 4), new IntVector2(2, 6), new IntVector2(2, 6), new IntVector2(9, 7), new IntVector2(9, 7)
                }),
                (new IntRect(new IntVector2(-3, -2), new IntVector2(10, 7)), new IntVector2[] {
                    new IntVector2(-3, -1), new IntVector2(2, 6), new IntVector2(9, 7), new IntVector2(5, -2), new IntVector2(5, 5), new IntVector2(10, 1)
                })
            };

            foreach ((IntRect expected, IntVector2[] points) in testCases)
            {
                IntRect boundingRect = IntRect.BoundingRect(points);

                Assert.AreEqual(expected, boundingRect, "Failed with " + Functions.ArrayToString(points));

                foreach (IntVector2 point in points)
                {
                    Assert.True(boundingRect.Contains(point), "Failed with " + point + " in " + Functions.ArrayToString(points));
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

                Assert.AreEqual(expected, boundingRect, "Failed with " + Functions.ArrayToString(rects));

                foreach (IntRect rect in rects)
                {
                    Assert.True(boundingRect.Contains(rect), "Failed with " + rect + " in " + Functions.ArrayToString(rects));
                }
            }

            // Cannot get bounding rect of 0 IntRects
            //Assert.Throws<ArgumentException>(() => IntRect.BoundingRect());   // The call is now ambiguous
        }

        [Test]
        [Category("Data Structures"), Category("Random")]
        public void RandomPoint()
        {
            foreach (IntVector2 bottomLeft in new IntRect(new IntVector2(-1, -1), IntVector2.zero))
            {
                foreach (IntVector2 topRight in bottomLeft + new IntRect(IntVector2.zero, new IntVector2(2, 3)))
                {
                    IntRect rect = new IntRect(bottomLeft, topRight);
                    Dictionary<IntVector2, int> counts = new Dictionary<IntVector2, int>();
                    foreach (IntVector2 pixel in rect)
                    {
                        counts[pixel] = 0;
                    }

                    const int iterations = 10_000;
                    for (int i = 0; i < iterations; i++)
                    {
                        IntVector2 randomPoint = rect.RandomPoint();
                        Assert.True(rect.Contains(randomPoint), "Failed with " + rect + " and " + randomPoint);

                        counts[randomPoint]++;
                    }

                    float expected = 1f / rect.Count;
                    float tolerance = expected / 5f;
                    foreach (IntVector2 pixel in rect)
                    {
                        Assert.That((float)counts[pixel] / iterations, Is.EqualTo(expected).Within(tolerance), "Failed with " + rect + " and " + pixel);
                    }
                }
            }
        }
    }
}
