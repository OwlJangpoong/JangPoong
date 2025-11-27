using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueControllerMulti : MonoBehaviour
{
    [Header("버블 오브젝트")]
    public GameObject leftBubble;
    public GameObject rightBubble;
    public GameObject centerBubble;

    [Header("버블 안의 UI")]
    public Image leftImage;
    public TextMeshProUGUI leftNameText;
    public TextMeshProUGUI leftDialogueText;

    public Image rightImage;
    public TextMeshProUGUI rightNameText;
    public TextMeshProUGUI rightDialogueText;

    public TextMeshProUGUI centerDialogueText;

    [Header("타이핑 속도")]
    public float typeSpeed = 0.03f;

    private Coroutine typingRoutine;

    // ✅ 이름 오버라이드 매개변수 추가 (기본값 null)
    public void ShowDialogue(Speaker speaker, string text, string speakerNameOverride = null)
    {
        // 모두 끄기 (널가드)
        if (leftBubble)  leftBubble.SetActive(false);
        if (rightBubble) rightBubble.SetActive(false);
        if (centerBubble) centerBubble.SetActive(false);

        // 타이핑 중단
        if (typingRoutine != null)
        {
            StopCoroutine(typingRoutine);
            typingRoutine = null;
        }

        // speaker 자체가 null이면 내레이션처럼 센터 버블로 처리
        if (speaker == null)
        {
            if (centerBubble) centerBubble.SetActive(true);
            if (centerDialogueText) typingRoutine = StartCoroutine(TypeText(centerDialogueText, text));
            return;
        }

        string displayName = !string.IsNullOrEmpty(speakerNameOverride) ? speakerNameOverride : speaker.speakerName;

        switch (speaker.position)
        {
            case SpeakerPosition.Left:
                if (leftBubble) leftBubble.SetActive(true);
                if (leftImage)
                {
                    leftImage.sprite = speaker.characterImage;
                    leftImage.enabled = (speaker.characterImage != null);
                }
                if (leftNameText) leftNameText.text = displayName;
                if (leftDialogueText) typingRoutine = StartCoroutine(TypeText(leftDialogueText, text));
                break;

            case SpeakerPosition.Right:
                if (rightBubble) rightBubble.SetActive(true);
                if (rightImage)
                {
                    rightImage.sprite = speaker.characterImage;
                    rightImage.enabled = (speaker.characterImage != null);
                }
                if (rightNameText) rightNameText.text = displayName;
                if (rightDialogueText) typingRoutine = StartCoroutine(TypeText(rightDialogueText, text));
                break;

            case SpeakerPosition.Center:
                if (centerBubble) centerBubble.SetActive(true);
                if (centerDialogueText) typingRoutine = StartCoroutine(TypeText(centerDialogueText, text));
                break;
        }
    }

    public void HideAll()
    {
        if (leftBubble)  leftBubble.SetActive(false);
        if (rightBubble) rightBubble.SetActive(false);
        if (centerBubble) centerBubble.SetActive(false);
    }

    private IEnumerator TypeText(TextMeshProUGUI target, string text)
    {
        if (!target) yield break;

        target.text = "";
        foreach (char c in text)
        {
            target.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
    }
    
    public void EndDialogueAndGoNext(string nextSceneName)
    {
        HideAll();
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
    
}