using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGameController : MonoBehaviour
{
    [SerializeField] private NewPlayerMovement playerMovement;
    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private StageData currentStageData;
    // [SerializeField] private (int chapter, int stage) currentLevel = (1, 1);
    // [SerializeField] private int tempChapter;
    // [SerializeField] private int tempStage;



    private void Awake()
    {
        playerMovement = GameObject.FindWithTag("Player").GetComponent<NewPlayerMovement>();
        //����� ���� �ҷ����� ������ ó�� �߰� ����
        //�ӽ÷� ���� ���� �����մϴ�.
        // currentLevel.chapter = tempChapter;
        // currentLevel.stage = tempStage;

        // currentStageData =
        //     Managers.Resource.Load<StageData>($"Data/Stage/Stage{currentLevel.chapter}-{currentLevel.stage}");
        // playerMovement.SetUp(currentStageData);
        cameraFollow.SetUp(currentStageData);
    }
}
