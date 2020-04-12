using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vader.CodeAnalysis;
using Vader.CodeAnalysis.Binding;
using Vader.CodeAnalysis.Syntax;
using Vader.CodeAnalysis.Text;

namespace Vader
{
    internal static class Program
    {
        static void Main()
        {
            var showTree = false;
            var variables = new Dictionary<VariableSymbol,object>();
            var textBuilder = new StringBuilder();
            Compilation previous = null;

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                if (textBuilder.Length == 0)
                    Console.Write(">");
                else
                    Console.Write("| ");
                Console.ResetColor();

                var input = Console.ReadLine();
                var isBlank = string.IsNullOrWhiteSpace(input);

                if (textBuilder.Length == 0)
                {
                    if (isBlank)
                    {
                        break;
                    }
                    else if (input == "#showTree")
                    {
                        showTree = !showTree;
                        Console.WriteLine(showTree ? "Showing parsed syntax tree" : "Not showing parsed syntax tree");
                        continue;
                    }
                    else if (input == "#clear")
                    {
                        Console.Clear();
                        continue;
                    }
                    else if (input == "#reset")
                    {
                        previous = null;
                        continue;
                    }
                }

                textBuilder.AppendLine(input);
                var text = textBuilder.ToString();

                var syntaxTree = SyntaxTree.Parse(text);

                if (!isBlank && syntaxTree.Diagnostics.Any())
                    continue;

                var compilation = previous == null 
                                    ? new Compilation(syntaxTree)
                                    : previous.ContinueWith(syntaxTree);
                var result = compilation.Evaluate(variables);
                if (showTree)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    syntaxTree.Root.WriteTo(Console.Out);
                    Console.ResetColor();
                }

                if (!result.Diagnostics.Any())
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine($"Result is: {result.Value}");
                    Console.ResetColor();

                    previous = compilation;
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
                Console.ResetColor();
                textBuilder.Clear();
            }
        }
    }
}
