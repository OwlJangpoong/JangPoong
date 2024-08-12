using UnityEngine;

public class HPpotionSmall : ItemBase
{
    public override void UpdateCollision(Transform target)
    {
        var playerDataManager = target.GetComponent<PlayerDataManager>();

        if (playerDataManager != null)
        {
            // �ڷ�ƾ�� PlayerDataManager���� ����
            playerDataManager.StartCoroutine(AddHpAfterDelay(playerDataManager));

            Destroy(gameObject);
        }
    }

    private System.Collections.IEnumerator AddHpAfterDelay(PlayerDataManager playerDataManager)
    {
        // ���� ���ô� �ð�
        yield return new WaitForSeconds(2f);

        // HP ����
        playerDataManager.GetComponent<PlayerDataManager>().Hp += 2;
    }
}