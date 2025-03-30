using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using PAC.Exceptions;
using PAC.Extensions;
using PAC.Extensions.System.Collections;
using PAC.Extensions.System.IO;

namespace PAC.Json
{
    /// <summary>
    /// Represents a piece of JSON data.
    /// </summary>
    public interface JsonData
    {
        /// <summary>
        /// Converts it into a string in JSON format.
        /// </summary>
        /// <param name="pretty">If true, puts the string into pretty form - e.g. with indentations.</param>
        public string ToJsonString(bool pretty);

        /// <summary>
        /// Attempts to parse the string as JsonData.
        /// </summary>
        public static JsonData Parse(string str)
        {
            if (str is null)
            {
                throw new ArgumentException("Cannot parse a null string.");
            }
            if (str.Length == 0)
            {
                throw new FormatException("Cannot parse an empty string.");
            }

            int index = 0;
            JsonData parsed = Parse(str, ref index);

            if (index < str.Length - 1)
            {
                throw new FormatException("Successfully parsed data as " + parsed.GetType().Name + " but this did not use the whole input string: " + str);
            }

            return parsed;
        }
        /// <summary>
        /// Reads the string, starting at the given index, and attempts to parse the string as JsonData. If successful, the index will be moved to the last character of the data.
        /// </summary>
        public static JsonData Parse(string str, ref int index)
        {
            if (str is null)
            {
                throw new ArgumentException("Cannot parse a null string.");
            }
            if (index < 0 || index >= str.Length)
            {
                throw new IndexOutOfRangeException("Index " + index + " out of range of string: " + str);
            }

            return Parse(new StringReader(str[index..]), ref index, str);
        }
        /// <summary>
        /// Reads the TextReader, starting at the given index, and attempts to parse it as JsonData. If successful, the last read character will be the last character of the parsed JSON.
        /// </summary>
        /// <param name="mustReadAll">If true, it will check that all the data in the reader was used when parsing. Setting this to false is useful for parsing JSON data inside a list or
        /// object.</param>
        public static JsonData Parse(TextReader reader, bool mustReadAll)
        {
            int index = 0;
            JsonData parsed = Parse(reader, ref index, null);

            if (mustReadAll && reader.Peek() != -1)
            {
                throw new FormatException("Successfully parsed data as " + parsed.GetType().Name + " but this did not use the whole reader.");
            }

            return parsed;
        }
        /// <summary>
        /// Used to combine parsing for the cases where the data to parse was originally a string or a TextReader, while still giving more useful exceptions with indices for the string case.
        /// </summary>
        /// <param name="index">Only used for the string case. Keeps track of which index in the string the reader has last read.</param>
        /// <param name="str">Only used for the string case. It is the whole string that is trying to be parsed (stays the same throughout the call stack, with just the index changing).
        /// Set this to null if it's the TextReader case.</param>
        internal static JsonData Parse(TextReader reader, ref int index, string str)
        {
            if (reader.Peek() == -1)
            {
                throw new EndOfStreamException("Given reader has nothing more to read.");
            }

            if (reader.Peek() == 'n')
            {
                return JsonData.Null.Parse(reader, ref index, str);
            }
            if (reader.Peek() == 't' || reader.Peek() == 'f')
            {
                return JsonData.Bool.Parse(reader, ref index, str);
            }
            if (reader.Peek() != -1 && (char.IsDigit((char)reader.Peek()) || reader.Peek() == '-'))
            {
                return JsonData.ParseNumber(reader, ref index, str);
            }
            if (reader.Peek() == '\"')
            {
                return JsonData.String.Parse(reader, ref index, str);
            }
            if (reader.Peek() == '[')
            {
                return JsonData.List.Parse(reader, ref index, str);
            }
            if (reader.Peek() == '{')
            {
                return JsonData.Object.Parse(reader, ref index, str);
            }

            if (str is not null)
            {
                throw new FormatException("Could not parse string from index " + index + " as it did not start with an expected character. String: " + str);
            }
            throw new FormatException("Could not parse data from reader as it did not start with an expected character.");
        }

        /// <summary>
        /// Returns whether the two pieces of JSON data are equal by value rather than whether they point to the same JsonData object.
        /// </summary>
        /// <param name="floatTolerance">
        /// A tolerance (inclusive) to allow close floats to be considered equal.
        /// </param>
        public static bool HaveSameData(JsonData jsonData1, JsonData jsonData2, float floatTolerance = 0f)
        {
            if (floatTolerance < 0f)
            {
                throw new ArgumentException("Cannot have negative float tolerance. Given float tolerance: " + floatTolerance, "floatTolerance");
            }

            if (jsonData1.GetType() != jsonData2.GetType())
            {
                return false;
            }

            Type jsonDataType = jsonData1.GetType();
            if (jsonDataType == typeof(JsonData.Null))
            {
                return true;
            }
            if (jsonDataType == typeof(JsonData.Bool))
            {
                return ((JsonData.Bool)jsonData1).value == ((JsonData.Bool)jsonData2).value;
            }
            if (jsonDataType == typeof(JsonData.Int))
            {
                return ((JsonData.Int)jsonData1).value == ((JsonData.Int)jsonData2).value;
            }
            if (jsonDataType == typeof(JsonData.Float))
            {
                return ((JsonData.Float)jsonData1).value == ((JsonData.Float)jsonData2).value || Math.Abs(((JsonData.Float)jsonData1).value - ((JsonData.Float)jsonData2).value) <= floatTolerance;
            }
            if (jsonDataType == typeof(JsonData.String))
            {
                return ((JsonData.String)jsonData1).value == ((JsonData.String)jsonData2).value;
            }
            if (jsonDataType == typeof(JsonData.List))
            {
                JsonData.List jsonList1 = (JsonData.List)jsonData1;
                JsonData.List jsonList2 = (JsonData.List)jsonData2;
                if (jsonList1.Count != jsonList2.Count)
                {
                    return false;
                }
                for (int i = 0; i < jsonList1.Count; i++)
                {
                    if (!HaveSameData(jsonList1[i], jsonList2[i], floatTolerance))
                    {
                        return false;
                    }
                }
                return true;
            }
            if (jsonDataType == typeof(JsonData.Object))
            {
                JsonData.Object jsonObj1 = (JsonData.Object)jsonData1;
                JsonData.Object jsonObj2 = (JsonData.Object)jsonData2;
                if (jsonObj1.Count != jsonObj2.Count)
                {
                    return false;
                }
                foreach (string key in jsonObj1.Keys)
                {
                    if (!jsonObj2.ContainsKey(key))
                    {
                        return false;
                    }
                    if (!HaveSameData(jsonObj1[key], jsonObj2[key], floatTolerance))
                    {
                        return false;
                    }
                }
                return true;
            }

            throw new ArgumentException("Unknown / unimplemented JSON data type: " + jsonDataType.Name, "jsonData1");
        }


        // JSON data types


        /// <summary>
        /// Represents a null value in JSON data.
        /// </summary>
        public class Null : JsonData
        {
            public Null() { }

            public static implicit operator string(JsonData.Null jsonNull)
            {
                return null;
            }
            public static implicit operator JsonData.List(JsonData.Null jsonNull)
            {
                return null;
            }
            public static implicit operator JsonData.Object(JsonData.Null jsonNull)
            {
                return null;
            }

            public string ToJsonString(bool pretty)
            {
                return "null";
            }

