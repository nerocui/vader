using System.Collections.Generic;

namespace Vader.CodeAnalysis
{
    internal sealed class Parser
    {
        private readonly SyntaxToken[] _tokens;
        private int _position;
        private List<string> _diagnostics = new List<string>();
        public Parser(string text)
        {
            var tokens = new List<SyntaxToken>();
            var lexer = new Lexer(text);
            SyntaxToken token;
            do
            {
                token = lexer.Lex();
                if (token.Kind != SyntaxKind.BadToken && token.Kind != SyntaxKind.WhitespaceToken)
                {
                    tokens.Add(token);
                }
            } while (token.Kind != SyntaxKind.EndOfFileToken);
            _tokens = tokens.ToArray();
            _diagnostics.AddRange(lexer.Diagnostics);
        }
        public IEnumerable<string> Diagnostics => _diagnostics;

        private SyntaxToken Peek(int offset)
        {
            var index = _position + offset;
            if (index >= _tokens.Length)
                return _tokens[_tokens.Length - 1];
            return _tokens[index];
        }
        private SyntaxToken Current => Peek(0);
        private SyntaxToken NextToken()
        {
            var current = Current;
            _position++;
            return current;
        }

        private SyntaxToken MatchToken(SyntaxKind kind)
        {
            if (Current.Kind == kind)
                return NextToken();
            _diagnostics.Add($"Error: Unexpected token <{Current.Kind}>, expected <{kind}>");
            return new SyntaxToken(kind, Current.Position, null, null);
        }

        public SyntaxTree Parse()
        {
            var expression = ParseExpression();
            var endOfFile = MatchToken(SyntaxKind.EndOfFileToken);
            return new SyntaxTree(_diagnostics, expression, endOfFile);
        }

        private ExpressionSyntax ParseExpression(int parentPrecedence = 0)
        {
            var left = ParsePrimaryExpression();
            while (true)
            {
                var precedfence = Current.Kind.GetBinaryOperatorPrecedence();
                if (precedfence == 0 || precedfence <= parentPrecedence)
                    break;
                var operatirToken = NextToken();
                var right = ParseExpression(precedfence);
                left = new BinaryExpressionSyntax(left, operatirToken, right);
            }
            return left;
        }

        private ExpressionSyntax ParsePrimaryExpression()
        {
            if (Current.Kind == SyntaxKind.OpenParenthesisoken)
            {
                var left = NextToken();
                var expression = ParseExpression();
                var right = MatchToken(SyntaxKind.CloseParenthesisToken);
                return new ParenthesizedExpressionSyntax(left, expression, right);
            }
            var numberToken = MatchToken(SyntaxKind.NumberToken);
            return new LiteralExpressionSyntax(numberToken);
        }
    }
}