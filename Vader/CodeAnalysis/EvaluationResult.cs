using System.Collections.Generic;
using System.Linq;

namespace Vader.CodeAnalysis
{
    public sealed class EvaluationResult
    {
        public IReadOnlyList<Diagnostic> Diagnostics { get; }
        public object Value { get; }
        public EvaluationResult(IEnumerable<Diagnostic> diagnostics, object value)
        {
            Value = value;
            Diagnostics = diagnostics.ToArray();
        }
    }
}