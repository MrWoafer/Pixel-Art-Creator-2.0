using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using PAC.DataStructures;
using PAC.Geometry;
using PAC.Geometry.Shapes.Interfaces;
using PAC.Tests.Geometry.Shapes.RequiredTests;
using PAC.Tests.Geometry.Shapes.TestUtils;

namespace PAC.Tests.Geometry.Shapes.DefaultTests
{
    /// <summary>
    /// Provides default implementations for the required tests in <see cref="ITranslatableShape_RequiredTests"/>.
    /// </summary>
    public abstract class ITranslatableShape_DefaultTests<T> : IDeepCopyableShape_DefaultTests<T>, ITranslatableShape_RequiredTests where T : ITranslatableShape<T>
    {
        [Test]
        [Category("Shapes")]
        public virtual void Translated() => Translated_Impl(testCases);
        internal static void Translated_Impl(IEnumerable<T> testCases)
        {
            foreach (T shape in testCases)
            {
                foreach (IntVector2 translation in new IntRect(new IntVector2(-2, -2), new IntVector2(2, 2)))
                {
                    IEnumerable<IntVector2> expected = shape.Select(p => p + translation);
                    IEnumerable<IntVector2> translated = shape.Translated(translation);
                    ShapeAssert.SameGeometry(expected, translated, $"Failed with {shape} and {translation}.");
                }
            }
        }
    }
}
