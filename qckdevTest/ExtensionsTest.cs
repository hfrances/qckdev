using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using qckdev;

namespace qckdevTest
{

    [TestClass]
    public class ExtensionsTest
    {

        [TestMethod]
        [DataRow("patata", true)]
        [DataRow("Patata", false)]
        public void InTest_String(string value, bool expected)
        {
            Assert.AreEqual(expected, 
                value.In("lechuga", "patata", "pepino"));
        }

        [TestMethod]
        [DataRow(true, "patata", true)]
        [DataRow(true, "Patata", true)]
        [DataRow(false, "patata", true)]
        [DataRow(false, "Patata", false)]
        public void InTest_String_CaseSensitive(bool ignorecase, string value, bool expected)
        {
            Assert.AreEqual(expected, 
                value.In(ignorecase, "lechuga", "patata", "pepino"));
        }

        [TestMethod]
        [DataRow(0, true)]
        [DataRow(10, true)]
        [DataRow(1000, true)]
        [DataRow(42, false)]
        [DataRow(null, true)]
        public void InTest_Int(int value, bool expected)
        {
            Assert.AreEqual(expected,
                value.In(0, 1, 10, 100, 1000));
        }

        [TestMethod]
        [DataRow(0, true)]
        [DataRow(10, true)]
        [DataRow(1000, true)]
        [DataRow(42, false)]
        [DataRow(null, false)]
        public void InTest_Int_Nullable(int? value, bool expected)
        {
            Assert.AreEqual(expected,
                value.In(0, 1, 10, 100, 1000));
        }

        [TestMethod]
        [DataRow(0, true)]
        [DataRow(10, true)]
        [DataRow(1000, true)]
        [DataRow(42, false)]
        [DataRow(null, true)]
        public void InTest_Int_Nullable2(int? value, bool expected)
        {
            Assert.AreEqual(expected,
                value.In(0, 1, 10, 100, 1000, null));
        }

        [TestMethod]
        public void StringBuilder_Clear()
        {
            var builder = new System.Text.StringBuilder();

            builder.Append("Hola mundo.");
            Assert.AreEqual(11, builder.Length);
            Extensions.Clear(builder);
            Assert.AreEqual(0, builder.Length);
        }

    }
}
