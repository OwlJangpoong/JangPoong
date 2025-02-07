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
        Managers.Player.OnMonsterPointChanged -= SetUIMonsterPoint;
        Managers.Player.OnMonsterPointChanged += SetUIMonsterPoint;
        
        SetUIMonsterPoint(Managers.Player.MonsterPoint);
    }
    //오브젝트 파괴시 don't destroy로 살아있는 오브젝트의 이벤트를 구독 중이라면 해제해준다.
    //그렇지 않는 경우 오브젝트가 파괴되어도 don't destroy로 살이있는 오브젝트의 이벤트의 리스너 목록에 파괴된 오브젝트의 구독이 남아있게된다. 이벤트 발생시 파괴된 오브젝트를 참조하려하기 때문에 null reference error가 발생한다.
    private void OnDestroy()
    {
        if (Managers.Player != null)
        {
            Managers.Player.OnMonsterPointChanged -= SetUIMonsterPoint;
        }
    }


    public void SetUIMonsterPoint(int val)
    {
        maxMonsterPoint = Managers.Player.MaxMonsterPoint;
        currentMonsterPoint = val;
        monsterPointSlider.maxValue = maxMonsterPoint;
        monsterPointSlider.value = currentMonsterPoint;
    }

}
