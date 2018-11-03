using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace qckdev
{

    /// <summary>
    /// Provides a dictionary with all command arguments.
    /// </summary>
    /// <remarks>
    /// Format: "/Param1:Value /Param2 /Param3:Value".
    /// </remarks>
    public class CommandArgsDictionary : Dictionary<string, string>
    {

        private CommandArgsDictionary() { }

        private CommandArgsDictionary(IEqualityComparer<string> comparer)
            : base(comparer) { }

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
            string key = null;
            string value = null;
            int pIniKey = 0;
            int pEndKey = 0;

            if (args == null)
                throw new ArgumentNullException("args");
            else if (ignoreCase)
                lst = new CommandArgsDictionary(StringComparer.InvariantCultureIgnoreCase);
            else
                lst = new CommandArgsDictionary();

            for (int i = 0; i <= args.Length - 1; i++)
            {
                key = i.ToString();
                value = args[i];
                pIniKey = args[i].IndexOf('/');
                if (pIniKey >= 0)
                {
                    pEndKey = args[i].IndexOf(':');
                    if (pEndKey < 0)
                    {
                        key = args[i].Substring(pIniKey + 1, args[i].Length - pIniKey - 1);
                        value = string.Empty;
                    }
                    else
                    {
                        key = args[i].Substring(pIniKey + 1, pEndKey - pIniKey - 1);
                        value = args[i].Substring(pEndKey + 1);
                        value = value.Substring("\"", "\""); // Quitar comillas (ejemplo: /Parametro:"hola mundo" => Parametro | hola mundo.
                    }
                }
                if (!lst.ContainsKey(key))
                {
                    lst.Add(key, value);
                }
            }
            return lst;
        }

    }
}