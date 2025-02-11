using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class NokmorIdleState : MonsterIdleState
{
    private MonsterNokmor nokmor;

    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        nokmor = animator.GetComponentInParent<MonsterNokmor>();
        monsterTransform = nokmor.transform;
        nokmor.ThinkDelay = 0; // 처음엔 바로 움직이도록
        
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (nokmor.target != null)
        {
            float distanceToTarget = Vector3.Distance(nokmor.target.position, monsterTransform.position);
            
            // ✅ 플레이어가 공격 범위 내에 들어오면 공격
            if (distanceToTarget <= nokmor.attackRange)
            {
                if (nokmor.AttackDelay <= 0)
                {
                    // animator.SetTrigger("Attack"); // ✅ 공격 실행
                    //targetTime만큼 대기
                    nokmor.RandomAttack();
                }
                return;
            }
            // ✅ 플레이어가 ScanRange 내에 있으면 해당 방향으로 이동
            nokmor.movement2D.MoveTo(Mathf.Sign(nokmor.target.position.x - monsterTransform.position.x));
            nokmor.FlipSprite(nokmor.target.position - monsterTransform.position);
            return;
        }

        // ✅ 플레이어가 없으면 일정 시간 대기 후 랜덤 이동
        if (nokmor.ThinkDelay <= 0)
        {
            if (hasDest)
            {
                Vector3 distance = destination - monsterTransform.position;
                
                if (Mathf.Abs(distance.x) < 0.1f) // ✅ 목적지 도착
                {
                    hasDest = false;
                    nokmor.ThinkDelay = nokmor.thinkTime;
                    nokmor.movement2D.MoveTo(0);
                }
                else
                {
                    nokmor.movement2D.MoveTo(Mathf.Sign(distance.x)); // ✅ 점프 없이 좌우 이동만
                    nokmor.FlipSprite(distance);
                }
            }
            else
            {
                float randomXpos = Mathf.Round(Random.Range(nokmor.minMoveRangeX, nokmor.maxMoveRangeX) * 10f) / 10f;
                if (Mathf.Abs(randomXpos - monsterTransform.position.x) < 0.5f)
                {
                    randomXpos += (randomXpos < monsterTransform.position.x) ? -0.5f : 0.5f;
                    randomXpos = Mathf.Clamp(randomXpos, nokmor.minMoveRangeX, nokmor.maxMoveRangeX);
                }
                
                destination = monsterTransform.position;
                destination.x = randomXpos;
                
                hasDest = true;
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
    }
}
