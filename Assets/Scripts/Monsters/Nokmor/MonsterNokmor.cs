using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterNokmor : Monster
{
    [Header("UI")]
    public UI_HpBar_Boss UIHpBarBoss;

    [Header("Map Move Limit")] public Transform minMoveX;
    public Transform maxMoveX;
    

    #region hp 재생 관련 변수

    private float lastHitTime;
    private float healCooldown = 10f; //10초 동안 공격을 받지 않으면 hp를 회복한다.
    private float healRate = 5f; //1초당 5만큼 회복
    
    #endregion

    #region Attack 관련 기본 변수

    private bool isAttacking;
    private float lastAttackType=-1f;
    private float[] attackTypes = { 0f, 0.4f, 0.6f, 0.8f, 1.0f};

    private MonsterWeaponCollider weaponCollider;

    #endregion

    #region DarkBullet 관련 변수
    [Header("Dark Bullet Attack")] 
    public GameObject bulletPrefab;
    public Transform firePoint; // 보스의 위치 (총알 스폰 기준)
    public float spawnRadius = 2f; // 총알이 생성되는 범위
    #endregion

    #region MP 드레인 누적

    private float mpDrainBuffer = 0f;

    #endregion

    #region DarkCreature 관련 변수

    [Header("Dark Creature Summon")] 
    public GameObject[] monsterPrefabs; // 소환할 수 있는 몬스터 프리팹 리스트
    public Transform summonPoint; // 소환 위치
    public int minSummonCount = 2;
    public int maxSummonCount = 5;
    
    private Dictionary<string, float> monsterSpawnChances = new Dictionary<string, float>()
    {
        { "Monster_DarkSlime", 18f },
        { "Monster_BombSlime", 9f },
        { "Monster_Slime_v2", 3f },
        { "Monster_KnifeGoblin_v3", 15f },
        { "Monster_BatGoblin_v2", 15f },
        { "Monster_NecroSkeleton_v1", 25f },
        { "Monster_WizardSkeleton_v1", 15f }
    };

    #endregion
    
    
    protected override void Start()
    {
        Init(); //부모 클래스의 Init() 호출
        UIHpBarBoss.gameObject.SetActive(true); //UI_HP_Bar active 하여 Start() 호출되도록 설정 (반드시 hp bar는 꺼둔 상태여야한다.)
        
        //min, max 제한
        minMoveRangeX = minMoveX.position.x;
        maxMoveRangeX = maxMoveX.position.x;
        
        
        //이벤트 구독
        weaponCollider = GetComponentInChildren<MonsterWeaponCollider>(true);
        weaponCollider.OnWeaponAttack -= SlashAttack;
        weaponCollider.OnWeaponAttack += SlashAttack;
        

        StartCoroutine(Healing());
    }


    #region hp heal

    private IEnumerator Healing()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            
            // 마지막으로 피해를 받은 후 10초가 지났다면 회복 시작
            if (Time.time - lastHitTime > healCooldown)
            {
                stat.CurrentHp += (int)healRate;

                Debug.Log($"보스 HP 회복: {stat.CurrentHp}/{stat.monsterData.MaxHp}");
            }
        }
    }

    public void UpdateLastHitTime(float value)
    {
        Debug.Log($"last hit time : {value}");
        lastHitTime = value;
    }

    #endregion


    #region Attack Skills

    public void RandomAttack()
    {
        if (isAttacking) return; // 이미 공격 중이면 실행하지 않음
        isAttacking = true;
        float attackType;

        do
        {
            attackType = attackTypes[Random.Range(0, attackTypes.Length)];
        } while (attackType == lastAttackType);

        lastAttackType = attackType; // 현재 공격을 다음 비교에 사용
        anim.SetFloat("AttackType", attackType);
        anim.SetTrigger("Attack");
        
    }
    
    public float GetAttackDuration()
    {
        float attackType = anim.GetFloat("AttackType");

        // ✅ attackType에 따라 애니메이션 길이 다르게 설정
        switch (attackType)
        {
            case 0f:   return 2.0f; // Slash
            case 0.2f: return 2.0f; // Dark Energy(간이)
            case 0.4f: return 3.5f; // Dark Bullet
            case 0.6f: return 2.5f; // Dark Creature
            case 0.8f: return 3.0f; // Gravity Field(간이)
            case 1.0f: return 3.0f; // Black Hole(간이)
            default:   return 1.0f;
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        anim.SetFloat("AttackType", -1f);
    }
    
    
    
    #region 후려치기
    //1. 후려치기
     public void SlashAttack(Collider2D collider2D)
     {
         Debug.Log("SlashAttack 호출");
         PlayerStatsController playerStatsController;
         if (collider2D.CompareTag("Player"))
         {
             playerStatsController = collider2D.GetComponent<PlayerStatsController>();
             playerStatsController.LoseMP(30);
             ApplyKnockback(collider2D);
             Debug.Log("SlashAttack 성공! ");

         }
         else
         {
             Debug.LogWarning("공격 대상이 플레이어가 아닙니다!!!");
             return;
         }
         

     }
    //넉백 적용
     private void ApplyKnockback(Collider2D player)
     {
         Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
         if (rb != null)
         {
             //넉백 방향 설정
             float knockbackX = Mathf.Sign(player.transform.position.x - transform.position.x)*5;
             
             Vector2 knockbackDirection = new Vector2(knockbackX, 1f); // 위쪽으로 약간 튕기도록 설정
             
             // 넉백 힘 적용
             float knockbackForce = 5f;
             player.GetComponent<MovementRigidbody2D>().ApplyKnockback(knockbackForce*knockbackDirection,0.3f);
             Debug.Log("넉백 성공!");
         }
     }
     
     #endregion
     
     
     #region 어둠의총알
     //2. 어둠의 총알
     public void DarkBulletAttack()
     {
         Debug.Log("어둠의 총알 스킬 호출");
         StartCoroutine(DarkBulletCoroutine());
         Debug.Log("어둠의 총알 성공!");
     }
     
     private IEnumerator DarkBulletCoroutine()
     {
         List<GameObject> bullets = new List<GameObject>();
         
         float[] spawnDelays = { 0.3f, 0.8f, 1.3f, 1.8f };

         for (int i = 0; i < spawnDelays.Length; i++)
         {
             yield return new WaitForSeconds(spawnDelays[i] - (i > 0 ? spawnDelays[i - 1] : 0));

             Vector2 spawnPos = (Vector2)firePoint.position + Random.insideUnitCircle * spawnRadius;
             GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.identity);

             if (bullet == null) continue; // 생성 실패 시 무시
             
             // 보스의 localScale.x 값에 따라 방향 설정
             float directionMultiplier = transform.localScale.x > 0 ? -1f : 1f;

             
             // 방향은 미리 설정하지만, 아직 발사하지 않음
             Vector2[] directions = {
                 Vector2.right * directionMultiplier, 
                 Vector2.right * directionMultiplier, 
                 Quaternion.Euler(0, 0, 45) * Vector2.right * directionMultiplier, 
                 Quaternion.Euler(0, 0, -45) * Vector2.right * directionMultiplier
             };
             int randomDir = Random.Range(0, directions.Length);

             NokmorDarkBullet bulletController = bullet.GetComponent<NokmorDarkBullet>();
             bulletController.SetDirection(directions[randomDir]);
             bulletController.IsShooting = false; // 바로 발사하지 않음

             bullets.Add(bullet); // 리스트에 추가

             Debug.Log($"총알 생성됨: {bullet.name}, 위치: {spawnPos}, 방향: {directions[randomDir]}");
         }

         yield return new WaitForSeconds(0.2f); // 총알 생성 후 잠깐 대기
         

         // 🔥 모든 총알을 한 번에 발사
         foreach (var bullet in bullets)
         {
             if (bullet == null) continue; // ✅ 삭제된 오브젝트는 무시
             NokmorDarkBullet bulletController = bullet.GetComponent<NokmorDarkBullet>(); 
             if (bulletController != null)
             {
                 bulletController.IsShooting = true; // 한 번에 발사
             }
         }
         
         bullets.Clear();
        
         
     }
     #endregion



     #region 어둠의생명체
     public void DarkCreatureSummon()
    {
        StartCoroutine(DarkCreatureSummonCoroutine());
        Debug.Log("어둠의 생명체 성공! ");
    }

    private IEnumerator DarkCreatureSummonCoroutine()
    {
        summonPoint.GetChild(0).gameObject.SetActive(true);
        
        int summonCount = Random.Range(minSummonCount, maxSummonCount + 1);
        Debug.Log($"🟣 어둠의 생명체 {summonCount}마리 소환!");

        for (int i = 0; i < summonCount; i++)
        {
            yield return new WaitForSeconds(0.5f); // 0.5초 간격으로 소환

            GameObject selectedMonster = GetRandomMonster();
            if (selectedMonster == null) continue;

            Vector2 summonPos = summonPoint.position + new Vector3(Random.Range(-2f, 2f), 0f, 0f);
            GameObject newMonster = Instantiate(selectedMonster, summonPos, Quaternion.identity, null);
            newMonster.SetActive(true);
            newMonster.GetComponent<Monster>().Init();
            
            EnhanceMonster(newMonster);
        }
        
        
        summonPoint.GetChild(0).gameObject.SetActive(false);
    }

    private GameObject GetRandomMonster()
    {
        float totalChance = 0f;
        foreach (var chance in monsterSpawnChances.Values)
        {
            totalChance += chance;
        }

        float randomValue = Random.Range(0, totalChance);
        float cumulativeChance = 0f;

        foreach (var kvp in monsterSpawnChances)
        {
            cumulativeChance += kvp.Value;
            if (randomValue <= cumulativeChance)
            {
                foreach (var prefab in monsterPrefabs)
                {
                    if (prefab.name.Contains(kvp.Key))
                    {
                        return prefab;
                    }
                }
            }
        }

        return null;
    }


    private void EnhanceMonster(GameObject monster)
    {
        MonsterStat monsterStat = monster.GetComponent<MonsterStat>();

       
        
        if (monsterStat == null)
        {
            Debug.LogWarning("❌ monsterStat이 null입니다! MonsterStat 컴포넌트가 있는지 확인하세요.");
            return;
        }

        if (monsterStat.monsterData == null)
        {
            Debug.LogWarning("❌ monsterStat.monsterData가 null입니다! Init()이 정상적으로 호출되지 않았을 가능성이 있습니다.");
            return;
        }
        
        if (monsterStat != null)
        {
            monsterStat.monsterData.IncreaseMaxHp(3);
            monsterStat.CurrentHp += 3;
            monsterStat.currentDamage *= 1.5f;
        }

        // ✅ 100% 포션 드랍
        Monster monsterScript = monster.GetComponent<Monster>();
        if (monsterScript != null)
        {
            foreach (var loot in monsterScript.lootTable)
            {
                loot.dropChance = 100f;
            }
        }

   
    }

    #endregion

    

    #endregion




    public void SlashSkill()
    {
        anim.SetFloat("AttackType",0f);
        anim.SetTrigger("Attack");
    }

    public void DarkBulletSkill()
    {
        anim.SetFloat("AttackType",0.4f);
        anim.SetTrigger("Attack");
    }
    public void DarkCreatureSkill()
    {
        anim.SetFloat("AttackType",0.6f);
        anim.SetTrigger("Attack");
    }
    
    // === 간이 스킬(비주얼 없이 효과만) ===

