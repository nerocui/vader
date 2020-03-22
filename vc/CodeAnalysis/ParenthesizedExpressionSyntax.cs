using System.Collections.Generic;

namespace Vader.CodeAnalysis
{
    sealed class ParenthesizedExpressionSyntax : ExpressionSyntax
    {
        public ParenthesizedExpressionSyntax(SyntaxToken openParenthesisoken, ExpressionSyntax expression, SyntaxToken closeParenthesisToken)
        {
            OpenParenthesisoken = openParenthesisoken;
            Expression = expression;
            CloseParenthesisToken = closeParenthesisToken;
        }

        public SyntaxToken OpenParenthesisoken { get; }
        public ExpressionSyntax Expression { get; }
        public SyntaxToken CloseParenthesisToken { get; }

        public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return OpenParenthesisoken;
            yield return Expression;
            yield return CloseParenthesisToken;
        }
    }
}