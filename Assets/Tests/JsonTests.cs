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

            public Class1(bool jsonBool, int jsonInt, float jsonFloat, string jsonString, string jsonString2, int[] jsonList, List<Class2> jsonList2)
            {
                this.jsonBool = jsonBool;
                this.jsonInt = jsonInt;
                this.jsonFloat = jsonFloat;
                this.jsonString = jsonString;
                this.jsonString2 = jsonString2;
                this.jsonList = jsonList;
                this.jsonList2 = jsonList2;
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
                new List<Class2> { new Class2("test", 0, false), new Class2("testing", 63, true) }
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
                }
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
                new List<Class2> { new Class2("test", 0, false), new Class2("testing", 63, true) }
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
                }
            };

            Class1 convertedObj = JsonConverter.FromJson<Class1>(jsonObj, true);

            Assert.True(convertedObj == expectedObj);
        }
    }
}
