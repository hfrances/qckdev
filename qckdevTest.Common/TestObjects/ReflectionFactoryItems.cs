namespace qckdevTest.Common.TestObjects
{
    internal sealed class ReflectionFactoryItems
    {
        public sealed class PublicCtorClass
        {
            public PublicCtorClass(string name, int number)
            {
                Name = name;
                Number = number;
            }

            public string Name { get; }
            public int Number { get; }
        }

        public sealed class PrivateCtorClass
        {
            private PrivateCtorClass(string value)
            {
                Value = value;
            }

            public string Value { get; }

            public static PrivateCtorClass Create(string value)
                => new PrivateCtorClass(value);
        }

        public sealed class NoMatchingCtorClass
        {
            public NoMatchingCtorClass(int value)
            {
                Value = value;
            }

            public int Value { get; }
        }

        public sealed class PlainObject
        {
        }
    }
}
