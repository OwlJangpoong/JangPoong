using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUIController : MonoBehaviour
{
    public GameObject leftBubble;
    public GameObject rightBubble;
    public GameObject centerBubble;

    public void ShowDialogue(Speaker speaker, string text)
    {
        HideAll();

        GameObject bubble = GetBubble(speaker?.position ?? SpeakerPosition.Center);

        var nameText = bubble.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        var dialogueText = bubble.transform.Find("SpeechText").GetComponent<TextMeshProUGUI>();
        var image = bubble.transform.Find("Portrait").GetComponent<Image>();

        nameText.text = speaker ? speaker.speakerName : "";
        dialogueText.text = text;
        image.sprite = speaker ? speaker.characterImage : null;

        bubble.SetActive(true);
    }

    public void HideAll()
    {
        leftBubble.SetActive(false);
        rightBubble.SetActive(false);
        centerBubble.SetActive(false);
    }

    private GameObject GetBubble(SpeakerPosition pos)
    {
        switch (pos)
        {
            case SpeakerPosition.Left: return leftBubble;
            case SpeakerPosition.Right: return rightBubble;
            case SpeakerPosition.Center: return centerBubble;
            default: return centerBubble;
        }
    }
}