            /// <summary>
            /// Attempts to parse the string as JsonData.Null.
            /// </summary>
            public static JsonData.Null Parse(string str)
            {
                if (str is null)
                {
                    throw new ArgumentException("Cannot parse a null string.");
                }
                if (str.Length == 0)
                {
                    throw new FormatException("Cannot parse an empty string.");
                }

                int index = 0;
                JsonData.Null parsed = Parse(str, ref index);

                if (index < str.Length - 1)
                {
                    throw new FormatException("Successfully parsed data as JsonData.Null but this did not use the whole input string: " + str);
                }

                return parsed;
            }
            /// <summary>
            /// Reads the string, starting at the given index, and attempts to parse the string as JsonData.Null. If successful, the index will be moved to the last character of the null.
            /// </summary>
            public static JsonData.Null Parse(string str, ref int index)
            {
                if (str is null)
                {
                    throw new ArgumentException("Cannot parse a null string.");
                }
                if (index < 0 || index >= str.Length)
                {
                    throw new IndexOutOfRangeException("Index " + index + " out of range of string: " + str);
                }

                return Parse(new StringReader(str[index..]), ref index, str);
            }
            /// <summary>
            /// Reads the TextReader, starting at the given index, and attempts to parse it as JsonData.Null. If successful, the last read character will be the last character of the parsed JSON.
            /// </summary>
            /// <param name="mustReadAll">If true, it will check that all the data in the reader was used when parsing. Setting this to false is useful for parsing JSON data inside a list or
            /// object.</param>
            public static JsonData.Null Parse(TextReader reader, bool mustReadAll)
            {
                int index = 0;
                JsonData.Null parsed = Parse(reader, ref index, null);

                if (mustReadAll && reader.Peek() != -1)
                {
                    throw new FormatException("Successfully parsed data as JsonData.Null but this did not use the whole reader.");
                }

                return parsed;
            }
            /// <summary>
            /// Used to combine parsing for the cases where the data to parse was originally a string or a TextReader, while still giving more useful exceptions with indices for the string case.
            /// </summary>
            /// <param name="index">Only used for the string case. Keeps track of which index in the string the reader has last read.</param>
            /// <param name="str">Only used for the string case. It is the whole string that is trying to be parsed (stays the same throughout the call stack, with just the index changing).
            /// Set this to null if it's the TextReader case.</param>
            internal static JsonData.Null Parse(TextReader reader, ref int index, string str)
            {
                if (reader.Peek() == -1)
                {
                    throw new EndOfStreamException("Given reader has nothing more to read.");
                }

                if (reader.ReadMatchAll("null"))
                {
                    index += 3;
                    return new JsonData.Null();
                }

                if (str is not null)
                {
                    throw new FormatException("null not found at index " + index + " in string: " + str);
                }
                throw new FormatException("null not found at start of reader.");
            }
        }

        /// <summary>
        /// Represents a bool in JSON data.
        /// </summary>
        public class Bool : JsonData
        {
            public bool value { get; set; }

            public Bool(bool value)
            {
                this.value = value;
            }

            public static implicit operator JsonData.Bool(bool value)
            {
                return new JsonData.Bool(value);
            }
            public static implicit operator bool(JsonData.Bool jsonBool)
            {
                return jsonBool.value;
            }

            public string ToJsonString(bool pretty)
            {
                return value ? "true" : "false";
            }

            /// <summary>
            /// Attempts to parse the string into a JsonData.Bool.
            /// </summary>
            public static JsonData.Bool Parse(string str)
            {
                if (str is null)
                {
                    throw new ArgumentException("Cannot parse a null string.");
                }
                if (str.Length == 0)
                {
                    throw new FormatException("Cannot parse an empty string.");
                }

                int index = 0;
                JsonData.Bool parsed = Parse(str, ref index);

                if (index < str.Length - 1)
                {
                    throw new FormatException("Successfully parsed data as JsonData.Bool " + parsed.value + " but this did not use the whole input string: " + str);
                }

                return parsed;
            }
            /// <summary>
            /// Reads the string, starting at the given index, and attempts to parse the string as JsonData.Bool. If successful, the index will be moved to the last character of the null.
            /// </summary>
            public static JsonData.Bool Parse(string str, ref int index)
            {
                if (str is null)
                {
                    throw new ArgumentException("Cannot parse a null string.");
                }
                if (index < 0 || index >= str.Length)
                {
                    throw new IndexOutOfRangeException("Index " + index + " out of range of string: " + str);
                }

                return Parse(new StringReader(str[index..]), ref index, str);
            }
            /// <summary>
            /// Reads the TextReader, starting at the given index, and attempts to parse it as a JsonData.Bool. If successful, the last read character will be the last character of the parsed JSON.
            /// </summary>
            /// <param name="mustReadAll">If true, it will check that all the data in the reader was used when parsing. Setting this to false is useful for parsing JSON data inside a list or
            /// object.</param>
            public static JsonData.Bool Parse(TextReader reader, bool mustReadAll)
            {
                int index = 0;
                JsonData.Bool parsed = Parse(reader, ref index, null);

                if (mustReadAll && reader.Peek() != -1)
                {
                    throw new FormatException("Successfully parsed data as JsonData.Bool " + parsed.value + " but this did not use the whole reader.");
                }

                return parsed;
            }
            /// <summary>
            /// Used to combine parsing for the cases where the data to parse was originally a string or a TextReader, while still giving more useful exceptions with indices for the string case.
            /// </summary>
            /// <param name="index">Only used for the string case. Keeps track of which index in the string the reader has last read.</param>
            /// <param name="str">Only used for the string case. It is the whole string that is trying to be parsed (stays the same throughout the call stack, with just the index changing).
            /// Set this to null if it's the TextReader case.</param>
            internal static JsonData.Bool Parse(TextReader reader, ref int index, string str)
            {
                if (reader.Peek() == -1)
                {
                    throw new EndOfStreamException("Given reader has nothing more to read.");
                }

                if (reader.Peek() == 't' && reader.ReadMatchAll("true"))
                {
                    index += 3;
                    return new JsonData.Bool(true);
                }
                if (reader.Peek() == 'f' && reader.ReadMatchAll("false"))
                {
                    index += 4;
                    return new JsonData.Bool(false);
                }

                if (str is not null)
                {
                    throw new FormatException("true/false not found at index " + index + " in string: " + str);
                }
                throw new FormatException("true/false not found at start of reader.");
            }
        }

        /// <summary>
        /// Represents an int in JSON data.
        /// </summary>
        public class Int : JsonData
        {
            public int value { get; set; }

            public Int(int value)
            {
                this.value = value;
            }

            public static implicit operator JsonData.Int(int value)
            {
                return new JsonData.Int(value);
            }
            public static implicit operator int(JsonData.Int jsonInt)
            {
                return jsonInt.value;
            }

            public static explicit operator JsonData.Int(float value)
            {
                return new JsonData.Int((int)value);
            }
            public static implicit operator float(JsonData.Int jsonInt)
            {
                return jsonInt.value;
            }

            public string ToJsonString(bool pretty)
            {
                return value.ToString();
            }

            /// <summary>
            /// Attempts to parse the string into a JsonData.Int.
            /// </summary>
            public static JsonData.Int Parse(string str)
            {
                if (str is null)
                {
                    throw new ArgumentException("Cannot parse a null string.");
                }
                if (str.Length == 0)
                {
                    throw new FormatException("Cannot parse an empty string.");
                }

                int index = 0;
                JsonData.Int parsed = Parse(str, ref index);

                if (index < str.Length - 1)
                {
                    throw new FormatException("Successfully parsed data as JsonData.Int " + parsed.value + " but this did not use the whole input string: " + str);
                }

                return parsed;
            }
            /// <summary>
            /// Reads the string, starting at the given index, and attempts to parse the string as a JsonData.Int. If successful, the index will be moved to the last digit of the int.
            /// </summary>
            public static JsonData.Int Parse(string str, ref int index)
            {
                if (str is null)
                {
                    throw new ArgumentException("Cannot parse a null string.");
                }
                if (index < 0 || index >= str.Length)
                {
                    throw new IndexOutOfRangeException("Index " + index + " out of range of string: " + str);
                }

                return Parse(new StringReader(str[index..]), ref index, str);
            }
            /// <summary>
            /// Reads the TextReader, starting at the given index, and attempts to parse it as a JsonData.Int. If successful, the last read character will be the last character of the parsed JSON.
            /// </summary>
            /// <param name="mustReadAll">If true, it will check that all the data in the reader was used when parsing. Setting this to false is useful for parsing JSON data inside a list or
            /// object.</param>
            public static JsonData.Int Parse(TextReader reader, bool mustReadAll)
            {
                int index = 0;
                JsonData.Int parsed = Parse(reader, ref index, null);

                if (mustReadAll && reader.Peek() != -1)
                {
                    throw new FormatException("Successfully parsed data as JsonData.Int " + parsed.value + " but this did not use the whole reader.");
                }

                return parsed;
            }
            /// <summary>
            /// Used to combine parsing for the cases where the data to parse was originally a string or a TextReader, while still giving more useful exceptions with indices for the string case.
            /// </summary>
            /// <param name="index">Only used for the string case. Keeps track of which index in the string the reader has last read.</param>
            /// <param name="str">Only used for the string case. It is the whole string that is trying to be parsed (stays the same throughout the call stack, with just the index changing).
            /// Set this to null if it's the TextReader case.</param>
            internal static JsonData.Int Parse(TextReader reader, ref int index, string str)
            {
                int oldIndex = index;
                JsonData parsed = JsonData.ParseNumber(reader, ref index, str);
                if (parsed.GetType() != typeof(JsonData.Int))
                {
                    index = oldIndex;
                    throw new FormatException("Successfully parsed data as JsonData.Float, not JsonData.Int, as there was a decimal point.");
                }
                return (JsonData.Int)parsed;
            }
        }

