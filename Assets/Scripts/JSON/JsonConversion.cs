using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

using PAC.Extensions.System;

using UnityEngine;

namespace PAC.Json
{
    public static class JsonConversion
    {
        /// <summary>
        /// <para>
        /// Attempts to convert the C# object into JSON data.
        /// </para>
        /// <para>
        /// NOTE: When converting an enum value which has more than one name, one of those names will be chosen, but not necessarily the one originally used to assign that value. E.g.
        /// for an enum with values Value1 = 0, Value2 = 0, Value3 = 1, converting Value1 into JSON may end up as "Value2".
        /// </para>
        /// </summary>
        /// <param name="obj">The object to convert into JSON.</param>
        /// <param name="allowUndefinedConversions">
        /// If false, an exception will be thrown if the code encounters a type that can not be converted using conversions for primitive JSON types and/or custom converters.
        /// </param>
        public static JsonData ToJson(object obj, bool allowUndefinedConversions = false) => ToJson(obj, null, allowUndefinedConversions);
        /// <summary>
        /// <para>
        /// Attempts to convert the C# object into JSON data.
        /// </para>
        /// <para>
        /// NOTE: When converting an enum value which has more than one name, one of those names will be chosen, but not necessarily the one originally used to assign that value. E.g.
        /// for an enum with values Value1 = 0, Value2 = 0, Value3 = 1, converting Value1 into JSON may end up as "Value2".
        /// </para>
        /// </summary>
        /// <param name="obj">The object to convert into JSON.</param>
        /// <param name="customConverters">A collection of JsonConverter objects that defined custom conversions for certain data types.</param>
        /// <param name="allowUndefinedConversions">
        /// If false, an exception will be thrown if the code encounters a type that can not be converted using conversions for primitive JSON types and/or custom converters.
        /// </param>
        public static JsonData ToJson(object obj, JsonConverterSet customConverters, bool allowUndefinedConversions = false) =>
            ToJson(obj, customConverters, allowUndefinedConversions, new HashSet<object>());
        /// <summary>
        /// <para>
        /// Attempts to convert the C# object into JSON data.
        /// </para>
        /// <para>
        /// NOTE: When converting an enum value which has more than one name, one of those names will be chosen, but not necessarily the one originally used to assign that value. E.g.
        /// for an enum with values Value1 = 0, Value2 = 0, Value3 = 1, converting Value1 into JSON may end up as "Value2".
        /// </para>
        /// </summary>
        /// <param name="obj">The object to convert into JSON.</param>
        /// <param name="customConverters">A collection of JsonConverter objects that defined custom conversions for certain data types.</param>
        /// <param name="allowUndefinedConversions">
        /// If false, an exception will be thrown if the code encounters a type that can not be converted using conversions for primitive JSON types and/or custom converters.
        /// </param>
        /// <param name="objectsAlreadyTryingToConvert">
        /// The objects trying to be converted in the current function call stack. Used to prevent infinite loops when converting objects with circular references.
        /// </param>
        private static JsonData ToJson(object obj, JsonConverterSet customConverters, bool allowUndefinedConversions, HashSet<object> objectsAlreadyTryingToConvert)
        {
            if (obj == null)
            {
                return new JsonData.Null();
            }

            Type objType = obj.GetType();

            // Custom JSON converters
            if (customConverters != null)
            {
                object converter = customConverters.GetConverterFor(objType);
                if (converter != null)
                {
                    MethodInfo toJsonMethod = converter.GetType().GetMethod("ToJson", new Type[] { objType });
                    return (JsonData)toJsonMethod.Invoke(converter, new object[] { obj });
                }
            }

            // Primitive types
            if (objType == typeof(bool))
            {
                return new JsonData.Bool((bool)obj);
            }
            if (objType == typeof(int))
            {
                return new JsonData.Int((int)obj);
            }
            if (objType == typeof(float))
            {
                return new JsonData.Float((float)obj);
            }
            if (objType == typeof(string))
            {
                return new JsonData.String((string)obj);
            }

            // Arrays / Lists
            if (objType.IsArray)
            {
                Array objArray = (Array)obj;
                JsonData.List jsonList = new JsonData.List(objArray.Length);
                for (int i = 0; i < objArray.Length; i++)
                {
                    object element = objArray.GetValue(i);

                    // Check for circular object references
                    if (element != null && !element.GetType().IsValueType && objectsAlreadyTryingToConvert.Contains(element))
                    {
                        throw new SerializationException("Cannot convert objects with circular object references; consider writing a custom converter. The circular reference is in type " +
                            element.GetType().Name + " which occurs in index " + i + " of type " + objType.Name);
                    }
                    objectsAlreadyTryingToConvert.Add(element);

                    JsonData value;
                    try
                    {
                        value = ToJson(element, customConverters, allowUndefinedConversions, objectsAlreadyTryingToConvert);
                    }
                    catch (Exception e)
                    {
                        throw new AggregateException("Could not convert " + element.GetType().Name + " to JSON for index " + i + " in type " + objType.Name, e);
                    }

                    jsonList.Add(value);
                    objectsAlreadyTryingToConvert.Remove(element);
                }
                return jsonList;
            }
            if (objType.IsGenericList())
            {
                IList objList = (IList)obj;
                JsonData.List jsonList = new JsonData.List(objList.Count);
                for (int i = 0; i < objList.Count; i++)
                {
                    object element = objList[i];

                    // Check for circular object references
                    if (element != null && !element.GetType().IsValueType && objectsAlreadyTryingToConvert.Contains(element))
                    {
                        throw new SerializationException("Cannot convert objects with circular object references; consider writing a custom converter. The circular reference is in type " +
                            element.GetType().Name + " which occurs in index " + i + " of type " + objType.Name);
                    }
                    objectsAlreadyTryingToConvert.Add(element);

                    JsonData value;
                    try
                    {
                        value = ToJson(element, customConverters, allowUndefinedConversions, objectsAlreadyTryingToConvert);
                    }
                    catch (Exception e)
                    {
                        throw new AggregateException("Could not convert " + element.GetType().Name + " to JSON for index " + i + " in type " + objType.Name, e);
                    }

                    jsonList.Add(value);
                    objectsAlreadyTryingToConvert.Remove(element);
                }
                return jsonList;
            }

            // Enums
            if (objType.IsEnum)
            {
                return new JsonData.String(Enum.GetName(objType, obj));
            }

            // Classes / Structs
            if (objType.IsClass || objType.IsStruct())
            {
                if (!allowUndefinedConversions)
                {
                    throw new SerializationException("The conversion for type " + objType.Name + " is undefined, but parameter allowUndefinedConversions = false. Consider providing a custom converter.");
                }

                JsonData.Object jsonObj = new JsonData.Object();

                // Fields
                IEnumerable<FieldInfo> fields = objType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).Where(f => f.GetCustomAttribute<CompilerGeneratedAttribute>() == null);
                foreach (FieldInfo field in fields)
                {
                    object fieldValue = field.GetValue(obj);

                    // Check for circular object references
                    if (fieldValue != null && !fieldValue.GetType().IsValueType && objectsAlreadyTryingToConvert.Contains(fieldValue))
                    {
                        throw new SerializationException("Cannot convert objects with circular object references; consider writing a custom converter. The circular reference is in type " +
                            field.FieldType.Name + " which occurs in the field " + field.Name + " of type " + objType.Name);
                    }
                    objectsAlreadyTryingToConvert.Add(fieldValue);

                    JsonData value;
                    try
                    {
                        value = ToJson(fieldValue, customConverters, allowUndefinedConversions, objectsAlreadyTryingToConvert);
                    }
                    catch (Exception e)
                    {
                        throw new AggregateException("Could not convert " + fieldValue.GetType().Name + " to JSON for field " + field.Name + " in type " + objType.Name, e);
                    }

                    jsonObj.Add(field.Name, value);
                    objectsAlreadyTryingToConvert.Remove(field);
                }

                // Auto Properties
                IEnumerable<PropertyInfo> autoProperties = objType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).Where(p => p.IsAutoProperty());
                foreach (PropertyInfo property in autoProperties)
                {
                    object propertyValue = property.GetValue(obj);

                    // Check for circular object references
                    if (propertyValue != null && !propertyValue.GetType().IsValueType && objectsAlreadyTryingToConvert.Contains(propertyValue))
                    {
                        throw new SerializationException("Cannot convert objects with circular object references; consider writing a custom converter. The circular reference is in type " +
                            property.PropertyType.Name + " which occurs in the auto property " + property.Name + " of type " + objType.Name);
                    }
                    objectsAlreadyTryingToConvert.Add(propertyValue);

                    JsonData value;
                    try
                    {
                        value = ToJson(propertyValue, customConverters, allowUndefinedConversions, objectsAlreadyTryingToConvert);
                    }
                    catch (Exception e)
                    {
                        throw new AggregateException("Could not convert " + propertyValue.GetType().Name + " to JSON for auto property " + property.Name + " in type " + objType.Name, e);
                    }

                    jsonObj.Add(property.Name, value);
                    objectsAlreadyTryingToConvert.Remove(propertyValue);
                }

