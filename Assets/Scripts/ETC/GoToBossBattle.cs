using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GoToBossBattle : MonoBehaviour
{
    private Button button;
    public Image fadePanel;  //페이드용 Panel UI 할당 필요
    // Start is called before the first frame update
    void Start()
    {
        button = GetComponent<Button>();
        button.onClick.RemoveListener(GoToBoss);
        button.onClick.AddListener(GoToBoss);
        Debug.Log("aafafasfasfasf");
        
    }


    public void GoToBoss()
    {
        
        Managers.Data.SetSlotNum(4);
        Managers.Data.DeleteAllFilesInSlot();
        
        Managers.Game.LoadBattleData();
        Managers.Player.ForceInit();
        Managers.Inventory.ForceInit();
        
        fadePanel.gameObject.SetActive(true);
        StartCoroutine(FadeEffect.Fade(fadePanel, 0f, 1f, fadeTime: 0.5f,
            action: () => Managers.Scene.LoadScene("Boss fight")));

    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
