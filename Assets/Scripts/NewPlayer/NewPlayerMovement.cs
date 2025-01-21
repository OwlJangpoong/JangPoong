using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class NewPlayerMovement : MonoBehaviour
{

    Rigidbody2D rb;
    CapsuleCollider2D CapsuleCollider2D;
    private PlayerDataManager playerDataManager;
    public NewPlayerAnimationController playerAnimator;
    private MovementRigidbody2D movement;

    // ���� ������
    [SerializeField] float jumpForce = 600f, speed = 5.0f;
    float moveX;

    public bool isSliding = false;    // �����̵� ���̸� true
    private Vector2 slideDirection;    // �����̵� ����
    private float slideRemainingDistance;  // ���� �����̵� �Ÿ�
    private Vector2 originalColliderSize;  // ���� Player Collider Size
    private Vector2 originalColliderOffset;  // ���� Player Collider Offset

    #region Move Speed Control Variables / Methods
    [SerializeField] private float originSlideSpeed = 7.0f;

    [NonSerialized] public float ssgSlideSpeed;

    [NonSerialized] public float sgSlideSpeed;

    private void WalkSpeedState(int state)
    {
        switch (state)
        {
            case 0:
                //���� �ӵ����
                slideSpeed = originSlideSpeed;
                break;
            case 1:
                //ssg
                slideSpeed = ssgSlideSpeed;
                break;
            case 2:
                //sg
                slideSpeed = sgSlideSpeed;
                break;
        }
    }
    #endregion

    [SerializeField] private float slideDistance = 3.0f;  // �����̵� �Ÿ�
    [SerializeField] private LayerMask groundLayer;  // Ground ���̾� ����

    public float speedMultiplier = 1.5f;                                // �޸����� �� �ӵ� ���

    public bool doubleJumpState = false;
    public bool isGround = false;
    public bool isJumping = false;
    public bool isDoubleJumping = false;
    public bool isRunning = false;                                        // �޸��� ���̸� true

    [NonSerialized] public float slideSpeed = 7.0f;  // �����̵� �ӵ�


    public GameObject gameOver;
    public bool gameOverFlag = false; // true�� ���� ���� ����



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        CapsuleCollider2D = GetComponent<CapsuleCollider2D>();
        playerAnimator = GetComponent<NewPlayerAnimationController>();
        playerDataManager = GetComponentInChildren<PlayerDataManager>();
        movement = GetComponent<MovementRigidbody2D>();

        originalColliderSize = CapsuleCollider2D.size;
        originalColliderOffset = CapsuleCollider2D.offset;

        // �ӵ� ����
        slideSpeed = originSlideSpeed;
        ssgSlideSpeed = originSlideSpeed * 0.5f;
        sgSlideSpeed = originSlideSpeed * 0.75f;

        // speed controlAction ���
        movement.controlSpeedAction -= WalkSpeedState;
        movement.controlSpeedAction += WalkSpeedState;

        // DieAction ����
        Managers.PlayerData.DieAction -= SetPlayerDead;
        Managers.PlayerData.DieAction += SetPlayerDead;


        // GAMEOVER ��Ȱ��ȭ
        gameOver.SetActive(false);

    }

    void Update()
    {
        if(UI_GamePopUp.isPaused) return;
        Jump();
        UpdateSlide();
        UpdateRun();
        UpdateJangPoong();

        float x = GetHorizontalInput();

        if (gameOverFlag == true)
        {
            movement.MoveTo(0);
        }
        else
        {
            if (!isSliding)
            {
                movement.MoveTo(x);
            }

            if (isRunning)
            {
                movement.MoveTo(x * speedMultiplier);
            }

            playerAnimator.SetSpeedMultiplier(isRunning ? 1.5f : 1.0f);
            playerAnimator.UpdateAnimation(x);
        }

    }

    #region �̵� & ���� & ���� ����
    private float GetHorizontalInput()
    {
        float x = 0;

        if (Input.GetKey(Managers.KeyBind.GetKeyCode(Define.ControlKey.leftKey))) // �·� �̵� Ű ������ ��
        {
            x = -1;
        }
        else if (Input.GetKey(Managers.KeyBind.GetKeyCode(Define.ControlKey.rightKey))) // ��� �̵� Ű ������ ��
        {
            x = 1;
        }
        return x;
    }

    // ����
    void Jump()
    {

        if (!isSliding && !gameOverFlag)
        {
            if (movement.IsGrounded)
            // rb.velocity.y == 0
            {
                isGround = true;
            }
            else
                isGround = false;

            // 1�� ����
            if (isGround && Input.GetKeyDown(Managers.KeyBind.GetKeyCode(Define.ControlKey.jumpKey)))
            {
                JumpAddForce();
                isJumping = true;
                isDoubleJumping = false;  // ù ���������� ���� ���� false�� ����
                doubleJumpState = true; // ���� ���� ����
            }
            // ���� ����
            else if (doubleJumpState && Input.GetKeyDown(Managers.KeyBind.GetKeyCode(Define.ControlKey.jumpKey)))
            {
                JumpAddForce();
                doubleJumpState = false;  // ���� ���� �Ұ���
                isDoubleJumping = true;   // ���� ���� ���� true
                StartCoroutine(ResetDoubleJumpFlag());  // ���� ���� ���� ���� Ÿ�̸� ����
            }

            moveX = Input.GetAxis("Horizontal") * speed * speedMultiplier;
            rb.velocity = new Vector2(moveX, rb.velocity.y);
        }

    }

    // ���� ���� ���� ���� Ÿ�̸�
    IEnumerator ResetDoubleJumpFlag()
    {
        yield return new WaitForSeconds(0.3f);
        isDoubleJumping = false;  // ���� ���� �ִϸ��̼��� ����� ����� �� false�� ����
    }

    // ���� Force
    void JumpAddForce()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce);
    }
    #endregion

    #region �����̵�
    private void UpdateSlide()
    {
        if (isGround)
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

    // �����̵� ����
    private void StartSlide()
    {
        isSliding = true;
        slideRemainingDistance = slideDistance;
        slideDirection = new Vector2(transform.localScale.x, 0).normalized;

        // Player Collider ũ��� ��ġ ����
        CapsuleCollider2D.size = new Vector2(4.255104f, 4.660773f);
        CapsuleCollider2D.offset = new Vector2(0.5280471f, -2.357519f);

        // �����̵� �ִϸ��̼� ����
        playerAnimator.StartSliding();

        //Debug.Log("�����̵� ����");
    }

    // �����̵� ����
    private void HandleSliding()
    {
        // �Ӹ� ���� Ground ���̾� ���� üũ
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 1.0f, groundLayer);
        if (hit.collider != null)
        {
            // Debug.Log("�Ӹ� ���� ��ֹ� ����");

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

    // �����̵� ����
    private void EndSlide()
    {
        isSliding = false;
        // Player Collider ũ�� �� ��ġ ������� ����
        CapsuleCollider2D.size = originalColliderSize;
        CapsuleCollider2D.offset = originalColliderOffset;
        rb.velocity = Vector2.zero;  // �ӵ� �ʱ�ȭ

        // �����̵� ���� �ִϸ��̼�
        playerAnimator.StopSliding();

        //Debug.Log("�����̵� ����");
    }
    #endregion

    #region �޸��� ���
    private void UpdateRun()
    {

        if (Input.GetKeyDown(Managers.KeyBind.GetKeyCode(Define.ControlKey.runKey)))
        {
            isRunning = !isRunning; // �޸��� ���� ���
        }

    }
    #endregion

    #region ��ǳ �߻�
    private void UpdateJangPoong()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;  ////UI Ŭ���ô� ��ǳ �߻簡 ���� �ʵ��� ó�� (240802 ����)
        }
        if (Input.GetKeyDown(KeyCode.C))    // cŰ �� ��ǳ �߻�
        {
            if (playerDataManager.Mana >= playerDataManager.manaConsumption)
            {
                playerDataManager.Mana -= playerDataManager.manaConsumption;

                Managers.Sound.Play("56_Attack_03");

                // �޸��� �߿� ��ǳ �ӷ� ����
                if (isRunning)
                    playerDataManager.jangPoongSpeed = 14;
                else
                    playerDataManager.jangPoongSpeed = 12;

                Vector3 spawnPosition = transform.position;
                spawnPosition.y += isSliding ? -0.38f : -0.08f;     // �����̵� �ÿ��� y�� -0.08f���� ��ǳ �߻�ǵ���

                GameObject jangPoong = Instantiate(playerDataManager.jangPoongPrefab, spawnPosition, Quaternion.identity);
                Rigidbody2D jangPoongRb = jangPoong.GetComponent<Rigidbody2D>();



                //��ǳ alive time ������(240809) - ����
                JangpoongController jc = jangPoong.GetComponent<JangpoongController>();
                jc.AliveTime = playerDataManager.jangPoongDistance / playerDataManager.jangPoongSpeed;

                Vector2 jangPoongDirection = new Vector2(transform.localScale.x, 0).normalized;
                jangPoongRb.velocity = jangPoongDirection * playerDataManager.jangPoongSpeed;
                jangPoong.transform.localScale = new Vector3((jangPoongDirection.x > 0 ? 0.5f : -0.5f), 0.5f, 0.5f); //����

                playerAnimator.JangPoongShooting();

                //Destroy(jangPoong, playerDataManager.jangPoongDistance / playerDataManager.jangPoongSpeed); //Destory ���� ��ǳ ������Ʈ���� �����ϵ��� ����(240809) - ����
            }
            else // �ܿ� ������ < 5
            {
                Debug.Log("������ ����");
            }
        }
    }
    #endregion

    #region ���
    private void SetPlayerDead()
    {
        movement.MoveTo(0);
        gameOverFlag = true;
        playerAnimator.PlayerDead();
        Debug.Log("�÷��̾� ����");
        gameOver.SetActive(true);
    }
    public void OnButtonClick_Restart()
    {
        gameOverFlag = false;
        gameOver.SetActive(false);
        playerAnimator.animator.SetBool("Dead", false);

        // ������� �� HP�� 0�� ���·� ���۵Ǵ� ���� ����


    }
    public void OnButtonClick_Exit()
    {
        // Managers.Scene.LoadScene("Main");
    }

    #endregion
}
