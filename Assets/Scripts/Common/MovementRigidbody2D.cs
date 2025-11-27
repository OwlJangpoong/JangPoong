using System;
using System.Collections;
using UnityEngine;
using TMPro;

public class MovementRigidbody2D : MonoBehaviour
{
    [Header("LayerMask")]
    [SerializeField]
    private LayerMask groundCheckLayer; // 바닥 체크를 위한 충돌 레이어
    [SerializeField]
    private LayerMask aboveColiisionLayer; // 머리 충돌 체크를 위한 레이어
    [SerializeField]
    private LayerMask belowColiisionLayer; // 발 충돌 체크를 위한 레이어

    [Header("Move")]
    public float originWalkSpeed = 5; // 걷는 속도

    #region Move Speed Control Variables / Methods
    [NonSerialized] public float walkSpeed;
    [NonSerialized] public float ssgWalkSpeed;
    [NonSerialized] public float sgWalkSpeed;
    private bool isInSsg = false;
    private bool isInSg = false;


    public bool IsInSsg
    {
        get { return isInSsg; }
        set
        {
            isInSsg = value;
            int state = value ? 1 : (IsInSg ? 2 : 0);
            controlSpeedAction?.Invoke(state);
            
        }
    }


    public bool IsInSg
    {
        get { return isInSg; }
        set
        {
            isInSg = value;
            int state = isInSsg ? 1 : (value ? 2 : 0);
            controlSpeedAction?.Invoke(state);
        }
    }


    public Action<int> controlSpeedAction = null;

    /// <summary>
    /// 0 : 원래 속도대로
    /// 1 : ssg 안에서 감속 (0.5배)
    /// 2 : sg 안에서 감속(0.75배)
    /// </summary>
    /// <param name="state"></param>
    private void WalkSpeedState(int state)
    {
        switch (state)
        {
            case 0:
                //원래 속도대로
                walkSpeed = originWalkSpeed;
                break;
            case 1:
                //ssg
                walkSpeed = ssgWalkSpeed;
                break;
            case 2:
                //sg
                walkSpeed = sgWalkSpeed;
                break;
        }
        // Debug.Log($"state : {state} / walkspeed : {walkSpeed}");

    }

    #endregion

    
    [SerializeField]
    public float runSpeed = 8; // 뛰는 속도
    
    
    [Header("Jump")]
    [SerializeField]
    private float jumpForce = 10; // 점프 힘
    [SerializeField]
    private float lowGravityScale = 2; // 점프 키를 오래 누르고 있을 때 적용되는 중력 (높은 점프)
    [SerializeField]
    private float highGravityScale = 3.0f; // 일반적으로 적용되는 중력 (낮은 점프)

    [SerializeField] private float moveSpeed; // 이동 속도

    // 바닥에 착지 직전 조금 빨리 점프 키를 눌렀을 때 바닥에 착지하면 바로 점프가 되도록
    private float jumpBufferTime = 0.1f; // 공중에 떠있을 때 점프 키 + 0.1초 안에 착지하면 자동 점프
    private float jumpBufferCounter;

    // 낭떠러지에서 떨어질 때 아주 잠시 동안 점프가 가능하도록 설정하기 위한 변수
    private float hangTime = 0.3f; // 점프가 가능한 한계 시간 (바닥에서 발이 떨어지고 0.3초 내에 점프 가능)
    private float hangCounter; // 시간 계산을 위한 변수

    private Vector2 collisionSize; // 머리, 발 위치에 생성하는 충돌 박스 크기
    private Vector2 footPosition; // 발 위치
    private Vector2 headPosition; // 머리 위치

    private Rigidbody2D rigid2D; // 물리를 제어하는 컴포넌트
#pragma warning disable CS0108 // 멤버가 상속된 멤버를 숨깁니다. new 키워드가 없습니다.
    private Collider2D collider2D; // 현재 오브젝트의 충돌 범위
#pragma warning restore CS0108 // 멤버가 상속된 멤버를 숨깁니다. new 키워드가 없습니다.
    public bool IsLongJump { set; get; } = false; // 낮은 점프, 높은 점프 체크
    public bool IsGrounded { private set; get; } = false; // 바닥 체크 (바닥에 닿아있을 때 true)
    public Collider2D HitAboveObject { private set; get; } // 머리에 충돌한 오브젝트 정보
                                                           // 머리의 오브젝트 충돌 여부를 MovementRigidbody2D에서 검사하기 때문에 set은 현재 클래스에서만 할 수 있도록 private으로 설정
    public Collider2D HitBelowObject { private set; get; }  // 발에 충돌한 오브젝트 정보

    public Vector2 Velocity => rigid2D.velocity; // rigid2D.velocity를 반환하는 GET만 가능한 프로퍼티 Velocity 정의

    
    
    //넉백 추가 (250211)
    private bool isKnockedBack = false; // 넉백 상태 변수
    
    
    
    private void Awake()
    {
        rigid2D = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
        if (collider2D == null) collider2D = GetComponentInChildren<Collider2D>();
        
        //speed 설정 & ssg, sg에 의해 감속된 speed 값 구해두기
        walkSpeed = originWalkSpeed;
        ssgWalkSpeed = originWalkSpeed * 0.5f;
        sgWalkSpeed = originWalkSpeed * 0.75f;
        
        //action 등록
        controlSpeedAction -= WalkSpeedState;
        controlSpeedAction += WalkSpeedState;


    }

    private void Update()
    {
        if (!isKnockedBack)
        {
            UpdateCollision();
            JumpHeight();
            JumpAdditive();
        }
        
        UpdateGroundedState();
    }

