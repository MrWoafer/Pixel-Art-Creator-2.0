using System;
using System.Collections.Generic;
using NUnit.Framework;
using PAC.Json;
using System.Linq;
using PAC.Exceptions;

namespace PAC.Tests
{
    /// <summary>
    /// Tests the JSON system.
    /// </summary>
    public class JsonTests
    {
        private class Class1
        {
            public bool jsonBool;
            protected int jsonInt;
            public float jsonFloat;
            private string jsonString { get; set; }
            public string jsonString2;
            public int[] jsonList;
            public List<Class2> jsonList2;
            public int property => jsonInt + 1;
            public Enum1 jsonEnum;

            public Class1(bool jsonBool, int jsonInt, float jsonFloat, string jsonString, string jsonString2, int[] jsonList, List<Class2> jsonList2, Enum1 jsonEnum)
            {
                this.jsonBool = jsonBool;
                this.jsonInt = jsonInt;
                this.jsonFloat = jsonFloat;
                this.jsonString = jsonString;
                this.jsonString2 = jsonString2;
                this.jsonList = jsonList;
                this.jsonList2 = jsonList2;
                this.jsonEnum = jsonEnum;
            }

            public static bool operator ==(Class1 obj1, Class1 obj2)
            {
                return obj1.jsonBool == obj2.jsonBool && obj1.jsonInt == obj2.jsonInt && obj1.jsonFloat == obj2.jsonFloat && obj1.jsonString == obj2.jsonString &&
                    obj1.jsonString2 == obj2.jsonString2 && obj1.jsonList.SequenceEqual(obj2.jsonList) && obj1.jsonList2.SequenceEqual(obj2.jsonList2) && obj1.property == obj2.property;
            }
            public static bool operator !=(Class1 obj1, Class1 obj2)
            {
                return !(obj1 == obj2);
            }
            public override bool Equals(object obj) => this.Equals(obj as Class1);
            public bool Equals(Class1 obj)
            {
                if (obj is null)
                {
                    return false;
                }
                return this == obj;
            }
        }

        private struct Class2
        {
            public string name;
            private int id;
            public bool flag { get; private set; }

            public Class2(string name, int id, bool flag)
            {
                this.name = name;
                this.id = id;
                this.flag = flag;
            }

            public static bool operator ==(Class2 obj1, Class2 obj2)
            {
                return obj1.name == obj2.name && obj1.id == obj2.id && obj1.flag == obj2.flag;
            }
            public static bool operator !=(Class2 obj1, Class2 obj2)
            {
                return !(obj1 == obj2);
            }
            public override bool Equals(object obj) => this.Equals((Class2)obj);
            public bool Equals(Class2 obj)
            {
                return this == obj;
            }
        }

        private enum Enum1
        {
            Value1 = 0,
            Value2 = -10,
            Value3 = 8,
        }

        /// <summary>
        /// Checks that ToJson() works properly for undefined conversions.
        /// </summary>
        [Test]
        [Category("JSON")]
        public void ToJsonUndefined()
        {
            Class1 obj = new Class1
            (
                true,
                1,
                -4.32f,
                "hello",
                null,
                new int[] { 1, 2, 4, -10 },
                new List<Class2> { new Class2("test", 0, false), new Class2("testing", 63, true) },
                Enum1.Value3
            );

            JsonData.Object jsonObj = new JsonData.Object
            {
                { "jsonBool", new JsonData.Bool(true) },
                { "jsonInt", new JsonData.Int(1) },
                { "jsonFloat", new JsonData.Float(-4.32f) },
                { "jsonString", new JsonData.String("hello") },
                { "jsonString2", new JsonData.Null() },
                { "jsonList", new JsonData.List(new JsonData.Int(1), new JsonData.Int(2), new JsonData.Int(4), new JsonData.Int(-10)) },
                { "jsonList2", new JsonData.List(
                    new JsonData.Object { { "name", new JsonData.String("test")}, { "id", new JsonData.Int(0) }, { "flag", new JsonData.Bool(false) } },
                    new JsonData.Object { { "name", new JsonData.String("testing")}, { "id", new JsonData.Int(63) }, { "flag", new JsonData.Bool(true) } }
                    )
                },
                { "jsonEnum", new JsonData.String("Value3") }
            };

            Assert.True(JsonData.HaveSameData(JsonConversion.ToJson(obj, true), jsonObj));
        }

