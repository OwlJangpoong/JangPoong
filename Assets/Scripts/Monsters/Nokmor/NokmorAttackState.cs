using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NokmorAttackState : MonsterAttackState
{
    private MonsterNokmor nokmor;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        nokmor = animator.GetComponentInParent<MonsterNokmor>();
        
        float attackDuration = nokmor.GetAttackDuration(); 
        nokmor.StartCoroutine(EndAttackAfterDelay(attackDuration));
        
        
        //스킬 호출
        // NokmorAttackState.cs - OnStateEnter 내부
        float attackType = animator.GetFloat("AttackType");

        if (Mathf.Approximately(attackType, 0.2f))
        {
            nokmor.DarkEnergyAttack();
        }
        else if (Mathf.Approximately(attackType, 0.4f))
        {
            nokmor.DarkBulletAttack();
        }
        else if (Mathf.Approximately(attackType, 0.6f))
        {
            nokmor.DarkCreatureSummon();
        }
        else if (Mathf.Approximately(attackType, 0.8f))
        {
            nokmor.GravityFieldAttack();
        }
        else if (Mathf.Approximately(attackType, 1.0f))
        {
            nokmor.BlackHoleAttack();
        }
    }
    
    
    private IEnumerator EndAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        nokmor.EndAttack(); // Idle 상태로 복귀
    }
}
