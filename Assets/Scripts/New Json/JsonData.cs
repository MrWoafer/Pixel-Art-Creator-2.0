using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.Categorization;

namespace PAC.Json
{
    public interface JsonData
    {
        public string ToJsonString(bool pretty);

        public static JsonData Parse(string str)
        {
            try
            {
                int index = 0;
                JsonData jsonData = Parse(str, ref index);

                if (index < str.Length - 1)
                {
                    throw new Exception("Successfully parsed data as type " + jsonData.GetType().Name + " but this did not use the whole input string: " + str);
                }

                return jsonData;
            }
            catch (Exception e)
            {
                throw e;
            }
        }
        /// <summary>
        /// Reads the string, starting at the given index, and attempts to parse the string as JSON data. If successful, the index will be moved to the last character of the data.
        /// </summary>
        public static JsonData Parse(string str, ref int index)
        {
            if (index < 0 || index >= str.Length)
            {
                throw new IndexOutOfRangeException("Index " + index + " out of range string: " + str);
            }

            Exception nullException;
            try
            {
                return JsonNull.Parse(str, ref index);
            }
            catch (Exception e)
            {
                nullException = e;
            }

            Exception boolException;
            try
            {
                return JsonBool.Parse(str, ref index);
            }
            catch (Exception e)
            {
                boolException = e;
            }

            Exception intException;
            try
            {
                return JsonInt.Parse(str, ref index);
            }
            catch (Exception e)
            {
                intException = e;
            }

            Exception floatException;
            try
            {
                return JsonFloat.Parse(str, ref index);
            }
            catch (Exception e)
            {
                floatException = e;
            }

            Exception stringException;
            try
            {
                return JsonString.Parse(str, ref index);
            }
            catch (Exception e)
            {
                stringException = e;
            }

            Exception listException;
            try
            {
                return JsonList.Parse(str, ref index);
            }
            catch (Exception e)
            {
                listException = e;
            }

            Exception objException;
            try
            {
                return JsonObj.Parse(str, ref index);
            }
            catch (Exception e)
            {
                objException = e;
            }

            throw new Exception(
                "Could not parse string: " + str +
                "\nExceptions for each data type:" +
                "\nnull: " + nullException +
                "\nbool: " + boolException +
                "\nint: " + intException +
                "\nfloat: " + floatException +
                "\nstring: " + stringException +
                "\nlist: " + listException +
                "\nobject: " + objException
            );
        }
    }

    public class JsonNull : JsonData
    {
        public static implicit operator string(JsonNull jsonNull)
        {
            return null;
        }
        public static implicit operator JsonList(JsonNull jsonNull)
        {
            return null;
        }
        public static implicit operator JsonObj(JsonNull jsonNull)
        {
            return null;
        }

        public string ToJsonString(bool pretty)
        {
            return "null";
        }

        public static JsonNull Parse(string str)
        {
            int index = 0;
            JsonNull parsed = Parse(str, ref index);

            if (index < str.Length - 1)
            {
                throw new Exception("String ended too soon. null found at index " + index + " in string: " + str);
            }

            return parsed;
        }
        /// <summary>
        /// Reads the string, starting at the given index, and attempts to parse the string as null. If successful, the index will be moved to the last character of the null.
        /// </summary>
        public static JsonNull Parse(string str, ref int index)
        {
            if (index < 0 || index >= str.Length)
            {
                throw new IndexOutOfRangeException("Index " + index + " out of range string: " + str);
            }

            if (index + 3 >= str.Length)
            {
                throw new Exception("null not found at index " + index + " in string: " + str);
            }
            if (str[index..(index + 4)] == "null")
            {
                index += 3;
                return new JsonNull();
            }

            throw new Exception("null not found at index " + index + " in string: " + str);
        }
    }

    public class JsonBool : JsonData
    {
        public bool value { get; set; }

        public JsonBool(bool value)
        {
            this.value = value;
        }

        public static implicit operator JsonBool(bool value)
        {
            return new JsonBool(value);
        }
        public static implicit operator bool(JsonBool jsonBool)
        {
            return jsonBool.value;
        }

        public string ToJsonString(bool pretty)
        {
            return value ? "true" : "false";
        }

