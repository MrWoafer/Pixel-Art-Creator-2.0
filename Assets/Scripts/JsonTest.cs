using PAC.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PAC.Json
{
    public class JsonTest : MonoBehaviour
    {
        private void Start()
        {
            JsonString jString = new JsonString("hello");
            JsonString jString2 = new JsonString("hello");
            Debug.Log(jString == jString2);

            JsonNull jNull = new JsonNull();

            //Debug.Log(int.Parse("3.0"));
            Debug.Log(int.Parse(" 3 "));
            Debug.Log("😀");
            Debug.Log("abcdef"[1..^1]);

            JsonList jList1 = new JsonList(1, 2, 3);
            JsonList jList = new JsonList("hi", "hello");
            JsonObj jObj = new JsonObj
            {
                { "jString", jString },
                { "jString2", jString2 },
                { "jList", jList }
            };

            Debug.Log(jObj.ToJsonString(false));
            Debug.Log(jObj.ToJsonString(true));

            JsonObj jObj2 = new JsonObj
            {
                { "null", new JsonNull() },
                { "bool", new JsonBool(true) },
                { "obj", jObj },
                { "int", new JsonInt(2) },
                { "list", jList },
                { "float", new JsonFloat(-2.68f) },
                { "string", new JsonString("Hi, there") },
            };

            Debug.Log(jObj2.ToJsonString(false));
            Debug.Log(jObj2.ToJsonString(true));


            Debug.Log(new JsonString("hello\" there \u03B5").ToJsonString(false));
            Debug.Log(JsonString.Parse("\"hello there \\u03b5 woah\"").value);
            Debug.Log(JsonString.Parse("\"hello there \\u03b5\"").value);
            Debug.Log(JsonString.Parse("\"hello \\n\\tthere \\n\"").value);

            Debug.Log(JsonNull.Parse("null").ToJsonString(false));
            Debug.Log(JsonBool.Parse("true").value);
            Debug.Log(JsonBool.Parse("false").value);
            Debug.Log(JsonInt.Parse("105").value);
            Debug.Log(JsonInt.Parse("-903008").value);
            Debug.Log(JsonFloat.Parse("-3.25E30").value);
            Debug.Log(JsonData.Parse("40019").ToJsonString(false));

            Debug.Log(JsonList.Parse("[0, 5.32, \"hi there] [\"]").ToJsonString(true));
            Debug.Log(JsonList.Parse("[]").ToJsonString(true));
            Debug.Log(JsonList.Parse("[              ]").ToJsonString(true));
            Debug.Log(JsonList.Parse("[0, 5.32, \"hi there] [\", null]").ToJsonString(true));

            Debug.Log(JsonObj.Parse("{\"id\": 3, \"truth\"  : false }").ToJsonString(true));
            Debug.Log(JsonObj.Parse(jObj2.ToJsonString(true)).ToJsonString(true));
            //Debug.Log(JsonObj.Parse("{\"id\": 3, \"truth\"  : false, \"hello\" }").ToJsonString(true));

            int[] test1 = new int[] { 5, 3, -10, 4 };
            Debug.Log(JsonConverter.ToJson(test1).ToJsonString(false));

            List<int> test2 = new List<int> { 5, 3, -10, 4 };
            Debug.Log(JsonConverter.ToJson(test2).ToJsonString(false));

            /*
            Test test3 = new Test(2, -5.32f, "hi, there!");
            test3.test = test3;
            Debug.Log(JsonConverter.ToJson(test3).ToJsonString(true));
            */

            new JsonList(new int[] { 1, 4, 3, 2 });

            Debug.Log(new JsonFloat(4f).ToJsonString(true));

            Debug.Log(JsonConverter.ToJson(new Vector3(3f, -2.2f, 8f)).ToJsonString(true));
            Debug.Log(JsonConverter.ToJson(new Vector2(3f, -2.2f)).ToJsonString(true));

            Debug.Log(JsonConverter.FromJson<Test>(JsonObj.Parse("{\"integer\": -9, \"flt\": 8.0, \"str\": \"hello\"}")).ToString());

            Debug.Log(string.Join(", ", JsonConverter.FromJson<int[]>(JsonData.Parse("[1, -10, 4, 3]"))));
            Debug.Log(string.Join(",\n", JsonConverter.FromJson<Test[]>(JsonData.Parse(
                "[{\"integer\": -9, \"flt\": 8.0, \"str\": \"hello\"}," +
                "{\"integer\": 2, \"flt\": -0.0, \"str\": \"hi\"}," +
                "{\"integer\": 9, \"flt\": 2.345, \"str\": \"yo\"}]"
                )).Select(x => x.ToString())));
            Debug.Log(string.Join(",\n", JsonConverter.FromJson<List<Test>>(JsonData.Parse(
                "[{\"integer\": -9, \"flt\": 8.0, \"str\": \"hello\"}," +
                "{\"integer\": 2, \"flt\": -0.0, \"str\": \"hi\"}," +
                "{\"integer\": 9, \"flt\": 2.345, \"str\": \"yo\"}]"
                )).Select(x => x.ToString())));
            /*
            Debug.Log(string.Join(",\n", JsonConverter.FromJson<Test[]>(JsonData.Parse(
                "[{\"integer\": -9, \"flt\": 8.0, \"str\": \"hello\"}," +
                "{\"integer\": 2, \"flt\": true, \"str\": \"hi\"}," +
                "{\"integer\": 9, \"flt\": 2.345, \"str\": \"yo\"}]"
                )).Select(x => x.ToString())));
            */

            Test2 test3 = new Test2(5, -4.3f, "name", 7f);
            Debug.Log(typeof(Test2).GetProperty("prop", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).IsAutoProperty());
            Debug.Log(typeof(Test2).GetProperty("prop2", BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).IsAutoProperty());
            Debug.Log(JsonConverter.ToJson(test3).ToJsonString(false));
            Debug.Log(JsonConverter.FromJson<Test2>(JsonConverter.ToJson(test3)).ToString());

            Debug.Log(JsonConverter.ToJson(new Vector3(3f, 2f, 1f), JsonConverters.allConverters).ToJsonString(false));
            Debug.Log(JsonConverter.FromJson<Color>(new JsonList(4f, 2f, 1f, 3f), JsonConverters.allConverters));

            Debug.Log(JsonConverter.ToJson(TestEnum.Test1).ToJsonString(false));
            Debug.Log(JsonConverter.ToJson(TestEnum.Test4).ToJsonString(false));
            Debug.Log(JsonConverter.ToJson(TestEnum.Test2).ToJsonString(false));
            Debug.Log(JsonConverter.FromJson<TestEnum>(JsonString.Parse("\"Test3\"")));
        }

        private class Test
        {
            public int integer;
            public float flt;
            public string str;

            public Test(int integer, float flt, string str)
            {
                this.integer = integer;
                this.flt = flt;
                this.str = str;
            }

            public override string ToString()
            {
                return integer + ", " + flt + ", " + str;
            }
        }

        private class Test2
        {
            public int integer;
            public float flt;
            private string str;
            private float prop { get; set; }
            public float prop2 => flt;

            public Test2(int integer, float flt, string str, float prop)
            {
                this.integer = integer;
                this.flt = flt;
                this.str = str;
                this.prop = prop;
            }

            public override string ToString()
            {
                return integer + ", " + flt + ", " + str + ", " + prop;
            }
        }

        public enum TestEnum
        {
            Test1 = 2,
            Test2 = 5,
            Test3 = -10,
            Test4 = 2
        }
    }
}
