using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common = qckdevTest.Common;

namespace qckdevTest
{
    [TestClass]
    public class LinqCollectionTest
    {
        readonly Common.LinqCollectionTest Inner = new Common.LinqCollectionTest(new AssertCommon());

        [TestMethod] public void AddRange_And_IndexOf() => Inner.AddRange_And_IndexOf();
    }
}
