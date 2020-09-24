using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics;

namespace qckdev.Linq.Expressions
{

    /// <summary>
    /// Provide methods to convert a <see cref="String"/> expression to <see cref="ExpressionTree"/>.
    /// </summary>
    public sealed partial class ExpressionStringBuilder
    {

        #region dictionaires

        /// <summary>
        /// Relación entre los operadores en formato texto y los operadores en formato objeto.
        /// </summary>
        static readonly Dictionary<string, ExpressionOperatorType> OperationMap
            = new Dictionary<string, ExpressionOperatorType>(StringComparer.CurrentCultureIgnoreCase)
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

        /// <summary>
        /// Lista de caracteres que indican que se cambia de elemento durante la lectura.
        /// </summary>
        static readonly List<char> BreakerChars = new List<char>() { ' ', ',' };

        /// <summary>
        /// Lista de caracteres que tienen funcionamiento especial pero, que si vienen dos veces seguidas, se refiere al carácter en sí.
        /// </summary>
        static readonly List<char> ScapeChars = new List<char>() { '\'', '"' };

        /// <summary>
        /// Relación entre el caracter de apertura y el de cierre.
        /// </summary>
        static readonly Dictionary<char, char> DelimiterChars = new Dictionary<char, char>()
        {
            {'(', ')' },
            {'\'', '\'' },
            {'#', '#' },
            {'[', ']' },
        };

        /// <summary>
        /// Relación entre el caracter de apertura y el tipo de dato.
        /// </summary>
        static readonly Dictionary<char, ExpressionNodeType> DelimiterTypes = new Dictionary<char, ExpressionNodeType>()
        {
            {'\'', ExpressionNodeType.StringType },
            {'#', ExpressionNodeType.DateType },
            {'[', ExpressionNodeType.PropertyType },
        };

        #endregion


        #region ctor

        private int _lastNodeId = 0;

        private ExpressionStringBuilder()
        { }

        #endregion


        #region properties
        #endregion


        #region methods

        /// <summary>
        /// Recorre todos los caracteres de la cadena y los procesa.
        /// </summary>
        /// <param name="value">Cadena de texto.</param>
        /// <returns>El <see cref="ExpressionTree"/> con los elementos de la cadena.</returns>
        private ExpressionTree ParseExpressionString(string value)
        {
            var buffer = new StringBuilder();
            var tree = ExpressionTree.Create(value);
            var root = tree.Root;

            // Procesar cadena.
            ProcessSubstring(buffer, root, 0, null, delimiterOpened: false);

            // Finalizar.
            UploadEndIndexAllLevels(root);
            CollapseTree(ref root);
            tree.Root = root;
            return tree;
        }

        /// <summary>
        /// Procesa el rango específico de la cadena contenida en <see cref="ExpressionTree.Value"/> 
        /// (accesible desde <seealso cref="ExpressionNode.ExpressionTree"/>).
        /// </summary>
        /// <param name="buffer">Almacenamiento temporal de la parte de la cadena que todavía no se ha interpretado como expressión.</param>
        /// <param name="node">Nodo de trabajo.</param>
        /// <param name="startIndex">Índice del primer caracter a leer en <see cref="ExpressionTree.Value"/></param>
        /// <param name="endIndex">Índice del último caracter a leer en <see cref="ExpressionTree.Value"/> o null para leer hasta el final.</param>
        /// <param name="delimiterOpened">
        /// Indica si se está buscando dentro de una zona incluida en <see cref="DelimiterChars"/>, 
        /// para no lanzar error de formato si encuentra un carácter de cierre.
        /// </param>
        private void ProcessSubstring(StringBuilder buffer, ExpressionNode node, int startIndex, int? endIndex, bool delimiterOpened)
        {
            var value = node.ExpressionTree.Value;
            var lastCharType = CharType.None;
            ExpressionNode nearestNode = null;
            int i;

            for (i = startIndex; i <= (endIndex ?? value.Length - 1); i++)
            {
                ExpressionNode tmp;

                tmp = ProcessChar(buffer, nearestNode ?? node, ref lastCharType, ref i, delimiterOpened);
                nearestNode = tmp ?? nearestNode;
            }
            ProcessBuffer(buffer, nearestNode ?? node, i - 1, formattedText: false);
        }

