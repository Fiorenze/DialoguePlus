using System.Collections.Generic;

namespace DialoguePlus
{
    [System.Serializable]
    public class DialogueNodeBranch
    {
        public string Condition { get; set; }
        public List<DialogueNode> BranchNodes = new();
    }
}
