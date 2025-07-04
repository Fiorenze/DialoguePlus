using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

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
        public DialogueUI dialogueUI;

        public DialogueEngine DialogueEngine { get; private set; }


        void Start()
        {
            VariableManager.Init();
            SceneManager.Init();

            foreach (TextAsset textAsset in DefinitionFiles)
            {
                FileReader.ReadDefinitionFile(textAsset);
            }
            foreach (TextAsset textAsset in DialogueFiles)
            {
                FileReader.ReadDialogueFile(textAsset);
            }

            DialogueEngine = new();
            DialogueEngine.Init();

        }
        private void Update()
        {
            // Will separate input in the future

            if (!EventSystem.current.IsPointerOverGameObject(0))
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (!dialogueUI.IsWaitingForAnswer)
                        DialogueEngine.AdvanceDialogue();
                }
            }
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (!dialogueUI.IsWaitingForAnswer)
                    DialogueEngine.AdvanceDialogue();
            }
            if (Input.GetKeyDown(KeyCode.Q) || Input.mouseScrollDelta.y > 0)
            {
                DialogueEngine.Rollback();
            }
        }

    }
}

