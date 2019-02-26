using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using qckdev.ComponentModel;

namespace qckdevTest.ComponentModel
{

    [TestClass]
    public class TypeDescriptorTest
    {

        /// <summary>
        /// Check that inherited classes and interfaces can get properties from its base types.
        /// </summary>
        /// <param name="type">Validating type.</param>
        /// <param name="propertyNames">Property list (example: "Property1, Property2").</param>
        [TestMethod]
        [DataRow(typeof(TestObjects.TestClassBase), "Property1, Property2")]
        [DataRow(typeof(TestObjects.TestClass), "Property5, Property1, Property2")]
        [DataRow(typeof(TestObjects.ITestInterfaceBase), "Property1, Property2, Property4")]
        [DataRow(typeof(TestObjects.ITestInterface), "Property5, Property1, Property2, Property4")]
        public void GetCachedPropertyDescriptors(Type type, string propertyNames)
        {
            var propertyList = TypeDescriptorHelper.GetCachedPropertyDescriptors(type)
                                                   .OfType<System.ComponentModel.PropertyDescriptor>();
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
                Assert.Fail("Type {0} - Expected:<{1}>. Actual:<{2}>. ", type, string.Join(", ", propertyNameArr), string.Join(", ", propertyKeysArr));
            }
        }

    }
}
