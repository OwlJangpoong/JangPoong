using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterStat : MonoBehaviour
{
    public Action DieAction = null;
    
    public MonsterData monsterData;
    protected float currentHp;
    [SerializeField] private UI_HPBar uiHpBar;

    public float CurrentHp
    {
        get { return currentHp; }
        set
        {
            value = Mathf.Clamp(value, 0, monsterData.MaxHp);
            if (value != currentHp)
            {
                currentHp = value;
                if (currentHp == 0)
                {
                    DieAction?.Invoke();
                }
            }
        }
    }
    public float currentDamage;

    public void Init(MonsterData monsterData)
    {
        this.monsterData = ScriptableObject.Instantiate(monsterData);
        currentHp = monsterData.MaxHp;
        currentDamage = monsterData.Damage;

    }
    
    public virtual void OnAttacked(float damage)
    {
        //움직임 정지
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2(0, rb.velocity.y);
        //HP bar 보이기
        if (uiHpBar == null)
        {
            uiHpBar = gameObject.GetComponentInChildren<UI_HPBar>(true);
        }
        uiHpBar.ShowHP();
        
        if (damage < CurrentHp)
        {
            GetComponentInChildren<SpriteRenderer>().color = Color.red;
            StartCoroutine(CoChangeColorWithDelay(Color.white, 0.5f));
        }

        CurrentHp = CurrentHp - damage;
        Managers.Sound.Play("61_Hit_03");
    }
    
    /// <summary>
    /// delay 후 오브젝트의 자식 중 SpriteRenderer component 색깔 변화
    /// </summary>
    /// <param name="color">적용할 색상</param>
    /// <param name="delay">초</param>
    /// <returns></returns>
    protected IEnumerator CoChangeColorWithDelay(Color color, float delay)
    {
        yield return new WaitForSeconds(delay);
        GetComponentInChildren<SpriteRenderer>().color = color;
    }

}
