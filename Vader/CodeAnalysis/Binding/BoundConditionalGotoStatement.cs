namespace Vader.CodeAnalysis.Binding
{
    internal sealed class BoundConditionalGotoStatement : BoundStatement
    {
        public BoundConditionalGotoStatement(LabelSymbel label, BoundExpression condition, bool jumpIfFalse = false)
        {
            JumpIfFalse = jumpIfFalse;
            Condition = condition;
            Label = label;
        }

        public LabelSymbel Label { get; }
        public BoundExpression Condition { get; }
        public bool JumpIfFalse { get; }

        public override BoundNodeKind Kind => BoundNodeKind.ConditionalGotoStatement;
    }
}
