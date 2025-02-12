using Unity.Mathematics;
using UnityEngine;

public class Mon_MovementRigidbody2D : MonoBehaviour
{
    [Header("LayerMask")]
    [SerializeField]
    private LayerMask groundCheckLayer; // 바닥 체크를 위한 충돌 레이어
    [SerializeField]
    private LayerMask aboveColiisionLayer; // 머리 충돌 체크를 위한 레이어
    [SerializeField]
    private LayerMask belowColiisionLayer; // 발 충돌 체크를 위한 레이어

    [Header("Move")]
    [SerializeField]
    private float walkSpeed = 5; // 걷는 속도
    
#pragma warning disable CS0414 // 필드가 대입되었으나 값이 사용되지 않습니다
    [SerializeField]
    private float runSpeed = 8; // 뛰는 속도
#pragma warning restore CS0414 // 필드가 대입되었으나 값이 사용되지 않습니다
    
    public float WalkSpeed
    {
        get { return walkSpeed; }
    }
    
    [Header("Jump")]
    [SerializeField]
    private float jumpForce = 13; // 점프 힘
    [SerializeField]
    private float highGravityScale = 3.5f; // 일반적으로 적용되는 중력 (낮은 점프)

    public bool isJump = false;

    private float moveSpeed; // 이동 속도

    // 바닥에 착지 직전 조금 빨리 점프 키를 눌렀을 때 바닥에 착지하면 바로 점프가 되도록
    private float jumpBufferTime = 0.1f; // 공중에 떠있을 때 점프 키 + 0.1초 안에 착지하면 자동 점프
    private float jumpBufferCounter;

    // 낭떠러지에서 떨어질 때 아주 잠시 동안 점프가 가능하도록 설정하기 위한 변수
    private float hangTime = 0.3f; // 점프가 가능한 한계 시간 (바닥에서 발이 떨어지고 0.3초 내에 점프 가능)
    private float hangCounter; // 시간 계산을 위한 변수

    private Vector2 collisionSize; // 머리, 발 위치에 생성하는 충돌 박스 크기
    private Vector2 footPosition; // 발 위치
    private Vector2 headPosition; // 머리 위치
    public Vector2 FootPosition => footPosition;
    public Vector2 HeadPosition { get { return headPosition; } }

    private Rigidbody2D rigid2D; // 물리를 제어하는 컴포넌트
    
#pragma warning disable CS0108 // 멤버가 상속된 멤버를 숨깁니다. new 키워드가 없습니다.
    private Collider2D collider2D; // 현재 오브젝트의 충돌 범위
#pragma warning restore CS0108 // 멤버가 상속된 멤버를 숨깁니다. new 키워드가 없습니다.

    public bool IsLongJump { set; get; } = false; // 낮은 점프, 높은 점프 체크
    [SerializeField] private bool isGrounded = false;
    
    public bool IsGrounded
    {
        private set { isGrounded = value;}
        get { return isGrounded; }
    } // 바닥 체크 (바닥에 닿아있을 때 true)
    public Collider2D HitAboveObject { private set; get; } // 머리에 충돌한 오브젝트 정보
                                                           // 머리의 오브젝트 충돌 여부를 MovementRigidbody2D에서 검사하기 때문에 set은 현재 클래스에서만 할 수 있도록 private으로 설정
    public Collider2D HitBelowObject { private set; get; }  // 발에 충돌한 오브젝트 정보

    public Vector2 Velocity => rigid2D.velocity; // rigid2D.velocity를 반환하는 GET만 가능한 프로퍼티 Velocity 정의


    private Animator anim;

    private void Awake()
    {
        moveSpeed = walkSpeed;

        rigid2D = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
        if (collider2D == null) collider2D = GetComponentInChildren<Collider2D>();

        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        UpdateCollision();
        JumpHeight();
        JumpAdditive();
    }

    // x축 속력(velocity) 설정, 외부 클래스에서 호출
    public void MoveTo(float x)
    {
        // x축 방향 속력을 x * moveSpeed로 설정
        rigid2D.velocity = new Vector2(x * moveSpeed, rigid2D.velocity.y);
        if (HasParameter(anim, "velocityX"))
        {
            anim.SetFloat("velocityX", Mathf.Abs(x));
        }
        
    }
    
    bool HasParameter(Animator animator, string paramName)
    {
        foreach (AnimatorControllerParameter param in animator.parameters)
        {
            if (param.name == paramName)
            {
                return true;
            }
        }
        return false;
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
        // IsGrounded = Physics2D.OverlapBox(footPosition, collisionSize, 0, groundCheckLayer);
        IsGrounded = Physics2D.Raycast(transform.position, Vector2.down, 1f, groundCheckLayer);
        Debug.DrawRay(transform.position, Vector3.down*1f, Color.red);
        // Physics2D.OverlapBox(Vector2 point, Vector2 size, float angle, int layerMask);
        // point 위치에 size 크기의 충돌 박스(BoxCollider2D)를 angle 각도만큼 회전해서 생성
        // 이 충돌 박스는 layerMask에 설정된 레이어만 충돌이 가능      

        // 플레이어의 머리/발에 충돌한 오브젝트 정보를 저장하는 충돌 박스
        HitAboveObject = Physics2D.OverlapBox(headPosition, collisionSize, 0, aboveColiisionLayer);
        HitBelowObject = Physics2D.OverlapBox(footPosition, collisionSize, 0, belowColiisionLayer);
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
            rigid2D.gravityScale = highGravityScale;
        
    }

    private void JumpAdditive()
    {
        // 낭떠러지에서 떨어질 때 아주 잠시동안 점프가 가능하도록 설정
        if (IsGrounded) hangCounter = hangTime;
        else hangCounter -= Time.deltaTime;
        
        // 바닥 착지 직전 조금 빨리 점프 키를 눌렀을 때 바닥에 착지하면 바로 점프하도록 설정
        if (jumpBufferCounter > 0)
        {
            // Debug.Log("jumpbuffercounter 양수");
            // Debug.Log(hangCounter);
            jumpBufferCounter -= Time.deltaTime;
        }

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

}

