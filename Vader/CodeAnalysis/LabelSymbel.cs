namespace Vader.CodeAnalysis
{
    internal sealed class LabelSymbel
    {
        internal LabelSymbel(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public override string ToString() => Name;
    }
}