        /// <summary>
        /// Checks that FromJson() works properly for undefined conversions.
        /// </summary>
        [Test]
        [Category("JSON")]
        public void FromJsonUndefined()
        {
            Class1 expectedObj = new Class1
            (
                true,
                1,
                -4.32f,
                "hello",
                null,
                new int[] { 1, 2, 4, -10 },
                new List<Class2> { new Class2("test", 0, false), new Class2("testing", 63, true) },
                Enum1.Value3
            );

            JsonData.Object jsonObj = new JsonData.Object
            {
                { "jsonBool", new JsonData.Bool(true) },
                { "jsonInt", new JsonData.Int(1) },
                { "jsonFloat", new JsonData.Float(-4.32f) },
                { "jsonString", new JsonData.String("hello") },
                { "jsonString2", new JsonData.Null() },
                { "jsonList", new JsonData.List(new JsonData.Int(1), new JsonData.Int(2), new JsonData.Int(4), new JsonData.Int(-10)) },
                { "jsonList2", new JsonData.List(
                    new JsonData.Object { { "name", new JsonData.String("test")}, { "id", new JsonData.Int(0) }, { "flag", new JsonData.Bool(false) } },
                    new JsonData.Object { { "name", new JsonData.String("testing")}, { "id", new JsonData.Int(63) }, { "flag", new JsonData.Bool(true) } }
                    )
                },
                { "jsonEnum", new JsonData.String("Value3") }
            };

            Class1 convertedObj = JsonConversion.FromJson<Class1>(jsonObj, true);

            Assert.AreEqual(expectedObj, convertedObj);
        }

        /// <summary>
        /// Checks that ToJsonString() correctly formats JSON data into a string, with pretty = true.
        /// </summary>
        [Test]
        [Category("JSON")]
        public void ToJsonStringPretty()
        {
            JsonData.Object jsonObj = new JsonData.Object
            {
                { "jsonBool", new JsonData.Bool(true) },
                { "jsonInt", new JsonData.Int(1) },
                { "jsonFloat", new JsonData.Float(-4.32f) },
                { "jsonString", new JsonData.String("hello") },
                { "jsonString2", new JsonData.Null() },
                { "jsonList", new JsonData.List(new JsonData.Int(1), new JsonData.Int(2), new JsonData.Int(4), new JsonData.Int(-10)) },
                { "jsonList2", new JsonData.List(
                    new JsonData.Object { { "name", new JsonData.String("test")}, { "id", new JsonData.Int(0) }, { "flag", new JsonData.Bool(false) } },
                    new JsonData.Object { { "name", new JsonData.String("testing")}, { "id", new JsonData.Int(63) }, { "flag", new JsonData.Bool(true) } }
                    )
                }
            };

            string jsonString =
                "{" + "\n" +
                "\t" + "\"jsonBool\": true," + "\n" +
                "\t" + "\"jsonInt\": 1," + "\n" +
                "\t" + "\"jsonFloat\": -4.32," + "\n" +
                "\t" + "\"jsonString\": \"hello\"," + "\n" +
                "\t" + "\"jsonString2\": null," + "\n" +
                "\t" + "\"jsonList\": [" + "\n" +
                "\t" + "\t" + "1," + "\n" +
                "\t" + "\t" + "2," + "\n" +
                "\t" + "\t" + "4," + "\n" +
                "\t" + "\t" + "-10" + "\n" +
                "\t" + "]," + "\n" +
                "\t" + "\"jsonList2\": [" + "\n" +
                "\t" + "\t" + "{" + "\n" +
                "\t" + "\t" + "\t" + "\"name\": \"test\"," + "\n" +
                "\t" + "\t" + "\t" + "\"id\": 0," + "\n" +
                "\t" + "\t" + "\t" + "\"flag\": false" + "\n" +
                "\t" + "\t" + "}," + "\n" +
                "\t" + "\t" + "{" + "\n" +
                "\t" + "\t" + "\t" + "\"name\": \"testing\"," + "\n" +
                "\t" + "\t" + "\t" + "\"id\": 63," + "\n" +
                "\t" + "\t" + "\t" + "\"flag\": true" + "\n" +
                "\t" + "\t" + "}" + "\n" +
                "\t" + "]" + "\n" +
                "}"
                ;

            Assert.AreEqual(jsonString, jsonObj.ToJsonString(true));
        }

