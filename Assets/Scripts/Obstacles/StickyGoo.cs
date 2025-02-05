using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StickyGoo : MonoBehaviour
{
    //데미지
    [SerializeField] private float damage = 0.2f;
    [SerializeField] private float damageDelayTime = 2f;

    
    //속도 회복 딜레이
    [SerializeField] private float delayTime = 10f;

    private bool isCoroutineRunning = false;
    private MovementRigidbody2D playerMovement = null;
    private PlayerStatsController playerStatsController;
    
    //2. 범위 내에서 속도 3/4배로 감소
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerStatsController = other.GetComponent<PlayerStatsController>();

            if (playerStatsController != null)
            {
                playerMovement = other.GetComponent<MovementRigidbody2D>();
                playerMovement.IsInSg = true;
            }
            
            
        }
    }

    
    //1. 데미지 2초당 0.2 hp 감소
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player")&&!isCoroutineRunning && !playerStatsController.IsInvincible)
        {
            isCoroutineRunning = true;
            StartCoroutine(nameof(AttackPlayerOverTime));
        }
    }

    private IEnumerator AttackPlayerOverTime()
    {
        //attack
        
        playerStatsController.OnAttacked(damage);
        // Managers.Player.OnAttacked(damage);
        
        //delay
        yield return new WaitForSeconds(damageDelayTime);
        isCoroutineRunning = false;
    }

    
    //3. 10초 딜레이 후 속도 회복!
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StopAllCoroutines();
            //속도 회복
            isCoroutineRunning = false;
            
            //10초후에 속도 회복 시키기
            StartCoroutine(nameof(RestoreSpeedAfterDelay));
        }
    }

    private IEnumerator RestoreSpeedAfterDelay()
    {
        yield return new WaitForSeconds(delayTime);
        if (playerMovement == null)
        {
            Debug.LogError("sticky goo : 플레이어 속도 정상화에 오류가 발생했습니다! 플레이어 movement2d를 찾을 수 없습니다.");
        }
        else
        {
            playerMovement.IsInSg = false;
        }
    }

    
    
    
}
