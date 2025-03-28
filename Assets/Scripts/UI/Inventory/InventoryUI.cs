using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.SceneManagement;

public class InventoryUI : MonoBehaviour
{
    //배열로 자료구조 변경
    [SerializeField, Tooltip("hpSmall, hpLarge, mpSmall, mpLarge, invisibility 순으로 아이템 개수를 표시하는 TMP_Text UI 오브젝트를 넣어주세요.")] public TMP_Text[] UI_itemTexts;
    
    private PlayerStatsController playerStatsController;

    public delegate void Item_Hp_EventHandler(float increase);
    public event Item_Hp_EventHandler OnHpPotionUsed;

    public delegate void Item_Mana_EventHandler(int increase);
    public event Item_Mana_EventHandler OnManaPotionUsed;

    public delegate void Item_Invisible_EventHandler(float durationTime);

    public event Item_Invisible_EventHandler OnInvisiblePositionUsed; 

    private void Start()
    {
        StartCoroutine(WaitForManagersInitialization());
    }
    
    private IEnumerator WaitForManagersInitialization()
    {
        // Managers 초기화 대기
        while (!Managers.IsInitialized)
        {
            yield return null; // 다음 프레임까지 대기
        }
        
        //초기화 완료 후 변수 할당
        playerStatsController = GameObject.FindWithTag("Player").GetComponent<PlayerStatsController>();

        // 초기화 완료 후 이벤트 연결 및 UI 업데이트
        Managers.Inventory.OnInventoryUpdated -= UpdateItemCntTextUI;
        Managers.Inventory.OnInventoryUpdated += UpdateItemCntTextUI;
        UpdateItemCntTextUI(); // 초기 아이템 개수 표시
    }

    public void Update()
    {
        // // hpSmall ���
        // if (Input.GetKeyDown(KeyCode.Alpha1) && Managers.Inventory.GetItemCount(Define.Item.hpPotionSmall) > 0)
        // {
        //     Managers.Inventory.InventoryItem(Define.Item.hpPotionSmall,-1);
        //     StartCoroutine(AddHpAfterDelay(2f, 2f));
        //     Debug.Log("hpSmall used");
        // }
        //
        // // hpLarge ���
        // if (Input.GetKeyDown(KeyCode.Alpha2) && Managers.Inventory.GetItemCount(Define.Item.hpPotionLarge) > 0)
        // {
        //     Managers.Inventory.InventoryItem(Define.Item.hpPotionLarge,-1);
        //     StartCoroutine(AddHpAfterDelay(3f, 4f));
        //     Debug.Log("hpLarge used");
        // }
        //
        // // mpSmall ���
        // if (Input.GetKeyDown(KeyCode.Alpha3) && Managers.Inventory.GetItemCount(Define.Item.mpPotionSmall) > 0)
        // {
        //     Managers.Inventory.InventoryItem(Define.Item.mpPotionSmall,-1);
        //     StartCoroutine(AddManaAfterDelay(1.5f, 25));
        //     Debug.Log("mpSmall used");
        // }
        //
        // // mpLarge ���
        // if (Input.GetKeyDown(KeyCode.Alpha4) && Managers.Inventory.GetItemCount(Define.Item.mpPotionLarge) > 0)
        // {
        //     Managers.Inventory.InventoryItem(Define.Item.mpPotionLarge,-1);
        //     StartCoroutine(AddManaAfterDelay(2f,50));
        //     Debug.Log("mpLarge used");
        // }
        //
        // // invisibility ���
        // if (Input.GetKeyDown(KeyCode.Alpha5) && Managers.Inventory.GetItemCount(Define.Item.invisibilityPotion) > 0)
        // {
        //     Managers.Inventory.InventoryItem(Define.Item.invisibilityPotion,-1);
        //     StartCoroutine(StartInvisibilityAfterDelay(PlayerDataManager));
        //     Debug.Log("invinsibility used");
        // }
        //
        // // �κ��丮 ������ ���� ���� ���� ������Ʈ
        
        CheckItemUse(KeyCode.Alpha1, Define.Item.hpPotionSmall, 0f, 2f, PotionEffectAfterDelay);
        CheckItemUse(KeyCode.Alpha2, Define.Item.hpPotionLarge, 0f, 4f, PotionEffectAfterDelay);
        CheckItemUse(KeyCode.Alpha3, Define.Item.mpPotionSmall, 0f, 25f, PotionEffectAfterDelay);
        CheckItemUse(KeyCode.Alpha4, Define.Item.mpPotionLarge, 0f, 50f, PotionEffectAfterDelay);
        CheckItemUse(KeyCode.Alpha5, Define.Item.invisibilityPotion, 0f, 0f,PotionEffectAfterDelay);
    }   

    public void UpdateItemCntTextUI()
    {
        int[] itemCnts = Managers.Inventory.GetAllItemCount();
        if (UI_itemTexts.Length == 0||UI_itemTexts==null)
        {
            Debug.LogError("아이템 개수를 표시하는 UI 텍스트 배열이 비어있거나 초기화되지 않았습니다.");
            return;
        }
        for (int i = 0; i < itemCnts.Length && i < UI_itemTexts.Length; i++)
        {
            UI_itemTexts[i].text = "x" + itemCnts[i];
        }
    }

