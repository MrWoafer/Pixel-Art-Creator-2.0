using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using PAC.DataStructures;
using PAC.Geometry.Shapes.Interfaces;
using PAC.Tests.Geometry.Shapes.RequiredTests;
using PAC.Tests.Geometry.Shapes.TestUtils;

namespace PAC.Tests.Geometry.Shapes.DefaultTests
{
    /// <summary>
    /// Provides default implementations for the required tests in <see cref="IFlippableShape_RequiredTests"/>.
    /// </summary>
    public abstract class IFlippableShape_DefaultTests<T> : IDeepCopyableShape_DefaultTests<T>, IFlippableShape_RequiredTests where T : IFlippableShape<T>
    {
        [Test]
        [Category("Shapes")]
        public virtual void Flip() => Flip_Impl(testCases);
        internal static void Flip_Impl(IEnumerable<T> testCases)
        {
            foreach (T shape in testCases)
            {
                foreach (FlipAxis axis in new FlipAxis[] { FlipAxis.None, FlipAxis.Vertical, FlipAxis.Horizontal })
                {
                    IEnumerable<IntVector2> expected = shape.Select(p => p.Flip(axis));
                    IEnumerable<IntVector2> flipped = shape.Flip(axis);
                    ShapeAssert.SameGeometry(expected, flipped, $"Failed with {shape} and {axis}.");
                }
            }
        }
    }
}
