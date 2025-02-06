using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destination : MonoBehaviour
{
    public AudioClip audioClip;
    public string NextSceneName;

    [SerializeField] private GameObject flagEffect;
    private GameObject keyInfo;

    [SerializeField] private UI_FadeController fadeController;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player"))
        {
            if (keyInfo == null)
            {
                keyInfo = Managers.UI.MakeWorldSpaceUI<UI_KeyInfo>(transform).gameObject;
            }
            //keyInfo 활성화
            keyInfo.SetActive(true);

            if (Input.GetKey(KeyCode.F))
            {
                GoToNextState();
            }
            
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && Input.GetKey(KeyCode.F))
        {
            Managers.Inventory.CommitInventoryState();
            Managers.Player.CommitPlayerData();
            GoToNextState();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (keyInfo != null)
            {
                keyInfo.SetActive(false);
            }
        }
    }


    private void GoToNextState()
    {
        fadeController = FindObjectOfType<UI_FadeController>();

        //파티클 뿌리고
        Instantiate(flagEffect, transform.position, Quaternion.identity);
            
        //효과음 빰빠밤
        Managers.Sound.Play(audioClip);
            
        //씬 이동
        //Managers.Scene.LoadSceneAfterDelay(NextSceneName,1f);
        // 페이드아웃 후 진행할 액션 등록
        fadeController.RegisterCallback(OnFadeOutComplete); 
        // FadeOut 호출
        fadeController.FadeOut();
    }

    private void OnFadeOutComplete(){
        StartCoroutine(Managers.Scene.LoadSceneAfterDelay(NextSceneName, 0.5f));
    }
}
