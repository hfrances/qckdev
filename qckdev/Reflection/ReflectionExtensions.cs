using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace qckdev.Reflection
{

    /// <summary>
    /// Defines the extension methods to the <see cref="System.Reflection"/> namespace.
    /// </summary>
    public static partial class ReflectionExtensions
    {

        /// <summary>
        /// When overridden in a derived class, searches for the properties of the current <see cref="Type"/>, using the specified binding constraints. 
        /// Includes all levels for interfaces.
        /// </summary>
        /// <returns>
        /// An array of <see cref="PropertyInfo"/> objects representing all properties
        /// of the current <see cref="Type"/> that match the specified binding constraints.-or-
        /// An empty array of type <see cref="PropertyInfo"/>, if the current <see cref="Type"/>
        /// does not have properties, or if none of the properties match the binding constraints.
        /// </returns>
#if PORTABLE
        public static IEnumerable<PropertyInfo> GetPropertiesFull(this Type type)
        {
            return type.GetRuntimeProperties().OrderBy(x => x.Name);
        }
#else
        public static IEnumerable<PropertyInfo> GetPropertiesFull(this Type type)
        {
            return GetPropertiesFull(type, BindingFlags.Instance | BindingFlags.Public);
        }

        /// <summary>
        /// When overridden in a derived class, searches for the properties of the current System.Type, using the specified binding constraints. 
        /// Includes all levels for interfaces.
        /// </summary>
        /// <param name="type">Type declaration.</param>
        /// <param name="bindingAttr">A bitmask comprised of one or more <see cref="BindingFlags"/> that specify how the search is conducted.-or- Zero, to return null.</param>
        /// <returns>
        /// An array of <see cref="PropertyInfo"/> objects representing all properties
        /// of the current <see cref="Type"/> that match the specified binding constraints.-or-
        /// An empty array of type <see cref="PropertyInfo"/>, if the current <see cref="Type"/>
        /// does not have properties, or if none of the properties match the binding constraints.
        /// </returns>
        public static IEnumerable<PropertyInfo> GetPropertiesFull(this Type type, BindingFlags bindingAttr)
        {
            IEnumerable<PropertyInfo> rdo;

            rdo = type.GetProperties(bindingAttr);
            if (type.IsInterface)
                rdo = rdo.Concat(type.GetInterfaces().SelectMany(x => GetPropertiesFull(x, bindingAttr)));
            return rdo.OrderBy(x => x.Name);
        }

#endif

        /// <summary>
        /// When overridden in a derived class, searches for the properties of the current <see cref="Type"/>, using the specified binding constraints. Includes all levels for interfaces.
        /// </summary>
        /// <param name="type">A <see cref="Type"/> that represents the component to get properties for.</param>
        /// <returns>
        /// An <see cref="Dictionary{TKey, TValue}"/> of <see cref="PropertyInfo"/> objects representing all properties
        /// of the current <see cref="Type"/> that match the specified binding constraints.-or-
        /// An empty array of type <see cref="PropertyInfo"/>, if the current <see cref="Type"/>
        /// does not have properties, or if none of the properties match the binding constraints.
        /// </returns>
        public static Dictionary<string, PropertyInfo> GetCachedProperties(this Type type)
        {
            return PropertyInfoCache.Default.Cache(type);
        }

    }
}
