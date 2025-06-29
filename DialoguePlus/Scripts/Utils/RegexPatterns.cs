using System.Text.RegularExpressions;

namespace DialoguePlus
{
    public static class RegexPatterns
    {
        // group 1 = condition
        public static Regex IfBlockPattern = new(@"^if\s+(.+)$");

        public static Regex MenuPattern = new(@"^menu:\s*$");

        // group 1 = text, group 2 = condition (optional)
        public static Regex MenuOptionPattern = new(@"^""([^""]+)""(?:\s+if\s+(.+))?$");

        // group 1 = label
        public static Regex LabelPattern = new(@"^label\s+(\w+)$");

        // group 1 = action, group 2 = operation, group 3 = modifier
        public static Regex VariableActionPattern = new(@"^\$\s*(\w+)\s*(=|\+=|-=|\*=|/=)\s*(.+?)$");

        // group 1 = action, group 2 = text (can be scene label, image reference etc.)
        public static Regex CommandActionPattern = new(@"^(jump|label|scene|show|hide)\s+(.+)$");

        // group 1 = speaker (optional), group 2 = text
        public static Regex LinePattern = new(@"^(?:\s*(\w+)\s+)?\""(.*?)\""$");

        // group 1 = if/elif, group 2 = condition
        public static Regex ConditionPattern = new(@"^(if|elif)\s+(.+)$");
        public static Regex ElsePattern = new(@"^else$");

        // group 1 = name, group 2 = value, group 3 = color
        public static Regex CharacterDefinitionPattern = new(@"^define\s+(\w+)\s*=\s*""([^""]+)""(?:\s*,\s*(.+))?$");

        // group 1 = name, group 2 = value
        public static Regex VariableDefinitionPattern = new(@"^default\s+(\w+)\s*=\s*(.+)$");

    }
}

