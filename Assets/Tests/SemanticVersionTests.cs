using System;
using NUnit.Framework;
using PAC.Files;

namespace PAC.Tests
{
    /// <summary>
    /// Tests the SemanticVersion struct.
    /// </summary>
    public class SemanticVersionTests
    {
        /// <summary>
        /// Checks that major, minor and patch cannot be negative for SemanticVersion.
        /// </summary>
        [Test]
        public void NonNegative()
        {
            Assert.Throws<ArgumentException>(() => new SemanticVersion(-1, 2, 3));
            Assert.Throws<ArgumentException>(() => new SemanticVersion(1, -2, 3));
            Assert.Throws<ArgumentException>(() => new SemanticVersion(1, 2, -3));

            Assert.DoesNotThrow(() => new SemanticVersion(1, 2, 3));
        }

        /// <summary>
        /// Tests the == and != operators for SemanticVersion.
        /// </summary>
        [Test]
        public void Equality()
        {
            Assert.True(new SemanticVersion(0, 2, 3) == new SemanticVersion(0, 2, 3));
            Assert.True(new SemanticVersion(5, 7, 1) == new SemanticVersion(5, 7, 1));
            Assert.True(new SemanticVersion(2, 200, 22) == new SemanticVersion(2, 200, 22));

            Assert.AreEqual(new SemanticVersion(0, 2, 3), new SemanticVersion(0, 2, 3));
            Assert.AreEqual(new SemanticVersion(5, 7, 1), new SemanticVersion(5, 7, 1));
            Assert.AreEqual(new SemanticVersion(2, 200, 22), new SemanticVersion(2, 200, 22));

            Assert.True(new SemanticVersion(0, 2, 3) != new SemanticVersion(1, 2, 3));
            Assert.True(new SemanticVersion(0, 2, 3) != new SemanticVersion(0, 3, 3));
            Assert.True(new SemanticVersion(0, 2, 4) != new SemanticVersion(0, 2, 3));
        }

        /// <summary>
        /// Tests the &lt;, &gt;, &lt;= and &gt;= operators for SemanticVersion.
        /// </summary>
        [Test]
        public void Inequality()
        {
            Assert.True(new SemanticVersion(0, 1, 5) < new SemanticVersion(1, 0, 0));
            Assert.True(new SemanticVersion(1, 1, 5) < new SemanticVersion(1, 2, 0));
            Assert.True(new SemanticVersion(1, 1, 5) < new SemanticVersion(1, 1, 6));

            Assert.False(new SemanticVersion(0, 1, 5) > new SemanticVersion(1, 0, 0));
            Assert.False(new SemanticVersion(1, 1, 5) > new SemanticVersion(1, 2, 0));
            Assert.False(new SemanticVersion(1, 1, 5) > new SemanticVersion(1, 1, 6));

            Assert.True(new SemanticVersion(2, 4, 5) <= new SemanticVersion(2, 10, 6));
            Assert.True(new SemanticVersion(3, 1, 5) >= new SemanticVersion(1, 2, 100));

            Assert.True(new SemanticVersion(1, 1, 5) <= new SemanticVersion(1, 1, 5));
            Assert.True(new SemanticVersion(1, 1, 5) >= new SemanticVersion(1, 1, 5));
            Assert.False(new SemanticVersion(1, 1, 5) < new SemanticVersion(1, 1, 5));
            Assert.False(new SemanticVersion(1, 1, 5) > new SemanticVersion(1, 1, 5));
        }

        /// <summary>
        /// Tests the - operator for SemanticVersion.
        /// </summary>
        [Test]
        public void Minus()
        {
            Assert.AreEqual(new SemanticVersion(1, 0, 0), new SemanticVersion(2, 4, 3) - new SemanticVersion(1, 6, 10));
            Assert.AreEqual(new SemanticVersion(0, 2, 0), new SemanticVersion(1, 4, 3) - new SemanticVersion(1, 6, 10));
            Assert.AreEqual(new SemanticVersion(0, 0, 7), new SemanticVersion(1, 4, 3) - new SemanticVersion(1, 4, 10));

            // Commutativity
            Assert.AreEqual(new SemanticVersion(2, 4, 3) - new SemanticVersion(1, 6, 10), new SemanticVersion(1, 6, 10) - new SemanticVersion(2, 4, 3));
            Assert.AreEqual(new SemanticVersion(1, 4, 3) - new SemanticVersion(1, 6, 10), new SemanticVersion(1, 6, 10) - new SemanticVersion(1, 4, 3));
            Assert.AreEqual(new SemanticVersion(1, 4, 3) - new SemanticVersion(1, 4, 10), new SemanticVersion(1, 4, 10) - new SemanticVersion(1, 4, 3));
        }

        /// <summary>
        /// Tests SemanticVersion.ToString().
        /// </summary>
        [Test]
        public new void ToString()
        {
            Assert.AreEqual(new SemanticVersion(1, 4, 3).ToString(), "1.4.3");
            Assert.AreEqual(new SemanticVersion(0, 2, 0).ToString(), "0.2.0");
            Assert.AreEqual(new SemanticVersion(100, 0, 3).ToString(), "100.0.3");
        }
    }
}
