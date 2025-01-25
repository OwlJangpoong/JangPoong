using System;
using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private StageData stageData;
    // 슬라이딩 데이터 설정
    [SerializeField]
    private float slideDistance = 3.0f;                                    // 슬라이딩 거리


    #region Move Speed Control Variables / Methods
    [SerializeField] private float originSlideSpeed = 7.0f;
    [NonSerialized] public float slideSpeed;

    [NonSerialized] public float ssgSlideSpeed;

    [NonSerialized] public float sgSlideSpeed;
    
    private void WalkSpeedState(int state)
    {
        switch (state)
        {
            case 0:
                //원래 속도대로
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
   
    // 슬라이딩 속도
    [SerializeField]
    private LayerMask groundLayer;                                    // 슬라이딩 시 바닥/장애물 레이어

    private MovementRigidbody2D movement;
    private PlayerAnimator playerAnimator;
    private CapsuleCollider2D capsuleCollider;
    private Rigidbody2D rb;
    private PlayerDataManager playerDataManager;

    private bool isSliding = false;                                            // 슬라이딩 중이면 true
    private Vector2 slideDirection;                                         // 슬라이딩 방향
    private float slideRemainingDistance;                            // 남은 슬라이딩 거리 (머리 위에 장애물 있을 때)
    private Vector2 originalColliderSize;                              // 기존 Player Collider Size
    private Vector2 originalColliderOffset;                           // 기존 Player Collider Offset

    public float speedMultiplier = 2.0f;                                // 달리기할 때 속도 배속
    private bool isRunning = false;                                        // 달리기 중이면 true

    public GameObject gameOver;
    public bool gameOverFlag = false; // true면 게임 오버 상태

    // 더블 클릭 (달리기) 데이터 설정
    /*private float doubleClickTimeLimit = 0.25f;
    private float lastClickTime = -1.0f;
    private KeyCode lastKeyPressed;*/

    // private KeyBindingManager keyBindingManager;

    private void Awake()
    {
        movement = GetComponent<MovementRigidbody2D>();
        playerAnimator = GetComponentInChildren<PlayerAnimator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        playerDataManager = GetComponentInChildren<PlayerDataManager>();
        // keyBindingManager = KeyBindingManager.Instance;

        originalColliderSize = capsuleCollider.size;
        originalColliderOffset = capsuleCollider.offset;
        
        //speed 설정
        slideSpeed = originSlideSpeed;
        ssgSlideSpeed = originSlideSpeed * 0.5f;
        sgSlideSpeed = originSlideSpeed * 0.75f;
        
        //action 등록
        movement.controlSpeedAction -= WalkSpeedState;
        movement.controlSpeedAction += WalkSpeedState;

        // GAMEOVER 비활성화
        gameOver.SetActive(false);

        // 플레이어 DieAction 등록
        Managers.PlayerData.DieAction -= SetPlayerDead;
        Managers.PlayerData.DieAction += SetPlayerDead;

    }

    public void SetUp(StageData stageData)
    {
        this.stageData = stageData;
        transform.position = this.stageData.PlayerPosition;
    }

    private void Update()
    {
        float x = GetHorizontalInput();
        float offset = 0.5f + Input.GetAxisRaw("Sprint") * 0.5f;
        x *= offset;

        if (gameOverFlag == false)
        {
            UpdateMove(x);
            UpdateJump();
            UpdateSlide();
            UpdateRun();
            UpdateJangPoong();
        }
        playerAnimator.UpdateAnimation(x);

    }

    private float GetHorizontalInput()
    {
        float x = 0;
        if (Input.GetKey(Managers.KeyBind.GetKeyCode(Define.ControlKey.leftKey)))
        {
            x = -1;
        }
        else if (Input.GetKey(Managers.KeyBind.GetKeyCode(Define.ControlKey.rightKey)))
        {
            x = 1;
        }
        return x;
    }

    #region 이동
    private void UpdateMove(float x)
    {
        if (!isSliding)
        {
            if (isRunning)
            {
                x *= speedMultiplier;
                playerAnimator.SetSpeedMultiplier(speedMultiplier);
            }
            else
            {
                playerAnimator.SetSpeedMultiplier(1.0f);
            }
            movement.MoveTo(x);
        }

        if (stageData == null) return;
        float xPos = Mathf.Clamp(transform.position.x, stageData.PlayerLimitMinX, stageData.PlayerLimitMaxX);
        transform.position = new Vector2(xPos, transform.position.y);
    }
    #endregion

    #region 달리기
    private void UpdateRun()
    {
        /*if (Input.GetKeyDown(Managers.KeyBind.runKeyCode))
        {
            isRunning = true;
        }
        else if (Input.GetKeyUp(Managers.KeyBind.runKeyCode))
        {
            isRunning = false;
        }*/

        if (Input.GetKeyDown(Managers.KeyBind.GetKeyCode(Define.ControlKey.runKey)))
        {
            isRunning = !isRunning; // 달리기 상태 토글
        }
    }
    #endregion

    #region 점프, 더블점프
    private void UpdateJump()
    {
        if (Input.GetKeyDown(Managers.KeyBind.GetKeyCode(Define.ControlKey.jumpKey)))
        {
            movement.Jump();
        }

        // 더블 점프 체크
        if (Input.GetKey(Managers.KeyBind.GetKeyCode(Define.ControlKey.jumpKey)))
        {
            movement.IsLongJump = true;
        }
        else if (Input.GetKeyUp(Managers.KeyBind.GetKeyCode(Define.ControlKey.jumpKey)))
        {
            movement.IsLongJump = false;
        }
    }
    #endregion

    #region 슬라이딩
    private void UpdateSlide()
    {
        if (movement.IsGrounded)
        {                          // 공중 슬라이딩 방지 (isGrounded : 바닥에 닿았을 때 true)
            if (Input.GetKeyDown(Managers.KeyBind.GetKeyCode(Define.ControlKey.slideKey)))     // 슬라이딩 키 눌렀을 때
            {
                if (!isSliding)
                {
                    isSliding = true;
                    slideRemainingDistance = slideDistance;
                    slideDirection = new Vector2(transform.localScale.x, 0).normalized;

                    // Player Capsule Collider 2D size, offset 조정
                    capsuleCollider.size = new Vector2(capsuleCollider.size.x, 1.0f);
                    capsuleCollider.offset = new Vector2(capsuleCollider.offset.x, capsuleCollider.offset.y - 0.41f);
                    playerAnimator.StartSliding();
                    // Debug.Log("슬라이딩 시작");
                }
            }
        }

        if (isSliding)                                                        // 슬라이딩 시
        {
            // 머리 위에 Ground 레이어 유무 체크
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 1.0f, groundLayer);
            if (hit.collider != null)
            {
                // Debug.Log("머리 위 블럭");

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

            if (slideRemainingDistance <= 0)                  // 슬라이딩 종료 시
            {
                isSliding = false;
                capsuleCollider.size = originalColliderSize;
                capsuleCollider.offset = originalColliderOffset;
                rb.velocity = Vector2.zero;
                playerAnimator.StopSliding();
                // Debug.Log("슬라이딩 종료");
            }
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
        if (Input.GetKeyDown(KeyCode.Space))    // 스페이스 바로 장풍 발사
        {
            if (playerDataManager.Mana >= playerDataManager.manaConsumption)
            {
                playerDataManager.Mana -= playerDataManager.manaConsumption;

                Managers.Sound.Play("56_Attack_03");

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

    #region 더블 클릭 체크(삭제되었음)
    /*   // 더블 클릭 체크
    private void CheckDoubleClick(KeyCode key)
    {
        if (Input.GetKeyDown(key))
        {
            if (Time.time - lastClickTime < doubleClickTimeLimit && lastKeyPressed == key)
            {
                isDoubleClicking = true;
                StopAllCoroutines();
                StartCoroutine(DoubleClickTimer());
            }
            else
            {
                lastClickTime = Time.time;
                lastKeyPressed = key;
            }
        }

        if (Input.GetKeyUp(key))
        {
            if (isDoubleClicking)
            {
                StopAllCoroutines();
                StartCoroutine(DoubleClickCooldown());
            }
        }
    }

    private IEnumerator DoubleClickTimer()
    {
        while (Input.GetKey(lastKeyPressed))
        {
            yield return null;
        }
        isDoubleClicking = false;
    }

    private IEnumerator DoubleClickCooldown()
    {
        yield return new WaitForSeconds(0.5f);
        isDoubleClicking = false;
    }
=======
    /*   // 더블 클릭 체크
       private void CheckDoubleClick(KeyCode key)
       {
           if (Input.GetKeyDown(key))
           {
               if (Time.time - lastClickTime < doubleClickTimeLimit && lastKeyPressed == key)
               {
                   isDoubleClicking = true;
                   StopAllCoroutines();
                   StartCoroutine(DoubleClickTimer());
               }
               else
               {
                   lastClickTime = Time.time;
                   lastKeyPressed = key;
               }
           }

           if (Input.GetKeyUp(key))
           {
               if (isDoubleClicking)
               {
                   StopAllCoroutines();
                   StartCoroutine(DoubleClickCooldown());
               }
           }
       }

       private IEnumerator DoubleClickTimer()
       {
           while (Input.GetKey(lastKeyPressed))
           {
               yield return null;
           }
           isDoubleClicking = false;
       }

       private IEnumerator DoubleClickCooldown()
       {
           yield return new WaitForSeconds(0.5f);
           isDoubleClicking = false;
       }*/
    #endregion

    #region 사망
    private void SetPlayerDead()
    {
        movement.MoveTo(0);
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
    }

    #endregion
}
