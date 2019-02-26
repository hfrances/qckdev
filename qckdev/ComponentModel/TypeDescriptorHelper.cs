#if PORTABLE // EXCLUDE.
#else

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Linq;
using qckdev.Linq;

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
        public static PropertyDescriptorCollection GetCachedPropertyDescriptors(Type type)
        {
            return PropertyDescriptorCache.Default.Cache(type);
        }

        /// <summary>
        /// Returns the collection of properties for a specified type of component.
        /// Includes all levels for interfaces.
        /// </summary>
        /// <param name="componentType">A <see cref="System.Type"/> that represents the component to get properties for.</param>
        /// <returns>A <see cref="System.ComponentModel.PropertyDescriptorCollection"/> with the properties for a specified type of component.</returns>
        public static PropertyDescriptorCollection GetPropertiesFull(Type componentType)
        {
            var rdo = new List<PropertyDescriptor>();

            rdo.AddRange(TypeDescriptor.GetProperties(componentType));
            if (componentType.IsInterface)
                componentType.GetInterfaces()
                    .SelectMany(x => GetPropertiesFull(x).OfType<PropertyDescriptor>())
                    .ForEach(rdo.Add);

            return new PropertyDescriptorCollection(rdo.ToArray(), readOnly: true);
        }

    }

}

#endif