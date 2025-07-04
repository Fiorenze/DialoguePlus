using UnityEngine;

namespace DialoguePlus
{
    public class CommandActionNode : DialogueNode
    {
        public string Command { get; set; }
        public string Text { get; set; }

        public override bool IsDisplayable => false;

        private string OldSceneLabel;

        public override void Execute(DialogueEngine engine)
        {
            if (Command == "jump")
            {
                OldSceneLabel = engine.ActiveScene.SceneLabel;
                engine.JumpToScene(Text);
            }
            else
            {
                Debug.Log("Only 'jump' action is supported for now!");
                return;
            }
        }

        public override void Undo(DialogueEngine engine)
        {
            if (Command == "jump")
            {
                engine.JumpToScene(OldSceneLabel);
            }
        }



    }
}