        /// <summary>
        /// Checks that a string in JSON format is correctly parsed into a JsonData object.
        /// </summary>
        [Test]
        [Category("JSON")]
        public void Parse()
        {
            JsonData.Object expectedObj = new JsonData.Object
            {
                { "jsonBool", new JsonData.Bool(true) },
                { "jsonInt", new JsonData.Int(1) },
                { "jsonFloat", new JsonData.Float(-4.32f) },
                { "jsonString", new JsonData.String("hello") },
                { "jsonString2", new JsonData.Null() },
                { "jsonList", new JsonData.List(new JsonData.Int(1), new JsonData.Int(2), new JsonData.Int(4), new JsonData.Int(-10)) },
                { "jsonList2", new JsonData.List(
                    new JsonData.Object { { "name", new JsonData.String("test")}, { "id", new JsonData.Int(0) }, { "flag", new JsonData.Bool(false) } },
                    new JsonData.Object { { "name", new JsonData.String("testing")}, { "id", new JsonData.Int(63) }, { "flag", new JsonData.Bool(true) } }
                    )
                }
            };

            string jsonString =
                "{" + "\n" +
                "\t" + "\"jsonBool\": true," + "\n" +
                "\t" + "\"jsonInt\": 1," + "\n" +
                "\t" + "\"jsonFloat\": -4.32," + "\n" +
                "\t" + "\"jsonString\": \"hello\"," + "\n" +
                "\t" + "\"jsonString2\": null," + "\n" +
                "\t" + "\"jsonList\": [" + "\n" +
                "\t" + "\t" + "1," + "\n" +
                "\t" + "\t" + "2," + "\n" +
                "\t" + "\t" + "4," + "\n" +
                "\t" + "\t" + "-10" + "\n" +
                "\t" + "]," + "\n" +
                "\t" + "\"jsonList2\": [" + "\n" +
                "\t" + "\t" + "{" + "\n" +
                "\t" + "\t" + "\t" + "\"name\": \"test\"," + "\n" +
                "\t" + "\t" + "\t" + "\"id\": 0," + "\n" +
                "\t" + "\t" + "\t" + "\"flag\": false" + "\n" +
                "\t" + "\t" + "}," + "\n" +
                "\t" + "\t" + "{" + "\n" +
                "\t" + "\t" + "\t" + "\"name\": \"testing\"," + "\n" +
                "\t" + "\t" + "\t" + "\"id\": 63," + "\n" +
                "\t" + "\t" + "\t" + "\"flag\": true" + "\n" +
                "\t" + "\t" + "}" + "\n" +
                "\t" + "]" + "\n" +
                "}"
                ;

            JsonData.Object parsedObj = JsonData.Object.Parse(jsonString);

            Assert.True(JsonData.HaveSameData(parsedObj, expectedObj));
        }

        /// <summary>
        /// Checks an exception is thrown if ToJson() encounters a type that can not be converted using conversions for primitive JSON types.
        /// </summary>
        [Test]
        [Category("JSON")]
        public void ToJsonUndefinedConversion()
        {
            List<int> definedList = new List<int> { 4, 3, 2, 1 };
            List<Class2> undefinedList = new List<Class2> { new Class2("hi", 1, false), new Class2("hello", 2, true) };

            Assert.DoesNotThrow(() => JsonConversion.ToJson(definedList, false));
            Assert.DoesNotThrow(() => JsonConversion.ToJson(undefinedList, true));
            Assert.Catch(() => JsonConversion.ToJson(undefinedList, false));
        }

