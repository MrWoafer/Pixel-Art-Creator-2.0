using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PAC.JSON
{
    /// <summary>
    /// A class to represent data in a JSON format.
    /// </summary>
    public class JSON
    {
        /// <summary>
        /// A key is a variable name and the value is the data for that variable.
        /// </summary>
        private Dictionary<string, JSONProperty> data = new Dictionary<string, JSONProperty>();

        /// <summary>
        /// Create an empty JSON object.
        /// </summary>
        public JSON() { }
        /// <summary>
        /// Parse the JSON-format string into a JSON object.
        /// </summary>
        public JSON(string jsonToParse)
        {
            AddParse(jsonToParse);
        }

        /// <summary>The keys appearing in the outermost scope of the JSON data.</summary>
        public string[] Keys
        {
            get
            {
                List<string> list = new List<string>();
                foreach (string key in data.Keys)
                {
                    list.Add(key);
                }
                return list.ToArray();
            }
        }

        /// <summary>
        /// A class to store an individual piece of JSON data.
        /// </summary>
        public class JSONProperty
        {
            /// <summary>The data.</summary>
            public string data;
            /// <summary>Whether quotation marks should be added around the data - e.g. if the data represents a string.</summary>
            public bool addQuotationMarks;

            /// <param name="addQuotationMarks">Whether quotation marks should be added around the data - e.g. if the data represents a string.</param>
            public JSONProperty(string data, bool addQuotationMarks)
            {
                this.data = data;
                this.addQuotationMarks = addQuotationMarks;
            }
        }

        /// <summary>
        /// Accesses the string form of the data for the given key - the same as Get(key) and Add(key, value). To access the JSONProperty for the key, use GetJSONProperty(key).
        /// </summary>
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

        /// <summary>
        /// Returns true if this JSON object has the given key.
        /// </summary>
        public bool ContainsKey(string key)
        {
            return data.ContainsKey(key);
        }

        /// <summary>
        /// Returns the string form of the data at the given key.
        /// </summary>
        public string Get(string key)
        {
            if (!data.ContainsKey(key))
            {
                throw new KeyNotFoundException("No key found with name: " + key);
            }
            return data[key].data;
        }

        /// <summary>
        /// Returns the JSONProperty at the given key.
        /// </summary>
        public JSONProperty GetJSONProperty(string key)
        {
            if (!data.ContainsKey(key))
            {
                throw new KeyNotFoundException("No key found with name: " + key);
            }
            return data[key];
        }

        /// <summary>
        /// Adds the given data at the given key, overriding any existing data.
        /// </summary>
        public void Add(string key, JSONProperty data)
        {
            this.data[key] = data;
        }

        /// <summary>
        /// Adds the given string to the JSON, adding quotation marks around the string.
        /// </summary>
        public void Add(string key, string str) => Add(key, new JSONProperty(str, true));
        /// <summary>
        /// Adds the given data (given in string form) to the JSON.
        /// </summary>
        public void Add(string key, string str, bool addQuotationMarks) => Add(key, new JSONProperty(str, addQuotationMarks));
        /// <summary>
        /// Adds the given strings to the JSON in the format of a JSON array, adding quotation marks around each string.
        /// </summary>
        /// <param name="separateLines">Whether to start a new line for each element of the array.</param>
        public void Add(string key, IEnumerable<string> strings, bool separateLines) => Add(key, new JSONProperty(ToJSONString(strings, separateLines, true), false));
        /// <summary>
        /// Adds the given data (given in string form) to the JSON in the format of a JSON array.
        /// </summary>
        /// <param name="separateLines">Whether to start a new line for each element of the array.</param>
        /// <param name="addQuotationMarks">Whether to add quotation marks around each element of the array.</param>
        public void Add(string key, IEnumerable<string> strings, bool separateLines, bool addQuotationMarks) => Add(key, new JSONProperty(ToJSONString(strings, separateLines, addQuotationMarks), false));

        /// <summary>
        /// Adds the given bool to the JSON.
        /// </summary>
        public void Add(string key, bool boolean) => Add(key, new JSONProperty(boolean ? "true" : "false", true));

        /// <summary>
        /// Adds the string form of the given JSON to this JSON object.
        /// </summary>
        public void Add(string key, JSON json) => Add(key, new JSONProperty(json.ToString(), false));
        /// <summary>
        /// Adds the string form of the JSON of the given object to this JSON object.
        /// </summary>
        public void Add(string key, IJSONable jsonableObject) => Add(key, jsonableObject.ToJSON());
        /// <summary>
        /// Adds the string forms of the JSON of the given objects to this JSON object in the format of a JSON array.
        /// </summary>
        public void Add(string key, IEnumerable<IJSONable> jsonableObjects) => Add(key, new JSONProperty(ToJSONString(jsonableObjects), false));

        /// <summary>
        /// Adds the string form of the given object to the JSON, adding no quotation marks.
        /// </summary>
        public void Add(string key, object obj) => Add(key, new JSONProperty(obj.ToString(), false));
        /// <summary>
        /// Adds the string form of the given object to the JSON.
        /// </summary>
        public void Add(string key, object obj, bool addQuotationMarks) => Add(key, new JSONProperty(obj.ToString(), addQuotationMarks));
        /// <summary>
        /// Adds the string form of the given objects to the JSON in the format of a JSON array, adding no quotation marks.
        /// </summary>
        /// <param name="separateLines">Whether to start a new line for each element of the array.</param>
        public void Add(string key, IEnumerable objects, bool separateLines) => Add(key, new JSONProperty(ToJSONString(objects, separateLines), false));
        /// <summary>
        /// Adds the string form of the given objects to the JSON in the format of a JSON array.
        /// </summary>
        /// <param name="separateLines">Whether to start a new line for each element of the array.</param>
        /// <param name="addQuotationMarks">Whether to add quotation marks around each element of the array.</param>
        public void Add(string key, IEnumerable objects, bool separateLines, bool addQuotationMarks) => Add(key, new JSONProperty(ToJSONString(objects, separateLines, addQuotationMarks), false));

        /// <summary>
        /// Adds the data of the given JSON object to the end of this JSON object. Throws error if they have a key in common.
        /// </summary>
        public void Append(JSON json)
        {
            foreach (string key in json.Keys)
            {
                if (data.ContainsKey(key))
                {
                    throw new System.Exception("The two JSON objects have a common key: " + key);
                }

                Add(key, json.GetJSONProperty(key));
            }
        }

        /// <summary>
        /// Parses the JSON string into a new JSON object.
        /// </summary>
        public static JSON Parse(string jsonToParse)
        {
            return new JSON(jsonToParse);
        }
        /// <summary>
        /// Parses the JSON string and adds all data to this JSON object. Does not delete any existing data in this object, except when keys collide, in which case new data will override old data.
        /// </summary>
        public void AddParse(string jsonToParse)
        {
            string variable;
            string data;
            int index = 0;

            while (index < jsonToParse.Length)
            {
                variable = "";
                data = "";

                // Find start of next variable name
                while (jsonToParse[index] != '\"')
                {
                    index++;
                    if (index >= jsonToParse.Length)
                    {
                        return;
                    }
                }
                index++;

                // Read variable name
                while (jsonToParse[index] != '\"')
                {
                    variable += jsonToParse[index];
                    index++;
                }

                // Find colon
                while (jsonToParse[index] != ':')
                {
                    index++;
                }
                index++;

                // Find start of data
                while (string.IsNullOrWhiteSpace(jsonToParse[index].ToString()))
                {
                    index++;
                }

                // Read data
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

                    // To catch reaching the end of the JSON (so where there's not a comma to signify the end of the data)
                    if (openCloseBalance < 0)
                    {
                        break;
                    }

                    data += jsonToParse[index];
                    index++;
                }
                while (!(openCloseBalance == 0 && !inString && jsonToParse[index] == ','));

                data = data.Trim();

                // Trim start/end "" if data is a string
                if (data[0] == '\"')
                {
                    data = data.Remove(0, 1);
                    data = data.Remove(data.Length - 1, 1);

                    Add(variable, data, true);
                }
                else
                {
                    if (data == "null")
                    {
                        Add(variable, new JSONProperty(null, false));
                    }
                    else
                    {
                        Add(variable, data, false);
                    }
                }
            }
        }

        /// <summary>
        /// Removes the " " pair enclosing a string, if there is one; otherwise throws an error.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string StripQuotationMarks(string str)
        {
            if (str[0] != '\"' || str[^1] != '\"')
            {
                throw new System.Exception("String is not enclosed in quotation marks: " + str);
            }
            str = str.Remove(0, 1);
            str = str.Remove(str.Length - 1, 1);
            return str;
        }

        /// <summary>
        /// Returns the data as a string in JSON format.
        /// </summary>
        public override string ToString()
        {
            string str = "{";

            foreach (string key in data.Keys)
            {
                str += "\n\t\"" + key + "\": ";

                if (data[key].data == null)
                {
                    str += "null";
                }
                else
                {
                    if (data[key].addQuotationMarks)
                    {
                        str += "\"";
                    }

                    string[] lines = data[key].data.Split('\n');
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (i > 0)
                        {
                            str += "\t\t";
                        }
                        str += lines[i] + "\n";
                    }

                    str = str.Remove(str.Length - 1);

                    if (data[key].addQuotationMarks)
                    {
                        str += "\"";
                    }
                }

                str += ",";
            }

            str = str.TrimEnd(',') + "\n}";
            return str;
        }

        /// <summary>
        /// Returns the JSON of the given objects in a JSON-format array in string form.
        /// </summary>
        public static string ToJSONString(IEnumerable<IJSONable> jsonableObjects)
        {
            return ToJSONString(from obj in jsonableObjects select obj.ToJSON().ToString(), true, false);
        }
        /// <summary>
        /// Returns the JSON of the given objects in a JSON-format array in string form, adding no quotation marks.
        /// </summary>
        public static string ToJSONString(IEnumerable objects, bool separateLines) => ToJSONString(objects, separateLines, false);
        /// <summary>
        /// Returns the JSON of the given objects in a JSON-format array in string form.
        /// </summary>
        /// <param name="addQuotationMarks">Whether to add quotation marks around each element of the array.</param>
        public static string ToJSONString(IEnumerable objects, bool separateLines, bool addQuotationMarks)
        {
            List<string> strings = new List<string>();
            foreach (object obj in objects)
            {
                strings.Add(obj.ToString());
            }
            return ToJSONString(strings, separateLines, addQuotationMarks);
        }
        /// <summary>
        /// Returns the given strings in a JSON-format array in string form, adding quotation marks around each element.
        /// </summary>
        public static string ToJSONString(IEnumerable<string> strings, bool separateLines) => ToJSONString(strings, separateLines, true);
        /// <summary>
        /// Returns the given strings in a JSON-format array in string form.
        /// </summary>
        /// <param name="addQuotationMarks">Whether to add quotation marks around each element of the array.</param>
        public static string ToJSONString(IEnumerable<string> strings, bool separateLines, bool addQuotationMarks)
        {
            string jsonStr = "[";
            bool firstElement = true;
            foreach (string str in strings)
            {
                if (separateLines)
                {
                    jsonStr += "\n";
                }
                else
                {
                    if (firstElement)
                    {
                        firstElement = false;
                    }
                    else
                    {
                        jsonStr += " ";
                    }
                }

                string layerJSON = addQuotationMarks ? "\"" + str + "\"" : str;

                string[] lines = layerJSON.Split('\n');
                foreach (string line in lines)
                {
                    if (separateLines)
                    {
                        jsonStr += "\t" + line + "\n";
                    }
                    else
                    {
                        jsonStr += line + "\n";
                    }
                }

                jsonStr = jsonStr.Remove(jsonStr.Length - 1);

                jsonStr += ",";
            }

            jsonStr = jsonStr.TrimEnd(',') + (separateLines ? "\n]" : "]");
            return jsonStr;
        }

        /// <summary>
        /// Splits a JSON string representing an array into a string[] of the data.
        /// </summary>
        /// <param name="jsonString">Include the start/end square brackets in the string.</param>
        public static string[] SplitArray(string jsonString)
        {
            if (jsonString[0] != '[')
            {
                throw new System.Exception("jsonString must start with [");
            }
            if (jsonString[^1] != ']')
            {
                throw new System.Exception("jsonString must end with ]");
            }

            List<string> split = new List<string>();

            int index = 1;
            while (string.IsNullOrWhiteSpace(jsonString[index].ToString()))
            {
                index++;
            }
            int endIndex = jsonString.Length - 2;
            while (string.IsNullOrWhiteSpace(jsonString[endIndex].ToString()))
            {
                endIndex--;
            }

            // The current element of the array.
            string data = "";

            // Elements of the array may be strings, which could contain commas that we then don't want to split at.
            // This variable keeps track of whether the char we are currently looking at is in a string.
            bool inString = false;
            // Since elements of the array may be arrays, and we are only splitting the outermost array, there may be some commas we don't want to split at.
            // To deal with this, this variable keeps track of whether brackets pairs have been open and closed.
            int openCloseBalance = 0;
            while (index <= endIndex)
            {
                // Check if we are currently in an array element that is a string
                if (inString)
                {
                    // Check if we have reached the end of the string
                    if (jsonString[index] == '\"')
                    {
                        inString = false;
                    }
                }
                else
                {
                    // Check if we are opening a string
                    if (jsonString[index] == '\"')
                    {
                        inString = true;
                    }
                    // Check if we are opening / closing a bracket and adjust the bracket balance variable accordingly.
                    else if (jsonString[index] == '{' || jsonString[index] == '[' || jsonString[index] == '(')
                    {
                        openCloseBalance++;
                    }
                    else if (jsonString[index] == '}' || jsonString[index] == ']' || jsonString[index] == ')')
                    {
                        openCloseBalance--;
                    }
                }

                // Check if we have reached the end of the current array element
                if (jsonString[index] == ',' && !inString && openCloseBalance == 0)
                {
                    // We have reached the last character of the current array element, so we add it to our list.
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
                    // Read the next character of the current array element
                    data += jsonString[index];
                    index++;
                }
            }
            split.Add(data);

            return split.ToArray();
        }
    }
}
