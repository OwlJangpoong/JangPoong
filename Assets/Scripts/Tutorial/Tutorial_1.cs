using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Tutorial_1 : MonoBehaviour
{
    // 아무 키나 마우스 클릭 입력 되면 3초 동안 보여주고 페이드 아웃으로 사라지게
    // 15초 동안 아무 키나 마우스 클릭이 없다면 자동으로 다음 안내 메시지 출력되게끔

    public GameObject[] objects;                // 순서대로 등장할 오브젝트들
    public float fadeDuration = 0.5f;           // 페이드 인/아웃 시간
    public float visibleDuration = 3f;          // 안내 메시지(오브젝트)가 보여지는 시간
    public float autoSkipDuration = 15f;    // 입력이 없을 때 자동으로 넘어가는 시간
    private int currentIndex = 0;               // 현재 활성화된 오브젝트 인덱스

    private void Start()
    {
        // 모든 오브젝트를 비활성화
        foreach (GameObject obj in objects)
        {
            obj.SetActive(false);
        }

        // 오브젝트가 존재하면 시퀀스 실행
        if (objects.Length > 0)
        {
            StartCoroutine(HandleObjectSequence());
        }
    }

    private IEnumerator HandleObjectSequence()
    {
        while (currentIndex < objects.Length)
        {
            GameObject currentObject = objects[currentIndex];

            Image image = currentObject.GetComponent<Image>();
            TextMeshProUGUI text = currentObject.GetComponentInChildren<TextMeshProUGUI>();

            // 현재 오브젝트 활성화
            currentObject.SetActive(true);

            // 마지막 오브젝트가 아니라면 페이드 인/아웃
            if (currentIndex < objects.Length - 1)
            {
                // 페이드 인
                yield return StartCoroutine(FadeIn(image, text));

                // 입력 대기 또는 자동 넘어가기
                float elapsedTime = 0f;

                while (elapsedTime < autoSkipDuration)
                {
                    // 입력 감지
                    if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
                    {
                        break;
                    }

                    // 경과 시간 증가
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                // 오브젝트가 화면에 보이는 시간 보장
                yield return new WaitForSeconds(visibleDuration);

                // 페이드 아웃
                yield return StartCoroutine(FadeOut(image, text));

            }
            else // 마지막 오브젝트는 페이드 인만 적용. (계속 화면에 보이도록 함)
            {
                yield return StartCoroutine(FadeIn(image, text));
            }

            // 현재 오브젝트 비활성화 (마지막 오브젝트 제외)
            if (currentIndex < objects.Length - 1)
            {
                currentObject.SetActive(false);
            }

            // 다음 오브젝트로
            currentIndex++;
        }
    }

    #region 페이드 인
    private IEnumerator FadeIn(Image image, TextMeshProUGUI text)
    {
        float timer = 0f;

        Color imageColor = image.color;
        Color textColor = text.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);

            imageColor.a = alpha;
            textColor.a = alpha;

            image.color = imageColor;
            text.color = textColor;

            yield return null;
        }

        // 최종 Alpha 값 설정
        imageColor.a = 1f;
        textColor.a = 1f;

        image.color = imageColor;
        text.color = textColor;
    }
    #endregion

    #region 페이드 아웃
    private IEnumerator FadeOut(Image image, TextMeshProUGUI text)
    {
        float timer = 0f;

        Color imageColor = image.color;
        Color textColor = text.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;

            float alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);

            imageColor.a = alpha;
            textColor.a = alpha;

            image.color = imageColor;
            text.color = textColor;

            yield return null;
        }

        // 최종 Alpha 값 설정
        imageColor.a = 0f;
        textColor.a = 0f;

        image.color = imageColor;
        text.color = textColor;
    }
    #endregion
}
