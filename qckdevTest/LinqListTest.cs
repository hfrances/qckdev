using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common = qckdevTest.Common;

namespace qckdevTest
{
    [TestClass]
    public class LinqListTest
    {
        readonly Common.LinqListTest Inner = new Common.LinqListTest(new AssertCommon());

        [TestMethod] public void AddRange_TryReplace_TryRemove() => Inner.AddRange_TryReplace_TryRemove();
    }
}
