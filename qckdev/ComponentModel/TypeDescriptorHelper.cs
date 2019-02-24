#if PORTABLE // EXCLUDE.
#else

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace qckdev.ComponentModel
{

    /// <summary>
    /// Provides information about the characteristics for a component, such as its attributes, properties, and events. 
    /// This class cannot be inherited.
    /// </summary>
    public static class TypeDescriptorHelper
    {

        /// <summary>
        /// Returns the collection of properties for a specified type of component.
        /// </summary>
        /// <param name="type">A <see cref="Type"/> that represents the component to get properties for.</param>
        /// <returns>
        /// A <see cref="PropertyDescriptorCollection"/> with the properties for a specified type of component.
        /// </returns>
        public static PropertyDescriptorCollection GetCachedPropertyDescriptors(this Type type)
        {
            return PropertyDescriptorCache.Default.Cache(type);
        }

    }

}

#endif