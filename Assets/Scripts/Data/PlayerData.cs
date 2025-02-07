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



    public PlayerData(string playerName, float hp, float maxhp, int mana, int maxMana, int monsterPoint, int maxMonsterPoint, int currentJangPoongLevel, int tokenCnt, bool isRunning)
    {
        this.playerName = playerName;
        this.hp = hp;
        this.maxHp = maxhp;
        this.mana = mana;
        this.maxMana = maxMana;
        this.monsterPoint = monsterPoint;
        this.maxMonsterPoint = maxMonsterPoint;
        this.currentJangPoongLevel = currentJangPoongLevel;
        this.tokenCnt = tokenCnt;
        this.isRunning = isRunning;
    }
}




