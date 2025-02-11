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
    
    
    
    protected override void Start()
    {
        Init(); //부모 클래스의 Init() 호출
        UIHpBarBoss.gameObject.SetActive(true); //UI_HP_Bar active 하여 Start() 호출되도록 설정 (반드시 hp bar는 꺼둔 상태여야한다.)

        StartCoroutine(Healing());
    }


    #region hp 재생

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
    
}