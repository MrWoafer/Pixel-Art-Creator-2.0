using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using PAC.DataStructures;
using PAC.Exceptions;
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
        [Test]
        [Category("Shapes")]
        public virtual void Flip() => Flip_Impl(testCases);
        internal static void Flip_Impl(IEnumerable<T> testCases)
        {
            IEnumerable<A> axes;
            if (typeof(A) == typeof(VerticalAxis))
            {
                axes = (IEnumerable<A>)new CardinalOrdinalAxis[] { CardinalAxis.Vertical };
            }
            else if (typeof(A) == typeof(HorizontalAxis))
            {
                axes = (IEnumerable<A>)new CardinalOrdinalAxis[] { CardinalAxis.Horizontal };
            }
            else if (typeof(A) == typeof(Diagonal45Axis))
            {
                axes = (IEnumerable<A>)new CardinalOrdinalAxis[] { OrdinalAxis.Diagonal45 };
            }
            else if (typeof(A) == typeof(Minus45Axis))
            {
                axes = (IEnumerable<A>)new CardinalOrdinalAxis[] { OrdinalAxis.Minus45 };
            }
            else if (typeof(A) == typeof(CardinalAxis))
            {
                axes = (IEnumerable<A>)CardinalAxis.Axes;
            }
            else if (typeof(A) == typeof(OrdinalAxis))
            {
                axes = (IEnumerable<A>)OrdinalAxis.Axes;
            }
            else if (typeof(A) == typeof(CardinalOrdinalAxis))
            {
                axes = (IEnumerable<A>)CardinalOrdinalAxis.Axes;
            }
            else
            {
                throw new UnreachableException();
            }

            foreach (T shape in testCases)
            {
                foreach (A axis in axes)
                {
                    IEnumerable<IntVector2> expected = shape.Select(p => p.Flip(axis));
                    IEnumerable<IntVector2> flipped = shape.Flip(axis);
                    ShapeAssert.SameGeometry(expected, flipped, $"Failed with {shape} and {axis}.");
                }
            }
        }
    }
}
