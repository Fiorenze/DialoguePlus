using UnityEngine;

namespace DialoguePlus
{
    public static class LineParser
    {
        public static bool IsMatch(string line)
            => RegexPatterns.LinePattern.IsMatch(line.Trim());

        public static DialogueNode Parse(string line)
        {
            var (speaker, speakerColor, dialogueText) = ExtractSpeakerAndText(line.Trim());
            return new DialogueNode { Speaker = speaker, Text = dialogueText, SpeakerColor = speakerColor };
        }

        public static (string speaker, Color speakerColor, string dialogueText) ExtractSpeakerAndText(string line)
        {
            var match = RegexPatterns.LinePattern.Match(line);
            string speaker = match.Groups[1].Value;
            string dialogueText = match.Groups[2].Value;

            CharacterDefinition characterDefinition = DialogueEngine.CharacterDefinitions.Find(x => x.Key == speaker);
            Color speakerColor = characterDefinition != null ? characterDefinition.Color : Color.white;
            speaker = characterDefinition != null ? characterDefinition.Value : speaker;

            // Parse for variables referenced in the dialogue text
            if (dialogueText.Contains('{'))
            {
                string[] split = dialogueText.Split('{');
                string variableKey = split[1].Split('}')[0];
                object variableValue = DialogueEngine.GetVariableValue(variableKey);

                if (variableValue != null)
                    dialogueText = split[0] + variableValue + split[1].Split('}')[1];

                else
                    Debug.LogWarning($"Couldn't find the variable {variableKey} in definitions");
            }

            return (speaker.Trim(), speakerColor, dialogueText.Trim());
        }

    }
}

