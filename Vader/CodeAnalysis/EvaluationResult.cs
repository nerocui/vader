using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Vader.CodeAnalysis
{
    public sealed class EvaluationResult
    {
        public ImmutableArray<Diagnostic> Diagnostics { get; }
        public object Value { get; }
        public EvaluationResult(ImmutableArray<Diagnostic> diagnostics, object value)
        {
            Value = value;
            Diagnostics = diagnostics;
        }
    }
}