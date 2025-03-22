using System;

using NUnit.Framework;

using PAC.Colour;

namespace PAC.Tests.Colour
{
    /// <summary>
    /// Tests for <see cref="RGB"/>.
    /// </summary>
    public class RGB_Tests
    {
        /// <summary>
        /// Tests that converting <see cref="RGB"/> to <see cref="HSL"/> and then back to <see cref="RGB"/> gives the original colour.
        /// </summary>
        [Test]
        [Category("Colour")]
        public void CastTo_HSL_AndBackIsIdentity()
        {
            Random random = new Random(TestContext.CurrentContext.Test.Name.GetHashCode());
            for (int iteration = 0; iteration < 1_000; iteration++)
            {
                RGB input = random.NextRGB();
                RGB observed = (RGB)(HSL)input;
                Assert.True(input.Equals(observed, 0.001f), $"Failed with {input}.\nExpected: {input}\nObserved: {observed}");
            }
        }

        /// <summary>
        /// Tests that converting <see cref="RGB"/> to <see cref="HSV"/> and then back to <see cref="RGB"/> gives the original colour.
        /// </summary>
        [Test]
        [Category("Colour")]
        public void CastTo_HSV_AndBackIsIdentity()
        {
            Random random = new Random(TestContext.CurrentContext.Test.Name.GetHashCode());
            for (int iteration = 0; iteration < 1_000; iteration++)
            {
                RGB input = random.NextRGB();
                RGB observed = (RGB)(HSV)input;
                Assert.True(input.Equals(observed, 0.001f), $"Failed with {input}.\nExpected: {input}\nObserved: {observed}");
            }
        }
    }
}
