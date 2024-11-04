using System;
using System.Collections.Generic;
using NUnit.Framework;
using PAC.Json;
using System.Linq;
using UnityEngine;
using PAC.Exceptions;

namespace PAC.Tests
{
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

            public static bool operator ==(Class1 obj1 , Class1 obj2)
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
            public bool flag {  get; private set; }

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

            JsonObj jsonObj = new JsonObj
            {
                { "jsonBool", new JsonBool(true) },
                { "jsonInt", new JsonInt(1) },
                { "jsonFloat", new JsonFloat(-4.32f) },
                { "jsonString", new JsonString("hello") },
                { "jsonString2", new JsonNull() },
                { "jsonList", new JsonList(new JsonInt(1), new JsonInt(2), new JsonInt(4), new JsonInt(-10)) },
                { "jsonList2", new JsonList(
                    new JsonObj { { "name", new JsonString("test")}, { "id", new JsonInt(0) }, { "flag", new JsonBool(false) } },
                    new JsonObj { { "name", new JsonString("testing")}, { "id", new JsonInt(63) }, { "flag", new JsonBool(true) } }
                    )
                },
                { "jsonEnum", new JsonString("Value3") }
            };

            Assert.True(JsonData.HaveSameData(JsonConverter.ToJson(obj, true), jsonObj));
        }

        /// <summary>
        /// Checks that FromJson() works properly for undefined conversions.
        /// </summary>
        [Test]
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

            JsonObj jsonObj = new JsonObj
            {
                { "jsonBool", new JsonBool(true) },
                { "jsonInt", new JsonInt(1) },
                { "jsonFloat", new JsonFloat(-4.32f) },
                { "jsonString", new JsonString("hello") },
                { "jsonString2", new JsonNull() },
                { "jsonList", new JsonList(new JsonInt(1), new JsonInt(2), new JsonInt(4), new JsonInt(-10)) },
                { "jsonList2", new JsonList(
                    new JsonObj { { "name", new JsonString("test")}, { "id", new JsonInt(0) }, { "flag", new JsonBool(false) } },
                    new JsonObj { { "name", new JsonString("testing")}, { "id", new JsonInt(63) }, { "flag", new JsonBool(true) } }
                    )
                },
                { "jsonEnum", new JsonString("Value3") }
            };

            Class1 convertedObj = JsonConverter.FromJson<Class1>(jsonObj, true);

            Assert.AreEqual(convertedObj, expectedObj);
        }

        /// <summary>
        /// Checks that ToJsonString() correctly formats JSON data into a string, with pretty = true.
        /// </summary>
        [Test]
        public void ToJsonStringPretty()
        {
            JsonObj jsonObj = new JsonObj
            {
                { "jsonBool", new JsonBool(true) },
                { "jsonInt", new JsonInt(1) },
                { "jsonFloat", new JsonFloat(-4.32f) },
                { "jsonString", new JsonString("hello") },
                { "jsonString2", new JsonNull() },
                { "jsonList", new JsonList(new JsonInt(1), new JsonInt(2), new JsonInt(4), new JsonInt(-10)) },
                { "jsonList2", new JsonList(
                    new JsonObj { { "name", new JsonString("test")}, { "id", new JsonInt(0) }, { "flag", new JsonBool(false) } },
                    new JsonObj { { "name", new JsonString("testing")}, { "id", new JsonInt(63) }, { "flag", new JsonBool(true) } }
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

            Assert.AreEqual(jsonObj.ToJsonString(true), jsonString);
        }

        /// <summary>
        /// Checks that a string in JSON format is correctly parsed into a JsonData object.
        /// </summary>
        [Test]
        public void Parse()
        {
            JsonObj expectedObj = new JsonObj
            {
                { "jsonBool", new JsonBool(true) },
                { "jsonInt", new JsonInt(1) },
                { "jsonFloat", new JsonFloat(-4.32f) },
                { "jsonString", new JsonString("hello") },
                { "jsonString2", new JsonNull() },
                { "jsonList", new JsonList(new JsonInt(1), new JsonInt(2), new JsonInt(4), new JsonInt(-10)) },
                { "jsonList2", new JsonList(
                    new JsonObj { { "name", new JsonString("test")}, { "id", new JsonInt(0) }, { "flag", new JsonBool(false) } },
                    new JsonObj { { "name", new JsonString("testing")}, { "id", new JsonInt(63) }, { "flag", new JsonBool(true) } }
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

            JsonObj parsedObj = JsonObj.Parse(jsonString);

            Assert.True(JsonData.HaveSameData(parsedObj, expectedObj));
        }

        /// <summary>
        /// Checks an exception is thrown if ToJson() encounters a type that can not be converted using conversions for primitive JSON types.
        /// </summary>
        [Test]
        public void ToJsonUndefinedConversion()
        {
            List<int> definedList = new List<int> { 4, 3, 2, 1 };
            List<Class2> undefinedList = new List<Class2> { new Class2("hi", 1, false), new Class2("hello", 2, true) };

            Assert.DoesNotThrow(() => JsonConverter.ToJson(definedList, false));
            Assert.DoesNotThrow(() => JsonConverter.ToJson(undefinedList, true));
            Assert.Catch(() => JsonConverter.ToJson(undefinedList, false));
        }

        /// <summary>
        /// Checks an exception is thrown if FromJson() encounters a type that can not be converted using conversions for primitive JSON types.
        /// </summary>
        [Test]
        public void FromJsonUndefinedConversion()
        {
            JsonList definedData = new JsonList { new JsonInt(4), new JsonInt(3), new JsonInt(2), new JsonInt(1) };
            JsonList undefinedData = new JsonList {
                new JsonObj { { "name", new JsonString("hi")}, { "id", new JsonInt(1) }, { "flag", new JsonBool(false) } },
                new JsonObj { { "name", new JsonString("hello")}, { "id", new JsonInt(2) }, { "flag", new JsonBool(true) } }
            };

            Assert.DoesNotThrow(() => JsonConverter.FromJson<List<int>>(definedData, false));
            Assert.DoesNotThrow(() => JsonConverter.FromJson<List<Class2>>(undefinedData, true));
            Assert.Catch(() => JsonConverter.FromJson<List<Class2>>(undefinedData, false));
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
        public void ToJsonCircularReferencesInField()
        {
            Class3 parent = new Class3("0");
            Class3 child1 = new Class3("1");
            Class3 child2 = new Class3("2");
            parent.child = child1;
            child1.child = child2;

            Assert.DoesNotThrow(() => JsonConverter.ToJson(parent, true));

            child2.child = parent;

            Assert.Catch(() => JsonConverter.ToJson(parent, true));
        }

        /// <summary>
        /// Checks that a exception is thrown if you try to use ToJson() on an object that has circular references, when the circular reference is detected in an auto property's value.
        /// </summary>
        [Test]
        public void ToJsonCircularReferencesInAutoProperty()
        {
            Class4 parent = new Class4("0");
            Class4 child1 = new Class4("1");
            Class4 child2 = new Class4("2");
            parent.child = child1;
            child1.child = child2;

            Assert.DoesNotThrow(() => JsonConverter.ToJson(parent, true));

            child2.child = parent;

            Assert.Catch(() => JsonConverter.ToJson(parent, true));
        }

        /// <summary>
        /// Checks that a exception is thrown if you try to use ToJson() on an object that has circular references, when the circular reference is detected in an array.
        /// </summary>
        [Test]
        public void ToJsonCircularReferencesInArray()
        {
            Class5 parent = new Class5("0");
            Class5 child1 = new Class5("1");
            Class5 child2 = new Class5("2");
            parent.children = new Class5[] { child1, child2 };

            Assert.DoesNotThrow(() => JsonConverter.ToJson(parent, true));

            parent.children = new Class5[] { child1, child2, parent };

            Assert.Catch(() => JsonConverter.ToJson(parent, true));
        }

        /// <summary>
        /// Checks that a exception is thrown if you try to use ToJson() on an object that has circular references, when the circular reference is detected in a list.
        /// </summary>
        [Test]
        public void ToJsonCircularReferencesInList()
        {
            Class6 parent = new Class6("0");
            Class6 child1 = new Class6("1");
            Class6 child2 = new Class6("2");
            parent.children = new List<Class6> { child1, child2 };

            Assert.DoesNotThrow(() => JsonConverter.ToJson(parent, true));

            parent.children.Add(parent);

            Assert.Catch(() => JsonConverter.ToJson(parent, true));
        }

        /// <summary>
        /// Checks that a exception is thrown if you try to use FromJson() on JSON data that has circular references, when the circular reference is detected in a value in a JSON object.
        /// </summary>
        [Test]
        public void FromJsonCircularReferencesInValue()
        {
            JsonObj child2 = new JsonObj
            {
                { "name", "2" },
                { "child", new JsonNull() }
            };
            JsonObj child1 = new JsonObj
            {
                { "name", "1" },
                { "child", child2 }
            };
            JsonObj parent = new JsonObj
            {
                { "name", "0" },
                { "child", child1 }
            };

            Assert.DoesNotThrow(() => JsonConverter.FromJson<Class3>(parent, true));

            child2["child"] = parent;

            Assert.Catch(() => JsonConverter.FromJson<Class3>(parent, true));
        }

        /// <summary>
        /// Checks that a exception is thrown if you try to use FromJson() on JSON data that has circular references, when the circular reference is detected in a JSON list.
        /// </summary>
        [Test]
        public void FromJsonCircularReferencesInList()
        {
            JsonObj jsonObj = new JsonObj
            {
                { "name", "0" },
                { "children", new JsonList(
                    new JsonObj { { "name", new JsonString("1")}, { "children", new JsonList() } },
                    new JsonObj { { "name", new JsonString("2")}, { "children", new JsonList() } }
                    )
                }
            };

            Assert.DoesNotThrow(() => JsonConverter.FromJson<Class6>(jsonObj, true));

            ((JsonList)jsonObj["children"]).Add(jsonObj);

            Assert.Catch(() => JsonConverter.FromJson<Class6>(jsonObj, true));
        }

        private struct ComplexNumber
        {
            public float real { get; set; }
            public float imaginary { get; set; }

            public ComplexNumber(float real,  float imaginary)
            {
                this.real = real;
                this.imaginary = imaginary;
            }

            public override string ToString()
            {
                return real + " + " + imaginary + "i";
            }
        }

        private class ComplexNumberConverter : IJsonConverter<ComplexNumber, JsonList>
        {
            public override JsonList ToJson(ComplexNumber obj)
            {
                return new JsonList(obj.real, obj.imaginary);
            }

            public override ComplexNumber FromJson(JsonList jsonData)
            {
                if (jsonData.Count != 2)
                {
                    throw new Exception("Expected length 2.");
                }
                return new ComplexNumber((JsonFloat)jsonData[0], (JsonFloat)jsonData[1]);
            }
        }

        /// <summary>
        /// Checks that if you provide a custom converter for a type, it will be used in ToJson().
        /// </summary>
        [Test]
        public void ToJsonCustomConverter()
        {
            JsonConverterSet converters = new JsonConverterSet(new ComplexNumberConverter());

            ComplexNumber obj = new ComplexNumber(5f, -2.3f);

            JsonObj expectedWithoutConverter = new JsonObj
            {
                { "real", 5f },
                { "imaginary", -2.3f }
            };

            JsonList expectedWithConverter = new JsonList(5f, -2.3f);

            // Without converter
            Assert.Catch(() => JsonConverter.ToJson(obj, false));
            Assert.True(JsonData.HaveSameData(JsonConverter.ToJson(obj, true), expectedWithoutConverter));

            // With converter
            Assert.True(JsonData.HaveSameData(JsonConverter.ToJson(obj, converters, false), expectedWithConverter));
            Assert.True(JsonData.HaveSameData(JsonConverter.ToJson(obj, converters, true), expectedWithConverter));
        }

        /// <summary>
        /// Checks that if you provide a custom converter for a type, it will be used in FromJson().
        /// </summary>
        [Test]
        public void FromJsonCustomConverter()
        {
            JsonConverterSet converters = new JsonConverterSet(new ComplexNumberConverter());

            ComplexNumber expectedObj = new ComplexNumber(5f, -2.3f);

            JsonObj withoutConverter = new JsonObj
            {
                { "real", 5f },
                { "imaginary", -2.3f }
            };

            JsonList withConverter = new JsonList(5f, -2.3f);

            // Without converter
            Assert.Catch(() => JsonConverter.FromJson<ComplexNumber>(withoutConverter, false));
            Assert.Catch(() => JsonConverter.FromJson<ComplexNumber>(withConverter, true));
            Assert.AreEqual(JsonConverter.FromJson<ComplexNumber>(withoutConverter, true), expectedObj);

            // With converter
            Assert.Catch(() => JsonConverter.FromJson<ComplexNumber>(withoutConverter, converters, true));
            Assert.AreEqual(JsonConverter.FromJson<ComplexNumber>(withConverter, converters, false), expectedObj);
            Assert.AreEqual(JsonConverter.FromJson<ComplexNumber>(withConverter, converters, true), expectedObj);
        }

        [Test]
        public void ToJsonStringEscapedCharacters()
        {
            string str = "\" \\ / \b \f \n \r \t \u03b5 \u03B5";
            string expected = "\"\\\" \\\\ \\/ \\b \\f \\n \\r \\t " + @"\u03B5 \u03B5" + "\"";

            Assert.AreEqual(new JsonString(str).ToJsonString(false), expected);
        }

        [Test]
        public void ParseEscapedCharacters()
        {
            string str = "\"\\\" \\\\ \\/ \\b \\f \\n \\r \\t " + @"\u03b5 \u03B5" + "\"";
            string expected = "\" \\ / \b \f \n \r \t \u03B5 \u03B5";

            Assert.AreEqual(JsonString.Parse(str).value, expected);

            // Should error as there is no escaped character after the \
            Assert.Throws<FormatException>(() => JsonString.Parse("\"hello \\\""));
            Assert.DoesNotThrow(() => JsonString.Parse("\"hello \\\\\""));
            // Should error as there are only 3 hex digits, not 4
            Assert.Throws<FormatException>(() => JsonString.Parse("\"\\uA32\""));
        }

        [Test]
        public void ParseInt()
        {
            Assert.AreEqual(JsonInt.Parse("39").value, 39);
            Assert.AreEqual(JsonInt.Parse("-39").value, -39);
            // Not allowed decimal point
            Assert.Throws<FormatException>(() => JsonInt.Parse("47.2"));
            // - followed by nothing
            Assert.Throws<FormatException>(() => JsonInt.Parse("-"));
            // Not allowed double -
            Assert.Throws<FormatException>(() => JsonInt.Parse("--3"));
            // Number ends too soon
            Assert.Throws<FormatException>(() => JsonInt.Parse("356abc"));
        }

        [Test]
        public void ParseFloat()
        {
            Assert.AreEqual(JsonFloat.Parse("3.259").value, 3.259, 0.0005f);
            Assert.AreEqual(JsonFloat.Parse("-3").value, -3f);
            // Not allowed two decimal points
            Assert.Throws<FormatException>(() => JsonFloat.Parse("47.2.4"));
            // - followed by nothing
            Assert.Throws<FormatException>(() => JsonFloat.Parse("-"));
            // Not allowed double -
            Assert.Throws<FormatException>(() => JsonFloat.Parse("--3"));
            // Number ends too soon
            Assert.Throws<FormatException>(() => JsonFloat.Parse("356.5abc"));
        }

        [Test]
        public void ParseIntENotation()
        {
            Assert.AreEqual(JsonInt.Parse("-39E2").value, -3900);
            Assert.AreEqual(JsonInt.Parse("-39E+2").value, -3900);
            // Not allowed negative exponent
            Assert.Throws<FormatException>(() => JsonInt.Parse("47000e-2"));
            // Not allowed decimal exponent
            Assert.Throws<FormatException>(() => JsonInt.Parse("47000e2.0"));
            // E not followed by a number
            Assert.Throws<FormatException>(() => JsonInt.Parse("47000E"));
            // + not followed by a number
            Assert.Throws<FormatException>(() => JsonInt.Parse("47000e+"));
            // Number ends too soon
            Assert.Throws<FormatException>(() => JsonInt.Parse("2e4abc"));
            // Overflow
            Assert.Throws<OverflowException>(() => JsonInt.Parse("2e" + int.MaxValue));
        }

        [Test]
        public void ParseFloatENotation()
        {
            Assert.AreEqual(JsonFloat.Parse("-3.259e2").value, -325.9f, 0.05f);
            Assert.AreEqual(JsonFloat.Parse("-3.259E+2").value, -325.9f, 0.05f);
            Assert.AreEqual(JsonFloat.Parse("10.4E-2").value, 0.104f, 0.0005f);
            // Not allowed decimal exponent
            Assert.Throws<FormatException>(() => JsonFloat.Parse("47000e-2.0"));
            // E not followed by a number
            Assert.Throws<FormatException>(() => JsonFloat.Parse("47000e"));
            // + not followed by a number
            Assert.Throws<FormatException>(() => JsonFloat.Parse("47000e+"));
            // - not followed by a number
            Assert.Throws<FormatException>(() => JsonFloat.Parse("47000E-"));
            // Number ends too soon
            Assert.Throws<FormatException>(() => JsonFloat.Parse("2.3e-4abc"));
            // Overflow
            Assert.Throws<OverflowException>(() => JsonFloat.Parse("2.0e" + int.MaxValue));
            // Underflow
            Assert.Throws<UnderflowException>(() => JsonFloat.Parse("2.0e" + int.MinValue));
        }

        [Test]
        public void ToJsonStringFloat()
        {
            Assert.AreEqual(new JsonFloat(-3.2f).ToJsonString(false), "-3.2");
            Assert.AreEqual(new JsonFloat(4f).ToJsonString(false), "4.0");
        }
    }
}
