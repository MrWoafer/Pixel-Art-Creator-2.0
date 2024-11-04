using NUnit.Framework;
using System.Collections.Generic;
using PAC.Extensions;
using System;
using System.Reflection;

namespace PAC.Tests
{
    public class TypeExtensionsTests
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

        [Test]
        [Category("Extensions")]
        public void IsStruct()
        {
            Assert.True(typeof(Struct1).IsStruct());
            Assert.False(typeof(Class1).IsStruct());
            Assert.False(typeof(Enum1).IsStruct());
            Assert.False(typeof(int).IsStruct());
        }

        [Test]
        [Category("Extensions")]
        public void IsGenericList()
        {
            Assert.True(typeof(List<int>).IsGenericList());
            Assert.True(typeof(List<>).IsGenericList());
            Assert.False(typeof(List).IsGenericList());
            Assert.False(typeof(int[]).IsGenericList());
        }

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

            Assert.Throws<ArgumentException>(() => typeof(Class4).IsSubclassOfRawGeneric(typeof(Class3)));
        }

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
    }
}
