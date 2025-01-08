using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class dialogueBox : MonoBehaviour
{
    public Talk_Manager talkManager;
    public GameObject talkPanel;
    public TextMeshProUGUI talkText;
    public Image PortraImage;
    public GameObject scanObject;
    public bool isAction;
    public int talkIndex;

    public void Action(GameObject scanObj)
    {
        scanObject = scanObj;
        ObjectData objectData = scanObject.GetComponent<ObjectData>();
        Talk(objectData.id, objectData.isNPC);
        
        talkPanel.SetActive(isAction);
        
    }

    void Talk(int id, bool isNPC)
    {
        string talkData = talkManager.GetTalk(id, talkIndex);
        if (talkData == null)
        {
            isAction = false;
            talkIndex = 0;
            return;
        }
        
        if (isNPC)
        {
            talkText.text = talkData.Split('@')[0];
            PortraImage.sprite = talkManager.GetPortrait(id, int.Parse(talkData.Split('@')[1]));
            PortraImage.color = new Color(1, 1, 1, 1);
        }
        else
        {
            talkText.text = talkData;
            PortraImage.color = new Color(0, 0, 0, 0);
        }

        isAction = true;
        talkIndex++;
    }
    
}
