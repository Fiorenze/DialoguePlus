using UnityEngine;

namespace DialoguePlus.Debugging
{
    public class VariablesDebug : MonoBehaviour
    {
        void Awake()
        {
            UpdateVariableTable();
            VariableDatabase.OnVariablesUpdated += UpdateVariableTable;
        }
        void OnDestroy()
        {
            VariableDatabase.OnVariablesUpdated -= UpdateVariableTable;
        }

        [SerializeField] private GameObject variableObjectPrefab;
        [SerializeField] private Transform variablePanelTransform;

        private void UpdateVariableTable()
        {
            foreach (Transform t in variablePanelTransform)
            {
                Destroy(t.gameObject);
            }

            foreach (Variable var in VariableDatabase.Variables)
            {
                VariableUIObject variableObject = Instantiate(variableObjectPrefab, variablePanelTransform.position, variablePanelTransform.rotation, variablePanelTransform).GetComponent<VariableUIObject>();

                variableObject.Set(var.Key, var.Value);
            }
        }
    }
}

