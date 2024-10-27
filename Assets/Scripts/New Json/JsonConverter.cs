using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
                return jsonObj;
            }

            throw new Exception("Could not convert object of type " + objType.Name + " to JSON.");
        }

        // Pre-defined JSON conversion for uneditable types
        public static JsonData ToJson(Vector3 obj)
        {
            return new JsonList { new JsonFloat(obj.x), new JsonFloat(obj.y), new JsonFloat(obj.y) };
        }
        public static JsonData ToJson(Vector2 obj)
        {
            return new JsonList { new JsonFloat(obj.x), new JsonFloat(obj.y) };
        }
    }
}
