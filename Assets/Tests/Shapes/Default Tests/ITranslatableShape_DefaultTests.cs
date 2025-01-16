using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using PAC.DataStructures;
using PAC.Shapes.Interfaces;
using PAC.Tests.Shapes.RequiredTests;

namespace PAC.Tests.Shapes.DefaultTests
{
    /// <summary>
    /// Provides default implementations for the required tests in <see cref="ITranslatableShape_RequiredTests"/>.
    /// </summary>
    public abstract class ITranslatableShape_DefaultTests<T> : IDeepCopyableShape_DefaultTests<T>, ITranslatableShape_RequiredTests where T : ITranslatableShape<T>
    {
        [Test]
        [Category("Shapes")]
        public virtual void Translate() => Translate_Impl(testCases);
        internal static void Translate_Impl(IEnumerable<T> testCases)
        {
            foreach (T shape in testCases)
            {
                HashSet<IntVector2> original = shape.ToHashSet();
                foreach (IntVector2 translation in new IntRect(new IntVector2(-2, -2), new IntVector2(2, 2)))
                {
                    HashSet<IntVector2> expected = original.Select(p => p + translation).ToHashSet();
                    HashSet<IntVector2> translated = shape.Translate(translation).ToHashSet();

                    Assert.True(expected.SetEquals(translated), $"Failed with {shape} and {translation}.");
                }
            }
        }
    }
}
