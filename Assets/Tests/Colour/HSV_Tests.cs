using System;

using NUnit.Framework;

using PAC.Colour;
using PAC.Extensions;

namespace PAC.Tests.Colour
{
    /// <summary>
    /// Tests for <see cref="HSV"/>.
    /// </summary>
    public class HSV_Tests
    {
        /// <summary>
        /// Tests converting <see cref="HSV"/> to <see cref="RGB"/> with some examples from <see href="https://en.wikipedia.org/wiki/HSL_and_HSV#Examples"/>.
        /// </summary>
        [Test]
        [Category("Colour")]
        public void CastTo_RGB_Examples()
        {
            (HSV input, RGB expected)[] testCases =
            {
                (new HSV(0f, 1f, 1f), new RGB(1f, 0f, 0f)),
                (new HSV(180f / 360f, 0.5f, 1f), new RGB(0.5f, 1f, 1f)),
                (new HSV(300f / 360f, 0.667f, 0.75f), new RGB(0.75f, 0.25f, 0.75f)),
                (new HSV(251.1f / 360f, 0.887f, 0.918f), new RGB(0.255f, 0.104f, 0.918f)),
                (new HSV(56.9f / 360f, 0.467f, 0.998f), new RGB(0.998f, 0.974f, 0.532f)),
            };

            foreach ((HSV input, RGB expected) in testCases)
            {
                RGB observed = (RGB)input;
                Assert.True(expected.Equals(observed, 0.001f), $"Failed with {input}.\nExpected: {expected}\nObserved: {observed}");
            }
        }

        /// <summary>
        /// Tests that <c>HSV(h, 0, v)</c> converts to <c>RGB(v, v, v)</c>.
        /// </summary>
        [Test]
        [Category("Colour")]
        public void CastTo_RGB_Saturation0()
        {
            Random random = new Random(TestContext.CurrentContext.Test.Name.GetHashCode());
            for (int iteration = 0; iteration < 1_000; iteration++)
            {
                float v = random.NextFloat();
                HSV input = new HSV(random.NextFloat(), 0f, v);
                RGB observed = (RGB)input;
                RGB expected = new RGB(v, v, v);
                Assert.True(expected.Equals(observed, 0.001f), $"Failed with {input}.\nExpected: {expected}\nObserved: {observed}");
            }
        }

        /// <summary>
        /// Tests that converting <see cref="HSV"/> to <see cref="RGB"/> and then back to <see cref="HSV"/> gives the original colour.
        /// </summary>
        [Test]
        [Category("Colour")]
        public void CastTo_RGB_AndBackIsIdentity()
        {
            Random random = new Random(TestContext.CurrentContext.Test.Name.GetHashCode());
            for (int iteration = 0; iteration < 1_000; iteration++)
            {
                HSV input = random.NextHSV();
                HSV observed = (HSV)(RGB)input;
                Assert.True(input.Equals(observed, 0.001f), $"Failed with {input}.\nExpected: {input}\nObserved: {observed}");
            }
        }

        /// <summary>
        /// Tests that converting <see cref="HSV"/> to <see cref="HSL"/> and then back to <see cref="HSV"/> gives the original colour.
        /// </summary>
        [Test]
        [Category("Colour")]
        public void CastTo_HSL_AndBackIsIdentity()
        {
            Random random = new Random(TestContext.CurrentContext.Test.Name.GetHashCode());
            for (int iteration = 0; iteration < 1_000; iteration++)
            {
                HSV input = random.NextHSV();
                HSV observed = (HSV)(HSL)input;
                Assert.True(input.Equals(observed, 0.001f), $"Failed with {input}.\nExpected: {input}\nObserved: {observed}");
            }
        }
    }
}
