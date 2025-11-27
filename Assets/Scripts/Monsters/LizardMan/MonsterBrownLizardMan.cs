using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBrownLizardMan : Monster
{
    
    public float healInterval = 15f;
    public float speedNormal = 0.8f;
    public float speedCharge = 1.5f;
    public float chargeDamage = 7f;
    public float chargeFailPenalty = 5f;
    public float chargeRange = 10f;
    public float roarCooldown = 10f;

    private bool isCharging = false;
    private bool isRoaring = false;
    private float lastRoarTime = -Mathf.Infinity;

    protected override void Start()
    {
        base.Start();
        StartCoroutine(RecoverHealth());
    }

    private void Update()
    {
        base.Update();

        if (target == null || isCharging || isRoaring) return;

        // 돌진 조건: 플레이어가 같은 y레벨에 있고, 범위 내에 있음
        if (Mathf.Abs(transform.position.y - target.position.y) < 0.5f &&
            Vector3.Distance(transform.position, target.position) <= chargeRange)
        {
            StartCoroutine(ChargeAtPlayer());
        }
        // 포효 조건: 쿨타임이 끝났고, 범위 내 (5타일 이내 등)
        else if (Time.time >= lastRoarTime + roarCooldown &&
                 Vector3.Distance(transform.position, target.position) <= 5f)
        {
            StartCoroutine(Roar());
        }
        else
        {
            // 일반 이동 (예: Idle이나 추적)
            movement2D.MoveTo(Mathf.Sign(direction.x) * speedNormal);
        }
    }

    private IEnumerator ChargeAtPlayer()
    {
        isCharging = true;
        anim.SetTrigger("Charge");

        Vector3 dir = (target.position - transform.position).normalized;
        movement2D.MoveTo(Mathf.Sign(dir.x) * speedCharge);

        float chargeTime = 1.5f;
        float elapsed = 0f;
        bool hit = false;

        while (elapsed < chargeTime)
        {
            if (target != null &&
                Mathf.Abs(transform.position.x - target.position.x) < 0.5f &&
                Mathf.Abs(transform.position.y - target.position.y) < 1f)
            {
                // 돌진 성공
                target.GetComponent<PlayerStatsController>().OnAttacked(chargeDamage);
                hit = true;
                break;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        movement2D.MoveTo(0);

        if (!hit)
        {
            stat.CurrentHp -= chargeFailPenalty;
            Debug.Log("돌진 실패 - 자해 5 HP");
        }

        yield return new WaitForSeconds(1f);
        isCharging = false;
    }

    private IEnumerator Roar()
    {
        isRoaring = true;
        lastRoarTime = Time.time;

        anim.SetTrigger("Roar");
        Debug.Log("포효!");

        if (target != null)
        {
            PlayerStatsController playerStats = target.GetComponent<PlayerStatsController>();
            if (playerStats != null)
            {
                playerStats.ApplyConfusion(10f); // 혼란 10초
            }
        }

        yield return new WaitForSeconds(2f);
        isRoaring = false;
    }

    private IEnumerator RecoverHealth()
    {
        while (true)
        {
            yield return new WaitForSeconds(healInterval);

            if (stat.CurrentHp < stat.monsterData.MaxHp)
            {
                stat.CurrentHp += 1f;
                Debug.Log("HP 회복: " + stat.CurrentHp);
            }
        }
    }
}
