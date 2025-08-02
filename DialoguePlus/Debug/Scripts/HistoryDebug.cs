using System.Collections.Generic;
using UnityEngine;

namespace DialoguePlus.Debugging
{
    public class HistoryDebug : MonoBehaviour
    {
        [SerializeField] private GameObject historyObjectPrefab;
        [SerializeField] private Transform historyObjectParent;

        private Dictionary<DialogueContext, GameObject> objectsInHistory;

        void Awake()
        {
            objectsInHistory = new();
            DialogueHistory.OnAddedToHistory += OnAddedToHistory;
            DialogueHistory.OnRemovedFromHistory += OnRemovedFromHistory;
            DialogueHistory.OnHistoryCleared += OnHistoryCleared;
            ClearHistoryPanel();
        }
        void OnDestroy()
        {
            DialogueHistory.OnHistoryCleared -= OnHistoryCleared;
            DialogueHistory.OnAddedToHistory -= OnAddedToHistory;
            DialogueHistory.OnRemovedFromHistory -= OnRemovedFromHistory;
        }

        void ClearHistoryPanel()
        {
            foreach (Transform t in historyObjectParent)
            {
                Destroy(t.gameObject);
            }
        }

        private void OnAddedToHistory(DialogueContext historyItem)
        {
            InfoObject historyObject = Instantiate(historyObjectPrefab, historyObjectParent).GetComponent<InfoObject>();

            //string info = historyItem.ContextWhenExecuted?.Branch.ToString() + " " + historyItem.ContextWhenExecuted?.IndexInBranch;
            string displayText = historyItem.Node.Info;

            historyObject.Set(displayText);
            objectsInHistory.Add(historyItem, historyObject.gameObject);
        }
        private void OnRemovedFromHistory(DialogueContext historyItem)
        {
            if (objectsInHistory.ContainsKey(historyItem))
            {
                Destroy(objectsInHistory[historyItem]);
                objectsInHistory.Remove(historyItem);
            }
        }

        private void OnHistoryCleared()
        {
            ClearHistoryPanel();
        }
    }

}
