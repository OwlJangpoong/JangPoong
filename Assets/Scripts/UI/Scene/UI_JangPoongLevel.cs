using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_JangPoongLevel : MonoBehaviour
{
    [Header("JangPoong")]
    // 장풍 데이터 설정
    [SerializeField]
    public Sprite[] jangPoongImgs;
    
    public Image jpImg;
    public TMP_Text jplevel;


    private void Start()
    {
        
        //로컬 저장 구현하면서 ui 업데이트 로직 변경(250202)
        //플레이어 데이터를 전역 관리하는 Managers.Player의 이벤트를 구독한다.
        Managers.Player.OnJangPongLevelChanged -= HandleLevelUpTokenUpdated;
        Managers.Player.OnJangPongLevelChanged += HandleLevelUpTokenUpdated;
        
        // 모든 LevelUpToken 객체를 찾아 이벤트 구독
        // LevelUpToken[] levelUpTokens = FindObjectsOfType<LevelUpToken>();
        // foreach (var token in levelUpTokens)
        // {
        //     token.OnLevelUpTokenUpdated -= HandleLevelUpTokenUpdated;
        //     token.OnLevelUpTokenUpdated += HandleLevelUpTokenUpdated;
        // }
    }

    private void HandleLevelUpTokenUpdated(int jangPoongLevel)
    {
        jpImg.sprite = jangPoongImgs[jangPoongLevel-1]; //이미지 배열 인덱스는 0부터 시작하므로 -1 해준다.
        jplevel.text = "Lv." + (jangPoongLevel);
        
        // int level = (int)(Managers.Player.jangPoongLevel);
        
    }
}
