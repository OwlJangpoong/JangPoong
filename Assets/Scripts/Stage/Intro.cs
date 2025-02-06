using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
public class Intro : MonoBehaviour
{   
    [SerializeField] private DialogueText dialogueText;
    [SerializeField] private DialogueController dialogueController;
    [SerializeField] private GameObject dialogueBox;
    [SerializeField] private Speaker speaker;
    [SerializeField] private Image img;
    [SerializeField] private Image img2;
    [SerializeField] private Image blackImg;
    [SerializeField] private Sprite[] imgArray;
    [SerializeField] private GameObject clickBlocker;

    private RectTransform imgRect;
    
    private int turn = -1;
    private bool isClick = false;

    void Awake()
    {
        imgRect = img.GetComponent<RectTransform>();
    }

    void Start()
    {
        // PlayerPrefs.SetString("PlayerName", "전설");
        speaker.speakerName = Managers.Player.PlayerName;
        Debug.Log("주인공의 이름 : " + speaker.speakerName);

        //씬 진입씨 첫 턴은 자동 진행
        turn++;
        Debug.Log("현재 턴 : " + turn);
        ImageChecker();
        turnChecker();
        Talk(dialogueText);

        //이미지들 활성화
        img.gameObject.SetActive(true);
        img2.gameObject.SetActive(true);

        //페이드인아웃용 이미지 활성화
        blackImg.gameObject.SetActive(true);

        //클릭방지 오브젝트 비활성화
        clickBlocker.SetActive(false);
    }

    void Update()
    {
        if(dialogueController.isTyping){
            return;
        }

        if(clickBlocker.activeSelf){
            return;
        }

        if (turn != -1 && Input.GetMouseButtonDown(0)) {
            turn++;
            Debug.Log("현재 턴 :" + turn);
            ImageChecker();
            turnChecker();
            Talk(dialogueText);
        }
    }

    private void turnChecker(){
        if(string.IsNullOrWhiteSpace(dialogueText.paragraphs[turn])){
            dialogueBox.SetActive(false);
        }
        else{
            dialogueBox.SetActive(true);
        }
    }
    public void Talk(DialogueText dialogueText)
    {
        dialogueController.DisplayNextParagraph(dialogueText);
        if (turn == 0){
            dialogueBox.SetActive(false);
        }
    }