        public static JsonBool Parse(string str)
        {
            int index = 0;
            JsonBool parsed = Parse(str, ref index);

            if (index < str.Length - 1)
            {
                throw new Exception("String ended too soon. " + (parsed.value ? "true" : "false") + " found at index " + index + " in string: " + str);
            }

            return parsed;
        }
        /// <summary>
        /// Reads the string, starting at the given index, and attempts to parse the string as null. If successful, the index will be moved to the last character of the true/false.
        /// </summary>
        public static JsonBool Parse(string str, ref int index)
        {
            if (index < 0 || index >= str.Length)
            {
                throw new IndexOutOfRangeException("Index " + index + " out of range string: " + str);
            }

            if (index + 3 >= str.Length)
            {
                throw new Exception("true/false not found at index " + index + " in string: " + str);
            }
            if (str[index..(index + 4)] == "true")
            {
                index += 3;
                return new JsonBool(true);
            }
            if (index + 4 >= str.Length)
            {
                throw new Exception("true/false not found at index " + index + " in string: " + str);
            }
            if (str[index..(index + 5)] == "false")
            {
                index += 4;
                return new JsonBool(true);
            }

            throw new Exception("true/false not found at index " + index + " in string: " + str);
        }
    }

    public class JsonInt : JsonData
    {
        public int value { get; set; }

        public JsonInt(int value)
        {
            this.value = value;
        }

        public static implicit operator JsonInt(int value)
        {
            return new JsonInt(value);
        }
        public static implicit operator int(JsonInt jsonInt)
        {
            return jsonInt.value;
        }

        public static explicit operator JsonInt(float value)
        {
            return new JsonInt((int)value);
        }
        public static implicit operator float(JsonInt jsonInt)
        {
            return jsonInt.value;
        }

        public string ToJsonString(bool pretty)
        {
            return value.ToString();
        }

        public static JsonInt Parse(string str)
        {
            int index = 0;
            JsonInt parsed = Parse(str, ref index);

            if (index < str.Length - 1)
            {
                throw new Exception("String ended too soon. End of int found at index " + index + " in string: " + str);
            }

            return parsed;
        }
        /// <summary>
        /// Reads the string, starting at the given index, and attempts to parse the string as an int. If successful, the index will be moved to the last digit of the int.
        /// </summary>
        public static JsonInt Parse(string str, ref int index)
        {
            if (index < 0 || index >= str.Length)
            {
                throw new IndexOutOfRangeException("Index " + index + " out of range string: " + str);
            }

            int currentIndex = index;
            if (str[index] == '-')
            {
                if (index >= str.Length || !char.IsDigit(str[index + 1]))
                {
                    throw new Exception("Found - followed by no digits at index " + index + " of string: " + str);
                }
                currentIndex++;
            }
            else if (!char.IsDigit(str[index]))
            {
                throw new Exception("Expected - or a digit at index " + index + " of string: " + str);
            }

            while (currentIndex < str.Length)
            {
                if (!char.IsDigit(str[currentIndex]))
                {
                    if (str[currentIndex] == '.')
                    {
                        throw new Exception("Found decimal point at index " + currentIndex + " trying to parse an int from index " + index + " in string: " + str);
                    }

                    if (str[currentIndex] != 'e' && str[currentIndex] != 'E')
                    {
                        break;
                    }

                    // E notation
                    int mantissa = int.Parse(str[index..currentIndex]);
                    int exponentStartIndex = currentIndex + 1;
                    if (exponentStartIndex >= str.Length)
                    {
                        throw new Exception("Found " + str[currentIndex] + " followed by no digits or + at index " + currentIndex + " of string: " + str);
                    }
                    if (str[exponentStartIndex] == '-')
                    {
                        throw new Exception("Integers cnnot have negative exponents. Found - at index " + exponentStartIndex + " of string: " + str);
                    }
                    if (!char.IsDigit(str[exponentStartIndex]) && str[exponentStartIndex] != '+')
                    {
                        throw new Exception("Found " + str[currentIndex] + " followed by no digits or + at index " + currentIndex + " of string: " + str);
                    }
                    if (str[exponentStartIndex] == '+')
                    {
                        if (exponentStartIndex == str.Length - 1 || !char.IsDigit(str[exponentStartIndex + 1]))
                        {
                            throw new Exception("Found + followed by no digits at index " + exponentStartIndex + " of string: " + str);
                        }
                        currentIndex++;
                    }

                    currentIndex++;
                    while (currentIndex < str.Length && char.IsDigit(str[currentIndex]))
                    {
                        currentIndex++;
                    }

                    if (currentIndex < str.Length && str[currentIndex] == '.')
                    {
                        throw new Exception("Found decimal point at index " + currentIndex + " trying to parse an exponent (which must be an integer) " +
                            "for a number starting at index " + index + " in string: " + str);
                    }

                    int exponent = int.Parse(str[exponentStartIndex..currentIndex]);
                    for (int i = 0; i < exponent; i++)
                    {
                        try
                        {
                            mantissa = checked(mantissa * 10);
                        }
                        catch (OverflowException)
                        {
                            throw new Exception("Overflow error when parsing " + str[index..currentIndex] + " at index " + index + " in string: " + str);
                        }
                    }
                    index = currentIndex - 1;
                    return new JsonInt(mantissa);
                }

                currentIndex++;
            }

            int number = int.Parse(str[index..(currentIndex)]);
            index = currentIndex - 1;
            return new JsonInt(number);
        }
    }

