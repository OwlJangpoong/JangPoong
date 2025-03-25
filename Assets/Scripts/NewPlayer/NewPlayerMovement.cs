using System;
using System.Collections;
using System.Collections.Generic;
// using UnityEditor.U2D.Aseprite;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class NewPlayerMovement : MonoBehaviour
{
    
    //플레이어 맵 이동 제한
    private StageData stageData; //삭제 금지!!! (250121)

    Rigidbody2D rb;
    CapsuleCollider2D CapsuleCollider2D;
    private PlayerStatsController playerStatsController;
    public NewPlayerAnimationController playerAnimator;
    private MovementRigidbody2D movement;
    private Transform transformForSpriteControl; //Sprite와 관련된 처리는 이 변수 이용(Sprite와 애니메이션은 자식 오브젝트로 이동시켰기 때문)

    // 점프 데이터
    [SerializeField] float jumpForce = 600f, speed = 5.0f;
    float moveX;

    public bool isSliding = false;    // 슬라이딩 중이면 true
    private Vector2 slideDirection;    // 슬라이딩 방향
    private float slideRemainingDistance;  // 남은 슬라이딩 거리
    private Vector2 originalColliderSize;  // 기존 Player Collider Size
    private Vector2 originalColliderOffset;  // 기존 Player Collider Offset

    #region Move Speed Control Variables / Methods
    [SerializeField] private float originSlideSpeed = 7.0f;

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

    [SerializeField] private float slideDistance = 3.0f;  // 슬라이딩 거리
    [SerializeField] private LayerMask groundLayer;  // Ground 레이어 설정

    public float speedMultiplier = 1.5f;                                // 달리기할 때 속도 배속

    public bool doubleJumpState = false;
    public bool isGround = false;
    public bool isJumping = false;
    public bool isDoubleJumping = false;
    //public bool isRunning = false;  // 달리기 중이면 true //전역 관리로 변경(250204)
    public bool isDown = false;

    [NonSerialized] public float slideSpeed = 7.0f;  // 슬라이딩 속도


    public GameObject gameOver;
    public bool gameOverFlag = false; // true면 게임 오버 상태

    [SerializeField] private GameObject levelUpEffect;

    
    
    //키업력 방지 플래그
    public bool isInputBlocked = false; // 입력 차단 변수 추가

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        CapsuleCollider2D = GetComponent<CapsuleCollider2D>();
        playerAnimator = GetComponentInChildren<NewPlayerAnimationController>();
        playerStatsController = GetComponent<PlayerStatsController>();
        movement = GetComponent<MovementRigidbody2D>();
        transformForSpriteControl = playerAnimator.transform; //Sprite 컴포넌트 갖는 오브젝트의 transform

        originalColliderSize = CapsuleCollider2D.size;
        originalColliderOffset = CapsuleCollider2D.offset;

        // 속도 설정
        slideSpeed = originSlideSpeed;
        ssgSlideSpeed = originSlideSpeed * 0.5f;
        sgSlideSpeed = originSlideSpeed * 0.75f;

        // speed controlAction 등록
        movement.controlSpeedAction -= WalkSpeedState;
        movement.controlSpeedAction += WalkSpeedState;

        // DieAction 설정
        Managers.Player.OnDie -= SetPlayerDead;
        Managers.Player.OnDie += SetPlayerDead;

        // // 모든 LevelUpToken 객체를 찾아 이벤트 구독 => playermanager 수정하면서 중앙에서 관리하는 이벤트를 구독하는 것으로 변경(250203). 모듈화, 확장성 용이
        // LevelUpToken[] levelUpTokens = FindObjectsOfType<LevelUpToken>();
        // foreach (var token in levelUpTokens)
        // {
        //     token.OnLevelUpTokenUpdated -= HandleLevelUpTokenUpdated;
        //     token.OnLevelUpTokenUpdated += HandleLevelUpTokenUpdated;
        // }
        
        //LevelUpToken 획득 파티클 이벤트
        Managers.Player.OnTokenCntChanged-=HandleLevelUpTokenUpdated;
        Managers.Player.OnTokenCntChanged+=HandleLevelUpTokenUpdated;

        //gameover 오브젝트 자동 할당을 위한 코드 추가(250121)
        GameObject ui_Game_Root = GameObject.FindWithTag("UI_Root");
        if (ui_Game_Root.GetComponentInChildren<UI_GameOverButtons>(true))
        {
            gameOver = ui_Game_Root.GetComponentInChildren<UI_GameOverButtons>(true).gameObject;
            gameOver.SetActive(false);
        }
       
        


    }
    
    //오브젝트 파괴시 don't destroy로 살아있는 오브젝트의 이벤트를 구독 중이라면 해제해준다.
    //그렇지 않는 경우 오브젝트가 파괴되어도 don't destroy로 살이있는 오브젝트의 이벤트의 리스너 목록에 파괴된 오브젝트의 구독이 남아있게된다. 이벤트 발생시 파괴된 오브젝트를 참조하려하기 때문에 null reference error가 발생한다.
    private void OnDestroy()
    {
        if (Managers.Player != null)
        {
            Managers.Player.OnDie -= SetPlayerDead;
            Managers.Player.OnTokenCntChanged -= HandleLevelUpTokenUpdated;
        }
    }


    void Update()
    {
        if (UI_GamePopUp.isPaused || isInputBlocked)
        {
            return;
        }
        Jump();
        UpdateSlide();
        UpdateRun();
        UpdateJangPoong();
        UpdateUlt();
        UpdateDown();

        // 기본 이동, HandleSliding 파트 FixedUpdate문으로 옮김 (250201 다인)

        /////플레이어 맵 이동 제한을 위한 코드! 삭제 금지 ----------
        if (stageData == null) return;
        float xPos = Mathf.Clamp(transform.position.x, stageData.PlayerLimitMinX, stageData.PlayerLimitMaxX);
        transform.position = new Vector2(xPos, transform.position.y);
        ////-------------
        
    }

    // 물리 엔진 업데이트되는 함수
    void FixedUpdate()
    {
        if (isInputBlocked) // ✅ 입력 차단되면 움직이지 않음
        {
            movement.MoveTo(0);
            playerAnimator.animator.SetBool("isWalking",false);
            return;
        }
        
        
        // 기본 이동
        float x = GetHorizontalInput();

        if (gameOverFlag == true || isDown)
        {
            movement.MoveTo(0);
        }
        else
        {
            if (!isSliding)
            {
                movement.MoveTo(x);
            }

            if (Managers.Player.IsRunning)
            {
                movement.MoveTo(x * speedMultiplier);
            }

            playerAnimator.SetSpeedMultiplier(Managers.Player.IsRunning ? 1.5f : 1.0f);
            playerAnimator.UpdateAnimation(x);
        }

        if(gameOverFlag == false)
        {
            // 슬라이딩 유지
            if (isSliding)
            {
                HandleSliding();
            }
        }

    }

    // 웅크리기
    private void UpdateDown()
    {
        if (isGround)
        {
            if (Input.GetKeyDown(Managers.KeyBind.GetKeyCode(Define.ControlKey.slideKey)))
            {
                isDown = true;

                // Player Collider 크기와 위치 조정
                CapsuleCollider2D.size = new Vector2(4.255104f, 4.660773f);
                CapsuleCollider2D.offset = new Vector2(0.5280471f, -2.357519f);

                playerAnimator.PlayerDown();
            }
            else if(Input.GetKeyUp(Managers.KeyBind.GetKeyCode(Define.ControlKey.slideKey)))
            {
                isDown = false;

                // Player Collider 크기 및 위치 원래대로 복구
                CapsuleCollider2D.size = originalColliderSize;
                CapsuleCollider2D.offset = originalColliderOffset;

                playerAnimator.PlayerUp();
            }
        }
    }

    #region 이동 & 점프 & 더블 점프
    private float GetHorizontalInput()
    {
        float x = 0;

        if (!isSliding)
        {
            if (Input.GetKey(Managers.KeyBind.GetKeyCode(Define.ControlKey.leftKey))) // 좌로 이동 키 눌렀을 때
            {
                x = -1;
            }
            else if (Input.GetKey(Managers.KeyBind.GetKeyCode(Define.ControlKey.rightKey))) // 우로 이동 키 눌렀을 때
            {
                x = 1;
            }
        }

        return x;
    }

    // 점프
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

            // 1단 점프
            if (isGround && Input.GetKeyDown(Managers.KeyBind.GetKeyCode(Define.ControlKey.jumpKey)))
            {
                JumpAddForce();
                isJumping = true;
                isDoubleJumping = false;  // 첫 점프에서는 더블 점프 false로 유지
                doubleJumpState = true; // 더블 점프 가능
            }
            // 더블 점프
            else if (doubleJumpState && Input.GetKeyDown(Managers.KeyBind.GetKeyCode(Define.ControlKey.jumpKey)))
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
        if (isGround && isDown)
        {
            if (Input.GetKeyDown(Managers.KeyBind.GetKeyCode(Define.ControlKey.rightKey)) ||
                Input.GetKeyDown(Managers.KeyBind.GetKeyCode(Define.ControlKey.leftKey)))  // 웅크리기 상태에서 좌우이동키 눌렀을 때
            {
                if (!isSliding)
                {
                    StartSlide();
                }
            }
        }

        /*     if (isSliding)
          {
              HandleSliding();
          }*/
    }

        // 슬라이딩 시작
        private void StartSlide()
    {
        isSliding = true;
        slideRemainingDistance = Managers.Player.IsRunning ? slideDistance*speedMultiplier : slideDistance;
        slideDirection = new Vector2(transformForSpriteControl.localScale.x, 0).normalized; //Player 프리팹 구조 변경으로 인한 코드 수정(250122)

        /*// Player Collider 크기와 위치 조정
        CapsuleCollider2D.size = new Vector2(4.255104f, 4.660773f);
        CapsuleCollider2D.offset = new Vector2(0.5280471f, -2.357519f);*/

        // 누른 좌우이동키에 따른 방향 설정
        if (Input.GetKey(Managers.KeyBind.GetKeyCode(Define.ControlKey.rightKey)))
        {
            transformForSpriteControl.localScale = new Vector3(1, transformForSpriteControl.localScale.y, transformForSpriteControl.localScale.z);
            slideDirection = Vector2.right;
        }
        else if (Input.GetKey(Managers.KeyBind.GetKeyCode(Define.ControlKey.leftKey)))
        {
            transformForSpriteControl.localScale = new Vector3(-1, transformForSpriteControl.localScale.y, transformForSpriteControl.localScale.z);
            slideDirection = Vector2.left;
        }

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
            rb.velocity = new Vector2(slideDirection.x * slideSpeed * (Managers.Player.IsRunning ? speedMultiplier : 1f), rb.velocity.y);
            // rb.velocity = new Vector2(slideDirection.x * slideSpeed, rb.velocity.y);
            return;
        }

        float moveStep = Managers.Player.IsRunning ? slideSpeed * Time.deltaTime * speedMultiplier : slideSpeed * Time.deltaTime;
        //float moveStep = slideSpeed * Time.deltaTime;
        if (moveStep > slideRemainingDistance)
        {
            moveStep = slideRemainingDistance;
        }

        rb.velocity = new Vector2(slideDirection.x * slideSpeed * (Managers.Player.IsRunning ? speedMultiplier : 1f), rb.velocity.y);
        // rb.velocity = new Vector2(slideDirection.x * slideSpeed, rb.velocity.y);

        slideRemainingDistance -= moveStep;
        // Debug.Log($"moveStep: {moveStep}, slideRemainingDistance: {slideRemainingDistance}");

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
        /*// Player Collider 크기 및 위치 원래대로 복구
        CapsuleCollider2D.size = originalColliderSize;
        CapsuleCollider2D.offset = originalColliderOffset;*/
        rb.velocity = Vector2.zero;  // 속도 초기화

        // 슬라이딩 종료 애니메이션
        playerAnimator.StopSliding();

        //Debug.Log("슬라이딩 종료");
    }
    #endregion

    #region 달리기 토글
    private void UpdateRun()
    {

        if (Input.GetKeyDown(Managers.KeyBind.GetKeyCode(Define.ControlKey.runKey)))
        {
            Managers.Player.IsRunning = !Managers.Player.IsRunning; // 달리기 상태 토글
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
        if (Input.GetKeyDown(Managers.KeyBind.GetKeyCode(Define.ControlKey.attackKey)))    // c키 로 장풍 발사
        {
            if (Managers.Player.Mana >= playerStatsController.ManaConsumption)
            {
                // ✅ 장풍 사용 횟수 증가
                Managers.Game.Statistic.jpCnt++;
                Managers.Game.SaveStatisticData();
                
                Managers.Player.SetMana(Managers.Player.Mana - playerStatsController.ManaConsumption);

                Managers.Sound.Play("56_Attack_03");
                

                Vector3 spawnPosition = transform.position;
                spawnPosition.y += isSliding ? -0.38f : -0.08f;     // 슬라이딩 시에는 y값 -0.08f에서 장풍 발사되도록

                GameObject jangPoong =
                    Instantiate(Managers.Player.jangPoongPrefab_list[Managers.Player.CurrentJangPoongLevel],
                        spawnPosition, Quaternion.identity);
                
                //direction만 설정해주고 나머지 설정은 장풍 오브젝트 내에서 처리(아래 코드들)
                jangPoong.GetComponent<JangpoongController>().jangPoongDirection = new Vector2(transformForSpriteControl.localScale.x, 0).normalized;
                
                ////================== 장풍 오브젝트가 생성될 때 오브젝트에서 설정되도록 수정(250202 도현)=================/////
                // 달리기 중에 장풍 속력 증가 
                // if (isRunning)
                //     Managers.Player.jangPoongSpeed = 14;
                // else
                //     Managers.Player.jangPoongSpeed = 12;
                
                // Rigidbody2D jangPoongRb = jangPoong.GetComponent<Rigidbody2D>();
                //
                //
                //
                // //장풍 alive time 설정가(240809) - 도현
                // JangpoongController jc = jangPoong.GetComponent<JangpoongController>();
                // jc.AliveTime = Managers.Player.jangPoongDistance / Managers.Player.jangPoongSpeed;
                //
                // Vector2 jangPoongDirection = new Vector2(transformForSpriteControl.localScale.x, 0).normalized; //플레이어 프리팹 수정으로 인한 코드 변경 (250122)
                //
                // jangPoongRb.velocity = jangPoongDirection * Managers.Player.jangPoongSpeed;
                // jangPoong.transform.localScale = new Vector3((jangPoongDirection.x > 0 ? 0.5f : -0.5f), 0.5f, 0.5f); //수정
                /////====================================////
                
                
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

    #region 궁극기 발사
    private void UpdateUlt()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;  ////UI 클릭시는 장풍 발사가 되지 않도록 처리 (240802 도현)
        }
        if (Input.GetKeyDown(Managers.KeyBind.GetKeyCode(Define.ControlKey.ultiKey)))    // x키로 궁극기 발사
        {
            if (Managers.Player.MonsterPoint >= Managers.Player.MaxMonsterPoint) // 몬스터 포인트가 50 이상일 경우
            {
                // ✅ 궁극기 사용 횟수 증가
                Managers.Game.Statistic.ultCnt++;
                Managers.Game.SaveStatisticData();
                //playerDataManager.MonsterPoint -= playerDataManager.maxMonsterPoint;
                Managers.Player.SetMonsterPoint(Managers.Player.MonsterPoint - Managers.Player.MaxMonsterPoint);

                Managers.Sound.Play("56_Attack_03"); // 일단 장풍이랑 같은 소리 나게 설정

                
                //250202 코드 이동 - UltController에서 처리//
                // 달리기 중에 궁극기 속력 증가
                // if (isRunning)
                //     Managers.Player.ultSpeed = 7;
                // else
                //     Managers.Player.ultSpeed = 6;

                Vector3 spawnPosition = transform.position;
                spawnPosition.y += isSliding ? -0.38f : -0.08f;     // 슬라이딩 시에는 y값 -0.08f에서 궁극기 발사되도록

                
                //250202 코드 리팩토링 - PlayerManager//
                GameObject ult = Instantiate(Managers.Player.ultPrefab, spawnPosition, Quaternion.identity); 
                
                //===250202 코드 리팩토링 - PlayerManager===//
                // Rigidbody2D ultRb = ult.GetComponent<Rigidbody2D>();
                //궁극기 alive time 설정가(240809) - 도현
                // UltController uc = ult.GetComponent<UltController>();
                // uc.AliveTime = 5f;

                
                ult.GetComponent<UltController>().ultDirection = new Vector2(transformForSpriteControl.localScale.x, 0).normalized; //플레이어 프리팹 수정으로 인한 코드 변경 (250122) + direction만 설정해주고 나머지 설정은 ult 오브젝트에서 처리(250202)
                
                // ultRb.velocity = ultDirection * Managers.Player.ultSpeed;
                // ult.transform.localScale = new Vector3((ultDirection.x > 0 ? 1f : -1f), 1f, 1f); // 궁극기 크기는 장풍의 2배 크기

                playerAnimator.JangPoongShooting(); //애니메이션은 장풍 쏠 때와 동일

                //Destroy(jangPoong, playerDataManager.jangPoongDistance / playerDataManager.jangPoongSpeed); //Destory 로직 장풍 오브젝트에서 관리하도록 수정(240809) - 도현
            }
            else // 몬스터 포인트 < 50
            {
                Debug.Log("몬스터 포인트 부족");
            }
        }
    }
    #endregion

    #region 사망
    private void SetPlayerDead()
    {
        movement.MoveTo(0);
        gameOverFlag = true;
        playerAnimator.PlayerDead();
        gameObject.layer = (int)Define.Layer.PlayerDamaged;
        Debug.Log("플레이어 죽음");
        gameOver.SetActive(true);
    }
    public void OnButtonClick_Restart()
    {
        gameOverFlag = false;
        gameOver.SetActive(false);
        playerAnimator.animator.SetBool("Dead", false);
    }
    #endregion

    #region 레벨업토큰 파티클
    private void HandleLevelUpTokenUpdated(int tokenCnt)
    {
        Instantiate(levelUpEffect, transform.position, Quaternion.identity);
    }
    #endregion


    #region 플레이어 맵 이동 컨트롤
    public void SetUp(StageData stageData)
    {
        this.stageData = stageData;
        transform.position = this.stageData.PlayerPosition;
    }
    

    #endregion
    
}
