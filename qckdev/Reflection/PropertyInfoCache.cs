using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace qckdev.Reflection
{

    sealed class PropertyInfoCache
    {

        public static PropertyInfoCache Default { get; } 
            = new PropertyInfoCache();

        private Dictionary<Type, Dictionary<string, PropertyInfo>> InnerList { get; } 
            = new Dictionary<Type, Dictionary<string, PropertyInfo>>();

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
        public Dictionary<string, PropertyInfo> Cache(Type type)
        {
            Dictionary<string, PropertyInfo> properties;

            if (!InnerList.TryGetValue(type, out properties))
            {
                properties = type.GetPropertiesFull()
                    .ToDictionary(x => x.Name, y => y);
                InnerList.Add(type, properties);
            }
            return properties;
        }

    }

}