        /// <summary>
        /// Analiza el caracter en una cadena y lo procesa, pudiendo crear nodos hijos, asignarles un operador o rellenar en <paramref name="buffer"/>.
        /// </summary>
        /// <param name="buffer">
        /// Almacenamiento temporal de la parte de la cadena que todavía no se ha interpretado como expressión.
        /// </param>
        /// <param name="node">
        /// Nodo que se está procesando en estos momentos.
        /// </param>
        /// <param name="lastCharType">
        /// Último tipo de caracter leído. 
        /// Este permite identificar cuándo se ha cambiado de valor a operación u otros, sin tener que separar mediante espacios.
        /// </param>
        /// <param name="charIndex">
        /// Índice del caracter que se está analizando dentro de <paramref name="node"/>
        /// (<seealso cref="ExpressionNode.ExpressionTree"/>, <seealso cref="ExpressionTree.Value"/>).
        /// Si es conveniente, este valor puede ser reemplazado por otro (por ejemplo, al encontrar una apertura de paréntesis, 
        /// se moverá hasta el paréntesis de cierre.
        /// </param>
        /// <param name="delimiterOpened">
        /// Indica si se está buscando dentro de una zona incluida en <see cref="DelimiterChars"/>, 
        /// para no lanzar error de formato si encuentra un carácter de cierre.
        /// </param>
        /// <exception cref="FormatException">La cadena a procesar es incorrecta.</exception>
        private ExpressionNode ProcessChar(StringBuilder buffer, ExpressionNode node, ref CharType lastCharType, ref int charIndex, bool delimiterOpened)
        {
            var value = node.ExpressionTree.Value;
            var c = value[charIndex];
            var negativeSymbol = false;
            ExpressionNode nearestNode = null;

            if (DelimiterChars.ContainsKey(c))
            {
                ExpressionNode tmp;

                // Se ha detectado un carácter de apertura (un paréntesis, una comilla, un corchete...). 
                // Procesa su contenido hasta encontrar el carácter de cierre.
                // Mueve el contador hasta el último carácter leído.
                tmp = ProcessBuffer(buffer, node, charIndex - 1, formattedText: false);
                tmp = AddChildNodeFromOpenCharacter(buffer, tmp ?? node, ref charIndex);
                nearestNode = tmp ?? nearestNode;
            }
            else if (!delimiterOpened && DelimiterChars.ContainsValue(c))
            {
                throw new FormatException($"Invalid format for the following sentence:\n{value}\nClose character {c} was found without its opening one.");
            }
            else if (BreakerChars.Contains(c))
            {
                ExpressionNode tmp;

                // Se ha detectado un caracter de rotura (una coma, un espacio...). 
                // Procesar el contenido.
                // Mueve el contador hasta el último carácter leído.
                tmp = ProcessBuffer(buffer, node, charIndex - 1, formattedText: false);
                nearestNode = tmp ?? nearestNode;
            }
            else
            {
                if (c == '-' && (
                        // El nodo actual un nodo-valor y el elemento que hay en el buffer es un operador.
                        // Procesar el buffer para crear el nodo-operador y empezar a rellenar el nodo-valor.
                        (lastCharType == CharType.Operator && IsValueNode(node.Type)) ||
                        // El nodo actual es un nodo-operador, por lo que ya estamos en la fase de rellenar el nodo-valor.
                        (buffer.Length == 0 && IsOperatorNode(node.Type)) ||
                        // No hay ninguna operación aritmética en proceso. Es un nodo-valor.
                        (buffer.Length == 0 && !IsArithmeticBranch(node))
                    ))
                {
                    // Es símbolo negativo.
                    nearestNode =
                        ProcessBuffer(buffer, node, charIndex - 1, formattedText: false)
                            ?? nearestNode;
                    negativeSymbol = true;
                }
                else if (buffer.Length > 0 && lastCharType != GetCharType(c.ToString()))
                {
                    nearestNode =
                        ProcessBuffer(buffer, node, charIndex - 1, formattedText: false)
                            ?? nearestNode;
                }

                buffer.Append(c);
            }
            lastCharType = (negativeSymbol ?
                CharType.Other :
                    GetCharType(buffer.ToString()));
            return nearestNode;
        }

