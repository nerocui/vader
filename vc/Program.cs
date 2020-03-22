﻿using System;
using System.Collections.Generic;
using System.Linq;
using Vader.CodeAnalysis;

namespace Vader
{
    class Program
    {
        static void Main(string[] args)
        {
            var showTree = false;
            while (true)
            {
                Console.WriteLine(">");
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

                var color = Console.ForegroundColor;
                if (showTree)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    PrettyPrint(syntaxTree.Root);
                    Console.ForegroundColor = color;
                }

                if (!syntaxTree.Diagnostics.Any())
                {
                    var e = new Evaluator(syntaxTree.Root);
                    var result = e.Evaluate();
                    Console.WriteLine($"Result is: {result}");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    foreach (var diagnostic in syntaxTree.Diagnostics)
                    {
                        Console.WriteLine(diagnostic);
                    }
                }
                Console.ForegroundColor = color;
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
            indent += isLast ? "    " : "│   ";
            foreach (var child in node.GetChildren())
                PrettyPrint(child, indent, child == node.GetChildren().LastOrDefault());
        }
    }
}
