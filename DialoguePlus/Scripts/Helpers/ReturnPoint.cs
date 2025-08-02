
namespace DialoguePlus
{
    [System.Serializable]
    public class ReturnPoint
    {
        public string SceneLabel;
        public int IndexInScene;
        public BranchContext ContextWhenExecuted;


        public ReturnPoint(string sceneLabel, int indexInScene, BranchContext contextWhenExecuted)
        {
            SceneLabel = sceneLabel;
            IndexInScene = indexInScene;
            ContextWhenExecuted = contextWhenExecuted;
        }
    }
}
