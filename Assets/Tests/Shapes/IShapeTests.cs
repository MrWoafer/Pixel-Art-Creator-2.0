using PAC.Drawing;
using NUnit.Framework;
using PAC.DataStructures;
using System.Collections.Generic;
using System.Linq;
using System;

namespace PAC.Tests
{
    // I guess this should technically be IIShapeTests
    /// <summary>
    /// The tests that should be implemented for every shape.
    /// </summary>
    public interface IShapeTests
    {
        /// <summary>
        /// Tests that instances of the shape that are single points have the correct shape.
        /// </summary>
        public void ShapeSinglePoint();

        public void BoundingRect();

        public void Count();
        public void Contains();

        /// <summary>
        /// Tests that the shape's enumerator doesn't repeat any pixels it doesn't need to.
        /// </summary>
        public void NoRepeats();

        /// <summary>
        /// Tests that the shape of the shape is only determined by the width and height, not by the position.
        /// </summary>
        public void TranslationalInvariance();
        /// <summary>
        /// Tests that rotating the shape gives the same shape as creating one with the width/height swapped.
        /// </summary>
        public void RotationalInvariance();
        /// <summary>
        /// Tests that reflecting the shape gives the same shape as creating one with the corners reflected.
        /// </summary>
        public void ReflectiveInvariance();
    }

    public static class IShapeTestHelper
    {
        /// <summary>
        /// Tests that the shape's boundingRect property is the correct bounding rect.
        /// </summary>
        /// <param name="shape"></param>
        public static void BoundingRect(Shapes.IShape shape)
        {
            Assert.AreEqual(IntRect.BoundingRect(shape), shape.boundingRect, "Failed with " + shape);
        }

        /// <summary>
        /// Tests that the shape's Count property matches the number of distinct pixels in its enumerator.
        /// </summary>
        public static void Count(Shapes.IShape shape)
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
            IntRect testRegion = shape.boundingRect;
            testRegion.bottomLeft -= IntVector2.one;
            testRegion.topRight += IntVector2.one;

            HashSet<IntVector2> pixels = shape.ToHashSet();

            foreach (IntVector2 pixel in testRegion)
            {
                Assert.AreEqual(pixels.Contains(pixel), shape.Contains(pixel), "Failed with " + shape + " and " + pixel);
            }
        }

        /// <summary>
        /// Tests that no pixels are repeated at all in the shape's enuemrator.
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