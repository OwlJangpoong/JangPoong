using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Monster : MonoBehaviour
{
    [Header("Stat")] 
    public string statFileName;
    public MonsterStat stat; 
    [Header("Movement")] 
    [SerializeField] private LayerMask obstacleLayer; //장애물로 인식하는 레이어
    public Mon_MovementRigidbody2D movement2D;
    public SpriteRenderer spriteRenderer;
    public Vector3 direction = Vector3.zero;
    public Transform target;
    private GameObject player;
    
    
    [Header("Position")]
    [NonSerialized] public Vector3 startPos; 
    
    [Header("Distance/Time Range")]
    [SerializeField] private float moveRange = 2;
    public float scanRange = 8;
    public float attackRange = 1.5f;
    public float minMoveRangeX;
    public float maxMoveRangeX;

    public float thinkTime = 2f;
    private float thinkDelay;
    public float targetTime = 1f;

    public float ThinkDelay
    {
        get => thinkDelay;
        set => thinkDelay = value;
    }
    
    
    public float attackCoolTime = 1.5f; //공격 딜레이 시간
    private float attackDelay;
    public float AttackDelay
    {
        get
        {
            return attackDelay;
        }
        set => attackDelay = value;
    }
    
    [Header("Loot")] public List<LootItem> lootTable = new();


    protected Animator anim;
    
    protected virtual void Start()
    {
        Init(); //초기화
        
        //UI hp bar 설정
        if (gameObject.GetComponentInChildren<UI_HPBar>() == null)
        {
            
            Managers.UI.MakeWorldSpaceUI<UI_HPBar>(transform).gameObject.SetActive(false);
        }
    }
    
    private void Update()
    {
        CalcDirection();
        SetTarget();
        
        if(player==null) player = GameObject.FindWithTag("Player");

        if (attackDelay >= 0)
        {
            attackDelay -= Time.deltaTime;
        }

        if (thinkDelay >= 0)
        {
            thinkDelay -= Time.deltaTime;
        }
        
        
    }
    
    
    #region Event Functions
    public virtual void Init()
    {
        stat = gameObject.GetComponent<MonsterStat>();
        
        MonsterData md = Managers.Resource.Load<MonsterData>($"Data/Monster/{statFileName}");
        if (md != null)
        {
            stat.Init(md);
        }
        else
        {
            Debug.Log("Can't Find Monster Stat Data!");

        }

        //movement
        movement2D = GetComponent<Mon_MovementRigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        
        //Postion 설정
        startPos = transform.position;
        minMoveRangeX = startPos.x - moveRange;
        maxMoveRangeX = startPos.x + moveRange;
        
        
        //이벤트 구독
        stat.DieAction -= OnDie;
        stat.DieAction += OnDie;
        
        
        player = GameObject.FindWithTag("Player");
        
        SetDelayTime(); //타겟팅 딜레이 시간, 공격 딜레이 시간 랜덤하게 설정
    }

    protected virtual void OnDie()
    {
        Debug.Log("쥬금");
        // ✅ 몬스터 처치 횟수 증가
        Managers.Game.Statistic.killCnt++;
        Managers.Game.SaveStatisticData();
        
        //레이어 변경
        gameObject.layer = (int)Define.Layer.MonsterDie;
        
        //애니메이션
        anim.SetTrigger("Die");
        //사운드
        Managers.Sound.Play("77_flesh_02");
        //코루틴 정지
        StopAllCoroutines();
        //이동 정지
        movement2D.MoveTo(0);
        //몬스터포인트 추가
        Managers.Player.SetMonsterPoint(Managers.Player.MonsterPoint+ stat.monsterData.MonsterPoint);
        // Managers.Player.MonsterPoint += stat.monsterData.MonsterPoint;
        Debug.Log("몬스터포인트 추가 : " + stat.monsterData.MonsterPoint);
        
        //아이템 드랍
        Invoke(nameof(SpawnItem), 0.9f);
        
        //게임 관리 매니져로 수정할 예정
        Destroy(gameObject, 1f); //1초 후에 사라지기
        
        
    }

    public virtual void OnAttacked(float damage)
    {
        if (stat != null)
        {
            stat.OnAttacked(damage);
        }
    }

    #endregion

    #region Control
    //타겟 검사
    void SetTarget()
    {
        if (player!=null && direction.magnitude <= scanRange)
        {
            if (player.GetComponent<PlayerStatsController>().IsInvisible)
            {
                target = null;
            }
            else
            {
                target = player.transform;
            }
        }
        else
        {
            target = null;
        }
        
    }
    
    // 이동 방향에 따라 이미지 flip
    // spriteRenderer의 flipX를 사용하지 않고 localScale 값 조정으로 구현
    public void FlipSprite()
    {
        GameObject hpBar = GetComponentInChildren<UI_HPBar>(true).gameObject;
        Vector3 scale = transform.localScale;
        Vector3 hpBarScale = hpBar.transform.localScale;
        if (direction.x> 0)
        {
            scale.x = - Mathf.Abs(scale.x);
            transform.localScale = scale;
            hpBarScale.x = Mathf.Abs(hpBarScale.x);
            hpBar.transform.localScale = hpBarScale;
        }
        else if (direction.x< 0)
        {
            scale.x = Mathf.Abs(scale.x);
            transform.localScale = scale;
            hpBarScale.x = -Mathf.Abs(hpBarScale.x);
            hpBar.transform.localScale = hpBarScale;


        }

    }

    public void FlipSprite(Vector3 dir)
    {
        Vector3 scale = transform.localScale;
        if (dir.x> 0)
        {
            scale.x = - Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
        else if (dir.x< 0)
        {
            scale.x = Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    void CalcDirection() //사정거리 제한 없이 플레이어의 위치에 따른 거리를 계산
    {
        if (player != null)
        {
            direction.x = player.transform.position.x - transform.position.x;

        }
        else
        {
            Debug.Log($"{name} has no target(player)");
        }
    }

    protected void SpawnItem()
    {
        int itemCount = 0;
        foreach (LootItem lootItem in lootTable)
        {
            GameObject dropItem = null;
            if (Random.Range(0, 100f) <= lootItem.dropChance)
            {
                dropItem = Managers.Resource.Instantiate($"Items/Items_Player/{lootItem.item}");

                float offsetX = 0.5f * itemCount;
                dropItem.transform.position = new Vector3(transform.position.x+offsetX, transform.position.y + 0.3f,
                    transform.position.z);
                itemCount++;
            }
            
        }
    }


    /// <summary>
    /// 더 이상 갈 수 없는 장애물 감지시 -1 반환, 점프해서 이동할 수 있는 경우 1, 앞으로 나아갈 수 있는 경우 0 반환
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public virtual int Detect(Vector3 direction)
    {
        direction.y = 0;
        direction.z = 0;
        Debug.DrawRay(transform.position,direction.normalized*1.5f,Color.green);
        if (Physics2D.Raycast(transform.position, direction, 1.5f, LayerMask.GetMask("LevelN")))
        {
            // Debug.Log("올라갈수 없는 곳 감지");
            return -1;
        }
        else if (Physics2D.Raycast(transform.position, direction, 1.5f, LayerMask.GetMask("Level1")))
        {
            //Debug.Log("점프 가능");
            return 1;
        }
        else
        {
            return 0;
        }
    }

    private void SetDelayTime()
    {
        thinkTime = Mathf.Round(Random.Range(1.5f, 2.0f) * 10) / 10;
        attackCoolTime = Mathf.Round(Random.Range(1.0f, 1.5f) * 10) / 10;
    }
    

    #endregion

    #region Coroutine

    public IEnumerator CoTargetting()
    {
        movement2D.MoveTo(0); //움직임 정지 처리
        
        yield return new WaitForSeconds(targetTime);
        
        anim.SetBool("isFollow", true);
    }

    public IEnumerator CoJump(Vector3 dir)
    {
        
        if (!movement2D.isJump)
        {
            anim.SetBool("isJump", true);
            movement2D.isJump = true;
            movement2D.Jump();
        }
        yield return new WaitUntil(() => !movement2D.IsGrounded); //점프를 시작시 y 속력이 양수지만 아직 바닥과 붙어있을 수 있음. 바닥과 떨어지는 순간까지 기다림
        
        while (true)
        {
            //UpdateDirection();
            movement2D.MoveTo(Mathf.Sign(dir.x));
            //y축 속력에 따라 올라가는 or 내려가는 애니메이션 재생
            anim.SetFloat("velocityY", movement2D.Velocity.y);
         
            if (movement2D.IsGrounded)
            {
                //Debug.Log("바닥착지");
                //바닥에 착지시 동작 멈추고 반복문 종료
                anim.SetBool("isJump", false);
                movement2D.MoveTo(0);
                // //대기 행동 코루틴 호출
                yield return new WaitForSeconds(0.5f);
                movement2D.isJump = false;


                // if (target != null)
                // {
                //     anim.SetBool("isFollow", true);
                // }
                // else
                // {
                //     anim.SetBool("isFollow", false);
                // }
                yield break;
            }
            yield return null;
        }
    }

    #endregion
}