// ▼ 어둠 기파: 1초 예고 후, 플레이어가 보스와 같은 수평 라인(가까운 Y) 이면 5 데미지 1회
public void DarkEnergyAttack()
{
    StartCoroutine(CoDarkEnergySimple());
}
private IEnumerator CoDarkEnergySimple()
{
    // 예고 타임 (마법진 대신 대기)
    yield return new WaitForSeconds(1f);

    if (target == null) yield break;
    var player = target.GetComponent<PlayerStatsController>();
    if (player == null) yield break;

    // 수평 쓸기: 보스와 플레이어의 Y 차가 작으면 맞은 걸로 간주
    float yDiff = Mathf.Abs(target.position.y - transform.position.y);
    if (yDiff <= 1.0f)   // 필요하면 조정
    {
        player.OnAttacked(5f);
        Debug.Log("어둠 기파 적중(간이): HP -5");
    }
    Debug.Log("어둠 기파 성공! ");
}

// ▼ 중력장: 30초 동안 플레이어 중력 배수 ↑, 끝나면 복구. 시작 시 보스 즉시 20 힐
public void GravityFieldAttack()
{
    StartCoroutine(CoGravityFieldSimple());
}
private IEnumerator CoGravityFieldSimple()
{
    // 보스 즉시 힐
    if (stat != null)
    {
        stat.CurrentHp += 20f;
        if (stat.CurrentHp > stat.monsterData.MaxHp) stat.CurrentHp = stat.monsterData.MaxHp;
        Debug.Log("중력장 발동: 보스 즉시 20 회복");
    }

    if (target == null) yield break;

    var rb = target.GetComponent<Rigidbody2D>();
    if (rb == null) yield break;

    float originalGravity = rb.gravityScale;
    rb.gravityScale = originalGravity * 2.0f; // 필요하면 3.0f 등으로 조정

    float t = 0f;
    float duration = 30f;
    while (t < duration)
    {
        t += Time.deltaTime;
        yield return null;
    }

    // 복구
    if (rb != null)
        rb.gravityScale = originalGravity;

    Debug.Log("중력장 종료: 중력 복구");
    Debug.Log("중력장 성공! ");
}

