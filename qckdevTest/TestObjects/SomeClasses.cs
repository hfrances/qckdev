using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace qckdevTest.TestObjects
{

    class TestClassBase
    {
        public string Property1 { get; }
        public int Property2 { get; set; }
        public bool Property3 { set => Property4 = value; }
        private bool Property4 { get; set; }

    }

    class TestClass : TestClassBase
    {
        public string Property5 { get; }

    }

    interface ITestInterfaceBase
    {
        string Property1 { get; }
        int Property2 { get; set; }
        bool Property3 { set; }
        bool Property4 { get; set; }

    }

    interface ITestInterface : ITestInterfaceBase
    {
        string Property5 { get; }

    }
}
