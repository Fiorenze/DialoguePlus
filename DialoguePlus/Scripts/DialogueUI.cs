using System.Collections;
using DialoguePlus;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DialoguePlus
{
    public class DialogueUI : MonoBehaviour
    {

        [Header("Text")]
        public GameObject DialoguePanel;
        public TMP_Text NameText;
        public TMP_Text DialogueText;

        [Header("Question")]
        public GameObject QuestionPanel;
        public GameObject[] OptionButtons;

        public bool IsWaitingForAnswer => QuestionPanel.activeSelf;

        private Coroutine TextWriter;

        void Start()
        {
            DialogueEngine.Instance.OnDialogueEnd += EndDialogue;
        }
        void OnDestroy()
        {
            DialogueEngine.Instance.OnDialogueEnd -= EndDialogue;
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
                TextWriter = StartCoroutine(TypeSentence(n.Speaker, n.DisplayText));
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

            foreach (char letter in sentence.ToCharArray())
            {
                DialogueText.text += letter;
                yield return null;
            }
        }

        private MenuNode currentMenuNode;
        private void DisplayQuestionPanel(MenuNode node)
        {
            currentMenuNode = node;

            QuestionPanel.SetActive(true);
            DialoguePanel.SetActive(false);

            foreach (GameObject button in OptionButtons)
            {
                button.SetActive(false);
            }

            for (int i = 0; i < node.Options.Count; i++)
            {
                OptionButtons[i].SetActive(true);
                OptionButtons[i].GetComponent<Button>().interactable = true;
                OptionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = node.Options[i].Text;

                if (node.Options[i].SoftConditions.Count > 0)
                {
                    if (!DialogueEngine.Instance.CheckSoftConditions(node.Options[i].SoftConditions))
                    {
                        OptionButtons[i].GetComponent<Button>().interactable = false;
                    }
                }
                else if (node.Options[i].HardConditions.Count > 0)
                {
                    if (!DialogueEngine.Instance.CheckHardConditions(node.Options[i].HardConditions))
                    {
                        OptionButtons[i].GetComponent<Button>().interactable = false;
                    }
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
    }
}

