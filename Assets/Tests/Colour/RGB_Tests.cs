using System;

using NUnit.Framework;

using PAC.Colour;
using PAC.Extensions.System;

namespace PAC.Tests.Colour
{
    /// <summary>
    /// Tests for <see cref="RGB"/>.
    /// </summary>
    public class RGB_Tests
    {
        /// <summary>
        /// Tests converting <see cref="RGB"/> to <see cref="HSL"/> with some examples from <see href="https://en.wikipedia.org/wiki/HSL_and_HSV#Examples"/>.
        /// </summary>
        [Test]
        [Category("Colour")]
        public void CastTo_HSL_Examples()
        {
            (RGB input, HSL expected)[] testCases =
            {
                (new RGB(1f, 0f, 0f), new HSL(0f, 1f, 0.5f)),
                (new RGB(0f, 0.5f, 0f), new HSL(120f / 360f, 1f, 0.25f)),
                (new RGB(0.704f, 0.187f, 0.897f), new HSL(283.7f / 360f, 0.775f, 0.542f)),
                (new RGB(0.099f, 0.795f,  0.591f), new HSL(162.4f / 360f, 0.779f, 0.447f)),
                (new RGB(0.495f, 0.493f, 0.721f), new HSL(240.5f / 360f, 0.290f, 0.607f)),
            };

            foreach ((RGB input, HSL expected) in testCases)
            {
                HSL observed = (HSL)input;
                Assert.True(expected.Equals(observed, 0.001f), $"Failed with {input}.\nExpected: {expected}\nObserved: {observed}");
            }
        }

        /// <summary>
        /// Tests that <c>RGB(0, 0, 0)</c> converts to <c>HSL(*, 0, 0)</c>.
        /// </summary>
        [Test]
        [Category("Colour")]
        public void CastTo_HSL_Black()
        {
            RGB black = new RGB(0f, 0f, 0f);
            HSL observed = (HSL)black;
            HSL expected = new HSL(observed.h, 0f, 0f);
            Assert.True(expected.Equals(observed, 0.001f), $"Failed.\nExpected: {expected}\nObserved: {observed}");
        }

        /// <summary>
        /// Tests that <c>RGB(1, 1, 1)</c> converts to <c>HSL(*, *, 1)</c>.
        /// </summary>
        [Test]
        [Category("Colour")]
        public void CastTo_HSL_White()
        {
            RGB white = new RGB(1f, 1f, 1f);
            HSL observed = (HSL)white;
            HSL expected = new HSL(observed.h, observed.s, 1f);
            Assert.True(expected.Equals(observed, 0.001f), $"Failed.\nExpected: {expected}\nObserved: {observed}");
        }

        /// <summary>
        /// Tests that <c>RGB(x, x, x)</c> converts to <c>HSL(*, 0, x)</c>.
        /// </summary>
        [Test]
        [Category("Colour")]
        public void CastTo_HSL_Greyscale()
        {
            Random random = new Random(TestContext.CurrentContext.Test.Name.GetHashCode());
            for (int iteration = 0; iteration < 1_000; iteration++)
            {
                float x = random.NextFloat();
                RGB input = new RGB(x, x, x);
                HSL observed = (HSL)input;
                HSL expected = new HSL(observed.h, 0f, x);
                Assert.True(expected.Equals(observed, 0.001f), $"Failed with {input}.\nExpected: {expected}\nObserved: {observed}");
            }
        }

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
        /// Tests converting <see cref="RGB"/> to <see cref="HSV"/> with some examples from <see href="https://en.wikipedia.org/wiki/HSL_and_HSV#Examples"/>.
        /// </summary>
        [Test]
        [Category("Colour")]
        public void CastTo_HSV_Examples()
        {
            (RGB input, HSV expected)[] testCases =
            {
                (new RGB(1f, 0f, 0f), new HSV(0f, 1f, 1f)),
                (new RGB(0f, 0.5f, 0f), new HSV(120f / 360f, 1f, 0.5f)),
                (new RGB(0.704f, 0.187f, 0.897f), new HSV(283.7f / 360f, 0.792f, 0.897f)),
                (new RGB(0.099f, 0.795f,  0.591f), new HSV(162.4f / 360f, 0.875f, 0.795f)),
                (new RGB(0.495f, 0.493f, 0.721f), new HSV(240.5f / 360f, 0.316f, 0.721f)),
            };

            foreach ((RGB input, HSV expected) in testCases)
            {
                HSV observed = (HSV)input;
                Assert.True(expected.Equals(observed, 0.001f), $"Failed with {input}.\nExpected: {expected}\nObserved: {observed}");
            }
        }

        /// <summary>
        /// Tests that <c>RGB(0, 0, 0)</c> converts to <c>HSV(*, *, 0)</c>.
        /// </summary>
        [Test]
        [Category("Colour")]
        public void CastTo_HSV_Black()
        {
            RGB black = new RGB(0f, 0f, 0f);
            HSV observed = (HSV)black;
            HSV expected = new HSV(observed.h, observed.s, 0f);
            Assert.True(expected.Equals(observed, 0.001f), $"Failed.\nExpected: {expected}\nObserved: {observed}");
        }

        /// <summary>
        /// Tests that <c>RGB(1, 1, 1)</c> converts to <c>HSV(*, 0, 1)</c>.
        /// </summary>
        [Test]
        [Category("Colour")]
        public void CastTo_HSV_White()
        {
            RGB white = new RGB(1f, 1f, 1f);
            HSV observed = (HSV)white;
            HSV expected = new HSV(observed.h, 0f, 1f);
            Assert.True(expected.Equals(observed, 0.001f), $"Failed.\nExpected: {expected}\nObserved: {observed}");
        }

        /// <summary>
        /// Tests that <c>RGB(x, x, x)</c> converts to <c>HSV(*, 0, x)</c>.
        /// </summary>
        [Test]
        [Category("Colour")]
        public void CastTo_HSV_Greyscale()
        {
            Random random = new Random(TestContext.CurrentContext.Test.Name.GetHashCode());
            for (int iteration = 0; iteration < 1_000; iteration++)
            {
                float x = random.NextFloat();
                RGB input = new RGB(x, x, x);
                HSV observed = (HSV)input;
                HSV expected = new HSV(observed.h, 0f, x);
                Assert.True(expected.Equals(observed, 0.001f), $"Failed with {input}.\nExpected: {expected}\nObserved: {observed}");
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
