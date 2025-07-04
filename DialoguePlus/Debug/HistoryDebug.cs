using UnityEngine;
using DialoguePlus;
using TMPro;
using UnityEngine.UI;

public class HistoryDebug : MonoBehaviour
{
    private DialogueEngine dialogueEngine;

    [SerializeField] private TMP_Text DebugText;

    void Start()
    {
        Invoke(nameof(Init), .01f);
    }

    void Init()
    {
        dialogueEngine = DialogueManager.Instance.DialogueEngine;

        dialogueEngine.OnHistoryChanged += OnHistoryChanged;
    }

    void OnDestroy()
    {
        dialogueEngine.OnHistoryChanged -= OnHistoryChanged;
    }

    [SerializeField] private GameObject HistoryObjectPrefab;
    [SerializeField] private Transform historyPanelTransform;

    private void OnHistoryChanged()
    {
        UpdateHistory();
        UpdateDebugText();
    }

    private void UpdateHistory()
    {
        foreach (Transform t in historyPanelTransform)
        {
            Destroy(t.gameObject);
        }

        foreach (HistoryItem item in dialogueEngine.History)
        {
            InfoObject historyObject = Instantiate(HistoryObjectPrefab, historyPanelTransform.position, historyPanelTransform.rotation, historyPanelTransform).GetComponent<InfoObject>();
            string historyText;

            string info = item.ContextWhenExecuted?.Branch.ToString() + " " + item.ContextWhenExecuted?.IndexInBranch;

            if (item.Node is DialogueLineNode dln)
            {
                if (dln.Text.Length < 25)
                    historyText = dln.Text;
                else
                    historyText = dln.Text[..25] + "...";
            }
            else if (item.Node is MenuNode menunode)
            {
                historyText = $"Menu with {menunode.Options.Count} options";
            }
            else if (item.Node is IfBlockNode ifnode)
            {
                historyText = $"If with {ifnode.DialogueNodeBranches.Count} branches";
            }
            else if (item.Node is CommandActionNode commandnode)
            {
                historyText = commandnode.Text + " " + commandnode.Command;
            }
            else if (item.Node is VariableActionNode varnode)
            {
                historyText = varnode.Variable + " " + varnode.Operation + " " + varnode.Modifier;
            }
            else
            {
                historyText = item.Node.ToString();
            }

            historyObject.Set(historyText, info);
        }
    }

    private void UpdateDebugText()
    {
        DebugText.text = $"Idx: {dialogueEngine.NodeIndex}\nBIdx: {dialogueEngine.CurrentBranch?.IndexInBranch}";
    }
}