        /// <summary>
        /// Crea un sub elemento a partir de un caracter de apertura.
        /// </summary>
        /// <param name="buffer">Almacenamiento temporal de la parte de la cadena que todavía no se ha interpretado como expressión.</param>
        /// <param name="node">Nodo actual de procesamiento.</param>
        /// <param name="charIndex">
        /// Índice del caracter que se está analizando dentro de <paramref name="node"/>
        /// (<seealso cref="ExpressionNode.ExpressionTree"/>, <seealso cref="ExpressionTree.Value"/>).
        /// Si es conveniente, este valor puede ser reemplazado por otro (por ejemplo, al encontrar una apertura de paréntesis, 
        /// se moverá hasta el paréntesis de cierre.
        /// </param>
        /// <exception cref="FormatException">La cadena a procesar es incorrecta.</exception>
        [SuppressMessage("Minor Code Smell", "S3241:Methods should not return values that are never used", Justification = "Return value could be necessary in the future.")]
        private ExpressionNode AddChildNodeFromOpenCharacter(StringBuilder buffer, ExpressionNode node, ref int charIndex)
        {
            var value = node.ExpressionTree.Value;
            var c = value[charIndex];
            Func<char, bool> closeCriteria;
            ExpressionNodeType nodeType;
            ExpressionNode newNode = null;
            bool isLocked;
            bool recursive;

            switch (c)
            {
                case '(':
                    isLocked = true; // Bloquear el nodo ya que es una expresión entre paréntesis (lo que se encuentre dentro deberá ir dentro y lo que se encuentre fuera deberá ir fuera).
                    recursive = true;
                    if (node.Operator == ExpressionOperatorType.In)
                        nodeType = ExpressionNodeType.ListType;
                    else
                        nodeType = ExpressionNodeType.Default;
                    break;

                default: // ', #, [
                    nodeType = DelimiterTypes[c];
                    isLocked = false;
                    recursive = false;
                    break;

            }
            closeCriteria = (x) => x == DelimiterChars[c];
            newNode = CreateNode(node, charIndex, nodeType: nodeType, isLocked: isLocked);

            node = CloseNodeFromOpenCharacter(buffer, newNode, ref charIndex, closeCriteria, recursive);
            if (newNode.EndIndex == null)
            {
                // Si llega null, es que no se encontró caracter de cierre, por lo que el formato de texto probablemente sea erróneo.
                throw new FormatException($"Invalid format for the following sentence:\n{value}\nClose character {closeCriteria} not found.");
            }
            else if (newNode.Locked)
            {
                node = newNode;
            }
            return node;
        }

