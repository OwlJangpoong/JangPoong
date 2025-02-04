using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UI_MonsterPointBar : MonoBehaviour
{
    private Slider monsterPointSlider;
    private float maxMonsterPoint;
    private float currentMonsterPoint;
    
    void Start()
    {
       Init();
        
    }
    private void Init()
    {
        monsterPointSlider = GetComponent<Slider>();
        Managers.Player.OnMonsterPointChanged += SetUIMonsterPoint;
    }

    public void SetUIMonsterPoint(int val)
    {
        maxMonsterPoint = Managers.Player.MaxMonsterPoint;
        currentMonsterPoint = val;
        monsterPointSlider.maxValue = maxMonsterPoint;
        monsterPointSlider.value = currentMonsterPoint;
    }

}
