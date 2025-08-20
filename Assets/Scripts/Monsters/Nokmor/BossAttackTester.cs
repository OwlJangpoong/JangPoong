using UnityEngine;
using System.Collections;

/// <summary>
/// 숫자키(1~6)로 각 스킬을 실행하고,
/// 지정된 측정창 동안 Player HP/MP 변화와 Boss HP 변화를 집계하여 디버그 로그로 출력.
/// T 키: 랜덤 패턴 루프 토글(기존 기능)
/// </summary>
public class BossAttackTester : MonoBehaviour
{
    public MonsterNokmor boss;

    [Header("랜덤 패턴 루프")]
    public bool autoLoop = false;
    public float loopInterval = 3.0f;
    private Coroutine loopRoutine;

    [Header("측정창(초) - 스킬별 추천값")]
    public float windowSlash       = 0.8f;  // 후려치기: 즉시타격
    public float windowDarkEnergy  = 2.0f;  // 어둠 기파(간이): 1초 예고 후 타격
    public float windowDarkBullet  = 2.5f;  // 어둠 총알: 탄 생성/발사 후 피격까지 여유
    public float windowDarkCreature= 4.0f;  // 소환 직후 충돌까지 여유
    public float windowGravity     = 0.5f;  // 중력장(간이): 직접 피해 없음(보스 힐만 체크)
    public float windowBlackHole   = 2.0f;  // 블랙홀: 초당 MP 드레인, 일부만 관측

    // 캐시
    private PlayerStatsController playerCtrl;
    private MonsterStat bossStat;

    void Reset()
    {
        if (boss == null) boss = GetComponent<MonsterNokmor>();
    }

    void Start()
    {
        if (boss == null) boss = GetComponent<MonsterNokmor>();
        if (boss == null) Debug.LogError("[BossTester] MonsterNokmor 를 찾을 수 없습니다.");

        // 플레이어/보스 Stat 캐시
        var playerGo = GameObject.FindGameObjectWithTag("Player");
        if (playerGo != null) playerCtrl = playerGo.GetComponent<PlayerStatsController>();
        bossStat = boss ? boss.GetComponent<MonsterStat>() : null;

        if (playerCtrl == null) Debug.LogWarning("[BossTester] PlayerStatsController 를 찾지 못했습니다. HP/MP 측정이 제한될 수 있습니다.");
        if (bossStat == null) Debug.LogWarning("[BossTester] MonsterStat 를 찾지 못했습니다. Boss HP 측정이 제한될 수 있습니다.");
    }

    void Update()
    {
        if (boss == null) return;

        // 1: 후려치기
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            StartCoroutine(MeasureAttack("Slash", windowSlash, () => boss.SlashSkill()));
        }

        // 2: 어둠 기파(간이)
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            StartCoroutine(MeasureAttack("DarkEnergy", windowDarkEnergy, () => boss.DarkEnergyAttack()));
        }

        // 3: 어둠 총알
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            StartCoroutine(MeasureAttack("DarkBullet", windowDarkBullet, () => boss.DarkBulletSkill()));
        }

        // 4: 어둠의 생명체 소환
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            StartCoroutine(MeasureAttack("DarkCreature", windowDarkCreature, () => boss.DarkCreatureSkill()));
        }

        // 5: 중력장(간이) - 보스 힐/플레이어 중력 변화, 직접피해 없음
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            StartCoroutine(MeasureAttack("GravityField", windowGravity, () => boss.GravityFieldAttack()));
        }

        // 6: 블랙홀(간이) - MP 초당 10 소모, 10초 지속(측정창 내 일부만 관측)
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            StartCoroutine(MeasureAttack("BlackHole", windowBlackHole, () => boss.BlackHoleAttack()));
        }

        // T: 랜덤 루프 토글
        if (Input.GetKeyDown(KeyCode.T))
        {
            autoLoop = !autoLoop;
            if (autoLoop)
            {
                if (loopRoutine == null) loopRoutine = StartCoroutine(AutoLoop());
                Debug.Log("[BossTester] 랜덤 패턴 루프 시작");
            }
            else
            {
                if (loopRoutine != null) StopCoroutine(loopRoutine);
                loopRoutine = null;
                Debug.Log("[BossTester] 랜덤 패턴 루프 중지");
            }
        }
    }

    private IEnumerator AutoLoop()
    {
        while (true)
        {
            // 루프 자체는 측정 없이 기존처럼 동작
            boss.RandomAttack();
            yield return new WaitForSeconds(loopInterval);
        }
    }

    /// <summary>
    /// 스킬 트리거 → 측정창 대기 → HP/MP/BossHP 변화량 로그
    /// </summary>
    private IEnumerator MeasureAttack(string label, float windowSeconds, System.Action trigger)
    {
        // 스냅샷(시작)
        float playerHp0 = Managers.Player != null ? Managers.Player.Hp   : (playerCtrl ? Managers.Player.Hp : 0f);
        int   playerMp0 = Managers.Player != null ? Managers.Player.Mana : (playerCtrl ? Managers.Player.Mana : 0);
        float bossHp0   = bossStat != null ? bossStat.CurrentHp : 0f;

        // 트리거 실행
        trigger?.Invoke();

        // 측정창 대기
        float t = 0f;
        while (t < windowSeconds)
        {
            t += Time.deltaTime;
            yield return null;
        }

        // 스냅샷(끝)
        float playerHp1 = Managers.Player != null ? Managers.Player.Hp   : (playerCtrl ? Managers.Player.Hp : 0f);
        int   playerMp1 = Managers.Player != null ? Managers.Player.Mana : (playerCtrl ? Managers.Player.Mana : 0);
        float bossHp1   = bossStat != null ? bossStat.CurrentHp : 0f;

        // 변화량(+는 회복, -는 피해)
        float dPlayerHp = playerHp1 - playerHp0;
        int   dPlayerMp = playerMp1 - playerMp0;
        float dBossHp   = bossHp1 - bossHp0;

        // 표기 편하게 부호 반전(피해를 양수처럼 보이기)
        float playerHpDamage = -Mathf.Min(dPlayerHp, 0f);
        int   playerMpDrain  = -Mathf.Min(dPlayerMp, 0);
        float bossDamage     = -Mathf.Min(dBossHp, 0f);
        float bossHeal       =  Mathf.Max(dBossHp, 0f);

        // 로그 출력
        Debug.Log(
            $"[BossTester:{label}] " +
            $"Player HP -{playerHpDamage:0.##} (Δ {dPlayerHp:+0.##;-0.##;0}), " +
            $"MP -{playerMpDrain} (Δ {dPlayerMp:+0;-0;0}), " +
            $"Boss HP {(bossDamage>0? "-" + bossDamage.ToString("0.##"): (bossHeal>0? "+"+bossHeal.ToString("0.##"):"±0"))} (Δ {dBossHp:+0.##;-0.##;0}) " +
            $"| window {windowSeconds:0.##}s"
        );
    }
}