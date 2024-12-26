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
        // hpSmall ���
        if (Input.GetKeyDown(KeyCode.Alpha1) && Managers.Inventory.hpSmallCnt > 0)
        {
            Managers.Inventory.hpSmallCnt--;
            StartCoroutine(AddHpAfterDelay(PlayerDataManager, 2f, 2f));
            Debug.Log("hpSmall used");
        }

        // hpLarge ���
        if (Input.GetKeyDown(KeyCode.Alpha2) && Managers.Inventory.hpLargeCnt > 0)
        {
            Managers.Inventory.hpLargeCnt--;
            StartCoroutine(AddHpAfterDelay(PlayerDataManager, 3f, 4f));
            Debug.Log("hpLarge used");
        }

        // mpSmall ���
        if (Input.GetKeyDown(KeyCode.Alpha3) && Managers.Inventory.mpSmallCnt > 0)
        {
            Managers.Inventory.mpSmallCnt--;
            StartCoroutine(AddManaAfterDelay(PlayerDataManager, 1.5f, 25));
            Debug.Log("mpSmall used");
        }

        // mpLarge ���
        if (Input.GetKeyDown(KeyCode.Alpha4) && Managers.Inventory.mpLargeCnt > 0)
        {
            Managers.Inventory.mpLargeCnt--;
            StartCoroutine(AddManaAfterDelay(PlayerDataManager, 2f,50));
            Debug.Log("mpLarge used");
        }

        // invisibility ���
        if (Input.GetKeyDown(KeyCode.Alpha5) && Managers.Inventory.invinsibilityCnt > 0)
        {
            Managers.Inventory.invinsibilityCnt--;
            StartCoroutine(StartInvisibilityAfterDelay(PlayerDataManager));
            Debug.Log("invinsibility used");
        }

        // �κ��丮 ������ ���� ���� ���� ������Ʈ
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
        // ���� ���ô� �ð�
        yield return new WaitForSeconds(sec);

        // HP ����
        playerDataManager.GetComponent<PlayerDataManager>().Hp += increase;
    }

    private System.Collections.IEnumerator AddManaAfterDelay(PlayerDataManager playerDataManager, float sec, int increase)
    {
        // ���� ���ô� �ð�
        yield return new WaitForSeconds(sec);

        // Mana ����
        playerDataManager.GetComponent<PlayerDataManager>().Mana += increase;
    }

    private System.Collections.IEnumerator StartInvisibilityAfterDelay(PlayerDataManager playerDataManager)
    {
        // ���� ���ô� �ð�
        //yield return new WaitForSeconds(2f); -> ����ȭ ������ ���ô� �ð� ������ ����

        // InvisibilityCoroutine ����
        PlayerDataManager.isInvisible = true; //����ȭ ���� ����
        PlayerDataManager.IsInvincible = true;//���� ���� ����
        yield return playerDataManager.StartCoroutine(InvisibilityCoroutine(playerDataManager));
    }

    private System.Collections.IEnumerator InvisibilityCoroutine(PlayerDataManager playerDataManager)
    {
        Renderer renderer = playerDataManager.GetComponentInChildren<Renderer>();

        // ���� ���� ����
        Color originalColor = renderer.material.color;

        // ������ ���� (�帮�� ���̱�)
        Color invisibleColor = originalColor;
        invisibleColor.a = 0.5f;
        renderer.material.color = invisibleColor;

        //����ȭ �߿� ���� HP ȸ��
        float duration = 15f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            // playerDataManager.Hp = Mathf.Clamp(playerDataManager.Hp + 0.4f, 0, playerDataManager.maxHp); // HP ȸ��
            elapsed += 1f; // 1�� ���
            yield return new WaitForSeconds(1f);
        }

        // ���� �������� ����
        renderer.material.color = originalColor;
        //����ȭ ���� ����
        PlayerDataManager.isInvisible = false; 
        // ����ȭ �� ���� ȿ�� ���� ����
        PlayerDataManager.IsInvincible = false;

        yield break;
    }
}
