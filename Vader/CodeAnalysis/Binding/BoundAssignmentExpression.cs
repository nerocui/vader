using System;

namespace Vader.CodeAnalysis.Binding
{
    internal sealed class BoundAssignmentExpression : BoundExpression
    {
        public string Name { get; }
        public BoundExpression Expression { get; }
        public override Type Type => Expression.Type;

        public override BoundNodeKind Kind => BoundNodeKind.AssignmentExpression;
        public BoundAssignmentExpression(string name, BoundExpression expression)
        {
            this.Expression = expression;
            this.Name = name;

        }
    }
}