    //코드 리팩토링 : 아이템 사용 체크 관련 코드 간소화 및 중복 코드 정리를 위한 메소드 생성
    private void CheckItemUse(KeyCode key, Define.Item itemType, float sec, float increase,
        System.Func<Define.Item, float, float, IEnumerator> effectCoroutine)
    {   
        
        //hp, mana full인 상태에서 아이템 사용 방지 처리
        if (Input.GetKeyDown(key) && Managers.Inventory.GetItemCount(itemType) > 0)
        {
            if (!CanUse(itemType))
            {
                Debug.Log("이미 최대치이거나 포션 사용 중 입니다!");
                return;
            }
            
            Managers.Inventory.InventoryItem(itemType,-1);
            StartCoroutine(effectCoroutine(itemType, sec, increase));
            Debug.Log($"{itemType} used");
            UpdateItemCntTextUI();
        }

        // 아이템 개수가 0일 때 투명도 50% 처리
        // 자식 오브젝트 찾기
        GameObject item = transform.Find(itemType.ToString()).gameObject;

        Image itemImg = item.GetComponent<Image>();

        // 아이템 개수 체크
        int itemCount = Managers.Inventory.GetItemCount(itemType);

        // 투명도 설정
        Color color = itemImg.color;

        // 투명화 포션 처리
        if (itemType.ToString() == "invisibilityPotion")
        {
            // 씬 인덱스를 기준으로 2-1 이후의 씬을 확인
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            // 2-1 이후 씬에서는 다른 아이템들과 동일한 로직을 적용
            if (currentSceneIndex >= 14)
            {
                item.SetActive(true);
                color.a = itemCount == 0 ? 0.5f : 1f; // 2-1 이후부터는 다른 아이템들과 동일한 로직 적용
            }
            else
            {
                item.SetActive(false); // 씬 2-1 전까지는 투명도 0%
            }
        }
        else
        {
            // 다른 아이템들의 경우
            color.a = itemCount == 0 ? 0.5f : 1f; // 개수에 따라 투명도 설정
        }

    itemImg.color = color;
    }

    private bool CanUse(Define.Item itemType)
    {
        switch (itemType)
        {
            case Define.Item.hpPotionLarge: case Define.Item.hpPotionSmall:
                float epsilon = 0.00001f;
                return ((Managers.Player.MaxHp - Managers.Player.Hp) > epsilon);
            
            case Define.Item.mpPotionLarge: case Define.Item.mpPotionSmall:
                return (Managers.Player.MaxMana != Managers.Player.Mana);
            
            case Define.Item.invisibilityPotion:
                return !playerStatsController.IsInvincible;
            
            default: return true;
        }
    }
    
    //코드 리팩토링 : 포션 효과 관련 코드 간소화 및 중복 코드 정리를 위한 메소드 생성
    private IEnumerator PotionEffectAfterDelay(Define.Item itemType, float sec, float increase)
    {
        yield return new WaitForSeconds(sec);
        
        //hp : 0,1
        //mana : 2, 3
        //invisibility : 4
        
        //플레이어 데이터 저장 구축 후 아래 코드 다시 리팩토링 예정(250129)
        //코드 수정 완료(250203)
        switch ((int)itemType)
        {
            case 0: case 1:
                Managers.Player.SetHp(Managers.Player.Hp+increase);
                // Managers.Player.Hp += increase;
                OnHpPotionUsed?.Invoke(increase);
                break;
            case 2: case 3:
                Managers.Player.SetMana(Managers.Player.Mana+(int)increase);
                // Managers.Player.Mana += (int)increase;
                OnManaPotionUsed?.Invoke((int)increase);
                break;
            case 4:
                // playerStatsController.isInvisible = true; //����ȭ ���� ����
                // playerStatsController.IsInvincible = true;//���� ���� ����
                // yield return Managers.Player.StartCoroutine(InvisibilityCoroutine(Managers.Player));
                
                OnInvisiblePositionUsed?.Invoke(playerStatsController.invisibleDuration);
                
                yield return StartCoroutine(playerStatsController.InvisibilityCoroutine());
                break;
        }
       
        
    }

    // public System.Collections.IEnumerator AddHpAfterDelay(float sec, float increase)
    // {
    //     // ���� ���ô� �ð�
    //     yield return new WaitForSeconds(sec);
    //
    //     // HP ����
    //     Managers.PlayerData.Hp += increase;
    //
    //     OnHpPotionUsed?.Invoke(increase);
    // }
    //
    // private System.Collections.IEnumerator AddManaAfterDelay( float sec, int increase)
    // {
    //     // ���� ���ô� �ð�
    //     yield return new WaitForSeconds(sec);
    //
    //     // Mana ����
    //     Managers.PlayerData.Mana += increase;
    //
    //     OnManaPotionUsed?.Invoke(increase);
    // }
    //
    // private System.Collections.IEnumerator StartInvisibilityAfterDelay(PlayerDataManager playerDataManager)
    // {
    //     // ���� ���ô� �ð�
    //     //yield return new WaitForSeconds(2f); -> ����ȭ ������ ���ô� �ð� ������ ����
    //
    //     // InvisibilityCoroutine ����
    //     PlayerDataManager.isInvisible = true; //����ȭ ���� ����
    //     PlayerDataManager.IsInvincible = true;//���� ���� ����
    //     yield return playerDataManager.StartCoroutine(InvisibilityCoroutine(playerDataManager));
    // }

   //InvisibilityCoroutine Function (투명화 효과 코루틴) 코드 이동 -> PlayerStatsController로 이동
   
    
}
