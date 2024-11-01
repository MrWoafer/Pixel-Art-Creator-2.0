using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Codice.CM.Common;
using PAC.Extensions;
using UnityEngine;

namespace PAC.Json
{
    public abstract class IJsonConverter<T, JsonDataType> where JsonDataType : JsonData
    {
        public abstract JsonData ToJson(T obj);
        public abstract T FromJson(JsonDataType jsonData);
        public T FromJson(JsonData jsonData)
        {
            if (jsonData.GetType() == typeof(JsonDataType))
            {
                return FromJson((JsonDataType)jsonData);
            }
            throw new Exception("Expected the JSON data to be of type " + typeof(JsonDataType).Name + " but found type " + jsonData.GetType().Name);
        }
    }

    public class JsonConverterList : IEnumerable
    {
        public List<object> converters = new List<object>();

        public JsonConverterList() { }
        public JsonConverterList(params object[] converters)
        {
            foreach (object converter in converters)
            {
                Add(converter);
            }
        }

        public void Add(object converter)
        {
            Type type = converter.GetType();
            if (type.IsSubclassOfRawGeneric(typeof(IJsonConverter<,>)))
            {
                Type converterType = type.GetTypeOfRawGenericSuperclass(typeof(IJsonConverter<,>)).GetGenericArguments()[0];
                foreach (object existingConverter in converters)
                {
                    Type existingConverterType = existingConverter.GetType().GetTypeOfRawGenericSuperclass(typeof(IJsonConverter<,>)).GetGenericArguments()[0];
                    if (converterType == existingConverterType)
                    {
                        throw new Exception("The list already contains a converter for type " + converterType);
                    }
                }
                converters.Add(converter);
            }
            else
            {
                throw new Exception("JsonConverterList can only add objects that implement IJsonConverter<,>. The provided object was of type " + type.Name);
            }
        }

