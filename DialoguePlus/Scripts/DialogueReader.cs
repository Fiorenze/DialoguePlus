using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace DialoguePlus
{
    public class DialogueReader
    {
        public DialogueReader(DialogueUI dialogueUI)
        {
            this.dialogueUI = dialogueUI;
            DialogueManager.Instance.DialogueEngine.OnSceneStarted += OnSceneStarted;
        }
        public void OnDestroy()
        {
            DialogueManager.Instance.DialogueEngine.OnSceneStarted -= OnSceneStarted;
        }

        private DialogueUI dialogueUI;

        //Dialogue
        private DialogueNode currentDialogueNode;

        //Branch
        private int currentTreeBranch = -1, currentTreeBranchNode = 0;
        private bool isOnBranch = false;


        private void OnSceneStarted()
        {
            Progress();
        }

        public void Progress()
        {
            if (!isOnBranch)
                currentDialogueNode = DialogueManager.Instance.DialogueEngine.GetNextNode();

            if (currentDialogueNode == null)
            {
                EndDialogue();
                return;
            }

            if (currentDialogueNode.DialogueNodeTree.branches.Count > 0 && !isOnBranch)
            {
                isOnBranch = true;
                currentTreeBranch = SelectTreeBranch();
                currentTreeBranchNode = 0;
            }

            if (isOnBranch)
            {
                ProgressBranch();
            }
            else
            {
                HandleDialogueNode(currentDialogueNode);
            }

            if (isOnBranch && currentTreeBranchNode > currentDialogueNode.DialogueNodeTree.branches[currentTreeBranch].BranchNodes.Count - 1)
            {
                isOnBranch = false;
            }
        }
        private void ProgressBranch()
        {
            DialogueNode branchNode = currentDialogueNode.DialogueNodeTree.branches[currentTreeBranch].BranchNodes[currentTreeBranchNode];
            currentTreeBranchNode++;
            HandleDialogueNode(branchNode);            
        }

        private void HandleDialogueNode(DialogueNode node)
        {
            if (!string.IsNullOrEmpty(node.Action))
            {
                DialogueEngine.Instance.ExecuteAction(node.Action.Trim());
                Progress();
            }
            else
            {
                DisplaySentence(node);
            }
        }

        private void DisplaySentence(DialogueNode node)
        {
            dialogueUI.DisplaySentence(node);
        }

        private int SelectTreeBranch()
        {
            int selectedBranch = -1;

            DialogueNodeTree nodeTree = currentDialogueNode.DialogueNodeTree;
            for (int i = 0; i < nodeTree.branches.Count; i++)
            {
                if (DialogueEngine.Instance.CheckCondition(nodeTree.branches[i].Condition))
                {
                    selectedBranch = i;
                    break;
                }
            }

            return selectedBranch;
        }
        private void EndDialogue()
        {
            dialogueUI.EndDialogue();
        }
    }
}

