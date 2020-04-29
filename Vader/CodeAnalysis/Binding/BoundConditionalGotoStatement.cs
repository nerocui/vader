namespace Vader.CodeAnalysis.Binding
{
    internal sealed class BoundConditionalGotoStatement : BoundStatement
    {
        public BoundConditionalGotoStatement(LabelSymbel label, BoundExpression condition, bool jumpIfTrue = true)
        {
            JumpIfTrue = jumpIfTrue;
            Condition = condition;
            Label = label;
        }

        public LabelSymbel Label { get; }
        public BoundExpression Condition { get; }
        public bool JumpIfTrue { get; }

        public override BoundNodeKind Kind => BoundNodeKind.ConditionalGotoStatement;
    }
}
