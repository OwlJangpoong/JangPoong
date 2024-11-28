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
        // ��/�� ����Ű �Է��� ���� ��
        if (x != 0)
        {
            // �÷��̾� ��������Ʈ ��/�� ���� : �ٶ󺸴� ���� ����
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

    // SpriteRenderer ������Ʈ�� Filp�� �̿��� �̹����� �������� ��
    // ȭ�鿡 ��µǴ� �̹��� ��ü�� �����Ǳ� ������
    // �÷��̾��� ���� Ư�� ��ġ���� �߻�ü�� �����ϴ� �Ͱ� ����
    // ������ȯ�� �ʿ��� ���� Transform.Scale.x�� -1, 1�� ���� ����
    private void SpriteFlipX(float x)
    {
        transform.localScale = new Vector3((x < 0 ? -1 : 1), 1, 1);
    }

    // player �����̵� �ִϸ��̼�
    public void StartSliding()
    {
        // Debug.Log("�����̵� �ִϸ��̼� ����");
        animator.SetBool("isSliding", true);
    }

    public void StopSliding()
    {
        // Debug.Log("�����̵� �ִϸ��̼� ����");
        animator.SetBool("isSliding", false);
    }

    // �޸��� �� �ִϸ��̼� ���
    public void SetSpeedMultiplier(float multiplier)
    {
        animator.speed = multiplier;
    }

    // player ��ǳ �ִϸ��̼�
    public void JangPoongShooting()
    {
        // Debug.Log("��ǳ �ִϸ��̼�");
        if (animator.GetBool("isShooting")) return; // �̹� ���� ���̸� ����
        animator.SetBool("isShooting", true);
    }

    // ��ǳ �ִϸ��̼� ����
    public void OnShootingEnd()
    {
        // Debug.Log("��ǳ �ִϸ��̼� ����");
        animator.SetBool("isShooting", false);
    }

    // �÷��̾� �׾��� ��
    public void PlayerDead()
    {
        animator.SetBool("Dead", true);
        // Debug.Log("�÷��̾� ���");
    }
}
