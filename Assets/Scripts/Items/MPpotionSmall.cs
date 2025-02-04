using UnityEngine;

public class MPpotionSmall : ItemBase
{
    public AudioClip audioClip;
    public override void UpdateCollision(Transform target)
    {
        Managers.Sound.Play(audioClip);

        var playerDataManager = target.GetComponent<PlayerStatsController>();

        if (playerDataManager != null)
        {
            Destroy(gameObject);
        }
    }

}
