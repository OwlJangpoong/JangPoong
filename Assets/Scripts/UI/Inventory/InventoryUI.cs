using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] public TMP_Text hpPotion_Small;
    [SerializeField] public TMP_Text hpPotion_Large;
    [SerializeField] public TMP_Text mpPotion_Small;
    [SerializeField] public TMP_Text mpPotion_Large;
    [SerializeField] public TMP_Text invisibilityPotion;

    public PlayerDataManager PlayerDataManager;

    public void Update()
    {
        // hpSmall 사용
        if (Input.GetKeyDown(KeyCode.Alpha1) && Managers.Inventory.hpSmallCnt > 0)
        {
            Managers.Inventory.hpSmallCnt--;
            StartCoroutine(AddHpAfterDelay(PlayerDataManager, 2f, 2f));
            Debug.Log("hpSmall used");
        }

        // hpLarge 사용
        if (Input.GetKeyDown(KeyCode.Alpha2) && Managers.Inventory.hpLargeCnt > 0)
        {
            Managers.Inventory.hpLargeCnt--;
            StartCoroutine(AddHpAfterDelay(PlayerDataManager, 3f, 4f));
            Debug.Log("hpLarge used");
        }

        // mpSmall 사용
        if (Input.GetKeyDown(KeyCode.Alpha3) && Managers.Inventory.mpSmallCnt > 0)
        {
            Managers.Inventory.mpSmallCnt--;
            StartCoroutine(AddManaAfterDelay(PlayerDataManager, 1.5f, 25));
            Debug.Log("mpSmall used");
        }

        // mpLarge 사용
        if (Input.GetKeyDown(KeyCode.Alpha4) && Managers.Inventory.mpLargeCnt > 0)
        {
            Managers.Inventory.mpLargeCnt--;
            StartCoroutine(AddManaAfterDelay(PlayerDataManager, 2f,50));
            Debug.Log("mpLarge used");
        }

        // invisibility 사용
        if (Input.GetKeyDown(KeyCode.Alpha5) && Managers.Inventory.invinsibilityCnt > 0)
        {
            Managers.Inventory.invinsibilityCnt--;
            StartCoroutine(StartInvisibilityAfterDelay(PlayerDataManager));
            Debug.Log("invinsibility used");
        }

        // 인벤토리 아이템 현재 보유 개수 업데이트
        UpdateItemCntTextUI();
    }

    public void UpdateItemCntTextUI()
    {
        hpPotion_Small.text = "x" + Managers.Inventory.hpSmallCnt;
        hpPotion_Large.text = "x" + Managers.Inventory.hpLargeCnt;
        mpPotion_Small.text = "x" + Managers.Inventory.mpSmallCnt;
        mpPotion_Large.text = "x" + Managers.Inventory.mpLargeCnt;
        invisibilityPotion.text = "x" + Managers.Inventory.invinsibilityCnt;
    }

    public System.Collections.IEnumerator AddHpAfterDelay(PlayerDataManager playerDataManager, float sec, float increase)
    {
        // 포션 마시는 시간
        yield return new WaitForSeconds(sec);

        // HP 증가
        playerDataManager.GetComponent<PlayerDataManager>().Hp += increase;
    }

    private System.Collections.IEnumerator AddManaAfterDelay(PlayerDataManager playerDataManager, float sec, int increase)
    {
        // 포션 마시는 시간
        yield return new WaitForSeconds(sec);

        // Mana 증가
        playerDataManager.GetComponent<PlayerDataManager>().Mana += increase;
    }

    private System.Collections.IEnumerator StartInvisibilityAfterDelay(PlayerDataManager playerDataManager)
    {
        // 포션 마시는 시간
        //yield return new WaitForSeconds(2f); -> 투명화 포션은 마시는 시간 딜레이 없음

        // InvisibilityCoroutine 실행
        PlayerDataManager.isInvisible = true; //투명화 여부 변경
        PlayerDataManager.IsInvincible = true;//무적 여부 변경
        yield return playerDataManager.StartCoroutine(InvisibilityCoroutine(playerDataManager));
    }

    private System.Collections.IEnumerator InvisibilityCoroutine(PlayerDataManager playerDataManager)
    {
        Renderer renderer = playerDataManager.GetComponentInChildren<Renderer>();

        // 현재 색상 저장
        Color originalColor = renderer.material.color;

        // 투명도 설정 (흐리게 보이기)
        Color invisibleColor = originalColor;
        invisibleColor.a = 0.5f;
        renderer.material.color = invisibleColor;

        //투명화 중에 매초 HP 회복
        float duration = 15f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // playerDataManager.Hp = Mathf.Clamp(playerDataManager.Hp + 0.4f, 0, playerDataManager.maxHp); // HP 회복
            elapsed += 1f; // 1초 대기
            yield return new WaitForSeconds(1f);
        }

        // 원래 색상으로 복원
        renderer.material.color = originalColor;
        //투명화 여부 변경
        PlayerDataManager.isInvisible = false; 
        // 투명화 중 무적 효과 적용 해제
        PlayerDataManager.IsInvincible = false;

        yield break;
    }
}