        /// <summary>
        /// Checks an exception is thrown if FromJson() encounters a type that can not be converted using conversions for primitive JSON types.
        /// </summary>
        [Test]
        [Category("JSON")]
        public void FromJsonUndefinedConversion()
        {
            JsonData.List definedData = new JsonData.List { new JsonData.Int(4), new JsonData.Int(3), new JsonData.Int(2), new JsonData.Int(1) };
            JsonData.List undefinedData = new JsonData.List {
                new JsonData.Object { { "name", new JsonData.String("hi")}, { "id", new JsonData.Int(1) }, { "flag", new JsonData.Bool(false) } },
                new JsonData.Object { { "name", new JsonData.String("hello")}, { "id", new JsonData.Int(2) }, { "flag", new JsonData.Bool(true) } }
            };

            Assert.DoesNotThrow(() => JsonConversion.FromJson<List<int>>(definedData, false));
            Assert.DoesNotThrow(() => JsonConversion.FromJson<List<Class2>>(undefinedData, true));
            Assert.Catch(() => JsonConversion.FromJson<List<Class2>>(undefinedData, false));
        }

        private class Class3
        {
            public string name;
            public Class3 child;

            public Class3(string name)
            {
                this.name = name;
            }
        }

        private class Class4
        {
            public string name;
            public Class4 child { get; set; }

            public Class4(string name)
            {
                this.name = name;
            }
        }

        private class Class5
        {
            public string name;
            public Class5[] children;

            public Class5(string name)
            {
                this.name = name;
            }
        }

        private class Class6
        {
            public string name;
            public List<Class6> children;

            public Class6(string name)
            {
                this.name = name;
            }
        }

        /// <summary>
        /// Checks that a exception is thrown if you try to use ToJson() on an object that has circular references, when the circular reference is detected in a field's value.
        /// </summary>
        [Test]
        [Category("JSON")]
        public void ToJsonCircularReferencesInField()
        {
            Class3 parent = new Class3("0");
            Class3 child1 = new Class3("1");
            Class3 child2 = new Class3("2");
            parent.child = child1;
            child1.child = child2;

            Assert.DoesNotThrow(() => JsonConversion.ToJson(parent, true));

            child2.child = parent;

            Assert.Catch(() => JsonConversion.ToJson(parent, true));
        }

        /// <summary>
        /// Checks that a exception is thrown if you try to use ToJson() on an object that has circular references, when the circular reference is detected in an auto property's value.
        /// </summary>
        [Test]
        [Category("JSON")]
        public void ToJsonCircularReferencesInAutoProperty()
        {
            Class4 parent = new Class4("0");
            Class4 child1 = new Class4("1");
            Class4 child2 = new Class4("2");
            parent.child = child1;
            child1.child = child2;

            Assert.DoesNotThrow(() => JsonConversion.ToJson(parent, true));

            child2.child = parent;

            Assert.Catch(() => JsonConversion.ToJson(parent, true));
        }

        /// <summary>
        /// Checks that a exception is thrown if you try to use ToJson() on an object that has circular references, when the circular reference is detected in an array.
        /// </summary>
        [Test]
        [Category("JSON")]
        public void ToJsonCircularReferencesInArray()
        {
            Class5 parent = new Class5("0");
            Class5 child1 = new Class5("1");
            Class5 child2 = new Class5("2");
            parent.children = new Class5[] { child1, child2 };

            Assert.DoesNotThrow(() => JsonConversion.ToJson(parent, true));

            parent.children = new Class5[] { child1, child2, parent };

            Assert.Catch(() => JsonConversion.ToJson(parent, true));
        }

        /// <summary>
        /// Checks that a exception is thrown if you try to use ToJson() on an object that has circular references, when the circular reference is detected in a list.
        /// </summary>
        [Test]
        [Category("JSON")]
        public void ToJsonCircularReferencesInList()
        {
            Class6 parent = new Class6("0");
            Class6 child1 = new Class6("1");
            Class6 child2 = new Class6("2");
            parent.children = new List<Class6> { child1, child2 };

            Assert.DoesNotThrow(() => JsonConversion.ToJson(parent, true));

            parent.children.Add(parent);

            Assert.Catch(() => JsonConversion.ToJson(parent, true));
        }

        /// <summary>
        /// Checks that a exception is thrown if you try to use FromJson() on JSON data that has circular references, when the circular reference is detected in a value in a JSON object.
        /// </summary>
        [Test]
        [Category("JSON")]
        public void FromJsonCircularReferencesInValue()
        {
            JsonData.Object child2 = new JsonData.Object
            {
                { "name", "2" },
                { "child", new JsonData.Null() }
            };
            JsonData.Object child1 = new JsonData.Object
            {
                { "name", "1" },
                { "child", child2 }
            };
            JsonData.Object parent = new JsonData.Object
            {
                { "name", "0" },
                { "child", child1 }
            };

            Assert.DoesNotThrow(() => JsonConversion.FromJson<Class3>(parent, true));

            child2["child"] = parent;

            Assert.Catch(() => JsonConversion.FromJson<Class3>(parent, true));
        }

