using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
        static readonly Dictionary<string, ExpressionOperatorType> OperationMap = new Dictionary<string, ExpressionOperatorType>()
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

        private ExpressionString()
        {
            StringBuffer = new StringBuilder();
        }

        #endregion


        #region properties

        StringBuilder StringBuffer { get; }

        ExpressionNode CurrentNode { get; set; }

        #endregion


        #region methods

        /// <summary>
        /// Recorre todos los caracteres de la cadena y los procesa.
        /// </summary>
        /// <param name="value">Cadena de texto.</param>
        /// <returns>El <see cref="ExpressionTree"/> con los elementos de la cadena.</returns>
        private ExpressionTree ParseExpressionString(string value)
        {
            var tree = ExpressionTree.Create(value);
            var root = tree.Root;

            // Procesar cadena.
            ProcessSubstring(ref root, 0, null, delimiterOpened: false);

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
        /// <param name="node">Nodo de trabajo.</param>
        /// <param name="startIndex">Índice del primer caracter a leer en <see cref="ExpressionTree.Value"/></param>
        /// <param name="endIndex">Índice del último caracter a leer en <see cref="ExpressionTree.Value"/> o null para leer hasta el final.</param>
        /// <param name="delimiterOpened">
        /// Indica si se está buscando dentro de una zona incluida en <see cref="DelimiterChars"/>, 
        /// para no lanzar error de formato si encuentra un carácter de cierre.
        /// </param>
        private void ProcessSubstring(ref ExpressionNode node, int startIndex, int? endIndex, bool delimiterOpened)
        {
            var value = node.ExpressionTree.Value;
            var lastCharType = CharType.None;
            int i;

            for (i = startIndex; i <= (endIndex ?? value.Length - 1); i++)
            {
                ProcessChar(ref node, ref lastCharType, ref i, delimiterOpened);
            }
            ProcessBuffer(ref node, i - 1, formattedText: false);
        }

        /// <summary>
        /// Analiza el caracter en una cadena y lo procesa, pudiendo crear nodos hijos, asignarles un operador o rellenar en <see cref="StringBuffer"/>.
        /// </summary>
        /// <param name="node">
        /// Nodo que se está procesando en estos momentos. Si es conveniente, este nodo puede ser reemplazado por otro
        /// (por ejemplo, este nodo pasa a formar parte de otro debido a las operaciones siguientes).
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
        private void ProcessChar(ref ExpressionNode node, ref CharType lastCharType, ref int charIndex, bool delimiterOpened)
        {
            var value = node.ExpressionTree.Value;
            var c = value[charIndex];

            if (DelimiterChars.ContainsKey(c))
            {
                // Se ha detectado un carácter de apertura (un paréntesis, una comilla, un corchete...). 
                // Procesa su contenido hasta en contrar el carácter de cierre.
                // Mueve el contador hasta el último carácter leído.
                ProcessBuffer(ref node, charIndex - 1, formattedText: false);
                AddChildNodeFromOpenCharacter(node, ref charIndex);
            }
            else if (!delimiterOpened && DelimiterChars.ContainsValue(c))
            {
                throw new FormatException($"Invalid format for the following sentence:\n{value}\nClose character {c} was found without its opening one.");
            }
            else if (BreakerChars.Contains(c))
            {
                ProcessBuffer(ref node, charIndex - 1, formattedText: false);
            }
            else
            {
                if (StringBuffer.Length > 0 && lastCharType != GetCharType(c.ToString()))
                    ProcessBuffer(ref node, charIndex - 1, formattedText: false);

                StringBuffer.Append(c);
            }
            lastCharType = GetCharType(StringBuffer.ToString());
        }

        /// <summary>
        /// Crea un sub elemento a partir de un caracter de apertura.
        /// </summary>
        /// <param name="parentNode">Nodo actual de procesamiento.</param>
        /// <param name="charIndex">
        /// Índice del caracter que se está analizando dentro de <paramref name="parentNode"/>
        /// (<seealso cref="ExpressionNode.ExpressionTree"/>, <seealso cref="ExpressionTree.Value"/>).
        /// Si es conveniente, este valor puede ser reemplazado por otro (por ejemplo, al encontrar una apertura de paréntesis, 
        /// se moverá hasta el paréntesis de cierre.
        /// </param>
        /// <exception cref="FormatException">La cadena a procesar es incorrecta.</exception>
        [SuppressMessage("Minor Code Smell", "S3241:Methods should not return values that are never used", Justification = "Return value could be necessary in the future.")]
        private ExpressionNode AddChildNodeFromOpenCharacter(ExpressionNode parentNode, ref int charIndex)
        {
            ExpressionNode childNode = null;
            var currentNode = this.CurrentNode ?? parentNode;
            string value = currentNode.ExpressionTree.Value;
            char c = value[charIndex];
            Func<char, bool> closeCriteria;
            ExpressionNodeType nodeType;
            bool isLocked;
            bool recursive;

            switch (c)
            {
                case '(':
                    isLocked = true; // Bloquear el nodo ya que es una expresión entre paréntesis (lo que se encuentre dentro deberá ir dentro y lo que se encuentre fuera deberá ir fuera).
                    recursive = true;
                    if (currentNode.Operator == ExpressionOperatorType.In)
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
            closeCriteria = delegate (char x) { return x == DelimiterChars[c]; };
            childNode = currentNode.Nodes.AddNew();
            childNode.Type = nodeType;
            childNode.Locked = isLocked;
            childNode.StartIndex = charIndex;
            CurrentNode = childNode;

            CloseNodeFromOpenCharacter(childNode, ref charIndex, closeCriteria, recursive);
            CurrentNode = childNode;
            if (childNode.EndIndex == null)
            {
                // Si llega null, es que no se encontró caracter de cierre, por lo que el formato de texto probablemente sea erróneo.
                throw new FormatException($"Invalid format for the following sentence:\n{value}\nClose character {closeCriteria} not found.");
            }
            return childNode;
        }

        /// <summary>
        /// Recorre el texto en busca hasta que se encuenta el caracter de cierre.
        /// </summary>
        /// <param name="node">Nodo actual de procesamiento.</param>
        /// <param name="charIndex"></param>
        /// <param name="closeCriteria"></param>
        /// <param name="recursive"></param>
        [SuppressMessage("Critical Code Smell", "S3776:Cognitive Complexity of methods should not be too high", Justification = "Cognitive complexity could be higher if this method is split.")]
        private void CloseNodeFromOpenCharacter(ExpressionNode node, ref int charIndex, Func<char, bool> closeCriteria, bool recursive)
        {
            var lastCharType = CharType.None;
            var value = node.ExpressionTree.Value;
            var flag = 0; // [0] Continuar leyendo; [1] Posible fin; [2] Caracter de escape; [99] Fin de la cadena.
            var useFormattedText = false;
            int i;

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
                        var processNode = node;

                        ProcessChar(ref processNode, ref lastCharType, ref i, true);
                        if (processNode != node)
                            throw new NotSupportedException($"Oops! Something was not planned for method {nameof(CloseNodeFromOpenCharacter)}.\n{value}"); // Control de robustez.
                    }
                    else
                    {
                        StringBuffer.Append(c);
                    }
                }
            }
            charIndex = i - 1;

            if (flag == 99)
            {
                if (recursive)
                {
                    // Es necesario para los paréntesis ya que puede llegar a crear nodos hijos.
                    var processNode = node;
                    ProcessBuffer(ref processNode, processNode.EndIndex.Value - 1, useFormattedText);
                    if (processNode != node)
                        throw new NotSupportedException($"Oops! Something was not planned for method {nameof(CloseNodeFromOpenCharacter)}.\n{value}"); // Control de robustez.
                }
                else
                {
                    // Se utiliza para los nodos de tipo valor. Se aplica el valor directamente al nodo.
                    if (useFormattedText)
                        node.FormattedText = StringBuffer.ToString();
                    this.CurrentNode = node;
                    StringBuffer.Clear();
                }
            }
            else
            {
                // TODO: Controlar error o continuar adelante? (creo que si continua adelante el ProcessBuffer hará el resto pero hay que probarlo).
            }
        }

        /// <summary>
        /// Procesa el texto pendiente en el buffer.
        /// </summary>
        /// <param name="parentNode">
        /// Nodo actual de procesamiento.
        /// Para <see cref="ProcessBufferLogicOperator"/> el nodo podría ser sustituido por otro.
        /// </param>
        /// <param name="charIndex">
        /// Índice del caracter que se está analizando dentro de <paramref name="parentNode"/>
        /// </param>
        /// <param name="formattedText">
        /// Establece si el valor pendiente del buffer debe almacenarse en <see cref="ExpressionNode.FormattedText"/>.
        /// Este valor sustituirá al de la propiedad <see cref="ExpressionNode.Text"/> durante el procesamiento.
        /// </param>
        private void ProcessBuffer(ref ExpressionNode parentNode, int charIndex, bool formattedText)
        {

            if (StringBuffer.Length > 0)
            {
                var upperTextBuffer = StringBuffer.ToString().ToUpperInvariant();
                var currentNode = this.CurrentNode ?? parentNode; //(LastArithmeticNode ?? LastRelationalNode ?? LastValueNode ?? parentNode);  // Para variables, operaciones aritmeticas y relacionales.

                if (OperationMap.TryGetValue(upperTextBuffer, out ExpressionOperatorType @operator))
                {
                    ProcessOperator(ref parentNode, ref currentNode, @operator, charIndex);
                }
                else
                {
                    ExpressionNode newNode;

                    newNode = currentNode.Nodes.AddNew();
                    newNode.StartIndex = charIndex - (StringBuffer.Length - 1);
                    newNode.Type = ExpressionNodeType.UnknownType;
                    if (formattedText)
                    {
                        newNode.FormattedText = StringBuffer.ToString();
                    }
                    newNode.EndIndex = charIndex;
                    currentNode = newNode;
                }
                this.CurrentNode = currentNode;
                StringBuffer.Clear();
            }
        }


        private void ProcessOperator(ref ExpressionNode parent, ref ExpressionNode current, ExpressionOperatorType @operator, int currentIndex)
        {
            switch (@operator)
            {
                case ExpressionOperatorType.And:
                case ExpressionOperatorType.Or:
                    ProcessBufferLogicOperator(ref current, @operator);
                    break;

                case ExpressionOperatorType.Not:
                    ProcessBufferLogicOperatorNot(current, @operator, currentIndex);
                    break;

                case ExpressionOperatorType.Equals:
                case ExpressionOperatorType.NotEqual:
                case ExpressionOperatorType.GreaterThan:
                case ExpressionOperatorType.GreaterThanOrEqual:
                case ExpressionOperatorType.LessThan:
                case ExpressionOperatorType.LessThanOrEqual:
                case ExpressionOperatorType.Like:
                case ExpressionOperatorType.In:
                    ProcessBufferEqualityOperator(current, @operator);
                    break;

                case ExpressionOperatorType.Add:
                case ExpressionOperatorType.Substract:
                case ExpressionOperatorType.Multiply:
                case ExpressionOperatorType.Divide:
                case ExpressionOperatorType.Modulo:
                case ExpressionOperatorType.Power:
                    ProcessBufferArithmeticOperator(ref current, @operator);
                    break;

                default:
                    throw new NotImplementedException(@operator.ToString());
            }
        }

        private void ProcessBufferArithmeticOperator(ref ExpressionNode currentNode, ExpressionOperatorType @operator)
        {
            bool isValidNode(ExpressionNodeType type)
            {
                return type.In(ExpressionNodeType.ArithmeticOperator, ExpressionNodeType.Default) || IsValueNode(type);
            }

            // Buscar el nodo donde se puede aplicar el operador.
            currentNode = FindNodeForOperator(currentNode, @operator, isValidNode, GetArithmeticOperator);

            // Las operaciones se van solapando.
            ApplyParentNode(currentNode, ExpressionNodeType.ArithmeticOperator, @operator);
        }

        private void ProcessBufferLogicOperator(ref ExpressionNode currentNode, ExpressionOperatorType @operator)
        {
            bool isValidNode(ExpressionNodeType type)
            {
                return type.In(
                    ExpressionNodeType.LogicalOperator,
                    ExpressionNodeType.ArithmeticOperator,
                    ExpressionNodeType.RelationalOperator);
            }

            // Buscar el nodo donde se puede aplicar el operador.
            currentNode = FindNodeForOperator(currentNode, @operator, isValidNode, GetLogicalOperator);

            // Las operaciones se van solapando.
            ApplyParentNode(currentNode, ExpressionNodeType.LogicalOperator, @operator);
        }

        private void ProcessBufferLogicOperatorNot(ExpressionNode myparent, ExpressionOperatorType @operator, int currentIndex)
        {
            ExpressionNode rdo;

            CurrentNode?.UpdateEndIndex();
            CurrentNode = null;

            rdo = myparent.Nodes.AddNew();
            rdo.StartIndex = currentIndex;
            rdo.Type = ExpressionNodeType.LogicalOperator;
            rdo.Operator = @operator;
        }

        private void ProcessBufferEqualityOperator(ExpressionNode myparent, ExpressionOperatorType @operator)
        {
            if (CurrentNode == null)
            {
                throw new FormatException("Expression operator must have some property or constant.");
            }
            else
            {
                ApplyParentNode(myparent, ExpressionNodeType.RelationalOperator, @operator); // Convierte el nodo en un nodo de tipo RelationalOperator y crea por debajo un nodo con la información del nodo original.
                CurrentNode = myparent;
            }
        }

        #endregion

        #region methods static 

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
                EndIndex = node.EndIndex,
                FormattedText = node.FormattedText,
                Locked = node.Locked,
            };
            rdo.Nodes.AddRange(node.Nodes);

            node.Nodes.Clear();
            node.Nodes.Add(rdo);
            node.Type = parentType;
            node.Operator = parentOperator;
            node.EndIndex = null;
            node.FormattedText = null;
            node.Locked = false;
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
            bool repeat = true;
            int newPriority = GetOperatorPriority(@operator);
            var currentOperator = getOperatorFunc(currentNode);
            var currentPriority = GetOperatorPriority(currentOperator);

            if (isValidNodeFunc(currentNode.Type) && newPriority >= currentPriority)
            {
                repeat = false; // Puesto que la operación anterior es de menor prioridad, este es el nivel correcto.
            }

            while (repeat)
            {
                var parentNode = currentNode.ParentNode;

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

                    if (isValidNodeFunc(currentNode.Type) && newPriority >= parentPriority)
                    {
                        repeat = false; // Puesto que la operación de nivel superior es de menor prioridad, este es el nivel correcto.
                    }
                    else
                    {
                        currentNode = parentNode; // Probar suerte con el padre.
                    }
                }
            }
            return currentNode;
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