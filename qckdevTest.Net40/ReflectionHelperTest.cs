using Microsoft.VisualStudio.TestTools.UnitTesting;
using Common = qckdevTest.Common;

namespace qckdevTest
{
    [TestClass]
    public class ReflectionHelperTest
    {
        readonly Common.ReflectionHelperTest Inner = new Common.ReflectionHelperTest(new AssertCommon());

        [TestMethod] public void GetHashCode_HandlesNulls_AndOverloads() => Inner.GetHashCode_HandlesNulls_AndOverloads();
        [TestMethod] public void CreateInstance_And_GetConstructor() => Inner.CreateInstance_And_GetConstructor();
        [TestMethod] public void GetCallingMethod_ReturnsExpectedMethod() => Inner.GetCallingMethod_ReturnsExpectedMethod();
    }
}
