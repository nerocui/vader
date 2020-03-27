using System;
using System.Collections.Generic;
using System.Linq;
using Vader.CodeAnalysis.Binding;
using Vader.CodeAnalysis.Syntax;

namespace Vader.CodeAnalysis
{
    public sealed class Compilation
    {
        public SyntaxTree Syntax { get; }
        public Compilation(SyntaxTree syntax)
        {
            Syntax = syntax;
        }

        public EvaluationResult Evaluate(Dictionary<string, object> variables)
        {
            var binder = new Binder(variables);
            var boundExpression = binder.BindExpression(Syntax.Root);

            var diagnostics = Syntax.Diagnostics.Concat(binder.Diagnostics).ToArray();
            if (diagnostics.Any())
                return new EvaluationResult(diagnostics, null);
            var evaluator = new Evaluator(boundExpression, variables);
            var value = evaluator.Evaluate();
            return new EvaluationResult(Array.Empty<Diagnostic>(), value);
        }
    }
}