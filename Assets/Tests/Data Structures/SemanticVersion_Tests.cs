using System;
using NUnit.Framework;
using PAC.DataStructures;
using PAC.Json;

namespace PAC.Tests.DataStructures
{
    /// <summary>
    /// Tests the SemanticVersion struct.
    /// </summary>
    public class SemanticVersion_Tests
    {
        /// <summary>
        /// Checks that major, minor and patch cannot be negative for SemanticVersion.
        /// </summary>
        [Test]
        [Category("Data Structures")]
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
        [Category("Data Structures")]
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
        [Category("Data Structures")]
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
        [Category("Data Structures")]
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
        [Category("Data Structures")]
        public new void ToString()
        {
            Assert.AreEqual(new SemanticVersion(1, 4, 3).ToString(), "1.4.3");
            Assert.AreEqual(new SemanticVersion(0, 2, 0).ToString(), "0.2.0");
            Assert.AreEqual(new SemanticVersion(100, 0, 3).ToString(), "100.0.3");
        }

        /// <summary>
        /// Checks that ToJson() works properly for the custom JSON converter for type KeyboardShortcut.
        /// </summary>
        [Test]
        [Category("JSON"), Category("Data Structures")]
        public void SemanticVersionToJson()
        {
            JsonConversion.JsonConverterSet converters = new JsonConversion.JsonConverterSet(new SemanticVersion.JsonConverter());

            (SemanticVersion, JsonData)[] testCases =
            {
                (new SemanticVersion(0, 0, 0), new JsonData.String("0.0.0")),
                (new SemanticVersion(1, 4, 3), new JsonData.String("1.4.3")),
                (new SemanticVersion(410, 24, 19), new JsonData.String("410.24.19")),
                (new SemanticVersion(4, 0, 10), new JsonData.String("4.0.10"))
            };

            foreach ((SemanticVersion version, JsonData expected) in testCases)
            {
                Assert.True(JsonData.HaveSameData(JsonConversion.ToJson(version, converters, false), expected));
            }
        }

        /// <summary>
        /// Checks that FromJson() works properly for the custom JSON converter for type KeyboardShortcut.
        /// </summary>
        [Test]
        [Category("JSON"), Category("Data Structures")]
        public void SemanticVersionFromJson()
        {
            JsonConversion.JsonConverterSet converters = new JsonConversion.JsonConverterSet(new SemanticVersion.JsonConverter());

            (SemanticVersion, JsonData)[] testCases =
            {
                (new SemanticVersion(0, 0, 0), new JsonData.String("0.0.0")),
                (new SemanticVersion(1, 4, 3), new JsonData.String("1.4.3")),
                (new SemanticVersion(410, 24, 19), new JsonData.String("410.24.19")),
                (new SemanticVersion(4, 0, 10), new JsonData.String("4.0.10"))
            };

            foreach ((SemanticVersion expected, JsonData jsonData) in testCases)
            {
                Assert.AreEqual(expected, JsonConversion.FromJson<SemanticVersion>(jsonData, converters, false));
            }

            Assert.Catch(() => JsonConversion.FromJson<SemanticVersion>(new JsonData.String(""), converters, false));
            Assert.Catch(() => JsonConversion.FromJson<SemanticVersion>(new JsonData.String("12"), converters, false));
            Assert.Catch(() => JsonConversion.FromJson<SemanticVersion>(new JsonData.String("12."), converters, false));
            Assert.Catch(() => JsonConversion.FromJson<SemanticVersion>(new JsonData.String("12.0"), converters, false));
            Assert.Catch(() => JsonConversion.FromJson<SemanticVersion>(new JsonData.String("12.0."), converters, false));
            Assert.Catch(() => JsonConversion.FromJson<SemanticVersion>(new JsonData.String("12.0.6."), converters, false));
            Assert.Catch(() => JsonConversion.FromJson<SemanticVersion>(new JsonData.String("12.0.6.2"), converters, false));
            Assert.Catch(() => JsonConversion.FromJson<SemanticVersion>(new JsonData.String("12.0.6 "), converters, false));
            Assert.Catch(() => JsonConversion.FromJson<SemanticVersion>(new JsonData.String(" 12.0.6"), converters, false));
            Assert.Catch(() => JsonConversion.FromJson<SemanticVersion>(new JsonData.String("12 .0.6"), converters, false));
            Assert.Catch(() => JsonConversion.FromJson<SemanticVersion>(new JsonData.String("12.0.b"), converters, false));
            Assert.Catch(() => JsonConversion.FromJson<SemanticVersion>(new JsonData.String("12.0.12b"), converters, false));

            Assert.Catch(() => JsonConversion.FromJson<SemanticVersion>(new JsonData.String("-1.3.6"), converters, false));
            Assert.Catch(() => JsonConversion.FromJson<SemanticVersion>(new JsonData.String("1.-3.6"), converters, false));
            Assert.Catch(() => JsonConversion.FromJson<SemanticVersion>(new JsonData.String("1.3.-6"), converters, false));
        }
    }
}
