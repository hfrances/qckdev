#if PORTABLE // EXCLUDE.
#else

using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace qckdev.Reflection
{
    public static partial class ReflectionHelper
    {

        /// <summary>
        /// Creates an instance of the type designated by the specified generic type parameter.
        /// It works with public and non-public constructors.
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
            object o;
            ConstructorInfo c;

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
            ConstructorInfo[] carr;
            int i = 0;

            carr = objectType.GetConstructors(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            while (i < carr.Length && c == null)
            {
                var itemParameters = carr[i].GetParameters();
                if (itemParameters.Length == parameters.Length)
                {
                    bool b = true;
                    int iParam = 0;

                    while (iParam < parameters.Length && b)
                    {
                        b = parameters[iParam] == null || itemParameters[iParam].ParameterType.IsInstanceOfType(parameters[iParam]);
                        iParam += 1;
                    }
                    if (b)
                        c = carr[i];
                }
                i += 1;
            }
            return c;
        }

        /// <summary>
        /// Returns a <see cref="MethodBase"/> object representing the method that called to the executing method.
        /// </summary>
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static MethodBase GetCallingMethod()
        {
            var frame = new StackFrame(2);

            return frame.GetMethod();
        }

    }
}

#endif