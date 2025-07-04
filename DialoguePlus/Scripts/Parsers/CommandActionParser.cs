using UnityEngine;

namespace DialoguePlus
{
    public static class CommandActionParser
    {
        public static bool IsMatch(string line)
            => RegexPatterns.CommandActionPattern.IsMatch(line.Trim());

        public static DialogueNode Parse(string line)
        {
            // group 1 = action, group 2 = text (can be scene label, image reference etc.)
            var match = RegexPatterns.CommandActionPattern.Match(line);

            string action = match.Groups[1].Value;
            string text = match.Groups[2].Value;

            CommandActionNode node = new()
            {
                Command = action,
                Text = text
            };

            return node;
        }
    }
}


