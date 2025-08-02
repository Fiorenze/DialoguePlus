using System.Collections.Generic;
using UnityEngine;

namespace DialoguePlus
{
    public static class SceneDatabase
    {
        public static SceneList Scenes { get; private set; } = new();

        public static void Init()
        {
            Scenes = new();
        }

        public static void AddScene(SceneData sceneData)
        {
            if (Scenes.Contains(sceneData.SceneLabel))
            {
                Debug.LogWarning($"Scene '{sceneData.SceneLabel}' already exists!");
                return;
            }

            Scenes.Add(sceneData);
        }

        public static SceneData GetScene(string sceneLabel)
        {
            if (Scenes.TryGetSceneData(sceneLabel, out SceneData sceneData))
            {
                return sceneData;
            }
            else
            {
                Debug.LogError($"Scene '{sceneLabel}' not found!");
                return null;
            }
        }
    }

    public class SceneList
    {
        public List<SceneData> SceneDataList = new();
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
        public List<DialogueNode> Nodes = new();
    }
}
