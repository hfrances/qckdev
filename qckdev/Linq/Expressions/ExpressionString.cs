using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace qckdev.Linq.Expressions
{

    /// <summary>
    /// Provide methods to convert a <see cref="String"/> expression to <see cref="ExpressionTree"/>.
    /// </summary>
    public sealed partial class ExpressionString
    {

        /// <summary>
        /// Converts a <see cref="String"/> expression to <see cref="ExpressionTree"/>.
        /// </summary>
        /// <param name="value">The <see cref="String"/> expression to parse.</param>
        /// <returns>The <see cref="ExpressionTree"/> parsed.</returns>
        public static ExpressionTree BuildTree(string value)
        {
            ExpressionTree tree;

            var fsp = new ExpressionString();
            tree = fsp.ParseExpressionString(value);

            return tree;
        }

        /*
        /// <summary>
        /// Serializes the specified <see cref="ExpressionTree"/> and writes the XML document to a file.
        /// </summary>
        /// <param name="item">The <see cref="ExpressionTree"/> to serialize.</param>
        /// <param name="fileName">The complete file path to write.</param>
        public static void Serialize(ExpressionTree item, string fileName)
        {
            var serializer = new XmlSerializer(typeof(ExpressionTree));

            using (var writer = new System.IO.StreamWriter(fileName))
            {
                serializer.Serialize(writer, item);
            }
        }

        /// <summary>
        /// Deserializes the XML document contained by the specified file.
        /// </summary>
        /// <param name="fileName">The complete file path to read.</param>
        /// <returns>The <see cref="ExpressionTree"/> being deserialized.</returns>
        /// <exception cref="InvalidOperationException">
        /// An error occurred during deserialization. The original exception is available using the <see cref="Exception.InnerException"/> property.
        /// </exception>
        public static ExpressionTree Deserialize(string fileName)
        {
            ExpressionTree rdo = null;
            var serializer = new XmlSerializer(typeof(ExpressionTree));

            using (var reader = new System.IO.StreamReader(fileName))
            {
                rdo = (ExpressionTree)serializer.Deserialize(reader);
            }
            return rdo;
        }

        /// <summary>
        /// Deserializes the XML document contained by the specified <see cref="String"/>.
        /// </summary>
        /// <param name="content">The <see cref="String"/> to be deserilized.</param>
        /// <returns>The <see cref="ExpressionTree"/> being deserialized.</returns>
        /// <exception cref="InvalidOperationException">
        /// An error occurred during deserialization. The original exception is available using the <see cref="Exception.InnerException"/> property.
        /// </exception>
        public static ExpressionTree DeserializeString(string content)
        {
            ExpressionTree rdo = null;
            var serializer = new XmlSerializer(typeof(ExpressionTree));

            using (var reader = new System.IO.StringReader(content))
            {
                rdo = (ExpressionTree)serializer.Deserialize(reader);
            }
            return rdo;
        }
        */
    }

}