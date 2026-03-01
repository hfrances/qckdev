using System;
using System.Reflection;

namespace qckdevTest.Common
{
    public sealed class EnumHelperTest
    {
        readonly IAssert Assert;

        [Flags]
        enum TestFlags
        {
            None = 0,
            A = 1,
            B = 2,
            C = 4
        }

        public EnumHelperTest(IAssert assert)
        {
            Assert = assert;
        }

        public void ContainsAll_True_WhenAllFlagsExist()
        {
            var result = InvokeContainsAll(TestFlags.A | TestFlags.B, TestFlags.A, TestFlags.B);
            Assert.IsTrue(result);
        }

        public void ContainsAll_False_WhenCombinedFlagIsNotFullyContained()
        {
            var result = InvokeContainsAll(TestFlags.A, TestFlags.A | TestFlags.B);
            Assert.IsFalse(result);
        }

        static bool InvokeContainsAll(TestFlags value, params TestFlags[] others)
        {
            var enumType = typeof(qckdev.Key).Assembly.GetType("qckdev.EnumHelper", throwOnError: true);
            var method = enumType.GetMethod("ContainsAll", BindingFlags.Public | BindingFlags.Static);
            var generic = method.MakeGenericMethod(typeof(TestFlags));
            return (bool)generic.Invoke(null, new object[] { value, others });
        }
    }
}
