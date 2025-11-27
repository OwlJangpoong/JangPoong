using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterNokmor : Monster
{
    // ───────────────────────────── UI/맵 한계 ─────────────────────────────
    [Header("UI")]
    public UI_HpBar_Boss UIHpBarBoss;

    [Header("Map Move Limit")]
    public Transform minMoveX;
    public Transform maxMoveX;

    // ───────────────────────────── HP 재생 ─────────────────────────────
    #region HP Regen
    private float lastHitTime;
    private float healCooldown = 10f;   // 10초간 피해 없으면 회복
    private float healRate     = 5f;    // 초당 5 회복
    #endregion

    // ───────────────────────────── 공격 공통 ─────────────────────────────
    #region Attack Common
    private bool  isAttacking;
    private float lastAttackType = -1f;
    // AttackType 구간:
    // 0.0 = Swing
    // [0.1,0.3) = DarkEnergy (세로 기둥)
    // [0.3,0.5) = Dark Bullet
    // [0.5,0.7) = Dark Creature
    // [0.7,0.9) = Gravity
    // [0.9,1.0+] = Black Hole
    private float[] attackTypes = { 0f, 0.2f, 0.4f, 0.6f, 0.8f, 1.0f };

    private MonsterWeaponCollider weaponCollider;
    #endregion

    // ───────────────────────────── Dark Bullet ─────────────────────────────
    #region Dark Bullet
    [Header("Dark Bullet")]
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float spawnRadius = 2f;
    #endregion

    // ───────────────────────────── MP Drain 버퍼 ─────────────────────────────
    #region MP Drain Buffer
    private float mpDrainBuffer = 0f;
    #endregion

    // ───────────────────────────── Dark Creature 소환 ─────────────────────────────
    #region Dark Creature
    [Header("Dark Creature Summon")]
    public GameObject[] monsterPrefabs;
    public Transform summonPoint;
    public int minSummonCount = 2;
    public int maxSummonCount = 5;

    private readonly Dictionary<string, float> monsterSpawnChances = new()
    {
        { "Monster_DarkSlime",          16f },
        { "Monster_BombSlime",           9f },
        { "Monster_Slime_v2",            3f },
        { "Monster_KnifeGoblin_v3",     13f },
        { "Monster_BatGoblin_v2",       13f },
        { "Monster_NecroSkeleton_v1",   23f },
        { "Monster_WizardSkeleton_v1",  13f },
        {"Monster_BlueLizardMan_v1", 10f}
    };
    #endregion

    // ───────────────────────────── Dark Energy (세로 기둥) ─────────────────────────────
    #region Dark Energy Pillar (Vertical)
    [Header("Dark Energy (Vertical Pillar)")]
    public GameObject telegraphPrefab;          // 예고 띠 프리팹
    public GameObject pillarPrefab;             // 실제 기둥 프리팹

    [Tooltip("예고 표시 시간(초)")]  public float energyWarnTime     = 1.2f;
    [Tooltip("기둥 유지 시간(초)")]  public float energyLifeTime     = 2.0f;   // 애니 포함 전체 수명
    [Tooltip("기둥 데미지/초")]       public float energyDamagePerSec = 5f;     // 장판에 서있는 동안 지속 피해
    [Tooltip("동시에 떨어질 기둥 개수")] public int energyPillarCount = 3;
    [Tooltip("보스 x 기준 좌우 최대 오프셋")] public float energyMaxOffsetX = 8f;
    [Tooltip("기둥 폭(월드 단위)")]    public float energyPillarWidth  = 1.0f;
    [Tooltip("기둥 높이(월드 단위)")]  public float energyPillarHeight = 12f;
    #endregion

    // ───────────────────────────── Unity 라이프사이클 ─────────────────────────────
    protected override void Start()
    {
        Init();
        UIHpBarBoss.gameObject.SetActive(true);

        minMoveRangeX = minMoveX.position.x;
        maxMoveRangeX = maxMoveX.position.x;

        // 무기 이벤트(후려치기) 연결 + 충돌 off
        weaponCollider = GetComponentInChildren<MonsterWeaponCollider>(true);
        if (weaponCollider != null)
        {
            weaponCollider.OnWeaponAttack -= SlashAttack;
            weaponCollider.OnWeaponAttack += SlashAttack;

            var col = weaponCollider.GetComponent<Collider2D>();
            if (col) col.enabled = false;
        }

        StartCoroutine(Healing());
        //startPos = transform.position;
    }
    
    // private void Update()
    // {
    //     //FloatMotion();   // 위아래
    //     //HorizontalMove(); // 좌우
    // }

    // ───────────────────────────── HP 재생 ─────────────────────────────
    #region HP Regen Impl
    private IEnumerator Healing()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            if (Time.time - lastHitTime > healCooldown)
            {
                stat.CurrentHp += (int)healRate;
                if (stat.CurrentHp > stat.monsterData.MaxHp)
                    stat.CurrentHp = stat.monsterData.MaxHp;

                Debug.Log($"보스 HP 회복: {stat.CurrentHp}/{stat.monsterData.MaxHp}");
            }
        }
    }

    public void UpdateLastHitTime(float value) => lastHitTime = value;
    #endregion

    // ───────────────────────────── 공격 API & 타이밍 ─────────────────────────────
    #region Attack Flow
    public void RandomAttack()
    {
        Debug.Log("랜덤 어택 시도됨");
        if (isAttacking) { Debug.Log("이미 공격 중"); return; }
        
        if (isAttacking) return;
        isAttacking = true;

        float attackType;
        do
        {
            attackType = attackTypes[Random.Range(0, attackTypes.Length)];
        } while (Mathf.Approximately(attackType, lastAttackType));

        lastAttackType = attackType;

        anim.SetFloat("AttackType", attackType);
        anim.SetTrigger("Attack");

        AttackDelay = attackCoolTime;
    }

    public float GetAttackDuration()
    {
        float t = anim.GetFloat("AttackType");
        switch (t)
        {
            case 0.0f: return 1.2f; // Swing
            case 0.2f: return 2.1f; // Dark Energy
            case 0.4f: return 2.1f; // Dark Bullet
            case 0.6f: return 2.1f; // Dark Creature
            case 0.8f: return 2.1f; // Gravity
            case 1.0f: return 1.8f; // Black Hole
            default:   return 1.0f;
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        anim.SetFloat("AttackType", -1f);
    }
    #endregion

    // ───────────────────────────── 후려치기 ─────────────────────────────
    #region Melee Slash
    public void SlashAttack(Collider2D targetCol)
    {
        if (!targetCol.CompareTag("Player")) return;

        var playerStats = targetCol.GetComponent<PlayerStatsController>();
        if (playerStats == null) return;

        playerStats.LoseMP(30);
        ApplyKnockback(targetCol);
        Debug.Log("SlashAttack 성공!");
    }

    private void ApplyKnockback(Collider2D player)
    {
        var rb = player.GetComponent<Rigidbody2D>();
        if (!rb) return;

        float knockbackX = Mathf.Sign(player.transform.position.x - transform.position.x) * 5f;
        Vector2 dir = new(knockbackX, 1f);
        float force = 5f;

        player.GetComponent<MovementRigidbody2D>()?.ApplyKnockback(force * dir, 0.3f);
    }
    #endregion

    // ───────────────────────────── Dark Bullet ─────────────────────────────
    #region Dark Bullet Impl
    public void DarkBulletAttack()
    {
        Debug.Log("어둠의 총알 스킬 호출");
        StartCoroutine(DarkBulletCoroutine());
    }

    private IEnumerator DarkBulletCoroutine()
    {
        var bullets = new List<GameObject>();
        float[] spawnDelays = { 0.3f, 0.8f, 1.3f, 1.8f };

        for (int i = 0; i < spawnDelays.Length; i++)
        {
            yield return new WaitForSeconds(spawnDelays[i] - (i > 0 ? spawnDelays[i - 1] : 0));

            Vector2 spawnPos = (Vector2)firePoint.position + Random.insideUnitCircle * spawnRadius;
            GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
            if (!bullet) continue;

            float dirMul = transform.localScale.x > 0 ? -1f : 1f;
            Vector2[] dirs =
            {
                Vector2.right * dirMul,
                Vector2.right * dirMul,
                Quaternion.Euler(0,0, 45)*Vector2.right * dirMul,
                Quaternion.Euler(0,0,-45)*Vector2.right * dirMul,
            };
            int id = Random.Range(0, dirs.Length);

            var c = bullet.GetComponent<NokmorDarkBullet>();
            c.SetDirection(dirs[id]);
            c.IsShooting = false;

            bullets.Add(bullet);
        }

        yield return new WaitForSeconds(0.2f);

        foreach (var b in bullets)
        {
            if (!b) continue;
            var c = b.GetComponent<NokmorDarkBullet>();
            if (c) c.IsShooting = true;
        }

        bullets.Clear();
    }
    #endregion

    // ───────────────────────────── Dark Creature 소환 ─────────────────────────────
    #region Dark Creature Impl
    public void DarkCreatureSummon()
    {
        StartCoroutine(DarkCreatureSummonCoroutine());
    }

    private IEnumerator DarkCreatureSummonCoroutine()
    {
        summonPoint.GetChild(0).gameObject.SetActive(true);

        int count = Random.Range(minSummonCount, maxSummonCount + 1);
        for (int i = 0; i < count; i++)
        {
            yield return new WaitForSeconds(0.5f);

            GameObject prefab = GetRandomMonster();
            if (!prefab) continue;

            Vector2 pos = summonPoint.position + new Vector3(Random.Range(-2f, 2f), 0f, 0f);
            GameObject m = Instantiate(prefab, pos, Quaternion.identity, null);
            m.SetActive(true);
            m.GetComponent<Monster>()?.Init();
            EnhanceMonster(m);
        }

        summonPoint.GetChild(0).gameObject.SetActive(false);
    }

    private GameObject GetRandomMonster()
    {
        float total = 0f; foreach (var v in monsterSpawnChances.Values) total += v;
        float r = Random.Range(0, total), acc = 0f;

        foreach (var kv in monsterSpawnChances)
        {
            acc += kv.Value;
            if (r <= acc)
            {
                foreach (var p in monsterPrefabs)
                    if (p.name.Contains(kv.Key)) return p;
            }
        }
        return null;
    }

    private void EnhanceMonster(GameObject m)
    {
        var s = m.GetComponent<MonsterStat>();
        if (s == null || s.monsterData == null) return;

        s.monsterData.IncreaseMaxHp(3);
        s.CurrentHp += 3;
        s.currentDamage *= 1.5f;

        var mon = m.GetComponent<Monster>();
        if (mon != null)
        {
            foreach (var loot in mon.lootTable)
                loot.dropChance = 100f;
        }
    }
    #endregion

    // ───────────────────────────── Dark Energy (세로 기둥) ─────────────────────────────
    #region Dark Energy Impl
    public void DarkEnergyAttack()
    {
        StartCoroutine(CoDarkEnergyVertical());
    }

    private IEnumerator CoDarkEnergyVertical()
    {
        if (!pillarPrefab || !telegraphPrefab) yield break;

        // 1) 생성할 X 샘플링: 플레이어 라인 + 좌/우 + 랜덤
        var xs = new List<float>();
        float bossX   = transform.position.x;
        float playerX = target ? target.position.x : bossX;

        xs.Add(playerX);
        xs.Add(Mathf.Clamp(bossX - 3f, bossX - energyMaxOffsetX, bossX + energyMaxOffsetX));
        xs.Add(Mathf.Clamp(bossX + 3f, bossX - energyMaxOffsetX, bossX + energyMaxOffsetX));
        while (xs.Count < energyPillarCount)
            xs.Add(bossX + Random.Range(-energyMaxOffsetX, energyMaxOffsetX));

        // 2) 예고 띠 생성
        var warns = new List<GameObject>();
        for (int i = 0; i < xs.Count; i++)
        {
            Vector3 pos = new(xs[i], transform.position.y + 3f, 0f);
            var w = Instantiate(telegraphPrefab, pos, Quaternion.identity);
            float d = energyPillarWidth*4;
            w.transform.localScale = new Vector3(d, d, 1f);
            warns.Add(w);
        }

        yield return new WaitForSeconds(energyWarnTime);

        // 3) 예고 제거 & 기둥 생성
        foreach (var w in warns) if (w) Destroy(w);

        for (int i = 0; i < xs.Count; i++)
        {
            Vector3 pos = new(xs[i], transform.position.y, 0f);
            var go = Instantiate(pillarPrefab, pos, Quaternion.identity);
            var pillar = go.GetComponent<DarkEnergyPillar>();
            pillar.Init(
                lifeSeconds:      energyLifeTime,
                dps:              energyDamagePerSec,   // 초당 피해
                size:             new Vector2(energyPillarWidth, energyPillarHeight),
                tickInterval:     0.25f                // 0.25초마다 틱 데미지
            );
        }
    }
    #endregion

    // ───────────────────────────── Gravity / BlackHole ─────────────────────────────
   // ───────────────────────────── Gravity / Black Hole ─────────────────────────────
#region Gravity / Black Hole

// === Gravity Field (중력장) ===
[Header("Gravity Field")]
[SerializeField] private float gravityDuration   = 30f;
[SerializeField] private float gravityMultiplier = 2f;  // 플레이어 중력배수
[SerializeField] private int   gravityHealHp     = 20;  // 시작 즉시 힐

public void GravityFieldAttack()
{
    StartCoroutine(CoGravityField());
}

private IEnumerator CoGravityField()
{
    // 시작 즉시 힐
    if (stat != null)
    {
        stat.CurrentHp += gravityHealHp;
        if (stat.CurrentHp > stat.monsterData.MaxHp)
            stat.CurrentHp = stat.monsterData.MaxHp;
    }

    if (!target) yield break;

    var rb = target.GetComponent<Rigidbody2D>();
    if (!rb) yield break;

    float original = rb.gravityScale;
    rb.gravityScale = original * gravityMultiplier;

    float t = 0f;
    while (t < gravityDuration)
    {
        t += Time.deltaTime;
        yield return null;
    }

    // 복구
    rb.gravityScale = original;
}


// === Black Hole (프리팹 기반) ===
[Header("Black Hole")]
public GameObject blackHolePrefab;
public Vector2    blackHoleSpawnOffset = new Vector2(0f, 1.0f);
public bool       oneBlackHoleAtATime  = true;     // 동시에 하나만 유지할지

// 튜닝 값(프리팹 기본값을 덮어쓰고 싶으면 사용)
public float blackHoleLife        = 10f;
public float blackHolePull        = 14f;
public float blackHoleSlow        = 0.5f;   // 0.5면 절반속도 느낌의 감속
public float blackHoleMpPerSec    = 10f;
public float blackHoleContactDps  = 5f;     // 접촉 중 초당 HP 데미지
public float blackHoleTick        = 0.25f;  // 접촉 틱 간격

private GameObject currentBlackHole;

/// <summary>
/// 애니메이션 이벤트(BlackHole 상태 중간 프레임)로 호출해.
/// 프리팹을 스폰하고, 플레이어를 주입하고, 필요하면 튜닝값도 밀어넣음.
/// </summary>
public void BlackHoleAttack()
{
    SpawnOrRefreshBlackHole();
}

/// <summary>이미 떠있으면 정리하고 새로 스폰(옵션).</summary>
private void SpawnOrRefreshBlackHole()
{
    if (!blackHolePrefab || !target) return;

    if (oneBlackHoleAtATime && currentBlackHole)
    {
        // 기존 것을 End로 마무리
        var oldCtrl = currentBlackHole.GetComponent<BlackHoleController>();
        if (oldCtrl) oldCtrl.TriggerEnd();
        Destroy(currentBlackHole, 0.1f);
        currentBlackHole = null;
    }

    Vector3 pos = transform.position + (Vector3)blackHoleSpawnOffset;
    currentBlackHole = Instantiate(blackHolePrefab, pos, Quaternion.identity);

    var ctrl = currentBlackHole.GetComponent<BlackHoleController>();
    if (ctrl)
    {
        // 플레이어 참조 주입
        ctrl.Init(target);

        // ▼ 블랙홀 프리팹이 ApplyTuning(...)을 구현한 경우 자동 반영 (없으면 프리팹 기본값 사용)
        // 시그니처: void ApplyTuning(float life, float pull, float slow, float mpPerSec, float contactDps, float tick)
        var m = ctrl.GetType().GetMethod(
            "ApplyTuning",
            new System.Type[] { typeof(float), typeof(float), typeof(float), typeof(float), typeof(float), typeof(float) }
        );
        if (m != null)
        {
            m.Invoke(ctrl, new object[] { blackHoleLife, blackHolePull, blackHoleSlow, blackHoleMpPerSec, blackHoleContactDps, blackHoleTick });
        }
    }
}

/// <summary>
/// 애니에서 “End”로 넘어가거나, 강제로 블랙홀 제거하고 싶을 때 호출.
/// (필요하면 애니메이션 이벤트나 상태 종료 시점에 배치)
/// </summary>
public void ForceEndBlackHole()
{
    if (!currentBlackHole) return;
    var ctrl = currentBlackHole.GetComponent<BlackHoleController>();
    if (ctrl) ctrl.TriggerEnd();
    currentBlackHole = null;
}

#endregion

    // ───────────────────────────── Gizmos ─────────────────────────────
    private void OnDrawGizmos()
    {
        if (!firePoint) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(firePoint.position, spawnRadius);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, scanRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    // MonsterNokmor 내부 어딘가에 추가
    public void ClearAttacking()
    {
        isAttacking = false;
        anim.ResetTrigger("Attack");
        anim.SetFloat("AttackType", -1f);
    }
    
    // [Header("Float Movement")]
    // public float floatAmplitude = 0.5f;   // 위아래 이동 거리
    // public float floatSpeed = 2f;         // 속도
    //
    // private Vector3 startPos;
    //
    // private void FloatMotion()
    // {
    //     float newY = startPos.y + Mathf.Sin(Time.time * floatSpeed) * floatAmplitude;
    //     transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    // }
    
    // [Header("Horizontal Float")]
    // public float moveSpeed = 2f;
    // private int moveDir = 1;
    
    // private void HorizontalMove()
    // {
    //     // X 이동
    //     transform.Translate(Vector2.right * moveDir * moveSpeed * Time.deltaTime);
    //
    //     // 범위 체크해서 방향 반전
    //     if (transform.position.x > maxMoveRangeX)
    //         moveDir = -1;
    //     else if (transform.position.x < minMoveRangeX)
    //         moveDir = 1;
    // }
    
    // ───────────────────────────── 디버그용 ─────────────────────────────
    public void SlashSkill()        { anim.SetFloat("AttackType", 0.0f); anim.SetTrigger("Attack"); }
    public void DarkEnergySkill()   { anim.SetFloat("AttackType", 0.2f); anim.SetTrigger("Attack"); }
    public void DarkBulletSkill()   { anim.SetFloat("AttackType", 0.4f); anim.SetTrigger("Attack"); }
    public void DarkCreatureSkill() { anim.SetFloat("AttackType", 0.6f); anim.SetTrigger("Attack"); }
    public void GravitySkill()      { anim.SetFloat("AttackType", 0.8f); anim.SetTrigger("Attack"); }
    public void BlackHoleSkill()    { anim.SetFloat("AttackType", 1.0f); anim.SetTrigger("Attack"); }
}