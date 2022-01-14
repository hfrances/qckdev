using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace qckdev.Collections
{

    /// <summary>
    /// Represents a collection of keys and values where its items are alive during the time set in <see cref="CacheTimeout"/> property.
    /// </summary>
    /// <typeparam name="TKey">The type of the keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of the values in the dictionary.</typeparam>
    public class CacheDictionary<TKey, TValue> : IDictionary<TKey, TValue>
    {

        private sealed class TimedValue
        {
            public DateTime Time { get; set; }
            public TValue Content { get; set; }
        }

        static Func<TimedValue, TimeSpan, bool> FilterPredicate = (item, timeout) => DateTime.UtcNow - item.Time < timeout;

        Dictionary<TKey, TimedValue> InnerList { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheDictionary{TKey, TValue}"/> class
        /// that is empty, has the default initial capacity, and uses the default equality
        /// comparer for the key type.
        /// </summary>
        public CacheDictionary()
        {
            this.InnerList = new Dictionary<TKey, TimedValue>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheDictionary{TKey, TValue}"/> class
        /// that is empty, has the default initial capacity, and uses the specified <see cref="IEqualityComparer{T}"/>.
        /// </summary>
        /// <param name="comparer">
        /// The <see cref="IEqualityComparer{T}"/> implementation to use when
        /// comparing keys, or null to use the default <see cref="IEqualityComparer{T}"/>
        /// for the type of the key.
        /// </param>
        public CacheDictionary(IEqualityComparer<TKey> comparer)
        {
            this.InnerList = new Dictionary<TKey, TimedValue>(comparer);
        }


        public TimeSpan CacheTimeout { get; set; } = TimeSpan.FromMinutes(10);

        /// <summary>
        /// Gets a collection containing the keys in the <see cref="CacheDictionary{TKey, TValue}"/>.
        /// </summary>
        public ICollection<TKey> Keys
            => new Dictionary<TKey, TValue>.KeyCollection(FilteredList().ToDictionary(x=> x.Key, y=> y.Value));

        /// <summary>
        /// Gets a collection containing the values in the <see cref="CacheDictionary{TKey, TValue}"/>.
        /// </summary>
        public ICollection<TValue> Values
            => new Dictionary<TKey, TValue>.ValueCollection(FilteredList().ToDictionary(x => x.Key, y => y.Value));

        /// <summary>
        /// Gets the number of key/value pairs contained in the <see cref="CacheDictionary{TKey, TValue}"/>.
        /// </summary>
        public int Count
            => FilteredList().Count();

        /// <summary>
        /// Gets or sets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get or set.</param>
        /// <returns>
        /// The value associated with the specified key. If the specified key is not found,
        /// a get operation throws a System.Collections.Generic.KeyNotFoundException, and
        /// a set operation creates a new element with the specified key.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// The property is retrieved and key does not exist in the collection.
        /// </exception>
        public TValue this[TKey key]
        {
            get
            {
                var item = InnerList[key];

                if (FilterPredicate(item, this.CacheTimeout))
                {
                    return item.Content;
                }
                else
                {
                    throw new KeyNotFoundException();
                }
            }
            set => InnerList[key] = new TimedValue() { Time = DateTime.UtcNow, Content = value };
        }

        /// <summary>
        /// Adds the specified key and value to the dictionary.
        /// </summary>
        /// <param name="key">The key of the element to add.</param>
        /// <param name="value">The value of the element to add. The value can be null for reference types.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// An element with the same key already exists in the <see cref="CacheDictionary{TKey, TValue}"/>.
        /// </exception>
        public void Add(TKey key, TValue value)
        {
            if (InnerList.TryGetValue(key, out TimedValue item) && !FilterPredicate(item, this.CacheTimeout))
            {
                item.Time = DateTime.UtcNow;
                item.Content = value;
            }
            else
            {
                InnerList.Add(key, new TimedValue() { Time = DateTime.UtcNow, Content = value });
            }
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">The key of the value to get.</param>
        /// <param name="value">
        /// When this method returns, contains the value associated with the specified key,
        /// if the key is found; otherwise, the default value for the type of the value parameter.
        /// This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// true if the <see cref="CacheDictionary{TKey, TValue}"/> contains an element with
        /// the specified key; otherwise, false.
        /// </returns>
        /// <see cref="ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </see>
        public bool TryGetValue(TKey key, out TValue value)
        {
            if (InnerList.TryGetValue(key, out TimedValue item) && FilterPredicate(item, this.CacheTimeout))
            {
                value = item.Content;
                return true;
            }
            else
            {
                value = default;
                return false;
            }
        }

        /// <summary>
        /// Determines whether the <see cref="CacheDictionary{TKey, TValue}"/> contains the specified key.
        /// </summary>
        /// <param name="key">The key to locate in the <see cref="CacheDictionary{TKey, TValue}"/>.</param>
        /// <returns>
        /// true if the <see cref="CacheDictionary{TKey, TValue}"/> contains an element with
        /// the specified key; otherwise, false.
        /// </returns>
        /// <see cref="ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </see>
        public bool ContainsKey(TKey key)
            => InnerList.TryGetValue(key, out TimedValue item) && FilterPredicate(item, this.CacheTimeout);

        /// <summary>
        /// Removes the value with the specified key from the <see cref="CacheDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <param name="key">The key of the element to remove.</param>
        /// <returns>
        /// true if the element is successfully found and removed; otherwise, false. This
        /// method returns false if key is not found in the <see cref="CacheDictionary{TKey, TValue}"/>.
        /// </returns>
        /// <see cref="ArgumentNullException">
        /// <paramref name="key"/> is null.
        /// </see>
        public bool Remove(TKey key)
            => InnerList.Remove(key);

        /// <summary>
        /// Returns an enumerator that iterates through the <see cref="CacheDictionary{TKey, TValue}"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="IEnumerator{T}"/> structure for the <see cref="CacheDictionary{TKey, TValue}"/>.
        /// </returns>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
            => FilteredList().GetEnumerator();

        /// <summary>
        /// Removes all keys and values from the <see cref="CacheDictionary{TKey, TValue}"/>.
        /// </summary>
        public void Clear()
            => InnerList.Clear();

        bool ICollection<KeyValuePair<TKey, TValue>>.IsReadOnly
            => ((IDictionary)InnerList).IsReadOnly;

        void ICollection<KeyValuePair<TKey, TValue>>.Add(KeyValuePair<TKey, TValue> item)
            => ((ICollection<KeyValuePair<TKey, TimedValue>>)InnerList)
                .Add(new KeyValuePair<TKey, TimedValue>(item.Key, new TimedValue() { Time = DateTime.UtcNow, Content = item.Value }));

        void ICollection<KeyValuePair<TKey, TValue>>.CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
            => throw new InvalidOperationException("CopyTo method is not available for CacheDictionary.");

        bool ICollection<KeyValuePair<TKey, TValue>>.Remove(KeyValuePair<TKey, TValue> item)
            => ((ICollection<KeyValuePair<TKey, TValue>>)InnerList).Remove(item);

        bool ICollection<KeyValuePair<TKey, TValue>>.Contains(KeyValuePair<TKey, TValue> item)
            => FilteredList().Contains(item);

        IEnumerator IEnumerable.GetEnumerator()
            => FilteredList().GetEnumerator();


        private IEnumerable<KeyValuePair<TKey, TValue>> FilteredList()
            => InnerList
                .Where(item => FilterPredicate(item.Value, this.CacheTimeout))
                .Select(item => new KeyValuePair<TKey, TValue>(item.Key, item.Value.Content));

    }
}
