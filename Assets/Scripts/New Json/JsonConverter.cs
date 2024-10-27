using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using PAC.Extensions;
using UnityEngine;

namespace PAC.Json
{
    public interface IJsonConvertable
    {
        public JsonData ToJson();
    }

    public static class JsonConverter
    {
        public static JsonData ToJson(object obj)
        {
            return ToJson(obj, new HashSet<Type>());
        }
        private static JsonData ToJson(object obj, HashSet<Type> typesAlreadyTryingToConvert)
        {
            Type objType = obj.GetType();

            // Types that define their own JSON conversion
            if (objType.IsAssignableFrom(typeof(IJsonConvertable)))
            {
                return ((IJsonConvertable)obj).ToJson();
            }

            // Uneditable types for which I have defined a JSON conversion
            MethodInfo method = typeof(JsonConverter).GetMethod("ToJson", new Type[] { objType });
            if (method != null && method.GetParameters()[0].ParameterType != typeof(object))
            {
                method.Invoke(null, new object[] { obj });
            }

            // Primitive types
            if (obj == null)
            {
                return new JsonNull();
            }
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
                FieldInfo[] fields = objType.GetFields();
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
                    jsonObj.Add(field.Name, ToJson(field.GetValue(obj), typesAlreadyTryingToConvert));
                    typesAlreadyTryingToConvert.Remove(field.FieldType);
                }

                // Auto Properties
                IEnumerable<PropertyInfo> autoProperties = objType.GetProperties().Where(p => p.IsAutoProperty());
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
                    jsonObj.Add(property.Name, ToJson(property.GetValue(obj), typesAlreadyTryingToConvert));
                    typesAlreadyTryingToConvert.Remove(property.PropertyType);
                }

                return jsonObj;
            }

            throw new Exception("Could not convert object of type " + objType.Name + " to JSON.");
        }

        // Pre-defined JSON conversion for uneditable types
        public static JsonData ToJson(Vector3 obj)
        {
            return new JsonList { new JsonFloat(obj.x), new JsonFloat(obj.y), new JsonFloat(obj.z) };
        }
        public static JsonData ToJson(Vector2 obj)
        {
            return new JsonList { new JsonFloat(obj.x), new JsonFloat(obj.y) };
        }

        public static T FromJson<T>(JsonData jsonData)
        {
            Type returnType = typeof(T);
            Type jsonDataType = jsonData.GetType();

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
        public static T FromJson<T>(JsonList jsonList)
        {
            Type returnType = typeof(T);

            if (!returnType.IsArray && !returnType.IsGenericList())
            {
                throw new Exception("Cannot convert list JSON data into type " + returnType.Name + " as it is not a list.");
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
        public static T FromJson<T>(JsonObj jsonObj)
        {
            Type returnType = typeof(T);
            T obj = (T)FormatterServices.GetSafeUninitializedObject(returnType);

            // Fields
            FieldInfo[] fields = returnType.GetFields();
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
            IEnumerable<PropertyInfo> autoProperties = returnType.GetProperties().Where(p => p.IsAutoProperty());
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

            if (fields.Length < jsonObj.Count)
            {
                Debug.LogWarning("There were unused identifiers in the JSON object when converting to type " + returnType.Name);
            }

            return obj;
        }
    }
}
