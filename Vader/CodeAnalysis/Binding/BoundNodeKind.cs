namespace Vader.CodeAnalysis.Binding
{
    internal enum BoundNodeKind
    {
        //Expressions
        UnaryExpression,
        LiteralExpression,
        BinaryExpression,
        VariableExpression,
        AssignmentExpression,

        //Statements
        BlockStatement,
        ExpressionStatement
    }
}