        /// <summary>
        /// Checks that a exception is thrown if you try to use FromJson() on JSON data that has circular references, when the circular reference is detected in a JSON list.
        /// </summary>
        [Test]
        [Category("JSON")]
        public void FromJsonCircularReferencesInList()
        {
            JsonData.Object jsonObj = new JsonData.Object
            {
                { "name", "0" },
                { "children", new JsonData.List(
                    new JsonData.Object { { "name", new JsonData.String("1")}, { "children", new JsonData.List() } },
                    new JsonData.Object { { "name", new JsonData.String("2")}, { "children", new JsonData.List() } }
                    )
                }
            };

            Assert.DoesNotThrow(() => JsonConversion.FromJson<Class6>(jsonObj, true));

            ((JsonData.List)jsonObj["children"]).Add(jsonObj);

            Assert.Catch(() => JsonConversion.FromJson<Class6>(jsonObj, true));
        }

        private struct ComplexNumber
        {
            public float real { get; set; }
            public float imaginary { get; set; }

            public ComplexNumber(float real, float imaginary)
            {
                this.real = real;
                this.imaginary = imaginary;
            }

            public override string ToString()
            {
                return real + " + " + imaginary + "i";
            }
        }

        private class ComplexNumberConverter : JsonConversion.JsonConverter<ComplexNumber, JsonData.List>
        {
            public override JsonData.List ToJson(ComplexNumber obj)
            {
                return new JsonData.List(obj.real, obj.imaginary);
            }

            public override ComplexNumber FromJson(JsonData.List jsonData)
            {
                if (jsonData.Count != 2)
                {
                    throw new Exception("Expected length 2.");
                }
                return new ComplexNumber((JsonData.Float)jsonData[0], (JsonData.Float)jsonData[1]);
            }
        }

        /// <summary>
        /// Checks that if you provide a custom converter for a type, it will be used in ToJson().
        /// </summary>
        [Test]
        [Category("JSON")]
        public void ToJsonCustomConverter()
        {
            JsonConversion.JsonConverterSet converters = new JsonConversion.JsonConverterSet(new ComplexNumberConverter());

            ComplexNumber obj = new ComplexNumber(5f, -2.3f);

            JsonData.Object expectedWithoutConverter = new JsonData.Object
            {
                { "real", 5f },
                { "imaginary", -2.3f }
            };

            JsonData.List expectedWithConverter = new JsonData.List(5f, -2.3f);

            // Without converter
            Assert.Catch(() => JsonConversion.ToJson(obj, false));
            Assert.True(JsonData.HaveSameData(JsonConversion.ToJson(obj, true), expectedWithoutConverter));

            // With converter
            Assert.True(JsonData.HaveSameData(JsonConversion.ToJson(obj, converters, false), expectedWithConverter));
            Assert.True(JsonData.HaveSameData(JsonConversion.ToJson(obj, converters, true), expectedWithConverter));
        }

        /// <summary>
        /// Checks that if you provide a custom converter for a type, it will be used in FromJson().
        /// </summary>
        [Test]
        [Category("JSON")]
        public void FromJsonCustomConverter()
        {
            JsonConversion.JsonConverterSet converters = new JsonConversion.JsonConverterSet(new ComplexNumberConverter());

            ComplexNumber expectedObj = new ComplexNumber(5f, -2.3f);

            JsonData.Object withoutConverter = new JsonData.Object
            {
                { "real", 5f },
                { "imaginary", -2.3f }
            };

            JsonData.List withConverter = new JsonData.List(5f, -2.3f);

            // Without converter
            Assert.Catch(() => JsonConversion.FromJson<ComplexNumber>(withoutConverter, false));
            Assert.Catch(() => JsonConversion.FromJson<ComplexNumber>(withConverter, true));
            Assert.AreEqual(expectedObj, JsonConversion.FromJson<ComplexNumber>(withoutConverter, true));

            // With converter
            Assert.Catch(() => JsonConversion.FromJson<ComplexNumber>(withoutConverter, converters, true));
            Assert.AreEqual(expectedObj, JsonConversion.FromJson<ComplexNumber>(withConverter, converters, false));
            Assert.AreEqual(expectedObj, JsonConversion.FromJson<ComplexNumber>(withConverter, converters, true));
        }