    public class JsonFloat : JsonData
    {
        public float value { get; set; }

        public JsonFloat(float value)
        {
            this.value = value;
        }

        public static implicit operator JsonFloat(float value)
        {
            return new JsonFloat(value);
        }
        public static implicit operator float(JsonFloat jsonfloat)
        {
            return jsonfloat.value;
        }

        public static implicit operator JsonFloat(int value)
        {
            return new JsonFloat(value);
        }
        public static explicit operator int(JsonFloat jsonfloat)
        {
            return (int)jsonfloat.value;
        }

        public string ToJsonString(bool pretty)
        {
            // Add .0 if it's an integer
            if (value == Math.Floor(value))
            {
                return value.ToString() + ".0";
            }
            return value.ToString();
        }

        public static JsonFloat Parse(string str)
        {
            int index = 0;
            JsonFloat parsed = Parse(str, ref index);

            if (index < str.Length - 1)
            {
                throw new Exception("String ended too soon. End of float found at index " + index + " in string: " + str);
            }

            return parsed;
        }
        /// <summary>
        /// Reads the string, starting at the given index, and attempts to parse the string as a float. If successful, the index will be moved to the last digit of the float.
        /// </summary>
        public static JsonFloat Parse(string str, ref int index)
        {
            if (index < 0 || index >= str.Length)
            {
                throw new IndexOutOfRangeException("Index " + index + " out of range string: " + str);
            }

            int currentIndex = index;
            if (str[index] == '-')
            {
                if (index >= str.Length || !char.IsDigit(str[index + 1]))
                {
                    throw new Exception("Found - followed by no digits at index " + index + " of string: " + str);
                }
                currentIndex++;
            }
            else if (!char.IsDigit(str[index]))
            {
                throw new Exception("Expected - or a digit at index " + index + " of string: " + str);
            }

            int decimalPointIndex = -1;
            while (currentIndex < str.Length)
            {
                if (!char.IsDigit(str[currentIndex]))
                {
                    // Decimal point
                    if (str[currentIndex] == '.')
                    {
                        if (decimalPointIndex == -1)
                        {
                            decimalPointIndex = currentIndex;
                            currentIndex++;
                            continue;
                        }
                        else
                        {
                            throw new Exception("Found decimal point at index " + currentIndex + " but already found one at index " + decimalPointIndex + " in string: " + str);
                        }
                    }
                    
                    if (str[currentIndex] != 'e' && str[currentIndex] != 'E')
                    {
                        break;
                    }

                    // E notation
                    float mantissa = float.Parse(str[index..currentIndex]);
                    int exponentStartIndex = currentIndex + 1;
                    if (exponentStartIndex >= str.Length && !char.IsDigit(str[exponentStartIndex]) && str[exponentStartIndex] != '+' && str[exponentStartIndex] != '-')
                    {
                        throw new Exception("Found " + str[currentIndex] + " followed by no digits or +/- at index " + currentIndex + " of string: " + str);
                    }
                    if (str[exponentStartIndex] == '+' || str[exponentStartIndex] == '-')
                    {
                        if (exponentStartIndex == str.Length - 1 || !char.IsDigit(str[exponentStartIndex + 1]))
                        {
                            throw new Exception("Found + followed by no digits at index " + exponentStartIndex + " of string: " + str);
                        }
                        currentIndex++;
                    }

                    currentIndex++;
                    while (currentIndex < str.Length && char.IsDigit(str[currentIndex]))
                    {
                        currentIndex++;
                    }

                    if (currentIndex < str.Length && str[currentIndex] == '.')
                    {
                        throw new Exception("Found decimal point at index " + currentIndex + " trying to parse an exponent (which must be an integer) " +
                            "for a number starting at index " + index + " in string: " + str);
                    }

                    float exponent = float.Parse(str[exponentStartIndex..currentIndex]);
                    if (exponent >= 0)
                    {
                        for (int i = 0; i < exponent; i++)
                        {
                            try
                            {
                                mantissa = checked(mantissa * 10f);
                            }
                            catch (OverflowException)
                            {
                                throw new Exception("Overflow error when parsing " + str[index..currentIndex] + " at index " + index + " in string: " + str);
                            }

                            if (float.IsInfinity(mantissa))
                            {
                                throw new Exception("Overflow error when parsing " + str[index..currentIndex] + " at index " + index + " in string: " + str);
                            }
                        }
                    }
                    else
                    {
                        for (int i = 0; i > exponent; i--)
                        {
                            try
                            {
                                mantissa = checked(mantissa / 10f);
                            }
                            catch
                            {
                                throw new Exception("Underflow error when parsing " + str[index..currentIndex] + " at index " + index + " in string: " + str);
                            }
                        }
                    }
                    index = currentIndex - 1;
                    return new JsonFloat(mantissa);
                }

                currentIndex++;
            }

            float number = float.Parse(str[index..(currentIndex)]);
            index = currentIndex - 1;
            return new JsonFloat(number);
        }
    }

