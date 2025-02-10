using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx
{
   public string BeforeScene;
   
   //오타 방지를 위해 열거형으로 SceneName 관리
   public enum SceneNames
   {

   }

   public string GetActiveScene()
   {
      return SceneManager.GetActiveScene().name;
   }
   
   //LoadScene
   /// <summary>
   /// sceneNames로 넘겨받은 씬을 불러온다. 매개변수가 없는 경우 현재 씬을 다시 로드한다.
   /// </summary>
   /// <param name="sceneName"></param>
   public void LoadScene(string sceneName = "" , bool isRestart = false)
   {
      BeforeScene = GetActiveScene();
      
      //PlayerData의 currentStage 업데이트하기
      if (string.IsNullOrEmpty(sceneName)) sceneName = GetActiveScene();

      bool stageScene = Define.ReverseSceneNames.TryGetValue(sceneName, out string nextStage);
      if(stageScene && Managers.Player!=null)
      {
          Debug.Log($"씬 전환: {sceneName} (스테이지 업데이트)");
          //currentStage를 씬 전환 직전에 업데이트
          Managers.Player.currentStage = nextStage;
      }
      else
      {
          Debug.Log("define 목록에 있는 씬 아님!!!");
      }
      
      Managers.Clear();
      SceneManager.LoadScene(sceneName);

      if (!stageScene) return; //스테이지 씬아니면 저장x
      
      if (isRestart)
      {
          RestoreData(); // 다시하기일 경우, 데이터 복구
      }
      else
      {
          CommitData();  // 일반적인 씬 전환일 경우, 데이터 저장
      }
      
      Managers.Game.SaveStatisticData();     
   }

   private void RestoreData()
   {
       Managers.Inventory.RestoreInventoryState();
       Managers.Player.RestorePlayerData();
   }

   private void CommitData()
   {
       Managers.Inventory.CommitInventoryState();
       Managers.Player.CommitPlayerData();
   }
   
   //LoadScene by SceneNames
   /// <summary>
   /// SceneNames 열거형으로 매개변수를 받아온 경우
   /// </summary>
   /// <param name="sceneName"></param>
   public void LoadScene(SceneNames sceneName)
   {
      //Clear 로직 추가 예정
      SceneManager.LoadScene(sceneName.ToString());
   }


   // public void LoadSceneAfterDelay(string SceneName, float delayTime)
   // {
   //    float time = 0f;
   //    while (time<delayTime)
   //    {
   //       time += Time.deltaTime;
   //    }
   //
   //    SceneManager.LoadScene(SceneName);
   // }

   public IEnumerator LoadSceneAfterDelay(string sceneName, float delayTime)
   {
      yield return new WaitForSeconds(delayTime);
      
      this.LoadScene(sceneName);
   }

}
