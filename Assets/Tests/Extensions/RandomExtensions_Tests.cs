using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

using PAC.Extensions;

namespace PAC.Tests
{
    /// <summary>
    /// Tests for <see cref="RandomExtensions"/>.
    /// </summary>
    public class RandomExtensions_Tests
    {
        [Test]
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
    }
}
