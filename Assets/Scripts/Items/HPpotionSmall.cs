using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class HPpotionSmall : ItemBase
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