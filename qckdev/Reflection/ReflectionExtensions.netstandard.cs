#if PORTABLE // EXCLUDE.
#else

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using qckdev.Reflection;

namespace qckdev.Reflection
{
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
        {
            return (type.IsPrimitive || type.IsValueType || type == stringType);
        }

        // TODO: Quitar
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
        [Obsolete("Already exists in .NET Framework.")]
        public static object CreateInstance(this Type objectType, params object[] parameters)
        {
            return ReflectionHelper.CreateInstance(objectType, parameters);
        }

        // TODO: Quitar
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
        [Obsolete("Already exists in .NET Framework.")]
        public static ConstructorInfo GetConstructor(this Type objectType, params object[] parameters)
        {
            return ReflectionHelper.GetConstructor(objectType, parameters);
        }

        // TODO: Quitar
        [Obsolete("Already exists in .NET Framework.")]
        public static MethodInfo GetMethod(this Type type, string methodName, params object[] parameters)
        {
            return GetMethod(type, methodName, Type.GetTypeArray(parameters));
        }

        // TODO: Quitar
        [Obsolete("Already exists in .NET Framework.")]
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

#endif