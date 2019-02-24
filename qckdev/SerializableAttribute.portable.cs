﻿#if PORTABLE

using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    /// <summary>
    /// Indicates that a class can be serialized. This attribute is a mock for Portable libraries (it has not any utility).
    /// This class cannot be inherited.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Delegate, Inherited = false)]
    sealed class SerializableAttribute : Attribute
    {
    }
}

#else
    // Ya existe el atributo.
#endif