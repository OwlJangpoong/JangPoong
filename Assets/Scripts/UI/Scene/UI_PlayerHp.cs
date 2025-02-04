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
        Managers.Player.OnHpChanged += SetUIHp;

        hpText = GetComponentInChildren<TMP_Text>(true);
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
