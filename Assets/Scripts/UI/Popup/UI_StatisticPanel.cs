using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_StatisticPanel : MonoBehaviour
{
    public TMP_Text title;
    public GameObject killSlot;
    public GameObject deathSlot;
    public GameObject jpSlot;
    public GameObject ultSlot;
    public GameObject totalTimeSlot;
    
    
    void Start()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        title.text = Managers.Game.Player.playerName + "의 플레이 결과";
        killSlot.GetComponentInChildren<TMP_Text>().text = "누적 킬 수 : "+ Managers.Game.Statistic.killCnt + "마리";
        deathSlot.GetComponentInChildren<TMP_Text>().text = "누적 사망 수 : "+ Managers.Game.Statistic.deathCnt + "회";
        jpSlot.GetComponentInChildren<TMP_Text>().text = "장풍 사용 횟수 : "+ Managers.Game.Statistic.jpCnt + "회";
        ultSlot.GetComponentInChildren<TMP_Text>().text = "궁극기 사용 횟수 : "+ Managers.Game.Statistic.ultCnt + "회";
        totalTimeSlot.GetComponentInChildren<TMP_Text>().text =
            "총 플레이 시간 : " + Managers.Game.Statistic.GetFormatPlayTime();

    }
    

    public void GoToMain()
    {
        //페이드효과 넣을것인가?
        Managers.Scene.LoadScene("Main");
    }
}
