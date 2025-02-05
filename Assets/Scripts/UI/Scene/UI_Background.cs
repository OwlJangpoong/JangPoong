using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Background : MonoBehaviour
{   
    private Image UIBackground;

    void Start()
    {
        Init();
    }

    private void Init()
    {
        UIBackground = GetComponent<Image>();
        Managers.Player.OnHpChanged -= SetUIBackground;
        Managers.Player.OnHpChanged += SetUIBackground;
        
        SetUIBackground(Managers.Player.Hp);
    }
    
    //오브젝트 파괴시 don't destroy로 살아있는 오브젝트의 이벤트를 구독 중이라면 해제해준다.
    //그렇지 않는 경우 오브젝트가 파괴되어도 don't destroy로 살이있는 오브젝트의 이벤트의 리스너 목록에 파괴된 오브젝트의 구독이 남아있게된다. 이벤트 발생시 파괴된 오브젝트를 참조하려하기 때문에 null reference error가 발생한다.
    private void OnDestroy()
    {
        if (Managers.Player != null)
        {
            Managers.Player.OnHpChanged -= SetUIBackground;
        }
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
