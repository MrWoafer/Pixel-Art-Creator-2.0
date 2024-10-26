using Codice.Client.BaseCommands;
using PAC.Json;
using System.Collections.Generic;
using UnityEngine;

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
    }
}
