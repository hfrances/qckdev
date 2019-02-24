using System;
using System.Linq;
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
        [DataRow(typeof(TestClassBase), "Property1, Property2, Property3")]
        [DataRow(typeof(TestClass), "Property1, Property2, Property3, Property5")]
        [DataRow(typeof(TestInterfaceBase), "Property1, Property2, Property3, Property4")]
        [DataRow(typeof(TestInterface), "Property1, Property2, Property3, Property4, Property5")]
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


        #region helpers

        private class TestClassBase
        {
            public string Property1 { get; }
            public int Property2 { get; set; }
            public bool Property3 { set => Property4 = value; }
            private bool Property4 { get; set; }

        }

        private class TestClass : TestClassBase
        {
            public string Property5 { get; }

        }

        private interface TestInterfaceBase
        {
            string Property1 { get; }
            int Property2 { get; set; }
            bool Property3 { set; }
            bool Property4 { get; set; }

        }

        private interface TestInterface : TestInterfaceBase
        {
            string Property5 { get; }

        }

        #endregion


    }
}
