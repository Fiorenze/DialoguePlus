using System.Collections.Generic;
using UnityEngine;

namespace DialoguePlus
{
    public class IfBlockNode : DialogueNode
    {
        public override bool IsDisplayable => true;

        public override void Execute(DialogueEngine engine)
        {
            this.engine = engine;
            DialogueNodeBranch branch = SelectTreeBranch();

            engine.EnterBranch(branch);
        }

        public override void Undo(DialogueEngine engine)
        {
            engine.ExitBranch();
        }

        public List<DialogueNodeBranch> DialogueNodeBranches = new();

        // Logic
        DialogueEngine engine;

        private DialogueNodeBranch SelectTreeBranch()
        {
            foreach (DialogueNodeBranch branch in DialogueNodeBranches)
            {
                if (engine.CheckCondition(branch.Condition))
                {
                    return branch;
                }
            }
            return null;
        }
    }

    public class DialogueNodeBranch
    {
        public string Condition { get; set; }
        public List<DialogueNode> BranchNodes = new();
    }
}
