using System;
using System.Collections.Generic;
using System.Linq;
using Vader.CodeAnalysis;
using Vader.CodeAnalysis.Syntax;
using Vader.CodeAnalysis.Text;

namespace Vader
{
    internal sealed class VaderRepl : Repl
    {
        private Compilation _previous;
        private bool _showTree;
        private bool _showProgram;
        private readonly Dictionary<VariableSymbol, object> _variables = new Dictionary<VariableSymbol, object>();
        
        protected override void RenderLine(string line)
        {
            var tokens = SyntaxTree.ParseTokens(line);
            foreach (var token in tokens)
            {
                var isNumber = token.Kind == SyntaxKind.NumberToken;
                var isKeyword = token.Kind.ToString().EndsWith("Keyword");
                if (isKeyword)
                    Console.ForegroundColor = ConsoleColor.Blue;
                else if (!isNumber)
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                
                Console.Write(token.Text);
                Console.ResetColor();
            }
        }
        
        protected override void EvaluateMetaCommand(string input)
        {
            switch (input)
            {
                case "#showTree":
                    _showTree = !_showTree;
                    Console.WriteLine(_showTree ? "Showing parsed syntax tree" : "Not showing parsed syntax tree");
                    break;
                case "#showProgram":
                    _showProgram = !_showProgram;
                    Console.WriteLine(_showProgram ? "Showing parsed bound tree" : "Not showing parsed bound tree");
                    break;
                case "#clear":
                    Console.Clear();
                    break;
                case "#reset":
                    _previous = null;
                    break;
                default:
                    base.EvaluateMetaCommand(input);
                    break;
            }
        }

        protected override void EvaluateSubmission(string text)
        {
            var syntaxTree = SyntaxTree.Parse(text);

            var compilation = _previous == null
                                ? new Compilation(syntaxTree)
                                : _previous.ContinueWith(syntaxTree);

            if (_showTree)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                syntaxTree.Root.WriteTo(Console.Out);
                Console.ResetColor();
            }
            if (_showProgram)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                compilation.EmitTree(Console.Out);
                Console.ResetColor();
            }

            var result = compilation.Evaluate(_variables);
            if (!result.Diagnostics.Any())
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"Result is: {result.Value}");
                Console.ResetColor();

                _previous = compilation;
            }
            else
            {
                foreach (var diagnostic in result.Diagnostics)
                {
                    var lineIndex = syntaxTree.Text.GetLineIndex(diagnostic.Span.Start);
                    var lineNumber = lineIndex + 1;
                    var line = syntaxTree.Text.Lines[lineIndex];
                    var character = diagnostic.Span.Start - line.Start + 1;

                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write($"(line {lineNumber}, column {character}): ");
                    Console.WriteLine(diagnostic);
                    Console.ResetColor();

                    var prefixSpan = TextSpan.FromBounds(line.Start, diagnostic.Span.Start);
                    var suffixSpan = TextSpan.FromBounds(diagnostic.Span.End, line.End);

                    var prefix = syntaxTree.Text.ToString(prefixSpan);
                    var error = syntaxTree.Text.ToString(diagnostic.Span);
                    var suffix = syntaxTree.Text.ToString(suffixSpan);

                    Console.Write("    ");
                    Console.Write(prefix);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Write(error);
                    Console.ResetColor();

                    Console.Write(suffix);
                    Console.WriteLine();
                }
            }
        }

        protected override bool IsCompleteSubmission(string text)
        {
            if (string.IsNullOrEmpty(text))
                return true;
            
            var syntaxTree = SyntaxTree.Parse(text);

            if (syntaxTree.Diagnostics.Any())
                return false;
            
            return true;
        }
    }
}
