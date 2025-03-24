using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_HpBar_Boss : UI_HPBar
{
    private TMP_Text tmpText;

    public override void Init()
    {
        base.Init();
        tmpText = GetComponentInChildren<TMP_Text>(true);
    }
    
    protected override void Update()
    {
        float value = monster.stat.CurrentHp;
        SetHpRatio(value);
    }

    public override void ShowHP()
    {
        return;
    }

    public override void SetHpRatio(float value)
    {
        base.SetHpRatio(value);
        tmpText.text = monster.stat.CurrentHp + "/" + monster.stat.monsterData.MaxHp;
    }
}
