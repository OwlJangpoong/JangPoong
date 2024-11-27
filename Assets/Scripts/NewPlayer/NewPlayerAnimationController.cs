using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewPlayerAnimationController : MonoBehaviour
{
    public Animator animator;
    private NewPlayerMovement movement;
    Rigidbody2D rb;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<NewPlayerMovement>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void UpdateAnimation(float x)
    {
        // 좌/우 방향키 입력이 있을 때
        if (x != 0)
        {
            // 플레이어 스프라이트 좌/우 반전 : 바라보는 방향 설정
            SpriteFlipX(x);
        }

        if (Mathf.Abs(rb.velocity.normalized.x) < 0.3)
            animator.SetBool("isWalking", false);
        else
            animator.SetBool("isWalking", true);

        if(movement.isJumping)
            animator.SetBool("isJumping", true);

        if (movement.isDoubleJumping)
            animator.SetBool("isDoubleJumping", true);
        else
            animator.SetBool("isDoubleJumping", false);

        if (movement.isGround)
        {
            animator.SetBool("isJumping", false);
        }

    }

    // SpriteRenderer 컴포넌트의 Filp을 이용해 이미지를 반전했을 때
    // 화면에 출력되는 이미지 자체만 반전되기 때문에
    // 플레이어의 전방 특정 위치에서 발사체를 생성하는 것과 같이
    // 방향전환이 필요할 때는 Transform.Scale.x를 -1, 1과 같이 설정
    private void SpriteFlipX(float x)
    {
        transform.localScale = new Vector3((x < 0 ? -1 : 1), 1, 1);
    }

    // player 슬라이딩 애니메이션
    public void StartSliding()
    {
        // Debug.Log("슬라이딩 애니메이션 시작");
        animator.SetBool("isSliding", true);
    }

    public void StopSliding()
    {
        // Debug.Log("슬라이딩 애니메이션 종료");
        animator.SetBool("isSliding", false);
    }

    // 달리기 시 애니메이션 배속
    public void SetSpeedMultiplier(float multiplier)
    {
        animator.speed = multiplier;
    }

    // player 장풍 애니메이션
    public void JangPoongShooting()
    {
        // Debug.Log("장풍 애니메이션");
        if (animator.GetBool("isShooting")) return; // 이미 실행 중이면 종료
        animator.SetBool("isShooting", true);
    }

    // 장풍 애니메이션 종료
    public void OnShootingEnd()
    {
        // Debug.Log("장풍 애니메이션 종료");
        animator.SetBool("isShooting", false);
    }

    // 플레이어 죽었을 때
    public void PlayerDead()
    {
        animator.SetBool("Dead", true);
        // Debug.Log("플레이어 쥬금");
    }
}
