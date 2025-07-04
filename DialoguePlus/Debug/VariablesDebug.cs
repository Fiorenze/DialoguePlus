using UnityEngine;
using DialoguePlus;

public class VariablesDebug : MonoBehaviour
{
    void Start()
    {
        VariableManager.OnVariablesUpdated += OnVariablesUpdated;
    }
    void OnDestroy()
    {
        VariableManager.OnVariablesUpdated -= OnVariablesUpdated;
    }


    [SerializeField] private GameObject variableObjectPrefab;
    [SerializeField] private Transform variablePanelTransform;

    private void OnVariablesUpdated()
    {
        foreach (Transform t in variablePanelTransform)
        {
            Destroy(t.gameObject);
        }

        foreach (VariableDefinition var in VariableManager.Variables)
        {
            InfoObject variableObject = Instantiate(variableObjectPrefab, variablePanelTransform.position, variablePanelTransform.rotation, variablePanelTransform).GetComponent<InfoObject>();

            string variableText = var.Key;

            variableText += " = " + var.Value.ToString();

            /* if (var.Value is string s)
            {
                variableText += " : " + s;
            }
            else if (var.Value is int i)
            {
                variableText = " : " + i.ToString();
            }
            else if (var.Value is bool b)
            {
                variableText = " : " + b.ToString();
            } */

            variableObject.Set(variableText, string.Empty);
        }

    }
}
