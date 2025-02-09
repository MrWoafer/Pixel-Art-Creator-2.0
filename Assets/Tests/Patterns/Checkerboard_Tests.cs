using NUnit.Framework;

using PAC.DataStructures;
using PAC.Patterns;

using UnityEngine;

namespace PAC.Tests.Patterns
{
    /// <summary>
    /// Tests for <see cref="Checkerboard{T}"/>.
    /// </summary>
    public class Checkerboard_Tests
    {
        /// <summary>
        /// Tests that <see cref="Checkerboard{T}.this[IntVector2]"/> works correctly.
        /// </summary>
        [Test]
        [Category("Patterns")]
        public void Values()
        {
            Color[] colours = new Color[] { Color.red, Color.gray, Color.black, Color.white, Color.black, new Color(0.5f, 0.25f, 0.125f, 0.375f) };
            System.Random random = new System.Random(0);
            foreach (Color colourOfOrigin in colours)
            {
                foreach (Color otherColour in colours)
                {
                    Checkerboard<Color> checkerboard = new Checkerboard<Color>(colourOfOrigin, otherColour);

                    Assert.AreEqual(colourOfOrigin, checkerboard[(0, 0)], $"Failed with {checkerboard}.");
                    Assert.AreEqual(otherColour, checkerboard[(1, 0)], $"Failed with {checkerboard}.");
                    Assert.AreEqual(otherColour, checkerboard[(-1, 0)], $"Failed with {checkerboard}.");
                    Assert.AreEqual(otherColour, checkerboard[(0, 1)], $"Failed with {checkerboard}.");
                    Assert.AreEqual(otherColour, checkerboard[(0, -1)], $"Failed with {checkerboard}.");
                    Assert.AreEqual(colourOfOrigin, checkerboard[(1, 1)], $"Failed with {checkerboard}.");
                    Assert.AreEqual(colourOfOrigin, checkerboard[(1, -1)], $"Failed with {checkerboard}.");
                    Assert.AreEqual(colourOfOrigin, checkerboard[(-1, 1)], $"Failed with {checkerboard}.");
                    Assert.AreEqual(colourOfOrigin, checkerboard[(-1, -1)], $"Failed with {checkerboard}.");

                    const int points = 100;
                    for (int i = 0; i < points; i++)
                    {
                        IntVector2 point = new IntRect(new IntVector2(-100, -100), new IntVector2(100, 100)).RandomPoint(random);
                        Color colour = checkerboard[point];

                        if (colour == colourOfOrigin)
                        {
                            foreach (IntVector2 offset in IntVector2.upDownLeftRight)
                            {
                                Assert.AreEqual(otherColour, checkerboard[point + offset], $"Failed with {checkerboard}.");
                            }
                        }
                        else if (colour == otherColour)
                        {
                            foreach (IntVector2 offset in IntVector2.upDownLeftRight)
                            {
                                Assert.AreEqual(colourOfOrigin, checkerboard[point + offset], $"Failed with {checkerboard}.");
                            }
                        }
                        else
                        {
                            Assert.Fail($"Failed with {checkerboard} and {point}.");
                        }
                    }
                }
            }
        }
    }
}
