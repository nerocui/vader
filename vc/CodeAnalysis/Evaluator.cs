using System;
using Vader.CodeAnalysis.Syntax;

namespace Vader.CodeAnalysis
{
    public class Evaluator
    {
        private readonly ExpressionSyntax _root;

        public Evaluator(ExpressionSyntax root)
        {
            _root = root;
        }

        public int Evaluate()
        {
            return EvaluateExpression(_root);
        }

        private int EvaluateExpression(ExpressionSyntax node)
        {
            if (node is LiteralExpressionSyntax n)
                return (int) n.LiteralToken.Value;
            if (node is UnaryExpressionSyntax u)
            {
                var operand = EvaluateExpression(u.Operand);
                if (u.OperatirToken.Kind == SyntaxKind.PlusToken)
                    return operand;
                else if (u.OperatirToken.Kind == SyntaxKind.MinusToken)
                    return -operand;
                else
                    throw new Exception($"Error: Unexpected unary operator {u.OperatirToken.Kind}");
            }
            if (node is BinaryExpressionSyntax b)
            {
                var left = EvaluateExpression(b.Left);
                var right = EvaluateExpression(b.Right);

                if (b.OperatirToken.Kind == SyntaxKind.PlusToken)
                    return left + right;
                else if (b.OperatirToken.Kind == SyntaxKind.MinusToken)
                    return left - right;
                else if (b.OperatirToken.Kind == SyntaxKind.StarToken)
                    return left * right;
                else if (b.OperatirToken.Kind == SyntaxKind.SlashToken)
                    return left / right;
                else
                    throw new Exception($"Error: Unexpected binary operator {b.OperatirToken.Kind}");
            }

            if (node is ParenthesizedExpressionSyntax p)
                return EvaluateExpression(p.Expression);
            throw new Exception($"Error: Unexpected node {node.Kind}");
        }
    }
}