using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Background : MonoBehaviour
{   
    private Image UIBackground;

    void Awake()
    {
        Init();
    }

    private void Init()
    {
        UIBackground = GetComponent<Image>();
        Managers.Player.OnHpChanged += SetUIBackground;
    }

    public void SetUIBackground(float currentHp)
    {
        // 체력에 따라 배경 이미지 변경
        if (currentHp > 5)
        {
            UIBackground.sprite = Resources.Load<Sprite>("Textures/Player/UI_expression1");
        }
        else if (currentHp > 3)
        {
            UIBackground.sprite = Resources.Load<Sprite>("Textures/Player/UI_expression2");
        }
        else
        {
            UIBackground.sprite = Resources.Load<Sprite>("Textures/Player/UI_expression3");
        }
    }
}
