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
            return new DialogueLineNode { Speaker = speaker, Text = dialogueText, SpeakerColor = speakerColor };
        }

        public static (string speaker, Color speakerColor, string dialogueText) ExtractSpeakerAndText(string line)
        {
            var match = RegexPatterns.LinePattern.Match(line);
            string speaker = match.Groups[1].Value;
            string dialogueText = match.Groups[2].Value;

            CharacterDefinition characterDefinition = VariableManager.CharacterDefinitions.Find(x => x.Key == speaker);
            Color speakerColor = characterDefinition != null ? characterDefinition.Color : Color.white;
            speaker = characterDefinition != null ? characterDefinition.Value : speaker;

            return (speaker.Trim(), speakerColor, dialogueText.Trim());
        }

    }
}

