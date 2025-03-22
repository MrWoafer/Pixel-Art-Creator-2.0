using System;

using NUnit.Framework;

using PAC.Colour;
using PAC.Extensions;

namespace PAC.Tests.Colour
{
    /// <summary>
    /// Tests for <see cref="HSL"/>.
    /// </summary>
    public class HSL_Tests
    {
        /// <summary>
        /// Tests converting <see cref="HSL"/> to <see cref="RGB"/> with some examples from <see href="https://en.wikipedia.org/wiki/HSL_and_HSV#Examples"/>.
        /// </summary>
        [Test]
        [Category("Colour")]
        public void Cast_RGB_Examples()
        {
            (HSL input, RGB expected)[] testCases =
            {
                (new HSL(0f, 1f, 0.5f), new RGB(1f, 0f, 0f)),
                (new HSL(180f / 360f, 1f, 0.75f), new RGB(0.5f, 1f, 1f)),
                (new HSL(300f / 360f, 0.5f, 0.5f), new RGB(0.75f, 0.25f, 0.75f)),
                (new HSL(251.1f / 360f, 0.832f, 0.511f), new RGB(0.255f, 0.104f, 0.918f)),
                (new HSL(56.9f / 360f, 0.991f, 0.765f), new RGB(0.998f, 0.974f, 0.532f)),
            };

            foreach ((HSL input, RGB expected) in testCases)
            {
                RGB observed = (RGB)input;
                Assert.True(expected.Equals(observed, 0.001f), $"Failed with {input}.\nExpected: {expected}\nObserved: {observed}");
            }
        }

        /// <summary>
        /// Tests that <c>HSL(h, 0, l)</c> converts to <c>RGB(l, l, l)</c>.
        /// </summary>
        [Test]
        [Category("Colour")]
        public void Cast_RGB_Saturation0()
        {
            Random random = new Random(TestContext.CurrentContext.Test.Name.GetHashCode());
            for (int iteration = 0; iteration < 1_000; iteration++)
            {
                float l = random.NextFloat();
                HSL input = new HSL(random.NextFloat(), 0f, l);
                RGB observed = (RGB)input;
                RGB expected = new RGB(l, l, l);
                Assert.True(expected.Equals(observed, 0.001f), $"Failed with {input}.\nExpected: {expected}\nObserved: {observed}");
            }
        }

        /// <summary>
        /// Tests that <c>HSL(h, s, 0)</c> converts to <c>RGB(0, 0, 0)</c>.
        /// </summary>
        [Test]
        [Category("Colour")]
        public void Cast_RGB_Lightness0()
        {
            Random random = new Random(TestContext.CurrentContext.Test.Name.GetHashCode());
            for (int iteration = 0; iteration < 1_000; iteration++)
            {
                HSL input = new HSL(random.NextFloat(), random.NextFloat(), 0f);
                RGB observed = (RGB)input;
                RGB expected = new RGB(0f, 0f, 0f);
                Assert.True(expected.Equals(observed, 0.001f), $"Failed with {input}.\nExpected: {expected}\nObserved: {observed}");
            }
        }

        /// <summary>
        /// Tests that <c>HSL(h, s, 1)</c> converts to <c>RGB(1, 1, 1)</c>.
        /// </summary>
        [Test]
        [Category("Colour")]
        public void Cast_RGB_Lightness1()
        {
            Random random = new Random(TestContext.CurrentContext.Test.Name.GetHashCode());
            for (int iteration = 0; iteration < 1_000; iteration++)
            {
                HSL input = new HSL(random.NextFloat(), random.NextFloat(), 1f);
                RGB observed = (RGB)input;
                RGB expected = new RGB(1f, 1f, 1f);
                Assert.True(expected.Equals(observed, 0.001f), $"Failed with {input}.\nExpected: {expected}\nObserved: {observed}");
            }
        }

        /// <summary>
        /// Tests that converting <see cref="HSL"/> to <see cref="RGB"/> and then back to <see cref="HSL"/> gives the original colour.
        /// </summary>
        [Test]
        [Category("Colour")]
        public void CastTo_RGB_AndBackIsIdentity()
        {
            Random random = new Random(TestContext.CurrentContext.Test.Name.GetHashCode());
            for (int iteration = 0; iteration < 1_000; iteration++)
            {
                HSL input = random.NextHSL();
                HSL observed = (HSL)(RGB)input;
                Assert.True(input.Equals(observed, 0.001f), $"Failed with {input}.\nExpected: {input}\nObserved: {observed}");
            }
        }

        /// <summary>
        /// Tests that converting <see cref="HSL"/> to <see cref="HSV"/> and then back to <see cref="HSL"/> gives the original colour.
        /// </summary>
        [Test]
        [Category("Colour")]
        public void CastTo_HSV_AndBackIsIdentity()
        {
            Random random = new Random(TestContext.CurrentContext.Test.Name.GetHashCode());
            for (int iteration = 0; iteration < 1_000; iteration++)
            {
                HSL input = random.NextHSL();
                HSL observed = (HSL)(HSV)input;
                Assert.True(input.Equals(observed, 0.001f), $"Failed with {input}.\nExpected: {input}\nObserved: {observed}");
            }
        }
    }
}
