using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using PAC.Exceptions;
using PAC.Geometry;
using PAC.Geometry.Axes;
using PAC.Geometry.Shapes.Interfaces;
using PAC.Tests.Geometry.Shapes.RequiredTests;
using PAC.Tests.Geometry.Shapes.TestUtils;

namespace PAC.Tests.Geometry.Shapes.DefaultTests
{
    /// <summary>
    /// Provides default implementations for the required tests in <see cref="IFlippableShape_RequiredTests"/>.
    /// </summary>
    public abstract class IFlippableShape_DefaultTests<T, A> : IDeepCopyableShape_DefaultTests<T>, IFlippableShape_RequiredTests
        where T : IFlippableShape<T, A>
        where A : CardinalOrdinalAxis
    {
        private static IEnumerable<A> axes
        {
            get
            {
                if (typeof(A) == typeof(VerticalAxis))
                {
                    return (IEnumerable<A>)new CardinalOrdinalAxis[] { Axes.Vertical };
                }
                else if (typeof(A) == typeof(HorizontalAxis))
                {
                    return (IEnumerable<A>)new CardinalOrdinalAxis[] { Axes.Horizontal };
                }
                else if (typeof(A) == typeof(Diagonal45Axis))
                {
                    return (IEnumerable<A>)new CardinalOrdinalAxis[] { Axes.Diagonal45 };
                }
                else if (typeof(A) == typeof(Minus45Axis))
                {
                    return (IEnumerable<A>)new CardinalOrdinalAxis[] { Axes.Minus45 };
                }
                else if (typeof(A) == typeof(CardinalAxis))
                {
                    return (IEnumerable<A>)Axes.CardinalAxes;
                }
                else if (typeof(A) == typeof(OrdinalAxis))
                {
                    return (IEnumerable<A>)Axes.OrdinalAxes;
                }
                else if (typeof(A) == typeof(CardinalOrdinalAxis))
                {
                    return (IEnumerable<A>)Axes.CardinalOrdinalAxes;
                }
                else
                {
                    throw new UnreachableException();
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public virtual void Flipped() => Flipped_Impl(testCases);
        internal static void Flipped_Impl(IEnumerable<T> testCases)
        {
            foreach (T shape in testCases)
            {
                foreach (A axis in axes)
                {
                    IEnumerable<IntVector2> expected = shape.Select(p => p.Flip(axis));
                    IEnumerable<IntVector2> flipped = shape.Flipped(axis);
                    Assert.False(ReferenceEquals(shape, flipped), $"Failed with {shape} and {axis}.");
                    ShapeAssert.SameGeometry(expected, flipped, $"Failed with {shape} and {axis}.");
                }
            }
        }

        [Test]
        [Category("Shapes")]
        public virtual void FlipMatchesFlipped() => FlipMatchesFlipped_Impl(testCases);
        internal static void FlipMatchesFlipped_Impl(IEnumerable<T> testCases)
        {
            foreach (T shape in testCases)
            {
                foreach (A axis in axes)
                {
                    IEnumerable<IntVector2> flipped = shape.Flipped(axis);
                    shape.Flip(axis);
                    Assert.False(ReferenceEquals(shape, flipped), $"Failed with {shape} and {axis}.");
                    Assert.AreEqual(shape, flipped, $"Failed with {shape} and {axis}.");
                }
            }
        }
    }
}
