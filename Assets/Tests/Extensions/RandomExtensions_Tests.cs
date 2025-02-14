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
            Random random = new Random(seed);
            // We compare with a new System.Random with the same seed, instead of just against random, because ToSequence() does not deep-copy the System.Random, so calling random.Next() between
            // the generation of two elements would affect the sequence.
            Random randomCopy = new Random(seed);

            IEnumerable<int> sequence = random.ToSequence();

            bool isEmpty = true;
            foreach (int i in sequence.Take(100))
            {
                isEmpty = false;
                Assert.AreEqual(randomCopy.Next(), i);
            }
            Assert.False(isEmpty);

            /////
            
            random = new Random(seed);
            randomCopy = new Random(seed);

            const int min = -12;
            const int max = 19;
            sequence = random.ToSequence(min, max);

            isEmpty = true;
            foreach (int i in sequence.Take(100))
            {
                isEmpty = false;
                Assert.AreEqual(randomCopy.Next(min, max), i);
            }
            Assert.False(isEmpty);

            // Can't have maxValue < minValue
            // I'm doing the ToArray() because currently, due to using yield, the input validation only happens when you generate the first element 
            Assert.Throws<ArgumentOutOfRangeException>(() => random.ToSequence(10, 5).Take(1).ToArray());
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

        /// <summary>
        /// Tests <see cref="RandomExtensions.NextElement{T}(Random, IReadOnlyList{T})"/>.
        /// </summary>
        [Test]
        [Category("Extensions"), Category("Random")]
        public void NextElement()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                Random random = new Random(0);
                RandomExtensions.NextElement(random, new char[0]);
            });

            for (int length = 1; length <= 5; length++)
            {
                int[] elements = new int[length];
                for (int i = 0; i < length; i++)
                {
                    elements[i] = i * i * 17 - 3;
                }

                Random random = new Random(length);

                Dictionary<int, int> counts = new Dictionary<int, int>();
                foreach (int element in elements)
                {
                    counts[element] = 0;
                }

                const int numIterations = 10_000;
                for (int iterations = 0; iterations < numIterations; iterations++)
                {
                    int nextElement = RandomExtensions.NextElement(random, elements);
                    counts[nextElement]++;
                }

                float expected = 1f / length;
                foreach (int element in elements)
                {
                    Assert.AreEqual(expected, counts[element] / (float)numIterations, 0.01f, $"Failed with seed {length}.");
                }
            }
        }
    }
}
