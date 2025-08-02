using System.Text.RegularExpressions;
using UnityEngine;

namespace DialoguePlus
{
    public class DialogueLineNode : DialogueNode
    {
        public override bool IsDisplayable => true;
        public override string Info => Speaker + ": " + (DisplayText.Length > 60 ? DisplayText[..60] : DisplayText);

        // Reference name and text
        public string Speaker { get; set; }
        public Color SpeakerColor = Color.white;
        public string Text { get; set; }

        // Displayed text
        public string DisplayText { get; private set; }
        public string DisplayName { get; private set; }

        public override void Execute(DialogueEngine engine)
        {
            DisplayName = ParseName();
            DisplayText = ParseVariablesInText();
            engine.DisplaySentence(this);
        }

        public override void Undo(DialogueEngine engine)
        {

        }

        // To support changing names we parse them right before displaying
        // If we parsed at the beginning it would not be possible to change the name during gameplay
        private string ParseName()
        {
            CharacterDefinition characterDefinition = (CharacterDefinition)VariableDatabase.Variables.Find(x => x.Key == Speaker);
            if (characterDefinition != null)
            {
                return characterDefinition.Value.ToString();
            }
            else
            {
                return Speaker;
            }
        }

        // Again, to keep text updated we parse it right before displaying
        private string ParseVariablesInText()
        {
            // Use original Text
            string result = Text;

            var regex = new Regex(@"\{(\w+)\}");

            result = regex.Replace(result, match =>
            {
                string varName = match.Groups[1].Value;
                object value = VariableDatabase.GetVariableValue(varName);


                if (value is int intValue)
                    return intValue.ToString();
                else if (value is bool boolValue)
                    return boolValue ? "true" : "false";
                else if (value is string stringValue)
                    return stringValue;
                else
                    return "{" + varName + "}"; // fallback: leave as-is if undefined
            });

            return result;
        }
    }
}
