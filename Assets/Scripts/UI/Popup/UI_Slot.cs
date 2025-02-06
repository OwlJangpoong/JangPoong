using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UI_Slot : MonoBehaviour
{
    private Button slot;
    public GameObject slotText;
    private Color emptyColor;   //C0C0C0    
    private Color fullColor;   //65CFAC
    
    
    public TMP_Text emptyTxt;
    public TMP_Text playerName;
    public TMP_Text lastPlayTime;
    public TMP_Text stage;
    
    private void Awake()
    {
        slot = GetComponent<Button>();
        emptyColor = Util.HexToColor("#C0C0C0");
        fullColor = Util.HexToColor("#65CFAC");
    }

    public void InitSlot(bool hasData, string playerName=null, string lastPlayTime=null, string stage=null)
    {
        ColorBlock cb = slot.colors;
        cb.normalColor = hasData ? fullColor : emptyColor;
        slot.colors = cb;
        emptyTxt.gameObject.SetActive(!hasData);
        
        slotText.SetActive(hasData);

        if (hasData)
        {
            this.playerName.text = playerName;
            this.lastPlayTime.text = lastPlayTime;
            this.stage.text = stage;
            
            
        }
    }
    
    
}