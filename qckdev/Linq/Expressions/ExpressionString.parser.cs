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
            LastLogicNodes = new List<ExpressionNode>();
        }

        #endregion


        #region properties

        StringBuilder StringBuffer { get; }

        List<ExpressionNode> LastLogicNodes { get; }

        ExpressionNode LastRelationalNode { get; set; }

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
        [SuppressMessage("Minor Code Smell", "S3241:Methods should not return values that are never used", Justification = "Maybe it is necessary in the future.")]
        private ExpressionNode AddChildNodeFromOpenCharacter(ExpressionNode parentNode, ref int charIndex)
        {
            ExpressionNode childNode = null;
            string value = parentNode.ExpressionTree.Value;
            char c = value[charIndex];
            Func<char, bool> closeCriteria;
            var myparent = this.CurrentNode ?? parentNode;

            switch (c)
            {
                case '(':
                    closeCriteria = delegate (char x) { return x == DelimiterChars[c]; };
                    childNode = myparent.Nodes.AddNew();
                    childNode.Locked = true; // Bloquear el nodo ya que es una expresión entre paréntesis (lo que se encuentre dentro deberá ir dentro y lo que se encuentre fuera deberá ir fuera).
                    if (myparent.Operator == ExpressionOperatorType.In)
                    {
                        childNode.Type = ExpressionNodeType.ListType;
                        LastRelationalNode = childNode;
                    }
                    else
                    {
                        LastRelationalNode = null;
                    }
                    CurrentNode = childNode;
                    SetEndIndex(childNode, ref charIndex, closeCriteria, recursive: true);
                    CurrentNode = childNode;
                    break;

                case '\'':
                case '#':
                case '[':
                    closeCriteria = delegate (char x) { return x == DelimiterChars[c]; };
                    childNode = myparent.Nodes.AddNew();
                    childNode.Type = DelimiterTypes[c];
                    CurrentNode = childNode;
                    SetEndIndex(childNode, ref charIndex, closeCriteria, recursive: false);
                    CurrentNode = childNode;
                    break;

                default:
                    throw new NotSupportedException();

            }
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
        /// <returns></returns>
        private void SetEndIndex(ExpressionNode node, ref int charIndex, Func<char, bool> closeCriteria, bool recursive)
        {
            var lastCharType = CharType.None;
            var value = node.ExpressionTree.Value;
            var flag = 0; // 0 None, 1 Posible fin, 2 Escape, 99 found.
            var useFormattedText = false;
            int i;

            node.StartIndex = charIndex;
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
                            throw new NotSupportedException($"Oops! Something was not planned for method {nameof(SetEndIndex)}.\n{value}"); // Control de robustez.
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
                        throw new NotSupportedException($"Oops! Something was not planned for method {nameof(SetEndIndex)}.\n{value}"); // Control de robustez.
                }
                else
                {
                    // Se utiliza para los nodos de tipo valor. Se aplica el valor directamente al nodo.
                    if (useFormattedText)
                        node.FormattedText = StringBuffer.ToString();
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
            LastRelationalNode = null;
            switch (@operator)
            {
                case ExpressionOperatorType.And:
                case ExpressionOperatorType.Or:
                    ProcessBufferLogicOperator(ref parent, @operator, currentIndex);
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
            int newPriority = GetOperatorPriority(@operator);
            bool repeat = true;
            var currentOperator = GetArithmeticOperator(currentNode);
            var currentPriority = GetOperatorPriority(currentOperator);

            if (newPriority >= currentPriority)
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
                else if (parentNode.Locked)
                {
                    // A partir de la primera vez, si el nodo está bloqueado, ya no se podrá subir más hacia arriba
                    // (el nodo inicial puede ser de paréntesis, pero está fuera de é).
                    repeat = false;
                }
                else
                {
                    var parentOperator = GetArithmeticOperator(parentNode);
                    var parentPriority = GetOperatorPriority(parentOperator);

                    if (newPriority >= parentPriority)
                    {
                        repeat = false; // Puesto que la operación de nivel superior es de menor prioridad, este es el nivel correcto.
                    }
                    else
                    {
                        currentNode = parentNode; // Probar suerte con el padre.
                    }
                }
            }

            // Las operaciones se van solapando.
            ApplyParentNode(currentNode, ExpressionNodeType.ArithmeticOperator, @operator);
        }

        private void ProcessBufferLogicOperator(ref ExpressionNode parent, ExpressionOperatorType @operator, int currentIndex)
        {
            var backIndex = (currentIndex - StringBuffer.Length - 1);
            var previousParent = parent;

            CurrentNode?.UpdateEndIndex();
            CurrentNode = null;
            LastRelationalNode?.UpdateEndIndex();
            LastRelationalNode = null;

            parent = ApplyOrCreateLogicOperator(previousParent, @operator, backIndex);
            if (parent != previousParent)
            {
                LastLogicNodes.TryReplace(previousParent, parent);
            }
        }

        private void ProcessBufferLogicOperatorNot(ExpressionNode myparent, ExpressionOperatorType @operator, int currentIndex)
        {
            ExpressionNode rdo;

            CurrentNode?.UpdateEndIndex();
            CurrentNode = null;
            LastRelationalNode?.UpdateEndIndex();
            LastRelationalNode = null;

            rdo = myparent.Nodes.AddNew();
            rdo.StartIndex = currentIndex;
            rdo.Type = ExpressionNodeType.LogicalOperator;
            rdo.Operator = @operator;
        }

        private void ProcessBufferEqualityOperator(ExpressionNode myparent, ExpressionOperatorType @operator)
        {
            if (LastRelationalNode == null)
            {
                if (CurrentNode == null)
                {
                    throw new FormatException("Expression operator must have some property or constant.");
                }
                else
                {
                    ApplyParentNode(myparent, ExpressionNodeType.RelationalOperator, @operator); // Convierte el nodo en un nodo de tipo RelationalOperator y crea por debajo un nodo con la información del nodo original.
                    CurrentNode = myparent;
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
        /// Devuelve la prioridad de cálculo del operador de menor a mayor. 
        /// Esto permite hacer las multiplicaciones antes que las sumas y las potencias antes que las multiplicaciones.
        /// </summary>
        /// <param name="operator">Operador a validar.</param>
        /// <returns>La prioridad de cálculo del operador, de menor a mayor.</returns>
        /// <remarks>
        /// <seealso href="https://stackoverflow.com/questions/1241142/sql-logic-operator-precedence-and-and-or"/>
        /// </remarks>
        private static int GetOperatorPriority(ExpressionOperatorType @operator)
        {
            int rdo;

            switch (@operator)
            {
                case ExpressionOperatorType.Add:
                case ExpressionOperatorType.Substract:
                case ExpressionOperatorType.Or:
                    rdo = 0;
                    break;
                case ExpressionOperatorType.Multiply:
                case ExpressionOperatorType.Divide:
                case ExpressionOperatorType.And:
                    rdo = 1;
                    break;
                case ExpressionOperatorType.Power:
                    rdo = 2;
                    break;
                case ExpressionOperatorType.Modulo:
                    throw new NotImplementedException(@operator.ToString());
                default:
                    rdo = 0;
                    break; // Do Nothing.
            }
            return rdo;
        }

        /// <summary>
        /// Devuelve el operador adjunto al nodo actual.
        /// </summary>
        /// <param name="currentNode"></param>
        /// <returns></returns>
        private static ExpressionOperatorType GetArithmeticOperator(ExpressionNode currentNode)
        {
            ExpressionOperatorType? rdo = null;

            do
            {
                var parentNode = currentNode.ParentNode;

                if (parentNode == null)
                    rdo = currentNode.Operator; // Primer nivel.
                else if (currentNode.Type == ExpressionNodeType.ArithmeticOperator)
                    rdo = currentNode.Operator; // Operación aritmética.
                else if (parentNode.Locked)
                    rdo = parentNode.Operator; // El padre es paréntesis, no se puede subir más arriba.
                else
                    currentNode = parentNode; // Probar suerte con el padre.
            } while (rdo == null);
            return rdo.Value;
        }

        #endregion

    }
}