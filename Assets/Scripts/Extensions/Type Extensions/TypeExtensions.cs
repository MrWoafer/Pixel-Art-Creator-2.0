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
