using PAC.Drawing;
using NUnit.Framework;
using PAC.DataStructures;
using PAC.Extensions;
using System.Collections.Generic;
using System.Linq;
using System;

namespace PAC.Tests
{
    /// <summary>
    /// The tests that should be implemented for every shape.
    /// </summary>
    public abstract class IShapeTests<T> where T : Shapes.IShape
    {
        protected abstract IEnumerable<T> testCases
        {
            get;
        }

        [Test]
        [Category("Shapes")]
        public void NotEmpty()
        {
            foreach (T shape in testCases)
            {
                Assert.False(shape.IsEmpty(), "Failed with " + shape);
            }
        }

        /// <summary>
        /// Tests that instances of the shape that are single points have the correct shape.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public abstract void ShapeSinglePoint();

        [Test]
        [Category("Shapes")]
        public virtual void BoundingRect()
        {
            foreach (T shape in testCases)
            {
                IShapeTestHelper.BoundingRect(shape);
            }
        }

        /// <summary>
        /// The default implementation tests that the Count property matches the number of distinct pixels in the enumerator.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public virtual void Count()
        {
            foreach (T shape in testCases)
            {
                IShapeTestHelper.CountDistinct(shape);
            }
        }

        [Test]
        [Category("Shapes")]
        public virtual void Contains()
        {
            foreach (T shape in testCases)
            {
                IShapeTestHelper.Contains(shape);
            }
        }

        /// <summary>
        /// <para>
        /// Tests that the shape's enumerator doesn't repeat any pixels it doesn't need to.
        /// </para>
        /// <para>
        /// The default implementation tests that no pixels are repeated at all.
        /// </para>
        /// </summary>
        [Test]
        [Category("Shapes")]
        public virtual void NoRepeats()
        {
            foreach (T shape in testCases)
            {
                IShapeTestHelper.NoRepeatsAtAll(shape);
            }
        }

        /// <summary>
        /// Tests that the shape has exactly one connected component (defined in terms of pixels being adjacent, including diagonally).
        /// </summary>
        [Test]
        [Category("Shapes")]
        public virtual void Connected()
        {
            foreach (T shape in testCases)
            {
                IShapeTestHelper.Connected(shape);
            }
        }

        [Test]
        [Category("Shapes")]
        public virtual void Translate()
        {
            foreach (T shape in testCases)
            {
                IShapeTestHelper.Translate(shape);
            }
        }
        [Test]
        [Category("Shapes")]
        public virtual void Rotate()
        {
            foreach (T shape in testCases)
            {
                IShapeTestHelper.Rotate(shape);
            }
        }
        [Test]
        [Category("Shapes")]
        public virtual void Flip()
        {
            foreach (T shape in testCases)
            {
                IShapeTestHelper.Flip(shape);
            }
        }
    }

    /// <summary>
    /// Helper functions for making tests for IShapes.
    /// </summary>
    public static class IShapeTestHelper
    {
        /// <summary>
        /// Tests that the shape's boundingRect property is the correct bounding rect.
        /// </summary>
        public static void BoundingRect(Shapes.IShape shape)
        {
            Assert.AreEqual(IntRect.BoundingRect(shape), shape.boundingRect, "Failed with " + shape);
        }

        /// <summary>
        /// Tests that the shape's Count property matches the number of distinct pixels in its enumerator.
        /// </summary>
        public static void CountDistinct(Shapes.IShape shape)
        {
            int count = 0;
            HashSet<IntVector2> visited = new HashSet<IntVector2>();
            foreach (IntVector2 pixel in shape)
            {
                if (!visited.Contains(pixel))
                {
                    count++;
                    visited.Add(pixel);
                }
            }

            Assert.AreEqual(count, shape.Count, "Failed with " + shape);
        }

        /// <summary>
        /// Tests that the shape's Contains() method matches the set of pixels obtained from the shape's enumerator.
        /// </summary>
        public static void Contains(Shapes.IShape shape)
        {
            IntRect boundingRect = shape.boundingRect;
            IntRect testRegion = new IntRect(boundingRect.bottomLeft + IntVector2.downLeft, boundingRect.topRight + IntVector2.upRight);

            HashSet<IntVector2> pixels = shape.ToHashSet();

            foreach (IntVector2 pixel in testRegion)
            {
                // Using Assert.True(x == y) seems to be faster than Assert.AreEqual(x, y)
                Assert.True(pixels.Contains(pixel) == shape.Contains(pixel), "Failed with " + shape + " and " + pixel + ". Expected " + pixels.Contains(pixel));
            }
        }

        /// <summary>
        /// Tests that no pixels are repeated at all in the shape's enumerator.
        /// </summary>
        public static void NoRepeatsAtAll(Shapes.IShape shape)
        {
            HashSet<IntVector2> visited = new HashSet<IntVector2>();
            foreach (IntVector2 pixel in shape)
            {
                Assert.False(visited.Contains(pixel), "Failed with " + shape + " and " + pixel);
                visited.Add(pixel);
            }
        }

        /// <summary>
        /// <para>
        /// Tests that every pixel has another pixel adjacent to it (including diagonally).
        /// </para>
        /// <para>
        /// Note this is a strictly weaker test than Connected().
        /// </para>
        /// </summary>
        public static void NoIsolatedPoints(Shapes.IShape shape)
        {
            HashSet<IntVector2> pixels = shape.ToHashSet();
            foreach (IntVector2 pixel in shape)
            {
                bool hasAdjacent = false;
                foreach (IntVector2 check in (pixel + new IntRect(-IntVector2.one, IntVector2.one)).Where(p => p != pixel))
                {
                    if (pixels.Contains(check))
                    {
                        hasAdjacent = true;
                        break;
                    }
                }

                Assert.True(hasAdjacent, "Failed with " + shape + " and " + pixel);
            }
        }

