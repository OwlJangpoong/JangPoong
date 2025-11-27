using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueControllerLite : MonoBehaviour
{
    public TextMeshProUGUI npcNameText;
    public TextMeshProUGUI npcDialogueText;
    public Image npcImage;
    public GameObject dialogueUI;

    public float typeSpeed = 0.03f;

    private Queue<string> paragraphs = new Queue<string>();
    private Queue<Speaker> speakers = new Queue<Speaker>();

    private bool isTyping = false;
    private Coroutine typingCoroutine;
    private string currentParagraph;

    public System.Action OnDialogueEnd;

    public void StartDialogue(DialogueText dialogueData)
    {
        paragraphs.Clear();
        speakers.Clear();

        for (int i = 0; i < dialogueData.paragraphs.Count; i++)
        {
            paragraphs.Enqueue(dialogueData.paragraphs[i]);
            speakers.Enqueue(dialogueData.speakers[i]);
        }

        dialogueUI.SetActive(true);
        DisplayNextParagraph();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (isTyping)
            {
                StopCoroutine(typingCoroutine);
                npcDialogueText.text = currentParagraph;
                isTyping = false;
            }
            else
            {
                DisplayNextParagraph();
            }
        }
    }

    public void DisplayNextParagraph()
    {
        if (paragraphs.Count == 0)
        {
            dialogueUI.SetActive(false);
            OnDialogueEnd?.Invoke();
            return;
        }

        currentParagraph = paragraphs.Dequeue();
        Speaker currentSpeaker = speakers.Dequeue();

        npcNameText.text = currentSpeaker ? currentSpeaker.speakerName : "플레이어";
        npcImage.sprite = currentSpeaker ? currentSpeaker.characterImage : null;

        typingCoroutine = StartCoroutine(TypeText(currentParagraph));
    }

    private IEnumerator TypeText(string text)
    {
        isTyping = true;
        npcDialogueText.text = "";

        foreach (char c in text)
        {
            npcDialogueText.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }

        isTyping = false;
    }
}