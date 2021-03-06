﻿#if PORTABLE // EXCLUDE.
#else

using System;
using System.ComponentModel;
using System.Collections.Generic;

namespace qckdev.ComponentModel
{

    sealed class PropertyDescriptorCache
    {

        public static PropertyDescriptorCache Default { get; }
            = new PropertyDescriptorCache();

        private Dictionary<Type, PropertyDescriptorCollection> InnerList { get; }
            = new Dictionary<Type, PropertyDescriptorCollection>();

        /// <summary>
        /// Returns the collection of properties for a specified type of component.
        /// </summary>
        /// <param name="type">A <see cref="Type"/> that represents the component to get properties for.</param>
        /// <returns>
        /// A <see cref="PropertyDescriptorCollection"/> with the properties for a specified type of component.
        /// </returns>
        public PropertyDescriptorCollection Cache(Type type)
        {
            PropertyDescriptorCollection properties;

            if (!InnerList.TryGetValue(type, out properties))
            {
                properties = TypeDescriptorHelper.GetPropertiesFull(type);
                InnerList.Add(type, properties);
            }
            return properties;
        }

    }

}

#endif