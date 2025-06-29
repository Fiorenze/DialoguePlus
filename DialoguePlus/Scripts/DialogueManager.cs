using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DialoguePlus
{
    public class DialogueManager : MonoBehaviour
    {
        #region Singleton
        public static DialogueManager Instance { get; private set; }
        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;
            }
        }
        #endregion

        [SerializeField] private TextAsset[] DialogueFiles;
        [SerializeField] private TextAsset[] DefinitionFiles;
        [SerializeField] private DialogueUI dialogueUI;

        public DialogueEngine DialogueEngine { get; private set; }
        public DialogueReader DialogueReader { get; private set; }


        void Start()
        {
            DialogueEngine = new()
            {
                DialogueFiles = DialogueFiles,
                DefinitionFiles = DefinitionFiles
            };

            DialogueReader = new DialogueReader(dialogueUI);

            DialogueEngine.Init();
        }
        private void Update()
        {
            // Will separate input in the future
            if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Q))
            {
                if (!dialogueUI.IsWaitingForAnswer)
                    DialogueReader.Progress();
            }
        }

        void OnDestroy()
        {
            DialogueReader.OnDestroy();
        }
    }
}

