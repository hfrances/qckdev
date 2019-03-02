using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace qckdev.Linq.Expressions
{

    internal static class Extensions
    {

        internal static void UpdateEndIndex(this ExpressionNode node)
        {
            var maxEndIndex = node.Nodes?.Max(x => x.EndIndex) ?? node.EndIndex;
            node.EndIndex = maxEndIndex;
        }

        internal static PropertyInfo GetProperty(this Dictionary<string, PropertyInfo> properties, string propertyName)
        {
            return properties[propertyName]; // TODO: Mejorar error.
        }

        internal static void InsertTree(this ExpressionNode node, ExpressionTree tree)
        {
            node.Nodes.Add(tree.Root); // TODO: Si solo uno, reemplazar.<
            InsertTreeRenumerate(node, node.StartIndex.Value);
        }

        private static void InsertTreeRenumerate(ExpressionNode parentNode, int charIndexIncrement)
        {

            foreach (var childNode in parentNode.Nodes.Where(x => x.ExpressionTree != parentNode.ExpressionTree))
            {
                childNode.ExpressionTree = parentNode.ExpressionTree;
                InsertTreeRenumerate(childNode, charIndexIncrement);
            }
        }

    }
}
