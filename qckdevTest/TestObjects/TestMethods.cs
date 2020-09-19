using System;
using System.Collections.Generic;

namespace qckdevTest.TestObjects
{
    class TestMethods
    {

        public void Method<T>(IEnumerable<T> values)
            => throw new NotImplementedException();

        public void Method<T>(IList<T> values)
            => throw new NotImplementedException();

        public void Method<T>(ICollection<T> values)
            => throw new NotImplementedException();

    }
}
