using UnityEngine;

namespace DialoguePlus
{
    public static class VariableActionParser
    {
        public static bool IsMatch(string line)
            => RegexPatterns.VariableActionPattern.IsMatch(line.Trim());

        public static DialogueNode Parse(string line)
        {
            // group 1 = variable, group 2 = operation, group 3 = modifier
            var match = RegexPatterns.VariableActionPattern.Match(line);
            string variable = match.Groups[1].Value;
            string operation = match.Groups[2].Success ? match.Groups[2].Value : null;
            string modifier = match.Groups[3].Value?.Trim();

            VariableActionNode node = new()
            {
                Variable = variable,
                Operation = operation,
                Modifier = modifier
            };

            return node;
        }
    }
}

