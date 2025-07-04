using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using PAC.Geometry;
using PAC.Geometry.Shapes.Interfaces;
using PAC.Tests.Geometry.Shapes.RequiredTests;
using PAC.Tests.Geometry.Shapes.TestUtils;

namespace PAC.Tests.Geometry.Shapes.DefaultTests
{
    /// <summary>
    /// Provides default implementations for the required tests in <see cref="IRotatableShape_RequiredTests"/>.
    /// </summary>
    public abstract class IRotatableShape_DefaultTests<T> : IDeepCopyableShape_DefaultTests<T>, IRotatableShape_RequiredTests where T : IRotatableShape<T>
    {
        [Test]
        [Category("Shapes")]
        public virtual void Rotated() => Rotated_Impl(testCases);
        internal static void Rotated_Impl(IEnumerable<T> testCases)
        {
            foreach (T shape in testCases)
            {
                foreach (QuadrantalAngle angle in new QuadrantalAngle[] { QuadrantalAngle._0, QuadrantalAngle.Clockwise90, QuadrantalAngle._180, QuadrantalAngle.Anticlockwise90 })
                {
                    IEnumerable<IntVector2> expected = shape.Select(p => p.Rotate(angle));
                    IEnumerable<IntVector2> rotated = shape.Rotated(angle);
                    Assert.False(ReferenceEquals(shape, rotated), $"Failed with {shape} and {angle}.");
                    ShapeAssert.SameGeometry(expected, rotated, $"Failed with {shape} and {angle}.");
                }
            }
        }
    }
}
