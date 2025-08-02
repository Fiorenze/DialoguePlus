using UnityEngine;

namespace DialoguePlus
{
    public class CallActionNode : DialogueNode
    {
        public override bool IsDisplayable => false;
        public override string Info => "call " + SceneLabel;


        public string SceneLabel { get; set; }


        public override void Execute(DialogueEngine engine)
        {
            engine.CreateReturnPoint();
            engine.AddToHistory();
            engine.JumpToScene(SceneLabel);
        }
        public override void Undo(DialogueEngine engine)
        {

        }
    }
}
