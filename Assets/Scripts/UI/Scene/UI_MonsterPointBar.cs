using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class UI_MonsterPointBar : MonoBehaviour
{
    private Slider monsterPointSlider;
    private float maxMonsterPoint;
    private float currentMonsterPoint;
    
    void Awake()
    {
       Init();
        
    }
    private void Init()
    {
        monsterPointSlider = GetComponent<Slider>();
        Managers.PlayerData.UpdateMonsterPointAction += SetUIMonsterPoint;
    }

    public void SetUIMonsterPoint(int val)
    {
        maxMonsterPoint = Managers.PlayerData.maxMonsterPoint;
        currentMonsterPoint = Managers.PlayerData.MonsterPoint;
        monsterPointSlider.maxValue = maxMonsterPoint;
        monsterPointSlider.value = currentMonsterPoint;
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
