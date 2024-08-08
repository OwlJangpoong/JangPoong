using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.EventSystems;

public class PlayerController : MonoBehaviour
{
    private StageData stageData;
    // 점프 & 슬라이딩 데이터 설정
    [SerializeField]
    private KeyCode jumpKeyCode = KeyCode.W;
    [SerializeField]
    private KeyCode slideKeyCode = KeyCode.S;
    [SerializeField]
    private KeyCode runKeyCode = KeyCode.LeftShift;
    [SerializeField]
    private float slideDistance = 3.0f;
    [SerializeField]
    private float slideSpeed = 10.0f;
    [SerializeField]
    private LayerMask groundLayer;

    private MovementRigidbody2D movement;
    private PlayerAnimator playerAnimator;
    private CapsuleCollider2D capsuleCollider;
    private Rigidbody2D rb;
    private PlayerDataManager playerDataManager;
    private bool isSliding = false;
    private Vector2 slideDirection;
    private float slideRemainingDistance;
    private Vector2 originalColliderSize;
    private Vector2 originalColliderOffset;

    private float speedMultiplier = 2.0f;
    private bool isRunning = false;

    // 더블 클릭 (달리기) 데이터 설정
    /*private float doubleClickTimeLimit = 0.25f;
    private float lastClickTime = -1.0f;
    private KeyCode lastKeyPressed;*/

    private void Awake()
    {
        movement = GetComponent<MovementRigidbody2D>();
        playerAnimator = GetComponentInChildren<PlayerAnimator>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        playerDataManager = GetComponentInChildren<PlayerDataManager>();
        originalColliderSize = capsuleCollider.size;
        originalColliderOffset = capsuleCollider.offset;

        InvokeRepeating("RegenerateMana", 1f, 1f);  // 1占십몌옙占쏙옙 RegenerateMana 占쌨쇽옙占쏙옙 호占쏙옙
    }

    public void SetUp(StageData stageData)
    {
        this.stageData = stageData;
        transform.position = this.stageData.PlayerPosition;
    }

    private void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float offset = 0.5f + Input.GetAxisRaw("Sprint") * 0.5f;
        x *= offset;

        UpdateMove(x);
        UpdateJump();
        UpdateSlide();
        UpdateRun();
        UpdateJangPoong();
        playerAnimator.UpdateAnimation(x);

        /*CheckDoubleClick(KeyCode.A);
        CheckDoubleClick(KeyCode.D);*/
    }

    // 占싱듸옙
    private void UpdateMove(float x)
    {
        if (!isSliding)
        {
            if (isRunning) // 달리기
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
        //플레이어 x축 이동 한계 설정(240805)
        if (stageData == null) return;
        float xPos = Mathf.Clamp(transform.position.x, stageData.PlayerLimitMinX, stageData.PlayerLimitMaxX);
        transform.position = new Vector2(xPos, transform.position.y);
    }


    // 달리기
    private void UpdateRun()
    {
        if (Input.GetKeyDown(runKeyCode))
        {
            isRunning = true;
        }
        else if (Input.GetKeyUp(runKeyCode))
        {
            isRunning = false;
        }
    }

    // 점프
    private void UpdateJump()
    {
        if (Input.GetKeyDown(jumpKeyCode))
        {
            movement.Jump();
        }

        if (Input.GetKey(jumpKeyCode))
        {
            movement.IsLongJump = true;
        }
        else if (Input.GetKeyUp(jumpKeyCode))
        {
            movement.IsLongJump = false;
        }
    }

    // 占쏙옙占쏙옙占싱듸옙
    private void UpdateSlide()
    {
        if (Input.GetKeyDown(slideKeyCode))
        {
            if (!isSliding)
            {
                isSliding = true;
                slideRemainingDistance = slideDistance;
                slideDirection = new Vector2(transform.localScale.x, 0).normalized;
                capsuleCollider.size = new Vector2(capsuleCollider.size.x, 1.0f);
                capsuleCollider.offset = new Vector2(capsuleCollider.offset.x, capsuleCollider.offset.y - 0.41f);
                playerAnimator.StartSliding();
                Debug.Log("슬라이딩 시작");
            }
        }

        if (isSliding)
        {
            // 머리 위에 Ground 레이어 유무 체크
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, 1.0f, groundLayer);
            if (hit.collider != null)
            {
                Debug.Log("머리 위 블럭");
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

            if (slideRemainingDistance <= 0)
            {
                isSliding = false;
                capsuleCollider.size = originalColliderSize;
                capsuleCollider.offset = originalColliderOffset;
                rb.velocity = Vector2.zero;
                playerAnimator.StopSliding();
                Debug.Log("슬라이딩 종료");
            }
        }
    }

    // 장풍 발사
    private void UpdateJangPoong()
    {
        if (EventSystem.current.IsPointerOverGameObject()) {
            return;  ////UI 클릭시는 장풍 발사가 되지 않도록 처리 (240802 도현)
        }        
        if (Input.GetMouseButtonDown(0))
        {
            if (playerDataManager.mana >= playerDataManager.manaConsumption)
            {
                playerDataManager.mana -= playerDataManager.manaConsumption;

                Vector3 spawnPosition = transform.position;
                spawnPosition.y += isSliding ? -0.38f : -0.08f;

                GameObject jangPoong = Instantiate(playerDataManager.jangPoongPrefab, spawnPosition, Quaternion.identity);
                Rigidbody2D jangPoongRb = jangPoong.GetComponent<Rigidbody2D>();
                
                
                //장풍 alive time 설정가(240809) - 도현
                JangpoongController jc = jangPoong.GetComponent<JangpoongController>();
                jc.AliveTime = playerDataManager.jangPoongDistance / playerDataManager.jangPoongSpeed;
                
                Vector2 jangPoongDirection = new Vector2(transform.localScale.x, 0).normalized;
                jangPoongRb.velocity = jangPoongDirection * playerDataManager.jangPoongSpeed;
                jangPoong.transform.localScale = new Vector3((jangPoongDirection.x > 0 ? 0.5f : -0.5f), 0.5f, 0.5f); //수정
                Debug.Log(jangPoong.transform.localScale);
                
                playerAnimator.JangPoongShooting();
                
                //Destroy(jangPoong, playerDataManager.jangPoongDistance / playerDataManager.jangPoongSpeed); //Destory 로직 장풍 오브젝트에서 관리하도록 수정(240809) - 도현
            }
            else // 잔여 마나량 < 5
            {
                Debug.Log("마나량 부족");
            }
        }
    }

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
}
