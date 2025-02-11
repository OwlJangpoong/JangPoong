using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NokmorDarkBullet : MonoBehaviour
{
    [Header("Bullet Stats")]
    [SerializeField] private float speed = 10.0f;
    // [SerializeField] private float jangPoongDistance = 5.0f;
    [SerializeField] private float damage = 1f;

    [SerializeField] public Vector2 direction;

    [Header("Bullet Status")] 
    private bool isColliding = false;
    [SerializeField] private float aliveTime = 5f;
    
    
    
    private Animator animator;
    private Collider2D _collider2D;
    private Rigidbody2D rb;
    private bool isShooting;

    public bool IsShooting
    {
        get => isShooting;
        set
        {
            isShooting = value;
            if (value)
            {
                Destroy(gameObject, aliveTime); // 일정 시간이 지나면 삭제
            }
        }
    }
    
    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
        
        //local sacale 설정
        transform.localScale = new Vector3((direction.x > 0 ? 1f : -1f), 1f, 1f);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        rb.velocity = new Vector2(0,0);
        _collider2D.enabled = false;
        
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerStatsController>().OnAttacked(damage);

            // 총알 삭제
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        _collider2D = GetComponent<Collider2D>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        
        
    }
    
    void Update()
    {
        if(isShooting)
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }
}
