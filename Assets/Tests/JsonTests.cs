using System.Collections;
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
                jsonString2 = null
            };

            JsonObj jsonObj = new JsonObj
            {
                { "jsonBool", new JsonBool(true) },
                { "jsonInt", new JsonInt(1) },
                { "jsonFloat", new JsonFloat(-4.32f) },
                { "jsonString", new JsonString("hello") },
                { "jsonString2", new JsonNull() },
            };

            Assert.True(JsonData.HaveSameData(JsonConverter.ToJson(obj, true), jsonObj));
        }
    }
}