        public bool Remove(object converter)
        {
            return converters.Remove(converter);
        }
        public bool Remove(Type converterType)
        {
            for (int i = 0; i < converters.Count; i++)
            {
                Type existingConverterType = converters[i].GetType().GetTypeOfRawGenericSuperclass(typeof(IJsonConverter<,>)).GetGenericArguments()[0];
                if (converterType == existingConverterType)
                {
                    converters.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }

        public IEnumerator GetEnumerator()
        {
            return converters.GetEnumerator();
        }
    }

    public static class JsonConverter
    {
        public static JsonData ToJson(object obj, bool allowUndefinedConversions = false) => ToJson(obj, new JsonConverterList(), allowUndefinedConversions);
        public static JsonData ToJson(object obj, JsonConverterList customConverters, bool allowUndefinedConversions = false)
        {
            return ToJson(obj, new HashSet<object>(), customConverters, allowUndefinedConversions);
        }
        private static JsonData ToJson(object obj, HashSet<object> objectsAlreadyTryingToConvert, JsonConverterList customConverters, bool allowUndefinedConversions)
        {
            if (obj == null)
            {
                return new JsonNull();
            }

            Type objType = obj.GetType();

            // Custom JSON converters
            foreach (object converter in customConverters)
            {
                Type asIJsonConverter = converter.GetType().GetTypeOfRawGenericSuperclass(typeof(IJsonConverter<,>));
                Type conversionType = asIJsonConverter.GetGenericArguments()[0];
                if (conversionType == objType)
                {
                    MethodInfo toJsonMethod = converter.GetType().GetMethod("ToJson", new Type[] { conversionType });
                    return (JsonData)toJsonMethod.Invoke(converter, new object[] { obj });
                }
            }

            // Primitive types
            if (objType == typeof(bool))
            {
                return new JsonBool((bool)obj);
            }
            if (objType == typeof(int))
            {
                return new JsonInt((int)obj);
            }
            if (objType == typeof(float))
            {
                return new JsonFloat((float)obj);
            }
            if (objType == typeof(string))
            {
                return new JsonString((string)obj);
            }

            // Arrays / Lists
            if (objType.IsArray)
            {
                Array objArray = (Array)obj;
                JsonList jsonList = new JsonList(objArray.Length);
                foreach (object element in objArray)
                {
                    jsonList.Add(ToJson(element, customConverters, allowUndefinedConversions));
                }
                return jsonList;
            }
            if (objType.IsGenericList())
            {
                IList objList = (IList)obj;
                JsonList jsonList = new JsonList(objList.Count);
                foreach (object element in objList)
                {
                    jsonList.Add(ToJson(element, customConverters, allowUndefinedConversions));
                }
                return jsonList;
            }

            // Enums
            if (objType.IsEnum)
            {
                return new JsonString(Enum.GetName(objType, obj));
            }

            // Classes / Structs
            if (objType.IsClass || objType.IsStruct())
            {
                if (!allowUndefinedConversions)
                {
                    throw new Exception("The conversion for type " + objType.Name + " is undefined, but parameter allowUndefinedConversions = false");
                }

                JsonObj jsonObj = new JsonObj();

                // Fields
                IEnumerable<FieldInfo> fields = objType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).Where(f => f.GetCustomAttribute<CompilerGeneratedAttribute>() == null);
                foreach (FieldInfo field in fields)
                {
                    object fieldValue = field.GetValue(obj);

                    // Check for circular object references
                    if (objectsAlreadyTryingToConvert.Contains(fieldValue))
                    {
                        throw new Exception("Cannot convert objects with circular object references; consider writing your own method. The circular reference is in type " +
                            field.FieldType.Name + " which occurs in the field " + field.Name + " of type " + objType.Name);
                    }
                    objectsAlreadyTryingToConvert.Add(fieldValue);

                    jsonObj.Add(field.Name, ToJson(fieldValue, objectsAlreadyTryingToConvert, customConverters, allowUndefinedConversions));
                    objectsAlreadyTryingToConvert.Remove(fieldValue);
                }

                // Auto Properties
                IEnumerable<PropertyInfo> autoProperties = objType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).Where(p => p.IsAutoProperty());
                foreach (PropertyInfo property in autoProperties)
                {
                    object propertyValue = property.GetValue(obj);

                    // Check for circular object references
                    if (objectsAlreadyTryingToConvert.Contains(propertyValue))
                    {
                        throw new Exception("Cannot convert objects with circular object references; consider writing your own method. The circular reference is in type " +
                            property.PropertyType.Name + " which occurs in the auto property " + property.Name + " of type " + objType.Name);
                    }
                    objectsAlreadyTryingToConvert.Add(propertyValue);

                    jsonObj.Add(property.Name, ToJson(propertyValue, objectsAlreadyTryingToConvert, customConverters, allowUndefinedConversions));
                    objectsAlreadyTryingToConvert.Remove(propertyValue);
                }

                return jsonObj;
            }

            throw new Exception("Could not convert object of type " + objType.Name + " to JSON.");
        }

