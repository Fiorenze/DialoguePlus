using UnityEngine;

namespace DialoguePlus
{
    public class JumpActionNode : DialogueNode
    {
        public override bool IsDisplayable => false;
        public override string Info => "jump " + SceneLabel;


        public string SceneLabel { get; set; }


        public override void Execute(DialogueEngine engine)
        {
            engine.AddToHistory();
            engine.JumpToScene(SceneLabel);
        }
        public override void Undo(DialogueEngine engine)
        {
            
        }
    }
}
