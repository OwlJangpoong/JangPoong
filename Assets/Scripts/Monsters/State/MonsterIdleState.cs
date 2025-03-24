using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class MonsterIdleState : StateMachineBehaviour
{
    protected Monster monster;
    protected Transform monsterTransform;
    protected bool hasDest = false;
    protected Vector3 destination;
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        monster = animator.GetComponentInParent<Monster>();
        monsterTransform = monster.transform;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (monster.target != null)
        {
            if (monster.direction.magnitude <= monster.scanRange)
            {
                animator.SetTrigger("Targetting");
                return;
            }
        }

        if (monster.ThinkDelay <= 0)
        {
            if (hasDest)
            {
                Vector3 distance = destination - monsterTransform.position; //목적지까지의 distance(destination까지)
                
                //목적지에 도착하면 정지
                if (Mathf.Abs(distance.x) < 0.1f)
                {
                    hasDest = false;
                    monster.ThinkDelay = monster.thinkTime;
                    monster.movement2D.MoveTo(0);
                    
                    //Debug.Log("목적지까지 도착하면 정지합니다.");
                }

                //목적지까지 이동
                else
                {
                    if (monster.movement2D.isJump) return;
                    switch (monster.Detect(distance))
                    {
                        case -1:
                            monster.movement2D.MoveTo(0);
                            hasDest = false;
                            monster.ThinkDelay = monster.thinkTime;
                            break;
                        case 1:
                            if (!monster.movement2D.isJump)
                            {
                                monster.StartCoroutine(monster.CoJump(distance)); 
                            }
                            break;
                        case 0:
                            monster.movement2D.MoveTo(Mathf.Sign(distance.x));
                            break;
                    }
                    monster.FlipSprite(distance);
                }
            }
            else
            {
                float randomXpos = Mathf.Round(Random.Range(monster.minMoveRangeX, monster.maxMoveRangeX) * 10f) / 10f;
                if (Mathf.Abs(randomXpos - monsterTransform.position.x) < 0.5f)
                {
                    randomXpos += (randomXpos < monsterTransform.position.x) ? -0.5f : 0.5f;
                    randomXpos = Mathf.Clamp(randomXpos, monster.minMoveRangeX, monster.maxMoveRangeX);
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