    public class JsonString : JsonData
    {
        public string value { get; set; }

        public JsonString(string value)
        {
            this.value = value;
        }

        public static implicit operator JsonString(string value)
        {
            return new JsonString(value);
        }
        public static implicit operator string(JsonString jsonString)
        {
            return jsonString.value;
        }

        public string ToJsonString(bool pretty)
        {
            return "\"" + Escape(value) + "\"";
        }

        private static string Escape(string input)
        {
            StringBuilder escaped = new StringBuilder(input.Length);
            foreach (char chr in input)
            {
                escaped.Append(Escape(chr));
            }
            return escaped.ToString();
        }

        private static string Escape(char chr)
        {
            switch (chr)
            {
                case '\"': return "\\\"";
                case '\\': return @"\\";
                case '\0': return @"\0";
                case '\a': return @"\a";
                case '\b': return @"\b";
                case '\f': return @"\f";
                case '\n': return @"\n";
                case '\r': return @"\r";
                case '\t': return @"\t";
                case '\v': return @"\v"; 
                default:
                    // ASCII printable character
                    if (chr >= 0x20 && chr <= 0x7E)
                    {
                        return chr.ToString();
                        // As UTF16 escaped character
                    }
                    // Unicode characters
                    else
                    {
                        return @"\u" + ((int)chr).ToString("X4");
                    }
            }
        }

