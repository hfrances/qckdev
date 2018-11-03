using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace qckdev.Linq.Expressions
{

    internal static class Extensions
    {

        /// <summary>
        /// Añade y devuelve un nuevo valor de tipo <see cref="ExpressionNode"/> en la colección actual.
        /// </summary>
        /// <param name="collection">Colección de elementos donde se va a añadir.</param>
        /// <returns></returns>
        internal static ExpressionNode AddNew(this ExpressionNodeCollection collection)
        {
            var rdo = new ExpressionNode(collection.Owner.ExpressionTree);

            collection.Add(rdo);
            return rdo;
        }

        internal static void UpdateEndIndex(this ExpressionNode node)
        {
            var maxEndIndex = node.Nodes?.Max(x => x.EndIndex) ?? node.EndIndex;
            node.EndIndex = maxEndIndex;
        }

        internal static PropertyInfo GetProperty(this Dictionary<string, PropertyInfo> properties, string propertyName)
        {
            return properties[propertyName]; // TODO: Mejorar error.
        }

    }
}
