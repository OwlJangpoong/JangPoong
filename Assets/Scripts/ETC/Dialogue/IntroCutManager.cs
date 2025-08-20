using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCutManager : MonoBehaviour
{
    public GameObject[] cutGroups;
    public DialogueText[] dialogueDataPerCut;
    public DialogueControllerLite dialogueController;

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
            Debug.Log("모든 컷씬 종료!");
        }
    }
}
