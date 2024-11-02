using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using PAC.Json;

namespace PAC.Tests
{
    public class JsonTests
    {
        private class Class1
        {
            public bool jsonBool;
            public int jsonInt;
            public float jsonFloat;
            public string jsonString;
            public string jsonString2;
            public int[] jsonList;
            public List<Class2> jsonList2;
        }

        private class Class2
        {
            protected string name;
            private int id;
            public bool flag {  get; private set; }

            public Class2(string name, int id, bool flag)
            {
                this.name = name;
                this.id = id;
                this.flag = flag;
            }
        }

        [Test]
        public void ToJson()
        {
            Class1 obj = new Class1
            {
                jsonBool = true,
                jsonInt = 1,
                jsonFloat = -4.32f,
                jsonString = "hello",
                jsonString2 = null,
                jsonList = new int[] { 1, 2, 4, -10 },
                jsonList2 = new List<Class2> { new Class2("test", 0, false), new Class2("testing", 63, true) }
            };

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
    }
}