        /// <summary>
        /// Attempts to convert the JSON data into a C# object of the given type.
        /// </summary>
        /// <typeparam name="T">The type to convert the JSON into.</typeparam>
        /// <param name="jsonData">The JSON data to convert.</param>
        /// <param name="allowUndefinedConversions">
        /// If false, an exception will be thrown if the code encounters a type that can not be converted using conversions for primitive JSON types and/or custom converters.
        /// </param>
        public static T FromJson<T>(JsonData jsonData, bool allowUndefinedConversions = false) => FromJson<T>(jsonData, new JsonConverterList(), allowUndefinedConversions);
        /// <summary>
        /// Attempts to convert the JSON data into a C# object of the given type.
        /// </summary>
        /// <typeparam name="T">The type to convert the JSON into.</typeparam>
        /// <param name="jsonData">The JSON data to convert.</param>
        /// <param name="customConverters">A collection of IJsonConverter objects that defined custom conversions for certain data types.</param>
        /// <param name="allowUndefinedConversions">
        /// If false, an exception will be thrown if the code encounters a type that can not be converted using conversions for primitive JSON types and/or custom converters.
        /// </param>
        public static T FromJson<T>(JsonData jsonData, JsonConverterList customConverters, bool allowUndefinedConversions = false)
        {
            Type returnType = typeof(T);
            Type jsonDataType = jsonData.GetType();

            // Custom JSON converters
            foreach (object converter in customConverters)
            {
                Type asIJsonConverter = converter.GetType().GetTypeOfRawGenericSuperclass(typeof(IJsonConverter<,>));
                Type conversionType = asIJsonConverter.GetGenericArguments()[0];
                if (conversionType == returnType)
                {
                    MethodInfo fromJsonMethod = converter.GetType().GetMethod("FromJson", new Type[] { typeof(JsonData) });
                    return (T)fromJsonMethod.Invoke(converter, new object[] { jsonData });
                }
            }

            // Enums
            if (returnType.IsEnum)
            {
                if (jsonDataType != typeof(JsonString))
                {
                    throw new Exception("Expected string JSON data to convert into enum type " + returnType.Name + " but found type " + jsonDataType.Name);
                }
                return (T)Enum.Parse(returnType, ((JsonString)jsonData).value);
            }

            // Primitives
            if (jsonDataType == typeof(JsonNull))
            {
                if (returnType.IsValueType)
                {
                    throw new Exception("Cannot convert null JSON data into type " + returnType.Name + " which is non-nullable.");
                }
                return (T)(object)null;
            }
            if (jsonDataType == typeof(JsonBool))
            {
                if (returnType != typeof(bool))
                {
                    throw new Exception("Cannot convert bool JSON data into type " + returnType.Name + " as it is not a bool.");
                }
                return (T)(object)((JsonBool)jsonData).value;
            }
            if (jsonDataType == typeof(JsonInt))
            {
                if (returnType != typeof(int))
                {
                    throw new Exception("Cannot convert int JSON data into type " + returnType.Name + " as it is not a int.");
                }
                return (T)(object)((JsonInt)jsonData).value;
            }
            if (jsonDataType == typeof(JsonFloat))
            {
                if (returnType != typeof(float))
                {
                    throw new Exception("Cannot convert float JSON data into type " + returnType.Name + " as it is not a float.");
                }
                return (T)(object)((JsonFloat)jsonData).value;
            }
            if (jsonDataType == typeof(JsonString))
            {
                if (returnType != typeof(string))
                {
                    throw new Exception("Cannot convert string JSON data into type " + returnType.Name + " as it is not a string.");
                }
                return (T)(object)((JsonString)jsonData).value;
            }

            // Arrays / Lists
            if (jsonDataType == typeof(JsonList))
            {
                if (!returnType.IsArray && !returnType.IsGenericList())
                {
                    throw new Exception("Cannot convert list JSON data into type " + returnType.Name + " as it is not a list or array.");
                }

                JsonList jsonList = (JsonList)jsonData;

                // Array
                if (returnType.IsArray)
                {
                    Type elementType = returnType.GetElementType();
                    MethodInfo genericMethod = typeof(JsonConverter).GetMethod("FromJson", new Type[] { typeof(JsonData), typeof(JsonConverterList), typeof(bool) }).MakeGenericMethod(elementType);

                    Array array = Array.CreateInstance(elementType, jsonList.Count);
                    for (int i = 0; i < jsonList.Count; i++)
                    {
                        object value;
                        try
                        {
                            value = genericMethod.Invoke(null, new object[] { jsonList[i], customConverters, allowUndefinedConversions });
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Could not convert " + jsonList[i].GetType().Name + " to type " + elementType.Name + " for index " + i + " in array." + ". Exception: " + e);
                        }

                        array.SetValue(value, i);
                    }

                    return (T)(object)array;
                }
                // List
                else
                {
                    Type elementType = returnType.GetGenericArguments()[0];
                    MethodInfo genericMethod = typeof(JsonConverter).GetMethod("FromJson", new Type[] { typeof(JsonData), typeof(JsonConverterList), typeof(bool) }).MakeGenericMethod(elementType);

                    IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType), new object[] { jsonList.Count });
                    for (int i = 0; i < jsonList.Count; i++)
                    {
                        object value;
                        try
                        {
                            value = genericMethod.Invoke(null, new object[] { jsonList[i], customConverters, allowUndefinedConversions });
                        }
                        catch (Exception e)
                        {
                            throw new Exception("Could not convert " + jsonList[i].GetType().Name + " to type " + elementType.Name + " for index " + i + " in list." + ". Exception: " + e);
                        }

                        list.Add(value);
                    }

                    return (T)(object)list;
                }
            }

