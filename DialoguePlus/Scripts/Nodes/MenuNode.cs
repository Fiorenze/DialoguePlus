using System.Collections.Generic;
using UnityEngine;

namespace DialoguePlus
{
    public class MenuNode : DialogueNode
    {
        public override bool IsDisplayable => true;

        private DialogueEngine engine;

        public override void Execute(DialogueEngine engine)
        {
            this.engine = engine;
            DialogueManager.Instance.dialogueUI.DisplaySentence(this);
        }

        public override void Undo(DialogueEngine engine)
        {
            engine.ExitBranch();
        }

        public void ChooseOption(int optionNum)
        {
            DialogueNodeBranch chosenBranch = Options[optionNum].Branch;

            engine.EnterBranch(chosenBranch);
        }

        public List<DialogueOption> Options = new();
    }

    public class DialogueOption
    {
        public string Text { get; set; }
        public List<string> SoftConditions = new();
        public List<string> HardConditions = new();
        public DialogueNodeBranch Branch { get; set; } = new();
    }
}
