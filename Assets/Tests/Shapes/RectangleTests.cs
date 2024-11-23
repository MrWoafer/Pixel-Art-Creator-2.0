using NUnit.Framework;
using PAC.DataStructures;
using PAC.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace PAC.Tests
{
    public class RectangleTests : IShapeTests
    {
        [Test]
        [Category("Shapes")]
        public void ShapeSinglePoint()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 pixel in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
                {
                    Assert.True(new Shapes.Rectangle(pixel, pixel, filled).SequenceEqual(new IntVector2[] { pixel }), "Failed with " + pixel);
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void BoundingRect()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 bottomLeft in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
                {
                    foreach (IntVector2 topRight in bottomLeft + new IntRect(IntVector2.zero, new IntVector2(5, 5)))
                    {
                        IShapeTestHelper.BoundingRect(new Shapes.Rectangle(bottomLeft, topRight, filled));
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void Count()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 bottomLeft in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
                {
                    foreach (IntVector2 topRight in bottomLeft + new IntRect(IntVector2.zero, new IntVector2(5, 5)))
                    {
                        IShapeTestHelper.Count(new Shapes.Rectangle(bottomLeft, topRight, filled));
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void Contains()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 bottomLeft in new IntRect(new IntVector2(-2, -2), new IntVector2(2, 2)))
                {
                    foreach (IntVector2 topRight in bottomLeft + new IntRect(IntVector2.zero, new IntVector2(5, 5)))
                    {
                        IShapeTestHelper.Contains(new Shapes.Rectangle(bottomLeft, topRight, filled));
                    }
                }
            }
        }

        /// <summary>
        /// Tests that the enumerator doesn't repeat any pixels.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public void NoRepeats()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 topRight in new IntRect(IntVector2.zero, new IntVector2(10, 10)))
                {
                    IShapeTestHelper.NoRepeatsAtAll(new Shapes.Rectangle(IntVector2.zero, topRight, filled));
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void TranslationalInvariance()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 bottomLeft in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
                {
                    foreach (IntVector2 topRight in bottomLeft + new IntRect(IntVector2.zero, new IntVector2(5, 5)))
                    {
                        Shapes.Rectangle rectangle = new Shapes.Rectangle(bottomLeft, topRight, filled);

                        Shapes.Rectangle expected = new Shapes.Rectangle(IntVector2.zero, topRight - bottomLeft, filled);
                        IEnumerable<IntVector2> translated = rectangle.Select(p => p - bottomLeft);

                        Assert.True(expected.SequenceEqual(translated), "Failed with " + rectangle);
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void RotationalInvariance()
        {
            foreach (bool filled in new bool[] { false, true })
            {
                foreach (IntVector2 bottomLeft in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
                {
                    foreach (IntVector2 topRight in bottomLeft + new IntRect(IntVector2.zero, new IntVector2(5, 5)))
                    {
                        Shapes.Rectangle rectangle = new Shapes.Rectangle(bottomLeft, topRight, filled);

                        Shapes.Rectangle expected = new Shapes.Rectangle(rectangle.bottomLeft.Rotate(RotationAngle._90), rectangle.topRight.Rotate(RotationAngle._90), filled);
                        IEnumerable<IntVector2> rotated = rectangle.Select(p => p.Rotate(RotationAngle._90));

                        Assert.True(expected.ToHashSet().SetEquals(rotated), "Failed with " + rectangle);
                    }
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public void ReflectiveInvariance()
        {
            foreach (FlipAxis axis in new FlipAxis[] { FlipAxis.Vertical, FlipAxis.Horizontal, FlipAxis._45Degrees, FlipAxis.Minus45Degrees })
            {
                foreach (bool filled in new bool[] { false, true })
                {
                    foreach (IntVector2 bottomLeft in new IntRect(new IntVector2(-5, -5), new IntVector2(5, 5)))
                    {
                        foreach (IntVector2 topRight in bottomLeft + new IntRect(IntVector2.zero, new IntVector2(5, 5)))
                        {
                            Shapes.Rectangle rectangle = new Shapes.Rectangle(bottomLeft, topRight, filled);

                            Shapes.Rectangle expected = new Shapes.Rectangle(rectangle.bottomLeft.Flip(axis), rectangle.topRight.Flip(axis), filled);
                            IEnumerable<IntVector2> reflected = rectangle.Select(p => p.Flip(axis));

                            Assert.True(expected.ToHashSet().SetEquals(reflected), "Failed with " + rectangle + " and FlipAxis." + axis);
                        }
                    }
                }
            }
        }
    }
}
