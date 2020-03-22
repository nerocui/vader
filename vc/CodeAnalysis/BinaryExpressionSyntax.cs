using System.Collections.Generic;

namespace Vader.CodeAnalysis
{
    sealed class BinaryExpressionSyntax : ExpressionSyntax
    {
        public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operatirToken, ExpressionSyntax right)
        {
            Left = left;
            OperatirToken = operatirToken;
            Right = right;
        }

        public override SyntaxKind Kind => SyntaxKind.BinaryExpression;

        public ExpressionSyntax Left { get; }
        public SyntaxToken OperatirToken { get; }
        public ExpressionSyntax Right { get; }

        public override IEnumerable<SyntaxNode> GetChildren()
        {
            yield return Left;
            yield return OperatirToken;
            yield return Right;
        }
    }
}