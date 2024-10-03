using UnityEngine;
using UnityEngine.SceneManagement;

public class Silvan : NPC, ITalkable
{
    [SerializeField] private DialogueText dialogueText;
    [SerializeField] private DialogueController dialogueController;

    private void OnEnable()
    {
        // DialogueController의 OnConversationEnd 이벤트 구독
        dialogueController.OnConversationEnd += HandleConversationEnd;
    }

    private void OnDisable()
    {
        // 구독 해제
        dialogueController.OnConversationEnd -= HandleConversationEnd;
    }

    public override void Interact()
    {
        Talk(dialogueText);
    }

    public void Talk(DialogueText dialogueText)
    {
        dialogueController.DisplayNextParagraph(dialogueText);
    }

    // 대화 종료 시
    private void HandleConversationEnd()
    {
        // 투명 물약 2개 지급
        Managers.Inventory.invinsibilityCnt += 2;
        Debug.Log(Managers.Inventory.invinsibilityCnt + "개");

        // Scene 전환
        SceneManager.LoadScene("2-1 to demon castle");
    }
}
