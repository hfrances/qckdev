using System;
using System.Xml.Serialization;
using qckdev.Reflection;

namespace qckdev.Linq.Expressions
{

    /// <summary>
    /// Provides an intermediate language between a <see cref="string"/> expression and an <see cref="System.Linq.Expressions.Expression"/>.
    /// </summary>
    [Serializable]
    public sealed class ExpressionTree : IEquatable<ExpressionTree>
    {

        #region fields

        ExpressionNode _root;

        #endregion


        #region ctor

        private ExpressionTree() { } // Uso exclusivo Serializate.

        private ExpressionTree(string value)
        {
            this.Value = value;
            this.Root = new ExpressionNode(this) { StartIndex = 0 };
        }

        #endregion


        #region properties

        /// <summary>
        /// Gets or sets the <see cref="string"/> expression.
        /// </summary>
        [XmlElement]
        public string Value { get; set; }

        /// <summary>
        /// Gets or sets the root <see cref="ExpressionNode"/>.
        /// </summary>
        [XmlElement(ElementName = "ExpressionNode")]
        public ExpressionNode Root
        {
            get { return _root; }
            set
            {
                _root = value;
                ApplyExpressionTree(value);
            }
        }

        #endregion


        #region methods

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return string.Format("ExprTree: {0}", this.Value);
        }

        private void ApplyExpressionTree(ExpressionNode node)
        {
            if (node != null)
            {
                node.ExpressionTree = this;
                foreach (var child in node.Nodes)
                {
                    ApplyExpressionTree(child);
                }
            }
        }

        internal static ExpressionTree Create(string value)
        {
            return new ExpressionTree(value);
        }

        #endregion


        #region IEquatable implementation

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="ExpressionTree"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return ReflectionHelper.GetHashCode(this.Value, this.Root);
        }

        /// <summary>
        /// Determines whether the specified <see cref="ExpressionTree"/> is equal to the current <see cref="ExpressionTree"/>
        /// </summary>
        /// <param name="other">The <see cref="ExpressionTree"/> to compare with the current <see cref="ExpressionTree"/>.</param>
        /// <returns>
        /// true if the specified <see cref="ExpressionTree"/> is equal to the current <see cref="ExpressionTree"/>; otherwise, false.
        /// </returns>
        public bool Equals(ExpressionTree other)
        {
            bool rdo = true;

            if (other == null)
            {
                rdo = false;
            }
            else
            {
                rdo = (this.Value == other.Value && object.Equals(this.Root, other.Root));
            }
            return rdo;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="ExpressionTree"/>
        /// </summary>
        /// <param name="obj">The <see cref="Object"/> to compare with the current <see cref="ExpressionTree"/>.</param>
        /// <returns>
        /// true if the specified <see cref="Object"/> is equal to the current <see cref="ExpressionTree"/>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as ExpressionTree);
        }

        #endregion

    }
}
