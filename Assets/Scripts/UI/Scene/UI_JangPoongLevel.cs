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

        StartCoroutine(WaitForManagersInitialization());

        // 모든 LevelUpToken 객체를 찾아 이벤트 구독
        // LevelUpToken[] levelUpTokens = FindObjectsOfType<LevelUpToken>();
        // foreach (var token in levelUpTokens)
        // {
        //     token.OnLevelUpTokenUpdated -= HandleLevelUpTokenUpdated;
        //     token.OnLevelUpTokenUpdated += HandleLevelUpTokenUpdated;
        // }

        jpImg = GetComponentInChildren<Image>(true);
        jplevel = GetComponentInChildren<TMP_Text>(true);
    }
    
    
    //이벤트 핸들러 구독 해제(씬 이동 후 토큰 획득시 이전 씬에서 등록한 이벤트 리스너가 남아있어서 파괴된 jpImg에 접근하려는 오류가 발생함. 따라서 오브젝트 생명주기에서 이벤트 구독 해제 처리한다. 파괴될 때 이벤트 구독을 해제한다.
    
    //오브젝트 파괴시 don't destroy로 살아있는 오브젝트의 이벤트를 구독 중이라면 해제해준다.
    //그렇지 않는 경우 오브젝트가 파괴되어도 don't destroy로 살이있는 오브젝트의 이벤트의 리스너 목록에 파괴된 오브젝트의 구독이 남아있게된다. 이벤트 발생시 파괴된 오브젝트를 참조하려하기 때문에 null reference error가 발생한다.
    private void OnDestroy()
    {
        if (Managers.Player != null)
        {
            Managers.Player.OnJangPongLevelChanged -= HandleLevelUpTokenUpdated;
        }
    }


    private IEnumerator WaitForManagersInitialization()
    {
        while (!Managers.IsInitialized)
            yield return null;
        
        //로컬 저장 구현하면서 ui 업데이트 로직 변경(250202)
        //플레이어 데이터를 전역 관리하는 Managers.Player의 이벤트를 구독한다.
        Managers.Player.OnJangPongLevelChanged -= HandleLevelUpTokenUpdated;
        Managers.Player.OnJangPongLevelChanged += HandleLevelUpTokenUpdated;
        
        //장풍 레벨 ui 초기화
        HandleLevelUpTokenUpdated(Managers.Player.CurrentJangPoongLevel);
        
        
    }

    private void HandleLevelUpTokenUpdated(int jangPoongLevel)
    {
        if(jpImg==null) Debug.LogError("jpImg is null");
        jpImg.sprite = jangPoongImgs[jangPoongLevel-1]; //이미지 배열 인덱스는 0부터 시작하므로 -1 해준다.
        jplevel.text = "Lv." + (jangPoongLevel);
        
        // int level = (int)(Managers.Player.jangPoongLevel);
        
    }
    
    
    
}
