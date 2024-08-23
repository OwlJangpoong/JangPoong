using UnityEngine;

public class InvisibilityPotion : ItemBase
{
    public override void UpdateCollision(Transform target)
    {
        var playerDataManager = target.GetComponent<PlayerDataManager>();
        if (playerDataManager != null)
        {
            // �ڷ�ƾ�� PlayerDataManager���� ����
            // playerDataManager.StartCoroutine(StartInvisibilityAfterDelay(playerDataManager));

            Destroy(gameObject);
        }
    }

/*    private System.Collections.IEnumerator StartInvisibilityAfterDelay(PlayerDataManager playerDataManager)
    {
        // ���� ���ô� �ð�
        yield return new WaitForSeconds(2f);

        // InvisibilityCoroutine ����
        yield return playerDataManager.StartCoroutine(InvisibilityCoroutine(playerDataManager));
    }

    private System.Collections.IEnumerator InvisibilityCoroutine(PlayerDataManager playerDataManager)
    {
        Renderer renderer = playerDataManager.GetComponentInChildren<Renderer>();

        // ���� ���� ����
        Color originalColor = renderer.material.color;

        // ���� ���� (�帮�� ���̱�)
        Color invisibleColor = originalColor;
        invisibleColor.a = 0.5f;
        renderer.material.color = invisibleColor;

        // ����ȭ �߿� ���� HP ȸ��
        float duration = 15f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            playerDataManager.Hp = Mathf.Clamp(playerDataManager.Hp + 0.4f, 0, playerDataManager.maxHp); // HP ȸ��
            elapsed += 1f; // 1�� ���
            yield return new WaitForSeconds(1f);
        }

        // ���� �������� ����
        renderer.material.color = originalColor;
    }*/
}
