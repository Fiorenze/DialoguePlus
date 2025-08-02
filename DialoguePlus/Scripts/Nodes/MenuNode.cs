using System.Collections.Generic;

namespace DialoguePlus
{
    public class MenuNode : DialogueNode
    {
        public override bool IsDisplayable => true;
        public override string Info => $"Menu with {OptionCount} options";


        public int OptionCount => Options.Count;
        public List<MenuOption> Options { get; private set; } = new();


        private DialogueEngine engine;

        public override void Execute(DialogueEngine engine)
        {
            this.engine = engine;
            engine.DisplaySentence(this);
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

        public void SetOptions(List<MenuOption> options)
        {
            Options = options;
        }

    }
}
