using System.Text.RegularExpressions;
using UnityEngine;

namespace DialoguePlus
{
    public class DialogueLineNode : DialogueNode
    {
        public override bool IsDisplayable => true;

        public override void Execute(DialogueEngine engine)
        {
            DisplayText = ParseVariablesInText();
            DialogueManager.Instance.dialogueUI.DisplaySentence(this);
        }

        public override void Undo(DialogueEngine engine)
        {

        }

        private string ParseVariablesInText()
        {
            // Use original Text
            string result = Text;

            var regex = new Regex(@"\{(\w+)\}");

            result = regex.Replace(result, match =>
            {
                string varName = match.Groups[1].Value;
                object value = VariableManager.GetVariableValue(varName);


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

        public string Speaker { get; set; }
        public Color SpeakerColor = Color.white;
        public string Text { get; set; }
        public string DisplayText { get; private set; }
    }
}
