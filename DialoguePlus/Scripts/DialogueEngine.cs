using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;
using System.Text.RegularExpressions;

namespace DialoguePlus
{
    public class DialogueEngine
    {
        public static DialogueEngine Instance { get; private set; }

        public DialogueEngine()
        {
            Instance = this;
        }

        public static List<CharacterDefinition> CharacterDefinitions;
        public static List<VariableDefinition> Variables;

        public TextAsset[] DialogueFiles;
        public TextAsset[] DefinitionFiles;


        public UnityAction OnSceneStarted;

        public SceneList Scenes { get; private set; }



        //Active scene
        public int readIndex = 0;
        public int activeSceneIndex = 0;
        public string[] LoadedText;

        public void Init()
        {
            Scenes = new()
            {
                SceneDataList = new()
            };

            InitializeDefinitions();
            PreLoadSceneData();
            StartScene("start");
            OnSceneStarted.Invoke();
        }

        public void StartScene(string sceneLabel)
        {
            if (Scenes.TryGetSceneData(sceneLabel, out SceneData sceneData))
            {
                activeSceneIndex = Scenes.IndexOf(sceneLabel);
                readIndex = sceneData.SceneIndex;
                LoadText(sceneData.FileIndex);
            }
            else
            {
                Debug.LogError($"Scene '{sceneLabel}' not found!");
            }
        }
        private void LoadText(int FileIndex)
        {
            if (DialogueFiles.Length <= FileIndex + 1)
                LoadedText = DialogueFiles[FileIndex].text.Split('\n', System.StringSplitOptions.RemoveEmptyEntries);
            else
                Debug.LogError("Cannot find text file!");
        }
        public bool TryStartNextScene()
        {
            if (activeSceneIndex < Scenes.SceneDataList.Count - 1)
            {
                string nextSceneLabel = Scenes.SceneDataList[activeSceneIndex + 1].SceneLabel;

                StartScene(nextSceneLabel);
                return true;
            }
            return false;
        }

        #region Initialization
        private void PreLoadSceneData()
        {
            for (int i = 0; i < DialogueFiles.Length; i++)
            {
                string text = DialogueFiles[i].text;
                string[] stringToArray = text.Split('\n', System.StringSplitOptions.RemoveEmptyEntries);

                int line = 0;

                while (line < stringToArray.Length)
                {
                    if (stringToArray[line].Trim().StartsWith("label "))
                    {
                        string sceneLabel = stringToArray[line].Trim().Split(' ')[1];
                        SceneData sceneData = new() { SceneLabel = sceneLabel, SceneIndex = line, FileIndex = i };
                        Scenes.Add(sceneData);
                    }
                    line++;
                }
            }
        }
        private void InitializeDefinitions()
        {
            //Will be saved and loaded from a file
            CharacterDefinitions = new List<CharacterDefinition>();
            Variables = new List<VariableDefinition>();

            foreach (TextAsset DefinitionsFile in DefinitionFiles)
            {
                string[] stringToArray = DefinitionsFile.text.Split('\n', System.StringSplitOptions.RemoveEmptyEntries);
                foreach (string line in stringToArray)
                {
                    if (string.IsNullOrEmpty(line.Trim()))
                    {
                        continue;
                    }
                    else if (line.Trim().StartsWith("#"))
                    {
                        continue;
                    }
                    if (RegexPatterns.CharacterDefinitionPattern.IsMatch(line.Trim()))
                    {
                        RegisterCharacterDefinition(line.Trim());
                    }
                    else if (RegexPatterns.VariableDefinitionPattern.IsMatch(line.Trim()))
                    {
                        RegisterVariableDefinition(line.Trim());
                    }
                    else
                    {
                        Debug.LogWarning($"Cannot recognize definition '{line}'");
                    }
                }
            }
        }
        private void RegisterCharacterDefinition(string line)
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
        }
        private void RegisterVariableDefinition(string line)
        {
            // ie. default health = 5 or default tested = False or default testName = "TEST"
            // Only supports int, bool, and string for now

            // group 1 = name, group 2 = value
            var Match = RegexPatterns.VariableDefinitionPattern.Match(line);

            string variableKey = Match.Groups[1].Value.Trim();
            string variableValue = Match.Groups[2].Value.Trim();

            if (bool.TryParse(variableValue, out bool parsedBool))
            {
                Variables.Add(new VariableDefinition(variableKey, parsedBool, VariableDefinition.VarType.Bool));
            }
            else if (int.TryParse(variableValue, out int parsedInt))
            {
                Variables.Add(new VariableDefinition(variableKey, parsedInt, VariableDefinition.VarType.Int));
            }
            else if (variableValue.Contains('"'))
            {
                Variables.Add(new VariableDefinition(variableKey, variableValue.Trim('"'), VariableDefinition.VarType.String));
            }
            else
            {
                Debug.LogError($"Couldn't parse variable definition on line '{line}'");
            }
        }

