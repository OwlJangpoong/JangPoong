using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DialogueController_Version2 : MonoBehaviour
{
    // 대화 종료 이벤트
    public event Action OnConversationEnd;

    [SerializeField] private TextMeshProUGUI NPCNameText;
    [SerializeField] private TextMeshProUGUI NPCDialogueText;
    [SerializeField] private Image NPCImage;
    [SerializeField] private float typeSpeed = 15;

    private Queue<string> paragraphs = new Queue<string>();
    private Queue<Speaker> speakers = new Queue<Speaker>();

    public GameObject playerUI;

    private bool conversationEnded;
    private bool isTyping;
    private bool isWaitingForClick;
    private bool dialoguePlayed;

    private string currentParagraph;
    private Speaker currentSpeaker;

    private Coroutine typeDialogueCoroutine;

    private const string HTML_ALPHA = "<color=#00000000>";
    private const float MAX_TYPE_TIME = 0.1f;

    private void Start()
    {
        playerUI.SetActive(false);
    }

    private void Update()
    {
        if (!gameObject.activeSelf) return; // 대화 UI가 꺼져 있으면 입력 받지 않음

        // ✅ 타이핑이 끝난 상태에서 마우스 클릭하면 다음 문장 출력
        if (isWaitingForClick && Input.GetMouseButtonDown(0))
        {
            isWaitingForClick = false; // 다시 대기 상태 해제
            DisplayNextParagraph();
        }
    }

    public void StartDialogue(DialogueText dialogueText)
    {
        if (!gameObject.activeSelf)
        {
            gameObject.SetActive(true);
        }

        // 기존 대화 내용 초기화
        paragraphs.Clear();
        speakers.Clear();

        for (int i = 0; i < dialogueText.paragraphs.Length; i++)
        {
            paragraphs.Enqueue(dialogueText.paragraphs[i]);
            speakers.Enqueue(dialogueText.speakers[i]);
        }

        conversationEnded = false;
        dialoguePlayed = false;
        
        // ✅ 첫 문장 자동 출력 (사용자가 클릭할 필요 없음)
        DisplayNextParagraph();
    }

    public void DisplayNextParagraph()
    {
        if (dialoguePlayed) return;

        // ✅ 대화가 끝났으면 종료
        if (paragraphs.Count == 0 && !isTyping)
        {
            EndConversation();
            return;
        }

        // ✅ 타이핑 중이면 즉시 문장 완성
        if (isTyping)
        {
            FinishParagraphEarly();
            return;
        }

        // ✅ 다음 문장 출력
        if (paragraphs.Count > 0)
        {
            currentParagraph = paragraphs.Dequeue();
            currentSpeaker = speakers.Dequeue();

            string speakerName = string.IsNullOrEmpty(currentSpeaker.speakerName)
                ? Managers.Player.PlayerName
                : currentSpeaker.speakerName;

            NPCNameText.text = speakerName;
            
            
            
            NPCImage.sprite = currentSpeaker.characterImage;

            if (gameObject.activeSelf)
            {
                typeDialogueCoroutine = StartCoroutine(TypeDialogueText(currentParagraph));
            }
        }
    }

    private void EndConversation()
    {
        paragraphs.Clear();
        speakers.Clear();
        conversationEnded = true;
        dialoguePlayed = true;

        // ✅ 대화가 정말 끝났을 때만 실행
        OnConversationEnd?.Invoke();

        if (gameObject.activeSelf)
        {
            gameObject.SetActive(false);
        }
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
        isWaitingForClick = true; // ✅ 타이핑이 끝났으니 클릭을 기다림
    }

    private void FinishParagraphEarly()
    {
        StopCoroutine(typeDialogueCoroutine);

        NPCDialogueText.text = currentParagraph;
        isTyping = false;
        isWaitingForClick = true; // ✅ 클릭 대기 상태로 변경
    }
}
