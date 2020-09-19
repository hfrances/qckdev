#if PORTABLE // EXCLUDE.
#else

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace qckdev.Reflection
{
    public static partial class ReflectionHelper
    {

        /// <summary>
        /// Searches for the specified method whose parameters match the specified argument
        /// types and modifiers, using the specified binding constraints.
        /// Works with generic method.
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
        /// </param>
        /// For generic types, set generic parameter types before the parameters for the method.
        /// <returns>
        /// An object representing the method that matches the specified requirements, if
        /// found; otherwise, null.
        /// </returns>
        public static MethodInfo GetMethodExt(Type type, string name, params Type[] types)
            => GetMethodExt(type, name, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public, types);

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
        public static MethodInfo GetMethodExt(Type type, string name, BindingFlags bindingAttr, params Type[] types)
        {
            var query = type
                .GetMethods(bindingAttr)
                .Where(x => x.Name == name)
                .Select(x => GetEqualityLevel(x, types))
                .OrderBy(x => x.EqualityLevel)
                .FirstOrDefault(x => x.EqualityLevel > 0);

            return query?.Method;
        }

        static EqualityResult GetEqualityLevel(MethodInfo method, Type[] types)
        {
            long equalityLevel = 0;
            var genericTypes = method.GetGenericArguments();
            var parameters = method.GetParameters();
            EqualityResult equalityResult;

            if (genericTypes.Length + parameters.Length == types.Length)
            {

                if (genericTypes.Any())
                {
                    equalityResult = GetEqualityLevelParameters(genericTypes, types);
                    if (equalityResult.EqualityLevel > 0)
                    {
                        var matchedTypes = equalityResult.MatchedTypes;

                        method = method.MakeGenericMethod(
                            types.Take(matchedTypes.Count()).ToArray());
                        parameters = method.GetParameters();
                        equalityLevel = equalityResult.EqualityLevel;
                    }
                }
                else
                {
                    equalityLevel = 1;
                }

                equalityResult = GetEqualityLevelParameters(
                    parameters.Select(x => x.ParameterType),
                    types.Skip(genericTypes.Length)
                );
                equalityLevel *= equalityResult.EqualityLevel;
            }
            return new EqualityResult
            {
                Method = method,
                EqualityLevel = equalityLevel
            };
        }

        static EqualityResult GetEqualityLevelParameters(IEnumerable<Type> parameterTypes, IEnumerable<Type> types)
        {
            long equalityLevel = 1;
            var matchedTypeList = new List<Type>();
            var paramEtor = parameterTypes.GetEnumerator();
            var typeEtor = types.GetEnumerator();

            while (equalityLevel > 0 && paramEtor.MoveNext() && typeEtor.MoveNext())
            {
                Type parameterType = paramEtor.Current, type = typeEtor.Current;
                long tmp;

                equalityLevel = IsEquatableFrom(parameterType, type);
                if (equalityLevel == 0 && parameterType.IsGenericParameter 
                    && (tmp = IsEquatableFrom(parameterType.BaseType, type)) > 0)
                {
                    equalityLevel = 1024 * tmp;
                }

                if (equalityLevel > 0)
                {
                    matchedTypeList.Add(parameterType);
                }
            }
            return new EqualityResult
            {
                EqualityLevel = equalityLevel,
                MatchedTypes = matchedTypeList.ToArray()
            };
        }

        /// <summary>
        /// Determines whether an instance of a specified type can be assigned to an instance
        /// of the current type.
        /// </summary>
        /// <param name="type">The current type.</param>
        /// <param name="c">The type to compare with the current type.</param>
        /// <returns>
        /// A number that set the equality level of the assignation. 
        /// As higher is the number, further is the assignation. 
        /// --or 
        /// 0 when assignation is not possible.
        /// </returns>
        [SuppressMessage("Critical Code Smell", "S3776:Cognitive Complexity of methods should not be too high", Justification = "Cognitive complexity could be higher if this method is split.")]
        static long IsEquatableFrom(Type type, Type c)
        {
            long equalityLevel = 0;

            if (c == type)
            {
                equalityLevel = 1;
            }
            else if (c.BaseType == type)
            {
                equalityLevel = 1024;
            }
            else
            {
                var etor = c.GetInterfaces().GetEnumerator();

                while (etor.MoveNext())
                {
                    var e = (Type)etor.Current;
                    long tmp;

                    tmp = 1024 * IsEquatableFrom(type, e);
                    if (tmp > 0)
                    {
                        if (equalityLevel == 0)
                        {
                            equalityLevel = tmp;
                        }
                        else
                        {
                            equalityLevel = Math.Max(equalityLevel, tmp);
                        }
                    }
                }

                if (equalityLevel == 0 && c.BaseType != null)
                {
                    equalityLevel = 1024 * IsEquatableFrom(type, c.BaseType);
                }
            }
            return equalityLevel;
        }

        sealed class EqualityResult
        {

            public MethodInfo Method { get; set; }

            public IEnumerable<Type> MatchedTypes { get; set; }

            public long EqualityLevel { get; set; }

        }

    }
}

#endif