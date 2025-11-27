using System.Collections;
using UnityEngine;

public class MonsterPoisonLizardMan : Monster
{
    public GameObject poisonPrefab;
    public Transform firePoint;
    public float poisonSpeed = 200f;  // 발사 속도 (속도를 더 빠르게 조정)
    public float attackCooldown = 3f; // 3초 쿨타임

    private bool isAttacking = false;  // 공격 중인지 확인하는 변수
    private float lastAttackTime = -Mathf.Infinity; // 마지막 공격 시각
    public GameObject poisonAreaPrefab;
    public Transform mouthPoint;
    public float healInterval = 10f;

    private bool isSpraying = false;
    private float lastSprayTime = -Mathf.Infinity;

    private Rigidbody2D rb;
    
    public override void Init()
    {
        base.Init();
        rb = GetComponent<Rigidbody2D>();
        scanRange = 10f;
        attackRange = 2.0f;
    }

    private void Update()
    {
        base.Update();
        if (target != null)
        {
            float distance = Vector2.Distance(transform.position, target.position);

            if (distance < scanRange && distance > attackRange)
            {
                Vector3 dir = (target.position - transform.position).normalized;
                movement2D.MoveTo(Mathf.Sign(dir.x));
                FlipSprite(dir);
            }
            else
            {
                
                movement2D.MoveTo(0f);
            }
        }
        else
        {
            movement2D.MoveTo(0f);
        }

        if (rb != null)
        {
            anim.SetFloat("Speed", Mathf.Abs(rb.velocity.x));
        }
    }

    public override int Detect(Vector3 direction)
    {
        int detectResult = base.Detect(direction);

        if (detectResult != -1 && Time.time >= lastAttackTime + attackCooldown) // 쿨타임이 지나야 공격
        {
            StartCoroutine(PoisonAttack(target));
        }

        return detectResult;
    }

    private IEnumerator PoisonAttack(Transform target)
    {
        if (isAttacking) yield break; // 이미 공격 중이면 종료
        isAttacking = true;

        lastAttackTime = Time.time; // 공격 시작 시각 기록

        if (poisonPrefab != null && firePoint != null)
        {
            GameObject poison = Instantiate(poisonPrefab, firePoint.position, Quaternion.identity, this.transform);

            Vector3 fireDirection = Vector3.right; // 기본은 오른쪽
            if (target.position.x < firePoint.position.x)
            {
                fireDirection = Vector3.left; // 타겟이 왼쪽에 있으면 왼쪽으로 발사
            }

            float duration = 1f; // 독 발사 지속시간
            float elapsed = 0f;

            // Poison이 빠르게 날아가도록 처리
            while (elapsed < duration)
            {
                if (poison == null) yield break; // 독이 없으면 종료

                // 독을 더 빠르게 이동시키도록 함 (속도 증가)
                poison.transform.position += fireDirection * poisonSpeed * Time.deltaTime;

                elapsed += Time.deltaTime;
                yield return null;
            }

            if (poison != null)
            {
                Destroy(poison); // 발사 후 2초 뒤에 자동 삭제
            }

            Debug.Log("독 발사");
        }

        yield return new WaitForSeconds(attackCooldown); // 쿨타임 대기

        isAttacking = false; // 공격 끝
    }
    private IEnumerator SprayPoison()
    {
        if (isSpraying) yield break;
        isSpraying = true;
        lastSprayTime = Time.time;

        anim.SetTrigger("Spray"); // 애니메이션 트리거 (있다면)

        if (poisonAreaPrefab != null && mouthPoint != null)
        {
            GameObject poisonArea = Instantiate(poisonAreaPrefab, mouthPoint.position, Quaternion.identity);
            Destroy(poisonArea, 3f); // 범위 독 3초 유지
        }

        yield return new WaitForSeconds(1f);
        isSpraying = false;
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
