using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterWizardSkeleton : Monster
{
    public override void Init()
    {
        base.Init();
    }

    // 피 자동 회복
    public bool isRecovering = false;

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
    }

    // 얼음 원거리 공격
    public GameObject magicCircle;
    public GameObject icePrefab; // ice 프리팹 추가

    private GameObject iceInstance = null; // ice 생성 여부 확인

    Vector3 targetScale2 = new Vector3(5f, 5f, 5f);

    public override int Detect(Vector3 direction)
    {
        int detectResult = base.Detect(direction); // 부모 클래스의 Detect 실행

        if (detectResult != -1 && iceInstance == null) // 감지가 성공하면 실행
        {
            magicCircle.SetActive(true);
            StartCoroutine(AnimateSequence());
        }

        return detectResult;
    }

    private IEnumerator AnimateSequence()
    {
        // 1. magicCircle 크기 키우기
        yield return StartCoroutine(ScaleObject(magicCircle, Vector3.zero, targetScale2, 2f));

        // 2. ice가 이미 생성된 경우 새로 만들지 않음
        if (iceInstance == null && target != null)
        {
            iceInstance = Instantiate(icePrefab, target.position + Vector3.up * 50, Quaternion.identity, this.transform);

            yield return StartCoroutine(MoveObject(iceInstance, iceInstance.transform.position, target.position, 2f)); // 1초 동안 target으로 이동

            // 낙하 완료 후 제거
            Destroy(iceInstance.gameObject, 0.5f);
            iceInstance = null;
        }
    }

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
