using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using PAC.DataStructures;
using PAC.Shapes.Interfaces;
using PAC.Tests.Shapes.RequiredTests;

namespace PAC.Tests.Shapes.DefaultTests
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
                    HashSet<IntVector2> expected = shape.Select(p => p.Flip(axis)).ToHashSet();
                    HashSet<IntVector2> flipped = shape.Flip(axis).ToHashSet();
                    Assert.True(expected.SetEquals(flipped), $"Failed with {shape} and {axis}");
                }
            }
        }
    }
}