    private void UpdateGroundedState()
    {
        Bounds bounds = collider2D.bounds;
        footPosition = new Vector2(bounds.center.x, bounds.min.y);
        collisionSize = new Vector2((bounds.max.x - bounds.min.x) * 0.8f, 0.1f);

        // 바닥에 닿아 있는지 체크하여 IsGrounded 값 업데이트
        IsGrounded = Physics2D.OverlapBox(footPosition, collisionSize, 0, groundCheckLayer);
    }

    // x축 속력(velocity) 설정, 외부 클래스에서 호출
    public void MoveTo(float x)
    {
        // x의 절대값이 0.5이면 걷기(walkSpeed), 1이면 뛰기(runSpeed)
        // moveSpeed = Mathf.Abs(x) != 1 ? walkSpeed : runSpeed; //도현 : 뛰기 없으므로 x가 1일때 모두 walkspeed로 처리. 이를 위해 코드 주석 처리(0704)

        // x가 -0.5, 0.5의 값을 가질 때 x를 -1, 1로 변경
        //if (x != 0) x = Mathf.Sign(x); //도현 : 뛰기 없으므로 x가 1일때 모두 walkspeed로 처리. 이를 위해 코드 주석 처리(0704)

        // x축 방향 속력을 x * moveSpeed로 설정

        if (!isKnockedBack) //넉백 중에는 이동 금지
        {
            float moveSpeed = walkSpeed;
            rigid2D.velocity = new Vector2(x * moveSpeed, rigid2D.velocity.y);
        }
    }

    private void UpdateCollision()
    {
        // 플레이어 오브젝트의 Collider2D min, center, max 위치 정보
        Bounds bounds = collider2D.bounds;

        // 플레이어 발에 생성하는 충돌 범위
        collisionSize = new Vector2((bounds.max.x - bounds.min.x) * 0.8f, 0.1f);

        // 플레이어의 머리/발 위치
        headPosition = new Vector2(bounds.center.x, bounds.max.y);
        footPosition = new Vector2(bounds.center.x, bounds.min.y);

        // 플레이어가 바닥을 밟고 있는지 체크하는 충돌 박스
        IsGrounded = Physics2D.OverlapBox(footPosition, collisionSize, 0, groundCheckLayer);
        // Physics2D.OverlapBox(Vector2 point, Vector2 size, float angle, int layerMask);
        // point 위치에 size 크기의 충돌 박스(BoxCollider2D)를 angle 각도만큼 회전해서 생성
        // 이 충돌 박스는 layerMask에 설정된 레이어만 충돌이 가능      

        // 플레이어의 머리/발에 충돌한 오브젝트 정보를 저장하는 충돌 박스
        HitAboveObject = Physics2D.OverlapBox(headPosition, collisionSize, 0, aboveColiisionLayer);
        HitBelowObject = Physics2D.OverlapBox(footPosition, collisionSize, 0, belowColiisionLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(footPosition,collisionSize);
    }


    // 다른 클래스에서 호출하는 점프 메소드
    // y축 점프
    public void Jump()
    {
        if (IsGrounded == true)
        {
            rigid2D.velocity = new Vector2(rigid2D.velocity.x, jumpForce);
        }

        jumpBufferCounter = jumpBufferTime;
    }

    public void JumpTo(float force)
    {
        rigid2D.velocity = new Vector2(rigid2D.velocity.x, force);
    }

    private void JumpHeight()
    {
        // 낮은 점프, 높은 점프 구현을 위한 중력 계수(gravityScale) 조절 (Jump Up일 때만 적용된다)
        // 중력 계수가 낮은 if문은 높은 점프가 되고, 중력 계수가 높은 else 문은 낮은 점프가 된다
        if (IsLongJump && rigid2D.velocity.y > 0)
        {
            rigid2D.gravityScale = lowGravityScale;
        }
        else
        {
            rigid2D.gravityScale = highGravityScale;
        }
    }

    private void JumpAdditive()
    {
        // 낭떠러지에서 떨어질 때 아주 잠시동안 점프가 가능하도록 설정
        if (IsGrounded) hangCounter = hangTime;
        else hangCounter -= Time.deltaTime;

        // 바닥 착지 직전 조금 빨리 점프 키를 눌렀을 때 바닥에 착지하면 바로 점프하도록 설정
        if (jumpBufferCounter > 0) jumpBufferCounter -= Time.deltaTime;

        if (jumpBufferCounter > 0 && hangCounter > 0)
        {
            // 점프 힘(jumpForce)만큼 y축 방향 속력으로 설정
            rigid2D.velocity = new Vector2(rigid2D.velocity.x, jumpForce);
            jumpBufferCounter = 0;
            hangCounter = 0;
        }
    }

    public void ResetVelocityY()
    {
        rigid2D.velocity = new Vector2(rigid2D.velocity.x, 0);
    }
    
    
    public void ApplyKnockback(Vector2 force, float duration)
    {
        StartCoroutine(KnockbackRoutine(force, duration));
    }

    private IEnumerator KnockbackRoutine(Vector2 force, float duration)
    {
        isKnockedBack = true;

        // 넉백 적용 전에 기존 속도 초기화
        rigid2D.velocity = Vector2.zero;
        
        //  넉백 적용
        rigid2D.AddForce(force, ForceMode2D.Impulse);

        yield return new WaitForSeconds(duration);

        // 넉백 종료 후 다시 정상 이동 가능
        isKnockedBack = false;
    }
    
}

