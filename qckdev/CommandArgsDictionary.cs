using System;
using System.Collections.Generic;

namespace qckdev
{

    /// <summary>
    /// Provides a dictionary with all command arguments.
    /// </summary>
    /// <remarks>
    /// Format: "/Param1:Value /Param2 /Param3:Value".
    /// </remarks>
    [Serializable]
    public sealed class CommandArgsDictionary : Dictionary<string, string>
    {

        private CommandArgsDictionary() { }

        private CommandArgsDictionary(IEqualityComparer<string> comparer)
            : base(comparer) { }

#if PORTABLE // EXCLUDE.
#else
        private CommandArgsDictionary(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            :base(info, context) { }
#endif

        /// <summary>
        /// Returns a <see cref="CommandArgsDictionary"/> with the information splitted. In uppercase.
        /// </summary>
        /// <param name="args">Command arguments.</param>
        /// <returns>A <see cref="CommandArgsDictionary"/> with the information splitted.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="args"/> is null.</exception>
        public static CommandArgsDictionary Create(params string[] args)
        {
            return Create(true, args);
        }

        /// <summary>
        /// Returns a <see cref="CommandArgsDictionary"/> with the information splitted.
        /// </summary>
        /// <param name="ignoreCase">
        /// Performs a case-insensitive string comparison using the word comparison rules of the invariant culture.
        /// </param>
        /// <param name="args">Command arguments.</param>
        /// <returns>A <see cref="CommandArgsDictionary"/> with the information splitted.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="args"/> is null.</exception>
        public static CommandArgsDictionary Create(bool ignoreCase, params string[] args)
        {
            CommandArgsDictionary lst;
            if (args == null)
                throw new ArgumentNullException(nameof(args));
            else if (ignoreCase)
                lst = new CommandArgsDictionary(StringComparer.OrdinalIgnoreCase);
            else
                lst = new CommandArgsDictionary();

            for (int i = 0; i <= args.Length - 1; i++)
            {
                string key = i.ToString();
                string value = args[i];
                int pIniKey = args[i].IndexOf('/');

                if (pIniKey >= 0)
                {
                    int pEndKey = args[i].IndexOf(':');

                    if (pEndKey < 0)
                    {
                        key = args[i].Substring(pIniKey + 1, args[i].Length - pIniKey - 1);
                        value = string.Empty;
                    }
                    else
                    {
                        key = args[i].Substring(pIniKey + 1, pEndKey - pIniKey - 1);
                        value = args[i].Substring(pEndKey + 1);
                        value = Substring(value, "\"", "\""); // Quitar comillas (ejemplo: /Parametro:"hola mundo" => Parametro | hola mundo.
                    }
                }
                if (!lst.ContainsKey(key))
                {
                    lst.Add(key, value);
                }
            }
            return lst;
        }

        /// <summary>
        /// Retrieves a substring from this instance. The substring starts at a specified character and end at other specified character.
        /// </summary>
        /// <param name="value">The string to test.</param>
        /// <param name="start">The starting character.</param>
        /// <param name="end">The ending character.</param>
        /// <returns>
        /// A <see cref="String"/> equivalent to the substring that begins and ends at a specified characters.
        /// or the original <paramref name="value"/> if it does not contains starts and ends characters.
        /// </returns>
        private static string Substring(string value, string start, string end)
        {
            string rdo = value;

            if (!string.IsNullOrEmpty(value) && value.Length >= 2 && value.StartsWith(start) && value.EndsWith(end))
                rdo = value.Substring(1, value.Length - 2);

            return rdo;
        }

    }
}