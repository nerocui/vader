using System;

namespace Vader.CodeAnalysis.Binding
{
    internal sealed class BoundVariableExpression : BoundExpression
    {
        public string Name { get; }
        public override Type Type { get; }

        public override BoundNodeKind Kind => BoundNodeKind.VariableExpression;

        public BoundVariableExpression(string name, Type type)
        {
            this.Type = type;
            this.Name = name;

        }
    }
}