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

    public LevelUpToken levelUpToken;
    public Image jpImg;
    public TMP_Text jplevel;


    private void Start()
    {
        // 모든 LevelUpToken 객체를 찾아 이벤트 구독
        LevelUpToken[] levelUpTokens = FindObjectsOfType<LevelUpToken>();
        foreach (var token in levelUpTokens)
        {
            token.OnLevelUpTokenUpdated -= HandleLevelUpTokenUpdated;
            token.OnLevelUpTokenUpdated += HandleLevelUpTokenUpdated;
        }
    }

    private void HandleLevelUpTokenUpdated()
    {
        int level = (int)(Managers.PlayerData.jangPoongLevel);
        jpImg.sprite = jangPoongImgs[level];
        jplevel.text = "Lv." + (level+1);
    }
}
