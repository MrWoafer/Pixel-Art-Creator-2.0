using NUnit.Framework;
using System.Collections.Generic;
using System;
using System.Reflection;
using PAC.Extensions.System;

namespace PAC.Tests.Extensions.System
{
    /// <summary>
    /// Tests my extension methods for type Type.
    /// </summary>
    public class TypeExtensions_Tests
    {
        class Class1
        {
            public string name;
        }

        public struct Struct1
        {
            public string name;
        }

        public enum Enum1
        {
            Value1,
            Value2
        }

        /// <summary>
        /// Checks that the Type.IsStruct() extension method correctly identifies if a type is a struct or not.
        /// </summary>
        [Test]
        [Category("Extensions")]
        public void IsStruct()
        {
            Assert.True(typeof(Struct1).IsStruct());
            Assert.False(typeof(Class1).IsStruct());
            Assert.False(typeof(Enum1).IsStruct());
            Assert.False(typeof(int).IsStruct());
        }

        /// <summary>
        /// Checks that the Type.IsGenericList() extension method works correctly.
        /// </summary>
        [Test]
        [Category("Extensions")]
        public void IsGenericList()
        {
            Assert.True(typeof(List<int>).IsGenericList());
            Assert.True(typeof(List<>).IsGenericList());
            Assert.False(typeof(List).IsGenericList());
            Assert.False(typeof(int[]).IsGenericList());
        }

        /// <summary>
        /// Checks that the Type.IsRawGeneric() extension method works correctly.
        /// </summary>
        [Test]
        [Category("Extensions")]
        public void IsRawGeneric()
        {
            Assert.True(typeof(List<>).IsRawGeneric());
            Assert.False(typeof(List<int>).IsRawGeneric());
            Assert.False(typeof(List).IsRawGeneric());
            Assert.False(typeof(int).IsRawGeneric());
        }

        class Class2<T>
        {
            public string name;
        }

        class Class3 : Class2<int>
        {
            public int id;
        }

        class Class4 : Class3
        {
            public bool flag;
        }

        /// <summary>
        /// Checks that the Type.IsSubclassOfRawGeneric() extension method works correctly.
        /// </summary>
        [Test]
        [Category("Extensions")]
        public void IsSubclassOfRawGeneric()
        {
            Assert.True(typeof(Class3).IsSubclassOfRawGeneric(typeof(Class2<>)));
            Assert.False(typeof(Class3).IsSubclassOfRawGeneric(typeof(List<>)));
            Assert.Throws<ArgumentException>(() => typeof(Class3).IsSubclassOfRawGeneric(typeof(Class2<int>)));

            Assert.True(typeof(Class4).IsSubclassOfRawGeneric(typeof(Class2<>)));
            Assert.False(typeof(Class4).IsSubclassOfRawGeneric(typeof(List<>)));
            Assert.Throws<ArgumentException>(() => typeof(Class4).IsSubclassOfRawGeneric(typeof(Class2<int>)));

            Assert.True(typeof(Class2<int>).IsSubclassOfRawGeneric(typeof(Class2<>)));
            Assert.True(typeof(Class2<>).IsSubclassOfRawGeneric(typeof(Class2<>)));
            Assert.Throws<ArgumentException>(() => typeof(Class4).IsSubclassOfRawGeneric(typeof(Class3)));
        }

