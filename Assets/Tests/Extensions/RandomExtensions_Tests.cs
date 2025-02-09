using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using PAC.Extensions;

namespace PAC.Tests.Extensions
{
    /// <summary>
    /// Tests for <see cref="RandomExtensions"/>.
    /// </summary>
    public class RandomExtensions_Tests
    {
        [Test]
        [Category("Extensions"), Category("Random")]
        public void ToSequence()
        {
            const int seed = 0;
            Random rng = new Random(seed);
            // We compare with a new System.Random with the same seed, instead of just against rng, because ToSequence() does not deep-copy the System.Random, so calling rng.Next() between the
            // generation of two elements would affect the sequence.
            Random rngCopy = new Random(seed);

            IEnumerable<int> sequence = rng.ToSequence();

            bool isEmpty = true;
            foreach (int i in sequence.Take(100))
            {
                isEmpty = false;
                Assert.AreEqual(rngCopy.Next(), i);
            }
            Assert.False(isEmpty);

            /////
            
            rng = new Random(seed);
            rngCopy = new Random(seed);

            const int min = -12;
            const int max = 19;
            sequence = rng.ToSequence(min, max);

            isEmpty = true;
            foreach (int i in sequence.Take(100))
            {
                isEmpty = false;
                Assert.AreEqual(rngCopy.Next(min, max), i);
            }
            Assert.False(isEmpty);

            // Can't have maxValue < minValue
            // I'm doing the ToArray() because currently, due to using yield, the input validation only happens when you generate the first element 
            Assert.Throws<ArgumentOutOfRangeException>(() => rng.ToSequence(10, 5).Take(1).ToArray());
        }

        /// <summary>
        /// Tests <see cref="RandomExtensions.NextBool(Random)"/>.
        /// </summary>
        [Test]
        [Category("Extensions"), Category("Random")]
        public void NextBool()
        {
            for (int seed = 0; seed <= 2; seed++)
            {
                Random random = new Random(seed);

                int numTrue = 0;
                int numFalse = 0;
                const int numIterations = 10_000;
                for (int iterations = 0; iterations < numIterations; iterations++)
                {
                    if (RandomExtensions.NextBool(random) == true)
                    {
                        numTrue++;
                    }
                    else
                    {
                        numFalse++;
                    }
                }

                Assert.AreEqual(numTrue / (float)numIterations, numFalse / (float)numIterations, 0.01f, $"Failed with seed {seed}.");
            }
        }
    }
}
