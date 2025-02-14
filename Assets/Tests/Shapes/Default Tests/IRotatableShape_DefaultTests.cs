using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using PAC.DataStructures;
using PAC.Shapes.Interfaces;
using PAC.Tests.Shapes.RequiredTests;
using PAC.Tests.Shapes.TestUtils;

namespace PAC.Tests.Shapes.DefaultTests
{
    /// <summary>
    /// Provides default implementations for the required tests in <see cref="IRotatableShape_RequiredTests"/>.
    /// </summary>
    public abstract class IRotatableShape_DefaultTests<T> : IDeepCopyableShape_DefaultTests<T>, IRotatableShape_RequiredTests where T : IRotatableShape<T>
    {
        [Test]
        [Category("Shapes")]
        public virtual void Rotate() => Rotate_Impl(testCases);
        internal static void Rotate_Impl(IEnumerable<T> testCases)
        {
            foreach (T shape in testCases)
            {
                foreach (RotationAngle angle in new RotationAngle[] { RotationAngle._0, RotationAngle._90, RotationAngle._180, RotationAngle.Minus90 })
                {
                    IEnumerable<IntVector2> expected = shape.Select(p => p.Rotate(angle));
                    IEnumerable<IntVector2> rotated = shape.Rotate(angle);
                    ShapeAssert.SameGeometry(expected, rotated, $"Failed with {shape} and {angle}.");
                }
            }
        }
    }
}