        /// <summary>
        /// Represents a float in JSON data.
        /// </summary>
        public class Float : JsonData
        {
            public float value { get; set; }

            public Float(float value)
            {
                this.value = value;
            }

            public static implicit operator JsonData.Float(float value)
            {
                return new JsonData.Float(value);
            }
            public static implicit operator float(JsonData.Float jsonfloat)
            {
                return jsonfloat.value;
            }

            public static implicit operator JsonData.Float(int value)
            {
                return new JsonData.Float(value);
            }
            public static explicit operator int(JsonData.Float jsonfloat)
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

            /// <summary>
            /// Attempts to parse the string into a JsonData.Float.
            /// </summary>
            public static JsonData.Float Parse(string str)
            {
                if (str is null)
                {
                    throw new ArgumentException("Cannot parse a null string.");
                }
                if (str.Length == 0)
                {
                    throw new FormatException("Cannot parse an empty string.");
                }

                int index = 0;
                JsonData.Float parsed = Parse(str, ref index);

                if (index < str.Length - 1)
                {
                    throw new FormatException("Successfully parsed data as JsonData.Float " + parsed.value + " but this did not use the whole input string: " + str);
                }

                return parsed;
            }
            /// <summary>
            /// Reads the string, starting at the given index, and attempts to parse the string as a JsonData.Float. If successful, the index will be moved to the last digit of the float.
            /// </summary>
            public static JsonData.Float Parse(string str, ref int index)
            {
                if (str is null)
                {
                    throw new ArgumentException("Cannot parse a null string.");
                }
                if (index < 0 || index >= str.Length)
                {
                    throw new IndexOutOfRangeException("Index " + index + " out of range of string: " + str);
                }

                return Parse(new StringReader(str[index..]), ref index, str);
            }
            /// <summary>
            /// Reads the TextReader, starting at the given index, and attempts to parse it as a JsonData.Float. If successful, the last read character will be the last character of the parsed JSON.
            /// </summary>
            /// <param name="mustReadAll">If true, it will check that all the data in the reader was used when parsing. Setting this to false is useful for parsing JSON data inside a list or
            /// object.</param>
            public static JsonData.Float Parse(TextReader reader, bool mustReadAll)
            {
                int index = 0;
                JsonData.Float parsed = Parse(reader, ref index, null);

                if (mustReadAll && reader.Peek() != -1)
                {
                    throw new FormatException("Successfully parsed data as JsonData.Float " + parsed.value + " but this did not use the whole reader.");
                }

                return parsed;
            }
            /// <summary>
            /// Used to combine parsing for the cases where the data to parse was originally a string or a TextReader, while still giving more useful exceptions with indices for the string case.
            /// </summary>
            /// <param name="index">Only used for the string case. Keeps track of which index in the string the reader has last read.</param>
            /// <param name="str">Only used for the string case. It is the whole string that is trying to be parsed (stays the same throughout the call stack, with just the index changing).
            /// Set this to null if it's the TextReader case.</param>
            internal static JsonData.Float Parse(TextReader reader, ref int index, string str)
            {
                int oldIndex = index;
                JsonData parsed = JsonData.ParseNumber(reader, ref index, str);
                if (parsed.GetType() != typeof(JsonData.Float))
                {
                    index = oldIndex;
                    throw new FormatException("Successfully parsed data as JsonData.Int, not JsonData.Float, as there was no decimal point.");
                }
                return (JsonData.Float)parsed;
            }
        }