        /// <summary>
        /// Checks that the Type.GetTypeOfRawGenericSuperclass() extension method works correctly.
        /// </summary>
        [Test]
        [Category("Extensions")]
        public void GetTypeOfRawGenericSuperclass()
        {
            Assert.AreEqual(typeof(Class3).GetTypeOfRawGenericSuperclass(typeof(Class2<>)), typeof(Class2<int>));
            Assert.Throws<ArgumentException>(() => typeof(Class3).GetTypeOfRawGenericSuperclass(typeof(Class2<int>)));
            Assert.Throws<ArgumentException>(() => typeof(Class3).GetTypeOfRawGenericSuperclass(typeof(List<>)));

            Assert.AreEqual(typeof(Class4).GetTypeOfRawGenericSuperclass(typeof(Class2<>)), typeof(Class2<int>));
            Assert.Throws<ArgumentException>(() => typeof(Class4).GetTypeOfRawGenericSuperclass(typeof(Class2<int>)));
            Assert.Throws<ArgumentException>(() => typeof(Class4).GetTypeOfRawGenericSuperclass(typeof(List<>)));

            Assert.AreEqual(typeof(Class2<int>).GetTypeOfRawGenericSuperclass(typeof(Class2<>)), typeof(Class2<int>));
            Assert.AreEqual(typeof(Class2<>).GetTypeOfRawGenericSuperclass(typeof(Class2<>)), typeof(Class2<>));
            Assert.Catch(() => typeof(Class4).GetTypeOfRawGenericSuperclass(typeof(Class3)));
        }

        class Class5
        {
            public int autoprop1 { get; set; }
            public int autoprop2 { get; set; } = 4;
            protected int autoprop3 { get; set; }
            private int autoprop4 { get; set; }
            public int autoprop5 { get; private set; }
            public int autoprop6 { private get; set; }
            public int autoprop7 { get; protected set; }
            public int autoprop8 { protected get; set; }

            private int _prop1;
            public int prop1
            {
                get => _prop1;
                set => _prop1 = value;
            }
            public int _prop2;
            public int prop2
            {
                get => _prop2;
            }
            public int prop3 { get; }
            public int prop4 { get; } = 6;
        }

        /// <summary>
        /// Checks that the PropertyInfo.IsAutoProperty() extension method correctly identifies if a property is an auto property or not.
        /// </summary>
        [Test]
        [Category("Extensions")]
        public void IsAutoProperty()
        {
            Assert.True(typeof(Class5).GetProperty("autoprop1", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).IsAutoProperty());
            Assert.True(typeof(Class5).GetProperty("autoprop2", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).IsAutoProperty());
            Assert.True(typeof(Class5).GetProperty("autoprop3", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).IsAutoProperty());
            Assert.True(typeof(Class5).GetProperty("autoprop4", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).IsAutoProperty());
            Assert.True(typeof(Class5).GetProperty("autoprop5", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).IsAutoProperty());
            Assert.True(typeof(Class5).GetProperty("autoprop6", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).IsAutoProperty());
            Assert.True(typeof(Class5).GetProperty("autoprop7", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).IsAutoProperty());
            Assert.True(typeof(Class5).GetProperty("autoprop8", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).IsAutoProperty());

            Assert.False(typeof(Class5).GetProperty("prop1", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).IsAutoProperty());
            Assert.False(typeof(Class5).GetProperty("prop2", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).IsAutoProperty());
            Assert.False(typeof(Class5).GetProperty("prop3", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).IsAutoProperty());
            Assert.False(typeof(Class5).GetProperty("prop4", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance).IsAutoProperty());
        }

        private enum TestEnum0 { }
        private enum TestEnum1
        {
            Value1
        }
        private enum TestEnum2
        {
            Hello,
            There
        }
        private enum TestEnum3
        {
            How = 5,
            Are = -1,
            You = 2,
        }

        /// <summary>
        /// Tests <see cref="TypeExtensions.GetValues{T}"/>.
        /// </summary>
        [Test]
        [Category("Extensions")]
        public void GetValues()
        {
            CollectionAssert.IsEmpty(TypeExtensions.GetValues<TestEnum0>());
            CollectionAssert.AreEquivalent(new TestEnum1[] { TestEnum1.Value1 }, TypeExtensions.GetValues<TestEnum1>());
            CollectionAssert.AreEquivalent(new TestEnum2[] { TestEnum2.Hello, TestEnum2.There }, TypeExtensions.GetValues<TestEnum2>());
            CollectionAssert.AreEquivalent(new TestEnum3[] { TestEnum3.How, TestEnum3.Are, TestEnum3.You }, TypeExtensions.GetValues<TestEnum3>());
        }
    }
}
