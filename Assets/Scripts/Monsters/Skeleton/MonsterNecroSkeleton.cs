using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterNecroSkeleton : Monster
{
    public override void Init()
    {
        base.Init();
    }

    // 피 자동 회복
    /*public bool isRecovering = false;

    public override void OnAttacked(float damage)
    {
        base.OnAttacked(damage);

        if (!isRecovering)
        {
            StartCoroutine(RecoverHealth());
        }
    }

    private IEnumerator RecoverHealth()
    {
        isRecovering = true;
        yield return new WaitForSeconds(3f); // 3초 후 회복 시작

        while (stat.CurrentHp < stat.monsterData.MaxHp) // 최대 체력 도달 시 종료
        {
            stat.CurrentHp += 0.7f;
            yield return new WaitForSeconds(1f); // 초당 0.7 회복
        }

        Debug.Log("위자드 자동 회복 후 CurrentHp : " + stat.CurrentHp);
        isRecovering = false;
    }*/


    // 마법진 소환 후 미니 스켈레톤 소환
    // 마법진 소환 후 미니 스켈레톤 소환
    public GameObject magicCircle;
    public GameObject miniPrefab;

    private GameObject miniInstance = null;

    Vector3 targetScale2 = new Vector3(5f, 5f, 5f);

    // 소환 위치 범위 (이 값을 수정해서 소환 위치를 조정할 수 있음)
    public float spawnRangeX = 10f;  // X축 소환 범위
    public float spawnHeight = 0f;   // Y축 높이 고정 (예: 0으로 고정)

    public override int Detect(Vector3 direction)
    {
        int detectResult = base.Detect(direction); // 부모 클래스의 Detect 실행

        if (detectResult != -1 && miniInstance == null) // 감지가 성공하면 실행
        {
            // magicCircle.SetActive(true);
            StartCoroutine(AnimateSequence());
        }

        return detectResult;
    }

    private IEnumerator AnimateSequence()
    {
        // 1. magicCircle 크기 키우기
        yield return StartCoroutine(ScaleObject(magicCircle, Vector3.zero, targetScale2, 2f));

        // 2. miniPrefab이 이미 생성된 경우 새로 만들지 않음
        if (miniInstance == null && target != null)
        {
            // 3마리 미니 스켈레톤 소환
            for (int i = 0; i < 3; i++)
            {
                // X축 범위 내에서 랜덤 위치를 계산
                Vector3 randomPosition = transform.position + new Vector3(
                    Random.Range(-spawnRangeX, spawnRangeX), // X축 범위 10 내에서 랜덤
                    spawnHeight,                             // Y축은 고정
                    0);                                      // Z축 고정 (2D이므로 0으로 설정)

                miniInstance = Instantiate(miniPrefab, randomPosition, Quaternion.identity);  // 부모 없이 생성

                // 소환 후 잠시 대기
                yield return new WaitForSeconds(0f); // 0초 대기 후 다음 소환
            }
        }
    }

    // 크기 변화 처리
    private IEnumerator ScaleObject(GameObject obj, Vector3 startScale, Vector3 endScale, float duration)
    {
        float time = 0;
        obj.transform.localScale = startScale;

        while (time < duration)
        {
            obj.transform.localScale = Vector3.Lerp(startScale, endScale, time / duration);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        obj.transform.localScale = endScale; // 마지막 보정
    }

    // 랜덤 위치로 미니 오브젝트를 이동시키는 코루틴
    private IEnumerator MoveObject(GameObject obj, Vector3 startPos, Vector3 endPos, float duration)
    {
        float time = 0;

        while (time < duration)
        {
            obj.transform.position = Vector3.Lerp(startPos, endPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }

        obj.transform.position = endPos; // 마지막 보정
    }


}