        public static JsonString Parse(string str)
        {
            int index = 0;
            JsonString parsed = Parse(str, ref index);

            if (index < str.Length - 1)
            {
                throw new Exception("String ended too soon. Ending \" found at index " + index + " in string: " + str);
            }

            return parsed;
        }
        /// <summary>
        /// Reads the string, starting at the given index, and attempts to parse the string as a JSON string. If successful, the index will be moved to the closing double quotation mark.
        /// </summary>
        public static JsonString Parse(string str, ref int index)
        {
            if (index < 0 || index >= str.Length)
            {
                throw new IndexOutOfRangeException("Index " + index + " out of range string: " + str);
            }

            if (str[index] != '\"')
            {
                throw new Exception("Expected string data to start with \" at index " + index + " in string: " + str);
            }

            StringBuilder parsed = new StringBuilder();
            for (int i = index + 1; i < str.Length; i++)
            {
                // String ending too early
                if (str[i] == '\"')
                {
                    index = i;
                    return parsed.ToString();
                }
                // Non-escaped characters
                else if (str[i] != '\\')
                {
                    parsed.Append(str[i]);
                }
                // Escaped characters
                else
                {
                    if (i == str.Length - 1)
                    {
                        throw new Exception("Found \\ but no following character to escape at the end of string: " + str);
                    }
                    else if (str[i + 1] == 'b')
                    {
                        parsed.Append('\b');
                        i++;
                    }
                    else if (str[i + 1] == 'f')
                    {
                        parsed.Append('\f');
                        i++;
                    }
                    else if (str[i + 1] == 'n')
                    {
                        parsed.AppendLine();
                        i++;
                    }
                    else if (str[i + 1] == 'r')
                    {
                        parsed.Append('\r');
                        i++;
                    }
                    else if (str[i + 1] == 't')
                    {
                        parsed.Append('\t');
                        i++;
                    }
                    else if (str[i + 1] == '\\')
                    {
                        parsed.Append('\\');
                        i++;
                    }
                    else if (str[i + 1] == '/')
                    {
                        parsed.Append('/');
                        i++;
                    }
                    else if (str[i + 1] == '\"')
                    {
                        parsed.Append('\"');
                        i++;
                    }
                    else if (str[i + 1] == 'u')
                    {
                        if (i + 5 >= str.Length)
                        {
                            throw new Exception("Expected 4 hex digits, starting at index " + (i + 2) + ", but only found " + (str.Length - i - 2) + " in string: " + str);
                        }

                        int[] hexDigits = new int[4];
                        for (int digit = 0; digit < 4; digit++)
                        {
                            hexDigits[digit] = str[i + 2 + digit] switch
                            {
                                '0' => 0,
                                '1' => 1,
                                '2' => 2,
                                '3' => 3,
                                '4' => 4,
                                '5' => 5,
                                '6' => 6,
                                '7' => 7,
                                '8' => 8,
                                '9' => 9,
                                'a' => 10,
                                'A' => 10,
                                'b' => 11,
                                'B' => 11,
                                'c' => 12,
                                'C' => 12,
                                'd' => 13,
                                'D' => 13,
                                'e' => 14,
                                'E' => 14,
                                'f' => 15,
                                'F' => 15,
                                _ => throw new Exception("Invalid hex digit " + str[i + 2 + digit] + " at index " + (i + 2 + digit) + " in string: " + str)
                            };
                        }

                        parsed.Append(char.ConvertFromUtf32(4096 * hexDigits[0] + 256 * hexDigits[1] + 16 * hexDigits[2] + hexDigits[3]));
                        i += 5;
                    }
                    else
                    {
                        throw new Exception("Invalid escaped character " + str[i..(i + 2)] + " at index " + i + " in string: " + str);
                    }
                }
            }

            throw new Exception("Reached end of string before finding closing \" for string data starting at index " + index + " in string: " + str);
        }
    }

    public class JsonList : List<JsonData>, JsonData
    {
        public JsonList() : base() { }
        public JsonList(int capacity) : base(capacity) { }
        public JsonList(IEnumerable<JsonData> collection) : base(collection) { }
        public JsonList(params JsonData[] jsonData) : base(jsonData) { }

        public JsonList(params bool[] jsonData) => new JsonList((IEnumerable<bool>)jsonData);
        public JsonList(IEnumerable<bool> collection) : base(collection.Count())
        {
            foreach (JsonBool element in collection)
            {
                Add(new JsonBool(element));
            }
        }

        public JsonList(params int[] jsonData) => new JsonList((IEnumerable<int>)jsonData);
        public JsonList(IEnumerable<int> collection) : base(collection.Count())
        {
            foreach (JsonInt element in collection)
            {
                Add(new JsonInt(element));
            }
        }

        public JsonList(params float[] jsonData) => new JsonList((IEnumerable<float>)jsonData);
        public JsonList(IEnumerable<float> collection) : base(collection.Count())
        {
            foreach (JsonFloat element in collection)
            {
                Add(new JsonFloat(element));
            }
        }

        public JsonList(params string[] jsonData) => new JsonList((IEnumerable<string>)jsonData);
        public JsonList(IEnumerable<string> collection) : base(collection.Count())
        {
            foreach (JsonString element in collection)
            {
                Add(new JsonString(element));
            }
        }

        public string ToJsonString(bool pretty)
        {
            if (Count == 0)
            {
                return "[ ]";
            }

            StringBuilder str = new StringBuilder("[");
            if (pretty)
            {
                str.Append('\n');
            }

            foreach (JsonData jsonData in this)
            {
                if (pretty)
                {
                    string[] lines = jsonData.ToJsonString(pretty).Split('\n');
                    for (int i = 0; i < lines.Length; i++)
                    {
                        if (i > 0)
                        {
                            str.Append('\t');
                        }

                        str.Append("\t" + lines[i]);

                        if (i < lines.Length - 1)
                        {
                            str.Append('\n');
                        }
                    }
                }
                else
                {
                    str.Append(jsonData.ToJsonString(pretty));
                }

                if (pretty)
                {
                    str.Append(",\n");
                }
                else
                {
                    str.Append(", ");
                }
            }

            // Remove ", " from the end of the string
            if (Count > 0)
            {
                str.Remove(str.Length - 2, 2);
            }
            if (pretty)
            {
                str.Append('\n');
            }
            str.Append(']');

            return str.ToString();
        }

