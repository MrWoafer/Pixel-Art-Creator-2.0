using NUnit.Framework;
using PAC.Extensions;
using System.IO;
using System.Collections.Generic;

namespace PAC.Tests
{
    /// <summary>
    /// Tests my extension methods for type TextReader.
    /// </summary>
    public class TextReaderExtensionsTests
    {
        /// <summary>
        /// Checks that the TextReader.ReadMatch() extension methods work correctly.
        /// </summary>
        [Test]
        [Category("Extensions")]
        public void ReadMatch()
        {
            TextReader reader = new StringReader("hello there!");
            Assert.AreEqual("hello", reader.ReadMatch("hello"));
            Assert.AreEqual(" th", reader.ReadMatch(" thg"));
            Assert.AreEqual("ere!", reader.ReadMatch("ere!"));
            Assert.Catch(() => reader.ReadMatch("Woafer"));

            reader = new StringReader("hello there!");
            Assert.AreEqual("hello there!", reader.ReadMatch("hello there!!"));

            reader = new StringReader("hello there!");
            Assert.AreEqual("", reader.ReadMatch("abc"));

            reader = new StringReader("hello there!");
            Assert.AreEqual("", reader.ReadMatch(""));
            Assert.AreEqual("hello", reader.ReadMatch("hello"));

            reader = new StringReader("hello there!");
            Assert.AreEqual("hel", reader.ReadMatch(new List<char> { 'h', 'e', 'l', 'x' }));
            Assert.AreEqual("lo", reader.ReadMatch(new List<char> { 'l', 'o' }));

            reader = new StringReader("hello there!");
            Assert.AreEqual("he", reader.ReadMatch(new List<char[]> { new char[] { 'h' }, new char[] { 'i', 'e' } }));
            Assert.AreEqual("l", reader.ReadMatch(new List<char[]> { new char[] { 'l', 'a' }, new char[] { 'x' } }));

            reader = new StringReader("hi");
            Assert.AreEqual("hi", reader.ReadMatch(new List<char[]> { new char[] { 'h' }, new char[] { 'i', 'e' } }));
        }

        /// <summary>
        /// Checks that the TextReader.ReadMatchAll() extension methods work correctly.
        /// </summary>
        [Test]
        [Category("Extensions")]
        public void ReadMatchAll()
        {
            TextReader reader = new StringReader("hello there!");
            Assert.True(reader.ReadMatchAll("hello"));
            Assert.True(reader.ReadMatchAll(" th"));
            Assert.True(reader.ReadMatchAll("ere!"));
            Assert.Catch(() => reader.ReadMatchAll("Woafer"));

            reader = new StringReader("hello there!");
            Assert.False(reader.ReadMatchAll("hello there!!"));

            reader = new StringReader("hello there!");
            Assert.False(reader.ReadMatchAll("abc"));

            reader = new StringReader("hello there!");
            Assert.True(reader.ReadMatchAll(""));
            Assert.True(reader.ReadMatchAll("hello"));

            reader = new StringReader("hello there!");
            Assert.True(reader.ReadMatchAll(new List<char> { 'h', 'e', 'l', 'l', 'o' }));
            Assert.False(reader.ReadMatchAll(new List<char> { ' ', 'b' }));

            reader = new StringReader("hello there!");
            Assert.True(reader.ReadMatchAll(new List<char[]> { new char[] { 'h' }, new char[] { 'i', 'e' } }));
            Assert.False(reader.ReadMatchAll(new List<char[]> { new char[] { 'l', 'a' }, new char[] { 'x' } }));

            reader = new StringReader("hi");
            Assert.True(reader.ReadMatchAll(new List<char[]> { new char[] { 'h' }, new char[] { 'i', 'e' } }));
        }
    }
}