        /// <summary>
        /// Checks that JsonData.String.ToJsonString() correctly escapes escaped characters.
        /// </summary>
        [Test]
        [Category("JSON")]
        public void ToJsonStringEscapedCharacters()
        {
            string str = "\" \\ / \b \f \n \r \t \u03b5 \u03B5";
            string expected = "\"\\\" \\\\ \\/ \\b \\f \\n \\r \\t " + @"\u03B5 \u03B5" + "\"";

            Assert.AreEqual(expected, new JsonData.String(str).ToJsonString(false));
        }

        /// <summary>
        /// Checks that JsonData.String.Parse() correctly parses escaped characters.
        /// </summary>
        [Test]
        [Category("JSON")]
        public void ParseEscapedCharacters()
        {
            string str = "\"\\\" \\\\ \\/ \\b \\f \\n \\r \\t " + @"\u03b5 \u03B5" + "\"";
            string expected = "\" \\ / \b \f \n \r \t \u03B5 \u03B5";

            Assert.AreEqual(expected, JsonData.String.Parse(str).value);

            // Should error as there is no escaped character after the \
            Assert.Throws<FormatException>(() => JsonData.String.Parse("\"hello \\\""));
            Assert.DoesNotThrow(() => JsonData.String.Parse("\"hello \\\\\""));
            // Should error as there are only 3 hex digits, not 4
            Assert.Throws<FormatException>(() => JsonData.String.Parse("\"\\uA32\""));
        }

        /// <summary>
        /// Checks JsonData.Ints are parsed correctly.
        /// </summary>
        [Test]
        [Category("JSON")]
        public void ParseInt()
        {
            Assert.AreEqual(39, JsonData.Int.Parse("39").value);
            Assert.AreEqual(-39, JsonData.Int.Parse("-39").value);
            // Not allowed decimal point
            Assert.Throws<FormatException>(() => JsonData.Int.Parse("47.2"));
            // - followed by nothing
            Assert.Throws<FormatException>(() => JsonData.Int.Parse("-"));
            // Not allowed double -
            Assert.Throws<FormatException>(() => JsonData.Int.Parse("--3"));
            // Number ends too soon
            Assert.Throws<FormatException>(() => JsonData.Int.Parse("356abc"));
        }

        /// <summary>
        /// Checks JsonData.Floats are parsed correctly.
        /// </summary>
        [Test]
        [Category("JSON")]
        public void ParseFloat()
        {
            Assert.AreEqual(JsonData.Float.Parse("3.259").value, 3.259, 0.0005f);
            Assert.AreEqual(-3f, JsonData.Float.Parse("-3").value);
            // Not allowed two decimal points
            Assert.Throws<FormatException>(() => JsonData.Float.Parse("47.2.4"));
            // - followed by nothing
            Assert.Throws<FormatException>(() => JsonData.Float.Parse("-"));
            // Not allowed double -
            Assert.Throws<FormatException>(() => JsonData.Float.Parse("--3"));
            // Number ends too soon
            Assert.Throws<FormatException>(() => JsonData.Float.Parse("356.5abc"));
        }

        /// <summary>
        /// Checks JsonData.Ints are parsed correctly in E notation.
        /// </summary>
        [Test]
        [Category("JSON")]
        public void ParseIntENotation()
        {
            Assert.AreEqual(-3900, JsonData.Int.Parse("-39E2").value);
            Assert.AreEqual(-3900, JsonData.Int.Parse("-39E+2").value);
            Assert.AreEqual(39, JsonData.Int.Parse("39E0").value);
            // Not allowed negative exponent
            Assert.Throws<FormatException>(() => JsonData.Int.Parse("47000e-2"));
            // Not allowed decimal exponent
            Assert.Throws<FormatException>(() => JsonData.Int.Parse("47000e2.0"));
            // E not followed by a number
            Assert.Throws<FormatException>(() => JsonData.Int.Parse("47000E"));
            // + not followed by a number
            Assert.Throws<FormatException>(() => JsonData.Int.Parse("47000e+"));
            // Number ends too soon
            Assert.Throws<FormatException>(() => JsonData.Int.Parse("2e4abc"));
            // Overflow
            Assert.Throws<OverflowException>(() => JsonData.Int.Parse("2e" + int.MaxValue));
        }

