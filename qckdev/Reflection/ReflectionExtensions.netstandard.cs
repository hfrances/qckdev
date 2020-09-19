#if PORTABLE // EXCLUDE.
#else

using System;
using System.Reflection;

namespace qckdev.Reflection
{

    /// <summary>
    /// Provides a set of static methods for <see cref="System.Reflection"/> namespace.
    /// </summary>
    public static partial class ReflectionExtensions
    {

        readonly static Type stringType = typeof(string);

        /// <summary>
        /// Returns a value indicating whether the <see cref="Type"/> is one of the primitive types, a value type or the <see cref="string"/> type.
        /// </summary>
        /// <param name="type"><see cref="Type"/> to check.</param>
        /// <returns>
        /// true if the the <see cref="Type"/> is one of the primitive types, a value type or the <see cref="string"/> type; otherwise, false.
        /// </returns>
        public static bool HasOwnValue(this Type type)
            => (type.IsPrimitive || type.IsValueType || type == stringType);

        /// <summary>
        /// Searches for the specified method whose parameters match the specified argument
        /// types and modifiers, using the specified binding constraints.
        /// Works with generic methods.
        /// </summary>
        /// <param name="type">The <see cref="System.Type"/> where find the method.</param>
        /// <param name="name">
        /// The string containing the name of the method to get.
        /// </param>
        /// <param name="types">
        /// An array of <see cref="System.Type"/> objects representing the number, order, and type of the
        /// parameters for the method to get. 
        /// -or- 
        /// An empty array of <see cref="System.Type"/> objects
        /// (as provided by the <see cref="System.Type.EmptyTypes"/> field) to get a method that takes
        /// no parameters.
        /// For generic types, set generic parameter types before the parameters for the method.
        /// </param>
        /// <returns>
        /// An object representing the method that matches the specified requirements, if
        /// found; otherwise, null.
        /// </returns>
        public static MethodInfo GetMethodExt(this Type type, string name, params Type[] types)
            => ReflectionHelper.GetMethodExt(type, name, types);

        /// <summary>
        /// Searches for the specified method whose parameters match the specified argument
        /// types and modifiers, using the specified binding constraints.
        /// Works with generic methods.
        /// </summary>
        /// <param name="type">The <see cref="System.Type"/> where find the method.</param>
        /// <param name="name">The string containing the name of the method to get.</param>
        /// <param name="bindingAttr">
        /// A bitmask comprised of one or more <see cref="BindingFlags"/> that specify
        /// how the search is conducted. 
        /// -or-
        /// Zero, to return null.
        /// </param>
        /// <param name="types">
        /// An array of <see cref="System.Type"/> objects representing the number, order, and type of the
        /// parameters for the method to get. 
        /// -or- 
        /// An empty array of <see cref="System.Type"/> objects
        /// (as provided by the <see cref="System.Type.EmptyTypes"/> field) to get a method that takes
        /// no parameters.
        /// For generic types, set generic parameter types before the parameters for the method.
        /// </param>
        /// <returns>
        /// An object representing the method that matches the specified requirements, if
        /// found; otherwise, null.
        /// </returns>
        public static MethodInfo GetMethodExt(this Type type, string name, BindingFlags bindingAttr, params Type[] types)
            => ReflectionHelper.GetMethodExt(type, name, bindingAttr, types);
        
    }
}

#endif