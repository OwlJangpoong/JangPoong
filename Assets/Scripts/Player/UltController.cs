using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class UltController : MonoBehaviour
{  
    [Header("Ult Stats")]
    [SerializeField] private float ultSpeed = 7.5f; //일반 장풍의 1/2 속도 -> 1.5배 증가 (250325 다인)
    [SerializeField] private float ultDamage = 20f;
    
    [SerializeField] public Vector2 ultDirection;
    
    
    [Header("Ult Status")] 
    private float aliveTime;
    
    
    //궁극기 관련 컴포넌트
    private Animator animator;
    private Collider2D _collider2D;
    private Rigidbody2D rb;
    
    
    
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
        
        InitializeUlt();
    }


    #region Ult Control Method

    private void InitializeUlt()
    {
        //speed
        ultSpeed = Managers.Player.IsRunning ? 7 : 6;

        //velocity
        rb.velocity = ultDirection * ultSpeed;
        
        //local scale
        transform.localScale = new Vector3((ultDirection.x > 0 ? 1f : -1f), 1f, 1f);// 궁극기 크기는 장풍의 2배 크기
        
        //alive time
        aliveTime = 1.5f; // 5초 에서 1.5초로 변경 (250325 다인)
    }
    
    

    //1. 충돌
    //충돌 여부에 따라 애니메이션 조절
    //충돌시 폭파 애니메이션, 충돌 x시 일정 시간이 지나면 사라지도록
    //충돌 판정 : 몬스터, ground, level1, leveln, wall 충돌 시 폭파 처리 / 몬스터 충돌 시 몬스터 피격 처리
    //폭발할 때 collider 끄기?

    private void OnTriggerEnter2D(Collider2D other)
    {
        // isColliding = true;
        //장풍 일시정지
        // rb.velocity = new Vector2(0,0);
        // collider2D.enabled = false;
        // animator.SetTrigger("Explosion");

        if (other.gameObject.layer == (int)Define.Layer.Monster)
        {
            Debug.Log("몬스터 공격받음");
            other.GetComponent<MonsterStat>().OnAttacked(ultDamage);
        }
    }
    
    
    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(aliveTime-0.5f);
        
        animator.SetTrigger("Extinct");

        yield return new WaitForSeconds(0.5f);
        
        Destroy(gameObject);

    }


    #endregion
    
  



    #region AnimationEventMethod
    // public void JangpoongVanish()
    // {
    //     Destroy(gameObject);
    // }

    public void IncreaseScale()
    {
        Vector3 scale = transform.localScale;
        
        // 각 축에 대해 동일한 크기 증가 적용
        scale = Vector3.Scale(scale.normalized, Vector3.one * 0.2f) + scale;

        // 변경된 크기를 오브젝트에 적용
        transform.localScale = scale; 
        
        
    }

    public void UltFly()
    {
        animator.SetTrigger("Fly");
    }
    

    #endregion
   
}
