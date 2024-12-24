using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace PAC.Extensions
{
    public static class TypeExtensions
    {
        /// <summary>
        /// Determiens if the given type is a struct or not.
        /// </summary>
        public static bool IsStruct(this Type type) => type.IsValueType && !type.IsPrimitive && !type.IsEnum;

        /// <summary>
        /// Determines if the given type is <see cref="List{T}"/> for some type <typeparamref name="T"/>.
        /// </summary>
        public static bool IsGenericList(this Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);

        /// <summary>
        /// Determines if the given type is a raw generic - e.g. <c>List&lt;&gt;</c> is a raw generic, but <c>List&lt;<see langword="int"/>&gt;</c> is not. Also returns false for non-generic types.
        /// </summary>
        public static bool IsRawGeneric(this Type type) => type.IsGenericType && type.GetGenericTypeDefinition() == type;

        /// <summary>
        /// <para>
        /// Determines if the given type inherits from the given some concrete version of the given raw generic type - e.g. <c>List&lt;<see langword="int"/>&gt;</c> inherits from the raw generic
        /// <c>IEnumerable&lt;&gt;</c>.
        /// </para>
        /// <para>
        /// Throws an exception if <paramref name="rawGeneric"/> is not a raw generic. For example, <c>List&lt;&gt;</c> is a raw generic, but <c>List&lt;<see langword="int"/>&gt;</c> is not.
        /// </para>
        /// </summary>
        public static bool IsSubclassOfRawGeneric(this Type subType, Type rawGeneric)
        {
            if (!rawGeneric.IsRawGeneric())
            {
                throw new ArgumentException("Given generic type " + rawGeneric.Name + " is not a raw generic. E.g. List<int> is not a raw generic, but List<> is.", "rawGeneric");
            }

            while (subType != null && subType != typeof(object))
            {
                Type subClassGenericType = subType.IsGenericType ? subType.GetGenericTypeDefinition() : subType;
                if (subClassGenericType == rawGeneric)
                {
                    return true;
                }
                subType = subType.BaseType;
            }
            return false;
        }

        /// <summary>
        /// <para>
        /// Gets the concrete version of the given raw generic type that <paramref name="subType"/> inherits from - e.g. <c>List&lt;<see langword="int"/>&gt;</c> inherits from the raw generic
        /// <c>IEnumerable&lt;&gt;</c>, with the concrete type returned by this method being <c>IEnumerable&lt;<see langword="int"/>&gt;</c>.
        /// </para>
        /// <para>
        /// Throws an exception if <paramref name="rawGeneric"/> is not a raw generic. For example, <c>List&lt;&gt;</c> is a raw generic, but <c>List&lt;<see langword="int"/>&gt;</c> is not.
        /// </para>
        /// <para>
        /// Throws an exception if <paramref name="subType"/> does not inherit from <paramref name="rawGeneric"/>.
        /// </para>
        /// </summary>
        public static Type GetTypeOfRawGenericSuperclass(this Type subType, Type rawGeneric)
        {
            if (!rawGeneric.IsRawGeneric())
            {
                throw new ArgumentException("Given generic type " + rawGeneric.Name + " is not a raw generic. E.g. List<int> is not a raw generic, but List<> is.", "rawGeneric");
            }

            while (subType != null && subType != typeof(object))
            {
                Type subClassGenericType = subType.IsGenericType ? subType.GetGenericTypeDefinition() : subType;
                if (subClassGenericType == rawGeneric)
                {
                    return subType;
                }
                subType = subType.BaseType;
            }
            throw new ArgumentException("Type " + subType.Name + " is not a subclass of type " + rawGeneric.Name, "subClass");
        }

        /// <summary>
        /// Determines if the given property is an auto property - i.e. it has both a getter and setter with no body, for example
        /// <code>
        /// <see langword="public"/> <see langword="string"/> name { <see langword="get"/>; <see langword="set"/>; }
        /// </code>
        /// </summary>
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
