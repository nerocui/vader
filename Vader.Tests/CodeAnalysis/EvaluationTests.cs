using System;
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
            AssertValue(text, expectedValue);
        }

        [Fact]
        public void Evaluator_VariableDeclaration_Reports_Redeclaration()
        {
            var text = @"
            {
                var x = 10
                var y = 100
                {
                    var x = 10
                }
                var [x] = 5
            }
            ";
            var diagnostics = @"
                Error: Variable 'x' has already been declared.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_Name_Reports_Undefined()
        {
            var text = @"[x] * 10";
            var diagnostics = @"
                Error: Variable 'x' doesn't exist.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_Assigned_Reports_CannotAssign()
        {
            var text = @"
            {
                let x = 10
                x [=] 0
            }";
            var diagnostics = @"
                Error: Variable 'x' is read-only and cannot be assigned to.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_Assigned_Reports_CannotConvert()
        {
            var text = @"
            {
                var x = 10
                x = [true]
            }";
            var diagnostics = @"
                Error: Cannot convert type 'System.Boolean' to 'System.Int32'.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_Unary_Reports_Undefined()
        {
            var text = @"
            {
                [+]true
            }";
            var diagnostics = @"
                Error: Unary operator '+' is not defined for type 'System.Boolean'.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_Binary_Reports_Undefined()
        {
            var text = @"
            {
                true[+]false
            }";
            var diagnostics = @"
                Error: Binary operator '+' is not defined for type 'System.Boolean' and 'System.Boolean'.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        private static void AssertValue(string text, object expectedValue)
        {
            var syntaxTree = SyntaxTree.Parse(text);
            var compilation = new Compilation(syntaxTree);
            var variables = new Dictionary<VariableSymbol, object>();
            var result = compilation.Evaluate(variables);

            Assert.Empty(result.Diagnostics);
            Assert.Equal(expectedValue, result.Value);
        }

        private void AssertDiagnostics(string text, string diagnosticsText)
        {
            var annotatedText = AnnotatedText.Parse(text);
            var syntaxTree = SyntaxTree.Parse(annotatedText.Text);
            var compilation = new Compilation(syntaxTree);
            var result = compilation.Evaluate(new Dictionary<VariableSymbol, object>());

            var expectedDiagnostics = AnnotatedText.UnindentLines(diagnosticsText);

            if (annotatedText.Spans.Length != expectedDiagnostics.Length)
                throw new Exception("Error: Must mark as many spans as there are expected diagnostics");
            
            Assert.Equal(expectedDiagnostics.Length, result.Diagnostics.Length);

            for (var i = 0; i < expectedDiagnostics.Length; i++)
            {
                var expectedMessage = expectedDiagnostics[i];
                var actualMessage = result.Diagnostics[i].Message;
                Assert.Equal(expectedMessage, actualMessage);
                var expectedSpan = annotatedText.Spans[i];
                var actualSpan = result.Diagnostics[i].Span;
                Assert.Equal(expectedSpan, actualSpan);
            }
        }
    }
}