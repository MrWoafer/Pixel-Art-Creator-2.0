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
        /// Tests that <c>HSV(*, *, 0)</c> converts to <c>RGB(0, 0, 0)</c>.
        /// </summary>
        [Test]
        [Category("Colour")]
        public void CastTo_RGB_Black()
        {
            Random random = new Random(TestContext.CurrentContext.Test.Name.GetHashCode());
            for (int iteration = 0; iteration < 1_000; iteration++)
            {
                HSV black = new HSV(random.NextFloat(), random.NextFloat(), 0f);
                RGB observed = (RGB)black;
                RGB expected = new RGB(0f, 0f, 0f);
                Assert.True(expected.Equals(observed, 0.001f), $"Failed.\nExpected: {expected}\nObserved: {observed}");
            }
        }

        /// <summary>
        /// Tests that <c>HSV(*, 0, 1)</c> converts to <c>RGB(1, 1, 1)</c>.
        /// </summary>
        [Test]
        [Category("Colour")]
        public void CastTo_RGB_White()
        {
            Random random = new Random(TestContext.CurrentContext.Test.Name.GetHashCode());
            for (int iteration = 0; iteration < 1_000; iteration++)
            {
                HSV white = new HSV(random.NextFloat(), 0f, 1f);
                RGB observed = (RGB)white;
                RGB expected = new RGB(1f, 1f, 1f);
                Assert.True(expected.Equals(observed, 0.001f), $"Failed.\nExpected: {expected}\nObserved: {observed}");
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
        /// Tests that <c>HSV(h, s, 0)</c> converts to <c>RGB(0, 0, 0)</c>.
        /// </summary>
        [Test]
        [Category("Colour")]
        public void Cast_RGB_Value0()
        {
            Random random = new Random(TestContext.CurrentContext.Test.Name.GetHashCode());
            for (int iteration = 0; iteration < 1_000; iteration++)
            {
                HSV input = new HSV(random.NextFloat(), random.NextFloat(), 0f);
                RGB observed = (RGB)input;
                RGB expected = new RGB(0f, 0f, 0f);
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
        /// Tests converting <see cref="HSV"/> to <see cref="HSL"/> with some examples from <see href="https://en.wikipedia.org/wiki/HSL_and_HSV#Examples"/>.
        /// </summary>
        [Test]
        [Category("Colour")]
        public void CastTo_HSL_Examples()
        {
            (HSV input, HSL expected)[] testCases =
            {
                (new HSV(0f, 1f, 1f), new HSL(0f, 1f, 0.5f)),
                (new HSV(300f / 360f, 0.667f, 0.75f), new HSL(300f / 360f, 0.5f, 0.5f)),
                (new HSV(61.8f / 360f, 0.779f, 0.643f), new HSL(61.8f / 360f, 0.638f, 0.393f)),
                (new HSV(251.1f / 360f, 0.887f, 0.918f), new HSL(251.1f / 360f, 0.832f, 0.511f)),
                (new HSV(134.9f / 360f, 0.828f, 0.675f), new HSL(134.9f / 360f, 0.707f, 0.396f)),
            };

            foreach ((HSV input, HSL expected) in testCases)
            {
                HSL observed = (HSL)input;
                Assert.True(expected.Equals(observed, 0.001f), $"Failed with {input}.\nExpected: {expected}\nObserved: {observed}");
            }
        }

        /// <summary>
        /// Tests that <c>HSV(*, *, 0)</c> converts to <c>HSL(*, *, 0)</c>.
        /// </summary>
        [Test]
        [Category("Colour")]
        public void CastTo_HSL_Black()
        {
            Random random = new Random(TestContext.CurrentContext.Test.Name.GetHashCode());
            for (int iteration = 0; iteration < 1_000; iteration++)
            {
                HSV black = new HSV(random.NextFloat(), random.NextFloat(), 0f);
                HSL observed = (HSL)black;
                HSL expected = new HSL(observed.h, observed.s, 0f);
                Assert.True(expected.Equals(observed, 0.001f), $"Failed.\nExpected: {expected}\nObserved: {observed}");
            }
        }

        /// <summary>
        /// Tests that <c>HSV(*, 0, 1)</c> converts to <c>HSL(*, *, 1)</c>.
        /// </summary>
        [Test]
        [Category("Colour")]
        public void CastTo_HSL_White()
        {
            Random random = new Random(TestContext.CurrentContext.Test.Name.GetHashCode());
            for (int iteration = 0; iteration < 1_000; iteration++)
            {
                HSV white = new HSV(random.NextFloat(), 0f, 1f);
                HSL observed = (HSL)white;
                HSL expected = new HSL(observed.h, observed.s, 1f);
                Assert.True(expected.Equals(observed, 0.001f), $"Failed.\nExpected: {expected}\nObserved: {observed}");
            }
        }

        /// <summary>
        /// Tests that <c>HSV(h, *, *)</c> converts to <c>HSL(h, *, *)</c>.
        /// </summary>
        [Test]
        [Category("Colour")]
        public void Cast_HSL_HueUnchanged()
        {
            Random random = new Random(TestContext.CurrentContext.Test.Name.GetHashCode());
            for (int iteration = 0; iteration < 1_000; iteration++)
            {
                HSV input = random.NextHSV();
                HSL observed = (HSL)input;
                Assert.AreEqual(input.h, observed.h, 0.001f, $"Failed with {input}.");
            }
        }

        /// <summary>
        /// Tests that <c>HSV(h, 0, v)</c> converts to <c>HSL(h, 0, v)</c>.
        /// </summary>
        [Test]
        [Category("Colour")]
        public void Cast_HSL_Saturation0()
        {
            Random random = new Random(TestContext.CurrentContext.Test.Name.GetHashCode());
            for (int iteration = 0; iteration < 1_000; iteration++)
            {
                float h = random.NextFloat();
                float v = random.NextFloat();
                HSV input = new HSV(h, 0f, v);
                HSL observed = (HSL)input;
                HSL expected = new HSL(h, 0f, v);
                Assert.True(expected.Equals(observed, 0.001f), $"Failed with {input}.\nExpected: {expected}\nObserved: {observed}");
            }
        }

        /// <summary>
        /// Tests that <c>HSV(h, 1, 1)</c> converts to <c>HSL(h, 1, 0.5)</c>.
        /// </summary>
        [Test]
        [Category("Colour")]
        public void Cast_HSL_Saturation1Lightness1()
        {
            Random random = new Random(TestContext.CurrentContext.Test.Name.GetHashCode());
            for (int iteration = 0; iteration < 1_000; iteration++)
            {
                float h = random.NextFloat();
                HSV input = new HSV(h, 1f, 1f);
                HSL observed = (HSL)input;
                HSL expected = new HSL(h, 1f, 0.5f);
                Assert.True(expected.Equals(observed, 0.001f), $"Failed with {input}.\nExpected: {expected}\nObserved: {observed}");
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
