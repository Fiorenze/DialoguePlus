using TMPro;
using UnityEngine;

public class InfoObject : MonoBehaviour
{
    string InfoText;
    public void Set(string newtext, string info)
    {
        TMP_Text text = GetComponentInChildren<TMP_Text>();
        text.text = newtext;

        InfoText = info;
    }

    public void DebugInfo()
    {
        if (!string.IsNullOrEmpty(InfoText))
            Debug.Log(InfoText);
    }
}
