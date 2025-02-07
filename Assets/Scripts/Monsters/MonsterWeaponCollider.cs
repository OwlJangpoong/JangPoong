using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MonsterWeaponCollider : MonoBehaviour
{
    //무기 컨트롤
    public Vector2 boxSize;
    private float damage;
    private Collider2D weaponCollider;
    
    //사망 action 구독
    private MonsterStat stat;

    private void Awake()
    {
        weaponCollider = GetComponent<Collider2D>();
    }


    private void Start()
    {
        stat = GetComponentInParent<MonsterStat>();
        stat.DieAction -= InactiveWeapon;
        stat.DieAction += InactiveWeapon;
    }

    private void InactiveWeapon()
    {
        if (weaponCollider)
        {
            weaponCollider.enabled = false;
            Debug.Log($"weapon collider : {weaponCollider.enabled}");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("플레이어 충돌");
            
            //250203 수정
            other.GetComponent<PlayerStatsController>().OnAttacked(GetComponentInParent<MonsterStat>().monsterData.Damage);
            // Managers.Player.OnAttacked(GetComponentInParent<MonsterStat>().monsterData.Damage);
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position,boxSize);

    }


    public void AttackPlayerByWeapon()
    {
        // Debug.Log("무기 공격 호출함");
        Collider2D[] collider2Ds = Physics2D.OverlapBoxAll(transform.position, boxSize, 0);
        foreach (Collider2D collider in collider2Ds)
        {
            if (collider.CompareTag("Player"))
            {
                Debug.Log("몬스터가 플레이어를 공격합니다.");
                // Managers.Player.OnAttacked(GetComponentInParent<MonsterStat>().monsterData.Damage);
                
                //250203 수정
                collider.GetComponent<PlayerStatsController>().OnAttacked(GetComponentInParent<MonsterStat>().monsterData.Damage);
                
                
                
                
                // playerDataManager.OnAttacked(GetComponentInParent<MonsterStat>().monsterData.Damage);
                return;
            }
    
        }

    }
}
