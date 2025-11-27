using UnityEngine;

public class DialogueTextPlayer : MonoBehaviour
{
    public DialogueText dialogueData;
    public DialogueControllerMulti uiController;

    public Speaker playerTemplate;
    private int currentIndex = -1;

    void Start()
    {
        currentIndex = -1;
        ShowNext();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            ShowNext();
        }
    }

    void ShowNext()
    {
        
        if (dialogueData == null || uiController == null)
        {
            Debug.LogError("DialogueTextPlayer: dialogueData 또는 uiController가 비어있습니다.");
            return;
        }
        
        currentIndex++;
        if (currentIndex >= dialogueData.paragraphs.Count)
        {
            uiController.HideAll();
            Debug.Log("대화 종료 ");
            return;
        }
        
        Speaker speaker = null;
        if (currentIndex < dialogueData.speakers.Count)
            speaker = dialogueData.speakers[currentIndex];

        string line = dialogueData.paragraphs[currentIndex];

        // 플레이어 라인을 null로 저장해둔 경우 → 템플릿 스피커 사용 + 이름 오버라이드
        if (speaker == null)
        {
            if (playerTemplate == null)
            {
                Debug.LogWarning("플레이어 템플릿이 없습니다. 센터 내레이션으로 대체합니다.");
                uiController.ShowDialogue(null, line); // 센터 처리
                return;
            }

            // 이름은 플레이어 시스템에서 가져오기 (예: Managers.Player.PlayerName)
            string playerName = Managers.Player != null ? Managers.Player.PlayerName : "플레이어";
            uiController.ShowDialogue(playerTemplate, line, playerName);
            return;
        }

        // 스피커가 '플레이어'라는 이름으로 저장된 경우에도 런타임 이름 오버라이드(옵션)
        if (speaker.speakerName == "플레이어")
        {
            string playerName = Managers.Player != null ? Managers.Player.PlayerName : "플레이어";
            uiController.ShowDialogue(speaker, line, playerName);
        }
        else
        {
            uiController.ShowDialogue(speaker, line);
        }
    }
}