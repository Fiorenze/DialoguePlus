using UnityEngine;

namespace DialoguePlus
{
    public static class ConditionParser
    {
        public static bool IsMatch(string line)
        {
            bool doesMatch = RegexPatterns.ConditionPattern.IsMatch(line.Trim());
            if (!doesMatch) doesMatch = RegexPatterns.ElsePattern.IsMatch(line.Trim());

            return doesMatch;
        }

        public static string Parse(string line)
        {
            if (RegexPatterns.ConditionPattern.IsMatch(line.Trim()))
            {
                var match = RegexPatterns.ConditionPattern.Match(line.Trim());
                string condition = match.Groups[2].Value;
                return condition;
            }
            else if (RegexPatterns.ElsePattern.IsMatch(line.Trim()))
            {
                return "else";
            }
            else
            {
                // It should never get here
                Debug.LogError("Something went wrong");
                return null;
            }

        }


    }
}