// ▼ 블랙홀: 10초 동안 보스 기준 한 점으로 빨아들이고, 이동 저항 + MP 초당 10 소모
public void BlackHoleAttack()
{
    StartCoroutine(CoBlackHoleSimple());
    Debug.Log("블랙홀 성공! ");
}

private IEnumerator CoBlackHoleSimple()
{
    if (target == null) yield break;

    Vector3 holePos = transform.position + Vector3.up * 1.0f;

    var rb    = target.GetComponent<Rigidbody2D>();
    var stats = target.GetComponent<PlayerStatsController>();

    float life          = 10f;
    float pullStrength  = 12f;   // 끌림 세기
    float slowFactor    = 0.5f;  // 감속(0.5이면 절반 느낌)
    float mpDrainPerSec = 10f;   // 초당 10 MP

    // ✅ 시작할 때 버퍼 초기화(혹시 이전 스킬에서 남은 값 방지)
    mpDrainBuffer = 0f;

    while (life > 0f)
    {
        life -= Time.deltaTime;

        if (rb != null)
        {
            Vector2 dir = (holePos - target.position);
            rb.AddForce(dir.normalized * (pullStrength * Time.deltaTime), ForceMode2D.Force);
            rb.velocity *= (1f - (1f - slowFactor) * Time.deltaTime);
        }

        // ✅ 초당 10 누적 → 1 이상일 때 정수로 깎고, 남은 소수는 보존
        if (stats != null)
        {
            mpDrainBuffer += mpDrainPerSec * Time.deltaTime;

            if (mpDrainBuffer >= 1f)
            {
                int mpToDrain = Mathf.FloorToInt(mpDrainBuffer);
                stats.LoseMP(mpToDrain);          // LoseMP(int) 가정
                mpDrainBuffer -= mpToDrain;       // 남은 소수 보존
            }
        }

        yield return null;
    }
    

    // ✅ 종료 시 버퍼 정리(안 해도 되지만 깔끔하게)
    mpDrainBuffer = 0f;
    Debug.Log("블랙홀 종료");
}
    private void OnDrawGizmos()
    {
        if (firePoint == null) return;
    
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(firePoint.position, spawnRadius);
    }
    
    private void OnDrawGizmosSelected()
    {
        // ✅ ScanRange 기즈모 (파란색)
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, scanRange);

        // ✅ AttackRange 기즈모 (빨간색)
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }


}