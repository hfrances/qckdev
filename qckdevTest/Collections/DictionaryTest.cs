using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qckdevTest.Collections
{

    [TestClass]
    public class DictionaryTest
    {

        [TestMethod]
        public void CreateInstance_WithValues()
        {
            var dic = new System.Collections.Generic.Dictionary<int, string>
            {
                { 1, "a" },
                { 2, "b" },
                { 3, "c" },
            };
            Assert.AreEqual(3, dic.Count);
        }

        [TestMethod]
        public void Add()
        {
            var dic = new System.Collections.Generic.Dictionary<int, string>();

            dic.Add(1, "a");
            dic.Add(2, "b");
            dic.Add(3, "c");
            Assert.AreEqual(3, dic.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_Duplicated()
        {
            var dic = new System.Collections.Generic.Dictionary<int, string>();

            dic.Add(1, "a");
            dic.Add(2, "b");
            dic.Add(3, "c");
            dic.Add(1, "a");
            Assert.AreEqual(3, dic.Count);
        }

        [TestMethod]
        public void Add_TooManyItems()
        {
            const int count = 1000;
            var dic = new System.Collections.Generic.Dictionary<int, string>();

            for (int i = 0; i < count; i++)
            {
                dic.Add(i, $"Item {i}");
            }
            Assert.AreEqual(count, dic.Count);
        }

        [TestMethod]
        public void Count()
        {
            var dic = new System.Collections.Generic.Dictionary<int, string>();

            dic.Add(1, "a");
            dic.Add(2, "b");
            dic.Add(3, "c");
            Assert.AreEqual(3, dic.Count);
        }

        [TestMethod]
        public void TryGetValue()
        {
            var dic = new System.Collections.Generic.Dictionary<int, string>();

            dic.Add(1, "a");
            dic.Add(2, "b");
            dic.Add(3, "c");

            dic.TryGetValue(1, out string value);
            Assert.AreEqual("a", value);
        }

        [TestMethod]
        public void ContainsKey()
        {
            var dic = new System.Collections.Generic.Dictionary<int, string>();

            dic.Add(1, "a");
            dic.Add(2, "b");
            dic.Add(3, "c");

            Assert.IsTrue(dic.ContainsKey(1));
        }

        [TestMethod]
        public void Remove()
        {
            var dic = new System.Collections.Generic.Dictionary<int, string>();

            dic.Add(1, "a");
            dic.Add(2, "b");
            dic.Add(3, "c");
            dic.Remove(1);

            Assert.AreEqual(2, dic.Count);
        }

        [TestMethod]
        public void Remove_Twice()
        {
            var dic = new System.Collections.Generic.Dictionary<int, string>();

            dic.Add(1, "a");
            dic.Add(2, "b");
            dic.Add(3, "c");
            dic.Remove(1);
            dic.Remove(1);

            Assert.AreEqual(2, dic.Count);
        }

        [TestMethod]
        public void Clear()
        {
            var dic = new System.Collections.Generic.Dictionary<int, string>();

            dic.Add(1, "a");
            dic.Add(2, "b");
            dic.Add(3, "c");
            dic.Clear();

            Assert.AreEqual(0, dic.Count);
        }

        [TestMethod]
        public void This_Set()
        {
            var dic = new System.Collections.Generic.Dictionary<int, string>();

            dic[1] = "a";
            dic[2] = "b";
            dic[3] = "c";

            Assert.AreEqual(3, dic.Count);
        }

        [TestMethod]
        public void This_Set_Existing()
        {
            var dic = new System.Collections.Generic.Dictionary<int, string>();

            dic[1] = "a";
            dic[2] = "b";
            dic[3] = "c";
            dic[1] = "x";

            Assert.AreEqual("x", dic[1]);
        }

        [TestMethod]
        public void This_Get()
        {
            var dic = new System.Collections.Generic.Dictionary<int, string>();

            dic[1] = "a";
            dic[2] = "b";
            dic[3] = "c";

            Assert.AreEqual("a", dic[1]);
        }

        [TestMethod]
        public void Keys()
        {
            var dic = new System.Collections.Generic.Dictionary<int, string>();

            dic[1] = "a";
            dic[2] = "b";
            dic[3] = "c";

            Assert.AreEqual(3, dic.Keys.Count);
        }

        [TestMethod]
        public void Values()
        {
            var dic = new System.Collections.Generic.Dictionary<int, string>();

            dic[1] = "a";
            dic[2] = "b";
            dic[3] = "c";

            Assert.AreEqual(3, dic.Values.Count);
        }

    }
}
