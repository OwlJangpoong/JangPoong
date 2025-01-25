using UnityEngine;

public class MPpotionSmall : ItemBase
{
    public AudioClip audioClip;
    public override void UpdateCollision(Transform target)
    {
        Managers.Sound.Play(audioClip);

        var playerDataManager = target.GetComponent<PlayerDataManager>();

        if (playerDataManager != null)
        {
            Destroy(gameObject);
        }
    }

}
