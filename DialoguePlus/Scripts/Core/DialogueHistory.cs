using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace DialoguePlus
{
    public class DialogueHistory
    {
        public static UnityAction OnHistoryCleared;
        public static UnityAction<DialogueContext> OnAddedToHistory;
        public static UnityAction<DialogueContext> OnRemovedFromHistory;

        public int Count => History.Count;

        private List<DialogueContext> History = new List<DialogueContext>();
        private const int MaxHistorySize = 50;


        public DialogueHistory()
        {
            History = new();
        }

        public void Push(DialogueContext historyItem)
        {
            History.Add(historyItem);

            if (History.Count > MaxHistorySize)
            {
                Remove(History[0]);
            }

            OnAddedToHistory?.Invoke(historyItem);
        }

        public DialogueContext Pop()
        {
            DialogueContext last = History[^1];
            Remove(last);
            return last;
        }

        public void Clear()
        {
            History.Clear();
            OnHistoryCleared?.Invoke();
        }

        private void Remove(DialogueContext item)
        {
            History.Remove(item);
            OnRemovedFromHistory?.Invoke(item);
        }
    }
}
