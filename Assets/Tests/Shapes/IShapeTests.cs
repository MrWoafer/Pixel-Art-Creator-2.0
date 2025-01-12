using NUnit.Framework;
using PAC.DataStructures;
using PAC.Exceptions;
using PAC.Extensions;
using System.Collections.Generic;
using System.Linq;
using System;
using PAC.Shapes;
using PAC.Shapes.Interfaces;

namespace PAC.Tests
{
    /// <summary>
    /// The tests that should be implemented for every shape.
    /// </summary>
    public abstract class IShapeTests<T> where T : IShape
    {
        public abstract IEnumerable<T> testCases { get; }

        [Test]
        [Category("Shapes")]
        public void NotEmpty()
        {
            foreach (T shape in testCases)
            {
                CollectionAssert.IsNotEmpty(shape, "Failed with " + shape);
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
        /// Tests that the <see cref="IReadOnlyCollection{T}.Count"/> property of the <see cref="IShape"/> matches the length of the enumerator.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public virtual void Count()
        {
            foreach (T shape in testCases)
            {
                IShapeTestHelper.Count(shape);
            }
        }

        [Test]
        [Category("Shapes")]
        public virtual void Contains()
        {
            foreach (T shape in testCases)
            {
                try
                {
                    IShapeTestHelper.Contains(shape);
                }
                catch (Exception e)
                {
                    throw new TestException("Exception with " + shape, e);
                }
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
                try
                {
                    IShapeTestHelper.NoRepeatsAtAll(shape);
                }
                catch (Exception e)
                {
                    throw new TestException("Exception with " + shape, e);
                }
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
                try
                {
                    IShapeTestHelper.Connected(shape);
                }
                catch (Exception e)
                {
                    throw new TestException($"Exception with {shape}", e);
                }
            }
        }
    }

    public abstract class IDeepCopyableShapeTests<T> : IShapeTests<T> where T : IDeepCopyableShape<T>
    {
        [Test]
        [Category("Shapes")]
        public virtual void DeepCopy()
        {
            foreach (T shape in testCases)
            {
                Assert.AreEqual(shape, shape.DeepCopy(), "Failed with " + shape);
                Assert.False(ReferenceEquals(shape, shape.DeepCopy()), "Failed with " + shape);
            }
        }
    }

    public abstract class ITranslatableShapeTests<T> : IDeepCopyableShapeTests<T> where T : ITranslatableShape<T>
    {
        [Test]
        [Category("Shapes")]
        public virtual void Translate()
        {
            foreach (T shape in testCases)
            {
                IShapeTestHelper.Translate(shape);
            }
        }
    }

    public abstract class ITransformableShapeTests<T> : ITranslatableShapeTests<T> where T : ITransformableShape<T>
    {
        [Test]
        [Category("Shapes")]
        public virtual void Flip()
        {
            foreach (T shape in testCases)
            {
                foreach (FlipAxis axis in new FlipAxis[] { FlipAxis.None, FlipAxis.Vertical, FlipAxis.Horizontal })
                {
                    IShapeTestHelper.Flip(shape, axis);
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public virtual void Rotate()
        {
            foreach (T shape in testCases)
            {
                foreach (RotationAngle angle in new RotationAngle[] { RotationAngle._0, RotationAngle._90, RotationAngle._180, RotationAngle.Minus90 })
                {
                    IRotatableShapeTestHelper.Rotate(shape, angle);
                }
            }
        }
    }

    public abstract class I1DShapeTests<T> : ITransformableShapeTests<T> where T : I1DShape<T>
    {

    }

    public abstract class IFillableShapeTests<T> : IShapeTests<T> where T : IFillableShape
    {
        /// <summary>
        /// Tests that the unfilled version of a shape is precisely the border of the filled version.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public virtual void UnfilledIsBorderOfFilled()
        {
            foreach (T shape in testCases)
            {
                IFillableShapeTestHelper.UnfilledIsBorderOfFilled(shape);
            }
        }
    }

    public abstract class I2DShapeTests<T> : ITransformableShapeTests<T> where T : I2DShape<T>
    {
        /// <summary>
        /// Tests that the unfilled version of a shape is precisely the border of the filled version.
        /// </summary>
        [Test]
        [Category("Shapes")]
        public virtual void UnfilledIsBorderOfFilled()
        {
            foreach (T shape in testCases)
            {
                IFillableShapeTestHelper.UnfilledIsBorderOfFilled(shape);
            }
        }
    }

    public abstract class IIsometricShapeTests<T> : IFillableShapeTests<T> where T : IIsometricShape<T>
    {
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
        public virtual void Flip()
        {
            foreach (T shape in testCases)
            {
                foreach (FlipAxis axis in new FlipAxis[] { FlipAxis.None, FlipAxis.Vertical, FlipAxis.Horizontal })
                {
                    IShapeTestHelper.Flip(shape, axis);
                }
            }
        }
    }

    /// <summary>
    /// Helper functions for making tests for I
    /// </summary>
    public static class IShapeTestHelper
    {
        /// <summary>
        /// Tests that the shape's boundingRect property is the correct bounding rect.
        /// </summary>
        public static void BoundingRect(IShape shape)
        {
            Assert.AreEqual(IntRect.BoundingRect(shape), shape.boundingRect, "Failed with " + shape);
        }

        /// <summary>
        /// Tests that the <see cref="IReadOnlyCollection{T}.Count"/> property of the <see cref="IShape"/> matches the length of the enumerator.
        /// </summary>
        public static void Count(IShape shape)
        {
            Assert.AreEqual(shape.Count(), shape.Count, "Failed with " + shape);
        }

        /// <summary>
        /// Tests that the shape's Contains() method matches the set of pixels obtained from the shape's enumerator.
        /// </summary>
        public static void Contains(IShape shape)
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
        public static void NoRepeatsAtAll(IShape shape)
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
        public static void NoIsolatedPoints(IShape shape)
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
        public static void Connected(IShape shape)
        {
            HashSet<IntVector2> pixels = shape.ToHashSet();
            HashSet<IntVector2> visited = new HashSet<IntVector2>();
            Queue<IntVector2> toVisit = new Queue<IntVector2>();

            IntVector2 startingPixel = pixels.First();
            toVisit.Enqueue(startingPixel);
            visited.Add(startingPixel);

            while (toVisit.Count > 0)
            {
                IntVector2 pixel = toVisit.Dequeue();
                foreach (IntVector2 offsetPixel in pixel + new IntRect(-IntVector2.one, IntVector2.one))
                {
                    if (pixels.Contains(offsetPixel) && !visited.Contains(offsetPixel))
                    {
                        toVisit.Enqueue(offsetPixel);
                        visited.Add(offsetPixel);

                        if (visited.Count == pixels.Count)
                        {
                            break;
                        }
                    }
                }
            }

            Assert.AreEqual(pixels.Count, visited.Count, $"Failed with {shape}");
        }

        /// <summary>
        /// Applies Translate() to the shape and checks that the enumerator of the resulting shape is a translation of the original shape's enumerator.
        /// </summary>
        public static void Translate<T>(ITranslatableShape<T> shape) where T : IShape
        {
            HashSet<IntVector2> original = shape.ToHashSet();
            foreach (IntVector2 translation in new IntRect(new IntVector2(-2, -2), new IntVector2(2, 2)))
            {
                HashSet<IntVector2> expected = original.Select(p => p + translation).ToHashSet();
                HashSet<IntVector2> translated = shape.Translate(translation).ToHashSet();
                
                Assert.True(expected.SetEquals(translated), $"Failed with {shape} and {translation}");
            }
        }

        /// <summary>
        /// Applies Flip() to the shape and checks that the enumerator of the resulting shape is a reflection of the original shape's enumerator.
        /// </summary>
        public static void Flip<T>(IFlippableShape<T> shape, FlipAxis axis) where T : IShape
        {
            HashSet<IntVector2> expected = shape.Select(p => p.Flip(axis)).ToHashSet();
            HashSet<IntVector2> flipped = shape.Flip(axis).ToHashSet();
            Assert.True(expected.SetEquals(flipped), $"Failed with {shape} and {axis}");
        }

        /// <summary>
        /// Tests that the shape has reflective symmetry across the given axis.
        /// </summary>
        public static void ReflectiveSymmetry(IShape shape, FlipAxis axis)
        {
            CollectionAssert.AreEquivalent(shape.ToHashSet(), shape.Select(p => p.Flip(axis) + shape.boundingRect.bottomLeft - shape.boundingRect.Flip(axis).bottomLeft).ToHashSet(),
                "Failed with " + shape + " and FlipAxis." + axis);
        }

        /// <summary>
        /// Tests that the shape doesn't have reflective symmetry across the given axis.
        /// </summary>
        public static void ReflectiveAsymmetry(IShape shape, FlipAxis axis)
        {
            CollectionAssert.AreNotEquivalent(shape.ToHashSet(), shape.Select(p => p.Flip(axis) + shape.boundingRect.bottomLeft - shape.boundingRect.Flip(axis).bottomLeft).ToHashSet(),
                "Failed with " + shape + " and FlipAxis." + axis);
        }

        /// <summary>
        /// Tests that the shape has 180-degree rotational symmetry.
        /// </summary>
        public static void RotationalSymmetry180<T>(IIsometricShape<T> shape) where T : IShape
        {
            CollectionAssert.AreEquivalent(shape.ToHashSet(), shape.Select(p =>
                p.Flip(FlipAxis.Vertical).Flip(FlipAxis.Horizontal) + shape.boundingRect.bottomLeft - shape.boundingRect.Flip(FlipAxis.Vertical).Flip(FlipAxis.Horizontal).bottomLeft
                ).ToHashSet(),
                "Failed with " + shape);
        }
    }

    /// <summary>
    /// Helper functions for making tests for IFillable
    /// </summary>
    public static class IFillableShapeTestHelper
    {
        public static HashSet<IntVector2> GetBorder(IFillableShape shape)
        {
            HashSet<IntVector2> pixels = shape.ToHashSet();
            HashSet<IntVector2> border = new HashSet<IntVector2>();
            foreach (IntVector2 pixel in pixels)
            {
                foreach (IntVector2 offset in IntVector2.upDownLeftRight)
                {
                    if (!pixels.Contains(pixel + offset))
                    {
                        border.Add(pixel);
                        break;
                    }
                }
            }
            return border;
        }

        /// <summary>
        /// Tests that the unfilled version of the shape is precisely the border of the filled version.
        /// </summary>
        public static void UnfilledIsBorderOfFilled(IFillableShape shape)
        {
            shape.filled = true;
            HashSet<IntVector2> borderOfFilled = GetBorder(shape);

            shape.filled = false;
            Assert.True(borderOfFilled.SetEquals(shape), $"Failed with {shape}");
        }
    }

    /// <summary>
    /// Helper functions for making tests for IRotatable
    /// </summary>
    public static class IRotatableShapeTestHelper
    {
        /// <summary>
        /// Applies Rotate() to the shape and checks that the enumerator of the resulting shape is a rotation of the original shape's enumerator.
        /// </summary>
        public static void Rotate<T>(IRotatableShape<T> shape, RotationAngle angle) where T : IShape
        {
            HashSet<IntVector2> expected = shape.Select(p => p.Rotate(angle)).ToHashSet();
            HashSet<IntVector2> rotated = shape.Rotate(angle).ToHashSet();
            Assert.True(expected.SetEquals(rotated), $"Failed with {shape} and {angle}");
        }

        /// <summary>
        /// Tests that the shape has rotational symmetry by the given angle.
        /// </summary>
        public static void RotationalSymmetry<T>(IRotatableShape<T> shape, RotationAngle angle) where T : IShape
        {
            CollectionAssert.AreEquivalent(shape.ToHashSet(), shape.Select(p => p.Rotate(angle) + shape.boundingRect.bottomLeft - shape.boundingRect.Rotate(angle).bottomLeft).ToHashSet(),
                "Failed with " + shape + " and RotationAngle." + angle);
        }
    }
}
