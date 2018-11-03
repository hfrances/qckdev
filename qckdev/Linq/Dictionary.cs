using System;
using System.Collections.Generic;
using System.Text;

namespace qckdev.Linq
{

    /// <summary>
    /// Provides a set of static methods for querying objects that implement <see cref="IDictionary{TKey, TValue}"/>.
    /// </summary>
    public static class Dictionary
    {

        /// <summary>
        /// Gets the value associated with the specified key or null if it does not exist in the dictionary.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key">The key whose value to get.</param>
        /// <exception cref="ArgumentException">key is null.</exception>
        public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TValue : class
        {
            return TryGetValue(dictionary, key, null);
        }

        /// <summary>
        /// Gets the value associated with the specified key or null if it does not exist in the dictionary. Only for structs.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="defaultValue">The <typeparamref name="TValue"/> returned when the <typeparamref name="TKey"/> was not found.</param>
        /// <exception cref="ArgumentException">key is null.</exception>
        public static TValue? TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue? defaultValue) where TValue : struct
        {
            TValue? rdo = null;

            if (dictionary.TryGetValue(key, out TValue value))
                rdo = value;
            else
                rdo = defaultValue;
            return rdo;
        }

        /// <summary>
        /// Gets the value associated with the specified key or <paramref name="defaultValue"/> if it does not exist in the dictionary.
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="key">The key whose value to get.</param>
        /// <param name="defaultValue">The <typeparamref name="TValue"/> returned when the <typeparamref name="TKey"/> was not found.</param>
        /// <exception cref="ArgumentException">key is null.</exception>
        public static TValue TryGetValue<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue defaultValue) where TValue : class
        {
            if (!dictionary.TryGetValue(key, out TValue rdo))
                rdo = defaultValue;
            return rdo;
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <see cref="IDictionary{TKey, TValue}"/> if the key does not exists yet.
        /// </summary>
        /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
        /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
        /// <param name="dictionary"></param>
        /// <param name="key">The object to use as the key of the element to add.</param>
        /// <param name="value">The object to use as the value of the element to add.</param>
        /// <returns>Returns true if the key did not exist yet; otherwise, false.</returns>
        /// <exception cref="ArgumentNullException">key is null.</exception>
        /// <exception cref="NotSupportedException">The <see cref="IDictionary{TKey, TValue}"/> is read-only.</exception>
        public static bool AddIfNotExists<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, TValue value)
        {
            bool rdo = true;

            if (dictionary.ContainsKey(key))
                rdo = false;
            else
                dictionary.Add(key, value);
            return rdo;
        }

    }
}
