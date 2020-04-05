using System.Collections.Generic;

namespace Vader.CodeAnalysis.Syntax
{
    public sealed class NameExpressionSyntax : ExpressionSyntax
    {
        public NameExpressionSyntax(SyntaxToken identitifierToken)
        {
            IdentitifierToken = identitifierToken;
        }

        public SyntaxToken IdentitifierToken { get; }

        public override SyntaxKind Kind => SyntaxKind.NameExpression;
    }
}