        /// <summary>
        /// Tests that the shape has exactly one connected component (defined it terms of pixels being adjacent, including diagonally).
        /// </summary>
        public static void Connected(Shapes.IShape shape)
        {
            HashSet<IntVector2> pixels = shape.ToHashSet();
            HashSet<IntVector2> visited = new HashSet<IntVector2>();
            Queue<IntVector2> toVisit = new Queue<IntVector2>();
            toVisit.Enqueue(shape.First());
            
            while (toVisit.Count > 0)
            {
                IntVector2 pixel = toVisit.Dequeue();
                visited.Add(pixel);
                foreach (IntVector2 offsetPixel in pixel + new IntRect(-IntVector2.one, IntVector2.one))
                {
                    if (pixels.Contains(offsetPixel) && !visited.Contains(offsetPixel))
                    {
                        toVisit.Enqueue(offsetPixel);
                    }
                }
            }

            Assert.AreEqual(pixels.Count(), visited.Count, "Failed with " + shape);
        }

        /// <summary>
        /// Applies Translate() to the shape and checks that the enumerator of the resulting shape is a translation of the original shape's enumerator.
        /// </summary>
        public static void Translate(Shapes.IShape shape)
        {
            foreach (IntVector2 translation in new IntRect(new IntVector2(-2, -2), new IntVector2(2, 2)))
            {
                IEnumerable<IntVector2> expected = shape.Select(p => p + translation);
                Assert.True(expected.SequenceEqual(shape.Translate(translation)), "Failed with " + shape + " and " + translation);
            }
        }

        /// <summary>
        /// Applies Rotate() to the shape and checks that the enumerator of the resulting shape is a rotation of the original shape's enumerator.
        /// </summary>
        public static void Rotate(Shapes.IShape shape)
        {
            foreach (RotationAngle angle in new RotationAngle[] { RotationAngle._0, RotationAngle._90, RotationAngle._180, RotationAngle.Minus90 })
            {
                IEnumerable<IntVector2> expected = shape.Select(p => p.Rotate(angle));
                Assert.True(expected.ToHashSet().SetEquals(shape.Rotate(angle)), "Failed with " + shape + " and " + angle);
            }
        }

        /// <summary>
        /// Applies Flip() to the shape and checks that the enumerator of the resulting shape is a reflection of the original shape's enumerator.
        /// </summary>
        public static void Flip(Shapes.IShape shape)
        {
            foreach (FlipAxis axis in new FlipAxis[] { FlipAxis.None, FlipAxis.Vertical, FlipAxis.Horizontal, FlipAxis._45Degrees, FlipAxis.Minus45Degrees })
            {
                IEnumerable<IntVector2> expected = shape.Select(p => p.Flip(axis));
                Assert.True(expected.ToHashSet().SetEquals(shape.Flip(axis)), "Failed with " + shape + " and " + axis);
            }
        }

        /// <summary>
        /// Tests that the shape has reflective symmetry across the given axis.
        /// </summary>
        public static void ReflectiveSymmetry(Shapes.IShape shape, FlipAxis axis)
        {
            if (axis == FlipAxis.None)
            {
                Assert.Pass();
            }
            else if (axis == FlipAxis.Vertical)
            {
                Assert.True(shape.ToHashSet().SetEquals(shape.Select(p => new IntVector2(shape.boundingRect.bottomLeft.x + shape.boundingRect.topRight.x - p.x, p.y))),
                    "Failed with " + shape + " and FlipAxis." + axis);
            }
            else if (axis == FlipAxis.Horizontal)
            {
                Assert.True(shape.ToHashSet().SetEquals(shape.Select(p => new IntVector2(p.x, shape.boundingRect.bottomLeft.y + shape.boundingRect.topRight.y - p.y))),
                    "Failed with " + shape + " and FlipAxis." + axis);
            }
            else if (axis == FlipAxis._45Degrees)
            {
                Assert.True(shape.ToHashSet().SetEquals(shape.Select(p => shape.boundingRect.bottomRight + (p - shape.boundingRect.topLeft).Flip(FlipAxis._45Degrees))),
                    "Failed with " + shape + " and FlipAxis." + axis);
            }
            else if (axis == FlipAxis.Minus45Degrees)
            {
                Assert.True(shape.ToHashSet().SetEquals(shape.Select(p => shape.boundingRect.bottomLeft + (p - shape.boundingRect.topRight).Flip(FlipAxis.Minus45Degrees))),
                    "Failed with " + shape + " and FlipAxis." + axis);
            }
            else
            {
                throw new NotImplementedException("Unknown / unimplemented FlipAxis: " + axis);
            }
        }

        /// <summary>
        /// Tests that the shape has rotational symmetry by the given angle.
        /// </summary>
        public static void RotationalSymmetry(Shapes.IShape shape, RotationAngle angle)
        {
            Assert.True(shape.ToHashSet().SetEquals(shape.Select(p => p.Rotate(angle) + shape.boundingRect.bottomLeft - shape.boundingRect.Rotate(angle).bottomLeft)),
                "Failed with " + shape + " and RotationAngle." + angle);
        }
    }
}
