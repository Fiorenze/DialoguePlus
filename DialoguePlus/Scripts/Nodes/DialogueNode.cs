using UnityEngine;

namespace DialoguePlus
{
    [System.Serializable]
    public abstract class DialogueNode
    {
        public abstract void Execute(DialogueEngine engine);
        public abstract void Undo(DialogueEngine engine);
        public abstract bool IsDisplayable { get; }
        public abstract string Info { get; }
    }
}
