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
        public static bool IsStruct(this Type type)
        {
            return type.IsValueType && !type.IsPrimitive && !type.IsEnum;
        }

        /// <summary>
        /// Determines if the given type is List&lt;T&gt; for some type T.
        /// </summary>
        public static bool IsGenericList(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }

        /// <summary>
        /// Determines if the given type is a raw generic - e.g. List&lt;&gt; is a raw generic, but List&lt;int&gt; is not. Also returns false for non-generic types.
        /// </summary>
        public static bool IsRawGeneric(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == type;
        }

        /// <summary>
        /// <para>
        /// Determines if the given type inherits from the given some concrete version of the given raw generic type - e.g. List&lt;int&gt; inherits from the raw generic IEnumerable&lt;&gt;.
        /// </para>
        /// <para>
        /// Throws an exception if 'rawGeneric' is not a raw generic. For example, List&lt;&gt; is a raw generic, but List&lt;int&gt; is not.
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
        /// Gets the concrete version of the given raw generic type that 'subType' inherits - e.g. List&lt;int&gt; inherits from the raw generic IEnumerable&lt;&gt;, with the concrete type returned
        /// by this method being IEnumerable&lt;int&gt;.
        /// </para>
        /// <para>
        /// Throws an exception if 'rawGeneric' is not a raw generic. For example, List&lt;&gt; is a raw generic, but List&lt;int&gt; is not.
        /// </para>
        /// <para>
        /// Throws an exception if 'subType' does not inherit from 'rawGeneric'.
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
        /// Determines if the given property is an auto property - i.e. it has both a getter and setter with no body, for example <code>public string name { get; set; }</code>
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
