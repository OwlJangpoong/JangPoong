using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_MonsterPoint : MonoBehaviour
{
    private TMP_Text monsterPointText;
    private int maxMonsterPoint;
    private int currentMonsterPoint;

    void Awake()
    {
        Init();

    }
    private void Init()
    {
        Managers.PlayerData.UpdateMonsterPointAction += SetUIMonsterPoint;

        monsterPointText = GetComponentInChildren<TMP_Text>(true);
    }

    public void SetUIMonsterPoint(int val)
    {
        maxMonsterPoint = Managers.PlayerData.maxMonsterPoint;
        currentMonsterPoint = Managers.PlayerData.MonsterPoint;

        monsterPointText.text = $"MonsterPoint : {currentMonsterPoint}/{maxMonsterPoint}";

    }

}
