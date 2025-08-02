using UnityEngine;
using UnityEngine.EventSystems;

namespace DialoguePlus
{
    public class DialogueManager : Singleton<DialogueManager>
    {
        [Tooltip("Starts from this label automatically. Leave empty to start manually")]
        [SerializeField] private string StartingScene;
        [Header("Files")]
        [SerializeField] private TextAsset[] DialogueFiles;
        [SerializeField] private TextAsset[] DefinitionFiles;

        public DialogueEngine DialogueEngine { get; private set; }

        void Start()
        {
            VariableDatabase.Init();
            SceneDatabase.Init();
            PersistentData.Init();

            foreach (TextAsset textAsset in DefinitionFiles)
            {
                FileReader.ReadDefinitionFile(textAsset);
            }
            foreach (TextAsset textAsset in DialogueFiles)
            {
                FileReader.ReadDialogueFile(textAsset);
            }

            DialogueEngine = new();

            if (!string.IsNullOrWhiteSpace(StartingScene)) StartScene(StartingScene);
        }
        private void Update()
        {
            InputUpdate();
        }

        public void StartScene(string sceneLabel)
        {
            DialogueEngine.StartScene(sceneLabel);
        }

        private void InputUpdate()
        {
            // Will separate input in the future

            if (!EventSystem.current.IsPointerOverGameObject(0))
            {
                if (!DialogueUI.Instance.IsWaitingForAnswer)
                {
                    if (Input.GetKeyDown(KeyCode.Mouse0))
                    {
                        DialogueEngine.AdvanceDialogue();
                    }
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        DialogueEngine.AdvanceDialogue();
                    }
                }
                if (Input.GetKeyDown(KeyCode.Q) || Input.mouseScrollDelta.y > 0)
                {
                    DialogueEngine.Rollback();
                }
            }


            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                PersistentData.Settings.DialogueSpeed = 1;
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                PersistentData.Settings.DialogueSpeed = 3;
            }
            if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                PersistentData.Settings.DialogueSpeed = 5;
            }

        }

    }
}

