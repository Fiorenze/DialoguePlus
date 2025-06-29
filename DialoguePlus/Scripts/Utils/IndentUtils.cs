using UnityEngine;

namespace DialoguePlus
{
    public static class IndentUtils
    {
        public static int GetIndentLevel(string line)
        {
            int count = 0;
            foreach (char c in line)
            {
                if (c == ' ') count++;
                else if (c == '\t') count += 4; // or treat as 1 tab = 1 level
                else break;
            }
            return count;
        }
    }
}

