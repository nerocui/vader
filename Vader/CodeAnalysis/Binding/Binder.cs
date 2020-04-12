using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Vader.CodeAnalysis.Syntax;

namespace Vader.CodeAnalysis.Binding
{
    internal sealed class Binder
    {
        private readonly DiagnosticBag _diagnostics = new DiagnosticBag();
        private BoundScope _scope;
        public DiagnosticBag Diagnostics => _diagnostics;
        public Binder(BoundScope parent)
        {
            _scope = new BoundScope(parent);
        }
        public static BoundGlobalScope BindGlobalScope(BoundGlobalScope previous, CompilationUnitSyntax syntax)
        {
            var parentScope = CreateParentScopes(previous);
            var binder = new Binder(parentScope);
            var expression = binder.BindExpression(syntax.Expression);
            var variables = binder._scope.GetDeclaredVariables();
            var diagnostics = binder.Diagnostics.ToImmutableArray();

            //Since in CLI, the variables from different lines are chaining (nested)
            //we need to chain the diagnostics too.
            //otherwise, statements like a=b will give error, but
            //when we reference a again, it will not have the same error
            if (previous != null)
                diagnostics = diagnostics.InsertRange(0, previous.Diagnostics);

            return new BoundGlobalScope(previous, diagnostics, variables, expression);
        }

        private static BoundScope CreateParentScopes(BoundGlobalScope previous)
        {
            // submission 3 => submission 2 => submission 1
            var stack = new Stack<BoundGlobalScope>();
            while (previous != null)
            {
                stack.Push(previous);
                previous = previous.Previous;
            }
            // submission 1 => submission 2 => submission 3
            BoundScope parent = null;
            while (stack.Count > 0)
            {
                previous = stack.Pop();
                var scope = new BoundScope(parent);
                foreach (var v in previous.Variables)
                    scope.TryDeclare(v);
                parent = scope;
            }

            return parent;
        }
        public BoundExpression BindExpression(ExpressionSyntax syntax)
        {
            switch (syntax.Kind)
            {
                case SyntaxKind.LiteralExpression:
                    return BindLiteralExpression((LiteralExpressionSyntax)syntax);
                case SyntaxKind.UnaryExpression:
                    return BindUnaryExpression((UnaryExpressionSyntax)syntax);
                case SyntaxKind.BinaryExpression:
                    return BindBinaryExpression((BinaryExpressionSyntax)syntax);
                case SyntaxKind.ParenthesizedExpression:
                    return BindParenthesizedExpression((ParenthesizedExpressionSyntax)syntax);
                case SyntaxKind.NameExpression:
                    return BindNameExpression((NameExpressionSyntax)syntax);
                case SyntaxKind.AssignmentExpression:
                    return BindAssignmentExpression((AssignmentExpressionSyntax)syntax);
                default:
                    throw new Exception($"Error: Unexpected syntax {syntax.Kind}");
            }
        }

        private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
        {
            var name = syntax.IdentitifierToken.Text;
            if (!_scope.TryLookup(name, out var variable))
            {
                _diagnostics.ReportUndefinedName(syntax.IdentitifierToken.Span, name);
                return new BoundLiteralExpression(0);
            }
            return new BoundVariableExpression(variable);
        }

        private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
        {
            var name = syntax.IdentifierToken.Text;
            var boundExpression = BindExpression(syntax.Expression);
            if (!_scope.TryLookup(name, out var variable))
            {
                variable = new VariableSymbol(name, boundExpression.Type);
                _scope.TryDeclare(variable);
            }

            if (boundExpression.Type != variable.Type)
            {
                _diagnostics.ReportCannotConvert(syntax.Expression.Span, boundExpression.Type, variable.Type);
                return boundExpression;
            }
            return new BoundAssignmentExpression(variable, boundExpression);
        }

        private BoundExpression BindParenthesizedExpression(ParenthesizedExpressionSyntax syntax)
        {
            return BindExpression(syntax.Expression);
        }

        private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
        {
            var boundOperand = BindExpression(syntax.Operand);
            var boundOperator = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, boundOperand.Type);
            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundOperand.Type);
                return boundOperand;
            }
            return new BoundUnaryExpression(boundOperator, boundOperand);
        }

        private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
        {
            var boundLeft = BindExpression(syntax.Left);
            var boundRight = BindExpression(syntax.Right);
            var boundOperator = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);
            if (boundOperator == null)
            {
                _diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text, boundLeft.Type, boundRight.Type);
                return boundLeft;
            }
            return new BoundBinaryExpression(boundLeft, boundOperator, boundRight);
        }

        private BoundExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
        {
            var value = syntax.Value ?? 0;
            return new BoundLiteralExpression(value);
        }
    }
}