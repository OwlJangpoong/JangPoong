using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpToken : ItemBase
{
    public AudioClip audioClip;
    public delegate void LevelUpTokenEventHandler();
    public event LevelUpTokenEventHandler OnLevelUpTokenUpdated;

    public override void UpdateCollision(Transform target)
    {
        var playerDataManager = target.GetComponent<PlayerDataManager>();

        if (playerDataManager != null)
        {
            playerDataManager.LevelUpToken++;

            OnLevelUpTokenUpdated?.Invoke();
            Managers.Sound.Play(audioClip);

            Destroy(gameObject);
        }
    }
}
