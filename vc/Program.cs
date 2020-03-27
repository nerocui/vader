using System;
using System.Collections.Generic;
using System.Linq;
using Vader.CodeAnalysis;
using Vader.CodeAnalysis.Binding;
using Vader.CodeAnalysis.Syntax;

namespace Vader
{
    internal static class Program
    {
        static void Main()
        {
            var showTree = false;
            var variables = new Dictionary<VariableSymbol,object>();
            while (true)
            {
                Console.Write(">");
                var line = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    return;

                if (line == "#showTree")
                {
                    showTree = !showTree;
                    Console.WriteLine(showTree ? "Showing parsed syntax tree" : "Not showing parsed syntax tree");
                    continue;
                }

                if (line == "#clear")
                {
                    Console.Clear();
                    continue;
                }

                var syntaxTree = SyntaxTree.Parse(line);
                var compilation = new Compilation(syntaxTree);
                var result = compilation.Evaluate(variables);
                if (showTree)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    PrettyPrint(syntaxTree.Root);
                    Console.ResetColor();
                }

                var diagnostics = result.Diagnostics;
                if (!diagnostics.Any())
                {
                    Console.WriteLine($"Result is: {result.Value}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    foreach (var diagnostic in diagnostics)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(diagnostic);
                        Console.ResetColor();

                        var prefix = line.Substring(0, diagnostic.Span.Start);
                        var error = line.Substring(diagnostic.Span.Start, diagnostic.Span.Length);
                        var suffix = line.Substring(diagnostic.Span.End);
                        
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
            }
        }

        static void PrettyPrint(SyntaxNode node, string indent = "", bool isLast = true)
        {
            // ├─
            // └─
            // │
            var marker = isLast ? "└─" : "├─";
            Console.Write(indent);
            Console.Write(marker);
            Console.Write(node.Kind);

            if (node is SyntaxToken t && t.Value != null)
            {
                Console.Write(" ");
                Console.Write(t.Value);
            }
            Console.WriteLine();
            indent += isLast ? "   " : "│  ";
            foreach (var child in node.GetChildren())
                PrettyPrint(child, indent, child == node.GetChildren().LastOrDefault());
        }
    }
}
