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
        VariableDeclaration,

        //Statements
        BlockStatement,
        ExpressionStatement,
        IfStatement,
        WhileStatement,
        ForStatement,
    }
}