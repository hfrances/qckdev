using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;

namespace qckdev
{

    /// <summary>
    /// Provides a multi-field class which implements <see cref="IEnumerable{T}"/> and <see cref="IEqualityComparer{T}"/> interfaces. 
    /// This class can be uses in <see cref="IDictionary{TKey, TValue}"/> for TKey identification.
    /// </summary>
    public class Key : IEquatable<Key>, IEqualityComparer<Key>
    {

        #region fields

        readonly object[] InnerList;
        int? hashcode;

        #endregion


        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="Key"/> class. <see cref="IgnoreCase"/> is activated for <see cref="String"/> values.
        /// </summary>
        /// <param name="keys">The <see cref="Array"/> whose elements are copied to the <see cref="Key"/>.</param>
        public Key(params object[] keys) : this(keys, ignoreCase: true) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="Key"/> class.
        /// </summary>
        /// <param name="keys">The <see cref="Array"/> whose elements are copied to the <see cref="Key"/>.</param>
        /// <param name="ignoreCase">
        /// A <see cref="Boolean"/> indicating a case-sensitive or insensitive comparison in <see cref="String"/> values (true indicates a case-insensitive comparison).
        /// </param>
        public Key(object[] keys, bool ignoreCase)
        {
            IgnoreCase = ignoreCase;
            InnerList = keys;
        }

        #endregion


        #region properties

        /// <summary>
        /// A <see cref="Boolean"/> indicating a case-sensitive or insensitive comparison in <see cref="String"/> values (true indicates a case-insensitive comparison).
        /// </summary>
        public bool IgnoreCase { get; }

        /// <summary>
        /// Gets or sets the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to get or set.</param>
        /// <returns>The element at the specified index.</returns>
        /// <exception cref="ArgumentOutOfRangeException">index is less than zero.-or- index is equal to or greater than System.Collections.ArrayList.Count.</exception>
        protected object this[int index]
        {
            get => InnerList[index];
            set
            {
                InnerList[index] = value;
                hashcode = null;
            }
        }

        #endregion


        #region methods

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>A hash code for the current <see cref="Key"/>.</returns>
        [SuppressMessage("Sonar Bug", "S2328:\"GetHashCode\" should not reference mutable fields", Justification = "This object contains a dynamic list. Values can change. Hashcode is initializated each time that InnerList changes.")]
        public override int GetHashCode()
        {
            if (hashcode == null)
                hashcode = CalculateHashCode(InnerList, IgnoreCase, includeType: false);
            return hashcode.Value;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Key"/> is equal to the current <see cref="Key"/>
        /// </summary>
        /// <param name="other">The <see cref="Key"/> to compare with the current <see cref="Key"/>.</param>
        /// <returns>true if the specified <see cref="Key"/> is equal to the current <see cref="Key"/>; otherwise, false.</returns>
        public bool Equals(Key other)
        {
            bool rdo = true;

            if ((object)other == null)
            {
                rdo = false;
            }
            else if (this.InnerList.Length == other.InnerList.Length)
            {
                int length = this.InnerList.Length;
                for (int i = 0; i < length && rdo; i++)
                {
                    var val1 = this.InnerList[i];
                    var val2 = other.InnerList[i];

                    rdo = this.ItemEquals(val1, val2);
                }
            }
            else
            {
                rdo = false;
            }
            return rdo;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="Key"/>
        /// </summary>
        /// <param name="obj">The <see cref="Object"/> to compare with the current <see cref="Key"/>.</param>
        /// <returns>true if the specified <see cref="Object"/> is equal to the current <see cref="Key"/>; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            return this.Equals((Key)obj);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> instances are considered equal.
        /// </summary>
        /// <param name="item1">The first <see cref="Object"/> to compare.</param>
        /// <param name="item2">The second <see cref="Object"/> to compare.</param>
        /// <returns>true if the instances are equal; otherwise false.</returns>
        /// <remarks>
        /// If <paramref name="item1"/> and <paramref name="item2"/> are <see cref="String"/>, 
        /// it uses <see cref="string.Compare(string, string, StringComparison)"/> with <see cref="Key.IgnoreCase"/> parameter.
        /// </remarks>
        protected virtual bool ItemEquals(object item1, object item2)
        {
            bool rdo;

            if (item1 is string && item2 is string)
                rdo = (string.Compare((string)item1, (string)item2, (IgnoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal)) == 0);
            else
                rdo = object.Equals(item1, item2);
            return rdo;
        }

        /// <summary>
        /// Returns an <see cref="Array"/> containing copies of the elements of the current object.
        /// </summary>
        /// <returns>A <see cref="Array"/> containing copies of the elements of the current object.</returns>
        public object[] ToArray()
        {
            return InnerList.ToArray();
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>A string that represents the current object.</returns>
        public override string ToString()
        {
            var rdo = new System.Text.StringBuilder();

            foreach (var item in InnerList)
            {
                if (rdo.Length > 0) rdo.Append("\t");
                rdo.Append(item);
            }
            return rdo.ToString();
        }

        bool IEqualityComparer<Key>.Equals(Key x, Key y)
        {
            bool rdo;

            if ((object)x == null && (object)y == null)
                rdo = true;
            else if ((object)x == null)
                rdo = false;
            else
                rdo = x.Equals(y);
            return rdo;
        }

        int IEqualityComparer<Key>.GetHashCode(Key obj)
        {
            return obj?.GetHashCode() ?? 0;
        }

        #endregion


        #region static

        private static int CalculateHashCode(IList list, bool ignoreCase, bool includeType)
        {
            int rdo = 0;

            foreach (var item in list)
            {
                int myhashcode;

                if (item == null)
                {
                    myhashcode = 0;
                }
                else
                {
                    if (ignoreCase && item is string)
                        myhashcode = ((string)item).ToUpper().GetHashCode();
                    else
                        myhashcode = item.GetHashCode();

                    if (includeType)
                        myhashcode = ((int)(uint)(item.GetType().GetHashCode() % 0x8000) | (int)(uint)(myhashcode & 0xFFFF0000));
                    else
                        myhashcode = (int)(uint)myhashcode;
                }
                rdo ^= myhashcode;
            }
            return rdo;
        }

        #endregion


        #region operators

        /// <summary>
        /// Determines whether the specified <see cref="Key"/> instances are considered equal.
        /// </summary>
        /// <param name="val1">The first <see cref="Key"/> to compare.</param>
        /// <param name="val2">The second <see cref="Key"/> to compare.</param>
        /// <returns>true if the instances are equal; otherwise false.</returns>
        public static bool operator ==(Key val1, Key val2)
        {
            bool rdo;

            if ((object)val1 == null)
                rdo = ((object)val2 == null);
            else
                rdo = val1.Equals(val2);
            return rdo;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Key"/> instances are considered different.
        /// </summary>
        /// <param name="val1">The first <see cref="Key"/> to compare.</param>
        /// <param name="val2">The second <see cref="Key"/> to compare.</param>
        /// <returns>true if the instances are different; otherwise false.</returns>
        public static bool operator !=(Key val1, Key val2)
        {
            return !(val1 == val2);
        }

        #endregion

    }

}