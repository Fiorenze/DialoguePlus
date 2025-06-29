using UnityEngine;

namespace DialoguePlus
{
    public static class CommandActionParser
    {
        // Even though we don't do any operation, we validate the format of the line
        public static bool IsMatch(string line)
            => RegexPatterns.CommandActionPattern.IsMatch(line.Trim());

        public static string Parse(string line)
        {
            return line.Trim();
        }
    }
}


