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

    }

}