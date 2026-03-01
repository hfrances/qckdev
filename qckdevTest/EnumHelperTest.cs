using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common = qckdevTest.Common;

namespace qckdevTest
{
    [TestClass]
    public class EnumHelperTest
    {
        readonly Common.EnumHelperTest Inner = new Common.EnumHelperTest(new AssertCommon());

        [TestMethod] public void ContainsAll_True_WhenAllFlagsExist() => Inner.ContainsAll_True_WhenAllFlagsExist();
        [TestMethod] public void ContainsAll_False_WhenCombinedFlagIsNotFullyContained() => Inner.ContainsAll_False_WhenCombinedFlagIsNotFullyContained();
    }
}
