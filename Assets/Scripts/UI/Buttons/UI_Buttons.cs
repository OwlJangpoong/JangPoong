using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UI_Buttons: MonoBehaviour
{
    public Image fadePanel;  //페이드용 Panel UI 할당 필요
    public void OnClickNewGame()
    {
        Debug.Log($"슬롯{Managers.Data.SlotNum} : 새 게임을 시작합니다.");
        // Managers.RestPlayData(); //PlayPrefs 사용 안함
        
        Managers.Data.DeleteAllFilesInSlot(); //슬롯 내 모든 파일 삭제
        
        //필요한 데이터 파일 초기화 및 셋팅
        //Inventory Data, PlayerData, StatisticData, ProgressData
        Managers.Player.Init();
        Managers.Inventory.Init();
        //통계데이터
        Managers.Game.LoadStatisticData();
        //프로그래스 데이터

    }

    public void OnClickLoad()
    {
        Debug.Log($"이어하기 : 슬롯 {Managers.Data.SlotNum} 데이터를 불러옵니다.");
        
        fadePanel.gameObject.SetActive(true); 
        StartCoroutine(FadeEffect.Fade(fadePanel, 0f, 1f, fadeTime: 0.5f, action: () => StartCoroutine(LoadGameData())));

    }

    public void OnClickSettings()
    {
        Debug.Log("설정");
        Managers.Scene.LoadScene("Scenes/UI/SettingsHome");
    }

    public void OnClickExit()
    {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    public void OnClickChapters()
    {
        Debug.Log("챕터맵으로 이동");
        Managers.Scene.LoadScene("Chapters");
    }

    public void OnClickPause()
    {
        Debug.Log("게임 멈춤");
        Managers.Scene.LoadScene("Pause");
    }



    public void OnClick2ExitScene()
    {
        Time.timeScale = 1;
        Debug.Log("exit scene으로");
        Managers.Scene.LoadScene("Exit");
    }

    public void OnClickSaveAndExitToMain()
    {
        Time.timeScale = 1;
        Debug.Log("세이브 후 메인 화면으로 이동");
        //여기에 파일 세이브 코드 넣기
        Managers.Scene.LoadScene("Main");
    }

    public void OnClickExit2Windows()
    {
        Time.timeScale = 1;
        Debug.Log("세이브 후 윈도우 화면으로 이동 및 게임 종료");

        // 여기에다 게임 세이브 코드 작성

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }




    private IEnumerator LoadGameData()
    {
        yield return null; // 다음 프레임까지 기다려서 페이드 시작을 보장
    
        Managers.Player.Init();
        Managers.Inventory.Init();
        Managers.Game.LoadStatisticData();

        Debug.Log($"currentStage : {Managers.Player.currentStage}");
        string nextScene = Util.GetSceneNameByStageName(Managers.Player.currentStage);
    
        Debug.Log($"이동할 씬 : {nextScene}");
        Managers.Scene.LoadScene(nextScene);
    }
}
