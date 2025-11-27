using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D), typeof(Animator), typeof(SpriteRenderer))]
public class DarkEnergyPillar : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private BoxCollider2D hitCol;     // IsTrigger = true
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Animator anim;

    [Header("Damage")]
    [SerializeField] private LayerMask playerMask;     // Player 레이어 지정(없으면 비워도 Overlap 결과로 필터)
    private float dps;
    private float tickInterval = 0.25f;

    private float lifeSeconds;

    private void Reset()
    {
        hitCol = GetComponent<BoxCollider2D>();
        sr     = GetComponent<SpriteRenderer>();
        anim   = GetComponent<Animator>();
        if (hitCol) hitCol.isTrigger = true;
    }

    /// <summary>
    /// 스폰 직후 보스가 호출.
    /// </summary>
    public void Init(float lifeSeconds, float dps, Vector2 size, float tickInterval = 0.25f)
    {
        this.lifeSeconds = lifeSeconds;
        this.dps         = dps;
        this.tickInterval = Mathf.Max(0.05f, tickInterval);

        if (hitCol) hitCol.size = size;

        // 처음엔 Enter 재생
        if (anim) anim.Play("Enter", 0, 0f);

        // 실행
        StartCoroutine(CoRun());
    }

    private IEnumerator CoRun()
    {
        // 1) Enter 끝날 때까지 기다렸다가 Active로
        if (anim)
        {
            yield return null; // 재생 시작 1프레임 보장
            float enterLen = GetClipLength(anim, "Enter");
            yield return new WaitForSeconds(enterLen);
            FadeToState("Active", 0.05f);
        }

        // 2) 틱 데미지 시작
        Coroutine tick = StartCoroutine(CoTickDamage());

        // 3) lifeSeconds 만큼 유지
        yield return new WaitForSeconds(lifeSeconds);

        // 4) 종료: 틱 정지 → End 트리거 → End 길이 대기 → 파괴
        if (tick != null) StopCoroutine(tick);

        if (anim)
        {
            anim.SetTrigger("End");
            float endLen = GetClipLength(anim, "End");
            yield return new WaitForSeconds(endLen);
        }

        Destroy(gameObject);
    }

    private IEnumerator CoTickDamage()
    {
        while (true)
        {
            // OverlapBox 중심/크기는 콜라이더 기준
            Vector2 center = (Vector2)hitCol.bounds.center;
            Vector2 size   = hitCol.size;
            var hits = Physics2D.OverlapBoxAll(center, size, 0f);

            foreach (var h in hits)
            {
                if (!h || !h.CompareTag("Player")) continue;
                var p = h.GetComponent<PlayerStatsController>();
                if (p == null) continue;
                p.OnAttacked(dps * tickInterval);   // 초당 dps → tickInterval 만큼으로 환산
            }

            yield return new WaitForSeconds(tickInterval);
        }
    }

    private static float GetClipLength(Animator a, string clipName)
    {
        if (a == null || a.runtimeAnimatorController == null) return 0.1f;
        foreach (var c in a.runtimeAnimatorController.animationClips)
            if (c.name == clipName) return Mathf.Max(0.01f, c.length);
        return 0.1f; // 못 찾으면 기본
    }

    private void FadeToState(string stateName, float fade = 0.05f)
    {
        if (!anim) return;
        int hash = Animator.StringToHash(stateName);
        if (anim.HasState(0, hash)) anim.CrossFade(stateName, fade, 0, 0f);
    }

    private void OnDrawGizmosSelected()
    {
        if (hitCol == null) hitCol = GetComponent<BoxCollider2D>();
        if (hitCol == null) return;
        Gizmos.color = new Color(0.4f, 0f, 1f, 0.35f);
        Gizmos.DrawWireCube(hitCol.bounds.center, hitCol.size);
    }
    
    
}