        /// <summary>
        /// Attempts to parse the string into a JsonData.Int or JsonData.Float.
        /// </summary>
        public static JsonData ParseNumber(string str)
        {
            if (str is null)
            {
                throw new ArgumentException("Cannot parse a null string.");
            }
            if (str.Length == 0)
            {
                throw new FormatException("Cannot parse an empty string.");
            }

            int index = 0;
            JsonData parsed = ParseNumber(str, ref index);

            if (index < str.Length - 1)
            {
                if (parsed.GetType() == typeof(JsonData.Int))
                {
                    throw new FormatException("Successfully parsed data as JsonData.Int " + ((JsonData.Int)parsed).value + " but this did not use the whole input string: " + str);
                }
                throw new FormatException("Successfully parsed data as JsonData.Float " + ((JsonData.Float)parsed).value + " but this did not use the whole input string: " + str);
            }

            return parsed;
        }
        /// <summary>
        /// Reads the string, starting at the given index, and attempts to parse the string as a JsonData.Int or JsonData.Float. If successful, the index will be moved to the last digit of the number.
        /// </summary>
        public static JsonData ParseNumber(string str, ref int index)
        {
            if (str is null)
            {
                throw new ArgumentException("Cannot parse a null string.");
            }
            if (index < 0 || index >= str.Length)
            {
                throw new IndexOutOfRangeException("Index " + index + " out of range of string: " + str);
            }

            return ParseNumber(new StringReader(str[index..]), ref index, str);
        }
        /// <summary>
        /// Reads the TextReader, starting at the given index, and attempts to parse it as a JsonData.Int or JsonData.Float. If successful, the last read character will be the last character
        /// of the parsed JSON.
        /// </summary>
        /// <param name="mustReadAll">If true, it will check that all the data in the reader was used when parsing. Setting this to false is useful for parsing JSON data inside a list or
        /// object.</param>
        public static JsonData ParseNumber(TextReader reader, bool mustReadAll)
        {
            int index = 0;
            JsonData parsed = ParseNumber(reader, ref index, null);

            if (mustReadAll && reader.Peek() != -1)
            {
                if (parsed.GetType() == typeof(JsonData.Int))
                {
                    throw new FormatException("Successfully parsed data as JsonData.Int " + ((JsonData.Int)parsed).value + " but this did not use the whole reader.");
                }
                throw new FormatException("Successfully parsed data as JsonData.Float " + ((JsonData.Float)parsed).value + " but this did not use the whole reader.");
            }

            return parsed;
        }
        /// <summary>
        /// Used to combine parsing for the cases where the data to parse was originally a string or a TextReader, while still giving more useful exceptions with indices for the string case.
        /// </summary>
        /// <param name="index">Only used for the string case. Keeps track of which index in the string the reader has last read.</param>
        /// <param name="str">Only used for the string case. It is the whole string that is trying to be parsed (stays the same throughout the call stack, with just the index changing).
        /// Set this to null if it's the TextReader case.</param>
        internal static JsonData ParseNumber(TextReader reader, ref int index, string str)
        {
            if (reader.Peek() == -1)
            {
                throw new EndOfStreamException("Given reader has nothing more to read.");
            }

            bool isInt = true;

            string mantissaStr = "";
            // Should always point to the next character we're going to read in str
            int currentIndex = index;
            if (reader.Peek() == '-')
            {
                mantissaStr = "-";
                if (reader.Read() == -1 || !char.IsDigit((char)reader.Peek()))
                {
                    if (str is not null)
                    {
                        throw new FormatException("Found - followed by no digits at index " + index + " of string: " + str);
                    }
                    throw new FormatException("Found - followed by no digits at start of reader.");
                }
                currentIndex++;
            }
            else if (!char.IsDigit((char)reader.Peek()))
            {
                if (str is not null)
                {
                    throw new FormatException("Expected - or a digit at index " + index + " of string: " + str);
                }
                throw new FormatException("Expected - or a digit at start of reader.");
            }

            char chr = (char)0;
            int decimalPointIndex = -1;
            mantissaStr += reader.ReadMatch(IEnumerableExtensions.Repeat<Func<char, bool>>(char.IsDigit));
            currentIndex += mantissaStr.Length - (mantissaStr[0] == '-' ? 1 : 0);

            // Decimal point
            if (reader.Peek() == '.')
            {
                isInt = false;

                reader.Read();
                string decimalPart = reader.ReadMatch(IEnumerableExtensions.Repeat<Func<char, bool>>(char.IsDigit));
                currentIndex += decimalPart.Length + 1;
                mantissaStr += "." + decimalPart;
            }

            // Check for second decimal point
            if (reader.Peek() == '.')
            {
                if (str is not null)
                {
                    throw new FormatException("Found decimal point at index " + currentIndex + " but already found one at index " + decimalPointIndex + " in string: " + str);
                }
                throw new FormatException("Found two decimal points when parsing number.");
            }

            if (reader.Peek() != 'e' && reader.Peek() != 'E')
            {
                index = currentIndex - 1;
                if (isInt)
                {
                    int number = int.Parse(mantissaStr);
                    return new JsonData.Int(number);
                }
                else
                {
                    float number = float.Parse(mantissaStr);
                    return new JsonData.Float(number);
                }
            }

            // E notation

            char e = (char)reader.Read();
            currentIndex++;

            if (reader.Peek() == -1)
            {
                if (str is not null)
                {
                    throw new FormatException("Found " + e + " followed by no digits or +" + (isInt ? "" : "/-") + " at index " + (currentIndex - 1) + " of string: " + str);
                }
                throw new FormatException("Found " + e + " followed by no digits or +" + (isInt ? "" : "/-") + ".");
            }

            if (isInt && reader.Peek() == '-')
            {
                if (str is not null)
                {
                    throw new FormatException("Integers cannot have negative exponents. Found - at index " + (currentIndex - 1) + " of string: " + str);
                }
                throw new FormatException("Integers cannot have negative exponents.");
            }
            if (!char.IsDigit((char)reader.Peek()) && reader.Peek() != '+' && reader.Peek() != '-')
            {
                if (str is not null)
                {
                    throw new FormatException("Found " + e + " followed by no digits or +" + (isInt ? "" : "/-") + " at index " + (currentIndex - 2) + " of string: " + str);
                }
                throw new FormatException("Found " + e + " followed by no digits or +" + (isInt ? "" : "/-") + ".");
            }

            string exponentStr = "";
            if (reader.Peek() == '+' || reader.Peek() == '-')
            {
                chr = (char)reader.Read();
                currentIndex++;
                if (reader.Peek() == -1 || !char.IsDigit((char)reader.Peek()))
                {
                    if (str is not null)
                    {
                        throw new FormatException("Found " + chr + " followed by no digits at index " + (currentIndex - 1) + " of string: " + str);
                    }
                    throw new FormatException("Found " + chr + " followed by no digits in exponent.");
                }

                if (chr == '-')
                {
                    exponentStr += '-';
                }
            }

            exponentStr += reader.ReadMatch(IEnumerableExtensions.Repeat<Func<char, bool>>(char.IsDigit));
            currentIndex += exponentStr.Length - (exponentStr[0] == '-' ? 1 : 0);

            // Decimal point
            if (reader.Peek() == '.')
            {
                if (str is not null)
                {
                    throw new FormatException("Found decimal point at index " + (currentIndex - 1) + " when trying to parse an exponent (which must be an integer) " +
                    "for a number starting at index " + index + " in string: " + str);
                }
                throw new FormatException("Found decimal point when trying to parse an exponent (which must be an integer).");
            }

            // Calculate output

            if (isInt)
            {
                int mantissa = int.Parse(mantissaStr);
                int exponent = int.Parse(exponentStr);
                for (int i = 0; i < exponent; i++)
                {
                    try
                    {
                        mantissa = checked(mantissa * 10);
                    }
                    catch (OverflowException)
                    {
                        if (str is not null)
                        {
                            throw new OverflowException("Overflow error when parsing " + str[index..currentIndex] + " at index " + index + " in string: " + str);
                        }
                        throw new OverflowException("Overflow error when parsing JsonData.Int.");
                    }
                }

                index = currentIndex - 1;
                return new JsonData.Int(mantissa);
            }
            else
            {
                float mantissa = float.Parse(mantissaStr);
                if (mantissa == 0f)
                {
                    return new JsonData.Float(0f);
                }
                if (float.IsNaN(mantissa) || float.IsInfinity(mantissa))
                {
                    if (str is not null)
                    {
                        throw new OverflowException("Overflow error when parsing " + str[index..currentIndex] + " at index " + index + " in string: " + str);
                    }
                    throw new OverflowException("Overflow error when parsing JsonData.Float.");
                }

                int exponent = int.Parse(exponentStr);
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
                            if (str is not null)
                            {
                                throw new OverflowException("Overflow error when parsing " + str[index..currentIndex] + " at index " + index + " in string: " + str);
                            }
                            throw new OverflowException("Overflow error when parsing JsonData.Float.");
                        }

                        if (float.IsInfinity(mantissa))
                        {
                            if (str is not null)
                            {
                                throw new OverflowException("Overflow error when parsing " + str[index..currentIndex] + " at index " + index + " in string: " + str);
                            }
                            throw new OverflowException("Overflow error when parsing JsonData.Float.");
                        }
                    }
                }
                else
                {
                    for (int i = 0; i > exponent; i--)
                    {
                        mantissa = mantissa / 10f;
                        if (mantissa == 0f)
                        {
                            if (str is not null)
                            {
                                throw new UnderflowException("Underflow error when parsing " + str[index..currentIndex] + " at index " + index + " in string: " + str);
                            }
                            throw new UnderflowException("Underflow error when parsing JsonData.Float.");
                        }
                    }
                }

