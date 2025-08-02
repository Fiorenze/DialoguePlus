using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DialoguePlus
{
    public static class VariableDatabase
    {
        public static List<Variable> Variables { get; set; } = new();

        public static UnityAction OnVariablesUpdated;

        #region Functions

        public static void Init()
        {
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

            if (DoesExist(nameKey))
            {
                Debug.LogWarning($"Variable '{nameKey}' already exists!");
                return;
            }

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
            Variables.Add(characterDefinition);

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

            if (DoesExist(variableKey))
            {
                Debug.LogWarning($"Variable '{variableKey}' already exists!");
                return;
            }

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
            var v = Variables.Find(x => x.Key == key);
            if (v != null)
            {
                v.Value = value;
                OnVariablesUpdated?.Invoke();
                return;
            }

            Debug.LogError($"Variable '{key}' not found!");
            return;
        }

        public static bool DoesExist(string Key)
        {
            return Variables.Find(x => x.Key == Key) != null;
        }

        #endregion

    }

    [System.Serializable]
    public class CharacterDefinition : Variable
    {
        public Color Color;
        public CharacterDefinition(string key, string value, Color color)
        {
            Key = key;
            Value = value;
            Color = color == null ? Color.white : color;
        }
    }
    [System.Serializable]
    public class VariableDefinition : Variable
    {
        public VariableDefinition(string key, object value)
        {
            Key = key;
            Value = value;
        }
    }

    [System.Serializable]
    public class Variable
    {
        public string Key;
        public object Value;
    }
}
