using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Intro : MonoBehaviour
{   
    [SerializeField] private DialogueText dialogueText;
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private Speaker speaker;
    [SerializeField] private Image img;
    [SerializeField] private Sprite[] imgArray;
    
    private int turn = -1;
    private bool isClick = false;
    
    void Start()
    {
        PlayerPrefs.SetString("PlayerName", "전설");
        speaker.speakerName = PlayerPrefs.GetString("PlayerName");
        Debug.Log(speaker.speakerName);
    }

    void Update()
    {   
        if(!dialogueController.getIsTyping() && Input.GetMouseButtonDown(0) && turn < 58){
            turn++;
            Debug.Log("현재 턴 :" + turn);
            ImageChecker();
            turnChecker();
            Talk(dialogueText);
        }
    }

    private void turnChecker(){
        if(dialogueText.paragraphs[turn] == ""){
            dialogueBox.SetActive(false);
        }
        else{
            dialogueBox.SetActive(true);
        }
    }

    private void ImageChecker(){
        switch(turn){
            case 0:
                img.sprite = imgArray[0]; //침대에 누워있는 주인공
                break;
            case 4:
                img.sprite = imgArray[1]; //책장
                break;
            case 8:
                img.sprite = imgArray[2]; //침대 점프
                break;
            case 9:
                img.sprite = imgArray[3]; //책 펼침
                break;
            case 10:
                img.sprite = imgArray[4]; //1인칭 책펼침
                break;
            case 11:
                img.sprite = imgArray[5]; //책 넘김
                break;
            case 13:
                img.sprite = imgArray[6]; //소환진 
                break;
            case 14:
                speaker.speakerName = "???";
                break;
            case 15:
                img.sprite = imgArray[7]; //마법사 등장
                speaker.speakerName = PlayerPrefs.GetString("PlayerName");
                break;
            case 25:
                speaker.speakerName = "차르덴";
                break;
            case 26:
                speaker.speakerName = PlayerPrefs.GetString("PlayerName");
                break;
            case 27:
                img.sprite = imgArray[8]; //차르덴 단독샷
                speaker.speakerName = "차르덴";
                break;
            case 28:
                img.sprite = imgArray[9]; //국왕 납치
                break;
            case 29:
                img.sprite = imgArray[8]; //차르덴 단독샷
                break;
            case 30:
                img.sprite = imgArray[10]; //안타까워하는 주인공, 걱정하는 차르덴
                speaker.speakerName = PlayerPrefs.GetString("PlayerName");
                break;
            case 31:
                img.sprite = imgArray[11]; //킹받은 표정 변화
                break;
            case 34:
                speaker.speakerName = "차르덴";
                break;
            case 37:
                img.sprite = imgArray[12]; //킹받은 주인공, 당황한 차르덴
                speaker.speakerName = PlayerPrefs.GetString("PlayerName");
                break;
            case 38:
                speaker.speakerName = "차르덴";
                break;
            case 42:
                img.sprite = imgArray[13]; //부탁하는 차르덴
                break;
            case 43:
                img.sprite = imgArray[14]; //고민하는 주인공
                speaker.speakerName = PlayerPrefs.GetString("PlayerName");
                break;
            case 45:
                speaker.speakerName = "차르덴";
                break;
            case 47:
                speaker.speakerName = PlayerPrefs.GetString("PlayerName");
                break;
            case 48:
                img.sprite = imgArray[15]; //국왕 사진
                speaker.speakerName = "차르덴";
                break;
            case 49:
                img.sprite = imgArray[16]; //응큼 표정 주인공
                speaker.speakerName = PlayerPrefs.GetString("PlayerName");
                break;
            case 52:
                img.sprite = imgArray[17]; //물약과 지도
                speaker.speakerName = "차르덴";
                break;
            case 56:
                speaker.speakerName = PlayerPrefs.GetString("PlayerName");
                break;
        }
    }
    private void OnEnable()
    {
        // DialogueController의 OnConversationEnd 이벤트 구독
        dialogueController.OnConversationEnd += HandleConversationEnd;
    }

    private void OnDisable()
    {
        // 구독 해제
        dialogueController.OnConversationEnd -= HandleConversationEnd;
    }

    public void Talk(DialogueText dialogueText)
    {
        dialogueController.DisplayNextParagraph(dialogueText);
    }

    // 대화 종료 시
    private void HandleConversationEnd()
    {
        // 각 물약 1개씩 지급
        Managers.Inventory.hpSmallCnt += 1;
        Managers.Inventory.hpLargeCnt += 1;
        Managers.Inventory.mpSmallCnt += 1;
        Managers.Inventory.mpLargeCnt += 1;

        // Scene 전환
        SceneManager.LoadScene("0-1 tutorial");
    }
}
