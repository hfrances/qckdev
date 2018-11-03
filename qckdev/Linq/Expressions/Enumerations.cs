using System;

namespace qckdev.Linq.Expressions
{

    /// <summary>
    /// Defines the operation type to be applied in an <see cref="ExpressionNode"/>.
    /// </summary>
    public enum ExpressionOperatorType
    {
        /// <summary>The <see cref="ExpressionNode"/>there is not any operation.</summary>
        None,
        /// <summary>Represents an equality comparison.</summary>
        Equals,
        /// <summary>Represents an inequality comparison.</summary>
        NotEqual,
        /// <summary>A node that represents a "greater than" numeric comparison.</summary>
        GreaterThan,
        /// <summary>A node that represents a "greater than or equal" numeric comparison.</summary>
        GreaterThanOrEqual,
        /// <summary>A node that represents a "less than" numeric comparison.</summary>
        LessThan,
        /// <summary>A node that represents a "less than or equal" numeric comparison.</summary>
        LessThanOrEqual,

        /// <summary>Represents an string comparison agains a pattern.</summary>
        Like,
        /// <summary>
        /// Represents an equality comparison for a collection of elements.
        /// See also <seealso cref="ExpressionNodeType.ListType"/>.
        /// </summary>
        In,

        /// <summary>A node that represents a short-circuiting conditional AND operation.</summary>
        And,
        /// <summary>A node that represents a short-circuiting conditional OR operation.</summary>
        Or,
        /// <summary>A node that represents a bitwise complement operation.</summary>
        Not,

        /// <summary>A node that represents arithmetic addition.</summary>
        Add,
        /// <summary>A node that represents arithmetic subtraction.</summary>
        Substract,
        /// <summary>A node that represents arithmetic multiplication.</summary>
        Multiply,
        /// <summary>A node that represents arithmetic division.</summary>
        Divide,
        /// <summary>A node that represents an arithmetic remainder operation.</summary>
        Modulo,
        /// <summary>A node that represents raising a number to a power.</summary>
        Power,

    }

    /// <summary>
    /// Defines the expression type that represents an <see cref="ExpressionNode"/>.
    /// </summary>
    public enum ExpressionNodeType
    {
        /// <summary>Unknown <see cref="ExpressionNodeType"/>.</summary>
        Default,
        /// <summary>A node that represents a list of elements in <see cref="ExpressionOperatorType.In"/> operations.</summary>
        ListType,
        /// <summary>A node that represents a <see cref="String"/> constant.</summary>
        StringType,
        /// <summary>A node that represents a <see cref="DateTime"/> constant.</summary>
        DateType,
        /// <summary>A node that represents a <see cref="System.Reflection.PropertyInfo"/> constant.</summary>
        PropertyType,
        /// <summary>A node that represents a contant whose value type could not be defined.</summary>
        UnknownType,
        /// <summary>
        /// A node that represents a relational operator. 
        /// For example <see cref="ExpressionOperatorType.Equals"/>, <see cref="ExpressionOperatorType.GreaterThan"/>, ...
        /// </summary>
        RelationalOperator,
        /// <summary>
        /// A node that represents a relational operator. 
        /// For example <see cref="ExpressionOperatorType.And"/>, <see cref="ExpressionOperatorType.Not"/>, ...
        /// </summary>
        LogicalOperator,
        /// <summary>
        /// A node that represents a relational operator. 
        /// For example <see cref="ExpressionOperatorType.Add"/>, <see cref="ExpressionOperatorType.Modulo"/>, ...
        /// </summary>
        ArithmeticOperator,
    }

}
