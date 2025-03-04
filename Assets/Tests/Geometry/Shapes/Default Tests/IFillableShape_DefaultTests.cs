using System.Collections.Generic;

using NUnit.Framework;

using PAC.Geometry.Shapes.Interfaces;
using PAC.Tests.Geometry.Shapes.RequiredTests;
using PAC.Tests.Geometry.Shapes.TestUtils;

namespace PAC.Tests.Geometry.Shapes.DefaultTests
{
    /// <summary>
    /// Provides default implementations for the required tests in <see cref="IFillableShape_RequiredTests"/>.
    /// </summary>
    public abstract class IFillableShape_DefaultTests<T> : IShape_DefaultTests<T>, IFillableShape_RequiredTests where T : IFillableShape
    {
        [Test]
        [Category("Shapes")]
        public virtual void UnfilledIsBorderOfFilled() => UnfilledIsBorderOfFilled_Impl(testCases);
        internal static void UnfilledIsBorderOfFilled_Impl(IEnumerable<T> testCases)
        {
            foreach (T shape in testCases)
            {
                ShapeAssert.UnfilledIsBorderOfFilled(shape);
            }
        }
    }
}
