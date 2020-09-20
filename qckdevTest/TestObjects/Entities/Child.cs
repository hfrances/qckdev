using System;

namespace qckdevTest.TestObjects.Entities
{
    class Child
    {

        public Guid Id { get; set; }
        public int Line { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }


        public Parent Parent { get; set; }

    }
}
