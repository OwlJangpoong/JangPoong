using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class DialogueRunnerSimple : MonoBehaviour
{
    [Header("Required")]
    public DialogueControllerMulti ui;   // DialogueControllerMulti 참조 (IntroCanvas 등)
    public DialogueText dialogue;        // 재생할 대사 데이터

    [Header("Options")]
    public bool autoStart = true;        // 씬 시작 시 자동 재생
    public string nextScene = "";        // 대사 끝나면 로드할 씬명 (비우면 로드 안 함)
    public bool allowMouseClick = true;  // 마우스 클릭으로 넘기기
    public bool allowSpaceEnter = true;  // Space/Enter로 넘기기
    public string playerNameOverride = ""; // speaker == null일 때 이름 표시 (없으면 이름 비움)

    [Header("When Dialogue Ends")]
    public UnityEvent onDialogueEnd;     // 끝났을 때 실행할 추가 동작(선택)

    int idx = -1;
    bool running = false;

    void Start()
    {
        if (autoStart) Begin();
    }

    void Update()
    {
        if (!running) return;

        if (allowMouseClick && Input.GetMouseButtonDown(0))
            Advance();

        if (allowSpaceEnter && (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)))
            Advance();
    }

    /// <summary>외부(UI 버튼 등)에서 호출 가능</summary>
    public void Begin()
    {
        if (!ui || !dialogue)
        {
            Debug.LogWarning("[DialogueRunnerSimple] ui 또는 dialogue가 비어있습니다.");
            return;
        }
        idx = -1;
        running = true;
        Advance();
    }

    /// <summary>외부(UI 버튼 등)에서 호출 가능</summary>
    public void Advance()
    {
        if (!running) return;

        idx++;

        // 모두 끝났으면 종료 처리
        if (idx >= dialogue.paragraphs.Count)
        {
            running = false;
            ui.HideAll();

            // 1) UnityEvent
            onDialogueEnd?.Invoke();

            // 2) 다음 씬 로드 (세팅되어 있으면)
            if (!string.IsNullOrEmpty(nextScene))
            {
                SceneManager.LoadScene(nextScene);
            }
            return;
        }

        // 줄 재생
        var sp = dialogue.speakers[idx];  // null이면 내레이션/플레이어
        var tx = dialogue.paragraphs[idx];

        if (sp == null && !string.IsNullOrEmpty(playerNameOverride))
            ui.ShowDialogue(null, tx, playerNameOverride);
        else
            ui.ShowDialogue(sp, tx);
    }
}