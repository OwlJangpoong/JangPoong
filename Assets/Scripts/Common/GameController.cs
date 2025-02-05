// using System;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class GameController : MonoBehaviour
// {
//     [SerializeField] private PlayerController playerController;
//     [SerializeField] private CameraFollow cameraFollow;
//     [SerializeField] private StageData currentStageData;
//     // [SerializeField] private (int chapter, int stage) currentLevel = (1, 1);
//     // [SerializeField] private int tempChapter;
//     // [SerializeField] private int tempStage;
//     
//     
//     
//     private void Awake()
//     {
//         playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
//         //저장된 레벨 불러오기 데이터 처리 추가 예정
//         //임시로 레벨 수동 설정합니다.
//         // currentLevel.chapter = tempChapter;
//         // currentLevel.stage = tempStage;
//         
//         // currentStageData =
//         //     Managers.Resource.Load<StageData>($"Data/Stage/Stage{currentLevel.chapter}-{currentLevel.stage}");
//         playerController.SetUp(currentStageData);
//         cameraFollow.SetUp(currentStageData);
//     }
// }
