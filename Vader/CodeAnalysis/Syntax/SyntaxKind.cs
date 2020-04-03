namespace Vader.CodeAnalysis.Syntax
{
    public enum SyntaxKind
    {
        //tokens
        BadToken,
        EndOfFileToken,
        WhitespaceToken,
        NumberToken,
        PlusToken,
        MinusToken,
        StarToken,
        SlashToken,
        EqualsToken,
        BangToken,
        AmpersandAmpersandToken,
        PipePipeToken,
        EqualEqualToken,
        BangEqualToken,
        IdentifierToken,
        OpenParenthesisoken,
        CloseParenthesisToken,

        //expressions
        BinaryExpression,
        ParenthesizedExpression,
        LiteralExpression,
        UnaryExpression,
        NameExpression,
        AssignmentExpression,

        //Literals
        FalseKeyword,
        TrueKeyword,
    }
}