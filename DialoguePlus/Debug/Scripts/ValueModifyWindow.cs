using TMPro;
using UnityEngine;

namespace DialoguePlus.Debugging
{
    public class ValueModifyWindow : MonoBehaviour
    {
        [SerializeField] TMP_InputField variableInput;
        [SerializeField] TMP_Text VariableTypeText;
        [SerializeField] TMP_Text BooleanChangeButtonText;
        private string modifiedVariableKey;

        object variableToModify;
        bool tempBool = true;

        public void Init(string ValueKey)
        {
            modifiedVariableKey = ValueKey;
            variableToModify = VariableDatabase.GetVariableValue(ValueKey);
            variableInput.text = variableToModify.ToString();

            if (variableToModify is int)
            {
                VariableTypeText.text = "integer";
                variableInput.gameObject.SetActive(true);
                BooleanChangeButtonText.transform.parent.gameObject.SetActive(false);
                variableInput.contentType = TMP_InputField.ContentType.IntegerNumber;
            }
            else if (variableToModify is bool b)
            {
                VariableTypeText.text = "boolean";

                tempBool = b;
                BooleanChangeButtonText.text = tempBool ? "True" : "False";

                variableInput.gameObject.SetActive(false);
                BooleanChangeButtonText.transform.parent.gameObject.SetActive(true);
            }
            else
            {
                VariableTypeText.text = "string";

                variableInput.gameObject.SetActive(true);
                BooleanChangeButtonText.transform.parent.gameObject.SetActive(false);
                variableInput.contentType = TMP_InputField.ContentType.Standard;
            }
        }

        public void ChangeBoolValue()
        {
            tempBool = !tempBool;
            BooleanChangeButtonText.text = tempBool ? "True" : "False";
        }
        public void OnChange()
        {
            if (variableToModify is int)
            {
                VariableDatabase.SetVariableValue(modifiedVariableKey, int.Parse(variableInput.text));
            }
            else if (variableToModify is bool)
            {
                VariableDatabase.SetVariableValue(modifiedVariableKey, tempBool);
            }
            else if (variableToModify is string)
            {
                VariableDatabase.SetVariableValue(modifiedVariableKey, variableInput.text);
            }
        }
    }
}

