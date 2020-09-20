using System;
using System.Collections.Generic;

namespace qckdevTest.TestObjects.Entities
{
    class Parent
    {

        public Guid Id { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }


        public IEnumerable<Child> Childs { get; set; }
    }
}
