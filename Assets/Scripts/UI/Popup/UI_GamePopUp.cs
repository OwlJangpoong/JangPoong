using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_GamePopUp : MonoBehaviour
{
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject pauseUI;
    [SerializeField] private GameObject settingUI;
    public static bool isPaused;
    

    private void Start()
    {
        pauseUI.SetActive(false);
        settingUI.SetActive(false);
        isPaused = false;
    }

    private void Update(){
        if (Input.GetKeyDown(KeyCode.Escape)){
            if(isPaused){
                OnResume();
            }
            else{
                OnPause();
            }
        }
    }


    #region MyRegion
        public void OnPause()
        {
            Managers.Game.PauseSession();
            SetTimeScale(0);
            playerUI.SetActive(false);
            pauseUI.SetActive(true);
            isPaused = true;
        }

        public void OnResume()
        {
            Managers.Game.ResumeSession();
            SetTimeScale(1);
            pauseUI.SetActive(false);
            playerUI.SetActive(true);
            isPaused = false;
        }

        public void Onsetting(){
            SetTimeScale(0);
            pauseUI.SetActive(false);
            settingUI.SetActive(true);
            isPaused = false;
        }

        public void OnExitGame()
        {
            Managers.Game.EndSession();
            Debug.Log("exit scene으로");
            Managers.Scene.LoadScene("Exit");
        }
        
    

    #endregion

    

    #region GameControl

    public void SetTimeScale(float scale)
    {
        Time.timeScale = scale;
    }

    #endregion
}
