using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Text.RegularExpressions;

namespace DialoguePlus
{
    public class DialogueEngine
    {
        public static DialogueEngine Instance { get; private set; }

        public DialogueEngine()
        {
            Instance = this;
        }

        //Rollback history
        public int MaxHistorySize = 50;
        public Stack<HistoryItem> History = new Stack<HistoryItem>();

        //Active scene
        public SceneData ActiveScene { get; private set; }
        public int NodeIndex = 0;
        public BranchContext CurrentBranch; // null if not inside branch

        //Actions
        public UnityAction OnHistoryChanged;
        public UnityAction OnDialogueEnd;

        public void Init()
        {
            History = new();

            StartScene("dev");
        }

        public void StartScene(string sceneLabel)
        {
            NodeIndex = -1;

            ActiveScene = SceneManager.GetScene(sceneLabel.Trim());

            AdvanceDialogue();
        }

        public void AdvanceDialogue()
        {
            if (ActiveScene == null) return;

            if (NodeIndex >= ActiveScene.Nodes.Count - 1)
            {
                OnDialogueEnd?.Invoke();
                ActiveScene = null;
                return;
            }

            do
            {
                // Normal flow is; Add current node to history -> Step forward -> Show/Execute
                // Starting a scene starts with index -1 to account for StepForward here 
                // Entering branch starts with index -1 to account for StepForward here 
                // So, if we try to add to history as soon as we get into branch, it will throw an error

                if (NodeIndex != -1 && CurrentBranch?.IndexInBranch != -1)
                    AddToHistory();

                StepForward();
                ExecuteNode();
            }
            while (!GetCurrentNode().IsDisplayable);
        }

        public void Rollback()
        {
            if (ActiveScene == null) return;

            if (History.Count == 0)
                return;

            do
            {
                if (History.Count == 0) break;
                LoadLastState();
            }
            while (!GetCurrentNode().IsDisplayable || GetCurrentNode() is IfBlockNode);

            // Menu block waits for input before entering a branch
            // But if block enters a branch immediately upon executing
            // So if you execute if block node while rolling back dialogue will get stuck in if block

            if (GetCurrentNode().IsDisplayable && GetCurrentNode() is not IfBlockNode)
            {
                GetCurrentNode().Execute(this);
            }
        }

        public void EnterBranch(DialogueNodeBranch branch)
        {
            if (branch.BranchNodes.Count > 0)
            {
                AddToHistory();
                CurrentBranch = new BranchContext
                {
                    Branch = branch,
                    IndexInBranch = -1
                };
            }
            AdvanceDialogue();
        }

        public void ExitBranch()
        {
            CurrentBranch = null;
        }

        private void LoadLastState()
        {
            var last = History.Pop();

            //DEBUG
            OnHistoryChanged?.Invoke();

            last.Node.Undo(this);

            BranchContext context = last.ContextWhenExecuted;
            if (context != null && context.Branch != null)
            {
                CurrentBranch = new BranchContext
                {
                    Branch = context.Branch,
                    IndexInBranch = context.IndexInBranch
                };
            }
            NodeIndex = last.IndexInScene;
        }

        private void ExecuteNode()
        {
            var node = GetCurrentNode();
            node.Execute(this);

        }

        private void AddToHistory()
        {
            BranchContext snapshot = null;
            if (CurrentBranch != null)
            {
                snapshot = new BranchContext
                {
                    Branch = CurrentBranch.Branch,
                    IndexInBranch = CurrentBranch.IndexInBranch
                };
            }

            History.Push(new HistoryItem
            {
                Node = GetCurrentNode(),
                IndexInScene = NodeIndex,
                ContextWhenExecuted = snapshot
            });

            //DEBUG
            OnHistoryChanged?.Invoke();
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

        public void JumpToScene(string sceneLabel)
        {
            AddToHistory();
            StartScene(sceneLabel);
        }

        private DialogueNode GetCurrentNode()
        {
            if (CurrentBranch != null)
            {
                return CurrentBranch.Branch.BranchNodes[CurrentBranch.IndexInBranch];
            }
            else
            {
                return ActiveScene.Nodes[NodeIndex];
            }
        }

        #region Check Condition

        public bool CheckCondition(string conditionSentence)
        {
            // Comparison operators are => '<' '>' '=' '==' '<=' '>=' '!='
            // 'if' and 'elif' is trimmed before coming here
            // definedInteger = 50, definedInteger <= 50

            if (conditionSentence == "else") return true; // No need to check for 'else'

            var match = Regex.Match(conditionSentence.Trim(), @"(\w+)\s*(==|!=|>=|<=|>|<|=)\s*(.+)$");

            if (!match.Success)
            {
                Debug.LogError($"Wrong expression for condition; '{conditionSentence}'");
                return false;
            }

            string variableName = match.Groups[1].Value.Trim();
            string op = match.Groups[2].Success ? match.Groups[2].Value.Trim() : null;
            string expectedValue = match.Groups[3].Value?.Trim();

            if (op == "=") op = "==";

            return Evaluate(variableName, op, expectedValue);
        }

        public bool CheckSoftConditions(List<string> conditionList)
        {
            //Debug.Log("Checking soft conditions: " + string.Join(", ", conditionList));
            bool isTrue = false;
            foreach (string condition in conditionList)
            {
                if (CheckCondition(condition))
                {
                    isTrue = true;
                    break;
                }
            }
            return isTrue;
        }

        public bool CheckHardConditions(List<string> conditionList)
        {
            //Debug.Log("Checking hard conditions: " + string.Join(", ", conditionList));
            bool isTrue = true;
            foreach (string condition in conditionList)
            {
                if (CheckCondition(condition) == false)
                {
                    isTrue = false;
                    break;
                }
            }
            return isTrue;
        }

        bool Evaluate(string variable, string op, string expectedValue)
        {
            var value = VariableManager.GetVariableValue(variable);

            if (value is bool b)
            {
                bool expectedV;

                if (op == null)
                {
                    return b == true;
                }
                else
                {
                    if (!bool.TryParse(expectedValue, out expectedV))
                    {
                        Debug.LogError($"Couldn't parse given value '{expectedValue}'");
                        return false;
                    }
                }
                if (op == "==")
                {
                    return b == expectedV;
                }
                else if (op == "!=")
                {
                    return b != expectedV;
                }
                else
                {
                    Debug.LogError($"Operator '{op}' cannot be used for booleans!");
                }
            }
            else if (value is int i)
            {
                int target = int.Parse(expectedValue);

                return op switch
                {
                    "==" => i == target,
                    "!=" => i != target,
                    ">=" => i >= target,
                    "<=" => i <= target,
                    ">" => i > target,
                    "<" => i < target,
                    _ => false
                };
            }
            else if (value is string s)
            {
                if (op == "==")
                {
                    return s == expectedValue;
                }
                else if (op == "!=")
                {
                    return s != expectedValue;
                }
                else
                {
                    Debug.LogError($"Operator '{op}' cannot be used for comparing strings!");
                }
            }

            return false;
        }

        #endregion

    }

    public class HistoryItem
    {
        public DialogueNode Node;
        public int IndexInScene;
        public BranchContext ContextWhenExecuted;
    }

    public class BranchContext
    {
        public DialogueNodeBranch Branch; // MenuOption or IfOption
        public int IndexInBranch;
    }

}
