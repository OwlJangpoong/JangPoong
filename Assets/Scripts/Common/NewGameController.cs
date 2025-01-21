using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewGameController : MonoBehaviour
{
    [SerializeField] private NewPlayerMovement playerMovement;
    [SerializeField] private CameraFollow cameraFollow;
    [SerializeField] private StageData currentStageData;
    
    

    private void Awake()
    {
        playerMovement = GameObject.FindWithTag("Player").GetComponent<NewPlayerMovement>();
        playerMovement.SetUp(currentStageData);
        cameraFollow.SetUp(currentStageData);
    }
}
