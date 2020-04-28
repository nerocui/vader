namespace Vader.CodeAnalysis.Binding
{
    internal sealed class BoundGoToStatement : BoundStatement
    {
        public BoundGoToStatement(LabelSymbel label)
        {
            Label = label;
        }

        public override BoundNodeKind Kind => BoundNodeKind.GoToStatement;

        public LabelSymbel Label { get; }
    }
}
