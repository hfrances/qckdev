using System.Collections.Generic;

namespace qckdevTest.Common
{
    public sealed class LinqDictionaryTest
    {
        readonly IAssert Assert;

        public LinqDictionaryTest(IAssert assert)
        {
            Assert = assert;
        }

        public void TryGetValue_And_AddIfNotExists()
        {
            var classDic = new Dictionary<string, string>();
            var structDic = new Dictionary<string, int>();

            Assert.IsTrue(qckdev.Linq.Dictionary.AddIfNotExists(classDic, "a", "1"));
            Assert.IsFalse(qckdev.Linq.Dictionary.AddIfNotExists(classDic, "a", "2"));
            Assert.AreEqual("1", qckdev.Linq.Dictionary.TryGetValue(classDic, "a"));
            Assert.IsNull(qckdev.Linq.Dictionary.TryGetValue(classDic, "missing"));
            Assert.AreEqual("x", qckdev.Linq.Dictionary.TryGetValue(classDic, "missing", "x"));

            var missing = qckdev.Linq.Dictionary.TryGetValue(structDic, "missing", 7);
            Assert.AreEqual(7, missing.Value);
        }
    }
}
