using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCutManager : MonoBehaviour
{
    public GameObject[] cutGroups;
    public DialogueText[] dialogueDataPerCut;
    public DialogueControllerLite dialogueController;

    [Header("씬 전환")] 
    public string nextSceneName;

    public UI_FadeController fadeController;
    
    private int currentCutIndex = 0;

    private void Start()
    {
        SwitchCut(0);
    }

    private void SwitchCut(int index)
    {
        for (int i = 0; i < cutGroups.Length; i++)
        {
            cutGroups[i].SetActive(i == index);
        }

        if (index < dialogueDataPerCut.Length)
        {
            dialogueController.StartDialogue(dialogueDataPerCut[index]);
            dialogueController.OnDialogueEnd = MoveToNextCut;
        }
        else
        {
            MoveToNextCut();
        }
    }

    private void MoveToNextCut()
    {
        currentCutIndex++;
        if (currentCutIndex < cutGroups.Length)
        {
            SwitchCut(currentCutIndex);
        }
        else
        {
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                if (fadeController != null)
                {
                    fadeController.RegisterCallback(OnFadeOutComplete);
                    fadeController.FadeOut();
                }
                else
                {
                    StartCoroutine(Managers.Scene.LoadSceneAfterDelay(nextSceneName, 0.1f));
                }
            }
            else
            {
                Debug.Log("모든 컷씬 종료!"); 
                Managers.Scene.LoadScene("NextSceneName");
            }
            
        }
    }

    private void OnFadeOutComplete()
    {
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            StartCoroutine(Managers.Scene.LoadSceneAfterDelay(nextSceneName, 0.1f));
        }
    }
}
