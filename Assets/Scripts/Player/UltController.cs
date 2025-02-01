using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UltController : MonoBehaviour
{  
    //=====장풍 레벨 정보, 데미지 관리, 업그레이드를 어떻게 처리할 것인가? ======//
    
    private Animator animator;
#pragma warning disable CS0108 // 멤버가 상속된 멤버를 숨깁니다. new 키워드가 없습니다.
    private Collider2D collider2D;
#pragma warning restore CS0108 // 멤버가 상속된 멤버를 숨깁니다. new 키워드가 없습니다.
    private Rigidbody2D rb;
   
    [SerializeField] private float damage = 20f;
    // private bool isColliding = false;

        
    private float aliveTime;
    public float AliveTime
    {
        set
        {
            aliveTime = value;
        }
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
            other.GetComponent<MonsterStat>().OnAttacked(damage);
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
    //
    //
    // Start is called before the first frame update
    void Start()
    {
        Init();
        StartCoroutine(nameof(DestroyAfterTime));
    }

    void Init()
    {
        collider2D = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    
    private IEnumerator DestroyAfterTime()
    {
        yield return new WaitForSeconds(aliveTime);

        // if (!isColliding)
        // {
            Destroy(gameObject);
        // }
    }




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
