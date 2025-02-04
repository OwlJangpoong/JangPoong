using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UI_PlayerHp : MonoBehaviour
{
    private Slider hpSlider;
    private TMP_Text hpText;
    private float maxHp;
    private float currentHp;
    
    void Awake()
    {
       Init();
        
    }
    private void Init()
    {
        hpSlider = GetComponent<Slider>();
        Managers.Player.OnHpChanged -= SetUIHp;
        Managers.Player.OnHpChanged += SetUIHp;

        hpText = GetComponentInChildren<TMP_Text>(true);
    }

    
    //오브젝트 파괴시 don't destroy로 살아있는 오브젝트의 이벤트를 구독 중이라면 해제해준다.
    //그렇지 않는 경우 오브젝트가 파괴되어도 don't destroy로 살이있는 오브젝트의 이벤트의 리스너 목록에 파괴된 오브젝트의 구독이 남아있게된다. 이벤트 발생시 파괴된 오브젝트를 참조하려하기 때문에 null reference error가 발생한다.
    private void OnDestroy()
    {
        if (Managers.Player != null)
        {
            Managers.Player.OnHpChanged -= SetUIHp;
        }
    }

    public void SetUIHp(float val)
    {
        maxHp = Managers.Player.MaxHp;
        currentHp = val;
        hpSlider.maxValue = maxHp;
        hpSlider.value = val;

        hpText.text = $"{currentHp}/{maxHp}";

    }
}
