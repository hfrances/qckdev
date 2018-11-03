using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace qckdev.Linq.Expressions
{
    public sealed partial class ExpressionString
    {

        #region dictionaires

        /// <summary>
        /// Relación entre los operadores en formato texto y los operadores en formato objeto.
        /// </summary>
        static readonly Dictionary<string, ExpressionOperatorType> operationTranslator = new Dictionary<string, ExpressionOperatorType>()
        {
            {"AND", ExpressionOperatorType.And },
            {"&&", ExpressionOperatorType.And },
            {"OR", ExpressionOperatorType.Or },
            {"||", ExpressionOperatorType.Or },
            {"NOT", ExpressionOperatorType.Not },
            {"!", ExpressionOperatorType.Not },
            {"==", ExpressionOperatorType.Equals },
            {"<>", ExpressionOperatorType.NotEqual },
            {"!=", ExpressionOperatorType.NotEqual },
            {"=", ExpressionOperatorType.Like },
            {"LIKE", ExpressionOperatorType.Like },
            {"IN", ExpressionOperatorType.In },
            {">", ExpressionOperatorType.GreaterThan },
            {">=", ExpressionOperatorType.GreaterThanOrEqual },
            {"=>", ExpressionOperatorType.GreaterThanOrEqual },
            {"<", ExpressionOperatorType.LessThan },
            {"<=", ExpressionOperatorType.LessThanOrEqual },
            {"=<", ExpressionOperatorType.LessThanOrEqual },
            {"+", ExpressionOperatorType.Add },
            {"-", ExpressionOperatorType.Substract },
            {"*", ExpressionOperatorType.Multiply },
            {"/", ExpressionOperatorType.Divide },
            {"%", ExpressionOperatorType.Modulo },
            {"^", ExpressionOperatorType.Power },
        };

        static readonly List<char> breakers = new List<char>() { ' ', ',' };

        /// <summary>
        /// Relación entre el caracter de apertura y el de cierre.
        /// </summary>
        static readonly Dictionary<char, char> delimiterCloser = new Dictionary<char, char>()
        {
            {'(', ')' },
            {'\'', '\'' },
            {'#', '#' },
            {'[', ']' },
        };

        /// <summary>
        /// Relación entre el caracter de apertura y el tipo de dato.
        /// </summary>
        static readonly Dictionary<char, ExpressionNodeType> delimiterType = new Dictionary<char, ExpressionNodeType>()
        {
            {'\'', ExpressionNodeType.StringType },
            {'#', ExpressionNodeType.DateType },
            {'[', ExpressionNodeType.PropertyType },
        };

        #endregion


        #region ctor

        private ExpressionString()
        {
            StringBuffer = new StringBuilder();
            LastLogicNodes = new List<ExpressionNode>();
        }

        #endregion


        #region properties

        StringBuilder StringBuffer { get; }

        List<ExpressionNode> LastLogicNodes { get; }

        ExpressionNode LastRelationalNode { get; set; }

        ExpressionNode LastArithmeticNode { get; set; }

        ExpressionNode LastValueNode { get; set; }

        #endregion


        #region methods

        private ExpressionTree ParseExpressionString(string value)
        {
            var tree = ExpressionTree.Create(value);
            var root = tree.Root;
            int i;

            for (i = 0; i < value.Length; i++)
            {
                var c = value[i];

                if (delimiterCloser.ContainsKey(c))
                {
                    ExpressionNode child;

                    child = AddChildNodeFromOpenCharacter(ref root, i);
                    i = child.EndIndex.Value;
                }
                else if (breakers.Contains(c))
                {
                    ProcessBuffer(ref root, i - 1);
                }
                else
                {
                    StringBuffer.Append(c);
                }
            }
            ProcessBuffer(ref root, i - 1);
            UploadEndIndexAllLevels(root);
            CollapseTree(ref root);
            tree.Root = root;
            return tree;
        }

        /// <summary>
        /// Crea un sub elemento a partir de un caracter de apertura.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="currentIndex"></param>
        /// <returns></returns>
        private ExpressionNode AddChildNodeFromOpenCharacter(ref ExpressionNode parent, int currentIndex)
        {
            ExpressionNode rdo = null;
            string value = parent.ExpressionTree.Value;
            char c = value[currentIndex];
            Func<char, bool> closeCriteria;

            ProcessBuffer(ref parent, currentIndex - 1);
            var myparent = (LastArithmeticNode ?? LastRelationalNode ?? LastValueNode ?? parent);
            switch (c)
            {
                case '(':
                    closeCriteria = delegate (char x) { return x == delimiterCloser[c]; };
                    rdo = myparent.Nodes.AddNew();
                    rdo.StartIndex = currentIndex;
                    if (myparent.Operator == ExpressionOperatorType.In)
                    {
                        rdo.Type = ExpressionNodeType.ListType;
                        LastRelationalNode = rdo;
                    }
                    else
                    {
                        LastRelationalNode = null;
                    }
                    LastArithmeticNode = null;
                    LastValueNode = rdo;
                    SetEndIndex(ref rdo, currentIndex + 1, closeCriteria, recursive: true);
                    break;

                case '\'':
                case '#':
                case '[':
                    closeCriteria = delegate (char x) { return x == delimiterCloser[c]; };
                    rdo = myparent.Nodes.AddNew();
                    rdo.StartIndex = currentIndex;
                    rdo.Type = delimiterType[c];
                    LastValueNode = rdo;
                    SetEndIndex(ref rdo, currentIndex + 1, closeCriteria, recursive: false);
                    break;

                default:
                    throw new NotSupportedException();

            }
            if (rdo.EndIndex == null)
            {
                throw new FormatException(value);
            }
            return rdo;
        }

        /// <summary>
        /// Recorre el texto en busca hasta que se encuenta el caracter de cierre.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="startIndex"></param>
        /// <param name="closeCriteria"></param>
        /// <param name="recursive"></param>
        /// <returns></returns>
        private void SetEndIndex(ref ExpressionNode node, int startIndex, Func<char, bool> closeCriteria, bool recursive)
        {
            string value = node.ExpressionTree.Value;
            int flag = 0; // 0 None, 1 Posible fin, 2 Escape, 99 found.

            for (int i = startIndex; i < value.Length && flag != 99; i++)
            {
                var c = value[i];
                var last = (i == value.Length - 1);

                if (flag == 0 && closeCriteria(c))
                {
                    if (last)
                    {
                        node.EndIndex = i; // Es el último caracter y es el de cierre.
                        flag = 99; // Fin de la cadena.
                    }
                    else
                    {
                        flag = 1; // Posible fin, depende del siguiente caracter.
                    }
                }
                else if (flag == 1 && !closeCriteria(c))
                {
                    node.EndIndex = i - 1; // Es el último caracter y es el de cierre.
                    flag = 99; // Fin de la cadena.
                }
                else if (!last && flag == 0 && c == '\\')
                {
                    flag = 2; // Caracter de escape.
                }
                else
                {
                    flag = 0;
                    if (recursive && delimiterCloser.ContainsKey(c))
                    {
                        ExpressionNode child;

                        child = AddChildNodeFromOpenCharacter(ref node, i);
                        i = child.EndIndex.Value; // TODO: Control de errores. Si llega null, es que no se encontró caracter de cierre.
                    }
                    else if (recursive && breakers.Contains(c))
                    {
                        ProcessBuffer(ref node, i - 1);
                    }
                    else
                    {
                        StringBuffer.Append(c);
                    }
                }
            }
            if (flag == 99 && recursive)
            {
                var pendingNode = node;
                ProcessBuffer(ref pendingNode, node.EndIndex.Value - 1);
                pendingNode.ToString();
            }
            StringBuffer.Clear();
        }

        /// <summary>
        /// Procesa el texto pendiente en el buffer.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="currentIndex"></param>
        private void ProcessBuffer(ref ExpressionNode parent, int currentIndex)
        {

            if (StringBuffer.Length > 0)
            {
                var upperTextBuffer = StringBuffer.ToString().ToUpperInvariant();
                var myparent = (LastArithmeticNode ?? LastRelationalNode ?? LastValueNode ?? parent);  // Para variables, operaciones aritmeticas y relacionales.

                if (operationTranslator.TryGetValue(upperTextBuffer, out ExpressionOperatorType @operator))
                {
                    switch (@operator)
                    {
                        case ExpressionOperatorType.And:
                        case ExpressionOperatorType.Or:
                            ProcessBufferLogicOperator(ref parent, @operator, currentIndex);
                            break;

                        case ExpressionOperatorType.Not:
                            ProcessBufferLogicOperatorNot(myparent, @operator, currentIndex);
                            break;

                        case ExpressionOperatorType.Equals:
                        case ExpressionOperatorType.NotEqual:
                        case ExpressionOperatorType.GreaterThan:
                        case ExpressionOperatorType.GreaterThanOrEqual:
                        case ExpressionOperatorType.LessThan:
                        case ExpressionOperatorType.LessThanOrEqual:
                        case ExpressionOperatorType.Like:
                        case ExpressionOperatorType.In:
                            ProcessBufferEqualityOperator(myparent, @operator);
                            break;

                        case ExpressionOperatorType.Add:
                        case ExpressionOperatorType.Substract:
                        case ExpressionOperatorType.Multiply:
                        case ExpressionOperatorType.Divide:
                        case ExpressionOperatorType.Modulo:
                        case ExpressionOperatorType.Power:
                            ApplyParentNode(myparent, ExpressionNodeType.ArithmeticOperator, @operator);
                            LastArithmeticNode = myparent;
                            break;

                        default:
                            throw new NotImplementedException(@operator.ToString());
                    }
                }
                else
                {
                    ExpressionNode rdo;

                    rdo = myparent.Nodes.AddNew();
                    rdo.StartIndex = currentIndex - (StringBuffer.Length - 1);
                    rdo.Type = ExpressionNodeType.UnknownType;
                    LastValueNode = rdo;
                    rdo.EndIndex = currentIndex;
                }
                StringBuffer.Clear();
            }
        }

        private void ProcessBufferLogicOperator(ref ExpressionNode parent, ExpressionOperatorType @operator, int currentIndex)
        {
            var backIndex = (currentIndex - StringBuffer.Length - 1);
            var previousParent = parent;

            LastArithmeticNode?.UpdateEndIndex();
            LastRelationalNode = null;
            LastRelationalNode?.UpdateEndIndex();
            LastArithmeticNode = null;
            LastValueNode = null;

            parent = ApplyOrCreateLogicOperator(previousParent, @operator, backIndex);
            if (parent != previousParent)
            {
                LastLogicNodes.TryReplace(previousParent, parent);
            }
        }

        private void ProcessBufferLogicOperatorNot(ExpressionNode myparent, ExpressionOperatorType @operator, int currentIndex)
        {
            ExpressionNode rdo;

            LastArithmeticNode?.UpdateEndIndex();
            LastRelationalNode = null;
            LastRelationalNode?.UpdateEndIndex();
            LastArithmeticNode = null;
            LastValueNode = null;

            rdo = myparent.Nodes.AddNew();
            rdo.StartIndex = currentIndex;
            rdo.Type = ExpressionNodeType.LogicalOperator;
            rdo.Operator = @operator;
            LastValueNode = rdo;
        }

        private void ProcessBufferEqualityOperator(ExpressionNode myparent, ExpressionOperatorType @operator)
        {
            if (LastRelationalNode == null)
            {
                if (LastValueNode == null)
                {
                    throw new FormatException("Expression operator must have some property or constant.");
                }
                else
                {
                    ApplyParentNode(myparent, ExpressionNodeType.RelationalOperator, @operator); // Convierte el nodo en un nodo de tipo RelationalOperator y crea por debajo un nodo con la información del nodo original.
                    LastArithmeticNode = myparent;
                    LastRelationalNode = myparent;
                }
            }
            else
            {
                throw new FormatException("Previous RelationalNode was not closed."); // TODO: Mejor excepción.
            }
        }

        #endregion

        #region methods static 

        /// <summary>
        /// Aplica el operador al nodo especificado. En caso de que el nodo ya tenga otro operador, crea un nodo superior.
        /// </summary>
        /// <param name="node">Nodo al que aplicar el operador.</param>
        /// <param name="operator">Operador a aplicar.</param>
        /// <param name="endIndex">Indica el punto de inicio del operador. Este valor se aplicará como EndIndex a <paramref name="node"/> cuando se tenga que crear un nodo padre.</param>
        /// <returns>Devuelve el nodo con el operador aplicado. Puede ser el mismo o un nodo creado y colocado en el nivel superior.</returns>
        private static ExpressionNode ApplyOrCreateLogicOperator(ExpressionNode node, ExpressionOperatorType @operator, int endIndex)
        {
            var rdo = node;

            if (node.Operator == ExpressionOperatorType.None)
            {
                // Reaprovechar el nodo.
                node.Type = ExpressionNodeType.LogicalOperator;
                node.Operator = @operator;
            }
            else if (node.Operator == @operator)
            {
                // Ya se está reaprovechando el nodo.
            }
            else
            {
                // Crear un nodo por encima.
                rdo = new ExpressionNode(node.ExpressionTree)
                {
                    Type = ExpressionNodeType.LogicalOperator,
                    Operator = @operator,
                    StartIndex = node.StartIndex, // Dado que aglutina a este nodo, se le pone su posición de inicio.
                    EndIndex = node.EndIndex
                };
                rdo.Nodes.Add(node);
                node.EndIndex = endIndex; // Al nodo original, se le pone la posición actual como punto final.
            }
            return rdo;
        }

        /// <summary>
        /// Modifica la expresión del parámetro <paramref name="node"/> añadiendo primero los datos actuales a un nodo inferior.
        /// </summary>
        /// <param name="node">Nodo a modificar.</param>
        /// <param name="parentType">Nuevo tipo</param>
        /// <param name="parentOperator">Nuevo operador.</param>
        private static void ApplyParentNode(ExpressionNode node, ExpressionNodeType parentType, ExpressionOperatorType parentOperator)
        {
            var rdo = new ExpressionNode(node.ExpressionTree)
            {

                // Mover la expresión debajo del operador.
                Type = node.Type,
                Operator = node.Operator,
                StartIndex = node.StartIndex,
                EndIndex = node.EndIndex
            };
            rdo.Nodes.AddRange(node.Nodes);

            node.Nodes.Clear();
            node.Nodes.Add(rdo);
            node.Type = parentType;
            node.Operator = parentOperator;
            node.EndIndex = null;
        }

        /// <summary>
        /// Elimina todos los nodos que están vacíos (<see cref="ExpressionNodeType.Default"/>) para dejar el arbol lo más limpio posible.
        /// </summary>
        /// <param name="node">Nodo desde el que limpiar.</param>
        private static void CollapseTree(ref ExpressionNode node)
        {

            foreach (var child in node.Nodes.ToArray())
            {
                var newChild = child;

                CollapseTree(ref newChild);
                if (newChild == null)
                {
                    node.Nodes.Remove(child);
                }
                else if (newChild != child)
                {
                    node.Nodes.TryReplace(child, newChild);
                }
            }
            if (node.Type == ExpressionNodeType.Default && node.Nodes.Count < 2)
            {
                node = node.Nodes.FirstOrDefault();
            }
        }

        /// <summary>
        /// Calcula el <see cref="ExpressionNode.EndIndex"/> del elemento y de todos sus elementos hijos que todavía tengan este valor vacío.
        /// </summary>
        /// <param name="node">Nodo a revisar.</param>
        /// <remarks>
        /// Se utiliza para todos aquellos tipos de elementos que no se sabe el valor de <see cref="ExpressionNode.EndIndex"/> 
        /// hasta tenerse todos los nodos hijos.
        /// </remarks>
        private static void UploadEndIndexAllLevels(ExpressionNode node)
        {

            foreach (var child in node.Nodes)
            {
                UploadEndIndexAllLevels(child);
            }
            if (node.EndIndex == null)
            {
                node.UpdateEndIndex();
            }
        }

        #endregion

    }
}