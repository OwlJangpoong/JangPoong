using UnityEngine;

public class BlueLizardAnimRelay : MonoBehaviour
{
    private MonsterBlueLizardMan blue;

    void Awake()
    {
        blue = GetComponentInParent<MonsterBlueLizardMan>();
    }

    // 애니메이션 이벤트에서 이 함수들을 지정
    public void OnNorHit() { blue?.OnNorHit(); }
    public void OnCriHit() { blue?.OnCriHit(); }
}