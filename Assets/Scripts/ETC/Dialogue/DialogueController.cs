using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{

    // 대화 종료 이벤트 선언
    public event Action OnConversationEnd;

    [SerializeField] private TextMeshProUGUI NPCNameText;
    [SerializeField] private TextMeshProUGUI NPCDialogueText;
    [SerializeField] private Image NPCImage;
    [SerializeField] private float typeSpeed = 15;
    private Queue<string> paragraphs = new Queue<string>();
    private Queue<Speaker> speakers = new Queue<Speaker>();

    public GameObject playerUI;

    public void Start()
    {
        playerUI.SetActive(false);
    }

    private bool conversationEnded;
    public bool isTyping;
    private bool dialoguePlayed;

    private string currentParagraph;
    private Speaker currentSpeaker;

    private Coroutine typeDialogueCoroutine;

    private const string HTML_ALPHA = "<color=#00000000>";
    private const float MAX_TYPE_TIME = 0.1f;

    public void DisplayNextParagraph(DialogueText dialogueText)
    {

        if (dialoguePlayed)
        {
            return;
        }

        //if there is nothing in the queue
        if (paragraphs.Count == 0)
        {
            if (!conversationEnded)
            {
                //start a conversation
                StartConversation(dialogueText);
            }
            else if (conversationEnded && !isTyping)
            {
                //end a conversation
                EndConversation();
                return;
            }
        }

        //If there is something in the queue
        if (!isTyping)
        {
            currentParagraph = paragraphs.Dequeue();
            currentSpeaker = speakers.Dequeue();

            NPCNameText.text = currentSpeaker.speakerName;
            NPCImage.sprite = currentSpeaker.characterImage;

            typeDialogueCoroutine = StartCoroutine(TypeDialogueText(currentParagraph));
        }
        //conversation is being typed out
        else
        {
            FinishParagraphEarly();
        }

        //update conversationEnded bool
        if (paragraphs.Count == 0)
        {
            conversationEnded = true;
            dialoguePlayed = true;
            EndConversation();
        }
    }

    private void StartConversation(DialogueText dialogueText)
    {
        //activate gameObject
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        //add dialogue text to the queue
        for (int i = 0; i < dialogueText.paragraphs.Length; i++)
        {
            paragraphs.Enqueue(dialogueText.paragraphs[i]);
            speakers.Enqueue(dialogueText.speakers[i]);
        }
    }

    private void EndConversation()
    {
        //clear the queue
        paragraphs.Clear();
        speakers.Clear();
        //return bool to false
        conversationEnded = false;

        // 대화 종료 이벤트 호출
        OnConversationEnd?.Invoke();

        //deactivate gameobject
        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }

        // Debug.Log("대화 종료");
    }

    private IEnumerator TypeDialogueText(string text)
    {
        isTyping = true;

        NPCDialogueText.text = "";

        string originalText = text;
        string displayedText = "";
        int alphaIndex = 0;
        foreach (char c in text.ToCharArray())
        {
            alphaIndex++;
            NPCDialogueText.text = originalText;

            displayedText = NPCDialogueText.text.Insert(alphaIndex, HTML_ALPHA);

            NPCDialogueText.text = displayedText;

            yield return new WaitForSeconds(MAX_TYPE_TIME / typeSpeed);
        }

        isTyping = false;
    }

    private void FinishParagraphEarly()
    {
        //stop the coroutine
        StopCoroutine(typeDialogueCoroutine);

        //finish displaying the text
        NPCDialogueText.text = currentParagraph;

        //update isTyping bool
        isTyping = false;
    }
}
