using UnityEngine;

public class InvisibilityPotion : ItemBase
{
    public AudioClip audioClip;
    public override void UpdateCollision(Transform target)
    {
        Managers.Sound.Play(audioClip);
        var playerDataManager = target.GetComponent<PlayerStatsController>();
        if (playerDataManager != null)
        {
            // 코루틴을 PlayerDataManager에서 실행
            // playerDataManager.StartCoroutine(StartInvisibilityAfterDelay(playerDataManager));

            Destroy(gameObject);
        }
    }

/*    private System.Collections.IEnumerator StartInvisibilityAfterDelay(PlayerDataManager playerDataManager)
    {
        // 포션 마시는 시간
        yield return new WaitForSeconds(2f);

        // InvisibilityCoroutine 실행
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

        // 투명화 중에 매초 HP 회복
        float duration = 15f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            playerDataManager.Hp = Mathf.Clamp(playerDataManager.Hp + 0.4f, 0, playerDataManager.maxHp); // HP 회복
            elapsed += 1f; // 1초 대기
            yield return new WaitForSeconds(1f);
        }

        // 원래 색상으로 복원
        renderer.material.color = originalColor;
    }*/
}
