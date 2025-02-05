using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UI_PlayerHp_old : MonoBehaviour
{
    [SerializeField] private List<Slider> hpHeartList;
    private float maxHp;
    


    private void Awake()
    {
        Managers.Player.OnHpChanged += SetUIHp;
    }


    //Max HP 증가
    public void IncreseMaxHp()
    {
        GameObject go = Managers.Resource.Instantiate("UI/Scene/UI_Hp_Heart 00", transform); //현재 오브젝트의 자식으로 삽입
        go.name = $"UI_Hp_Heart {maxHp:D2}"; //두자리 수로 표현
        Slider newHeart = go.GetComponent<Slider>();
        newHeart.value = 1;
        
        hpHeartList.Add(newHeart);
        maxHp = hpHeartList.Count;
    }
    
    //HP 증, 감시 UI 처리
    public void SetUIHp(float val)
    {
        //초기화
        InitUIHp();
        
        //채우기
        maxHp = hpHeartList.Count;
        int integerPart = Mathf.FloorToInt(val);
        float decimalPart = val - integerPart;
        for (int i = 0; i < integerPart; i++)
        {
            hpHeartList[i].value = 1;
        }

        if (integerPart < maxHp)
        {
            hpHeartList[integerPart].value = decimalPart;
        }
    }

    private void InitUIHp()
    {
        foreach (Slider hpHeart in hpHeartList)
        {
            hpHeart.value = 0;
        }
    }
}
