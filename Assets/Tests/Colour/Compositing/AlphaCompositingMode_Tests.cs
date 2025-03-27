using NUnit.Framework;

using PAC.Colour;
using PAC.Colour.Compositing;
using PAC.Extensions;

using UnityEngine;

namespace PAC.Tests.Colour.Compositing
{
    /// <summary>
    /// Tests for <see cref="AlphaCompositing"/>.
    /// </summary>
    public class AlphaCompositingMode_Tests
    {
        [Test]
        [Category("Colour"), Category("Compositing")]
        public void SourceOver_Premultiplied_ReturnsSourceWhenSourceOpaque()
        {
            System.Random random = new System.Random(0);
            for (int iteration = 0; iteration < 1_000; iteration++)
            {
                Color source = random.NextRGB().WithAlpha(1f).Premultiplied();
                Color destination = random.NextColor().Premultiplied();

                Color composited = AlphaCompositing.Premultiplied.SourceOver(source, destination);
                Assert.True(source.Equals(composited, 0.001f), $"Failed with {nameof(source)} = {source}, {nameof(destination)} = {destination}.");
            }
        }
        [Test]
        [Category("Colour"), Category("Compositing")]
        public void SourceOver_Straight_ReturnsSourceWhenSourceOpaque()
        {
            System.Random random = new System.Random(0);
            for (int iteration = 0; iteration < 1_000; iteration++)
            {
                Color source = random.NextRGB().WithAlpha(1f);
                Color destination = random.NextColor();

                Color composited = AlphaCompositing.Straight.SourceOver(source, destination);
                Assert.True(source.Equals(composited, 0.001f), $"Failed with {nameof(source)} = {source}, {nameof(destination)} = {destination}.");
            }
        }

        [Test]
        [Category("Colour"), Category("Compositing")]
        public void SourceOver_Premultiplied_ReturnsSourceWhenDestinationTransparent()
        {
            System.Random random = new System.Random(0);
            for (int iteration = 0; iteration < 1_000; iteration++)
            {
                Color source = random.NextColor().Premultiplied();
                Color destination = random.NextRGB().WithAlpha(0f).Premultiplied();

                Color composited = AlphaCompositing.Premultiplied.SourceOver(source, destination);
                Assert.True(source.Equals(composited, 0.001f), $"Failed with {nameof(source)} = {source}, {nameof(destination)} = {destination}.");
            }
        }
        [Test]
        [Category("Colour"), Category("Compositing")]
        public void SourceOver_Straight_ReturnsSourceWhenDestinationTransparent()
        {
            System.Random random = new System.Random(0);
            for (int iteration = 0; iteration < 1_000; iteration++)
            {
                Color source = random.NextColor();
                Color destination = random.NextRGB().WithAlpha(0f);

                Color composited = AlphaCompositing.Straight.SourceOver(source, destination);
                Assert.True(source.Equals(composited, 0.001f), $"Failed with {nameof(source)} = {source}, {nameof(destination)} = {destination}.");
            }
        }

        /// <summary>
        /// Tests the example in <see href="https://www.w3.org/TR/compositing-1/#ex-transparent-over-opaque"/> with premultiplied alpha.
        /// </summary>
        [Test]
        [Category("Colour"), Category("Compositing")]
        public void SourceOver_Premultiplied_Example1()
        {
            Color source = new Color(0f, 0f, 0.5f, 0.5f);
            Color destination = new Color(1f, 0f, 0f, 1f);

            Color composited = AlphaCompositing.Premultiplied.SourceOver(source, destination);
            Color expected = new Color(0.5f, 0f, 0.5f, 1f);
            Assert.True(expected.Equals(composited, 0.001f), $"Expected {expected} but got {composited}.");
        }
        /// <summary>
        /// Tests the example in <see href="https://www.w3.org/TR/compositing-1/#ex-transparent-over-opaque"/> with straight alpha.
        /// </summary>
        [Test]
        [Category("Colour"), Category("Compositing")]
        public void SourceOver_Straight_Example1()
        {
            Color source = new Color(0f, 0f, 1f, 0.5f);
            Color destination = new Color(1f, 0f, 0f, 1f);

            Color composited = AlphaCompositing.Straight.SourceOver(source, destination);
            Color expected = new Color(0.5f, 0f, 0.5f, 1f);
            Assert.True(expected.Equals(composited, 0.001f), $"Expected {expected} but got {composited}.");
        }

        /// <summary>
        /// Tests the example in <see href="https://www.w3.org/TR/compositing-1/#ex-two-transparent"/> with premultiplied alpha.
        /// </summary>
        [Test]
        [Category("Colour"), Category("Compositing")]
        public void SourceOver_Premultiplied_Example2()
        {
            Color source = new Color(0f, 0f, 0.5f, 0.5f);
            Color destination = new Color(0.5f, 0f, 0f, 0.5f);

            Color composited = AlphaCompositing.Premultiplied.SourceOver(source, destination);
            Color expected = new Color(0.25f, 0f, 0.5f, 0.75f);
            Assert.True(expected.Equals(composited, 0.001f), $"Expected {expected} but got {composited}.");
        }
        /// <summary>
        /// Tests the example in <see href="https://www.w3.org/TR/compositing-1/#ex-two-transparent"/> with straight alpha.
        /// </summary>
        [Test]
        [Category("Colour"), Category("Compositing")]
        public void SourceOver_Straight_Example2()
        {
            Color source = new Color(0f, 0f, 1f, 0.5f);
            Color destination = new Color(1f, 0f, 0f, 0.5f);

            Color composited = AlphaCompositing.Straight.SourceOver(source, destination);
            Color expected = new Color(0.333f, 0f, 0.666f, 0.75f);
            Assert.True(expected.Equals(composited, 0.001f), $"Expected {expected} but got {composited}.");
        }
    }
}
