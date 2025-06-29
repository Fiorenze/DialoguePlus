using UnityEngine;

namespace DialoguePlus
{
    public static class IfBlockParser
    {
        public static bool IsMatch(string line)
            => RegexPatterns.IfBlockPattern.IsMatch(line.Trim());

        public static (DialogueNode, int) Parse(string[] lines, int startIndex, DialogueEngine engine)
        {
            int i = startIndex;
            int baseIndent = IndentUtils.GetIndentLevel(lines[startIndex]);

            // find last line of conditional block
            int endLine = startIndex + 1;
            while (endLine < lines.Length && (IndentUtils.GetIndentLevel(lines[endLine]) > baseIndent || lines[endLine].Trim().StartsWith("elif") || lines[endLine].Trim().StartsWith("else")))
            {
                endLine++;
            }

            DialogueNode completeNode = new() { Speaker = string.Empty, Text = string.Empty };
            DialogueNodeTree newTree = new();
            DialogueNodeBranch newBranch = new() { Condition = string.Empty };

            while (i < endLine)
            {
                string currentLine = lines[i].Trim();

                if (string.IsNullOrEmpty(currentLine))
                {
                    i++;
                    continue;
                }
                if (currentLine.StartsWith('#'))
                {
                    i++;
                    continue;
                }

                if (ConditionParser.IsMatch(currentLine))
                {
                    // This means last condition block is complete
                    if (newBranch.Condition != string.Empty)
                    {
                        newTree.branches.Add(newBranch);
                        newBranch = new() { Condition = string.Empty };
                    }

                    newBranch.Condition = ConditionParser.Parse(currentLine);
                }
                else if (MenuBlockParser.IsMatch(currentLine))
                {
                    var (menuNode, newIndex) = MenuBlockParser.Parse(lines, i, engine);
                    i = newIndex;
                    newBranch.BranchNodes.Add(menuNode);
                }
                else if (LineParser.IsMatch(currentLine))
                {
                    var dialogueLine = LineParser.Parse(currentLine);
                    newBranch.BranchNodes.Add(dialogueLine);
                }
                else if (VariableActionParser.IsMatch(currentLine))
                {
                    var parsedAction = VariableActionParser.Parse(currentLine);

                    DialogueNode dialogueNode = new()
                    {
                        Action = parsedAction
                    };
                    newBranch.BranchNodes.Add(dialogueNode);
                }
                else if (CommandActionParser.IsMatch(currentLine))
                {
                    var parsedAction = CommandActionParser.Parse(currentLine);

                    DialogueNode dialogueNode = new()
                    {
                        Action = parsedAction
                    };
                    newBranch.BranchNodes.Add(dialogueNode);
                }
                else
                {
                    Debug.LogError($"Unknown line inside conditional block: '{currentLine}'");
                }

                i++;
            }

            // Last condition block is not added before here
            newTree.branches.Add(newBranch);

            completeNode.DialogueNodeTree = newTree;
            return (completeNode, endLine - 1);

        }
    }
}
