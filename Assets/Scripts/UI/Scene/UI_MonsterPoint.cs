using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_MonsterPoint : MonoBehaviour
{
    private TMP_Text monsterPointText;
    private int maxMonsterPoint;
    private int currentMonsterPoint;

    void Start()
    {
        Init();

    }
    private void Init()
    {
        monsterPointText = GetComponentInChildren<TMP_Text>(true);
        Managers.Player.OnMonsterPointChanged += SetUIMonsterPoint;
    }

    public void SetUIMonsterPoint(int val)
    {
        maxMonsterPoint = Managers.Player.MaxMonsterPoint;
        currentMonsterPoint = val;

        monsterPointText.text = $"MonsterPoint : {currentMonsterPoint}/{maxMonsterPoint}";
    }
}
