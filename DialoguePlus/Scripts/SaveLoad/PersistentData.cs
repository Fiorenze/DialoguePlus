using UnityEngine;

namespace DialoguePlus
{
    public static class PersistentData
    {
        public static void Init()
        {
            Settings = new();
        }
        
        public static Settings Settings { get; set; } = new();
    }

    public class Settings
    {
        public int DialogueSpeed = 4;
        public bool DevMode = true;
    }
}
