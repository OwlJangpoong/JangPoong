using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterBossStat : MonsterStat
{
    public override void OnAttacked(float damage)
    {
        //움직임 정지
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(0, rb.velocity.y);
        
        //last hit time update하기
        GetComponent<MonsterNokmor>().UpdateLastHitTime(Time.time);
        
        
        if (damage < CurrentHp)
        {
            GetComponentInChildren<SpriteRenderer>().color = Color.red;
            StartCoroutine(CoChangeColorWithDelay(Color.white, 0.5f));
        }
        
        CurrentHp = CurrentHp - damage;
        Managers.Sound.Play("61_Hit_03");
    }
}
