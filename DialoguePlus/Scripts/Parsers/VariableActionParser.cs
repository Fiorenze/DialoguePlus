using UnityEngine;

namespace DialoguePlus
{
    public static class VariableActionParser
    {
        // Even though we don't do any operation, we validate the format of the line
        public static bool IsMatch(string line)
            => RegexPatterns.VariableActionPattern.IsMatch(line.Trim());

        public static string Parse(string line)
        {
            return line.Trim();
        }
    }
}

