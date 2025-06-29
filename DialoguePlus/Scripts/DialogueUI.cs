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

        public void DisplaySentence(DialogueNode node)
        {
            DialoguePanel.SetActive(true);

            if (TextWriter != null)
            {
                StopCoroutine(TextWriter);
            }

            TextWriter = StartCoroutine(TypeSentence(node.Speaker, node.Text));
            NameText.color = node.SpeakerColor;

            if (node.Menu.Options != null && node.Menu.Options.Count > 0)
            {
                DisplayQuestionPanel(node);
            }
        }

        private IEnumerator TypeSentence(string _name, string sentence)
        {
            if (sentence == "endDialogue")
            {
                EndDialogue();
                yield break;
            }

            DialogueText.text = "";
            NameText.text = _name;

            foreach (char letter in sentence.ToCharArray())
            {
                DialogueText.text += letter;
                yield return null;
            }
        }

        private DialogueNode currentMenuNode;
        private void DisplayQuestionPanel(DialogueNode node)
        {
            currentMenuNode = node;

            QuestionPanel.SetActive(true);
            DialoguePanel.SetActive(false);

            foreach (GameObject button in OptionButtons)
            {
                button.SetActive(false);
            }

            for (int i = 0; i < node.Menu.Options.Count; i++)
            {
                OptionButtons[i].SetActive(true);
                OptionButtons[i].GetComponent<Button>().interactable = true;
                OptionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = node.Menu.Options[i].Text;

                if (node.Menu.Options[i].SoftConditions.Count > 0)
                {
                    if (!DialogueEngine.Instance.CheckSoftConditions(node.Menu.Options[i].SoftConditions))
                    {
                        OptionButtons[i].GetComponent<Button>().interactable = false;
                    }
                }
                else if (node.Menu.Options[i].HardConditions.Count > 0)
                {
                    if (!DialogueEngine.Instance.CheckHardConditions(node.Menu.Options[i].HardConditions))
                    {
                        OptionButtons[i].GetComponent<Button>().interactable = false;
                    }
                }
            }
        }

        public void AnswerQuestion(int optionnum)
        {
            if (!QuestionPanel.activeSelf) return;

            foreach (string action in currentMenuNode.Menu.Options[optionnum].Actions)
            {
                DialogueEngine.Instance.ExecuteAction(action);
            }

            QuestionPanel.SetActive(false);
            DialoguePanel.SetActive(true);

            DialogueManager.Instance.DialogueReader.Progress();
        }

        public void EndDialogue()
        {
            DialoguePanel.SetActive(false);
            QuestionPanel.SetActive(false);
        }
    }
}

