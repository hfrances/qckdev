using System;

namespace qckdevTest.Common
{
    public interface IAssert
    {
        void AreEqual(object expected, object actual, string message = null);
        void IsTrue(bool condition, string message = null);
        void IsFalse(bool condition, string message = null);
        void IsNull(object value, string message = null);
        void IsNotNull(object value, string message = null);
        void Throws<TException>(Action action) where TException : Exception;
    }
}
