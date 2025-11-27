using System.Collections;
using UnityEngine;

public class NokmorAttackState : MonsterAttackState
{
    private MonsterNokmor nokmor;

    // 범위 헬퍼
    private static bool In(float v, float minInc, float maxExc)
        => v >= minInc && v < maxExc;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        nokmor = animator.GetComponentInParent<MonsterNokmor>();

        float t = animator.GetFloat("AttackType");

        // ── 스킬 호출 (범위 기반)
        if (In(t, 0.0f, 0.1f))            // 0.0 = Swing
        {
            // 근접 휘두르기(애니 이벤트로 Collider on/off)
        }
        else if (In(t, 0.1f, 0.3f))      // 0.1~0.3 = DarkEnergy (기둥)
        {
            nokmor.DarkEnergyAttack();
        }
        else if (In(t, 0.3f, 0.5f))      // 0.3~0.5 = Dark Bullet
        {
            nokmor.DarkBulletAttack();
        }
        else if (In(t, 0.5f, 0.7f))      // 0.5~0.7 = Dark Creature
        {
            nokmor.DarkCreatureSummon();
        }
        else if (In(t, 0.7f, 0.9f))      // 0.7~0.9 = Gravity
        {
            nokmor.GravityFieldAttack();
        }
        else                              // 0.9 이상 = Black Hole
        {
            nokmor.BlackHoleAttack();
            Debug.Log($"블랙홀 됨");
        }

        // 공격 지속시간 후 종료
        float attackDuration = nokmor.GetAttackDuration();
        nokmor.StartCoroutine(EndAttackAfterDelay(attackDuration));
    }

    private IEnumerator EndAttackAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        nokmor.EndAttack(); // Idle 복귀(트리거/플래그 정리)
    }
}