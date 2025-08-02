
namespace DialoguePlus
{
    public class BranchContext
    {
        public DialogueNodeBranch Branch; // MenuOption or IfOption
        public int IndexInBranch;

        public BranchContext(DialogueNodeBranch branch, int indexInBranch)
        {
            Branch = branch;
            IndexInBranch = indexInBranch;
        }
    }
}