        #endregion



        #region Prepare Node

        public DialogueNode GetNextNode()
        {
            readIndex++;

            if (readIndex >= LoadedText.Length)
            {
                if (TryStartNextScene())
                {
                    return GetNextNode();
                }
                else
                {
                    Debug.Log("End of dialogue");
                    return null;
                }
            }

            string line = LoadedText[readIndex];

            // Ignore comments
            if (line.Trim().StartsWith("#"))
            {
                return GetNextNode();
            }
            // Ignore empty lines
            else if (string.IsNullOrWhiteSpace(line))
            {
                return GetNextNode();
            }
            else if (LineParser.IsMatch(line))
            {
                return LineParser.Parse(line);
            }
            else if (MenuBlockParser.IsMatch(line))
            {
                var (dialogueNode, newIndex) = MenuBlockParser.Parse(LoadedText, readIndex, this);
                readIndex = newIndex;
                return dialogueNode;
            }
            else if (IfBlockParser.IsMatch(line))
            {
                var (dialogueNode, newIndex) = IfBlockParser.Parse(LoadedText, readIndex, this);
                readIndex = newIndex;
                return dialogueNode;
            }
            else if (VariableActionParser.IsMatch(line))
            {
                DialogueNode node = new()
                {
                    Action = VariableActionParser.Parse(line)
                };
                return node;
            }
            else if (CommandActionParser.IsMatch(line))
            {
                DialogueNode node = new()
                {
                    Action = CommandActionParser.Parse(line)
                };
                return node;
            }
            else
            {
                Debug.LogWarning($"Don't know what to do with this line '{line}'");
                return GetNextNode();
            }

        }

        #endregion

        #region Native Actions

        public void ExecuteAction(string actionText)
        {
            if (RegexPatterns.CommandActionPattern.IsMatch(actionText))
            {
                // group 1 = action, group 2 = text (can be scene label, image reference etc.)
                var match = RegexPatterns.CommandActionPattern.Match(actionText);

                string action = match.Groups[1].Value;
                string text = match.Groups[2].Value;

                if (action.Trim() == "jump" || action.Trim() == "label")
                {
                    StartScene(text.Trim());
                }
                else
                {
                    Debug.Log("Only 'jump' action is supported for now!");
                }
            }
            else if (RegexPatterns.VariableActionPattern.IsMatch(actionText))
            {
                // group 1 = action, group 2 = operation, group 3 = modifier
                var match = RegexPatterns.VariableActionPattern.Match(actionText);

                string variableName = match.Groups[1].Value;
                string op = match.Groups[2].Success ? match.Groups[2].Value : null;
                string modifierValue = match.Groups[3].Value?.Trim();

                var value = GetVariableValue(variableName);

                if (value is int i)
                {
                    int modifier = int.Parse(modifierValue);

                    i = op switch
                    {
                        "=" => modifier,
                        "+=" => i += modifier,
                        "-=" => i -= modifier,
                        "*=" => i *= modifier,
                        "/=" => i /= modifier,
                        _ => i
                    };
                    SetVariableValue(variableName, i);
                }
                else if (value is bool b)
                {
                    bool modifier = bool.Parse(modifierValue);

                    if (op == "=") b = modifier;
                    else Debug.Log($"This operation '{op}' is not supported for booleans!");

                    SetVariableValue(variableName, b);
                }
                else if (value is string s)
                {
                    if (op == "=") s = modifierValue;
                    else Debug.Log($"This operation '{op}' is not supported for strings!");

                    SetVariableValue(variableName, s);
                }
            }
        }

        #endregion

        #region Check Condition

        public bool CheckCondition(string conditionSentence)
        {
            // Comparison operators are => '<' '>' '=' '==' '<=' '>=' '!='
            // 'if' and 'elif' is trimmed before coming here
            // definedInteger = 50, definedInteger <= 50

            //Debug.Log("Checking condition: " + conditionSentence);

            if (conditionSentence == "else") return true; // No need to check for 'else'

            var match = Regex.Match(conditionSentence.Trim(), @"(\w+)\s*(==|!=|>=|<=|>|<|=)\s*(.+)$");

            if (!match.Success)
            {
                Debug.LogError($"Wrong expression for condition; '{conditionSentence}'");
                return false;
            }

            string variableName = match.Groups[1].Value;
            string op = match.Groups[2].Success ? match.Groups[2].Value.Trim() : null;
            string expectedValue = match.Groups[3].Value?.Trim();

            if (op == "=") op = "==";

            return Evaluate(variableName, op, expectedValue);
        }

