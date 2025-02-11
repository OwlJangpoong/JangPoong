using System.Collections;
using UnityEngine;

public class MonsterNokmor : Monster
{
    [Header("UI")]
    public UI_HpBar_Boss UIHpBarBoss;

    #region hp 재생 관련 변수

    private float lastHitTime;
    private float healCooldown = 10f; //10초 동안 공격을 받지 않으면 hp를 회복한다.
    private float healRate = 5f; //1초당 5만큼 회복
    
    #endregion

    #region Attack

    private bool isAttacking;
    private float lastAttackType=-1f;
    private float[] attackTypes = { 0f, 0.2f, 0.4f, 0.6f, 0.8f, 1f };

    private MonsterWeaponCollider weaponCollider;

    #endregion
    
    
    protected override void Start()
    {
        Init(); //부모 클래스의 Init() 호출
        UIHpBarBoss.gameObject.SetActive(true); //UI_HP_Bar active 하여 Start() 호출되도록 설정 (반드시 hp bar는 꺼둔 상태여야한다.)
        
        
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
            case 0f: return 1.0f; // Slash
            case 0.2f: return 1.2f; // Dark Energy
            case 0.4f: return 1.5f; // Dark Bullet
            case 0.6f: return 1.8f; // Dark Creature
            case 0.8f: return 2.0f; // Gravity
            case 1.0f: return 2.2f; // Black Hole
            default: return 1.0f;
        }
    }

    public void EndAttack()
    {
        isAttacking = false;
        anim.SetFloat("AttackType", -1f);
    }
    
    
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
         }
     }
     
     
     //2. 어둠의 총알
     public void DarkBulletAttack()
     {
         
     }
    //
    // private void ApplyKnockback(Collider2D player)
    // {
    //     Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
    //     if (rb != null)
    //     {
    //         Vector2 knockbackDirection = (player.transform.position - transform.position).normalized;
    //         rb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
    //     }
    // }
    //
    // #endregion
    //
    // private void OnDrawGizmosSelected()
    // {
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    // }
    
    //2. 
    
    
    //3.
    
    
    
    //4.
    
    
    //5.
    
    
    
    //6.

    

    #endregion




    public void SlashSkill()
    {
        anim.SetFloat("AttackType",0f);
        anim.SetTrigger("Attack");
    }
    
}