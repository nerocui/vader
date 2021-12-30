using Vader.CodeAnalysis.Binding;

namespace Vader
{
    internal static class Program
    {
        static void Main()
        {
            var repl = new VaderRepl();
            repl.Run();
        }
    }
}