        public bool CheckSoftConditions(List<string> conditionList)
        {
            //Debug.Log("Checking soft conditions: " + string.Join(", ", conditionList));
            bool isTrue = false;
            foreach (string condition in conditionList)
            {
                if (CheckCondition(condition))
                {
                    isTrue = true;
                    break;
                }
            }
            return isTrue;
        }

        public bool CheckHardConditions(List<string> conditionList)
        {
            //Debug.Log("Checking hard conditions: " + string.Join(", ", conditionList));
            bool isTrue = true;
            foreach (string condition in conditionList)
            {
                if (CheckCondition(condition) == false)
                {
                    isTrue = false;
                    break;
                }
            }
            return isTrue;
        }

        bool Evaluate(string variable, string op, string expectedValue)
        {
            var value = GetVariableValue(variable);

            if (value is bool b)
            {
                bool expectedV;

                if (op == null)
                {
                    return b == true;
                }
                else
                {
                    if (!bool.TryParse(expectedValue, out expectedV))
                    {
                        Debug.LogError($"Couldn't parse given value '{expectedValue}'");
                        return false;
                    }
                }
                if (op == "==")
                {
                    return b == expectedV;
                }
                else if (op == "!=")
                {
                    return b != expectedV;
                }
                else
                {
                    Debug.LogError($"Operator '{op}' cannot be used for booleans!");
                }
            }
            else if (value is int i)
            {
                int target = int.Parse(expectedValue);

                return op switch
                {
                    "==" => i == target,
                    "!=" => i != target,
                    ">=" => i >= target,
                    "<=" => i <= target,
                    ">" => i > target,
                    "<" => i < target,
                    _ => false
                };
            }
            else if (value is string s)
            {
                if (op == "==")
                {
                    return s == expectedValue;
                }
                else if (op == "!=")
                {
                    return s != expectedValue;
                }
                else
                {
                    Debug.LogError($"Operator '{op}' cannot be used for comparing strings!");
                }
            }

            return false;
        }

        #endregion

        #region Extra Functions

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
        }

        #endregion

    }


    #region Helper classes

    public class SceneList
    {
        public List<SceneData> SceneDataList;
        public bool Contains(string sceneLabel)
        {
            foreach (SceneData sceneData in SceneDataList)
            {
                if (sceneData.SceneLabel == sceneLabel)
                {
                    return true;
                }
            }
            return false;
        }
        public int IndexOf(string sceneLabel)
        {
            int index = -1;
            foreach (SceneData sceneData in SceneDataList)
            {
                index++;
                if (sceneData.SceneLabel == sceneLabel)
                {
                    return index;
                }
            }
            return -1;
        }
        public SceneData GetSceneData(string sceneLabel)
        {
            return SceneDataList.Find(x => x.SceneLabel == sceneLabel);
        }
        public bool TryGetSceneData(string sceneLabel, out SceneData sceneData)
        {
            sceneData = SceneDataList.Find(x => x.SceneLabel == sceneLabel);
            return sceneData != null;
        }
        public void Add(SceneData sceneData)
        {
            SceneDataList.Add(sceneData);
        }
    }
    public class SceneData
    {
        public string SceneLabel;
        public int SceneIndex;
        public int FileIndex;
    }


    public class DialogueNode
    {
        public string Speaker { get; set; }
        public Color SpeakerColor = Color.white;
        public string Text { get; set; }
        public string Action { get; set; }
        public DialogueMenu Menu = new(); // For questions
        public DialogueNodeTree DialogueNodeTree = new();
    }
    public class DialogueMenu
    {
        public List<DialogueOption> Options = new();
    }
    public class DialogueOption
    {
        public string Text { get; set; }
        public List<string> SoftConditions = new();
        public List<string> HardConditions = new();
        public List<string> Actions = new(); // Actions like setting variables or jumping
    }
    public class DialogueNodeTree
    {
        public List<DialogueNodeBranch> branches = new();
    }
    public class DialogueNodeBranch
    {
        public string Condition { get; set; }
        public List<DialogueNode> BranchNodes = new();
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
        public enum VarType { Int, Bool, String }
        public string Key;
        public object Value;
        public VarType Type;
        public VariableDefinition(string key, object value, VarType type)
        {
            Key = key;
            Value = value;
            Type = type;
        }
    }

    #endregion

}
