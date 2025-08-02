
namespace DialoguePlus
{
    [System.Serializable]
    public class MenuOption
    {
        public string Text { get; set; }
        public string Condition { get; set; }
        public DialogueNodeBranch Branch { get; set; } = new();
    }
}
