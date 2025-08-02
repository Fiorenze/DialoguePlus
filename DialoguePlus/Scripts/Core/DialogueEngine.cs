using UnityEngine;
using UnityEngine.Events;

namespace DialoguePlus
{
    public class DialogueEngine
    {
        //Actions
        public static UnityAction<DialogueNode> OnDisplaySentence;
        public static UnityAction OnDialogueEnd;

        //Rollback history
        private DialogueHistory History { get; set; }

        //Active scene
        private SceneData ActiveScene { get; set; }
        private int NodeIndex = 0;
        private BranchContext CurrentBranch; // null if currently not inside a branch
        private DialogueNode CurrentNode => GetCurrentNode();

        // Return point for 'call' function
        private ReturnPoint ReturnPoint { get; set; }
        private bool justReturned = false; // Go back to ReturnPoint but do not add that node (again) to history on advance

        public DialogueEngine()
        {
            History = new();
            justReturned = false;
        }

        public void StartScene(string sceneLabel)
        {
            NodeIndex = -1;
            ActiveScene = SceneDatabase.GetScene(sceneLabel.Trim());
            AdvanceDialogue();
        }

        public void AdvanceDialogue()
        {
            if (ActiveScene == null) return;

            do
            {
                // Normal flow is; Add current node to history -> Step forward -> Show/Execute
                // Starting a scene starts with index -1 to account for StepForward here 
                // Entering a branch starts with index -1 to account for StepForward here 
                // So, if we try to add to history as soon as we start or get into a branch, it will throw an error

                if (NodeIndex != -1 && CurrentBranch?.IndexInBranch != -1 && justReturned == false)
                    AddToHistory();

                if (justReturned) justReturned = false;

                StepForward();

                // End scene if index is over scene length
                if (NodeIndex > ActiveScene.Nodes.Count - 1)
                {
                    OnDialogueEnd?.Invoke();
                    ActiveScene = null;
                    break;
                }

                CurrentNode.Execute(this);
            }
            while (CanSkip);
        }

        public void Rollback()
        {
            if (ActiveScene == null || History.Count == 0) return;

            do
            {
                if (History.Count == 0) break;

                var last = History.Pop();
                last.Node.Undo(this);
                LoadState(last);
            }
            while (!CurrentNode.IsDisplayable || CurrentNode is IfBlockNode);

            // Menu block waits for input before entering a branch
            // But, if block enters a branch immediately upon executing
            // So if you execute if block node while rolling back, dialogue will get stuck in if block

            if (History.Count == 0 || (CurrentNode.IsDisplayable && CurrentNode is not IfBlockNode))
            {
                CurrentNode.Execute(this);
            }
        }

        private void LoadState(DialogueContext context)
        {
            SetScene(context.SceneLabel);

            BranchContext branchContext = context.Context;
            if (branchContext != null && branchContext.Branch != null)
            {
                CurrentBranch = new BranchContext(branchContext.Branch, branchContext.IndexInBranch);
            }
            NodeIndex = context.IndexInScene;
            justReturned = context.ReturnFlag;
            ReturnPoint = context.ReturnPoint;
        }

        #region Branch control

        public void EnterBranch(DialogueNodeBranch branch)
        {
            if (branch.BranchNodes.Count > 0)
            {
                AddToHistory();
                CurrentBranch = new BranchContext(branch, -1);
            }
            AdvanceDialogue();
        }

        public void ExitBranch()
        {
            CurrentBranch = null;
        }

        #endregion

        #region Scene control

        // Starts from the beginning of the scene
        public void JumpToScene(string sceneLabel)
        {
            StartScene(sceneLabel);
        }

        // Changes scene without altering current index and branch
        public void SetScene(string sceneLabel)
        {
            if (ActiveScene.SceneLabel != sceneLabel)
                ActiveScene = SceneDatabase.GetScene(sceneLabel);
        }

        #endregion

        #region Call/Return

        public void CreateReturnPoint()
        {
            ReturnPoint = new ReturnPoint(ActiveScene.SceneLabel, NodeIndex, CurrentBranch);
        }
        public void Return()
        {
            SetScene(ReturnPoint.SceneLabel);
            NodeIndex = ReturnPoint.IndexInScene;
            CurrentBranch = ReturnPoint.ContextWhenExecuted;

            ReturnPoint = null;
            justReturned = true;

            AdvanceDialogue();
        }

        #endregion


        public void DisplaySentence(DialogueNode node)
        {
            OnDisplaySentence?.Invoke(node);
        }

        public void AddToHistory()
        {
            History.Push(GetSnapshot());
        }

        private void StepForward()
        {
            if (CurrentBranch != null)
            {
                CurrentBranch.IndexInBranch++;
                if (CurrentBranch.IndexInBranch >= CurrentBranch.Branch.BranchNodes.Count)
                {
                    // Reached end of branch > exit branch
                    CurrentBranch = null;
                    NodeIndex++; // back to main flow
                }
            }
            else
            {
                NodeIndex++;
            }
        }

        private DialogueContext GetSnapshot()
        {
            BranchContext branchCtx = null;
            if (CurrentBranch != null)
            {
                branchCtx = new BranchContext(CurrentBranch.Branch, CurrentBranch.IndexInBranch);
            }
            DialogueContext ctx = new()
            {
                SceneLabel = ActiveScene.SceneLabel,
                Node = CurrentNode,
                IndexInScene = NodeIndex,
                Context = branchCtx,
                ReturnFlag = justReturned,
                ReturnPoint = ReturnPoint
            };

            return ctx;
        }

        private DialogueNode GetCurrentNode()
        {
            if (CurrentBranch != null)
            {
                if (CurrentBranch.IndexInBranch == -1) return null;
                return CurrentBranch.Branch.BranchNodes[CurrentBranch.IndexInBranch];
            }
            else
            {
                return GetNode(NodeIndex);
            }
        }

        private DialogueNode GetNode(int index)
        {
            if (index < 0 || index >= ActiveScene.Nodes.Count)
            {
                Debug.LogWarning($"Node index '{index}' is out of bounds! Active scene '{ActiveScene.SceneLabel}' has {ActiveScene.Nodes.Count} nodes.");
            }

            return ActiveScene.Nodes[index];
        }

        bool CanSkip => CurrentBranch == null ?
            !CurrentNode.IsDisplayable && NodeIndex < ActiveScene.Nodes.Count - 1 :
            !CurrentNode.IsDisplayable && CurrentBranch.IndexInBranch < CurrentBranch.Branch.BranchNodes.Count - 1; 

    }
}
