using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace PAC.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsStruct(this Type type)
        {
            return type.IsValueType && !type.IsPrimitive && !type.IsEnum;
        }

        public static bool IsGenericList(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }

        public static bool IsRawGeneric(this Type generic)
        {
            return generic.IsGenericType && generic.GetGenericTypeDefinition() == generic;
        }

        public static bool IsSubclassOfRawGeneric(this Type subClass, Type generic)
        {
            if (!generic.IsRawGeneric())
            {
                throw new ArgumentException("Given generic type " + generic.Name + " is not a raw generic. E.g. List<int> is not a raw generic, but List<> is.", "generic");
            }

            while (subClass != null && subClass != typeof(object))
            {
                Type subClassGenericType = subClass.IsGenericType ? subClass.GetGenericTypeDefinition() : subClass;
                if (subClassGenericType == generic)
                {
                    return true;
                }
                subClass = subClass.BaseType;
            }
            return false;
        }

        public static Type GetTypeOfRawGenericSuperclass(this Type subClass, Type generic)
        {
            if (!generic.IsRawGeneric())
            {
                throw new ArgumentException("Given generic type " + generic.Name + " is not a raw generic. E.g. List<int> is not a raw generic, but List<> is.", "generic");
            }

            while (subClass != null && subClass != typeof(object))
            {
                Type subClassGenericType = subClass.IsGenericType ? subClass.GetGenericTypeDefinition() : subClass;
                if (subClassGenericType == generic)
                {
                    return subClass;
                }
                subClass = subClass.BaseType;
            }
            throw new ArgumentException("Type " + subClass.Name + " is not a subclass of type " + generic.Name, "subClass");
        }

        public static bool IsAutoProperty(this PropertyInfo property)
        {
            if (!property.CanWrite || !property.CanRead)
            {
                return false;
            }

            // Auto proprties are syntactic sugar. E.g. a public auto property called 'myProp' in class 'MyClass' will be edited by the compiler
            // so that its getters and setters use a new private field called MyClass___1<myProp>
            return property.DeclaringType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Any(f => f.Name.Contains("<" + property.Name + ">"));
        }
    }
}
