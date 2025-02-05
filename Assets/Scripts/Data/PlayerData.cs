using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


[Serializable]
public class PlayerData
{
    public string playerName;
    public float hp;
    public float maxHp;
    public int mana;
    public int maxMana;
    public int monsterPoint;
    public int maxMonsterPoint;
    public int currentJangPoongLevel;
    public int tokenCnt;
    public bool isRunning;
}


