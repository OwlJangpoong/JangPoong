using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PlayerManager
{
    //플레이어 관련 리소스
    public List<GameObject> jangPoongPrefab_list =
        new List<GameObject>(Enumerable.Repeat<GameObject>(null, (int)Define.JangPoongLevel.Length + 1));  //레벨에 따른 장풍 프리랩을 담는 배열 (0번 인덱스는 비워두고 1부터 채우기 위해 length+1 길이로 선언)
    public GameObject ultPrefab;
    
    #region Player Data Variables (Global)
    
    //"Hp & Mana"
    //초기화는 로컬 데이터 로드 or init해서 게임 시작할 때 한다.
    public float Hp { get; private set; } = 10.0f;
    public float MaxHp { get; private set; } = 10.0f;
    public int Mana { get; private set; } = 100;
    public int MaxMana { get; private set; } = 100;
    
    
    //MosterPoint
    public int MonsterPoint { get; private set; }
    public int MaxMonsterPoint { get; private set; }
    
    //JangPoong
    public int CurrentJangPoongLevel { get; private set; } = 1;
    
    //LevelUpToken
    //먹은 레벨업 토큰 수
    public int TokenCnt { get; set; }


    //달리기 토글 여부
    public bool IsRunning { get; set; } = false;
    
    
    //이벤트
    public event Action<float> OnHpChanged;
    public event Action<int> OnManaChanged; //OnManaChanged
    public Action OnDie; //OnDie
    
    public event Action<int> OnMonsterPointChanged;
    /// <summary>
    /// 플레이어가 현재 획득한 총 레벨업 토큰 수를 인자로 넘겨준다.
    /// </summary>
    public event Action<int> OnTokenCntChanged;
    /// <summary>
    /// 현재 장풍 레벨을 인자로 넘겨줌
    /// </summary>
    public event Action<int> OnJangPongLevelChanged;
    
    #endregion


    #region Set Player Stats (Set~ 메소드 모음)

    public void SetHp(float value)
    {
        float hp = Mathf.Clamp(value, 0, MaxHp);
        hp =  Mathf.Round(hp * 100) / 100f;
        Hp = hp;
        OnHpChanged?.Invoke(Hp);
        
        if (Hp <= 0) 
            OnDie?.Invoke(); //사망
        
    }

    public void SetMaxHp(float value)
    {
        MaxHp = Mathf.Max(MaxHp, value);
        OnHpChanged?.Invoke(Hp);
    }

    public void SetMana(int value)
    {
        Mana = Mathf.Clamp(value, 0, MaxMana);
        OnManaChanged?.Invoke(Mana);

    }

    public void SetMaxMana(int value)
    {
        MaxMana = Mathf.Max(MaxMana, value);
        OnManaChanged?.Invoke(Mana);
    }

    public void SetMonsterPoint(int value)
    {
        MonsterPoint = Mathf.Clamp(value, 0, MaxMonsterPoint);
        OnMonsterPointChanged?.Invoke(MonsterPoint);
    }

    public void SetMaxMonsterPoint(int value)
    {
        MaxMonsterPoint = Mathf.Max(MaxMonsterPoint, value);
    }

    public void SetTokenCnt(int value)
    {
        TokenCnt = Mathf.Clamp(value, 0, 9999);
        
        OnTokenCntChanged?.Invoke(TokenCnt);
    }
    
    
    /// <summary>
    /// Token 획득으로 장풍 레벨을 업데이트 하는 경우 이 메소드를 직접적으로 호출하면 안됨! 토큰 수 증가하면 자동으로 레벨 반영되도록 구현되어 있음.
    /// 토큰과 관계없이 게임 시나리오 상 장풍 레벨을 강제적으로 변경해야 하는 경우, 혹은 아이템 효과 등으로 장풍 레벨이 일시적으로 줄어드는 경우 사용하는 메소드
    /// </summary>
    /// <param name="level">설정하고자 하는 장풍 레벨</param>
    public void SetJangPoongLevel(int level)
    {
        CurrentJangPoongLevel = Mathf.Clamp(level, 1, 10);
        
        OnJangPongLevelChanged?.Invoke(CurrentJangPoongLevel);
    }

    #endregion
    

    // 장풍 레벨 설정

    // 데이터 초기화 (필요 시 사용)
    // public void ResetData()
    // {
    //     Hp = MaxHp;
    //     Mana = MaxMana;
    //     MonsterPoint = 0;
    // }



  
    #region Event Function

    //초기화 함수
    public void Init()
    {
        Debug.Log("PlayerManager 초기화");
        //플레이어 데이터 변수 초기화=셋팅 (전역 관리)
        LoadPlayerData();
        
        //리스트 초기화 : 장풍 프리팹 로드해서 리스트에 넣어주기
        LoadJangPoongPrefabs();
        LoadUltPrefab();
        
        
        //이벤트 구독
        OnTokenCntChanged += UpdateJangPoongLevel;

    }

    #endregion

    #region JangPoong & LevelUpToken

    private void LoadJangPoongPrefabs()
    {
        for (int i = 1; i <= (int)Define.JangPoongLevel.Length; i++)
        {
            //리소스 로드 후 리스트에 추가한다.
            
            //1. 리소스 로드
            GameObject jp = Managers.Resource.Load<GameObject>("Prefabs/jangpoong/jangpoong Lv" + i);

            //2. 로드 실패시 오류 메시지 & 함수 종료
            if (jp == null)
            {
                Debug.LogError($"장풍 프리팹 jangpoong Lv{i} 로드에 실패했습니다.");
                return;
            }
            
            //3. List에 추가
            jangPoongPrefab_list[i] = jp;
        }
        
        Debug.Log("장풍 프리팹 리스트 초기화를 완료하였습니다.");
    }


    private void LoadUltPrefab()
    {
        GameObject ult = Managers.Resource.Load<GameObject>("Prefabs/jangpoong/Ult");
        if (ult == null)
        {
            Debug.LogWarning("궁극기 프리팹 로드에 실패했습니다.");
            return;
        }

        ultPrefab = ult;
    }
    
    //레벨업 토큰 획득시 호출되는 이벤트 리스너
    //OnTokenCntChanged 구독
    public void UpdateJangPoongLevel(int tokenCnt)
    {
        //0개면 레벨1이고 배열 인덱스 1번
        //1개면 레벨2이고 배열 인덱스 2번....
        
        //9개면 레벨 10이고 배열 인덱스 10번...
        //그 이상은 레벨 10으로 유지
        
        CurrentJangPoongLevel = Mathf.Clamp(tokenCnt + 1, 1, 10);
        OnJangPongLevelChanged?.Invoke(CurrentJangPoongLevel);
    }
    #endregion

    #region Player Data (Global)

    private void LoadPlayerData()
    {
        //로컬에 저장된 or 초기화된 데이터를 읽어와서 플레이어 데이터 변수 셋팅하기
    }

    

    #endregion
    
}