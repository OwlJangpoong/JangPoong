using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class JangpoongController : MonoBehaviour
{  
    [Header("JangPoong Stats")]
    [SerializeField] private float jangPoongSpeed = 10.0f;
    [SerializeField] private float jangPoongDistance = 5.0f;
    [SerializeField] private Define.JangPoongLevel jangPoongLevel;
    [SerializeField] private float jangPoongDamage;

    [SerializeField] public Vector2 jangPoongDirection;
    
    
    
    [Header("JangPoong Status")] 
    private bool isColliding = false;
    private float aliveTime;
    // public float AliveTime
    // {
    //     set
    //     {
    //         aliveTime = value;
    //     }
    // }
    
    //장풍 관련 컴포넌트
    private Animator animator;
    private Collider2D _collider2D;
    private Rigidbody2D rb;
    
    
    // Start is called before the first frame update
    void Start()
    {
        Init();
        StartCoroutine(nameof(DestroyAfterTime));
    }

    void Init()
    {
        _collider2D = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
        //장풍 세부 스탯 초기 설정
        InitializeJangPoong();

    }
    
    
    #region JangPoong Control Method

    //Define에 정의된 배열에 접근하여 설정한 Define.JangPoongLevel에 따른 데미지 가져와서 반환
    private float GetJangPoongDamage()
    {
        return Define.JangPoongDamageList[(int)jangPoongLevel];
    }

    private void InitializeJangPoong()
    {
        //speed 설정
        jangPoongSpeed = Managers.Player.IsRunning ? 14 : 12;

        //레벨에 따른 damage 셋팅
        jangPoongDamage = GetJangPoongDamage();
        
        //velocity 설정
        rb.velocity = jangPoongDirection * jangPoongSpeed;
        
        //local scale
        transform.localScale = new Vector3((jangPoongDirection.x > 0 ? 0.5f : -0.5f), 0.5f, 0.5f);
        
        //alive time
        aliveTime = jangPoongDistance / jangPoongSpeed;
        
    }
    
    
    //1. 충돌
    //충돌 여부에 따라 애니메이션 조절
    //충돌시 폭파 애니메이션, 충돌 x시 일정 시간이 지나면 사라지도록
    //충돌 판정 : 몬스터, ground, level1, leveln, wall 충돌 시 폭파 처리 / 몬스터 충돌 시 몬스터 피격 처리
    //폭발할 때 collider 끄기?
    private void OnTriggerEnter2D(Collider2D other)
    {
        isColliding = true;
        //장풍 일시정지
        rb.velocity = new Vector2(0,0);
        _collider2D.enabled = false;
        animator.SetTrigger("Explosion");

        if (other.gameObject.layer == (int)Define.Layer.Monster)
        {
            Debug.Log("몬스터 공격받음");
            other.GetComponent<MonsterStat>().OnAttacked(jangPoongDamage);
        }
    }
    
    // //3. 좌우 이동에 따라 장풍 좌우 반전 처리
    // private void SetDirection()
    // {
    //     if (rb.velocity.x < 0)
    //     {
    //         Vector3 scale = transform.localScale;
    //         scale.x = -Mathf.Abs(transform.localScale.x);
    //         transform.localScale = scale;
    //         Debug.Log(transform.localScale.x);
    //     }
    // }

    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(aliveTime);

        if (!isColliding)
        {
            Destroy(gameObject);
        }
    }

    #endregion
    



    #region AnimationEventMethod
    public void JangpoongVanish()
    {
        Destroy(gameObject);
    }

    public void IncreaseScale()
    {
        Vector3 scale = transform.localScale;

        // 각 축에 대해 동일한 크기 증가 적용
        scale = Vector3.Scale(scale.normalized, Vector3.one * 0.2f) + scale;

        // 변경된 크기를 오브젝트에 적용
        transform.localScale = scale; }
    

    #endregion
   
}
