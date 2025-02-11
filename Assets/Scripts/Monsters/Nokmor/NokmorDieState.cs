using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NokmorDieState : MonsterDieState
{
    
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        monster = animator.GetComponentInParent<Monster>();
        monsterTransform = monster.transform;
        
        monster.FlipSprite();
        
        GameObject trigger = GameObject.FindWithTag("Trigger");
        trigger.GetComponentInChildren<TriggerForBattle>(true).EndBattle();
    }
    
    
    
    

}
