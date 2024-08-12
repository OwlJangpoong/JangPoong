using UnityEngine;

public class MPpotionLarge : ItemBase
{
    public override void UpdateCollision(Transform target)
    {
        var playerDataManager = target.GetComponent<PlayerDataManager>();

        if (playerDataManager != null)
        {
            // �ڷ�ƾ�� PlayerDataManager���� ����
            playerDataManager.StartCoroutine(AddManaAfterDelay(playerDataManager));

            Destroy(gameObject);
        }
    }

    private System.Collections.IEnumerator AddManaAfterDelay(PlayerDataManager playerDataManager)
    {
        // ���� ���ô� �ð�
        yield return new WaitForSeconds(2f);

        // Mana ����
        playerDataManager.GetComponent<PlayerDataManager>().Mana += 50;
    }
}