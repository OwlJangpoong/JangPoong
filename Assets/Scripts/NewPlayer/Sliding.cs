using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Sliding : MonoBehaviour
{
    DoubleJump dj;
    Rigidbody2D rb;
    BoxCollider2D BoxCollider2D;
    private bool isSliding = false;    // �����̵� ���̸� true
    private Vector2 slideDirection;    // �����̵� ����
    private float slideRemainingDistance;  // ���� �����̵� �Ÿ�
    private Vector2 originalColliderSize;  // ���� Player Collider Size
    private Vector2 originalColliderOffset;  // ���� Player Collider Offset

    [SerializeField] private float slideDistance = 3.0f;  // �����̵� �Ÿ�
    [SerializeField] private LayerMask groundLayer;  // Ground ���̾� ����

    [NonSerialized] public float slideSpeed = 5.0f;  // �����̵� �ӵ�

    void Start()
    {
        dj = GetComponent<DoubleJump>();
        BoxCollider2D = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();

        originalColliderSize = BoxCollider2D.size;
        originalColliderOffset = BoxCollider2D.offset;
    }

    void Update()
    {
        UpdateSlide();
    }

    #region �����̵�
    private void UpdateSlide()
    {
        if (dj.isGround)
        {
            if (Input.GetKeyDown(Managers.KeyBind.GetKeyCode(Define.ControlKey.slideKey)))  // �����̵� Ű ������ ��
            {
                if (!isSliding)
                {
                    StartSlide();
                }
            }
        }

        if (isSliding)
        {
            HandleSliding();
        }
    }

    private void StartSlide()
    {
        isSliding = true;
        slideRemainingDistance = slideDistance;
        slideDirection = new Vector2(transform.localScale.x, 0).normalized;

        // �ݶ��̴� ũ��� ��ġ ���� (�÷��̾ �۰�)
        BoxCollider2D.size = new Vector2(9f, 4.0f);
        BoxCollider2D.offset = new Vector2(BoxCollider2D.offset.x, BoxCollider2D.offset.y - 3.0f);

        Debug.Log("�����̵� ����");
    }

    private void HandleSliding()
    {
        // �Ӹ� ���� Ground ���̾� ���� üũ
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 1.0f, groundLayer);
        if (hit.collider != null)
        {
            Debug.Log("�Ӹ� ���� ��ֹ� ����");
            // �Ӹ� ���� ��ֹ��� �ִ� ���� �����̵� ���� ����
            rb.velocity = new Vector2(slideDirection.x * slideSpeed, rb.velocity.y);
            return;
        }

        float moveStep = slideSpeed * Time.deltaTime;
        if (moveStep > slideRemainingDistance)
        {
            moveStep = slideRemainingDistance;
        }

        rb.velocity = new Vector2(slideDirection.x * slideSpeed, rb.velocity.y);
        slideRemainingDistance -= moveStep;

        // �����̵� ���� ����
        if (slideRemainingDistance <= 0)
        {
            EndSlide();
        }
    }

    private void EndSlide()
    {
        isSliding = false;
        // �ݶ��̴� ũ�� �� ��ġ�� ������� ����
        BoxCollider2D.size = originalColliderSize;
        BoxCollider2D.offset = originalColliderOffset;
        rb.velocity = Vector2.zero;  // �ӵ� �ʱ�ȭ

        Debug.Log("�����̵� ����");
    }
    #endregion
}
