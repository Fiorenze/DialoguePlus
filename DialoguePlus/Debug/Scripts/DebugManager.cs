using UnityEngine;

namespace DialoguePlus.Debugging
{
    public class DebugManager : Singleton<DebugManager>
    {

        void Start()
        {
            Validate();
        }
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                DebugCanvas.SetActive(!DebugCanvas.activeSelf);
            }
        }

        private void Validate()
        {
            if (DialogueManager.Instance == null)
            {
                gameObject.SetActive(false);
                Debug.LogWarning("Dialogue manager not found, disabling debug menu!");
            }
            else
            {
                ScriptsObject.SetActive(true);
            }
        }

        public GameObject ScriptsObject;
        public GameObject DebugCanvas;
        public ValueModifyWindow valueModifyWindow;

        public void OpenValueModifyMenu(string valueKey)
        {
            valueModifyWindow.Init(valueKey);
            valueModifyWindow.gameObject.SetActive(true);
        }
    }
}

