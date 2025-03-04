using NUnit.Framework;

using PAC.Geometry.Shapes.Interfaces;
using PAC.Tests.Geometry.Shapes.RequiredTests;

namespace PAC.Tests.Geometry.Shapes.DefaultTests
{
    /// <summary>
    /// Provides default implementations for the required tests in <see cref="IDeepCopyableShape_RequiredTests"/>.
    /// </summary>
    public abstract class IDeepCopyableShape_DefaultTests<T> : IShape_DefaultTests<T>, IDeepCopyableShape_RequiredTests where T : IDeepCopyableShape<T>
    {
        [Test]
        [Category("Shapes")]
        public virtual void DeepCopy()
        {
            foreach (T shape in testCases)
            {
                Assert.AreEqual(shape, shape.DeepCopy(), $"Failed with {shape}.");
                Assert.False(ReferenceEquals(shape, shape.DeepCopy()), $"Failed with {shape}.");
            }
        }
    }
}
