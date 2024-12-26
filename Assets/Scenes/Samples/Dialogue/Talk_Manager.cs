using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Talk_Manager : MonoBehaviour
{
    Dictionary<int, string[]> talkData;
    Dictionary<int, Sprite> portraitData;

    public Sprite[] portraitArr;
    void Awake()
    {
        talkData = new Dictionary<int, string[]>();
        portraitData = new Dictionary<int, Sprite>();
        GenerateData();
    }
    
    void GenerateData()
    {
        talkData.Add(10001, new string[] {"hello@0", "i am a triangle@1", "i wish i was a circle@2", "give me money!@0"});
        talkData.Add(100, new string[] {"boo"});
        talkData.Add(200, new string[] {"hehe"});
        
        portraitData.Add(10001+0, portraitArr[0]); //number of picture to use for convo
        portraitData.Add(10001+1, portraitArr[1]);
        portraitData.Add(10001+2, portraitArr[2]);
    }

    public string GetTalk(int id, int talkIndex)
    {
        if (talkIndex == talkData[id].Length)
        {
            return null;
        }
        else
        {
            return talkData[id][talkIndex];
        }
    }

    public Sprite GetPortrait(int id, int portraitIndex)
    {
        return portraitData[id + portraitIndex];
    }
}