        /// <summary>
        /// Checks JsonData.Floats are parsed correctly in E Notation.
        /// </summary>
        [Test]
        [Category("JSON")]
        public void ParseFloatENotation()
        {
            Assert.AreEqual(JsonData.Float.Parse("-3.259e2").value, -325.9f, 0.05f);
            Assert.AreEqual(JsonData.Float.Parse("-3.259E+2").value, -325.9f, 0.05f);
            Assert.AreEqual(JsonData.Float.Parse("10.4E-2").value, 0.104f, 0.0005f);
            Assert.AreEqual(JsonData.Float.Parse("10.4E0").value, 10.4f, 0.05f);
            // Not allowed decimal exponent
            Assert.Throws<FormatException>(() => JsonData.Float.Parse("47000e-2.0"));
            // E not followed by a number
            Assert.Throws<FormatException>(() => JsonData.Float.Parse("47000e"));
            // + not followed by a number
            Assert.Throws<FormatException>(() => JsonData.Float.Parse("47000e+"));
            // - not followed by a number
            Assert.Throws<FormatException>(() => JsonData.Float.Parse("47000E-"));
            // Number ends too soon
            Assert.Throws<FormatException>(() => JsonData.Float.Parse("2.3e-4abc"));
            // Overflow
            Assert.Throws<OverflowException>(() => JsonData.Float.Parse("2.0e" + int.MaxValue));
            // Underflow
            Assert.Throws<UnderflowException>(() => JsonData.Float.Parse("2.0e" + int.MinValue));
        }

        /// <summary>
        /// Checks that when JsonData.Float.ToJsonString() always includes a decimal point.
        /// </summary>
        [Test]
        [Category("JSON")]
        public void ToJsonStringFloat()
        {
            Assert.AreEqual("-3.2", new JsonData.Float(-3.2f).ToJsonString(false));
            Assert.AreEqual("4.0", new JsonData.Float(4f).ToJsonString(false));
        }

        /// <summary>
        /// Checks that a JSON string cannot hold a null value.
        /// </summary>
        [Test]
        [Category("JSON")]
        public void JsonStringNull()
        {
            Assert.Throws<ArgumentException>(() => new JsonData.String(null));

            JsonData.String jsonString = new JsonData.String("");
            Assert.Throws<ArgumentException>(() => jsonString.value = null);
        }

        /// <summary>
        /// Checks that JSON strings, lists and objects handle parsing null values correctly.
        /// </summary>
        [Test]
        [Category("JSON")]
        public void ParseNull()
        {
            Assert.Catch<FormatException>(() => JsonData.String.Parse("null"));
            Assert.Catch<FormatException>(() => JsonData.List.Parse("null"));
            Assert.Catch<FormatException>(() => JsonData.Object.Parse("null"));

            Assert.True(JsonData.HaveSameData(JsonData.String.ParseMaybeNull("\"hi\""), new JsonData.String("hi")));
            Assert.True(JsonData.HaveSameData(JsonData.List.ParseMaybeNull("[]"), new JsonData.List()));
            Assert.True(JsonData.HaveSameData(JsonData.Object.ParseMaybeNull("{}"), new JsonData.Object()));
        }

        /// <summary>
        /// Checks that empty JSON lists/objects are parsed correctly.
        /// </summary>
        [Test]
        [Category("JSON")]
        public void ParseEmpty()
        {
            Assert.True(JsonData.HaveSameData(JsonData.List.Parse("[]"), new JsonData.List()));
            Assert.True(JsonData.HaveSameData(JsonData.Object.Parse("{}"), new JsonData.Object()));

            Assert.True(JsonData.HaveSameData(JsonData.List.Parse("[     ]"), new JsonData.List()));
            Assert.True(JsonData.HaveSameData(JsonData.Object.Parse("{   }"), new JsonData.Object()));
        }

        private class ParentClass
        {
            public int id;
        }

        private class ChildClass1 : ParentClass
        {
            public string description;

