using System.Collections.Generic;
using UnityEngine;

namespace DialoguePlus
{
    public static class FileReader
    {

        #region Dialogue 

        public static void ReadDialogueFile(TextAsset textAsset)
        {
            string[] stringToArray = textAsset.text.Split('\n', System.StringSplitOptions.RemoveEmptyEntries);

            List<LabelIndex> labelIndexes = IndexLabels(stringToArray);

            int sceneEndIndex;

            for (int i = 0; i < labelIndexes.Count; i++)
            {
                LabelIndex labelIndex = labelIndexes[i];

                // If not the last label, set the end index to the next label's start index
                if (i < labelIndexes.Count - 1)
                {
                    sceneEndIndex = labelIndexes[i + 1].startIndex - 1;
                }
                else
                {
                    // If it's the last label, set the end index to the end of the array
                    sceneEndIndex = stringToArray.Length;
                }

                // Extract scene data for this label
                SceneData sceneData = ExtractSceneData(stringToArray, labelIndex.startIndex, sceneEndIndex);
                SceneManager.AddScene(sceneData);
            }
        }

        public static SceneData ExtractSceneData(string[] sceneTextArray, int startIndex, int endIndex)
        {
            SceneData sceneData = new();

            int i = startIndex;

            while (i < endIndex)
            {
                string line = sceneTextArray[i].Trim();

                // Ignore comments and empty lines
                if (line.Trim().StartsWith("#") || string.IsNullOrEmpty(line.Trim()))
                {
                    i++;
                    continue;
                }
                if (RegexPatterns.LabelPattern.IsMatch(line))
                {
                    // group 1 = label
                    var Match = RegexPatterns.LabelPattern.Match(line);
                    string label = Match.Groups[1].Value;
                    sceneData.SceneLabel = label;
                    i++;
                    continue;
                }

                if (LineParser.IsMatch(line))
                {
                    sceneData.Nodes.Add(LineParser.Parse(line));
                }
                else if (MenuBlockParser.IsMatch(line))
                {
                    var (dialogueNode, newIndex) = MenuBlockParser.Parse(sceneTextArray, i);
                    i = newIndex;
                    sceneData.Nodes.Add(dialogueNode);
                }
                else if (IfBlockParser.IsMatch(line))
                {
                    var (dialogueNode, newIndex) = IfBlockParser.Parse(sceneTextArray, i);
                    i = newIndex;
                    sceneData.Nodes.Add(dialogueNode);
                }
                else if (VariableActionParser.IsMatch(line))
                {
                    DialogueNode dialogueNode = VariableActionParser.Parse(line);
                    sceneData.Nodes.Add(dialogueNode);
                }
                else if (CommandActionParser.IsMatch(line))
                {
                    DialogueNode dialogueNode = CommandActionParser.Parse(line);
                    sceneData.Nodes.Add(dialogueNode);
                }
                else
                {
                    Debug.LogWarning($"Don't know what to do with this line '{line}'");
                    return null;
                }

                i++;
            }

            return sceneData;
        }

        public static List<LabelIndex> IndexLabels(string[] sceneTextArray)
        {
            List<LabelIndex> indexList = new();

            for (int i = 0; i < sceneTextArray.Length; i++)
            {
                string line = sceneTextArray[i].Trim();

                if (RegexPatterns.LabelPattern.IsMatch(line))
                {
                    // group 1 = label
                    var match = RegexPatterns.LabelPattern.Match(line.Trim());
                    string label = match.Groups[1].Value;

                    LabelIndex labelIndex = new()
                    {
                        label = label,
                        startIndex = i
                    };
                    indexList.Add(labelIndex);
                }
            }

            return indexList;
        }



        #endregion


        #region Definition

        public static void ReadDefinitionFile(TextAsset textAsset)
        {
            string[] stringToArray = textAsset.text.Split('\n', System.StringSplitOptions.RemoveEmptyEntries);

            foreach (string line in stringToArray)
            {
                // Ignore comments and empty lines
                if (line.Trim().StartsWith("#") || string.IsNullOrEmpty(line.Trim()))
                {
                    continue;
                }
                if (RegexPatterns.CharacterDefinitionPattern.IsMatch(line.Trim()))
                {
                    VariableManager.RegisterCharacterDefinition(line.Trim());
                }
                else if (RegexPatterns.VariableDefinitionPattern.IsMatch(line.Trim()))
                {
                    VariableManager.RegisterVariableDefinition(line.Trim());
                }
                else
                {
                    Debug.LogWarning($"Cannot recognize definition '{line}'");
                }
            }
        }

        #endregion
    }

    public class LabelIndex
    {
        public string label;
        public int startIndex;

    }
}