            // Classes / Structs
            if (!allowUndefinedConversions)
            {
                throw new Exception("The conversion for type " + returnType.Name + " is undefined, but parameter allowUndefinedConversions = false");
            }
            if (jsonDataType == typeof(JsonObj))
            {
                return FromJsonUndefined<T>((JsonObj)jsonData, customConverters);
            }

            throw new Exception("Cannot convert JSON data of type " + jsonDataType.Name + " into type " + returnType.Name);
        }
        /// <summary>
        /// Attempts to convert the JSON data into a C# object of the given type. The object will be created using reflection; no custom converter will be used for the object, but custom
        /// converters will be used for converting the values of its fields/properties.
        /// </summary>
        /// <typeparam name="T">The type to convert the JSON into.</typeparam>
        /// <param name="jsonObj">The JSON object to convert.</param>
        /// <param name="customConverters">A collection of IJsonConverter objects that defined custom conversions for certain data types.</param>
        private static T FromJsonUndefined<T>(JsonObj jsonObj, JsonConverterList customConverters)
        {
            Type returnType = typeof(T);
            T obj = (T)FormatterServices.GetSafeUninitializedObject(returnType);

            // Fields
            IEnumerable<FieldInfo> fields = returnType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).Where(f => f.GetCustomAttribute<CompilerGeneratedAttribute>() == null);
            foreach (FieldInfo field in fields)
            {
                if (!jsonObj.ContainsKey(field.Name))
                {
                    throw new Exception("No field found with identifier " + field + " in JSON object.");
                }

                Type jsonDataType = jsonObj[field.Name].GetType();
                Type fieldType = field.FieldType;

                MethodInfo genericMethod = typeof(JsonConverter).GetMethod("FromJson", new Type[] { typeof(JsonData), typeof(JsonConverterList), typeof(bool) }).MakeGenericMethod(fieldType);
                object value;
                try
                {
                    value = genericMethod.Invoke(null, new object[] { jsonObj[field.Name], customConverters, true });
                }
                catch (Exception e)
                {
                    throw new Exception("Could not convert " + jsonDataType.Name + " to type " + fieldType.Name + " for field " + field.Name + " in type " + returnType.Name + ". Exception: " + e);
                }

                field.SetValue(obj, value);
            }

            // Auto Properties
            IEnumerable<PropertyInfo> autoProperties = returnType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).Where(p => p.IsAutoProperty());
            foreach (PropertyInfo property in autoProperties)
            {
                if (!jsonObj.ContainsKey(property.Name))
                {
                    throw new Exception("No auto property found with identifier " + property + " in JSON object.");
                }

                Type jsonDataType = jsonObj[property.Name].GetType();
                Type fieldType = property.PropertyType;

                MethodInfo genericMethod = typeof(JsonConverter).GetMethod("FromJson", new Type[] { typeof(JsonData), typeof(JsonConverterList), typeof(bool) }).MakeGenericMethod(fieldType);
                object value;
                try
                {
                    value = genericMethod.Invoke(null, new object[] { jsonObj[property.Name], customConverters, true });
                }
                catch (Exception e)
                {
                    throw new Exception("Could not convert " + jsonDataType.Name + " to type " + fieldType.Name + " for auto property " + property.Name + " in type " + returnType.Name + ". Exception: " + e);
                }

                property.SetValue(obj, value);
            }

            if (fields.Count() < jsonObj.Count)
            {
                Debug.LogWarning("There were unused identifiers in the JSON object when converting to type " + returnType.Name);
            }

            return obj;
        }
    }
}
