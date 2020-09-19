using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using qckdev.Reflection;

namespace qckdevTest.Reflection
{

    [TestClass]
    public class ExtensionTest
    {

        /// <summary>
        /// Check that inherited classes and interfaces can get properties from its base types.
        /// </summary>
        /// <param name="type">Validating type.</param>
        /// <param name="propertyNames">Property list (example: "Property1, Property2").</param>
        [DataRow(typeof(TestObjects.TestClassBase), "Property1, Property2, Property3")]
        [DataRow(typeof(TestObjects.TestClass), "Property1, Property2, Property3, Property5")]
        [DataRow(typeof(TestObjects.ITestInterfaceBase), "Property1, Property2, Property3, Property4")]
        [DataRow(typeof(TestObjects.ITestInterface), "Property1, Property2, Property3, Property4, Property5")]
        [TestMethod]
        public void GetPropertiesFull(Type type, string propertyNames)
        {
            var propertyList = type.GetPropertiesFull();
            var propertyKeysArr = propertyList.Select(x => x.Name).ToArray();
            var propertyNameArr = propertyNames.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                               .Select(x => x.Trim())
                                               .ToArray();

            if (propertyKeysArr.Length == propertyNameArr.Length)
            {
                for (int i = 0; i < propertyKeysArr.Length; i++)
                {
                    Assert.AreEqual(propertyNameArr[i], propertyKeysArr[i]);
                }
            }
            else
            {
                Assert.Fail("Expected:<{0}>. Actual:<{1}>. ", string.Join(", ", propertyNameArr), string.Join(", ", propertyKeysArr));
            }
        }

        [DataRow(typeof(Enumerable), nameof(Enumerable.Contains),
            new[] { typeof(Guid), typeof(IEnumerable<Guid>), typeof(Guid) },
            typeof(IEnumerable<Guid>))]
        [DataRow(typeof(Queryable), nameof(Queryable.Contains),
            new[] { typeof(Guid), typeof(IQueryable<Guid>), typeof(Guid) },
            typeof(IQueryable<Guid>))]
        [DataRow(typeof(TestObjects.TestMethods), nameof(TestObjects.TestMethods.Method),
            new[] { typeof(Guid), typeof(System.ComponentModel.BindingList<Guid>) },
            typeof(IList<Guid>))]
        [DataRow(typeof(TestObjects.TestMethods), nameof(TestObjects.TestMethods.Method),
            new[] { typeof(System.Security.Principal.IdentityReference),
                    typeof(System.Security.Principal.IdentityReferenceCollection) },
            typeof(ICollection<System.Security.Principal.IdentityReference>))]
        [DataRow(typeof(TestObjects.TestMethods), nameof(TestObjects.TestMethods.Method),
            new[] { typeof(Guid), typeof(IQueryable<Guid>) },
            typeof(IEnumerable<Guid>))]
        [TestMethod]
        public void GetMethodExt(Type objectType, string methodName, Type[] types, Type resultType)
        {
            MethodInfo methodInfo;

            methodInfo = objectType.GetMethodExt(methodName, types);
            Assert.IsNotNull(methodInfo);
            if (methodInfo != null)
            {
                Assert.AreEqual(resultType, methodInfo.GetParameters()?.FirstOrDefault()?.ParameterType);
            }
        }

        [DataRow(typeof(System.Data.IDbConnection), typeof(System.Data.Common.DbConnection))]
        [DataRow(typeof(IEnumerable), typeof(IQueryable))]
        [DataRow(typeof(IEnumerable<Guid>), typeof(IQueryable<Guid>))]
        [DataRow(typeof(IEnumerable), typeof(IQueryable<Guid>))]
        [DataRow(typeof(IEnumerable), typeof(System.ComponentModel.BindingList<>))]
        [TestMethod]
        public void IsEqualityFrom(Type type, Type c)
        {
            var isEquatableFromMethod = typeof(ReflectionHelper).GetMethod("IsEquatableFrom", BindingFlags.Static | BindingFlags.NonPublic);
            var expected = type.IsAssignableFrom(c);
            var equalityLevel = (long)isEquatableFromMethod.Invoke(null, new[] { type, c });

            Assert.AreEqual(expected, (equalityLevel > 0));
        }

    }
}
