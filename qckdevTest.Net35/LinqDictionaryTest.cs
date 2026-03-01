using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common = qckdevTest.Common;

namespace qckdevTest
{
    [TestClass]
    public class LinqDictionaryTest
    {
        readonly Common.LinqDictionaryTest Inner = new Common.LinqDictionaryTest(new AssertCommon());

        [TestMethod] public void TryGetValue_And_AddIfNotExists() => Inner.TryGetValue_And_AddIfNotExists();
    }
}
