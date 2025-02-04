using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

public class Goblin_BatController : MonoBehaviour
{
    private Rigidbody2D rb;
    [SerializeField] private float speed = 5f;
    public float rotationSpeed = 120f; //초당 회전 속도
    //방망이 속도
    [SerializeField] private float damage = 1f; //방망이 공격력
    [SerializeField] private float rangeMinX = 3.0f; 
    [SerializeField] private float rangeMaxX = 4.2f;

    [SerializeField] private GameObject target;
    private Vector3 destination;

    [SerializeField] private float heightArc = 1; //포물선의 최고점 높이
    public Vector3 startPosition;
    private bool isStart; //포물선 이동 시작 여부

    private float gravity = -9.81f; //중력 가속도
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindWithTag("Player");
        startPosition = transform.position;

        rb = GetComponent<Rigidbody2D>();
        
        
        //목표점 설정
        SetDest();
        //초기 속도 계산 및 물리 처리 시작
        //SetInitialVelocity();
    }
    

    // Update is called once per frame
    void Update()
    {
        if (isStart)
        {
            float x0 = startPosition.x;
            float x1 = destination.x;
            float distance = x1 - x0;
            float nextX = Mathf.MoveTowards(transform.position.x, x1, speed * Time.deltaTime);
            float baseY = Mathf.Lerp(startPosition.y, destination.y, (nextX - x0) / distance);
            float arc = heightArc * (nextX - x0) * (nextX - x1) / (-0.25f * distance * distance);
            
            Vector3 nextPosition = new Vector3(nextX, baseY + arc, transform.position.z);
            
            
            //이동 속도 계산
            float moveDistanceX = nextX - transform.position.x;
            float currentSpeedX = moveDistanceX / Time.deltaTime;

            float moveDistanceY = nextPosition.y - transform.position.y;
            float currentSpeedY = moveDistanceY / Time.deltaTime;
            
            transform.position = nextPosition;

            if (transform.position == destination)
            {
                isStart = false;
                Debug.Log(rb.velocity);
                rb.velocity = new Vector2(currentSpeedX, rb.velocity.y);

            }
            
            // if (Vector3.Distance(transform.position,destination) < 0.01f)
            // {
            //     Debug.Log("목표지점 근처 도달");
            //     isStart = false;
            //     rb.velocity = new Vector2(currentSpeedX,currentSpeedY);
            // }
        }
        
        //Rotation
        float rotationAngle = rotationSpeed * Time.deltaTime;
        transform.Rotate(0, 0, rotationAngle);
    }

    
    /// <summary>
    /// 방망이 목표점(destination) 설정하는 메소드
    /// </summary>
    private void SetDest()
    {
        float x0 = gameObject.transform.position.x;
        float x1 = target.transform.position.x;

        float dis = Mathf.Abs(x1 - x0);
        float sign = Mathf.Sign(x1 - x0);
        if (dis <= rangeMaxX)
        {
            destination = target.transform.position;
        }
        else
        {
            float randomX = (float)Math.Round(Random.Range(rangeMinX, rangeMaxX), 3)*sign;
            destination = new Vector3(randomX, transform.position.y, transform.position.z);
        }
        
        
        //출발 flag 설정
        isStart = true;
    }
    /// <summary>
    /// 방망이 던지는 시점에서 방망이 초기 x,y 속력 설정하는 메소드. (RigidBody2D로 물리연산 처리)
    /// </summary>
    private void SetInitialVelocity()
    {
        //1. direction 계산
        Vector3 direction = destination - startPosition;

        float vx = 0f;
        float vy = 0f;
        if (direction.y > 1f) // 목표 지점이 시작 지점보다 높은 곳에 있고, 그 차이가 1보다 큰 경우
        {
            float timeToReach = Mathf.Abs(direction.x) / speed;
            vx = direction.x/timeToReach;
            vy = (direction.y - 0.5f * gravity * Mathf.Pow(timeToReach, 2)) / timeToReach;
        }
        else
        {
            vy = Mathf.Sqrt(2*heightArc*Mathf.Abs(gravity));
            vx = Mathf.Sqrt(speed * speed - vy * vy);
            // rb.gravityScale = 2;
        }
        
        
        //2. destination까지 도착하는데 걸리는 시간(이동 시간) 계산
        
        //Debug.Log($"time to reach :{timeToReach}");
        
        //3. 방망이 던지는 시점의 초기 velocity 계산
        /* 포물선 운동이므로 x축, y축 분리해서 속력 계산한다.
         * initial velocity x = delta x / total time
         *delta y = initial velocity y * total time + 1/2 * 중력가속도 * total time^2 이므로,
         * initial velocity y = (delta y - 1/2 * 중력가속도 * total time^2) / total time
         */

        rb.velocity = new Vector2(vx, vy);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log(other.name);
        if (other.CompareTag("Player"))
        {
            PlayerStatsController playerStatsController = Util.FindChild<PlayerStatsController>(other.gameObject, null, true);
            playerStatsController.OnAttacked(damage);
        }
        Destroy(gameObject);
    }
}