        public static JsonList Parse(string str)
        {
            int index = 0;
            JsonList parsed = Parse(str, ref index);

            if (index < str.Length - 1)
            {
                throw new Exception("String ended too soon. Ending ] found at index " + index + " in string: " + str);
            }

            return parsed;
        }
        /// <summary>
        /// Reads the string, starting at the given index, and attempts to parse the string as a list of JSON data. If successful, the index will be moved to the closing square bracket.
        /// </summary>
        public static JsonList Parse(string str, ref int index)
        {
            void SkipWhitespace(string str, ref int index)
            {
                while (index < str.Length && char.IsWhiteSpace(str[index]))
                {
                    index++;
                }
            }

            if (index < 0 || index >= str.Length)
            {
                throw new IndexOutOfRangeException("Index " + index + " out of range string: " + str);
            }

            if (str[index] != '[')
            {
                throw new Exception("Expected list to start with [ at index " + index + " in string: " + str);
            }

            JsonList parsed = new JsonList();

            // Case for empty list
            int currentIndex = index + 1;
            SkipWhitespace(str, ref currentIndex);
            if (currentIndex < str.Length && str[currentIndex] == ']')
            {
                index = currentIndex;
                return parsed;
            }

            // Case for non-empty list
            currentIndex = index + 1;
            while (currentIndex < str.Length)
            {
                // Element
                SkipWhitespace(str, ref currentIndex);

                if (currentIndex >= str.Length)
                {
                    throw new Exception("Reached end of string while expecting element for list starting at index " + index + " in string: " + str);
                }
                if (str[currentIndex] == ']')
                {
                    throw new Exception("Found closing ] at index " + currentIndex + " but was expecting another element due to a comma in string: " + str);
                }

                parsed.Add(JsonData.Parse(str, ref currentIndex));
                currentIndex++;

                // Closing ] or ,
                SkipWhitespace(str, ref currentIndex);

                if (currentIndex >= str.Length)
                {
                    throw new Exception("Reached end of string while expecting ] or , for list starting at index " + index + " in string: " + str);
                }
                if (str[currentIndex] == ']')
                {
                    index = currentIndex;
                    return parsed;
                }
                if (str[currentIndex] != ',')
                {
                    throw new Exception("List has not ended, so expected a comma at index " + currentIndex + " in string: " + str);
                }

                currentIndex++;
            }

            throw new Exception("Reached end of string before finding closing ] for list starting at index " + index + " in string: " + str);
        }
    }

    public class JsonObj : Dictionary<string, JsonData>, JsonData
    {
        public JsonObj() : base() { }
        public JsonObj(int capacity) : base(capacity) { }
        public JsonObj(IEnumerable<KeyValuePair<string, JsonData>> collection) : base(collection) { }
        public JsonObj(IDictionary<string, JsonData> collection) : base(collection) { }

        public void Add(string key, JsonNull value)
        {
            Add(key, (JsonData)value);
        }
        public void Add(string key, JsonBool value)
        {
            Add(key, (JsonData)value);
        }
        public void Add(string key, JsonInt value)
        {
            Add(key, (JsonData)value);
        }
        public void Add(string key, JsonFloat value)
        {
            Add(key, (JsonData)value);
        }
        public void Add(string key, JsonString value)
        {
            Add(key, (JsonData)value);
        }
        public void Add(string key, JsonList value)
        {
            Add(key, (JsonData)value);
        }
        public void Add(string key, JsonObj value)
        {
            Add(key, (JsonData)value);
        }