        /// <summary>
        /// Recorre el texto en busca hasta que se encuenta el caracter de cierre.
        /// </summary>
        /// <param name="buffer">Almacenamiento temporal de la parte de la cadena que todavía no se ha interpretado como expressión.</param>
        /// <param name="node">Nodo actual de procesamiento.</param>
        /// <param name="charIndex"></param>
        /// <param name="closeCriteria"></param>
        /// <param name="recursive"></param>
        [SuppressMessage("Critical Code Smell", "S3776:Cognitive Complexity of methods should not be too high", Justification = "Cognitive complexity could be higher if this method is split.")]
        private ExpressionNode CloseNodeFromOpenCharacter(StringBuilder buffer, ExpressionNode node, ref int charIndex, Func<char, bool> closeCriteria, bool recursive)
        {
            var value = node.ExpressionTree.Value;
            var lastCharType = CharType.None;
            var flag = 0; // [0] Continuar leyendo; [1] Posible fin; [2] Caracter de escape; [99] Fin de la cadena.
            var useFormattedText = false;
            int i;
            ExpressionNode nearestNode = null;

            charIndex++;
            for (i = charIndex; i < value.Length && flag != 99; i++)
            {
                var c = value[i];
                var last = (i == value.Length - 1);

                if (flag == 0 && closeCriteria(c))
                {
                    var possibleScape = ScapeChars.Contains(c);

                    if (last || !possibleScape)
                    {
                        node.EndIndex = i; // Es el último caracter y es el de cierre.
                        flag = 99; // Fin de la cadena.
                    }
                    else
                    {
                        flag = 1; // Posible fin, depende del siguiente caracter.
                        useFormattedText = true;
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
                    useFormattedText = true;
                }
                else
                {
                    flag = 0;
                    if (recursive)
                    {
                        ExpressionNode tmp;

                        tmp = ProcessChar(buffer, nearestNode ?? node, ref lastCharType, ref i, true);
                        nearestNode = tmp ?? nearestNode;
                    }
                    else
                    {
                        buffer.Append(c);
                    }
                }
            }
            charIndex = i - 1;

            if (flag == 99)
            {
                if (recursive)
                {
                    // Es necesario para los paréntesis ya que puede llegar a crear nodos hijos.
                    nearestNode = ProcessBuffer(buffer, nearestNode ?? node, charIndex - 1, useFormattedText);
                }
                else
                {
                    ExpressionNode tmp = node;

                    // Se utiliza para los nodos de tipo valor. Se aplica el valor directamente al nodo.
                    if (useFormattedText)
                    {
                        tmp.FormattedText = buffer.ToString();
                    }
                    buffer.Clear();
                    nearestNode = tmp;
                }
            }
            else
            {
                // Do nothing: el resto del proceso es robusto y saltará un "FormatException".
            }
            return nearestNode;
        }

        /// <summary>
        /// Procesa el texto pendiente en el buffer.
        /// </summary>
        /// <param name="buffer">
        /// Almacenamiento temporal de la parte de la cadena que todavía no se ha interpretado como expressión.
        /// </param>
        /// <param name="node">
        /// Nodo de procesamiento.
        /// </param>
        /// <param name="charIndex">
        /// Índice del caracter que se está analizando dentro de <paramref name="node"/>
        /// </param>
        /// <param name="formattedText">
        /// Establece si el valor pendiente del buffer debe almacenarse en <see cref="ExpressionNode.FormattedText"/>.
        /// Este valor sustituirá al de la propiedad <see cref="ExpressionNode.Text"/> durante el procesamiento.
        /// </param>
        private ExpressionNode ProcessBuffer(StringBuilder buffer, ExpressionNode node, int charIndex, bool formattedText)
        {
            ExpressionNode nearestNode = null;

            if (buffer.Length > 0)
            {
                var textBuffer = buffer.ToString();

                if (OperationMap.TryGetValue(textBuffer, out ExpressionOperatorType @operator))
                {
                    // El buffer contiene un operador. Procesar el operador y reemplazar el nodo por el nuevo con operador si fuera preciso.
                    nearestNode = ProcessOperator(node, @operator, charIndex);
                }
                else
                {
                    // Crear un nodo con el contenido del buffer. 
                    // Este nodo parasá a ser el actual, es decir, sobre el que se seguirán colgando nodos.
                    int startIndex = charIndex - (textBuffer.Length - 1);
                    nearestNode = CreateNode(node, startIndex, charIndex, ExpressionNodeType.UnknownType,
                        formattedText: (formattedText ? textBuffer : null));
                }
                buffer.Clear();
            }
            return nearestNode;
        }

        private ExpressionNode CreateNode(ExpressionNode parent, int startIndex, int? endIndex = null,
            ExpressionNodeType nodeType = ExpressionNodeType.Default,
            ExpressionOperatorType @operator = ExpressionOperatorType.None,
            bool isLocked = false, string formattedText = null)
        {
            ExpressionNode node;

            node = parent.Nodes.AddNew();
            node.Id = ++_lastNodeId;
            node.Type = nodeType;
            node.Locked = isLocked;
            node.StartIndex = startIndex;
            node.EndIndex = endIndex;
            node.FormattedText = formattedText;
            node.Operator = @operator;
            return node;
        }

        #endregion

        #region methods static 


        private static ExpressionNode ProcessOperator(ExpressionNode current, ExpressionOperatorType @operator, int charIndex)
        {
            ExpressionNode rdo;

            switch (@operator)
            {
                case ExpressionOperatorType.Add:
                case ExpressionOperatorType.Substract:
                case ExpressionOperatorType.Multiply:
                case ExpressionOperatorType.Divide:
                case ExpressionOperatorType.Modulo:
                case ExpressionOperatorType.Power:
                    rdo = ProcessBufferArithmeticOperator(current, @operator);
                    break;

                case ExpressionOperatorType.And:
                case ExpressionOperatorType.Or:
                    rdo = ProcessBufferLogicOperator(current, @operator);
                    break;
                case ExpressionOperatorType.Not:
                    rdo = ProcessBufferLogicOperatorNot(current, charIndex);
                    break;

                case ExpressionOperatorType.Equals:
                case ExpressionOperatorType.NotEqual:
                case ExpressionOperatorType.GreaterThan:
                case ExpressionOperatorType.GreaterThanOrEqual:
                case ExpressionOperatorType.LessThan:
                case ExpressionOperatorType.LessThanOrEqual:
                case ExpressionOperatorType.Like:
                case ExpressionOperatorType.In:
                    rdo = ProcessBufferRelationalOperator(current, @operator);
                    break;

                default:
                    throw new NotImplementedException(@operator.ToString());
            }
            return rdo;
        }

        private static ExpressionNode ProcessBufferArithmeticOperator(ExpressionNode currentNode, ExpressionOperatorType @operator)
        {
            ExpressionNode rdo;
            bool isValidNode(ExpressionNodeType type)
            {
                return type.In(ExpressionNodeType.ArithmeticOperator, ExpressionNodeType.Default) || IsValueNode(type);
            }

            // Buscar el nodo donde se puede aplicar el operador.
            rdo = FindNodeForOperator(currentNode, @operator, isValidNode, GetArithmeticOperator);

            // Las operaciones se van solapando.
            ApplyParentNode(rdo, ExpressionNodeType.ArithmeticOperator, @operator);
            return rdo;
        }

        private static ExpressionNode ProcessBufferLogicOperator(ExpressionNode currentNode, ExpressionOperatorType @operator)
        {
            ExpressionNode rdo, tmp;
            bool isValidNode(ExpressionNodeType type)
            {
                return type.In(
                    ExpressionNodeType.LogicalOperator,
                    ExpressionNodeType.ArithmeticOperator,
                    ExpressionNodeType.RelationalOperator,
                    ExpressionNodeType.Default);
            }

            // Buscar el nodo donde se puede aplicar el operador.
            tmp = FindNodeForOperator(currentNode, @operator, isValidNode, GetLogicalOperator);

            // Las operaciones se van solapando.
            ApplyParentNode(tmp, ExpressionNodeType.LogicalOperator, @operator);
            rdo = tmp;
            return rdo;
        }

        private static ExpressionNode ProcessBufferLogicOperatorNot(ExpressionNode currentNode, int charIndex)
        {
            ExpressionNode newNode;

            newNode = currentNode.Nodes.AddNew();
            newNode.Id = currentNode.Id + 0.01F;
            newNode.StartIndex = charIndex;
            newNode.Type = ExpressionNodeType.LogicalOperator;
            newNode.Operator = ExpressionOperatorType.Not;
            return newNode;
        }

        private static ExpressionNode ProcessBufferRelationalOperator(ExpressionNode currentNode, ExpressionOperatorType @operator)
        {
            ExpressionNode rdo, tmp;
            bool isValidNode(ExpressionNodeType type)
            {
                return type.In(
                    ExpressionNodeType.ArithmeticOperator,
                    ExpressionNodeType.RelationalOperator,
                    ExpressionNodeType.Default);
            }

            // Buscar el nodo donde se puede aplicar el operador.
            tmp = FindNodeForOperator(currentNode, @operator, isValidNode, GetRelationalOperator);

            // Las operaciones se van solapando.
            ApplyParentNode(tmp, ExpressionNodeType.RelationalOperator, @operator);
            rdo = tmp;
            return rdo;
        }

        /// <summary>
        /// Converts a <see cref="String"/> expression to <see cref="ExpressionTree"/>.
        /// </summary>
        /// <param name="value">The <see cref="String"/> expression to parse.</param>
        /// <returns>The <see cref="ExpressionTree"/> parsed.</returns>
        public static ExpressionTree BuildTree(string value)
        {
            ExpressionTree tree;

            var fsp = new ExpressionStringBuilder();
            tree = fsp.ParseExpressionString(value);
            return tree;
        }

        /// <summary>
        /// Modifica la expresión del parámetro <paramref name="node"/> añadiendo primero los datos actuales a un nodo inferior.
        /// </summary>
        /// <param name="node">Nodo a modificar.</param>
        /// <param name="parentType">Nuevo tipo</param>
        /// <param name="parentOperator">Nuevo operador.</param>
        /// <returns>
        /// Devuelve el nodo creado bajo <paramref name="node"/> con las propiedades que éste tenía antes.
        /// </returns>
        private static ExpressionNode ApplyParentNode(ExpressionNode node, ExpressionNodeType parentType, ExpressionOperatorType parentOperator)
        {
            // Mover la expresión debajo del operador:
            // Crea una expresión, copia sus valores y sus nodos hijos.
            var rdo = new ExpressionNode(node.ExpressionTree)
            {
                Id = node.Id,
                Type = node.Type,
                Operator = node.Operator,
                StartIndex = node.StartIndex,
                EndIndex = node.EndIndex,
                FormattedText = node.FormattedText,
                Locked = node.Locked,
                ParentCollection = node.Nodes,
            };
            foreach (var child in node.Nodes.ToArray())
            {
                node.Nodes.Remove(child);
                rdo.Nodes.Add(child);
            }

            // Añade la expressión creada a la actual y aplica los nuevos valores.
            node.Nodes.Add(rdo);
            node.Id += 0.01F;
            node.Type = parentType;
            node.Operator = parentOperator;
            node.EndIndex = null;
            node.FormattedText = null;
            node.Locked = false;
            return rdo;
        }

        /// <summary>
        /// Devuelve el tipo de elemento almacenado en la cadena <paramref name="value"/>.
        /// </summary>
        /// <param name="value">La cadena a ser analizada.</param>
        /// <remarks>
        /// Se utiliza para detectar cuándo pasa de valor a operador, o paréntesis u otra cosa.
        /// </remarks>
        private static CharType GetCharType(string value)
        {
            CharType rdo = CharType.None;

            if (value?.Length > 0)
            {
                if (OperationMap.ContainsKey(value))
                    rdo = CharType.Operator;
                else if (value.Length == 1 && BreakerChars.Contains(value[0]))
                    rdo = CharType.Breaker;
                else if (value.Length == 1 && (DelimiterChars.ContainsKey(value[0]) || DelimiterChars.ContainsValue(value[0])))
                    rdo = CharType.Delimiter;
                else
                    rdo = CharType.Other;
            }
            return rdo;
        }

        /// <summary>
        /// Devuelve si el tipo de nodo puede contener un valor.
        /// </summary>
        /// <param name="nodeType">Tipo de nodo a validar.</param>
        /// <returns>True si el tipo puede contener un valor, false en caso contrario.</returns>
        private static bool IsValueNode(ExpressionNodeType nodeType)
        {
            return nodeType.In(
                ExpressionNodeType.UnknownType,
                ExpressionNodeType.StringType,
                ExpressionNodeType.DateType,
                ExpressionNodeType.ListType,
                ExpressionNodeType.PropertyType);
        }

        /// <summary>
        /// Devuelve si el tipo de nodo es un operador.
        /// </summary>
        /// <param name="nodeType">Tipo de nodo a validar.</param>
        /// <returns>True si el tipo es un operador, false en caso contrario.</returns>
        private static bool IsOperatorNode(ExpressionNodeType nodeType)
        {
            return nodeType.In(
                ExpressionNodeType.ArithmeticOperator,
                ExpressionNodeType.LogicalOperator,
                ExpressionNodeType.RelationalOperator);
        }

        /// <summary>
        /// Devuelve la prioridad de cálculo del operador de menor a mayor. 
        /// Esto permite hacer las multiplicaciones antes que las sumas, las potencias antes que las multiplicaciones, los AND antes que los OR, etc.
        /// </summary>
        /// <param name="operator">Operador a validar.</param>
        /// <returns>La prioridad de cálculo del operador, de menor a mayor.</returns>
        /// <remarks>
        /// <seealso href="https://stackoverflow.com/questions/1241142/sql-logic-operator-precedence-and-and-or"/>
        /// <seealso href="https://docs.microsoft.com/en-us/sql/t-sql/language-elements/operator-precedence-transact-sql?view=sql-server-2017"/>
        /// </remarks>
        private static int GetOperatorPriority(ExpressionOperatorType @operator)
        {
            int rdo;

            switch (@operator)
            {
                case ExpressionOperatorType.Like:
                case ExpressionOperatorType.In:
                case ExpressionOperatorType.Or:
                    rdo = 1;
                    break;
                case ExpressionOperatorType.And:
                    rdo = 2;
                    break;
                case ExpressionOperatorType.Not:
                    rdo = 3;
                    break;
                case ExpressionOperatorType.Equals:
                case ExpressionOperatorType.NotEqual:
                case ExpressionOperatorType.GreaterThan:
                case ExpressionOperatorType.GreaterThanOrEqual:
                case ExpressionOperatorType.LessThan:
                case ExpressionOperatorType.LessThanOrEqual:
                    rdo = 4;
                    break;
                case ExpressionOperatorType.Add:
                case ExpressionOperatorType.Substract:
                    rdo = 5;
                    break;
                case ExpressionOperatorType.Multiply:
                case ExpressionOperatorType.Divide:
                case ExpressionOperatorType.Modulo:
                    rdo = 6;
                    break;
                case ExpressionOperatorType.Power:
                    rdo = 7;
                    break;
                default:
                    rdo = 0;
                    break;
            }
            return rdo;
        }

        /// <summary>
        /// Devuelve el operador adjunto a un nodo específico.
        /// </summary>
        /// <param name="node">Nodo de búsqueda.</param>
        /// <returns>El operador adjunto al nodo especificado.</returns>
        private static ExpressionOperatorType GetArithmeticOperator(ExpressionNode node)
        {
            ExpressionOperatorType? rdo = null;

            do
            {
                var parentNode = node.ParentNode;

                if (parentNode == null)
                    rdo = node.Operator; // Primer nivel.
                else if (node.Type == ExpressionNodeType.ArithmeticOperator)
                    rdo = node.Operator; // Operación aritmética.
                else if (parentNode.Locked)
                    rdo = parentNode.Operator; // El padre es paréntesis, no se puede subir más arriba.
                else
                    node = parentNode; // Probar suerte con el padre.
            } while (rdo == null);
            return rdo.Value;
        }

        /// <summary>
        /// Devuelve el operador adjunto a un nodo específico.
        /// </summary>
        /// <param name="node">Nodo de búsqueda.</param>
        /// <returns>El operador adjunto al nodo especificado.</returns>
        private static ExpressionOperatorType GetLogicalOperator(ExpressionNode node)
        {
            ExpressionOperatorType? rdo = null;

            do
            {
                var parentNode = node.ParentNode;

                if (parentNode == null)
                    rdo = node.Operator; // Primer nivel.
                else if (node.Type == ExpressionNodeType.LogicalOperator)
                    rdo = node.Operator; // Operación lógica.
                else if (parentNode.Locked)
                    rdo = parentNode.Operator; // El padre es paréntesis, no se puede subir más arriba.
                else
                    node = parentNode; // Probar suerte con el padre.
            } while (rdo == null);
            return rdo.Value;
        }

        /// <summary>
        /// Devuelve el operador adjunto a un nodo específico.
        /// </summary>
        /// <param name="node">Nodo de búsqueda.</param>
        /// <returns>El operador adjunto al nodo especificado.</returns>
        private static ExpressionOperatorType GetRelationalOperator(ExpressionNode node)
        {
            ExpressionOperatorType? rdo = null;

            do
            {
                var parentNode = node.ParentNode;

                if (parentNode == null)
                    rdo = node.Operator; // Primer nivel.
                else if (node.Type == ExpressionNodeType.RelationalOperator)
                    rdo = node.Operator; // Operación relacional.
                else if (parentNode.Locked)
                    rdo = parentNode.Operator; // El padre es paréntesis, no se puede subir más arriba.
                else
                    node = parentNode; // Probar suerte con el padre.
            } while (rdo == null);
            return rdo.Value;
        }

        /// <summary>
        /// Devuelve un valor que indica si la rama en la que se encuentra el nodo es una rama aritmética.
        /// </summary>
        /// <param name="node">Nodo de búsqueda.</param>
        /// <returns>True si la rama es aritmética, false en caso contrario.</returns>
        /// <remarks>
        /// Se utiliza para poder diferenciar si el símbolo "-" es porque va a ser un número negativo o una resta.
        /// </remarks>
        private static bool IsArithmeticBranch(ExpressionNode node)
        {
            bool rdo;

            if (node.Type == ExpressionNodeType.Default && node.Nodes.Count == 1)
            {
                rdo = IsArithmeticBranch(node.Nodes[0]);
            }
            else
            {
                rdo = (GetArithmeticOperator(node) != ExpressionOperatorType.None);
            }
            return rdo;
        }

        /// <summary>
        /// Devuelve el nodo más próximo a <paramref name="currentNode"/> al que se le pueda asignar la operación especificada.
        /// </summary>
        /// <param name="currentNode">Nodo de inicio de búsqueda.</param>
        /// <param name="operator">Operador que se desea aplicar.</param>
        /// <param name="isValidNodeFunc">Función que valida los nodos sobre los que puede buscar.</param>
        /// <param name="getOperatorFunc">Función que devuelve el tipo de operador del nodo durante las búsquedas.</param>
        /// <returns>El nodo más próximo al que se le pueda asignar la operación especificada.</returns>
        private static ExpressionNode FindNodeForOperator(ExpressionNode currentNode, ExpressionOperatorType @operator,
            Func<ExpressionNodeType, bool> isValidNodeFunc, Func<ExpressionNode, ExpressionOperatorType> getOperatorFunc)
        {
            ExpressionNode rdo;
            var repeat = true;
            var newPriority = GetOperatorPriority(@operator);
            var currentOperator = getOperatorFunc(currentNode);
            var currentPriority = GetOperatorPriority(currentOperator);

            rdo = currentNode;
            if (isValidNodeFunc(rdo.Type) && newPriority > currentPriority)
            {
                repeat = false; // Puesto que la operación anterior es de menor prioridad, este es el nivel correcto.
            }

            while (repeat)
            {
                var parentNode = rdo.ParentNode;

                if (parentNode == null)
                {
                    repeat = false; // Top level, no se puede ir más arriba.
                }
                else if (parentNode.Locked || !isValidNodeFunc(parentNode.Type))
                {
                    // A partir de la primera vez, si el nodo está bloqueado, ya no se podrá subir más hacia arriba
                    // (el nodo inicial puede ser de paréntesis, pero está fuera de é).
                    repeat = false;
                }
                else
                {
                    var parentOperator = getOperatorFunc(parentNode);
                    var parentPriority = GetOperatorPriority(parentOperator);

                    if (isValidNodeFunc(rdo.Type) && newPriority > parentPriority)
                    {
                        repeat = false; // Puesto que la operación de nivel superior es de menor prioridad, este es el nivel correcto.
                    }
                    else
                    {
                        rdo = parentNode; // Probar suerte con el padre.
                    }
                }
            }
            return rdo;
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