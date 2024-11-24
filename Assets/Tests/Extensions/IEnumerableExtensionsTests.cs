using NUnit.Framework;
using PAC.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace PAC.Tests
{
    public class IEnumerableExtensionsTests
    {
        [Test]
        [Category("Extensions")]
        public void IsEmpty()
        {
            for (int inputLength = 0; inputLength <= 10; inputLength++)
            {
                int[] input = new int[inputLength];
                Assert.AreEqual(input.Length == 0, input.IsEmpty(), "Failed with length " + inputLength);
            }
        }

        [Test]
        [Category("Extensions")]
        public void CountExactly()
        {
            for (int inputLength = 0; inputLength <= 10; inputLength++)
            {
                int[] input = new int[inputLength];
                for (int testLength = -10; testLength <= 20; testLength++)
                {
                    Assert.AreEqual(input.Length == testLength, input.CountExactly(testLength), "Failed with input length " + inputLength + " and test length " + testLength);
                }
            }
        }

        [Test]
        [Category("Extensions")]
        public void CountAtLeast()
        {
            for (int inputLength = 0; inputLength <= 10; inputLength++)
            {
                int[] input = new int[inputLength];
                for (int testLength = -10; testLength <= 20; testLength++)
                {
                    Assert.AreEqual(input.Length >= testLength, input.CountAtLeast(testLength), "Failed with input length " + inputLength + " and test length " + testLength);
                }
            }
        }

        [Test]
        [Category("Extensions")]
        public void CountAtMost()
        {
            for (int inputLength = 0; inputLength <= 10; inputLength++)
            {
                int[] input = new int[inputLength];
                for (int testLength = -10; testLength <= 20; testLength++)
                {
                    Assert.AreEqual(input.Length <= testLength, input.CountAtMost(testLength), "Failed with input length " + inputLength + " and test length " + testLength);
                }
            }
        }
    }
}
