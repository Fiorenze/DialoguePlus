using System.Collections.Generic;
using UnityEngine;

namespace DialoguePlus
{
    public static class MenuBlockParser
    {
        public static bool IsMatch(string line)
            => RegexPatterns.MenuPattern.IsMatch(line.Trim());

        public static (DialogueNode, int) Parse(string[] lines, int startIndex)
        {
            int i = startIndex;
            int baseIndent = IndentUtils.GetIndentLevel(lines[startIndex]);
            //int optionIndent = IndentUtils.GetIndentLevel(lines[startIndex + 1]);
            int optionIndent = -1;

            // find last line of menu block
            int endLine = startIndex + 1;
            while (endLine < lines.Length && IndentUtils.GetIndentLevel(lines[endLine]) > baseIndent)
            {
                if (optionIndent == -1 && MenuOptionParser.IsMatch(lines[endLine].Trim()))
                {
                    optionIndent = IndentUtils.GetIndentLevel(lines[endLine]);
                }
                endLine++;
            }

            MenuNode completeNode = new();
            List<MenuOption> options = new();

            MenuOption dialogueOption = new();

            while (++i < endLine)
            {
                string currentLine = lines[i].Trim();

                if (MenuOptionParser.IsMatch(currentLine) && IndentUtils.GetIndentLevel(lines[i]) == optionIndent)
                {
                    // This means last option block is complete
                    if (options.Count > 0 || !string.IsNullOrEmpty(dialogueOption.Text))
                    {
                        options.Add(dialogueOption);
                        dialogueOption = new();
                    }

                    var (text, condition) = MenuOptionParser.Parse(currentLine);
                    dialogueOption.Text = text;

                    dialogueOption.Condition = condition;
                }
                else if (LineParser.IsMatch(currentLine) && IndentUtils.GetIndentLevel(lines[i]) > optionIndent)
                {
                    var parsedLine = LineParser.Parse(currentLine);
                    dialogueOption.Branch.BranchNodes.Add(parsedLine);
                }
                else if (VariableActionParser.IsMatch(currentLine))
                {
                    var parsedAction = VariableActionParser.Parse(currentLine);
                    dialogueOption.Branch.BranchNodes.Add(parsedAction);
                }
                else if (CommandActionParser.IsMatch(currentLine))
                {
                    var parsedAction = CommandActionParser.Parse(currentLine);
                    dialogueOption.Branch.BranchNodes.Add(parsedAction);
                }
                else if (RegexPatterns.ReturnPattern.IsMatch(currentLine))
                {
                    dialogueOption.Branch.BranchNodes.Add(new ReturnNode());
                }
                else
                {
                    Debug.LogError($"Unknown line inside menu option: '{currentLine}'");
                }
            }
            // Last option block is not added before here
            options.Add(dialogueOption);
            completeNode.SetOptions(options);


            return (completeNode, endLine - 1);
        }
    }

    public static class MenuOptionParser
    {
        public static bool IsMatch(string line)
            => RegexPatterns.MenuOptionPattern.IsMatch(line.Trim());

        public static (string text, string condition) Parse(string line)
        {
            var match = RegexPatterns.MenuOptionPattern.Match(line.Trim());

            if (!match.Success)
            {
                Debug.LogError($"Wrong expression for menu option; '{line.Trim()}'");
                return (string.Empty, string.Empty);
            }

            string text = match.Groups[1].Value;
            string condition = match.Groups[2].Success ? match.Groups[2].Value : string.Empty;

            return (text, condition);
        }

    }
}


