using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class BeforeBossFightDialogue : MonoBehaviour
{
    [Header("다이얼로그 씬 이름")]
    [SerializeField] private string dialogueSceneName = "BossDialogueScene";

    private bool used;

    private void Reset()
    {
        // 트리거 강제
        GetComponent<Collider2D>().isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (used || !other.CompareTag("Player")) return;
        used = true;

        // 페이드아웃 후 씬 전환
        var fade = GameObject.FindWithTag("UI_Root")?.GetComponentInChildren<UI_FadeController>(true);
        if (fade != null)
        {
            fade.RegisterCallback(() =>
            {
                Managers.Scene.LoadScene(dialogueSceneName);
            });
            fade.FadeOut();
        }
        else
        {
            Managers.Scene.LoadScene(dialogueSceneName);
        }
    }
}