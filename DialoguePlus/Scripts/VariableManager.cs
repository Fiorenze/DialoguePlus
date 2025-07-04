using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DialoguePlus
{
    public static class VariableManager
    {
        public static List<CharacterDefinition> CharacterDefinitions { get; private set; } = new();
        public static List<VariableDefinition> Variables { get; private set; } = new();

        public static UnityAction OnVariablesUpdated;

        #region Functions

        public static void Init()
        {
            CharacterDefinitions = new();
            Variables = new();
        }

        public static void RegisterCharacterDefinition(string line)
        {
            // ie. define ch = "Challenger", #FF0000

            // group 1 = name, group 2 = value, group 3 = color
            var Match = RegexPatterns.CharacterDefinitionPattern.Match(line);

            string nameKey = Match.Groups[1].Value;
            string nameValue = Match.Groups[2].Value;
            string htmlColor = Match.Groups[3].Success ? Match.Groups[3].Value : string.Empty;

            Color nameColor = Color.white;

            if (!string.IsNullOrEmpty(htmlColor))
            {
                if (ColorUtility.TryParseHtmlString(htmlColor.Trim(), out Color parsedColor))
                {
                    nameColor = parsedColor;
                }
                else
                {
                    Debug.LogWarning($"Cannot parse given color code, '{htmlColor}'");
                }
            }

            CharacterDefinition characterDefinition = new(nameKey, nameValue, nameColor);
            CharacterDefinitions.Add(characterDefinition);

            OnVariablesUpdated?.Invoke();
        }

        public static void RegisterVariableDefinition(string line)
        {
            // ie. default health = 5 or default tested = False or default testName = "TEST"
            // Only supports int, bool, and string for now

            // group 1 = name, group 2 = value
            var Match = RegexPatterns.VariableDefinitionPattern.Match(line);

            string variableKey = Match.Groups[1].Value.Trim();
            string variableValue = Match.Groups[2].Value.Trim();

            if (bool.TryParse(variableValue, out bool parsedBool))
            {
                Variables.Add(new VariableDefinition(variableKey, parsedBool));
            }
            else if (int.TryParse(variableValue, out int parsedInt))
            {
                Variables.Add(new VariableDefinition(variableKey, parsedInt));
            }
            else if (variableValue.Contains('"'))
            {
                Variables.Add(new VariableDefinition(variableKey, variableValue.Trim('"')));
            }
            else
            {
                Debug.LogError($"Couldn't parse variable definition on line '{line}'");
            }

            OnVariablesUpdated?.Invoke();
        }

        public static object GetVariableValue(string key)
        {
            var i = Variables.Find(x => x.Key == key);
            if (i != null) return i.Value;

            return null;
        }

        public static void SetVariableValue(string key, object value)
        {
            if (Variables.Find(x => x.Key == key) == null)
            {
                Debug.LogError($"Variable '{key}' not found!");
                return;
            }
            Variables.Find(x => x.Key == key).Value = value;

            OnVariablesUpdated?.Invoke();
        }

        #endregion

    }

    [System.Serializable]
    public class CharacterDefinition
    {
        public string Key, Value;
        public Color Color;
        public CharacterDefinition(string key, string value, Color color)
        {
            Key = key;
            Value = value;
            Color = color == null ? Color.white : color;
        }
    }
    [System.Serializable]
    public class VariableDefinition
    {
        public string Key;
        public object Value;
        public VariableDefinition(string key, object value)
        {
            Key = key;
            Value = value;
        }
    }
}
