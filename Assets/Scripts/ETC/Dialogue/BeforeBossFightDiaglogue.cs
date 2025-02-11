using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BeforeBossFightDiaglogue :  NPC, ITalkable
{
    [SerializeField] private DialogueText dialogueText;
    [SerializeField] private DialogueController_Version2 dialogueController;
    private Collider2D _collider2d;
    private NewPlayerMovement playerMovement; // 플레이어 움직임 제어용 변수
    private void Awake()
    {
        _collider2d = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerMovement = other.GetComponent<NewPlayerMovement>();
            if (playerMovement != null)
            {
                playerMovement.isInputBlocked = true;
            }
            
            Interact();
            //모든 키 입력 방지를 하고 싶음... 마우스 클릭으로만 대사 넘어가게
        }
        
        
    }

    protected override void Update()
    {
        
    }
    
    
    
    private void OnEnable()
    {
        // DialogueController�� OnConversationEnd �̺�Ʈ ����
        dialogueController.OnConversationEnd += HandleConversationEnd;
    }

    private void OnDisable()
    {
        // ���� ����
        dialogueController.OnConversationEnd -= HandleConversationEnd;
    }

    public override void Interact()
    {
        dialogueController.StartDialogue(dialogueText);
    }

    public void Talk(DialogueText dialogueText)
    {
        // dialogueController.DisplayNextParagraph(dialogueText);
    }
    
    private void HandleConversationEnd()
    {
        UI_FadeController fadeController =
            GameObject.FindWithTag("UI_Root").GetComponentInChildren<UI_FadeController>();
        
        // 콜백으로 씬 이동을 등록
        fadeController.RegisterCallback(() => Managers.Scene.LoadScene("Boss fight"));
        
        fadeController.FadeOut();
        
        if (playerMovement != null)
        {
            playerMovement.isInputBlocked = false;
        }
    }
    
}
