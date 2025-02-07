using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


[Serializable]
public class StatisticData
{
    //플레이 누적 시간, 궁극기 사용 횟수, 장풍 쏜 횟수, 누적킬, 누적 사망
    
    public string lastPlayTime; //마지막 플레이 시간
    public int ultCnt;//궁국기 사용 횟수
    public int jpCnt; //장풍 쏜 횟수
    public int deathCnt; //누적 사망 수
    public int killCnt; //누적 킬 수
    public float totalPlayTime; //누적 플레이 시간
    

}