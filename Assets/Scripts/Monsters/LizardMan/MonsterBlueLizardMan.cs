using System.Collections;
using UnityEngine;

public class MonsterBlueLizardMan : Monster
{
    [Header("Blue HitBox (Animation Event용)")]
    [SerializeField] private Transform hitBoxPivot;                        // 검/손 본
    [SerializeField] private Vector2 hitBoxOffsetLocal = new Vector2(1.6f, 0.0f); // 피벗 기준 앞으로
    [SerializeField] private Vector2 hitBoxSize        = new Vector2(2.8f, 1.2f); // 넉넉히

    [Header("Combat Spec")]
    [SerializeField, Range(0f,1f)] private float critChance = 0.3f;  // 크리 30%
    [SerializeField] private float normalDamage   = 2f;             // 평타 2
    [SerializeField] private float critMultiplier = 2f;             // 크리 2배 = 4

    [Header("AI Ranges")]
    [SerializeField] private float detectRange  = 10f;   // 플레이어 인식
    //[SerializeField] private float attackRange  = 2.2f;  // 공격 시도 거리
    [SerializeField] private float approachMargin = 0.4f; // 공격 트리거 여유(= 살짝 더 가까울 때 발동)

    private Mon_MovementRigidbody2D mover;
    private Rigidbody2D rb;
    private bool isAttacking;

    protected override void Start()
    {
        base.Start();

        mover = GetComponent<Mon_MovementRigidbody2D>();
        rb    = GetComponent<Rigidbody2D>();

        if (hitBoxPivot == null) hitBoxPivot = transform;

        // 몬스터 데이터(HP/DMG)는 SO로 세팅되어 있어야 함. 여기선 평타값만 현재데미지에 반영(선택)
        if (stat != null) stat.currentDamage = normalDamage;

        Debug.Log($"[Blue] Start on {name} | detect={detectRange}, attack={attackRange}, pivot={hitBoxPivot.name}");
    }

    private void Update()
    {
        base.Update(); // AttackDelay, ThinkDelay 감소 등 부모 로직

        // 1) 타겟 없을 때
        if (target == null)
        {
            if (Time.frameCount % 30 == 0) Debug.Log("[Blue] No target / Idle");
            mover.MoveTo(0f);
            anim.SetBool("isFollow", false);
            SetSpeedParam();
            return;
        }

        // 2) 거리 계산
        float distance = Vector2.Distance(transform.position, target.position);
        if (Time.frameCount % 30 == 0) Debug.Log($"[Blue] distance={distance:0.00} (attackRange={attackRange})");

        float triggerDistance = Mathf.Max(0.1f, attackRange - approachMargin); // 살짝 더 가까워질 때 트리거

        // 3) 추적/공격 분기
        if (distance < detectRange && distance > triggerDistance)
        {
            // 접근 이동
            Vector3 dir = (target.position - transform.position).normalized;
            mover.MoveTo(Mathf.Sign(dir.x));
            FlipSprite(dir);
            anim.SetBool("isFollow", true);   // Blue_D_walk로 전이되도록
        }
        else
        {
            // 정지 & 공격 체크
            mover.MoveTo(0f);
            anim.SetBool("isFollow", distance <= triggerDistance);

            if (distance <= triggerDistance && AttackDelay <= 0f && !isAttacking)
            {
                Debug.Log($"[Blue] Start AttackPattern distance={distance:0.00}");
                StartCoroutine(AttackPattern());
                AttackDelay = attackCoolTime;
            }
        }

        SetSpeedParam();
    }

    private IEnumerator AttackPattern()
    {
        isAttacking = true;
        mover.MoveTo(0f); // 공격 중 정지

        bool isCritical = Random.value < critChance; // 30%
        Debug.Log(isCritical ? "[Blue] ATTACK: CRIT" : "[Blue] ATTACK: NORMAL");

        if (isCritical)
        {
            anim.ResetTrigger("AtkNor");
            anim.SetTrigger("AtkCri");   // Blue_Atk_Cri (아래→위)
        }
        else
        {
            anim.ResetTrigger("AtkCri");
            anim.SetTrigger("AtkNor");   // Blue_Atk_Nor (위→아래)
        }

        // 실제 데미지는 애니메이션 이벤트(OnNorHit/OnCriHit)에서만 적용
        // 여기서는 애니메이션이 끝날 시간을 약간 점유만 함
        yield return new WaitForSeconds(0.5f);

        isAttacking = false;
        Debug.Log("[Blue] AttackPattern EXIT");
    }

    // ===== 애니메이션 이벤트(클립에서 호출) =====
    public void OnNorHit() => DoHit(normalDamage);                    // Blue_Atk_Nor 타격 프레임에 연결
    public void OnCriHit() => DoHit(normalDamage * critMultiplier);   // Blue_Atk_Cri  타격 프레임에 연결

    private void DoHit(float damageToDeal)
    {
        if (!hitBoxPivot)
        {
            Debug.LogWarning("[Blue] DoHit called but hitBoxPivot is null");
            return;
        }

        Vector2 center = hitBoxPivot.TransformPoint(hitBoxOffsetLocal);
        Debug.Log($"[Blue] DoHit center={center} size={hitBoxSize}");

        // 레이어 마스크 문제 회피: 전부 검사 후 Player 태그로 필터
        var hits = Physics2D.OverlapBoxAll(center, hitBoxSize, 0f);
        bool hitSomeone = false;
        foreach (var h in hits)
        {
            if (!h.CompareTag("Player")) continue;
            h.GetComponent<PlayerStatsController>()?.OnAttacked(damageToDeal);
            Debug.Log($"[Blue] HIT Player for {damageToDeal}");
            hitSomeone = true;
            break; // 1회만
        }

        if (!hitSomeone)
        {
            Debug.Log("[Blue] DoHit: no player in box");
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!hitBoxPivot) return;
        Vector2 center = hitBoxPivot.TransformPoint(hitBoxOffsetLocal);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(center, hitBoxSize);
    }

    private void SetSpeedParam()
    {
        if (rb != null) anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
    }
}