using System;
using System.Collections.Generic;

using NUnit.Framework;

using PAC.DataStructures;
using PAC.Extensions;
using PAC.Geometry;

using UnityEngine;

namespace PAC.Tests.Patterns
{
    /// <summary>
    /// Tests for <see cref="PAC.Patterns.Gradient.Linear"/>.
    /// </summary>
    public class GradientLinear_Tests
    {
        private IEnumerable<PAC.Patterns.Gradient.Linear> testCases
        {
            get
            {
                System.Random random = new System.Random(0);
                foreach (IntVector2 startCoord in new IntRect((-5, -5), (5, 5)))
                {
                    foreach (IntVector2 endCoord in new IntRect((-5, -5), (5, 5)))
                    {
                        Color startColour = new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
                        Color endColour = new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());

                        yield return new PAC.Patterns.Gradient.Linear((startCoord, startColour), (endCoord, endColour));
                    }
                }
            }
        }

        /// <summary>
        /// Projects <paramref name="pointToProject"/> onto the real line through <paramref name="pointOnLine1"/> and <paramref name="pointOnLine2"/>.
        /// </summary>
        private Vector2 ProjectOntoLine(Vector2 pointOnLine1, Vector2 pointOnLine2, Vector2 pointToProject)
        {
            if (pointOnLine1 == pointOnLine2)
            {
                throw new ArgumentException($"{nameof(pointOnLine1)} and {nameof(pointOnLine2)} cannot be equal as then they don't define a unique line.");
            }

            Vector2 vectorOfLine = pointOnLine2 - pointOnLine1;
            // Using the formula in https://math.stackexchange.com/a/2839959
            return pointOnLine1 + (Vector2.Dot(pointToProject - pointOnLine1, vectorOfLine) / vectorOfLine.sqrMagnitude) * vectorOfLine;
        }

        /// <summary>
        /// Tests that <see cref="Patterns.Gradient.Linear.start"/> and <see cref="Patterns.Gradient.Linear.end"/> have the correct colour.
        /// </summary>
        [Test]
        [Category("Patterns")]
        public void ColourOfEndpoints()
        {
            foreach (PAC.Patterns.Gradient.Linear gradient in testCases)
            {
                Assert.AreEqual(gradient.start.colour, gradient[gradient.start.coord], $"Failed with {gradient}.");
                if (gradient.start.coord != gradient.end.coord)
                {
                    Assert.True(gradient.end.colour == gradient[gradient.end.coord], $"Failed with {gradient}.");
                }
            }
        }

        /// <summary>
        /// Tests that points that project onto the segment of the line between <see cref="Patterns.Gradient.Linear.start"/> and <see cref="Patterns.Gradient.Linear.end"/> have the correct colour.
        /// </summary>
        [Test]
        [Category("Patterns")]
        public void ColourBetweenEndpoints()
        {
            foreach (PAC.Patterns.Gradient.Linear gradient in testCases)
            {
                if (gradient.start.coord == gradient.end.coord)
                {
                    continue;
                }

                float lengthOfLineSegment = Vector2.Distance(gradient.start.coord, gradient.end.coord);
                Vector2 vectorOfLineSegment = gradient.end.coord - gradient.start.coord;

                IntRect testRegion = new IntRect(gradient.start.coord, gradient.end.coord);
                foreach (IntVector2 point in testRegion)
                {
                    Vector2 projectedPoint = ProjectOntoLine(gradient.start.coord, gradient.end.coord, point);
                    float distance = Vector2.Distance(projectedPoint, gradient.start.coord);

                    // Check if projectedPoint is on the line segment between start and end
                    if (distance <= lengthOfLineSegment && Mathf.Sign(Vector2.Dot(vectorOfLineSegment, projectedPoint - gradient.start.coord)) >= 0f)
                    {
                        Color expectedColour = Color.LerpUnclamped(gradient.start.colour, gradient.end.colour, distance / lengthOfLineSegment);
                        Assert.True(expectedColour.Equals(gradient[point], 0.0001f), $"Failed with {gradient} and {point}.");
                    }
                }
            }
        }

        /// <summary>
        /// Tests that points that project to outside the segment of the line between <see cref="Patterns.Gradient.Linear.start"/> and <see cref="Patterns.Gradient.Linear.end"/> have the colour of
        /// the closer endpoint.
        /// </summary>
        [Test]
        [Category("Patterns")]
        public void ColourBeyondEndpoints()
        {
            foreach (PAC.Patterns.Gradient.Linear gradient in testCases)
            {
                if (gradient.start.coord == gradient.end.coord)
                {
                    continue;
                }

                float lengthOfLineSegment = Vector2.Distance(gradient.start.coord, gradient.end.coord);
                Vector2 vectorOfLineSegment = gradient.end.coord - gradient.start.coord;

                IntRect testRegion = new IntRect(gradient.start.coord, gradient.end.coord);
                testRegion = new IntRect(testRegion.bottomLeft + 2 * IntVector2.downLeft, testRegion.topRight + 2 * IntVector2.upRight);
                foreach (IntVector2 point in testRegion)
                {
                    Vector2 projectedPoint = ProjectOntoLine(gradient.start.coord, gradient.end.coord, point);
                    float distance = Vector2.Distance(projectedPoint, gradient.start.coord);

                    // Check if projectedPoint is NOT on the line segment between start and end
                    if (distance > lengthOfLineSegment || Mathf.Sign(Vector2.Dot(vectorOfLineSegment, projectedPoint - gradient.start.coord)) < 0f)
                    {
                        Color expectedColour = distance < Vector2.Distance(projectedPoint, gradient.end.coord) ? gradient.start.colour : gradient.end.colour;
                        Assert.True(expectedColour.Equals(gradient[point], 0.0001f), $"Failed with {gradient} and {point}.");
                    }
                }
            }
        }

        /// <summary>
        /// Tests that if the endpoints have the same coord then the pattern assigns the start colour to every point.
        /// </summary>
        [Test]
        [Category("Patterns")]
        public void EndpointsEqual()
        {
            foreach (PAC.Patterns.Gradient.Linear gradient in testCases)
            {
                if (gradient.start.coord == gradient.end.coord)
                {
                    foreach (IntVector2 point in new IntRect((-5, -5), (5, 5)))
                    {
                        Assert.True(gradient.start.colour == gradient[point], $"Failed with {gradient} and {point}.");
                    }
                }
            }
        }

        /// <summary>
        /// Tests that, if the start and end coord are different, swapping them along with their colours doesn't change the pattern.
        /// </summary>
        [Test]
        [Category("Patterns")]
        public void OrderDoesNotMatter()
        {
            foreach (PAC.Patterns.Gradient.Linear gradient in testCases)
            {
                if (gradient.start.coord == gradient.end.coord)
                {
                    continue;
                }

                PAC.Patterns.Gradient.Linear gradientOrderSwapped = new PAC.Patterns.Gradient.Linear(gradient.end, gradient.start);

                IntRect testRegion = new IntRect(gradient.start.coord, gradient.end.coord);
                testRegion = new IntRect(testRegion.bottomLeft + IntVector2.downLeft, testRegion.topRight + IntVector2.upRight);
                foreach (IntVector2 point in testRegion)
                {
                    Assert.True(gradient[point].Equals(gradientOrderSwapped[point], 0.0001f), $"Failed with {gradient} and {point}.");
                }
            }
        }
    }
}
