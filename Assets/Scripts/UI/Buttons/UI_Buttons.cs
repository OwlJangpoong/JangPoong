using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UI_Buttons: MonoBehaviour
{
    public void OnClickNewGame()
    {
        Debug.Log("새 게임");
        Managers.RestPlayData();
        
        Managers.Data.Init();
        Managers.Data.DeleteAllFilesInSlot();
        Managers.Player.Init();

    }

    public void OnClickLoad()
    {
        Debug.Log("이어하기");
        //이어하기 씬 이동 & 데이터 저장 필요
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

    public void OnClickNormal()
    {
        Debug.Log("보통 난이도 게임");

    }

    public void OnClickHard()
    {
        Debug.Log("어려움 난이도 게임");
    }

    public void OnClickImpossible()
    {
        Debug.Log("불가능 레벨");
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
}