    private void ImageChecker(){
        switch(turn){
            case 0:
                img.sprite = imgArray[0]; // 창문
                imgRect.anchoredPosition = new Vector2(-650, imgRect.anchoredPosition.y);
                // X 좌표를 -650에서 0으로 Ease Out 방식으로 이동
                imgRect.DOAnchorPosX(0, 1f).SetEase(Ease.OutCubic);
                break;
            case 1:
                img2.gameObject.SetActive(true);
                imgRect = img2.GetComponent<RectTransform>();
                imgRect.anchoredPosition = new Vector2(1400, imgRect.anchoredPosition.y);
                // X 좌표를 1400에서 0으로 Ease Out 방식으로 이동
                imgRect.DOAnchorPosX(0, 1f).SetEase(Ease.OutCubic);
                break;
            case 6:
                clickBlocker.SetActive(true); //애니메이션 진행 중 클릭 방지
                img2.gameObject.SetActive(false);
                imgRect = img.GetComponent<RectTransform>();
                imgRect.anchoredPosition = new Vector2(-745, imgRect.anchoredPosition.y);
                imgRect.DOAnchorPosX(0, 2f).SetEase(Ease.OutCubic);

                Sequence imageSequence1 = DOTween.Sequence();
                imageSequence1.AppendCallback(() => img.sprite = imgArray[1]); // 눈1
                imageSequence1.AppendInterval(0.5f);
                imageSequence1.AppendCallback(() => img.sprite = imgArray[2]); // 눈2
                imageSequence1.AppendInterval(0.5f);
                imageSequence1.AppendCallback(() => img.sprite = imgArray[3]); // 눈3
                imageSequence1.AppendInterval(0.5f);
                imageSequence1.AppendCallback(() => img.sprite = imgArray[4]); // 눈 + 책장
                imageSequence1.AppendInterval(0.5f);

                // 애니메이션 종료 후 클릭을 다시 허용
                imageSequence1.OnKill(() => {
                    clickBlocker.SetActive(false);
                });
                break;
            case 8:
                img.sprite = imgArray[5]; //책 들고 있음
                break;
            case 11:
                img.sprite = imgArray[6]; // 침대 위에서 책
                Sequence imageSequence2 = DOTween.Sequence();

                imgRect = img.GetComponent<RectTransform>();
                imgRect.anchoredPosition = new Vector2(-650, imgRect.anchoredPosition.y);
                imageSequence2.Append(imgRect.DOAnchorPosX(0, 1f).SetEase(Ease.OutCubic));
                break;
            case 12:
                img2.sprite = imgArray[7]; // 책 펼친 내용
                img2.gameObject.SetActive(true);
                imgRect = img2.GetComponent<RectTransform>();
                imgRect.anchoredPosition = new Vector2(1110, imgRect.anchoredPosition.y);
                // X 좌표를 1400에서 0으로 Ease Out 방식으로 이동
                imgRect.DOAnchorPosX(0, 1f).SetEase(Ease.OutCubic);
                imgRect = img.GetComponent<RectTransform>();
                break;
            case 13:
                img2.gameObject.SetActive(false);
                img.sprite = imgArray[8];
                break;
            case 14:
                clickBlocker.SetActive(true);

                Sequence imageSequence3 = DOTween.Sequence();
                imageSequence3.Append(DOTween.To(() => blackImg.color, x => blackImg.color = x, new Color(0, 0, 0, 1), 1f)); // 페이드인

                // 이미지 변경 애니메이션
                imageSequence3.AppendCallback(() => img.sprite = imgArray[9]); // 책 펼친 내용
                imageSequence3.Append(DOTween.To(() => blackImg.color, x => blackImg.color = x, new Color(0, 0, 0, 0), 1f)); // 페이드아웃
                imageSequence3.AppendInterval(1f); // 1초 대기
                imageSequence3.Append(DOTween.To(() => blackImg.color, x => blackImg.color = x, new Color(0, 0, 0, 1), 1f)); // 페이드인
                imageSequence3.AppendCallback(() => img.sprite = imgArray[10]); // 책 넘김
                imageSequence3.Append(DOTween.To(() => blackImg.color, x => blackImg.color = x, new Color(0, 0, 0, 0), 1f)); // 페이드아웃
                imageSequence3.AppendInterval(1f); // 1초 대기
                imageSequence3.Append(DOTween.To(() => blackImg.color, x => blackImg.color = x, new Color(0, 0, 0, 1), 1f)); // 페이드인
                imageSequence3.AppendCallback(() => img.sprite = imgArray[11]);
                imageSequence3.AppendInterval(1f); // 1초 대기
                imageSequence3.Append(DOTween.To(() => blackImg.color, x => blackImg.color = x, new Color(0, 0, 0, 0), 1f)); // 페이드아웃
                // 애니메이션 종료 후 클릭 차단 해제
                imageSequence3.OnKill(() => {
                    clickBlocker.SetActive(false);
                });
                break;
            case 15:
                speaker.speakerName = "???";
                break;
            case 16:
                img.sprite = imgArray[12]; //주인공 & 차르덴 투샷
                speaker.speakerName = PlayerPrefs.GetString("PlayerName");
                break;
            case 25:
                speaker.speakerName = "차르덴";
                break;
            case 26:
                speaker.speakerName = PlayerPrefs.GetString("PlayerName");
                break;
            case 27:
                img.sprite = imgArray[13]; //차르덴 단독샷
                speaker.speakerName = "차르덴";
                break;
            case 30:
                img.sprite = imgArray[12]; //주인공 & 차르덴 투샷
                speaker.speakerName = PlayerPrefs.GetString("PlayerName");
                break;
            case 34:
                speaker.speakerName = "차르덴";
                break;
            case 37:
                speaker.speakerName = PlayerPrefs.GetString("PlayerName");
                break;
            case 38:
                speaker.speakerName = "차르덴";
                break;
            case 42:
                img.sprite = imgArray[13];  //차르덴 단독샷
                break;
            case 43:
                img.sprite = imgArray[12]; //주인공 & 차르덴 투샷
                speaker.speakerName = PlayerPrefs.GetString("PlayerName");
                break;
            case 45:
                speaker.speakerName = "차르덴";
                break;
            case 47:
                speaker.speakerName = PlayerPrefs.GetString("PlayerName");
                break;
            case 48:
                img.sprite = imgArray[14]; //국왕 사진
                speaker.speakerName = "차르덴";
                break;
            case 49:
                speaker.speakerName = PlayerPrefs.GetString("PlayerName");
                break;
            case 51:
                img.sprite = imgArray[12];
                speaker.speakerName = PlayerPrefs.GetString("PlayerName");
                break;
            case 52:
                speaker.speakerName = "차르덴";
                break;
            case 54:
                img.sprite = imgArray[15]; //붉은 포션
                speaker.speakerName = "차르덴";
                break;
            case 55:
                img.sprite = imgArray[16]; //보라 포션
                break;
            case 56:
                img.sprite = imgArray[17]; //연두 포션
                break;
            case 57:
                img.sprite = imgArray[18]; //노란 포션
                break;
            case 58:
                img.sprite = imgArray[19]; //지도
                break;
            case 59:
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

    // 대화 종료 시
    private void HandleConversationEnd()
    {
        // 각 물약 1개씩 지급
        // Managers.Inventory.hpSmallCnt += 1;
        // Managers.Inventory.hpLargeCnt += 1;
        // Managers.Inventory.mpSmallCnt += 1;
        // Managers.Inventory.mpLargeCnt += 1;
        
        //인벤토리에 아이템 추가
        for (int i = 0; i < (int)Define.Item.levelUpToken; i++)
        {
            Managers.Inventory.InventoryItem((Define.Item)i,1);
        }
        //씬 이동 전에 인벤토리 데이터 저장되도록 처리 : GameManager로 커밋한다.
        Managers.Inventory.CommitInventoryState();

        // Scene 전환
        SceneManager.LoadScene("0-1 tutorial");
    }
}
