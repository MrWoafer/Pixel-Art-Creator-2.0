using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PAC.Exceptions;

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
        /// Attempts to parse the string as JSON data.
        /// </summary>
        public static JsonData Parse(string str)
        {
            try
            {
                int index = 0;
                JsonData jsonData = Parse(str, ref index);

                if (index < str.Length - 1)
                {
                    throw new FormatException("Successfully parsed data as type " + jsonData.GetType().Name + " but this did not use the whole input string: " + str);
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

            throw new AggregateException("Could not parse string: " + str, nullException, boolException, intException, floatException, stringException, listException, objException);
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
            if (jsonDataType == typeof(JsonNull))
            {
                return true;
            }
            if (jsonDataType == typeof(JsonBool))
            {
                return ((JsonBool)jsonData1).value == ((JsonBool)jsonData2).value;
            }
            if (jsonDataType == typeof(JsonInt))
            {
                return ((JsonInt)jsonData1).value == ((JsonInt)jsonData2).value;
            }
            if (jsonDataType == typeof(JsonFloat))
            {
                return ((JsonFloat)jsonData1).value == ((JsonFloat)jsonData2).value || Math.Abs(((JsonFloat)jsonData1).value - ((JsonFloat)jsonData2).value) <= floatTolerance;
            }
            if (jsonDataType == typeof(JsonString))
            {
                return ((JsonString)jsonData1).value == ((JsonString)jsonData2).value;
            }
            if (jsonDataType == typeof(JsonList))
            {
                JsonList jsonList1 = (JsonList)jsonData1;
                JsonList jsonList2 = (JsonList)jsonData2;
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
            if (jsonDataType == typeof(JsonObj))
            {
                JsonObj jsonObj1 = (JsonObj)jsonData1;
                JsonObj jsonObj2 = (JsonObj)jsonData2;
                if (jsonObj1.Count != jsonObj2.Count)
                {
                    return false;
                }
                foreach (string key in  jsonObj1.Keys)
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
    }

    /// <summary>
    /// Represents a null value in JSON data.
    /// </summary>
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

        /// <summary>
        /// Attempts to parse the string as JSON data.
        /// </summary>
        public static JsonNull Parse(string str)
        {
            int index = 0;
            JsonNull parsed = Parse(str, ref index);

            if (index < str.Length - 1)
            {
                throw new FormatException("String ended too soon. null found at index " + index + " in string: " + str);
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
                throw new FormatException("null not found at index " + index + " in string: " + str);
            }
            if (str[index..(index + 4)] == "null")
            {
                index += 3;
                return new JsonNull();
            }

            throw new FormatException("null not found at index " + index + " in string: " + str);
        }
    }

    /// <summary>
    /// Represents a bool in JSON data.
    /// </summary>
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

        /// <summary>
        /// Attempts to parse the string into a JSON bool.
        /// </summary>
        public static JsonBool Parse(string str)
        {
            int index = 0;
            JsonBool parsed = Parse(str, ref index);

            if (index < str.Length - 1)
            {
                throw new FormatException("String ended too soon. " + (parsed.value ? "true" : "false") + " found at index " + index + " in string: " + str);
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
                throw new FormatException("true/false not found at index " + index + " in string: " + str);
            }
            if (str[index..(index + 4)] == "true")
            {
                index += 3;
                return new JsonBool(true);
            }
            if (index + 4 >= str.Length)
            {
                throw new FormatException("true/false not found at index " + index + " in string: " + str);
            }
            if (str[index..(index + 5)] == "false")
            {
                index += 4;
                return new JsonBool(false);
            }

            throw new FormatException("true/false not found at index " + index + " in string: " + str);
        }
    }

    /// <summary>
    /// Represents an int in JSON data.
    /// </summary>
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

        /// <summary>
        /// Attempts to parse the string into a JSON int.
        /// </summary>
        public static JsonInt Parse(string str)
        {
            int index = 0;
            JsonInt parsed = Parse(str, ref index);

            if (index < str.Length - 1)
            {
                throw new FormatException("String ended too soon. End of int found at index " + index + " in string: " + str);
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
                if (index >= str.Length - 1 || !char.IsDigit(str[index + 1]))
                {
                    throw new FormatException("Found - followed by no digits at index " + index + " of string: " + str);
                }
                currentIndex++;
            }
            else if (!char.IsDigit(str[index]))
            {
                throw new FormatException("Expected - or a digit at index " + index + " of string: " + str);
            }

            while (currentIndex < str.Length)
            {
                if (!char.IsDigit(str[currentIndex]))
                {
                    if (str[currentIndex] == '.')
                    {
                        throw new FormatException("Found decimal point at index " + currentIndex + " trying to parse an int from index " + index + " in string: " + str);
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
                        throw new FormatException("Found " + str[currentIndex] + " followed by no digits or + at index " + currentIndex + " of string: " + str);
                    }
                    if (str[exponentStartIndex] == '-')
                    {
                        throw new FormatException("Integers cnnot have negative exponents. Found - at index " + exponentStartIndex + " of string: " + str);
                    }
                    if (!char.IsDigit(str[exponentStartIndex]) && str[exponentStartIndex] != '+')
                    {
                        throw new FormatException("Found " + str[currentIndex] + " followed by no digits or + at index " + currentIndex + " of string: " + str);
                    }
                    if (str[exponentStartIndex] == '+')
                    {
                        if (exponentStartIndex == str.Length - 1 || !char.IsDigit(str[exponentStartIndex + 1]))
                        {
                            throw new FormatException("Found + followed by no digits at index " + exponentStartIndex + " of string: " + str);
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
                        throw new FormatException("Found decimal point at index " + currentIndex + " trying to parse an exponent (which must be an integer) " +
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
                            throw new OverflowException("Overflow error when parsing " + str[index..currentIndex] + " at index " + index + " in string: " + str);
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

    /// <summary>
    /// Represents a float in JSON data.
    /// </summary>
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

        /// <summary>
        /// Attempts to parse the string into a JSON float.
        /// </summary>
        public static JsonFloat Parse(string str)
        {
            int index = 0;
            JsonFloat parsed = Parse(str, ref index);

            if (index < str.Length - 1)
            {
                throw new FormatException("String ended too soon. End of float found at index " + index + " in string: " + str);
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
                if (index >= str.Length - 1 || !char.IsDigit(str[index + 1]))
                {
                    throw new FormatException("Found - followed by no digits at index " + index + " of string: " + str);
                }
                currentIndex++;
            }
            else if (!char.IsDigit(str[index]))
            {
                throw new FormatException("Expected - or a digit at index " + index + " of string: " + str);
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
                            throw new FormatException("Found decimal point at index " + currentIndex + " but already found one at index " + decimalPointIndex + " in string: " + str);
                        }
                    }
                    
                    if (str[currentIndex] != 'e' && str[currentIndex] != 'E')
                    {
                        break;
                    }

                    // E notation
                    float mantissa = float.Parse(str[index..currentIndex]);
                    int exponentStartIndex = currentIndex + 1;
                    if (exponentStartIndex >= str.Length || (!char.IsDigit(str[exponentStartIndex]) && str[exponentStartIndex] != '+' && str[exponentStartIndex] != '-'))
                    {
                        throw new FormatException("Found " + str[currentIndex] + " followed by no digits or +/- at index " + currentIndex + " of string: " + str);
                    }
                    if (str[exponentStartIndex] == '+' || str[exponentStartIndex] == '-')
                    {
                        if (exponentStartIndex == str.Length - 1 || !char.IsDigit(str[exponentStartIndex + 1]))
                        {
                            throw new FormatException("Found " + str[exponentStartIndex] + " followed by no digits at index " + exponentStartIndex + " of string: " + str);
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
                        throw new FormatException("Found decimal point at index " + currentIndex + " trying to parse an exponent (which must be an integer) " +
                            "for a number starting at index " + index + " in string: " + str);
                    }

                    int exponent = int.Parse(str[exponentStartIndex..currentIndex]);
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
                                throw new OverflowException("Overflow error when parsing " + str[index..currentIndex] + " at index " + index + " in string: " + str);
                            }

                            if (float.IsInfinity(mantissa))
                            {
                                throw new OverflowException("Overflow error when parsing " + str[index..currentIndex] + " at index " + index + " in string: " + str);
                            }
                        }
                    }
                    else
                    {
                        if (mantissa == 0f)
                        {
                            return 0f;
                        }
                        for (int i = 0; i > exponent; i--)
                        {
                            mantissa = mantissa / 10f;
                            if (mantissa == 0f)
                            {
                                throw new UnderflowException("Underflow error when parsing " + str[index..currentIndex] + " at index " + index + " in string: " + str);
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

    /// <summary>
    /// Represents a non-null string in JSON data.
    /// </summary>
    public class JsonString : JsonData
    {
        private string _value;
        public string value
        {
            get => _value;
            set
            {
                if (value == null)
                {
                    throw new ArgumentException("A JsonString cannot hold a null string. Use JsonNull instead.", "value");
                }
                _value = value;
            }
        }

        public JsonString(string value)
        {
            this.value = value;
        }

        /// <summary>
        /// Creates a new JsonString with the given value, unless value is null in which case it returns JsonNull. Useful since JsonString cannot hold null strings.
        /// </summary>
        public static JsonData MaybeNull(string value)
        {
            if (value is null)
            {
                return new JsonNull();
            }
            return new JsonString(value);
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
        /// Attempts to parse the string into a JSON string.
        /// </summary>
        public static JsonString Parse(string str)
        {
            int index = 0;
            JsonString parsed = Parse(str, ref index);

            if (index < str.Length - 1)
            {
                throw new FormatException("String ended too soon. Ending \" found at index " + index + " in string: " + str);
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

            if (index + 3 < str.Length && str[index..(index + 4)] == "null")
            {
                throw new FormatException("Parsed a null string, but a JsonString cannot hold a null string. Use JsonNull.Parse() or JsonString.ParseMaybeNull() instead.");
            }

            if (str[index] != '\"')
            {
                throw new FormatException("Expected string data to start with \" at index " + index + " in string: " + str);
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
                        throw new FormatException("Found \\ but no following character to escape at the end of string: " + str);
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
                        parsed.Append('\n');
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
                            throw new FormatException("Expected 4 hex digits, starting at index " + (i + 2) + ", but only found " + (str.Length - i - 2) + " in string: " + str);
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
                                _ => throw new FormatException("Invalid hex digit " + str[i + 2 + digit] + " at index " + (i + 2 + digit) + " in string: " + str)
                            };
                        }

                        parsed.Append(char.ConvertFromUtf32(4096 * hexDigits[0] + 256 * hexDigits[1] + 16 * hexDigits[2] + hexDigits[3]));
                        i += 5;
                    }
                    else
                    {
                        throw new FormatException("Invalid escaped character " + str[i..(i + 2)] + " at index " + i + " in string: " + str);
                    }
                }
            }

            throw new FormatException("Reached end of string before finding closing \" for string data starting at index " + index + " in string: " + str);
        }

        /// <summary>
        /// Attempts to parse the string into a JSON string, with the potential to parse it into JsonNull.
        /// </summary>
        public static JsonData ParseMaybeNull(string str)
        {
            Exception stringException;
            try
            {
                return JsonString.Parse(str);
            }
            catch (Exception e)
            {
                stringException = e;
            }

            Exception nullException;
            try
            {
                return JsonNull.Parse(str);
            }
            catch (Exception e)
            {
                nullException = e;
            }

            throw new AggregateException("Could not parse string: " + str, nullException, stringException);
        }

        /// <summary>
        /// Attempts to parse the string into a JSON string, with the potential to parse it into JsonNull. If successful, moves the index to the last parsed character.
        /// </summary>
        public static JsonData ParseMaybeNull(string str, ref int index)
        {
            Exception stringException;
            try
            {
                return JsonString.Parse(str, ref index);
            }
            catch (Exception e)
            {
                stringException = e;
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

            throw new AggregateException("Could not parse string: " + str, nullException, stringException);
        }
    }

    // TODO: don't allow circular object references
    /// <summary>
    /// Represents a non-null list/array in JSON data.
    /// </summary>
    public class JsonList : List<JsonData>, JsonData
    {
        public JsonList() : base() { }
        public JsonList(int capacity) : base(capacity) { }
        public JsonList(IEnumerable<JsonData> collection) : base(collection) { }
        public JsonList(params JsonData[] jsonData) : base(jsonData) { }

        public JsonList(params JsonNull[] jsonData) : base(jsonData) { }
        public JsonList(IEnumerable<JsonNull> collection) : base(collection) { }

        public JsonList(params JsonBool[] jsonData) : base(jsonData) { }
        public JsonList(IEnumerable<JsonBool> collection) : base(collection) { }
        public JsonList(params bool[] jsonData) : base(jsonData.Length)
        {
            foreach (bool element in jsonData)
            {
                Add(new JsonBool(element));
            }
        }
        public JsonList(IEnumerable<bool> collection) : base(collection.Count())
        {
            foreach (bool element in collection)
            {
                Add(new JsonBool(element));
            }
        }

        public JsonList(params JsonInt[] jsonData) : base(jsonData) { }
        public JsonList(IEnumerable<JsonInt> collection) : base(collection) { }
        public JsonList(params int[] jsonData) : base(jsonData.Length)
        {
            foreach (int element in jsonData)
            {
                Add(new JsonInt(element));
            }
        }
        public JsonList(IEnumerable<int> collection) : base(collection.Count())
        {
            foreach (int element in collection)
            {
                Add(new JsonInt(element));
            }
        }

        public JsonList(params JsonFloat[] jsonData) : base(jsonData) { }
        public JsonList(IEnumerable<JsonFloat> collection) : base(collection) { }
        public JsonList(params float[] jsonData) : base(jsonData.Length)
        {
            foreach (float element in jsonData)
            {
                Add(new JsonFloat(element));
            }
        }
        public JsonList(IEnumerable<float> collection) : base(collection.Count())
        {
            foreach (float element in collection)
            {
                Add(new JsonFloat(element));
            }
        }

        public JsonList(params JsonString[] jsonData) : base(jsonData) { }
        public JsonList(IEnumerable<JsonString> collection) : base(collection) { }
        public JsonList(params string[] jsonData) : base(jsonData.Length)
        {
            foreach (string element in jsonData)
            {
                Add(new JsonString(element));
            }
        }
        public JsonList(IEnumerable<string> collection) : base(collection.Count())
        {
            foreach (string element in collection)
            {
                Add(new JsonString(element));
            }
        }

        public JsonList(params JsonList[] jsonData) : base(jsonData) { }
        public JsonList(IEnumerable<JsonList> collection) : base(collection) { }

        public JsonList(params JsonObj[] jsonData) : base(jsonData) { }
        public JsonList(IEnumerable<JsonObj> collection) : base(collection) { }

        public void Add(JsonNull jsonData) => Add((JsonData)jsonData);

        public void Add(JsonBool jsonData) => Add((JsonData)jsonData);
        public void Add(bool jsonData) => Add(new JsonBool(jsonData));

        public void Add(JsonInt jsonData) => Add((JsonData)jsonData);
        public void Add(int jsonData) => Add(new JsonInt(jsonData));

        public void Add(JsonFloat jsonData) => Add((JsonData)jsonData);
        public void Add(float jsonData) => Add(new JsonFloat(jsonData));

        public void Add(JsonString jsonData) => Add((JsonData)jsonData);
        public void Add(string jsonData)
        {
            Add(JsonString.MaybeNull(jsonData));
        }

        public void Add(JsonList jsonData) => Add((JsonData)jsonData);

        public void Add(JsonObj jsonData) => Add((JsonData)jsonData);

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

        /// <summary>
        /// Attempts to parse the string into a JSON list.
        /// </summary>
        public static JsonList Parse(string str)
        {
            int index = 0;
            JsonList parsed = Parse(str, ref index);

            if (index < str.Length - 1)
            {
                throw new FormatException("String ended too soon. Ending ] found at index " + index + " in string: " + str);
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

            if (index + 3 < str.Length && str[index..(index + 4)] == "null")
            {
                throw new FormatException("Parsed a null list, but a JsonList cannot hold a null list. Use JsonNull.Parse() or JsonList.ParseMaybeNull() instead.");
            }

            if (str[index] != '[')
            {
                throw new FormatException("Expected list to start with [ at index " + index + " in string: " + str);
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
                    throw new FormatException("Reached end of string while expecting element for list starting at index " + index + " in string: " + str);
                }
                if (str[currentIndex] == ']')
                {
                    throw new FormatException("Found closing ] at index " + currentIndex + " but was expecting another element due to a comma in string: " + str);
                }

                parsed.Add(JsonData.Parse(str, ref currentIndex));
                currentIndex++;

                // Closing ] or ,
                SkipWhitespace(str, ref currentIndex);

                if (currentIndex >= str.Length)
                {
                    throw new FormatException("Reached end of string while expecting ] or , for list starting at index " + index + " in string: " + str);
                }
                if (str[currentIndex] == ']')
                {
                    index = currentIndex;
                    return parsed;
                }
                if (str[currentIndex] != ',')
                {
                    throw new FormatException("List has not ended, so expected a comma at index " + currentIndex + " in string: " + str);
                }

                currentIndex++;
            }

            throw new FormatException("Reached end of string before finding closing ] for list starting at index " + index + " in string: " + str);
        }

        /// <summary>
        /// Attempts to parse the string into a JSON list, with the potential to parse it into JsonNull.
        /// </summary>
        public static JsonData ParseMaybeNull(string str)
        {
            Exception listException;
            try
            {
                return JsonList.Parse(str);
            }
            catch (Exception e)
            {
                listException = e;
            }

            Exception nullException;
            try
            {
                return JsonNull.Parse(str);
            }
            catch (Exception e)
            {
                nullException = e;
            }

            throw new AggregateException("Could not parse string: " + str, nullException, listException);
        }

        /// <summary>
        /// Attempts to parse the string into a JSON list, with the potential to parse it into JsonNull. If successful, moves the index to the last parsed character.
        /// </summary>
        public static JsonData ParseMaybeNull(string str, ref int index)
        {
            Exception listException;
            try
            {
                return JsonList.Parse(str, ref index);
            }
            catch (Exception e)
            {
                listException = e;
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

            throw new AggregateException("Could not parse string: " + str, nullException, listException);
        }
    }

    // TODO: don't allow circular object references
    /// <summary>
    /// Represents a non-null object in JSON data.
    /// </summary>
    public class JsonObj : Dictionary<string, JsonData>, JsonData
    {
        public JsonObj() : base() { }
        public JsonObj(int capacity) : base(capacity) { }
        public JsonObj(IEnumerable<KeyValuePair<string, JsonData>> collection) : base(collection) { }
        public JsonObj(IDictionary<string, JsonData> collection) : base(collection) { }

        public void Add(string key, JsonNull value) => Add(key, (JsonData)value);

        public void Add(string key, JsonBool value) => Add(key, (JsonData)value);
        public void Add(string key, bool value) => Add(key, new JsonBool(value));

        public void Add(string key, JsonInt value) => Add(key, (JsonData)value);
        public void Add(string key, int value) => Add(key, new JsonInt(value));

        public void Add(string key, JsonFloat value) => Add(key, (JsonData)value);
        public void Add(string key, float value) => Add(key, new JsonFloat(value));

        public void Add(string key, JsonString value) => Add(key, (JsonData)value);
        public void Add(string key, string value)
        {
            Add(key, JsonString.MaybeNull(value));
        }

        public void Add(string key, JsonList value) => Add(key, (JsonData)value);

        public void Add(string key, JsonObj value) => Add(key, (JsonData)value);

        /// <summary>
        /// Returns a JSON object with this object's entries, followed by the first objects's entries, then the second objects's entries, etc.
        /// </summary>
        public JsonObj Append(params JsonObj[] jsonObjs) => Append((IEnumerable<JsonObj>)jsonObjs);
        /// <summary>
        /// Returns a JSON object with this object's entries, followed by the first objects's entries, then the second objects's entries, etc.
        /// </summary>
        public JsonObj Append(IEnumerable<JsonObj> jsonObjs)
        {
            return Concat(Enumerable.Concat(new JsonObj[] { this }.AsEnumerable(), jsonObjs));
        }
        /// <summary>
        /// Returns a JSON object with the first objects's entries, then the second objects's entries, etc., then this object's entries.
        /// </summary>
        public JsonObj Prepend(params JsonObj[] jsonObjs) => Prepend((IEnumerable<JsonObj>)jsonObjs);
        /// <summary>
        /// Returns a JSON object with the first objects's entries, then the second objects's entries, etc., then this object's entries.
        /// </summary>
        public JsonObj Prepend(IEnumerable<JsonObj> jsonObjs)
        {
            return Concat(Enumerable.Concat(jsonObjs, new JsonObj[] { this }.AsEnumerable()));
        }
        /// <summary>
        /// Combines the JSON objects into one by putting the first object's entries first, then the second object's entries, etc.
        /// Returns an empty object if no JSON objects are given.
        /// </summary>
        public static JsonObj Concat(params JsonObj[] jsonObjs) => Concat((IEnumerable<JsonObj>)jsonObjs);
        /// <summary>
        /// Combines the JSON objects into one by putting the first object's entries first, then the second object's entries, etc.
        /// Returns an empty object if no JSON objects are given.
        /// </summary>
        public static JsonObj Concat(IEnumerable<JsonObj> jsonObjs)
        {
            JsonObj concat = new JsonObj();
            foreach (JsonObj jsonObj in jsonObjs)
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
        /// Attempts to parse the string into a JSON object.
        /// </summary>
        public static JsonObj Parse(string str)
        {
            int index = 0;
            JsonObj parsed = Parse(str, ref index);

            if (index < str.Length - 1)
            {
                throw new FormatException("String ended too soon. Ending } found at index " + index + " in string: " + str);
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

            if (index + 3 < str.Length && str[index..(index + 4)] == "null")
            {
                throw new FormatException("Parsed a null object, but a JsonObj cannot hold a null object. Use JsonNull.Parse() or JsonObj.ParseMaybeNull() instead.");
            }

            if (str[index] != '{')
            {
                throw new FormatException("Expected object to start with { at index " + index + " in string: " + str);
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
                    throw new FormatException("Reached end of string while expecting identifier for list starting at index " + index + " in string: " + str);
                }
                if (str[currentIndex] == '}')
                {
                    throw new FormatException("Found closing } at index " + currentIndex + " but was expecting another element due to a comma in string: " + str);
                }

                string identifier = JsonString.Parse(str, ref currentIndex).value;
                currentIndex++;

                if (parsed.ContainsKey(identifier))
                {
                    throw new FormatException("Object has already used the identifier " + identifier + " in string: " + str);
                }

                // Colon
                SkipWhitespace(str, ref currentIndex);

                if (currentIndex >= str.Length)
                {
                    throw new FormatException("Reached end of string while expecting : for object starting at index " + index + " in string: " + str);
                }
                if (str[currentIndex] == '}')
                {
                    throw new FormatException("Reached closing } while expecting : for object starting at index " + index + " in string: " + str);
                }

                if (str[currentIndex] != ':')
                {
                    throw new FormatException("Expected : at index " + currentIndex + " in string: " + str);
                }
                currentIndex++;

                // Value
                SkipWhitespace(str, ref currentIndex);

                if (currentIndex >= str.Length)
                {
                    throw new FormatException("Reached end of string while expecting value for identifier " + identifier + " in object starting at index " + index + " in string: " + str);
                }
                if (str[currentIndex] == '}')
                {
                    throw new FormatException("Reached closing } while expecting value for identifier " + identifier + " in object starting at index " + index + " in string: " + str);
                }

                JsonData value = JsonData.Parse(str, ref currentIndex);
                parsed.Add(identifier, value);
                currentIndex++;

                // Closing } or ,
                SkipWhitespace(str, ref currentIndex);

                if (currentIndex >= str.Length)
                {
                    throw new FormatException("Reached end of string while expecting } or , for list starting at index " + index + " in string: " + str);
                }
                if (str[currentIndex] == '}')
                {
                    index = currentIndex;
                    return parsed;
                }
                if (str[currentIndex] != ',')
                {
                    throw new FormatException("Object has not ended, so expected a comma at index " + currentIndex + " in string: " + str);
                }

                currentIndex++;
            }

            throw new FormatException("Reached end of string before finding closing } for object starting at index " + index + " in string: " + str);
        }

        /// <summary>
        /// Attempts to parse the string into a JSON object, with the potential to parse it into JsonNull.
        /// </summary>
        public static JsonData ParseMaybeNull(string str)
        {
            Exception objException;
            try
            {
                return JsonObj.Parse(str);
            }
            catch (Exception e)
            {
                objException = e;
            }

            Exception nullException;
            try
            {
                return JsonNull.Parse(str);
            }
            catch (Exception e)
            {
                nullException = e;
            }

            throw new AggregateException("Could not parse string: " + str, nullException, objException);
        }

        /// <summary>
        /// Attempts to parse the string into a JSON object, with the potential to parse it into JsonNull. If successful, moves the index to the last parsed character.
        /// </summary>
        public static JsonData ParseMaybeNull(string str, ref int index)
        {
            Exception objException;
            try
            {
                return JsonObj.Parse(str, ref index);
            }
            catch (Exception e)
            {
                objException = e;
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

            throw new AggregateException("Could not parse string: " + str, nullException, objException);
        }
    }
}