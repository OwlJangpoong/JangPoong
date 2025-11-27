using DG.Tweening;
using System.Collections;
using UnityEngine;

public class NokmorDieState : MonsterDieState
{
    [SerializeField] private string endingSceneName = "2-3-2 inside the castle";
    [SerializeField] private float victoryDelaySec = 1.2f; // 팬파레어 조금 재생 후 이동
    [SerializeField] private bool useRealtimeWait = true;  // 타임스케일 0 가능성 대비

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        monster = animator.GetComponentInParent<Monster>();
        monsterTransform = monster.transform;
        monster.FlipSprite();

        // 1) 전투 종료(벽 내림, 승리음 등)
        var triggerRoot = GameObject.FindWithTag("Trigger");
        var battle = triggerRoot ? triggerRoot.GetComponentInChildren<TriggerForBattle>(true) : null;
        if (battle != null)
        {
            Debug.Log("[NokmorDieState] EndBattle 호출");
            battle.EndBattle();
        }
        else
        {
            Debug.LogWarning("[NokmorDieState] TriggerForBattle 못 찾음 (Tag 'Trigger' 오브젝트/구성 확인)");
        }

        // 2) 엔딩 씬으로 이동 코루틴
        monster.StartCoroutine(CoGoToEnding());
    
    }

    private IEnumerator CoGoToEnding()
    {
        // 팬파레 후 잠깐 대기
        if (victoryDelaySec > 0f)
        {
            if (useRealtimeWait) yield return new WaitForSecondsRealtime(victoryDelaySec);
            else yield return new WaitForSeconds(victoryDelaySec);
        }

        // 페이드 컨트롤러 찾기
        var uiRoot = GameObject.FindWithTag("UI_Root");
        var fade = uiRoot ? uiRoot.GetComponentInChildren<UI_FadeController>(true) : null;

        if (fade != null)
        {
            Debug.Log("[NokmorDieState] FadeOut → 콜백으로 씬 전환");
            fade.RegisterCallback(() =>
            {
                Debug.Log($"[NokmorDieState] Managers.Scene.LoadScene(\"{endingSceneName}\")");
                Managers.Scene.LoadScene(endingSceneName);
            });
            fade.FadeOut();
        }
        else
        {
            Debug.LogWarning("[NokmorDieState] UI_FadeController 없음 → 즉시 씬 전환");
            Managers.Scene.LoadScene(endingSceneName);
        }
    }
}