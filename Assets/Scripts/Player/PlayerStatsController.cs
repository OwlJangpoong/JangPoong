using System;
using System.Collections;
using UnityEngine;
using TMPro;
using Unity.Mathematics;
using UnityEngine.Serialization;

public class PlayerStatsController : MonoBehaviour
{
    [Header("Mana Control")]
    [SerializeField] private int manaRegenerationRate = 3;

    public int ManaConsumption { get; private set; } = 5;
    
    [Header("Invincibility 무적")]
    [SerializeField][Tooltip("피격 시 추가되는 무적 지속 시간")] private float invincibilityDuration = 2;//피격시 추가되는 무적 시간
    private float invincibilityTime = 0; //무적 지속 시간
    public bool IsInvincible { get; set; } = false; //무적 여부
    
    //Invisibility 투명화
    public bool IsInvisible { get; set; } = false;
    public float invisibleDuration = 15;
    
    
    //넉백
    private bool isKnockedBack = false;
    
    
    
    //플레이어 관련 컴포넌트
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Color originColor;
    
    private NewPlayerMovement NewPlayerMovement;

    #region Event Function

    public void Start()
    {
        //Mana Regeneration : 1초마다 RegenerateMana 메소드 호출
        InvokeRepeating("RegenerateMana", 1f, 1f); 
        
        
        //플레이어 관련 컴포넌트 변수 초기화
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        originColor = spriteRenderer.color;
        
        //이벤트 리스너 구독
        
        NewPlayerMovement = GetComponentInChildren<NewPlayerMovement>();
    }
    

    #endregion
    
    
    #region Control Function
    
    //Mana
    private void RegenerateMana()
    {
        //Mana = Mathf.Min(mana + manaRegenerationRate, maxMana);  //코드 리팩토링하면서 아래 코드로 수정함.(250202)
        //
        if(NewPlayerMovement.gameOverFlag == true)
        {
            return;
        }
        else
        {
            Managers.Player.SetMana(Managers.Player.Mana + manaRegenerationRate);
        }
        
    }
    
    
    //피격
    public void OnAttacked(float damage, bool invincible = true)
    {
        if (damage <= 0)
        {
            Debug.Log("오류 : 몬스터 공격 데미지가 0 또는 음수입니다!!");
            return;
        }

        if (invincible)
        {
            //1. 무적 상태 처리
            if (IsInvincible) return; //무적 상태에서는 HP 감소 x

            OnInvincibility(invincibilityDuration); //공격 받으면 invincibilityDuration초 동안 무적 상태
        }
        else
        {
            //레이어를 바꾸는지 않고 
        }


        //2. 체력 감소 처리
        Managers.Player.SetHp(Managers.Player.Hp-damage);
    }
    
    
    public void LoseMP(int amount)
    {
        if (amount <= 0)
        {
            Debug.Log("오류 : 몬스터의 마나 흡수율이 0 또는 음수입니다!!");
            return;
        }
        //마나 감소 처리
        Managers.Player.SetMana(Managers.Player.Mana-amount);
    }
    
    
    //넉백
   
    

    #region 무적상태

    private void OnInvincibility(float time)
    {
        if (IsInvincible)
        {
            invincibilityTime += time;
        }
        else
        {
            invincibilityTime = time;
            StartCoroutine(nameof(Invincibility));

        }

    }

    private IEnumerator Invincibility()
    {
        //1. flag 설정
        IsInvincible = true;
        //2. invincibilityTime 동안 레이어 변경, 깜박이기 효과
        gameObject.layer = (int)Define.Layer.PlayerDamaged; //무적 상태 레이어로 변경

        //3. blink speed
        float blinkSpeed = 10;
        while (invincibilityTime > 0)
        {
            invincibilityTime -= Time.deltaTime;
            Color color = spriteRenderer.color;
            color.a = Mathf.SmoothStep(0, 1, Mathf.PingPong(Time.time * blinkSpeed, 1));
            //PingPong : 0~1 사이를 왕복
            //SmoothStep : 두 값 사이의 부드러운 전환(보간) 효과
            spriteRenderer.color = color;
            yield return null;
        }

        spriteRenderer.color = originColor; //alpha 복구
        gameObject.layer = (int)Define.Layer.Player; //원래 레이어로 복구
        IsInvincible = false;
    }
    

    #endregion

    #region 투명화
    public System.Collections.IEnumerator InvisibilityCoroutine()
    {
        // Renderer 컴포넌트 가져오기
        //Renderer renderer = playerManager.GetComponentInChildren<Renderer>();
        // 원래 색상 저장
        // Color originalColor = renderer.material.color;

        // 투명화 색상 적용 (알파값 0.5로 설정)
        // Color invisibleColor = originalColor;
        // invisibleColor.a = 0.5f;
        // renderer.material.color = invisibleColor;
        // 무적 상태 동안 HP 회복
        // float duration = 15f;
        // float elapsed = 0f;
        // while (elapsed < duration)
        // {
        //     // playerDataManager.Hp = Mathf.Clamp(playerDataManager.Hp + 0.4f, 0, playerDataManager.maxHp); // HP ȸ��
        //     elapsed += 1f; // 1�� ���
        //     yield return new WaitForSeconds(1f);
        // }
        //
        // // ���� �������� ����
        // GetComponent<Renderer>().material.color = originalColor;
        // //����ȭ ���� ����
        // this.playerStatsController.isInvisible = false; 
        // // ����ȭ �� ���� ȿ�� ���� ����
        // this.playerStatsController.IsInvincible = false;
        // yield break;

        //250203 코드 리팩토링 및 수정(도현)
        
        //투명화, 무적 활성화
        IsInvisible = true;
        IsInvincible = true;
        
        //색깔 설정
        Color originalColor = spriteRenderer.color;
        Color invisibleColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0.5f);
        spriteRenderer.color = invisibleColor;
        
        //투명화 상태 15초 유지
        float duration = invisibleDuration;
        float elapsed = 0f;
        
        Debug.Log($"{elapsed}/{duration}");
        while (elapsed < duration)
        {
            elapsed += 1f; // 1초마다 증가
            yield return new WaitForSeconds(1f);
            Managers.Player.SetHp(Managers.Player.Hp+0.4f); //hp 회복
        }
        
        // 원래 색상으로 복원, 투명화, 무적 해제
        spriteRenderer.color = originColor;
        IsInvincible = false;
        IsInvincible = false;
        
    }
    

    #endregion
    
    
    
    #endregion


}