                return jsonObj;
            }

            throw new SerializationException("Could not convert object of type " + objType.Name + " to JSON as it is not a JSON primitive, enum, array, list, struct or class.");
        }

        /// <summary>
        /// Attempts to convert the JSON data into a C# object of the given type.
        /// </summary>
        /// <typeparam name="T">The type to convert the JSON into.</typeparam>
        /// <param name="jsonData">The JSON data to convert.</param>
        /// <param name="allowUndefinedConversions">
        /// If false, an exception will be thrown if the code encounters a type that can not be converted using conversions for primitive JSON types and/or custom converters.
        /// </param>
        public static T FromJson<T>(JsonData jsonData, bool allowUndefinedConversions = false) => FromJson<T>(jsonData, new JsonConverterSet(), allowUndefinedConversions);
        /// <summary>
        /// Attempts to convert the JSON data into a C# object of the given type.
        /// </summary>
        /// <typeparam name="T">The type to convert the JSON into.</typeparam>
        /// <param name="jsonData">The JSON data to convert.</param>
        /// <param name="customConverters">A collection of JsonConverter objects that defined custom conversions for certain data types.</param>
        /// <param name="allowUndefinedConversions">
        /// If false, an exception will be thrown if the code encounters a type that can not be converted using conversions for primitive JSON types and/or custom converters.
        /// </param>
        public static T FromJson<T>(JsonData jsonData, JsonConverterSet customConverters, bool allowUndefinedConversions = false) =>
            FromJson<T>(jsonData, customConverters, allowUndefinedConversions, new HashSet<JsonData>());
        /// <summary>
        /// Attempts to convert the JSON data into a C# object of the given type.
        /// </summary>
        /// <typeparam name="T">The type to convert the JSON into.</typeparam>
        /// <param name="jsonData">The JSON data to convert.</param>
        /// <param name="customConverters">A collection of JsonConverter objects that defined custom conversions for certain data types.</param>
        /// <param name="allowUndefinedConversions">
        /// If false, an exception will be thrown if the code encounters a type that can not be converted using conversions for primitive JSON types and/or custom converters.
        /// </param>
        /// <param name="dataAlreadyTryingToConvert">
        /// The JSON data objects trying to be converted in the current function call stack. Used to prevent infinite loops when converting objects with circular references.
        /// </param>
        public static T FromJson<T>(JsonData jsonData, JsonConverterSet customConverters, bool allowUndefinedConversions, HashSet<JsonData> dataAlreadyTryingToConvert)
        {
            Type returnType = typeof(T);
            Type jsonDataType = jsonData.GetType();
            
            dataAlreadyTryingToConvert.Add(jsonData);

            // Custom JSON converters
            object converter = customConverters.GetConverterFor(returnType);
            if (converter != null)
            {
                MethodInfo fromJsonMethod = converter.GetType().GetMethod("FromJson", new Type[] { typeof(JsonData) });
                return (T)fromJsonMethod.Invoke(converter, new object[] { jsonData });
            }

            // Enums
            if (returnType.IsEnum)
            {
                if (jsonDataType != typeof(JsonData.String))
                {
                    throw new SerializationException("Expected string JSON data to convert into enum type " + returnType.Name + " but found type " + jsonDataType.Name);
                }
                return (T)Enum.Parse(returnType, ((JsonData.String)jsonData).value);
            }

            // Primitives
            if (jsonDataType == typeof(JsonData.Null))
            {
                if (returnType.IsValueType)
                {
                    throw new SerializationException("Cannot convert null JSON data into type " + returnType.Name + " which is non-nullable.");
                }
                return (T)(object)null;
            }
            if (jsonDataType == typeof(JsonData.Bool))
            {
                if (returnType != typeof(bool))
                {
                    throw new SerializationException("Cannot convert bool JSON data into type " + returnType.Name + " as it is not a bool.");
                }
                return (T)(object)((JsonData.Bool)jsonData).value;
            }
            if (jsonDataType == typeof(JsonData.Int))
            {
                if (returnType != typeof(int))
                {
                    throw new SerializationException("Cannot convert int JSON data into type " + returnType.Name + " as it is not a int.");
                }
                return (T)(object)((JsonData.Int)jsonData).value;
            }
            if (jsonDataType == typeof(JsonData.Float))
            {
                if (returnType != typeof(float))
                {
                    throw new SerializationException("Cannot convert float JSON data into type " + returnType.Name + " as it is not a float.");
                }
                return (T)(object)((JsonData.Float)jsonData).value;
            }
            if (jsonDataType == typeof(JsonData.String))
            {
                if (returnType != typeof(string))
                {
                    throw new SerializationException("Cannot convert string JSON data into type " + returnType.Name + " as it is not a string.");
                }
                return (T)(object)((JsonData.String)jsonData).value;
            }

            // Arrays / Lists
            if (jsonDataType == typeof(JsonData.List))
            {
                if (!returnType.IsArray && !returnType.IsGenericList())
                {
                    throw new SerializationException("Cannot convert list JSON data into type " + returnType.Name + " as it is not a list or array.");
                }

                JsonData.List jsonList = (JsonData.List)jsonData;

                // Array
                if (returnType.IsArray)
                {
                    Type elementType = returnType.GetElementType();
                    MethodInfo genericMethod = typeof(JsonConversion).GetMethod("FromJson",
                        new Type[] { typeof(JsonData), typeof(JsonConverterSet), typeof(bool), typeof(HashSet<JsonData>) }
                        ).MakeGenericMethod(elementType);

                    Array array = Array.CreateInstance(elementType, jsonList.Count);
                    for (int i = 0; i < jsonList.Count; i++)
                    {
                        JsonData element = jsonList[i];

                        // Check for circular JSON data references
                        if (dataAlreadyTryingToConvert.Contains(element))
                        {
                            throw new SerializationException("Cannot convert JSON data with circular object references; consider writing a custom converter. The circular reference was found with" +
                                "JSON data type " + jsonDataType.Name + " when trying to convert index " + i + " of type " + returnType.Name);
                        }
                        dataAlreadyTryingToConvert.Add(element);

                        object value;
                        try
                        {
                            value = genericMethod.Invoke(null, new object[] { element, customConverters, allowUndefinedConversions, dataAlreadyTryingToConvert });
                        }
                        catch (Exception e)
                        {
                            throw new AggregateException("Could not convert " + element.GetType().Name + " to type " + elementType.Name + " for index " + i + " in array.", e);
                        }

                        array.SetValue(value, i);
                        dataAlreadyTryingToConvert.Remove(element);
                    }

                    return (T)(object)array;
                }
                // List
                else
                {
                    Type elementType = returnType.GetGenericArguments()[0];
                    MethodInfo genericMethod = typeof(JsonConversion).GetMethod("FromJson",
                        new Type[] { typeof(JsonData), typeof(JsonConverterSet), typeof(bool), typeof(HashSet<JsonData>) }
                        ).MakeGenericMethod(elementType);

                    IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType), new object[] { jsonList.Count });
                    for (int i = 0; i < jsonList.Count; i++)
                    {
                        JsonData element = jsonList[i];

                        // Check for circular JSON data references
                        if (dataAlreadyTryingToConvert.Contains(element))
                        {
                            throw new SerializationException("Cannot convert JSON data with circular object references; consider writing a custom converter. The circular reference was found with" +
                                "JSON data type " + jsonDataType.Name + " when trying to convert index " + i + " of type " + returnType.Name);
                        }
                        dataAlreadyTryingToConvert.Add(element);

                        object value;
                        try
                        {
                            value = genericMethod.Invoke(null, new object[] { element, customConverters, allowUndefinedConversions, dataAlreadyTryingToConvert });
                        }
                        catch (Exception e)
                        {
                            throw new AggregateException("Could not convert " + element.GetType().Name + " to type " + elementType.Name + " for index " + i + " in list.", e);
                        }

                        list.Add(value);
                        dataAlreadyTryingToConvert.Remove(element);
                    }

                    return (T)(object)list;
                }
            }

            // Classes / Structs
            if (!allowUndefinedConversions)
            {
                throw new SerializationException("The conversion for type " + returnType.Name + " is undefined, but parameter allowUndefinedConversions = false. Consider providing a custom converter.");
            }
            if (jsonDataType == typeof(JsonData.Object))
            {
                JsonData.Object jsonObj = (JsonData.Object)jsonData;
                // We do not convert to type T here due to boxing of structs. It would work fine for classes, but structs get boxed when we do that so when we edit the fields/properties
                // it wouldn't actually change the struct in this variable, but a new one.
                object obj = FormatterServices.GetSafeUninitializedObject(returnType);

                // Fields
                IEnumerable<FieldInfo> fields = returnType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).Where(f => f.GetCustomAttribute<CompilerGeneratedAttribute>() == null);
                foreach (FieldInfo field in fields)
                {
                    if (!jsonObj.ContainsKey(field.Name))
                    {
                        throw new SerializationException("No field found with identifier " + field + " in JSON object.");
                    }

                    JsonData jsonValue = jsonObj[field.Name];

                    // Check for circular JSON data references
                    if (dataAlreadyTryingToConvert.Contains(jsonValue))
                    {
                        throw new SerializationException("Cannot convert JSON data with circular object references; consider writing a custom converter. The circular reference was found with" +
                            "JSON data type " + jsonDataType.Name + " which occurs in the field " + field.Name + " of type " + returnType.Name);
                    }
                    dataAlreadyTryingToConvert.Add(jsonValue);

                    Type jsonValueType = jsonObj[field.Name].GetType();
                    Type fieldType = field.FieldType;

                    MethodInfo genericMethod = typeof(JsonConversion).GetMethod("FromJson",
                        new Type[] {typeof(JsonData), typeof(JsonConverterSet), typeof(bool), typeof(HashSet<JsonData>) }
                        ).MakeGenericMethod(fieldType);
                    object value;
                    try
                    {
                        value = genericMethod.Invoke(null, new object[] { jsonObj[field.Name], customConverters, allowUndefinedConversions, dataAlreadyTryingToConvert });
                    }
                    catch (Exception e)
                    {
                        throw new AggregateException("Could not convert " + jsonValueType.Name + " to type " + fieldType.Name + " for field " + field.Name + " in type " + returnType.Name, e);
                    }
                    field.SetValue(obj, value);
                    dataAlreadyTryingToConvert.Remove(jsonValue);
                }

                // Auto Properties
                IEnumerable<PropertyInfo> autoProperties = returnType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).Where(p => p.IsAutoProperty());
                foreach (PropertyInfo property in autoProperties)
                {
                    if (!jsonObj.ContainsKey(property.Name))
                    {
                        throw new SerializationException("No auto property found with identifier " + property + " in JSON object.");
                    }

                    JsonData jsonValue = jsonObj[property.Name];

                    // Check for circular JSON data references
                    if (dataAlreadyTryingToConvert.Contains(jsonValue))
                    {
                        throw new SerializationException("Cannot convert JSON data with circular object references; consider writing a custom converter. The circular reference was found with" +
                            "JSON data type " + jsonDataType.Name + " which occurs in the auto property " + property.Name + " of type " + returnType.Name);
                    }
                    dataAlreadyTryingToConvert.Add(jsonValue);

                    Type jsonValueType = jsonObj[property.Name].GetType();
                    Type fieldType = property.PropertyType;

                    MethodInfo genericMethod = typeof(JsonConversion).GetMethod("FromJson",
                        new Type[] { typeof(JsonData), typeof(JsonConverterSet), typeof(bool), typeof(HashSet<JsonData>) }
                    ).MakeGenericMethod(fieldType);
                    object value;
                    try
                    {
                        value = genericMethod.Invoke(null, new object[] { jsonObj[property.Name], customConverters, allowUndefinedConversions, dataAlreadyTryingToConvert });
                    }
                    catch (Exception e)
                    {
                        throw new AggregateException("Could not convert " + jsonValueType.Name + " to type " + fieldType.Name + " for auto property " + property.Name + " in type " + returnType.Name, e);
                    }

                    property.SetValue(obj, value);
                    dataAlreadyTryingToConvert.Remove(jsonValue);
                }

                // Warn of any unused identifiers in the JSON object
                if (fields.Count() + autoProperties.Count() < jsonObj.Count)
                {
                    Debug.LogWarning("There were unused identifiers in the JSON object when converting to type " + returnType.Name);
                }

                return (T)obj;
            }

            throw new ArgumentException("Unknown / unimplemented JSON data type: " + jsonDataType.Name, "jsonData");
        }

        /// <summary>
        /// <para>
        /// An interface that custom JSON converters must implement.
        /// </para>
        /// <para>
        /// NOTE: Has a default implementation for an overload FromJson(JsonData jsonData) that ensures it is of type JsonDataType before calling the specific overload FromJSON(JsonDataType jsonData).
        /// </para>
        /// </summary>
        /// <typeparam name="T">The type the converter will convert to/from JSON.</typeparam>
        /// <typeparam name="JsonDataType">The type of JSON data the converter will convert to/from.</typeparam>
        public abstract class JsonConverter<T, JsonDataType> where JsonDataType : JsonData
        {
            /// <summary>
            /// Attempts to convert the C# object into JSON data.
            /// </summary>
            public abstract JsonDataType ToJson(T obj);
            /// <summary>
            /// <para>
            /// Attempts to convert the JSON data into a C# object of the given type.
            /// </para>
            /// <para>
            /// NOTE: JsonConverter has a default implementation for an overload FromJson(JsonData jsonData) that ensures it is of type JsonDataType before calling the
            /// specific overload FromJSON(JsonDataType jsonData).
            /// </para>
            /// </summary>
            public abstract T FromJson(JsonDataType jsonData);
            /// <summary>
            /// Attempts to convert the JSON data into a C# object of the given type. Ensures the JSON data is the correct type before calling the FromJson() overload for the specific type of JSON data.
            /// </summary>
            public T FromJson(JsonData jsonData)
            {
                if (jsonData.GetType() == typeof(JsonDataType))
                {
                    return FromJson((JsonDataType)jsonData);
                }
                throw new ArgumentException("Expected the JSON data to be of type " + typeof(JsonDataType).Name + " but found type " + jsonData.GetType().Name, "jsonData");
            }
        }

        /// <summary>
        /// <para>
        /// Represents a set of JsonConverter objects, making sure there's at most one custom converter for each type. For example, a class MyClass cannot have two JSON converters defined for it in
        /// the list.
        /// </para>
        /// </summary>
        public class JsonConverterSet : IEnumerable
        {
            private Dictionary<Type, object> converters = new Dictionary<Type, object>();

            /// <summary>
            /// Creates an empty JsonConverterSet.
            /// </summary>
            public JsonConverterSet() { }
            /// <summary>
            /// Creates a JsonConverterSet with the given converters. Throws an error if an object is not a type that implements JsonConverter, or if the set already contains a converter for the
            /// type a converter converts.
            /// </summary>
            public JsonConverterSet(params object[] converters)
            {
                foreach (object converter in converters)
                {
                    Add(converter);
                }
            }
            public JsonConverterSet(IEnumerable converters)
            {
                foreach (object converter in converters)
                {
                    Add(converter);
                }
            }

            /// <summary>
            /// Adds the converter to the set. Throws an error if it is not a type that implements JsonConverter, or if the set already contains a converter for the type the converter converts.
            /// </summary>
            public void Add(object converter)
            {
                Type type = converter.GetType();
                if (type.IsSubclassOfRawGeneric(typeof(JsonConverter<,>)))
                {
                    Type converterType = type.GetTypeOfRawGenericSuperclass(typeof(JsonConverter<,>)).GetGenericArguments()[0];
                    if (ContainsConverterFor(converterType))
                    {
                        throw new ArgumentException("The list already contains a converter for type " + converterType, "converter");
                    }
                    converters.Add(converterType, converter);
                }
                else
                {
                    throw new ArgumentException("JsonConverterList can only add objects that implement JsonConverter<,>. The provided object was of type " + type.Name, "converter");
                }
            }

            /// <summary>
            /// If the set contains a converter that converts the given type, this will remove it.
            /// </summary>
            /// <returns>true if a converter was removed.</returns>
            public bool RemoveConverterFor(Type converterType)
            {
                if (ContainsConverterFor(converterType))
                {
                    converters.Remove(converterType);
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Returns whether the set contains a converter that converts the given type.
            /// </summary>
            /// <remarks>
            /// If there is no converter for <paramref name="converterType"/>, this method will work its way up the inheritance tree to try and find a converter for a supertype.
            /// </remarks>
            /// <returns>true if the set has a converter for the given type.</returns>
            public bool ContainsConverterFor(Type converterType)
            {
                while (converterType != null)
                {
                    if (converters.ContainsKey(converterType))
                    {
                        return true;
                    }

                    converterType = converterType.BaseType;
                }

                return false;
            }

            /// <summary>
            /// If the set contains a converter that converts the given type, this will return it.
            /// </summary>
            /// <remarks>
            /// If there is no converter for <paramref name="converterType"/>, this method will work its way up the inheritance tree to try and find a converter for a supertype.
            /// </remarks>
            /// <returns>The converter for the given type, if there is one, otherwise null.</returns>
            public object GetConverterFor(Type converterType)
            {
                while (converterType != null)
                {
                    if (converters.ContainsKey(converterType))
                    {
                        return converters[converterType];
                    }

                    converterType = converterType.BaseType;
                }

                return null;
            }

            public IEnumerator GetEnumerator()
            {
                return converters.Values.GetEnumerator();
            }
        }
    }
}
