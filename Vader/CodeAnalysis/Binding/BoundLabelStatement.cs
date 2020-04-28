namespace Vader.CodeAnalysis.Binding
{
    internal sealed class BoundLabelStatement : BoundStatement
    {
        public BoundLabelStatement(LabelSymbel label)
        {
            Label = label;
        }

        public LabelSymbel Label { get; }

        public override BoundNodeKind Kind => BoundNodeKind.LabelStatement;
    }
}