                index = currentIndex - 1;
                return new JsonData.Float(mantissa);
            }
        }

        /// <summary>
        /// Represents a non-null string in JSON data.
        /// </summary>
        public class String : JsonData
        {
            private string _value;
            public string value
            {
                get => _value;
                set
                {
                    if (value == null)
                    {
                        throw new ArgumentException("A JsonData.String cannot hold a null string. Use JsonData.Null instead.", "value");
                    }
                    _value = value;
                }
            }

            public String(string value)
            {
                this.value = value;
            }

            /// <summary>
            /// Creates a new JsonData.String with the given value, unless value is null in which case it returns JsonData.Null. Useful since JsonData.String cannot hold null strings.
            /// </summary>
            public static JsonData MaybeNull(string value)
            {
                if (value is null)
                {
                    return new JsonData.Null();
                }
                return new JsonData.String(value);
            }

            public static implicit operator JsonData.String(string value)
            {
                return new JsonData.String(value);
            }
            public static implicit operator string(JsonData.String jsonString)
            {
                return jsonString.value;
            }

            public string ToJsonString(bool pretty)
            {
                return "\"" + Escape(value) + "\"";
            }

            /// <summary>
            /// Escapes special characters like \n so it will appear as \n when printing the string instead of a new line.
            /// </summary>
            private static string Escape(string input)
            {
                StringBuilder escaped = new StringBuilder(input.Length);
                foreach (char chr in input)
                {
                    escaped.Append(Escape(chr));
                }
                return escaped.ToString();
            }
            /// <summary>
            /// Escapes special characters like \n so it will appear as \n when printing the string instead of a new line.
            /// </summary>
            private static string Escape(char chr)
            {
                switch (chr)
                {
                    case '\"': return "\\\"";
                    case '\\': return @"\\";
                    case '/': return @"\/";
                    case '\b': return @"\b";
                    case '\f': return @"\f";
                    case '\n': return @"\n";
                    case '\r': return @"\r";
                    case '\t': return @"\t";
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

            /// <summary>
            /// Attempts to parse the string into a JsonData.String.
            /// </summary>
            public static JsonData.String Parse(string str)
            {
                if (str is null)
                {
                    throw new ArgumentException("Cannot parse a null string.");
                }
                if (str.Length == 0)
                {
                    throw new FormatException("Cannot parse an empty string.");
                }

                int index = 0;
                JsonData.String parsed = Parse(str, ref index);

                if (index < str.Length - 1)
                {
                    throw new FormatException("Successfully parsed data as JsonData.String " + parsed.value + " but this did not use the whole input string: " + str);
                }

                return parsed;
            }
            /// <summary>
            /// Reads the string, starting at the given index, and attempts to parse the string as a JsonData.String. If successful, the index will be moved to the closing quotation mark.
            /// </summary>
            public static JsonData.String Parse(string str, ref int index)
            {
                if (str is null)
                {
                    throw new ArgumentException("Cannot parse a null string.");
                }
                if (index < 0 || index >= str.Length)
                {
                    throw new IndexOutOfRangeException("Index " + index + " out of range of string: " + str);
                }

                return Parse(new StringReader(str[index..]), ref index, str);
            }
            /// <summary>
            /// Reads the TextReader, starting at the given index, and attempts to parse it as a JsonData.String. If successful, the last read character will be the last character of the parsed JSON.
            /// </summary>
            /// <param name="mustReadAll">If true, it will check that all the data in the reader was used when parsing. Setting this to false is useful for parsing JSON data inside a list or
            /// object.</param>
            public static JsonData.String Parse(TextReader reader, bool mustReadAll)
            {
                int index = 0;
                JsonData.String parsed = Parse(reader, ref index, null);

                if (mustReadAll && reader.Peek() != -1)
                {
                    throw new FormatException("Successfully parsed data as JsonData.String " + parsed.value + " but this did not use the whole reader.");
                }

                return parsed;
            }
            /// <summary>
            /// Used to combine parsing for the cases where the data to parse was originally a string or a TextReader, while still giving more useful exceptions with indices for the string case.
            /// </summary>
            /// <param name="index">Only used for the string case. Keeps track of which index in the string the reader has last read.</param>
            /// <param name="str">Only used for the string case. It is the whole string that is trying to be parsed (stays the same throughout the call stack, with just the index changing).
            /// Set this to null if it's the TextReader case.</param>
            internal static JsonData.String Parse(TextReader reader, ref int index, string str)
            {
                if (reader.Peek() == -1)
                {
                    throw new EndOfStreamException("Given reader has nothing more to read.");
                }

                if (reader.Peek() == 'n' && reader.ReadMatchAll("null"))
                {
                    throw new FormatException("Parsed a null string, but a JsonData.String cannot hold a null string. Use JsonData.Null.Parse() or JsonData.String.ParseMaybeNull() instead.");
                }

                if (reader.Read() != '\"')
                {
                    if (str is not null)
                    {
                        throw new FormatException("Expected string data to start with a quotation mark at index " + index + " in string: " + str);
                    }
                    throw new FormatException("Expected reader data to start with a quotation mark.");
                }

                StringBuilder parsed = new StringBuilder();
                char chr = (char)0;
                // Should always point to the next character we're going to read in str
                int currentIndex = index + 1;
                while (reader.TryReadIntoChar(ref chr))
                {
                    currentIndex++;
                    // Closing quotation mark
                    if (chr == '\"')
                    {
                        index = currentIndex - 1;
                        return parsed.ToString();
                    }
                    // Non-escaped characters
                    else if (chr != '\\')
                    {
                        parsed.Append(chr);
                    }
                    // Escaped characters
                    else
                    {
                        if (!reader.TryReadIntoChar(ref chr))
                        {
                            if (str is not null)
                            {
                                throw new FormatException("Found \\ but no following character to escape at the end of string: " + str);
                            }
                            throw new FormatException("Found \\ but no following character to escape as the reader ended.");
                        }
                        currentIndex++;

                        if (chr == 'b')
                        {
                            parsed.Append('\b');
                        }
                        else if (chr == 'f')
                        {
                            parsed.Append('\f');
                        }
                        else if (chr == 'n')
                        {
                            parsed.Append('\n');
                        }
                        else if (chr == 'r')
                        {
                            parsed.Append('\r');
                        }
                        else if (chr == 't')
                        {
                            parsed.Append('\t');
                        }
                        else if (chr == '\\')
                        {
                            parsed.Append('\\');
                        }
                        else if (chr == '/')
                        {
                            parsed.Append('/');
                        }
                        else if (chr == '\"')
                        {
                            parsed.Append('\"');
                        }
                        else if (chr == 'u')
                        {
                            char[] hexCharSet = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'A', 'b', 'B', 'c', 'C', 'd', 'D', 'e', 'E', 'f', 'F' };
                            string hexChars = reader.ReadMatch(new char[][] { hexCharSet, hexCharSet, hexCharSet, hexCharSet });
                            if (hexChars.Length < 4)
                            {
                                if (str is not null)
                                {
                                    throw new FormatException("Expected 4 hex digits after a \\u, starting at index " + currentIndex + ", but only found " + hexChars.Length + " in string: " + str);
                                }
                                throw new FormatException("Expected 4 hex digits after a \\u, but only found " + hexChars.Length + " in string: " + str);
                            }
                            currentIndex += 4;

                            int[] hexDigits = new int[4];
                            for (int digit = 0; digit < 4; digit++)
                            {
                                hexDigits[digit] = hexChars[digit] switch
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
                                    _ => throw new FormatException("Invalid hex digit " + hexChars[digit])
                                };
                            }

                            parsed.Append(char.ConvertFromUtf32(4096 * hexDigits[0] + 256 * hexDigits[1] + 16 * hexDigits[2] + hexDigits[3]));
                        }
                        else
                        {
                            if (str is not null)
                            {
                                throw new FormatException("Invalid escaped character \\" + chr + " at index " + (currentIndex - 2) + " in string: " + str);
                            }
                            throw new FormatException("Invalid escaped character \\" + chr + ".");
                        }
                    }
                }

                if (str is not null)
                {
                    throw new FormatException("Reached end of string before finding closing quotation mark for string data starting at index " + index + " in string: " + str);
                }
                throw new FormatException("Reached end of reader before finding closing quotation mark for string.");
            }

            /// <summary>
            /// Attempts to parse the string into a JSON string, with the potential to parse it into JsonData.Null.
            /// </summary>
            public static JsonData ParseMaybeNull(string str)
            {
                Exception stringException;
                try
                {
                    return JsonData.String.Parse(str);
                }
                catch (Exception e)
                {
                    stringException = e;
                }

                Exception nullException;
                try
                {
                    return JsonData.Null.Parse(str);
                }
                catch (Exception e)
                {
                    nullException = e;
                }

                throw new AggregateException("Could not parse string: " + str, nullException, stringException);
            }

            /// <summary>
            /// Attempts to parse the string into a JSON string, with the potential to parse it into JsonData.Null. If successful, moves the index to the last parsed character.
            /// </summary>
            public static JsonData ParseMaybeNull(string str, ref int index)
            {
                Exception stringException;
                try
                {
                    return JsonData.String.Parse(str, ref index);
                }
                catch (Exception e)
                {
                    stringException = e;
                }

                Exception nullException;
                try
                {
                    return JsonData.Null.Parse(str, ref index);
                }
                catch (Exception e)
                {
                    nullException = e;
                }

                throw new AggregateException("Could not parse string: " + str, nullException, stringException);
            }
        }

        // TODO: don't allow circular object references
        /// <summary>
        /// Represents a non-null list/array in JSON data.
        /// </summary>
        public class List : List<JsonData>, JsonData
        {
            public bool separateLinesInPrettyString { get; set; } = true;

            public List(bool separateLinesInPrettyString = true) : base()
            {
                this.separateLinesInPrettyString = separateLinesInPrettyString;
            }
            public List(int capacity) : base(capacity) { }
            public List(IEnumerable<JsonData> collection) : base(collection) { }
            public List(params JsonData[] jsonData) : base(jsonData) { }

            public List(params JsonData.Null[] jsonData) : base(jsonData) { }
            public List(IEnumerable<JsonData.Null> collection) : base(collection) { }

            public List(params JsonData.Bool[] jsonData) : base(jsonData) { }
            public List(IEnumerable<JsonData.Bool> collection) : base(collection) { }
            public List(params bool[] jsonData) : base(jsonData.Length)
            {
                foreach (bool element in jsonData)
                {
                    Add(new JsonData.Bool(element));
                }
            }
            public List(IEnumerable<bool> collection) : base(collection.Count())
            {
                foreach (bool element in collection)
                {
                    Add(new JsonData.Bool(element));
                }
            }

            public List(params JsonData.Int[] jsonData) : base(jsonData) { }
            public List(IEnumerable<JsonData.Int> collection) : base(collection) { }
            public List(params int[] jsonData) : base(jsonData.Length)
            {
                foreach (int element in jsonData)
                {
                    Add(new JsonData.Int(element));
                }
            }
            public List(IEnumerable<int> collection) : base(collection.Count())
            {
                foreach (int element in collection)
                {
                    Add(new JsonData.Int(element));
                }
            }

            public List(params JsonData.Float[] jsonData) : base(jsonData) { }
            public List(IEnumerable<JsonData.Float> collection) : base(collection) { }
            public List(params float[] jsonData) : base(jsonData.Length)
            {
                foreach (float element in jsonData)
                {
                    Add(new JsonData.Float(element));
                }
            }
            public List(IEnumerable<float> collection) : base(collection.Count())
            {
                foreach (float element in collection)
                {
                    Add(new JsonData.Float(element));
                }
            }

            public List(params JsonData.String[] jsonData) : base(jsonData) { }
            public List(IEnumerable<JsonData.String> collection) : base(collection) { }
            public List(params string[] jsonData) : base(jsonData.Length)
            {
                foreach (string element in jsonData)
                {
                    Add(new JsonData.String(element));
                }
            }
            public List(IEnumerable<string> collection) : base(collection.Count())
            {
                foreach (string element in collection)
                {
                    Add(new JsonData.String(element));
                }
            }

            public List(params JsonData.List[] jsonData) : base(jsonData) { }
            public List(IEnumerable<JsonData.List> collection) : base(collection) { }

            public List (params JsonData.Object[] jsonData) : base(jsonData) { }
            public List(IEnumerable<JsonData.Object> collection) : base(collection) { }

            public void Add(JsonData.Null jsonData) => Add((JsonData)jsonData);

            public void Add(JsonData.Bool jsonData) => Add((JsonData)jsonData);
            public void Add(bool jsonData) => Add(new JsonData.Bool(jsonData));

            public void Add(JsonData.Int jsonData) => Add((JsonData)jsonData);
            public void Add(int jsonData) => Add(new JsonData.Int(jsonData));

            public void Add(JsonData.Float jsonData) => Add((JsonData)jsonData);
            public void Add(float jsonData) => Add(new JsonData.Float(jsonData));

            public void Add(JsonData.String jsonData) => Add((JsonData)jsonData);
            public void Add(string jsonData)
            {
                Add(JsonData.String.MaybeNull(jsonData));
            }

            public void Add(JsonData.List jsonData) => Add((JsonData)jsonData);

            public void Add(JsonData.Object jsonData) => Add((JsonData)jsonData);

            public string ToJsonString(bool pretty)
            {
                if (Count == 0)
                {
                    if (pretty)
                    {
                        return "[ ]";
                    }
                    return "[]";
                }

                bool separateLines = pretty && separateLinesInPrettyString;

                StringBuilder str = new StringBuilder("[");
                if (separateLines)
                {
                    str.Append('\n');
                }

                foreach (JsonData jsonData in this)
                {
                    if (separateLines)
                    {
                        string[] lines = jsonData.ToJsonString(pretty).Split('\n');
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
                        str.Append(jsonData.ToJsonString(pretty));
                    }

                    if (separateLines)
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
                if (separateLines)
                {
                    str.Append('\n');
                }
                str.Append(']');

                return str.ToString();
            }

            /// <summary>
            /// Attempts to parse the string into a JsonData.List.
            /// </summary>
            public static JsonData.List Parse(string str)
            {
                if (str is null)
                {
                    throw new ArgumentException("Cannot parse a null string.");
                }
                if (str.Length == 0)
                {
                    throw new FormatException("Cannot parse an empty string.");
                }

                int index = 0;
                JsonData.List parsed = Parse(str, ref index);

                if (index < str.Length - 1)
                {
                    throw new FormatException("Successfully parsed data as JsonData.List but this did not use the whole input string: " + str);
                }

                return parsed;
            }
            /// <summary>
            /// Reads the string, starting at the given index, and attempts to parse the string as a JsonData.List. If successful, the index will be moved to the closing square bracket.
            /// </summary>
            public static JsonData.List Parse(string str, ref int index)
            {
                if (str is null)
                {
                    throw new ArgumentException("Cannot parse a null string.");
                }
                if (index < 0 || index >= str.Length)
                {
                    throw new IndexOutOfRangeException("Index " + index + " out of range of string: " + str);
                }

                return Parse(new StringReader(str[index..]), ref index, str);
            }
            /// <summary>
            /// Reads the TextReader, starting at the given index, and attempts to parse it as a JsonData.List. If successful, the last read character will be the last character of the parsed JSON.
            /// </summary>
            /// <param name="mustReadAll">If true, it will check that all the data in the reader was used when parsing. Setting this to false is useful for parsing JSON data inside a list or
            /// object.</param>
            public static JsonData.List Parse(TextReader reader, bool mustReadAll)
            {
                int index = 0;
                JsonData.List parsed = Parse(reader, ref index, null);

                if (mustReadAll && reader.Peek() != -1)
                {
                    throw new FormatException("Successfully parsed data as JsonData.List but this did not use the whole reader.");
                }

                return parsed;
            }
            /// <summary>
            /// Used to combine parsing for the cases where the data to parse was originally a string or a TextReader, while still giving more useful exceptions with indices for the string case.
            /// </summary>
            /// <param name="index">Only used for the string case. Keeps track of which index in the string the reader has last read.</param>
            /// <param name="str">Only used for the string case. It is the whole string that is trying to be parsed (stays the same throughout the call stack, with just the index changing).
            /// Set this to null if it's the TextReader case.</param>
            internal static JsonData.List Parse(TextReader reader, ref int index, string str)
            {
                void SkipWhitespace(TextReader reader, ref int index)
                {
                    int peek = reader.Peek();
                    while (peek != -1 && char.IsWhiteSpace((char)peek))
                    {
                        reader.Read();
                        index++;

                        peek = reader.Peek();
                    }
                }

                if (reader.Peek() == -1)
                {
                    throw new EndOfStreamException("Given reader has nothing more to read.");
                }

                if (reader.Peek() == 'n' && reader.ReadMatchAll("null"))
                {
                    throw new FormatException("Parsed a null string, but a JsonData.String cannot hold a null string. Use JsonData.Null.Parse() or JsonData.List.ParseMaybeNull() instead.");
                }

                if (reader.Read() != '[')
                {
                    if (str is not null)
                    {
                        throw new FormatException("Expected string data to start with [ at index " + index + " in string: " + str);
                    }
                    throw new FormatException("Expected reader data to start with [.");
                }

                JsonData.List parsed = new JsonData.List();
                char chr = (char)0;
                // Should always point to the next character we're going to read in str
                int currentIndex = index + 1;

                // Case for empty list
                SkipWhitespace(reader, ref currentIndex);
                if (reader.Peek() == -1)
                {
                    if (str is not null)
                    {
                        throw new FormatException("Reached end of string while expecting element or closing ] for list starting at index " + index + " in string: " + str);
                    }
                    throw new FormatException("Reached end of reader while expecting element or closing ] for list.");
                }
                if (reader.Peek() == ']')
                {
                    reader.Read();
                    currentIndex++;

                    index = currentIndex - 1;
                    return parsed;
                }

                // Case for non-empty list
                while (reader.Peek() != -1)
                {
                    // Element

                    SkipWhitespace(reader, ref currentIndex);

                    if (reader.Peek() == -1)
                    {
                        if (str is not null)
                        {
                            throw new FormatException("Reached end of string while expecting element for list starting at index " + index + " in string: " + str);
                        }
                        throw new FormatException("Reached end of reader while expecting element for list.");
                    }
                    if (reader.Peek() == ']')
                    {
                        if (str is not null)
                        {
                            throw new FormatException("Found closing ] at index " + (currentIndex - 1) + " but was expecting another element due to a comma in string: " + str);
                        }
                        throw new FormatException("Found closing ] but was expecting another element due to a comma.");
                    }

                    parsed.Add(JsonData.Parse(reader, ref currentIndex, str));
                    currentIndex++;

                    // Closing ] or ,

                    SkipWhitespace(reader, ref currentIndex);

                    if (!reader.TryReadIntoChar(ref chr))
                    {
                        if (str is not null)
                        {
                            throw new FormatException("Reached end of string while expecting a comma or closing ] for list starting at index " + index + " in string: " + str);
                        }
                        throw new FormatException("Reached end of reader while expecting a comma or closing ] for list.");
                    }
                    currentIndex++;

                    if (chr == ']')
                    {
                        index = currentIndex - 1;
                        return parsed;
                    }
                    if (chr != ',')
                    {
                        if (str is not null)
                        {
                            throw new FormatException("List has not ended, so expected a comma at index " + currentIndex + ", but found " + chr + " in string: " + str);
                        }
                        throw new FormatException("List has not ended, so expected a comma, but found " + chr);
                    }
                }

                if (str is not null)
                {
                    throw new FormatException("Reached end of string before finding closing ] for list starting at index " + index + " in string: " + str);
                }
                throw new FormatException("Reached end of reader before finding closing ] for list.");
            }

            /// <summary>
            /// Attempts to parse the string into a JSON list, with the potential to parse it into JsonData.Null.
            /// </summary>
            public static JsonData ParseMaybeNull(string str)
            {
                Exception listException;
                try
                {
                    return JsonData.List.Parse(str);
                }
                catch (Exception e)
                {
                    listException = e;
                }

                Exception nullException;
                try
                {
                    return JsonData.Null.Parse(str);
                }
                catch (Exception e)
                {
                    nullException = e;
                }

                throw new AggregateException("Could not parse string: " + str, nullException, listException);
            }

            /// <summary>
            /// Attempts to parse the string into a JSON list, with the potential to parse it into JsonData.Null. If successful, moves the index to the last parsed character.
            /// </summary>
            public static JsonData ParseMaybeNull(string str, ref int index)
            {
                Exception listException;
                try
                {
                    return JsonData.List.Parse(str, ref index);
                }
                catch (Exception e)
                {
                    listException = e;
                }

                Exception nullException;
                try
                {
                    return JsonData.Null.Parse(str, ref index);
                }
                catch (Exception e)
                {
                    nullException = e;
                }

                throw new AggregateException("Could not parse string: " + str, nullException, listException);
            }
        }

        // TODO: don't allow circular object references
        /// <summary>
        /// Represents a non-null object in JSON data.
        /// </summary>
        public class Object : Dictionary<string, JsonData>, JsonData
        {
            public Object() : base() { }
            public Object(int capacity) : base(capacity) { }
            public Object(IEnumerable<KeyValuePair<string, JsonData>> collection) : base(collection) { }
            public Object(IDictionary<string, JsonData> collection) : base(collection) { }

            public void Add(string key, JsonData.Null value) => Add(key, (JsonData)value);

            public void Add(string key, JsonData.Bool value) => Add(key, (JsonData)value);
            public void Add(string key, bool value) => Add(key, new JsonData.Bool(value));

            public void Add(string key, JsonData.Int value) => Add(key, (JsonData)value);
            public void Add(string key, int value) => Add(key, new JsonData.Int(value));

            public void Add(string key, JsonData.Float value) => Add(key, (JsonData)value);
            public void Add(string key, float value) => Add(key, new JsonData.Float(value));

            public void Add(string key, JsonData.String value) => Add(key, (JsonData)value);
            public void Add(string key, string value)
            {
                Add(key, JsonData.String.MaybeNull(value));
            }

            public void Add(string key, JsonData.List value) => Add(key, (JsonData)value);

            public void Add(string key, JsonData.Object value) => Add(key, (JsonData)value);

            /// <summary>
            /// Returns a JSON object with this object's entries, followed by the first objects's entries, then the second objects's entries, etc.
            /// </summary>
            public JsonData.Object Append(params JsonData.Object[] jsonObjs) => Append((IEnumerable<JsonData.Object>)jsonObjs);
            /// <summary>
            /// Returns a JSON object with this object's entries, followed by the first objects's entries, then the second objects's entries, etc.
            /// </summary>
            public JsonData.Object Append(IEnumerable<JsonData.Object> jsonObjs)
            {
                return Concat(Enumerable.Concat(new JsonData.Object[] { this }.AsEnumerable(), jsonObjs));
            }
            /// <summary>
            /// Returns a JSON object with the first objects's entries, then the second objects's entries, etc., then this object's entries.
            /// </summary>
            public JsonData.Object Prepend(params JsonData.Object[] jsonObjs) => Prepend((IEnumerable<JsonData.Object>)jsonObjs);
            /// <summary>
            /// Returns a JSON object with the first objects's entries, then the second objects's entries, etc., then this object's entries.
            /// </summary>
            public JsonData.Object Prepend(IEnumerable<JsonData.Object> jsonObjs)
            {
                return Concat(Enumerable.Concat(jsonObjs, new JsonData.Object[] { this }.AsEnumerable()));
            }
            /// <summary>
            /// Combines the JSON objects into one by putting the first object's entries first, then the second object's entries, etc.
            /// Returns an empty object if no JSON objects are given.
            /// </summary>
            public static JsonData.Object Concat(params JsonData.Object[] jsonObjs) => Concat((IEnumerable<JsonData.Object>)jsonObjs);
            /// <summary>
            /// Combines the JSON objects into one by putting the first object's entries first, then the second object's entries, etc.
            /// Returns an empty object if no JSON objects are given.
            /// </summary>
            public static JsonData.Object Concat(IEnumerable<JsonData.Object> jsonObjs)
            {
                JsonData.Object concat = new JsonData.Object();
                foreach (JsonData.Object jsonObj in jsonObjs)
                {
                    foreach (string key in jsonObj.Keys)
                    {
                        concat.Add(key, jsonObj[key]);
                    }
                }
                return concat;
            }

            public string ToJsonString(bool pretty)
            {
                if (Count == 0)
                {
                    if (pretty)
                    {
                        return "{ }";
                    }
                    return "{}";
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

            /// <summary>
            /// Attempts to parse the string into a JsonData.Object.
            /// </summary>
            public static JsonData.Object Parse(string str)
            {
                if (str is null)
                {
                    throw new ArgumentException("Cannot parse a null string.");
                }
                if (str.Length == 0)
                {
                    throw new FormatException("Cannot parse an empty string.");
                }

                int index = 0;
                JsonData.Object parsed = Parse(str, ref index);

                if (index < str.Length - 1)
                {
                    throw new FormatException("Successfully parsed data as JsonData.Object but this did not use the whole input string: " + str);
                }

                return parsed;
            }
            /// <summary>
            /// Reads the string, starting at the given index, and attempts to parse the string as a JsonData.Object. If successful, the index will be moved to the closing curly bracket.
            /// </summary>
            public static JsonData.Object Parse(string str, ref int index)
            {
                if (str is null)
                {
                    throw new ArgumentException("Cannot parse a null string.");
                }
                if (index < 0 || index >= str.Length)
                {
                    throw new IndexOutOfRangeException("Index " + index + " out of range of string: " + str);
                }

                return Parse(new StringReader(str[index..]), ref index, str);
            }
            /// <summary>
            /// Reads the TextReader, starting at the given index, and attempts to parse it as a JsonData.Object. If successful, the last read character will be the last character of the parsed JSON.
            /// </summary>
            /// <param name="mustReadAll">If true, it will check that all the data in the reader was used when parsing. Setting this to false is useful for parsing JSON data inside a list or
            /// object.</param>
            public static JsonData.Object Parse(TextReader reader, bool mustReadAll)
            {
                int index = 0;
                JsonData.Object parsed = Parse(reader, ref index, null);

                if (mustReadAll && reader.Peek() != -1)
                {
                    throw new FormatException("Successfully parsed data as JsonData.Object but this did not use the whole reader.");
                }

                return parsed;
            }
            /// <summary>
            /// Used to combine parsing for the cases where the data to parse was originally a string or a TextReader, while still giving more useful exceptions with indices for the string case.
            /// </summary>
            /// <param name="index">Only used for the string case. Keeps track of which index in the string the reader has last read.</param>
            /// <param name="str">Only used for the string case. It is the whole string that is trying to be parsed (stays the same throughout the call stack, with just the index changing).
            /// Set this to null if it's the TextReader case.</param>
            internal static JsonData.Object Parse(TextReader reader, ref int index, string str)
            {
                void SkipWhitespace(TextReader reader, ref int index)
                {
                    int peek = reader.Peek();
                    while (peek != -1 && char.IsWhiteSpace((char)peek))
                    {
                        reader.Read();
                        index++;

                        peek = reader.Peek();
                    }
                }

                if (reader.Peek() == -1)
                {
                    throw new EndOfStreamException("Given reader has nothing more to read.");
                }

                if (reader.Peek() == 'n' && reader.ReadMatchAll("null"))
                {
                    throw new FormatException("Parsed a null string, but a JsonData.String cannot hold a null string. Use JsonData.Null.Parse() or JsonData.List.ParseMaybeNull() instead.");
                }

                if (reader.Read() != '{')
                {
                    if (str is not null)
                    {
                        throw new FormatException("Expected string data to start with { at index " + index + " in string: " + str);
                    }
                    throw new FormatException("Expected reader data to start with {.");
                }

                JsonData.Object parsed = new JsonData.Object();
                char chr = (char)0;
                // Should always point to the next character we're going to read in str
                int currentIndex = index + 1;

                // Case for empty object
                SkipWhitespace(reader, ref currentIndex);
                if (reader.Peek() == -1)
                {
                    if (str is not null)
                    {
                        throw new FormatException("Reached end of string while expecting element or closing } for object starting at index " + index + " in string: " + str);
                    }
                    throw new FormatException("Reached end of reader while expecting element or closing } for object.");
                }
                if (reader.Peek() == '}')
                {
                    reader.Read();
                    currentIndex++;

                    index = currentIndex - 1;
                    return parsed;
                }

                // Case for non-empty object
                while (reader.Peek() != -1)
                {
                    // Identifier

                    SkipWhitespace(reader, ref currentIndex);

                    if (reader.Peek() == -1)
                    {
                        if (str is not null)
                        {
                            throw new FormatException("Reached end of string while expecting identifier in object starting at index " + index + " in string: " + str);
                        }
                        throw new FormatException("Reached end of reader while expecting identifier in object.");
                    }
                    if (reader.Peek() == '}')
                    {
                        if (str is not null)
                        {
                            throw new FormatException("Found closing } at index " + currentIndex + " but was expecting another element due to a comma in string: " + str);
                        }
                        throw new FormatException("Found closing } but was expecting another element due to a comma.");
                    }

                    string identifier = JsonData.String.Parse(reader, ref currentIndex, str).value;
                    currentIndex++;

                    if (parsed.ContainsKey(identifier))
                    {
                        if (str is not null)
                        {
                            throw new FormatException("Object has already used the identifier " + identifier + " in string: " + str);
                        }
                        throw new FormatException("Object has already used the identifier " + identifier);
                    }

                    // Colon

                    SkipWhitespace(reader, ref currentIndex);

                    if (!reader.TryReadIntoChar(ref chr))
                    {
                        if (str is not null)
                        {
                            throw new FormatException("Reached end of string while expecting : for object starting at index " + index + " in string: " + str);
                        }
                        throw new FormatException("Reached end of string while expecting : for object starting at index " + index + " in string: " + str);
                    }
                    currentIndex++;

                    if (chr == '}')
                    {
                        if (str is not null)
                        {
                            throw new FormatException("Reached closing } while expecting a colon for object starting at index " + index + " in string: " + str);
                        }
                        throw new FormatException("Reached closing } while expecting a colon.");
                    }
                    if (chr != ':')
                    {
                        if (str is not null)
                        {
                            throw new FormatException("Expected a colon at index " + (currentIndex - 1) + ", but found " + chr + " in string: " + str);
                        }
                        throw new FormatException("Expected a colon, but found " + chr);
                    }

                    // Value

                    SkipWhitespace(reader, ref currentIndex);

                    if (reader.Peek() == -1)
                    {
                        if (str is not null)
                        {
                            throw new FormatException("Reached end of string while expecting value for identifier " + identifier + " in object starting at index " + index + " in string: " + str);
                        }
                        throw new FormatException("Reached end of string while expecting value for identifier.");
                    }
                    if (reader.Peek() == '}')
                    {
                        if (str is not null)
                        {
                            throw new FormatException("Reached closing } while expecting value for identifier " + identifier + " in object starting at index " + index + " in string: " + str);
                        }
                        throw new FormatException("Reached closing } while expecting value for identifier.");
                    }

                    JsonData value = JsonData.Parse(reader, ref currentIndex, str);
                    currentIndex++;
                    parsed.Add(identifier, value);

                    // Closing } or ,

                    SkipWhitespace(reader, ref currentIndex);

                    if (!reader.TryReadIntoChar(ref chr))
                    {
                        if (str is not null)
                        {
                            throw new FormatException("Reached end of string while expecting } or a comma for object starting at index " + index + " in string: " + str);
                        }
                        throw new FormatException("Reached end of string while expecting } or a comma.");
                    }
                    currentIndex++;

                    if (chr == '}')
                    {
                        index = currentIndex - 1;
                        return parsed;
                    }
                    if (chr != ',')
                    {
                        if (str is not null)
                        {
                            throw new FormatException("Object has not ended, so expected a comma at index " + (currentIndex - 1) + ", but found + " + chr + " in string: " + str);
                        }
                        throw new FormatException("Object has not ended, so expected a comma, but found " + chr);
                    }
                }

                if (str is not null)
                {
                    throw new FormatException("Reached end of string before finding closing } for object starting at index " + index + " in string: " + str);
                }
                throw new FormatException("Reached end of reader before finding closing }.");
            }

            /// <summary>
            /// Attempts to parse the string into a JSON object, with the potential to parse it into JsonData.Null.
            /// </summary>
            public static JsonData ParseMaybeNull(string str)
            {
                Exception objException;
                try
                {
                    return JsonData.Object.Parse(str);
                }
                catch (Exception e)
                {
                    objException = e;
                }

                Exception nullException;
                try
                {
                    return JsonData.Null.Parse(str);
                }
                catch (Exception e)
                {
                    nullException = e;
                }

                throw new AggregateException("Could not parse string: " + str, nullException, objException);
            }

            /// <summary>
            /// Attempts to parse the string into a JSON object, with the potential to parse it into JsonData.Null. If successful, moves the index to the last parsed character.
            /// </summary>
            public static JsonData ParseMaybeNull(string str, ref int index)
            {
                Exception objException;
                try
                {
                    return JsonData.Object.Parse(str, ref index);
                }
                catch (Exception e)
                {
                    objException = e;
                }

                Exception nullException;
                try
                {
                    return JsonData.Null.Parse(str, ref index);
                }
                catch (Exception e)
                {
                    nullException = e;
                }

                throw new AggregateException("Could not parse string: " + str, nullException, objException);
            }
        }
    }
}