using UnityEngine;

namespace DialoguePlus
{
    public static class CommandActionParser
    {
        public static bool IsMatch(string line)
            => RegexPatterns.CommandActionPattern.IsMatch(line.Trim());

        public static DialogueNode Parse(string line)
        {
            // group 1 = action, group 2 = sceneLabel
            var match = RegexPatterns.CommandActionPattern.Match(line);

            string action = match.Groups[1].Value;
            string text = match.Groups[2].Value;

            if (action == "call")
            {
                CallActionNode callNode = new()
                {
                    SceneLabel = text
                };
                return callNode;
            }
            else if (action == "jump")
            {
                JumpActionNode jumpNode = new()
                {
                    SceneLabel = text
                };
                return jumpNode;
            }
            else
            {
                // It should never get here
                Debug.LogError("Unknown command action!");
                return null;
            }
        }
    }
}


