using UnityEngine;

namespace DialoguePlus
{
    public class ReturnNode : DialogueNode
    {
        public override bool IsDisplayable => false;
        public override string Info => "return";



        public override void Execute(DialogueEngine engine)
        {
            engine.AddToHistory();
            engine.Return();
        }
        public override void Undo(DialogueEngine engine)
        {

        }
    }
}