            public ChildClass1(int id, string description)
            {
                this.id = id;
                this.description = description;
            }

            public static bool operator !=(ChildClass1 a, ChildClass1 b) => !(a == b);
            public static bool operator ==(ChildClass1 a, ChildClass1 b)
            {
                return a.id == b.id && a.description == b.description;
            }

            public override bool Equals(object obj) => this.Equals(obj as ChildClass1);
            public bool Equals(ChildClass1 obj)
            {
                if (obj is null)
                {
                    return false;
                }
                return this == obj;
            }
        }

        private class ChildClass2 : ParentClass
        {
            public string info;

            public ChildClass2(int id, string info)
            {
                this.id = id;
                this.info = info;
            }

            public static bool operator !=(ChildClass2 a, ChildClass2 b) => !(a == b);
            public static bool operator ==(ChildClass2 a, ChildClass2 b)
            {
                return a.id == b.id && a.info == b.info;
            }

            public override bool Equals(object obj) => this.Equals(obj as ChildClass2);
            public bool Equals(ChildClass2 obj)
            {
                if (obj is null)
                {
                    return false;
                }
                return this == obj;
            }
        }

        [Test]
        [Category("JSON")]
        public void ToJsonSubclasses()
        {
            List<ParentClass> list = new List<ParentClass> { new ChildClass1(0, "hello"), new ChildClass2(1, "hi") };
            JsonData.List expected = new JsonData.List
            {
                new JsonData.Object
                {
                    { "id", 0 },
                    { "description", "hello" },
                },
                new JsonData.Object
                {
                    { "id", 1 },
                    { "info", "hi" },
                }
            };

            Assert.True(JsonData.HaveSameData(expected, JsonConversion.ToJson(list, true)));
        }

        [Test]
        [Category("JSON")]
        public void FromJsonSubclasses()
        {
            List<ParentClass> list = new List<ParentClass> { new ChildClass1(0, "hello"), new ChildClass2(1, "hi") };
            JsonData.List expected = new JsonData.List
            {
                new JsonData.Object
                {
                    { "id", 0 },
                    { "description", "hello" },
                },
                new JsonData.Object
                {
                    { "id", 1 },
                    { "info", "hi" },
                }
            };

            Assert.True(list.SequenceEqual(JsonConversion.FromJson<List<ParentClass>>(expected, true)));
        }

        /// <summary>
        /// Tests JsonData.Object.Concat(), JsonData.Object.Append() and JsonData.Object.Prepend().
        /// </summary>
        [Test]
        public void JsonObjectConcat()
        {
            Assert.True(JsonData.HaveSameData(new JsonData.Object(), JsonData.Object.Concat()));

            JsonData.Object jsonObj1 = new JsonData.Object
            {
                { "id", 1 },
                { "description", "hello" }
            };

            JsonData.Object jsonObj2 = new JsonData.Object
            {
                { "name", "Woafer" },
            };

            JsonData.Object jsonObj3 = new JsonData.Object
            {
                { "age", 1000 },
            };

            JsonData.Object[] concats =
            {
                JsonData.Object.Concat(jsonObj1, jsonObj2, jsonObj3),
                jsonObj1.Append(jsonObj2, jsonObj3),
                jsonObj3.Prepend(jsonObj1, jsonObj2)
            };

            foreach (JsonData.Object concat in concats)
            {
                JsonData.Object expected = new JsonData.Object
                {
                    { "id", 1 },
                    { "description", "hello" },
                    { "name", "Woafer" },
                    { "age", 1000 }
                };

                IEnumerator<KeyValuePair<string, JsonData>> concatEnumerable = expected.GetEnumerator();

                foreach (KeyValuePair<string, JsonData> expectedPair in expected)
                {
                    Assert.True(concatEnumerable.MoveNext(), "concat ran out before expected.");

                    KeyValuePair<string, JsonData> concatPair = concatEnumerable.Current;

                    Assert.AreEqual(expectedPair.Key, concatPair.Key, "Keys didn't match.");
                    Assert.True(JsonData.HaveSameData(expectedPair.Value, concatPair.Value), "Values didn't match.");
                }
                Assert.False(concatEnumerable.MoveNext(), "expected ran out before concat");
            }
        }
    }
}
