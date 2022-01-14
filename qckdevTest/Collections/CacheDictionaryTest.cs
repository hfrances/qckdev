using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qckdevTest.Collections
{

    [TestClass]
    public class CacheDictionaryTest
    {

        [TestMethod]
        public void CreateInstance_WithValues()
        {
            var dic = new qckdev.Collections.CacheDictionary<int, string>
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
            var dic = new qckdev.Collections.CacheDictionary<int, string>();

            dic.Add(1, "a");
            dic.Add(2, "b");
            dic.Add(3, "c");
            Assert.AreEqual(3, dic.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void Add_Duplicated()
        {
            var dic = new qckdev.Collections.CacheDictionary<int, string>()
            {
                CacheTimeout = TimeSpan.FromMilliseconds(200)
            };

            dic.Add(1, "a");
            dic.Add(2, "b");
            dic.Add(3, "c");
            dic.Add(1, "a");
            Assert.AreEqual(3, dic.Count);
        }

        [TestMethod]
        public void Add_DuplicatedWithDelay()
        {
            var dic = new qckdev.Collections.CacheDictionary<int, string>()
            {
                CacheTimeout = TimeSpan.FromMilliseconds(200)
            };

            dic.Add(1, "a");
            System.Threading.Thread.Sleep(200);
            dic.Add(2, "b");
            dic.Add(3, "c");
            dic.Add(1, "a");
            Assert.AreEqual(3, dic.Count);
        }

        [TestMethod]
        public void Count_WithDelay()
        {
            var dic = new qckdev.Collections.CacheDictionary<int, string>()
            {
                CacheTimeout = TimeSpan.FromMilliseconds(200)
            };

            dic.Add(1, "a");
            System.Threading.Thread.Sleep(210);
            dic.Add(2, "b");
            dic.Add(3, "c");
            Assert.AreEqual(2, dic.Count);
        }

        [TestMethod]
        public void TryGetValue()
        {
            var dic = new qckdev.Collections.CacheDictionary<int, string>()
            {
                CacheTimeout = TimeSpan.FromMilliseconds(200)
            };

            dic.Add(1, "a");
            dic.Add(2, "b");
            dic.Add(3, "c");

            dic.TryGetValue(1, out string value);
            Assert.AreEqual("a", value);
        }

        [TestMethod]
        public void TryGetValue_WithDelay()
        {
            var dic = new qckdev.Collections.CacheDictionary<int, string>()
            {
                CacheTimeout = TimeSpan.FromMilliseconds(200)
            };

            dic.Add(1, "a");
            System.Threading.Thread.Sleep(210);
            dic.Add(2, "b");
            dic.Add(3, "c");

            dic.TryGetValue(1, out string value);
            Assert.AreEqual(null, value);
        }

        [TestMethod]
        public void ContainsKey()
        {
            var dic = new qckdev.Collections.CacheDictionary<int, string>()
            {
                CacheTimeout = TimeSpan.FromMilliseconds(200)
            };

            dic.Add(1, "a");
            dic.Add(2, "b");
            dic.Add(3, "c");

            Assert.IsTrue(dic.ContainsKey(1));
        }

        [TestMethod]
        public void ContainsKey_WithDelay()
        {
            var dic = new qckdev.Collections.CacheDictionary<int, string>()
            {
                CacheTimeout = TimeSpan.FromMilliseconds(200)
            };

            dic.Add(1, "a");
            System.Threading.Thread.Sleep(210);
            dic.Add(2, "b");
            dic.Add(3, "c");

            Assert.IsFalse(dic.ContainsKey(1));
        }

        [TestMethod]
        public void Remove()
        {
            var dic = new qckdev.Collections.CacheDictionary<int, string>()
            {
                CacheTimeout = TimeSpan.FromMilliseconds(200)
            };

            dic.Add(1, "a");
            dic.Add(2, "b");
            dic.Add(3, "c");
            dic.Remove(1);

            Assert.AreEqual(2, dic.Count);
        }

        [TestMethod]
        public void Remove_Twice()
        {
            var dic = new qckdev.Collections.CacheDictionary<int, string>()
            {
                CacheTimeout = TimeSpan.FromMilliseconds(200)
            };

            dic.Add(1, "a");
            dic.Add(2, "b");
            dic.Add(3, "c");
            dic.Remove(1);
            dic.Remove(1);

            Assert.AreEqual(2, dic.Count);
        }

        [TestMethod]
        public void Remove_WithDelay()
        {
            var dic = new qckdev.Collections.CacheDictionary<int, string>()
            {
                CacheTimeout = TimeSpan.FromMilliseconds(200)
            };

            dic.Add(1, "a");
            System.Threading.Thread.Sleep(210);
            dic.Add(2, "b");
            dic.Add(3, "c");
            dic.Remove(1);

            Assert.AreEqual(2, dic.Count);
        }

        [TestMethod]
        public void Clear()
        {
            var dic = new qckdev.Collections.CacheDictionary<int, string>()
            {
                CacheTimeout = TimeSpan.FromMilliseconds(200)
            };

            dic.Add(1, "a");
            dic.Add(2, "b");
            dic.Add(3, "c");
            dic.Clear();

            Assert.AreEqual(0, dic.Count);
        }

        [TestMethod]
        public void Clear_WithDelay()
        {
            var dic = new qckdev.Collections.CacheDictionary<int, string>()
            {
                CacheTimeout = TimeSpan.FromMilliseconds(200)
            };

            dic.Add(1, "a");
            System.Threading.Thread.Sleep(210);
            dic.Add(2, "b");
            dic.Add(3, "c");
            dic.Clear();

            Assert.AreEqual(0, dic.Count);
        }

        [TestMethod]
        public void This_Set()
        {
            var dic = new qckdev.Collections.CacheDictionary<int, string>()
            {
                CacheTimeout = TimeSpan.FromMilliseconds(200)
            };

            dic[1] = "a";
            dic[2] = "b";
            dic[3] = "c";

            Assert.AreEqual(3, dic.Count);
        }

        [TestMethod]
        public void This_Set_WithDelay()
        {
            var dic = new qckdev.Collections.CacheDictionary<int, string>()
            {
                CacheTimeout = TimeSpan.FromMilliseconds(200)
            };

            dic[1] = "a";
            System.Threading.Thread.Sleep(210);
            dic[2] = "b";
            dic[3] = "c";

            Assert.AreEqual(2, dic.Count);
        }

        [TestMethod]
        public void This_Set_Existing()
        {
            var dic = new qckdev.Collections.CacheDictionary<int, string>()
            {
                CacheTimeout = TimeSpan.FromMilliseconds(200)
            };

            dic[1] = "a";
            dic[2] = "b";
            dic[3] = "c";
            dic[1] = "x";

            Assert.AreEqual("x", dic[1]);
        }

        [TestMethod]
        public void This_Set_Existing_WithDelay()
        {
            var dic = new qckdev.Collections.CacheDictionary<int, string>()
            {
                CacheTimeout = TimeSpan.FromMilliseconds(200)
            };

            dic[1] = "a";
            System.Threading.Thread.Sleep(210);
            dic[2] = "b";
            dic[3] = "c";
            dic[1] = "x";

            Assert.AreEqual("x", dic[1]);
        }

    }
}
