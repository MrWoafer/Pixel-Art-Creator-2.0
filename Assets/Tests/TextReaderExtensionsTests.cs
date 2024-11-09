using NUnit.Framework;
using PAC.Extensions;
using System.IO;

namespace PAC.Tests
{
    /// <summary>
    /// Tests my extension methods for type TextReader.
    /// </summary>
    public class TextReaderExtensionsTests
    {
        [Test]
        [Category("Extensions")]
        public void ReadMatch()
        {
            TextReader reader = new StringReader("hello there!");
            Assert.True(reader.ReadMatch("hello"));
            Assert.True(reader.ReadMatch(" th"));
            Assert.True(reader.ReadMatch("ere!"));
            Assert.Catch(() => reader.ReadMatch("Woafer"));

            reader = new StringReader("hello there!");
            Assert.False(reader.ReadMatch("hello there!!"));

            reader = new StringReader("hello there!");
            Assert.False(reader.ReadMatch("abc"));

            reader = new StringReader("hello there!");
            Assert.True(reader.ReadMatch(""));
            Assert.True(reader.ReadMatch("hello"));
        }
    }
}
