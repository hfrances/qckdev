#if PORTABLE

namespace System
{
    /// <summary>
    /// Indicates that a class can be serialized. This attribute is a mock for Portable libraries (it has not any utility).
    /// This class cannot be inherited.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate, Inherited = false)]
    internal sealed class SerializableAttribute : Attribute
    {
    }
}

#else
    // Already exists in the main framework.
#endif