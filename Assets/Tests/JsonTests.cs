using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PAC.Json;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

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

        [Test]
        public void ToJson()
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

        [Test]
        public void FromJson()
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

        [Test]
        public void ToJsonString()
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

        [Test]
        public void ToJsonUndefinedConversion()
        {
            List<int> definedList = new List<int> { 4, 3, 2, 1 };
            List<Class2> undefinedList = new List<Class2> { new Class2("hi", 1, false), new Class2("hello", 2, true) };

            Assert.DoesNotThrow(() => JsonConverter.ToJson(definedList, false));
            Assert.DoesNotThrow(() => JsonConverter.ToJson(undefinedList, true));
            Assert.Throws<Exception>(() => JsonConverter.ToJson(undefinedList, false));
        }

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
            Assert.Throws<Exception>(() => JsonConverter.FromJson<List<Class2>>(undefinedData, false));
        }
    }
}
