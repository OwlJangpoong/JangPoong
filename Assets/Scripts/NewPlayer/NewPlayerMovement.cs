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
    // BoxCollider2D BoxCollider2D;
    CapsuleCollider2D CapsuleCollider2D;
    private PlayerDataManager playerDataManager;
    private NewPlayerAnimationController playerAnimator;


    // 점프 데이터
    [SerializeField] float jumpForce = 600f, speed = 5.0f;
    float moveX;

    public bool isSliding = false;    // 슬라이딩 중이면 true
    private Vector2 slideDirection;    // 슬라이딩 방향
    private float slideRemainingDistance;  // 남은 슬라이딩 거리
    private Vector2 originalColliderSize;  // 기존 Player Collider Size
    private Vector2 originalColliderOffset;  // 기존 Player Collider Offset

    [SerializeField] private float slideDistance = 3.0f;  // 슬라이딩 거리
    [SerializeField] private LayerMask groundLayer;  // Ground 레이어 설정

    public float speedMultiplier = 1.5f;                                // 달리기할 때 속도 배속

    public bool doubleJumpState = false;
    public bool isGround = false;
    public bool isJumping = false;
    public bool isDoubleJumping = false;
    private bool isRunning = false;                                        // 달리기 중이면 true

    [NonSerialized] public float slideSpeed = 7.0f;  // 슬라이딩 속도


    /*    public GameObject gameOver;
        public bool gameOverFlag = false; // true면 게임 오버 상태*/



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        CapsuleCollider2D = GetComponent<CapsuleCollider2D>();
        playerAnimator = GetComponent<NewPlayerAnimationController>();
        playerDataManager = GetComponentInChildren<PlayerDataManager>();

        originalColliderSize = CapsuleCollider2D.size;
        originalColliderOffset = CapsuleCollider2D.offset;

        /*        // GAMEOVER 비활성화
                gameOver.SetActive(false);

                // 플레이어 DieAction 등록
                Managers.PlayerData.DieAction -= SetPlayerDead;
                Managers.PlayerData.DieAction += SetPlayerDead;*/
    }

    void Update()
    {
        Jump();
        UpdateSlide();
        UpdateRun();
        UpdateJangPoong();

        float x = GetHorizontalInput();

        // 달리기
        if (isRunning)
        {
            speedMultiplier = 1.5f;
            playerAnimator.SetSpeedMultiplier(speedMultiplier);
        }
        else
        {
            speedMultiplier = 1.0f;
            playerAnimator.SetSpeedMultiplier(speedMultiplier);
        }

        playerAnimator.UpdateAnimation(x);

    }

    #region 이동 & 점프 & 더블 점프
    private float GetHorizontalInput()
    {
        float x = 0;

        if (Input.GetKey(Managers.KeyBind.leftKeyCode)) // 좌로 이동 키 눌렀을 때
        {
            x = -1;
        }
        else if (Input.GetKey(Managers.KeyBind.rightKeyCode)) // 우로 이동 키 눌렀을 때
        {
            x = 1;
        }
        return x;
    }

    // 점프
    void Jump()
    {

        if (!isSliding)
        {
            if (rb.velocity.y == 0)
            {
                isGround = true;
            }
            else
                isGround = false;

            // 1단 점프
            if (isGround && Input.GetKeyDown(Managers.KeyBind.jumpKeyCode))
            {
                JumpAddForce();
                isJumping = true;
                isDoubleJumping = false;  // 첫 점프에서는 더블 점프 false로 유지
                doubleJumpState = true; // 더블 점프 가능
            }
            // 더블 점프
            else if (doubleJumpState && Input.GetKeyDown(Managers.KeyBind.jumpKeyCode))
            {
                JumpAddForce();
                doubleJumpState = false;  // 더블 점프 불가능
                isDoubleJumping = true;   // 더블 점프 상태 true
                StartCoroutine(ResetDoubleJumpFlag());  // 더블 점프 상태 유지 타이머 시작
            }

            moveX = Input.GetAxis("Horizontal") * speed * speedMultiplier;
            rb.velocity = new Vector2(moveX, rb.velocity.y);
        }

    }

    // 더블 점프 상태 유지 타이머
    IEnumerator ResetDoubleJumpFlag()
    {
        yield return new WaitForSeconds(0.3f);
        isDoubleJumping = false;  // 더블 점프 애니메이션이 충분히 재생된 후 false로 변경
    }

    // 점프 Force
    void JumpAddForce()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * jumpForce);
    }
    #endregion

    #region 슬라이딩
    private void UpdateSlide()
    {
        if (isGround)
        {
            if (Input.GetKeyDown(Managers.KeyBind.slideKeyCode))  // 슬라이딩 키 눌렀을 때
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

    // 슬라이딩 시작
    private void StartSlide()
    {
        isSliding = true;
        slideRemainingDistance = slideDistance;
        slideDirection = new Vector2(transform.localScale.x, 0).normalized;

        // Player Collider 크기와 위치 조정
        CapsuleCollider2D.size = new Vector2(4.255104f, 4.660773f);
        CapsuleCollider2D.offset = new Vector2(0.5280471f, -2.357519f);

        // 슬라이딩 애니메이션 시작
        playerAnimator.StartSliding();

        //Debug.Log("슬라이딩 시작");
    }

    // 슬라이딩 유지
    private void HandleSliding()
    {
        // 머리 위에 Ground 레이어 유무 체크
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 1.0f, groundLayer);
        if (hit.collider != null)
        {
            // Debug.Log("머리 위에 장애물 존재");

            // 머리 위에 장애물이 있는 동안 슬라이딩 상태 유지
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

        // 슬라이딩 종료 조건
        if (slideRemainingDistance <= 0)
        {
            EndSlide();
        }
    }

    // 슬라이딩 종료
    private void EndSlide()
    {
        isSliding = false;
        // Player Collider 크기 및 위치 원래대로 복구
        CapsuleCollider2D.size = originalColliderSize;
        CapsuleCollider2D.offset = originalColliderOffset;
        rb.velocity = Vector2.zero;  // 속도 초기화

        // 슬라이딩 종료 애니메이션
        playerAnimator.StopSliding();

        //Debug.Log("슬라이딩 종료");
    }
    #endregion

    #region 달리기 토글
    private void UpdateRun()
    {

        if (Input.GetKeyDown(Managers.KeyBind.runKeyCode))
        {
            isRunning = !isRunning; // 달리기 상태 토글
        }
    }
    #endregion

    #region 장풍 발사
    private void UpdateJangPoong()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;  ////UI 클릭시는 장풍 발사가 되지 않도록 처리 (240802 도현)
        }
        if (Input.GetKeyDown(KeyCode.C))    // c키 로 장풍 발사
        {
            if (playerDataManager.Mana >= playerDataManager.manaConsumption)
            {
                playerDataManager.Mana -= playerDataManager.manaConsumption;

                Managers.Sound.Play("56_Attack_03");

                // 달리기 중에 장풍 속력 증가
                if (isRunning)
                    playerDataManager.jangPoongSpeed = 14;
                else
                    playerDataManager.jangPoongSpeed = 12;

                Vector3 spawnPosition = transform.position;
                spawnPosition.y += isSliding ? -0.38f : -0.08f;     // 슬라이딩 시에는 y값 -0.08f에서 장풍 발사되도록

                GameObject jangPoong = Instantiate(playerDataManager.jangPoongPrefab, spawnPosition, Quaternion.identity);
                Rigidbody2D jangPoongRb = jangPoong.GetComponent<Rigidbody2D>();



                //장풍 alive time 설정가(240809) - 도현
                JangpoongController jc = jangPoong.GetComponent<JangpoongController>();
                jc.AliveTime = playerDataManager.jangPoongDistance / playerDataManager.jangPoongSpeed;

                Vector2 jangPoongDirection = new Vector2(transform.localScale.x, 0).normalized;
                jangPoongRb.velocity = jangPoongDirection * playerDataManager.jangPoongSpeed;
                jangPoong.transform.localScale = new Vector3((jangPoongDirection.x > 0 ? 0.5f : -0.5f), 0.5f, 0.5f); //수정

                playerAnimator.JangPoongShooting();

                //Destroy(jangPoong, playerDataManager.jangPoongDistance / playerDataManager.jangPoongSpeed); //Destory 로직 장풍 오브젝트에서 관리하도록 수정(240809) - 도현
            }
            else // 잔여 마나량 < 5
            {
                Debug.Log("마나량 부족");
            }
        }
    }
    #endregion

    #region 사망
    /*    private void SetPlayerDead()
        {
            moveX = 0;
            gameOverFlag = true;
            playerAnimator.PlayerDead();
            Debug.Log("플레이어 죽음");
            gameOver.SetActive(true);
        }
        public void OnButtonClick_Restart()
        {
            SceneManager.LoadScene("1-1 tutorial");
        }
        public void OnButtonClick_Exit()
        {
            SceneManager.LoadScene("Exit");
        }*/

    #endregion
}
