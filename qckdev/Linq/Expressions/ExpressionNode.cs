using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Xml.Serialization;
using qckdev.Reflection;

namespace qckdev.Linq.Expressions
{

    /// <summary>
    /// Represents a part of a <see cref="String"/> expression in a <see cref="ExpressionTree"/>.
    /// </summary>
    [Serializable]
    public sealed class ExpressionNode : IEquatable<ExpressionNode>
    {

        #region ctor

        private ExpressionNode() // Uso exclusivo Serializate.
        {
            this.Nodes = new ExpressionNodeCollection(this);
        }

        internal ExpressionNode(ExpressionTree tree) : this()
        {
            this.ExpressionTree = tree;
        }

        #endregion


        #region properties

        internal ExpressionTree ExpressionTree { get; set; }

        /// <summary>
        /// Gets the start index in the <see cref="ExpressionTree.Value"/>.
        /// </summary>
        public int? StartIndex { get; set; }

        /// <summary>
        /// Gets the end index in the <see cref="ExpressionTree.Value"/>.
        /// </summary>
        public int? EndIndex { get; set; }

        /// <summary>
        /// Gets the <see cref="ExpressionOperatorType"/> that determinates what kind of operation should be applied to this <see cref="ExpressionNode"/>.
        /// This value can be <see cref="ExpressionOperatorType.None"/> when the <see cref="ExpressionNode"/> represents parentheses.
        /// </summary>
        [XmlElement, DefaultValue(typeof(ExpressionOperatorType), "None")]
        public ExpressionOperatorType Operator { get; set; }

        /// <summary>
        /// Gets the <see cref="ExpressionNodeType"/> that determinates the Expression type.
        /// </summary>
        public ExpressionNodeType Type { get; set; }

        /// <summary>
        /// Gets the collection of <see cref="ExpressionNode"/> objects assigned to the current node.
        /// </summary>
        public ExpressionNodeCollection Nodes { get; }

        /// <summary>
        /// Gets the <see cref="String"/> part of the <see cref="ExpressionTree"/>.
        /// </summary>
        public string Text
        {
            get
            {
                var value = ExpressionTree.Value;
                return value.Substring(this.StartIndex.Value, 1 + this.EndIndex.Value - this.StartIndex.Value).Trim();
            }
        }

        /// <summary>
        /// Gets the final value of the <see cref="ExpressionNode"/> converted during the process or null if it must takes <see cref="Text"/> value.
        /// </summary>
        public string NewText
        {
            get; set;
        }

        #endregion


        #region metods 

        /// <summary>
        /// Retuns if the element has child nodes. This class is for Serialization.
        /// </summary>
        public bool ShouldSerializeNodes()
            => (Nodes.Count > 0);

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            var @operator = (this.Operator == ExpressionOperatorType.None ? (ExpressionOperatorType?)null : this.Operator);
            return string.Format("Expr {0} {1}:    {2}", this.Type, @operator, this.NewText ?? this.Text);
        }

        #endregion


        #region IEquatable implementation

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="ExpressionNode"/>.
        /// </returns>
        public override int GetHashCode()
        {
            var hashItems = new List<object>() { this.ExpressionTree.Value, this.StartIndex, this.EndIndex, this.Type, this.Operator };

            hashItems.AddRange(this.Nodes.OfType<object>());
            return ReflectionHelper.GetHashCode(hashItems);
        }

        /// <summary>
        /// Determines whether the specified <see cref="ExpressionNode"/> is equal to the current <see cref="ExpressionNode"/>
        /// </summary>
        /// <param name="other">The <see cref="ExpressionNode"/> to compare with the current <see cref="ExpressionNode"/>.</param>
        /// <returns>
        /// true if the specified <see cref="ExpressionNode"/> is equal to the current <see cref="ExpressionNode"/>; otherwise, false.
        /// </returns>
        public bool Equals(ExpressionNode other)
        {
            bool rdo = true;

            if (other == null)
            {
                rdo = false;
            }
            else
            {
                rdo = (this.ExpressionTree.Value == other.ExpressionTree.Value &&
                       this.StartIndex == other.StartIndex &&
                       this.EndIndex == other.EndIndex &&
                       this.Type == other.Type &&
                       this.Operator == other.Operator &&
                       this.Nodes.Count == other.Nodes.Count);

                for (int i = 0; i < this.Nodes.Count && rdo; i++)
                {
                    rdo = (this.Nodes[i].Equals(other.Nodes[i]));
                }
            }
            return rdo;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to the current <see cref="ExpressionNode"/>
        /// </summary>
        /// <param name="obj">The <see cref="Object"/> to compare with the current <see cref="ExpressionNode"/>.</param>
        /// <returns>
        /// true if the specified <see cref="Object"/> is equal to the current <see cref="ExpressionNode"/>; otherwise, false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as ExpressionNode);
        }

        #endregion

    }

    /// <summary>
    /// Represents a collection of <see cref="ExpressionTree"/> objects.
    /// </summary>
    public sealed class ExpressionNodeCollection : Collection<ExpressionNode>
    {

        internal ExpressionNodeCollection(ExpressionNode owner)
        {
            this.Owner = owner;
        }

        /// <summary>
        /// The parent <see cref="ExpressionNode"/>.
        /// </summary>
        public ExpressionNode Owner { get; }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="ExpressionNodeCollection"/>.
        /// </summary>
        /// <param name="collection">The collection whose elements should be added to the end of the <see cref="ExpressionNodeCollection"/>.</param>
        /// <exception cref="NullReferenceException">Collection is null.</exception>
        public void AddRange(IEnumerable<ExpressionNode> collection)
        {
            foreach (var item in collection)
            {
                this.Add(item);
            }
        }

        /// <summary>
        /// Inserts an element into the <see cref="ExpressionNodeCollection"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which item should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        protected override void InsertItem(int index, ExpressionNode item)
        {
            item.ExpressionTree = Owner.ExpressionTree;
            base.InsertItem(index, item);
        }

    }

}
