using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineSceneTransition : MonoBehaviour
{
    public string nextSceneName;  // 이동할 씬 이름
    public GameObject[] ui_objects;
    public void LoadNextScene()
    {
        Managers.Scene.LoadScene(nextSceneName);
    }

    public void EnableGameObject()
    {
        foreach (var ui in ui_objects)
        {
           ui.SetActive(true);
        }
        
    }
}