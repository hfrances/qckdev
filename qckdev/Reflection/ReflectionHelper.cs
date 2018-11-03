using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace qckdev.Reflection
{

    /// <summary>
    /// Provides helpful methods for reflection. This class cannot be inherited.
    /// </summary>
    public static class ReflectionHelper
    {


        /// <summary>
        /// Serves as a hash function for a object list.
        /// </summary>
        /// <param name="values">List of elements for HashCode creation.</param>
        /// <returns>A hash code for the object list.</returns>
        public static int GetHashCode(params object[] values)
        {
            return GetHashCode((IEnumerable<object>)values);
        }

        /// <summary>
        /// Serves as a hash function for a object list.
        /// </summary>
        /// <param name="values">List of elements for HashCode creation.</param>
        /// <returns>A hash code for the object list.</returns>
        public static int GetHashCode(IEnumerable<object> values)
        {
            int rdo = 0;

            foreach (var value in values)
            {
                rdo ^= (int)(uint)(value?.GetHashCode() ?? 0);
            }
            return rdo;
        }

        /// <summary>
        /// Creates an instance of the type designated by the specified generic type parameter.
        /// </summary>
        /// <param name="objectType">The type to create.</param >
        /// <param name="parameters">
        /// An array of arguments that match in number, order, and type the parameters of
        /// the constructor to invoke. If args is an empty array or null, the constructor
        /// that takes no parameters (the default constructor) is invoked.
        /// </param>
        /// <returns>A reference to the newly created object.</returns>
        public static object CreateInstance(Type objectType, params object[] parameters)
        {
            object o = null;
            ConstructorInfo c = null;

            c = GetConstructor(objectType, parameters);
            if (c == null)
                o = null; // TODO: MissingMethodException or throwError parameter.
            else
                o = c.Invoke(parameters);
            return o;
        }

        /// <summary>
        /// Creates an instance of the type designated by the specified generic type parameter.
        /// </summary>
        /// <typeparam name="T">The type to create.</typeparam>
        /// <param name="parameters">
        /// An array of arguments that match in number, order, and type the parameters of
        /// the constructor to invoke. If args is an empty array or null, the constructor
        /// that takes no parameters (the default constructor) is invoked.
        /// </param>
        /// <returns>A reference to the newly created object.</returns>
        public static T CreateInstance<T>(params object[] parameters)
        {
            return (T)CreateInstance(typeof(T), parameters);
        }

        /// <summary>
        /// Searches for a public/non-public instance constructor whose parameters match the types in the specified <paramref name="objectType"/>.
        /// </summary>
        /// <param name="objectType">The <see cref="Type"/> where find the constructor.</param>
        /// <param name="parameters">
        /// An array of arguments that match in number, order, and type the parameters of
        /// the constructor to invoke. If args is an empty array or null, the constructor
        /// that takes no parameters (the default constructor) is returned.
        /// </param>
        /// <returns>
        /// A <see cref="ConstructorInfo"/> object representing the public instance constructor
        /// whose parameters match the types in the parameter type array, if found; otherwise, null.
        /// </returns>
        public static ConstructorInfo GetConstructor(Type objectType, params object[] parameters)
        {
            ConstructorInfo c = null;
            int i = 0;

            ConstructorInfo[] carr = null;
            carr = objectType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            while (i < carr.Length && c == null)
            {
                var itemParameters = carr[i].GetParameters();
                if (itemParameters.Length == parameters.Length)
                {
                    bool b = true;
                    int iParam = 0;

                    while (iParam < parameters.Length && b )
                    {
                        b = parameters[iParam] == null || itemParameters[iParam].ParameterType.IsInstanceOfType(parameters[iParam]);
                        iParam += 1;
                    }
                    if (b )
                        c = carr[i];
                }
                i += 1;
            }
            return c;
        }

        /// <summary>
        /// Searches for a public/non-public instance constructor whose parameters match the types in the specified <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The <see cref="Type"/> where find the constructor.</typeparam>
        /// <param name="parameters">
        /// An array of arguments that match in number, order, and type the parameters of
        /// the constructor to invoke. If args is an empty array or null, the constructor
        /// that takes no parameters (the default constructor) is invoked.
        /// </param>
        /// <returns>
        /// A <see cref="ConstructorInfo"/> object representing the public instance constructor
        /// whose parameters match the types in the parameter type array, if found; otherwise, null.
        /// </returns>
        public static ConstructorInfo GetConstructor<T>(params object[] parameters)
        {
            return GetConstructor(typeof(T), parameters);
        }

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
        public static Dictionary<string, PropertyInfo> CacheProperties(Type type)
        {
            return PropertyInfoCache.Default.Cache(type);
        }

        /// <summary>
        /// Returns the collection of properties for a specified type of component.
        /// </summary>
        /// <param name="type">A <see cref="Type"/> that represents the component to get properties for.</param>
        /// <returns>
        /// A <see cref="PropertyDescriptorCollection"/> with the properties for a specified type of component.
        /// </returns>
        public static PropertyDescriptorCollection CachePropertyDescriptors(Type type)
        {
            return PropertyDescriptorCache.Default.Cache(type);
        }


        [MethodImpl(MethodImplOptions.NoInlining)]
        public static MethodBase GetCallingMethod()
        {
            var frame = new StackFrame(2);

            return frame.GetMethod();
        }


    }

}
