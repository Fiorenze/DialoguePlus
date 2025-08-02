using System.Collections.Generic;

namespace DialoguePlus
{
    public class IfBlockNode : DialogueNode
    {
        public override bool IsDisplayable => true;
        public override string Info => $"If with {BranchCount} branches";


        public int BranchCount => DialogueNodeBranches.Count;
        public List<DialogueNodeBranch> DialogueNodeBranches { get; private set; } = new();

        public override void Execute(DialogueEngine engine)
        {
            DialogueNodeBranch branch = SelectTreeBranch();
            engine.EnterBranch(branch);
        }

        public override void Undo(DialogueEngine engine)
        {
            engine.ExitBranch();
        }

        public void SetBranches(List<DialogueNodeBranch> branches)
        {
            DialogueNodeBranches = branches;
        }

        private DialogueNodeBranch SelectTreeBranch()
        {
            foreach (DialogueNodeBranch branch in DialogueNodeBranches)
            {
                if (ConditionEvaluator.CheckCondition(branch.Condition))
                {
                    return branch;
                }
            }
            return null;
        }
    }
}
