using System.Collections.Immutable;

namespace Vader.CodeAnalysis.Binding
{
    internal sealed class BoundGlobalScope
    {
        public BoundGlobalScope Previous { get; }
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public ImmutableArray<VariableSymbol> Variables { get; }
        public BoundExpression Expression { get; }
        public BoundGlobalScope(BoundGlobalScope previous, ImmutableArray<Diagnostic> diagnostics, ImmutableArray<VariableSymbol> variables, BoundExpression expression)
        {
            this.Expression = expression;
            this.Variables = variables;
            this.Diagnostics = diagnostics;
            this.Previous = previous;

        }
    }
}