using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using qckdev.Reflection;

namespace qckdev.Reflection
{


    public static class Extensions
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
        {
            return (type.IsPrimitive || type.IsValueType || type == stringType);
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
        public static object CreateInstance(this Type objectType, params object[] parameters)
        {
            return ReflectionHelper.CreateInstance(objectType, parameters);
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
        public static ConstructorInfo GetConstructor(this Type objectType, params object[] parameters)
        {
            return ReflectionHelper.GetConstructor(objectType, parameters);
        }

        /// <summary>
        /// When overridden in a derived class, searches for the properties of the current <see cref="Type"/>, using the specified binding constraints. Includes all levels for interfaces.
        /// </summary>
        /// <returns>
        /// An array of <see cref="PropertyInfo"/> objects representing all properties
        /// of the current <see cref="Type"/> that match the specified binding constraints.-or-
        /// An empty array of type <see cref="PropertyInfo"/>, if the current <see cref="Type"/>
        /// does not have properties, or if none of the properties match the binding constraints.
        /// </returns>
        public static IEnumerable<PropertyInfo> GetProperties2(this Type type)
        {
            return GetProperties2(type, BindingFlags.Instance | BindingFlags.Public);
        }

        /// <summary>
        /// When overridden in a derived class, searches for the properties of the current System.Type, using the specified binding constraints. Includes all levels for interfaces.
        /// </summary>
        /// <param name="type">Type declaration.</param>
        /// <param name="bindingAttr">A bitmask comprised of one or more <see cref="BindingFlags"/> that specify how the search is conducted.-or- Zero, to return null.</param>
        /// <returns>
        /// An array of <see cref="PropertyInfo"/> objects representing all properties
        /// of the current <see cref="Type"/> that match the specified binding constraints.-or-
        /// An empty array of type <see cref="PropertyInfo"/>, if the current <see cref="Type"/>
        /// does not have properties, or if none of the properties match the binding constraints.
        /// </returns>
        public static IEnumerable<PropertyInfo> GetProperties2(this Type type, BindingFlags bindingAttr)
        {
            IEnumerable<PropertyInfo> rdo;

            rdo = type.GetProperties(bindingAttr);
            if (type.IsInterface)
                rdo = rdo.Concat(type.GetInterfaces().SelectMany(x => GetProperties2(x, bindingAttr)));
            return rdo.OrderBy(x => x.Name);
        }

        public static MethodInfo GetMethod(this Type type, string methodName, params object[] parameters)
        {
            return GetMethod(type, methodName, Type.GetTypeArray(parameters));
        }

        public static MethodInfo GetMethod(this Type type, string methodName, params Type[] parameters)
        {
            MethodInfo m = null;
            int i = 0;

            MethodInfo[] carr = null;
            carr = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                             .Where(x => x.Name == methodName)
                             .ToArray();

            while (i < carr.Length && m == null)
            {
                var itemParameters = carr[i].GetParameters();
                if (itemParameters.Length == parameters.Length)
                {
                    bool b = true;
                    int iParam = 0;

                    while (iParam < parameters.Length && b)
                    {
                        b = parameters[iParam] == null || 
                            parameters[iParam].IsAssignableFrom(itemParameters[iParam].ParameterType);
                        iParam += 1;
                    }
                    if (b)
                        m = carr[i];
                }
                i += 1;
            }
            if (m == null && type.BaseType != null)
                m = GetMethod(type.BaseType, methodName, parameters);
            return m;
        }

    }
}
