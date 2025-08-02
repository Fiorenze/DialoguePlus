using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DialoguePlus
{
    public class DialogueUI : Singleton<DialogueUI>
    {

        [Header("Text")]
        public GameObject DialoguePanel;
        public TMP_Text NameText;
        public TMP_Text DialogueText;

        [Header("Question")]
        public GameObject QuestionPanel;
        public GameObject OptionButtonPrefab;
        public Transform OptionButtonParent;

        public bool IsWaitingForAnswer => QuestionPanel.activeSelf;

        private Coroutine TextWriter;

        protected override void Awake()
        {
            base.Awake();
            
            DialogueEngine.OnDisplaySentence += DisplaySentence;
            DialogueEngine.OnDialogueEnd += EndDialogue;
        }
        void OnDestroy()
        {
            DialogueEngine.OnDisplaySentence -= DisplaySentence;
            DialogueEngine.OnDialogueEnd -= EndDialogue;
        }

        public void DisplaySentence(DialogueNode node)
        {
            QuestionPanel.SetActive(false);
            DialoguePanel.SetActive(true);

            if (TextWriter != null)
            {
                StopCoroutine(TextWriter);
            }

            if (node is DialogueLineNode n)
            {
                TextWriter = StartCoroutine(TypeSentence(n.DisplayName, n.DisplayText));
                NameText.color = n.SpeakerColor;
            }
            else if (node is MenuNode m)
            {
                DisplayQuestionPanel(m);
            }
        }

        private IEnumerator TypeSentence(string _name, string sentence)
        {
            DialogueText.text = "";
            NameText.text = _name;

            char[] sentenceArray = sentence.ToCharArray();
            int arrayLength = sentenceArray.Length;
            int index = 0;
            int readSpeed = Mathf.Clamp(PersistentData.Settings.DialogueSpeed, 1, 5);

            if (readSpeed == 5)
            {
                DialogueText.text = sentence;
                yield break;
            }

            while (index < arrayLength)
            {
                for (int i = 0; i < readSpeed; i++)
                {
                    if (index >= arrayLength)
                        break;

                    DialogueText.text += sentenceArray[index];
                    index++;
                }
                yield return null;
            }
        }

        #region Question

        private MenuNode currentMenuNode;
        private void DisplayQuestionPanel(MenuNode node)
        {
            foreach (Transform t in OptionButtonParent)
            {
                Destroy(t.gameObject);
            }

            currentMenuNode = node;

            QuestionPanel.SetActive(true);
            DialoguePanel.SetActive(false);

            for (int i = 0; i < node.Options.Count; i++)
            {
                int index = i;

                GameObject optionButton = Instantiate(OptionButtonPrefab, OptionButtonParent);
                optionButton.GetComponentInChildren<TextMeshProUGUI>().text = node.Options[i].Text;
                optionButton.GetComponent<Button>().onClick.AddListener(() => AnswerQuestion(index));


                if (!string.IsNullOrEmpty(node.Options[index].Condition))
                {
                    if (!ConditionEvaluator.CheckCondition(node.Options[index].Condition))
                        optionButton.GetComponent<Button>().interactable = false;
                }
            }
        }

        public void AnswerQuestion(int optionnum)
        {
            if (!QuestionPanel.activeSelf) return;

            QuestionPanel.SetActive(false);
            DialoguePanel.SetActive(false);

            currentMenuNode.ChooseOption(optionnum);
        }

        public void EndDialogue()
        {
            DialoguePanel.SetActive(false);
            QuestionPanel.SetActive(false);
        }

        #endregion


    }
}

