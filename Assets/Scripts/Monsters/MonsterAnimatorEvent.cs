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
        weaponCollider.AttackPlayerByWeapon();
    }

    public void IncreaseSize()
    {
        transform.localScale = transform.localScale * 10;
    }
}
