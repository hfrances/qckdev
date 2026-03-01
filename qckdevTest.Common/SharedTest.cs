using System;
using System.Reflection;

namespace qckdevTest.Common
{
    public sealed class SharedTest
    {
        readonly IAssert Assert;

        public SharedTest(IAssert assert)
        {
            Assert = assert;
        }

        public void Choose_UsesConflictAction_WhenBothValuesExist()
        {
            var calls = 0;
            var result = InvokeChoose("A", "B", (a, b) =>
            {
                calls++;
                return a + b;
            });

            Assert.AreEqual("AB", result);
            Assert.AreEqual(1, calls);
        }

        public void Choose_ReturnsNonNullValue_WhenOtherIsNull()
        {
            var left = InvokeChoose("left", null, (a, b) => "invalid");
            var right = InvokeChoose(null, "right", (a, b) => "invalid");

            Assert.AreEqual("left", left);
            Assert.AreEqual("right", right);
        }

        static string InvokeChoose(string a, string b, Func<string, string, string> chooser)
        {
            var sharedType = typeof(qckdev.Key).Assembly.GetType("qckdev.Shared", throwOnError: true);
            var method = sharedType.GetMethod("Choose", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(typeof(string));
            return (string)method.Invoke(null, new object[] { a, b, chooser });
        }
    }
}
