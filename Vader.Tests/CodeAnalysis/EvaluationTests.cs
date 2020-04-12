using System.Collections.Generic;
using Vader.CodeAnalysis;
using Vader.CodeAnalysis.Syntax;
using Xunit;

namespace Vader.Tests.CodeAnalysis
{
    public class EvaluationTests
    {
        [Theory]
        [InlineData("1", 1)]
        [InlineData("+1", 1)]
        [InlineData("-1", -1)]
        [InlineData("11 + 33", 44)]
        [InlineData("65 - 21", 44)]
        [InlineData("-6 + 2", -4)]
        [InlineData("-6 * 2", -12)]
        [InlineData("11 * 12", 132)]
        [InlineData("32 / 4", 8)]
        [InlineData("(22)", 22)]
        [InlineData("(1 + 2) * 2", 6)]
        [InlineData("(1 * 2) /2", 1)]
        [InlineData("1 == 2", false)]
        [InlineData("33 != 22", true)]
        [InlineData("true && false", false)]
        [InlineData("true || false", true)]
        [InlineData("true && 2 == 3", false)]
        [InlineData("true == false ", false)]
        [InlineData("true == true ", true)]
        [InlineData("false == false ", true)]
        [InlineData("true != false ", true)]
        [InlineData("true", true)]
        [InlineData("false", false)]
        [InlineData("!true", false)]
        [InlineData("!false", true)]
        [InlineData("{ var a = 0 (a = 10) * 8 }", 80)]
        public void SyntaxFact_GetText_RoundTrips(string text, object expectedValue)
        {
            var syntaxTree = SyntaxTree.Parse(text);
            var compilation = new Compilation(syntaxTree);
            var variables = new Dictionary<VariableSymbol, object>();
            var result = compilation.Evaluate(variables);

            Assert.Empty(result.Diagnostics);
            Assert.Equal(expectedValue, result.Value);
        }
    }
}