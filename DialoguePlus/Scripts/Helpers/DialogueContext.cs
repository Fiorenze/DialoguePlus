
namespace DialoguePlus
{
    [System.Serializable]
    public class DialogueContext
    {
        public string SceneLabel;
        public DialogueNode Node;
        public int IndexInScene;
        public bool ReturnFlag;
        public ReturnPoint ReturnPoint;
        public BranchContext Context;
    }
}
