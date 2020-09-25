using System.Diagnostics.CodeAnalysis;

namespace qckdevTest.TestObjects
{

    class TestClassBase
    {
        public string Property1 { get; }
        public int Property2 { get; set; }
        [SuppressMessage("Critical Code Smell", "S2376:Write-only properties should not be used", Justification = "Necessary for test this possibility.")]
        public bool Property3 { set => Property4 = value; }
        [SuppressMessage("Code Smell", "IDE0052:Private property can be converted to a method as its get accessor is never invoked.", Justification = "Necessary for test this possibility.")]
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
        [SuppressMessage("Critical Code Smell", "S2376:Write-only properties should not be used", Justification = "Necessary for test this possibility.")]
        bool Property3 { set; }
        bool Property4 { get; set; }

    }

    interface ITestInterface : ITestInterfaceBase
    {
        string Property5 { get; }

    }
}
