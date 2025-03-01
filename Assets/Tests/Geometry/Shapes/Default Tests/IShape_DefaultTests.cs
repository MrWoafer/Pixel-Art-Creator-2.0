using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using PAC.DataStructures;
using PAC.Exceptions;
using PAC.Geometry.Shapes.Interfaces;
using PAC.Tests.Geometry.Shapes.RequiredTests;
using PAC.Tests.Geometry.Shapes.TestUtils;

namespace PAC.Tests.Geometry.Shapes.DefaultTests
{
    /// <summary>
    /// Provides default implementations for (most of) the required tests in <see cref="IShape_RequiredTests"/>.
    /// </summary>
    public abstract class IShape_DefaultTests<T> : IShape_RequiredTests where T : IShape
    {
        /// <summary>
        /// The test cases to use for the default tests provided in this class.
        /// </summary>
        protected abstract IEnumerable<T> testCases { get; }

        [Test]
        [Category("Shapes")]
        public virtual void NotEmpty()
        {
            foreach (T shape in testCases)
            {
                CollectionAssert.IsNotEmpty(shape, $"Failed with {shape}.");
            }
        }

        [Test]
        [Category("Shapes")]
        public abstract void ShapeSinglePoint();

        [Test]
        [Category("Shapes")]
        public virtual void BoundingRect()
        {
            foreach (T shape in testCases)
            {
                Assert.AreEqual(IntRect.BoundingRect(shape), shape.boundingRect, $"Failed with {shape}.");
            }
        }

        [Test]
        [Category("Shapes")]
        public virtual void Count()
        {
            foreach (T shape in testCases)
            {
                Assert.AreEqual(Enumerable.Count(shape), shape.Count, $"Failed with {shape}.");
            }
        }

        [Test]
        [Category("Shapes")]
        public virtual void Contains()
        {
            foreach (T shape in testCases)
            {
                IntRect boundingRect = IntRect.BoundingRect(shape);
                IntRect testRegion = new IntRect(boundingRect.bottomLeft + IntVector2.downLeft, boundingRect.topRight + IntVector2.upRight);

                HashSet<IntVector2> points = Enumerable.ToHashSet(shape);

                foreach (IntVector2 point in testRegion)
                {
                    // Using Assert.True(x == y) seems to be faster than Assert.AreEqual(x, y)
                    Assert.True(points.Contains(point) == shape.Contains(point), $"Failed with {shape} and {point}. Expected {points.Contains(point)}.");
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public virtual void Connected()
        {
            foreach (T shape in testCases)
            {
                try
                {
                    ShapeAssert.Connected(shape);
                }
                catch (Exception e)
                {
                    throw new TestException($"Exception with {shape}:", e);
                }
            }
        }
    }
}
