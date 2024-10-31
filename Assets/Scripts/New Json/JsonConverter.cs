using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
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
        public static JsonData ToJson(object obj) => ToJson(obj, new JsonConverterList());
        public static JsonData ToJson(object obj, JsonConverterList customConverters)
        {
            return ToJson(obj, new HashSet<Type>(), customConverters);
        }
        private static JsonData ToJson(object obj, HashSet<Type> typesAlreadyTryingToConvert, JsonConverterList customConverters)
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

            // Uneditable types for which I have defined a JSON conversion
            MethodInfo method = typeof(JsonConverter).GetMethod("ToJson", new Type[] { objType });
            if (method != null && method.GetParameters()[0].ParameterType != typeof(object))
            {
                method.Invoke(null, new object[] { obj });
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

            // Lists
            if (objType.IsArray)
            {
                Array objArray = (Array)obj;
                JsonList jsonList = new JsonList(objArray.Length);
                foreach (object element in objArray)
                {
                    jsonList.Add(ToJson(element));
                }
                return jsonList;
            }
            if (objType.IsGenericList())
            {
                IList objList = (IList)obj;
                JsonList jsonList = new JsonList(objList.Count);
                foreach (object element in objList)
                {
                    jsonList.Add(ToJson(element));
                }
                return jsonList;
            }

            // Enums
            if (objType.IsEnum)
            {
                throw new NotImplementedException("ToJson() has not been implemented for enums yet. Enum type: " + objType.Name);
            }

            // Classes / Structs
            if (objType.IsClass || objType.IsStruct())
            {
                JsonObj jsonObj = new JsonObj();

                // Fields
                IEnumerable<FieldInfo> fields = objType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).Where(f => f.GetCustomAttribute<CompilerGeneratedAttribute>() == null);
                foreach (FieldInfo field in fields)
                {
                    // Check for circular type references
                    if (field.FieldType.IsClass || field.FieldType.IsStruct())
                    {
                        if (typesAlreadyTryingToConvert.Contains(field.FieldType))
                        {
                            throw new Exception("Cannot convert types with circular type references; consider writing your own method. The circular reference is between types " +
                                objType.Name + " and " + field.FieldType.Name);
                        }
                        typesAlreadyTryingToConvert.Add(field.FieldType);
                    }
                    jsonObj.Add(field.Name, ToJson(field.GetValue(obj), typesAlreadyTryingToConvert, customConverters));
                    typesAlreadyTryingToConvert.Remove(field.FieldType);
                }

                // Auto Properties
                IEnumerable<PropertyInfo> autoProperties = objType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic).Where(p => p.IsAutoProperty());
                foreach (PropertyInfo property in autoProperties)
                {
                    // Check for circular type references
                    if (property.PropertyType.IsClass || property.PropertyType.IsStruct())
                    {
                        if (typesAlreadyTryingToConvert.Contains(property.PropertyType))
                        {
                            throw new Exception("Cannot convert types with circular type references; consider writing your own method. The circular reference is between types " +
                                objType.Name + " and " + property.PropertyType.Name);
                        }
                        typesAlreadyTryingToConvert.Add(property.PropertyType);
                    }
                    jsonObj.Add(property.Name, ToJson(property.GetValue(obj), typesAlreadyTryingToConvert, customConverters));
                    typesAlreadyTryingToConvert.Remove(property.PropertyType);
                }

                return jsonObj;
            }

            throw new Exception("Could not convert object of type " + objType.Name + " to JSON.");
        }

        public static T FromJson<T>(JsonData jsonData) => FromJson<T>(jsonData, new JsonConverterList());
        public static T FromJson<T>(JsonData jsonData, JsonConverterList customConverters)
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
            if (jsonDataType == typeof(JsonList))
            {
                return FromJson<T>((JsonList)jsonData);
            }
            if (jsonDataType == typeof(JsonObj))
            {
                return FromJson<T>((JsonObj)jsonData);
            }

            throw new Exception("Cannot convert JSON data of type " + jsonDataType.Name + " into type " + returnType.Name);
        }
        private static T FromJson<T>(JsonList jsonList)
        {
            Type returnType = typeof(T);

            if (!returnType.IsArray && !returnType.IsGenericList())
            {
                throw new Exception("Cannot convert list JSON data into type " + returnType.Name + " as it is not a list or array.");
            }

            if (returnType.IsArray)
            {
                Type elementType = returnType.GetElementType();
                MethodInfo genericMethod = typeof(JsonConverter).GetMethod("FromJson", new Type[] { typeof(JsonData) }).MakeGenericMethod(elementType);

                Array array = Array.CreateInstance(elementType, jsonList.Count);
                for (int i = 0; i < jsonList.Count; i++)
                {
                    object value;
                    try
                    {
                        value = genericMethod.Invoke(null, new object[] { jsonList[i] });
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Could not convert " + jsonList[i].GetType().Name + " to type " + elementType.Name + " for index " + i + " in array." + ". Exception: " + e);
                    }

                    array.SetValue(value, i);
                }

                return (T)(object)array;
            }
            else
            {
                Type elementType = returnType.GetGenericArguments()[0];
                MethodInfo genericMethod = typeof(JsonConverter).GetMethod("FromJson", new Type[] { typeof(JsonData) }).MakeGenericMethod(elementType);

                IList list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType), new object[] { jsonList.Count });
                for (int i = 0; i < jsonList.Count; i++)
                {
                    object value;
                    try
                    {
                        value = genericMethod.Invoke(null, new object[] { jsonList[i] });
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
        private static T FromJson<T>(JsonObj jsonObj)
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

                MethodInfo genericMethod = typeof(JsonConverter).GetMethod("FromJson", new Type[] { typeof(JsonData) }).MakeGenericMethod(fieldType);
                object value;
                try
                {
                    value = genericMethod.Invoke(null, new object[] { jsonObj[field.Name] });
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

                MethodInfo genericMethod = typeof(JsonConverter).GetMethod("FromJson", new Type[] { typeof(JsonData) }).MakeGenericMethod(fieldType);
                object value;
                try
                {
                    value = genericMethod.Invoke(null, new object[] { jsonObj[property.Name] });
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
