using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JSON
{
    private Dictionary<string, JSONProperty> dictionary = new Dictionary<string, JSONProperty>();

    public JSON() { }

    public JSON(string jsonToParse)
    {
        Parse(jsonToParse);
    }

    public string[] Keys
    {
        get
        {
            List<string> list = new List<string>();
            foreach (string key in dictionary.Keys)
            {
                list.Add(key);
            }
            return list.ToArray();
        }
    }

    public class JSONProperty
    {
        public bool useQuotationMarks;
        public string str;

        public JSONProperty(string str, bool useQuotationMarks)
        {
            this.useQuotationMarks = useQuotationMarks;
            this.str = str;
        }
    }

    public string this[string key]
    {
        get
        {
            return Get(key);
        }

        set
        {
            Add(key, value);
        }
    }

    public string Get(string key)
    {
        if (!dictionary.ContainsKey(key))
        {
            throw new KeyNotFoundException("No key found with name: " + key);
        }
        return dictionary[key].str;
    }

    public JSONProperty GetJSONProperty(string key)
    {
        if (!dictionary.ContainsKey(key))
        {
            throw new KeyNotFoundException("No key found with name: " + key);
        }
        return dictionary[key];
    }

    public void Add(string key, string str)
    {
        Add(key, new JSONProperty(str, true));
    }
    public void Add(string key, string str, bool useQuotationMarks)
    {
        Add(key, new JSONProperty(str, useQuotationMarks));
    }

    public void Add(string key, bool boolean)
    {
        Add(key, new JSONProperty(boolean ? "true" : "false", true));
    }

    public void Add(string key, JSON json)
    {
        Add(key, new JSONProperty(json.ToString(), false));
    }
    public void Add(string key, JSONProperty jsonProperty)
    {
        dictionary[key] = jsonProperty;
    }

    public void Add(string key, object obj)
    {
        Add(key, new JSONProperty(obj.ToString(), false));
    }
    public void Add(string key, object obj, bool useQuotationMarks)
    {
        Add(key, new JSONProperty(obj.ToString(), useQuotationMarks));
    }

    public void Append(JSON json)
    {
        foreach (string key in json.Keys)
        {
            if (dictionary.ContainsKey(key))
            {
                throw new System.Exception("The two JSON objects have a common key: " + key);
            }

            Add(key, json.GetJSONProperty(key));
        }
    }

    public static JSON ParseString(string jsonToParse)
    {
        return new JSON(jsonToParse);
    }
    /// <summary>
    /// Parses the JSON string and adds all data to this JSON object.
    /// </summary>
    /// <param name="jsonToParse"></param>
    public void Parse(string jsonToParse)
    {
        string variable;
        string data;
        int index = 0;

        while (index < jsonToParse.Length)
        {
            variable = "";
            data = "";

            /// Find start of next variable name
            while (jsonToParse[index] != '\"')
            {
                index++;
                if (index >= jsonToParse.Length)
                {
                    return;
                }
            }
            index++;

            /// Read variable name
            while (jsonToParse[index] != '\"')
            {
                variable += jsonToParse[index];
                index++;
            }

            /// Find colon
            while (jsonToParse[index] != ':')
            {
                index++;
            }
            index++;

            /// Find start of data
            while (string.IsNullOrWhiteSpace(jsonToParse[index].ToString()))
            {
                index++;
            }

            /// Read data
            bool inString = false;
            int openCloseBalance = 0;
            do
            {
                if (inString)
                {
                    if (jsonToParse[index] == '\"')
                    {
                        inString = false;
                    }
                }
                else
                {
                    if (jsonToParse[index] == '\"')
                    {
                        inString = true;
                    }
                    else if (jsonToParse[index] == '{' || jsonToParse[index] == '[')
                    {
                        openCloseBalance++;
                    }
                    else if (jsonToParse[index] == '}' || jsonToParse[index] == ']')
                    {
                        openCloseBalance--;
                    }
                }

                /// To catch reaching the end of the JSON (so where there's not a comma to signify the end of the data)
                if (openCloseBalance < 0)
                {
                    break;
                }

                data += jsonToParse[index];
                index++;
            }
            while (!(openCloseBalance == 0 && !inString && jsonToParse[index] == ','));

            data = data.Trim();

            /// Trim start/end "" if data is a string
            if (data[0] == '\"')
            {
                data = data.Remove(0, 1);
                data = data.Remove(data.Length - 1, 1);

                Add(variable, data, true);
            }
            else
            {
                Add(variable, data, false);
            }
        }
    }

    /// <summary>
    /// Splits a JSON string representing an array into a string[] of the data.
    /// </summary>
    /// <param name="jsonString">Include the start/end square brackets in the string.</param>
    /// <returns></returns>
    public static string[] SplitArray(string jsonString)
    {
        List<string> split = new List<string>();

        int index = 1;
        while (string.IsNullOrWhiteSpace(jsonString[index].ToString()))
        {
            index++;
        }

        string data = "";

        bool inString = false;
        int openCloseBalance = 0;
        while (index < jsonString.Length - 1)
        {
            if (inString)
            {
                if (jsonString[index] == '\"')
                {
                    inString = false;
                }
            }
            else
            {
                if (jsonString[index] == '\"')
                {
                    inString = true;
                }
                else if (jsonString[index] == '{' || jsonString[index] == '[' || jsonString[index] == '(')
                {
                    openCloseBalance++;
                }
                else if (jsonString[index] == '}' || jsonString[index] == ']' || jsonString[index] == ')')
                {
                    openCloseBalance--;
                }
            }

            if (jsonString[index] == ',' && !inString && openCloseBalance == 0)
            {
                split.Add(data);
                data = "";
                do
                {
                    index++;
                }
                while (string.IsNullOrWhiteSpace(jsonString[index].ToString()));
            }
            else
            {
                data += jsonString[index];
                index++;
            }
        }
        split.Add(data);

        return split.ToArray();
    }

    public override string ToString()
    {
        string str = "{";

        foreach (string key in dictionary.Keys)
        {
            str += "\n\t\"" + key + "\": ";

            if (dictionary[key].useQuotationMarks)
            {
                str += "\"";
            }

            string[] lines = dictionary[key].str.Split('\n');
            for (int i = 0; i < lines.Length; i++)
            {
                if (i > 0)
                {
                    str += "\t\t";
                }
                str += lines[i] + "\n";
            }

            str = str.Remove(str.Length - 1);

            if (dictionary[key].useQuotationMarks)
            {
                str += "\"";
            }

            str += ",";
        }

        str = str.TrimEnd(',') + "\n}";
        return str;
    }
}
