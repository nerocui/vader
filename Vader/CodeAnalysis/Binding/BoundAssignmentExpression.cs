using System;

namespace Vader.CodeAnalysis.Binding
{
    internal sealed class BoundAssignmentExpression : BoundExpression
    {
        public VariableSymbol Variable { get; }
        public BoundExpression Expression { get; }
        public override Type Type => Variable.Type;

        public override BoundNodeKind Kind => BoundNodeKind.AssignmentExpression;
        public BoundAssignmentExpression(VariableSymbol variable, BoundExpression expression)
        {
            Variable = variable;
            Expression = expression;
        }
    }
}