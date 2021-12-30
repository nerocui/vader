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
        [InlineData("~1", -2)]
        [InlineData("~2", -3)]
        [InlineData("~3", -4)]
        [InlineData("~4", -5)]
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
        [InlineData("33 > 22", true)]
        [InlineData("11 > 22", false)]
        [InlineData("22 > 22", false)]
        [InlineData("33 >= 22", true)]
        [InlineData("33 >= 34", false)]
        [InlineData("33 >= 33", true)]
        [InlineData("33 < 22", false)]
        [InlineData("11 < 22", true)]
        [InlineData("22 < 22", false)]
        [InlineData("33 <= 22", false)]
        [InlineData("33 <= 34", true)]
        [InlineData("33 <= 33", true)]
        [InlineData("1 | 2", 3)]
        [InlineData("1 | 0", 1)]
        [InlineData("1 & 2", 0)]
        [InlineData("1 & 0", 0)]
        [InlineData("1 ^ 0", 1)]
        [InlineData("1 ^ 1", 0)]
        [InlineData("0 ^ 1", 1)]
        [InlineData("1 ^ 3", 2)]
        [InlineData("true && false", false)]
        [InlineData("true & false", false)]
        [InlineData("false && false", false)]
        [InlineData("false & false", false)]
        [InlineData("true && true", true)]
        [InlineData("true & true", true)]
        [InlineData("false || false", false)]
        [InlineData("false | false", false)]
        [InlineData("true || false", true)]
        [InlineData("true | false", true)]
        [InlineData("true && 2 == 3", false)]
        [InlineData("true & 2 == 3", false)]
        [InlineData("true == false ", false)]
        [InlineData("true == true ", true)]
        [InlineData("false == false ", true)]
        [InlineData("true != false ", true)]
        [InlineData("true", true)]
        [InlineData("false", false)]
        [InlineData("!true", false)]
        [InlineData("!false", true)]
        [InlineData("{ var a = 10 }", 10)]
        [InlineData("{ var a = 0 (a = 10) * 8 }", 80)]
        [InlineData("{ var a = 0 if a == 0 a = 10 a }", 10)]
        [InlineData("{ var a = 0 if a == 4 a = 10 a }", 0)]
        [InlineData("{ var a = 0 if a == 0 a = 10 else a = 5 a }", 10)]
        [InlineData("{ var a = 0 if a == 4 a = 10 else a = 5 a }", 5)]
        [InlineData("{ var i = 10 var result = 0 while i > 0 {result = result + i i = i - 1} result}", 55)]
        [InlineData("{ var result = 0 for i = 1 to 10 {result = result + i } result}", 55)]
        [InlineData("{ var a = 10 for i = 1 to (a = a - 1) { } a }", 9)]
        public void SyntaxFact_GetText_RoundTrips(string text, object expectedValue)
        {
            AssertValue(text, expectedValue);
        }

        [Fact]
        public void Evaluator_BlockStatement_NoInfiniteLoop()
        {
            var text = @"
            {
            [)][]
            ";
            var diagnostics = @"
                Error: Unexpected token <CloseParenthesisToken>, expected <IdentifierToken>.
                Error: Unexpected token <EndOfFileToken>, expected <CloseBraceToken>.
            ";

            AssertDiagnostics(text, diagnostics);
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
        public void Evaluator_NameExpression_Reports_Undefined()
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
        public void Evaluator_IfStatement_Reports_CannotConvert()
        {
            var text = @"
            {
                var x = 0
                if [10]
                    x = 10
            }";
            var diagnostics = @"
                Error: Cannot convert type 'System.Int32' to 'System.Boolean'.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_WhileStatement_Reports_CannotConvert()
        {
            var text = @"
            {
                var x = 0
                while [10]
                    x = 10
            }";
            var diagnostics = @"
                Error: Cannot convert type 'System.Int32' to 'System.Boolean'.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_ForStatement_Reports_CannotConvert_LowerBound()
        {
            var text = @"
            {
                var x = 0
                for i = [false] to 10
                    x = x + i
            }";
            var diagnostics = @"
                Error: Cannot convert type 'System.Boolean' to 'System.Int32'.
            ";

            AssertDiagnostics(text, diagnostics);
        }

        [Fact]
        public void Evaluator_ForStatement_Reports_CannotConvert_UpperBound()
        {
            var text = @"
            {
                var x = 0
                for i = 1 to [false]
                    x = x + i
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
                throw new Exception($"Error: Must mark as many spans as there are expected diagnostics, got {annotatedText.Spans.Length}, expected {expectedDiagnostics.Length}");
            
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