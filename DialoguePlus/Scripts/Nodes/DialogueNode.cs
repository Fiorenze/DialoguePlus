using UnityEngine;

namespace DialoguePlus
{
    public abstract class DialogueNode
    {
        public abstract void Execute(DialogueEngine engine);
        public abstract void Undo(DialogueEngine engine);
        public abstract bool IsDisplayable { get; }
    }
}
