using TMPro;
using UnityEngine;

namespace DialoguePlus.Debugging
{
    public class VariableUIObject : MonoBehaviour
    {
        [SerializeField] private TMP_Text VariableNameText;
        [SerializeField] private TMP_Text VariableValueText;

        private string variableName;

        public void Set(string name, object value)
        {
            variableName = name;
            VariableNameText.text = name;
            VariableValueText.text = value.ToString();
        }

        public void OpenValueModifyMenu()
        {
            DebugManager.Instance.OpenValueModifyMenu(variableName);
        }
    }

}