        public string ToJsonString(bool pretty)
        {
            if (Count == 0)
            {
                return "{ }";
            }

            StringBuilder str = new StringBuilder("{");
            if (pretty)
            {
                str.Append('\n');
            }

            foreach (string key in Keys)
            {
                if (pretty)
                {
                    string[] lines = ("\"" + key + "\"" + ": " + this[key].ToJsonString(pretty)).Split('\n');
                    for (int i = 0; i < lines.Length; i++)
                    {
                        str.Append("\t" + lines[i]);

                        if (i < lines.Length - 1)
                        {
                            str.Append('\n');
                        }
                    }
                }
                else
                {
                    str.Append("\"" + key + "\"" + ": " + this[key].ToJsonString(pretty));
                }

                if (pretty)
                {
                    str.Append(",\n");
                }
                else
                {
                    str.Append(", ");
                }
            }

            // Remove ",\n" or ", " from the end of the string
            if (Count > 0)
            {
                str.Remove(str.Length - 2, 2);
            }
            if (pretty)
            {
                str.Append('\n');
            }
            str.Append('}');

            return str.ToString();
        }

        public static JsonObj Parse(string str)
        {
            int index = 0;
            JsonObj parsed = Parse(str, ref index);

            if (index < str.Length - 1)
            {
                throw new Exception("String ended too soon. Ending } found at index " + index + " in string: " + str);
            }

            return parsed;
        }
        /// <summary>
        /// Reads the string, starting at the given index, and attempts to parse the string as a JSON object. If successful, the index will be moved to the closing curly bracket.
        /// </summary>
        public static JsonObj Parse(string str, ref int index)
        {
            void SkipWhitespace(string str, ref int index)
            {
                while (index < str.Length && char.IsWhiteSpace(str[index]))
                {
                    index++;
                }
            }

            if (index < 0 || index >= str.Length)
            {
                throw new IndexOutOfRangeException("Index " + index + " out of range string: " + str);
            }

            if (str[index] != '{')
            {
                throw new Exception("Expected object to start with { at index " + index + " in string: " + str);
            }

            JsonObj parsed = new JsonObj();

            // Case for empty list
            int currentIndex = index + 1;
            SkipWhitespace(str, ref currentIndex);
            if (currentIndex < str.Length && str[currentIndex] == '}')
            {
                index = currentIndex;
                return parsed;
            }

            // Case for non-empty list
            currentIndex = index + 1;
            while (currentIndex < str.Length)
            {
                // Identifier
                SkipWhitespace(str, ref currentIndex);

                if (currentIndex >= str.Length)
                {
                    throw new Exception("Reached end of string while expecting identifier for list starting at index " + index + " in string: " + str);
                }
                if (str[currentIndex] == '}')
                {
                    throw new Exception("Found closing } at index " + currentIndex + " but was expecting another element due to a comma in string: " + str);
                }

                string identifier = JsonString.Parse(str, ref currentIndex).value;
                currentIndex++;

                if (parsed.ContainsKey(identifier))
                {
                    throw new Exception("Object has already used the identifier " + identifier + " in string: " + str);
                }

                // Colon
                SkipWhitespace(str, ref currentIndex);

                if (currentIndex >= str.Length)
                {
                    throw new Exception("Reached end of string while expecting : for object starting at index " + index + " in string: " + str);
                }
                if (str[currentIndex] == '}')
                {
                    throw new Exception("Reached closing } while expecting : for object starting at index " + index + " in string: " + str);
                }

                if (str[currentIndex] != ':')
                {
                    throw new Exception("Expected : at index " + currentIndex + " in string: " + str);
                }
                currentIndex++;

                // Value
                SkipWhitespace(str, ref currentIndex);

                if (currentIndex >= str.Length)
                {
                    throw new Exception("Reached end of string while expecting value for identifier " + identifier + " in object starting at index " + index + " in string: " + str);
                }
                if (str[currentIndex] == '}')
                {
                    throw new Exception("Reached closing } while expecting value for identifier " + identifier + " in object starting at index " + index + " in string: " + str);
                }

                JsonData value = JsonData.Parse(str, ref currentIndex);
                parsed.Add(identifier, value);
                currentIndex++;

                // Closing } or ,
                SkipWhitespace(str, ref currentIndex);

                if (currentIndex >= str.Length)
                {
                    throw new Exception("Reached end of string while expecting } or , for list starting at index " + index + " in string: " + str);
                }
                if (str[currentIndex] == '}')
                {
                    index = currentIndex;
                    return parsed;
                }
                if (str[currentIndex] != ',')
                {
                    throw new Exception("Object has not ended, so expected a comma at index " + currentIndex + " in string: " + str);
                }

                currentIndex++;
            }

            throw new Exception("Reached end of string before finding closing } for object starting at index " + index + " in string: " + str);
        }
    }
}