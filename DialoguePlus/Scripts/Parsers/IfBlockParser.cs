using System.Collections.Generic;
using UnityEngine;

namespace DialoguePlus
{
    public static class IfBlockParser
    {
        public static bool IsMatch(string line)
            => RegexPatterns.IfBlockPattern.IsMatch(line.Trim());

        public static (DialogueNode, int) Parse(string[] lines, int startIndex)
        {
            int i = startIndex;
            int baseIndent = IndentUtils.GetIndentLevel(lines[startIndex]);

            // find last line of conditional block
            int endLine = startIndex + 1;
            while (endLine < lines.Length && (IndentUtils.GetIndentLevel(lines[endLine]) > baseIndent || lines[endLine].Trim().StartsWith("elif") || lines[endLine].Trim().StartsWith("else")))
            {
                endLine++;
            }

            IfBlockNode completeNode = new();
            List<DialogueNodeBranch> branches = new();
            DialogueNodeBranch newBranch = new() { Condition = string.Empty };

            while (i < endLine)
            {
                string currentLine = lines[i].Trim();

                if (ConditionParser.IsMatch(currentLine))
                {
                    // This means last condition block is complete
                    if (newBranch.Condition != string.Empty)
                    {
                        branches.Add(newBranch);
                        newBranch = new() { Condition = string.Empty };
                    }

                    newBranch.Condition = ConditionParser.Parse(currentLine);
                }
                else if (MenuBlockParser.IsMatch(currentLine))
                {
                    var (menuNode, newIndex) = MenuBlockParser.Parse(lines, i);
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
                    var dialogueNode = VariableActionParser.Parse(currentLine);

                    newBranch.BranchNodes.Add(dialogueNode);
                }
                else if (CommandActionParser.IsMatch(currentLine))
                {
                    var dialogueNode = CommandActionParser.Parse(currentLine);

                    newBranch.BranchNodes.Add(dialogueNode);
                }
                else if (RegexPatterns.ReturnPattern.IsMatch(currentLine))
                {
                    newBranch.BranchNodes.Add(new ReturnNode());
                }
                else
                {
                    Debug.LogError($"Unknown line inside conditional block: '{currentLine}'");
                }

                i++;
            }

            // Last condition block is not added before here
            branches.Add(newBranch);
            completeNode.SetBranches(branches);

            return (completeNode, endLine - 1);

        }
    }

}
