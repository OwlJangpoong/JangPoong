using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterAnimatorEvent : MonoBehaviour
{
    [SerializeField] private MonsterWeaponCollider weaponCollider;
    // Start is called before the first frame update
    void Start()
    {
        weaponCollider = Util.FindChild<MonsterWeaponCollider>(transform.parent.gameObject);
    }

    public void WeaponAttack()
    {
        Debug.Log($"hit player by weaponAttack");
        weaponCollider.AttackPlayerByWeapon();
    }

    public void IncreaseSize()
    {
        transform.localScale = transform.localScale * 10;
    }
    
    public void OnNorHit()
    {
        Debug.Log("[AE] OnNorHit fired on " + gameObject.name);
        GetComponentInParent<MonsterBlueLizardMan>()?.OnNorHit();
    }

    public void OnCriHit()
    {
        Debug.Log("[AE] OnCriHit fired on " + gameObject.name);
        GetComponentInParent<MonsterBlueLizardMan>()?.OnCriHit();
    }
    
    public void ClearAttacking()
    {
        // 애니메이션 이벤트에서 호출됨 → 부모 보스에 위임
        GetComponentInParent<MonsterNokmor>()?.ClearAttacking();
    }
}
