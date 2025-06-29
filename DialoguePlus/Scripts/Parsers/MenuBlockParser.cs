using UnityEngine;
using UnityEngine.UI;

namespace DialoguePlus
{
    public static class MenuBlockParser
    {
        public static bool IsMatch(string line)
            => RegexPatterns.MenuPattern.IsMatch(line.Trim());

        public static (DialogueNode, int) Parse(string[] lines, int startIndex, DialogueEngine engine)
        {
            int i = startIndex;
            int baseIndent = IndentUtils.GetIndentLevel(lines[startIndex]);
            int optionIndent = IndentUtils.GetIndentLevel(lines[startIndex + 1]);

            // find last line of menu block
            int endLine = startIndex + 1;
            while (endLine < lines.Length && IndentUtils.GetIndentLevel(lines[endLine]) > baseIndent)
            {
                endLine++;
            }

            DialogueNode completeNode = new() { Speaker = string.Empty, Text = string.Empty };
            DialogueMenu menu = new();
            DialogueOption dialogueOption = new();

            while (++i < endLine)
            {
                string currentLine = lines[i].Trim();

                if (string.IsNullOrEmpty(currentLine)) continue;
                if (currentLine.StartsWith('#')) continue;

                if (MenuOptionParser.IsMatch(currentLine))
                {
                    // This means last option block is complete
                    if (!string.IsNullOrEmpty(dialogueOption.Text))
                    {
                        menu.Options.Add(dialogueOption);
                        dialogueOption = new();
                    }

                    var (text, condition) = MenuOptionParser.Parse(currentLine);
                    dialogueOption.Text = text;

                    if (condition != string.Empty)
                        dialogueOption.HardConditions.Add(condition);
                }
                else if (VariableActionParser.IsMatch(currentLine))
                {
                    var parsedAction = VariableActionParser.Parse(currentLine);
                    dialogueOption.Actions.Add(parsedAction);
                }
                else if (CommandActionParser.IsMatch(currentLine))
                {
                    var parsedAction = CommandActionParser.Parse(currentLine);
                    dialogueOption.Actions.Add(parsedAction);
                }
                else
                {
                    Debug.LogError($"Unknown line inside menu block: '{currentLine}'");
                }
            }

            menu.Options.Add(dialogueOption);
            completeNode.Menu = menu;

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


