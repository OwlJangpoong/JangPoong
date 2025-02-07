using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelUpToken : ItemBase
{
    public AudioClip audioClip;
    public delegate void LevelUpTokenEventHandler();
    // public event LevelUpTokenEventHandler OnLevelUpTokenUpdated; //Manager.Player의 이벤트로 대체

    public override void UpdateCollision(Transform target)
    {
        // 전달된 target이 플레이어의 하위 오브젝트일 수 있으므로, 최상위 오브젝트를 가져온다
        Transform root = target.root;
        if (root.CompareTag("Player"))
        {
            //토큰 수 증가
            Managers.Player.SetTokenCnt(Managers.Player.TokenCnt+1);
            
            //이벤트 발생 전파
            // OnLevelUpTokenUpdated?.Invoke();
            
            //효과음 재생
            Managers.Sound.Play(audioClip);
            
            //파괴
            Destroy(gameObject);
        }
    }
}
