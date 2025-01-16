using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;
using static System.Net.Mime.MediaTypeNames;
using UnityEngine.SceneManagement;

public class Tutorial_2 : MonoBehaviour
{

    public GameObject[] objects;                // 순서대로 등장할 오브젝트들
    public float fadeDuration = 0.5f;           // 페이드 인/아웃 시간
    private int currentIndex = 0;               // 현재 활성화된 오브젝트 인덱스

    public Monster monster;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private UI_FadeController fadeController;
    public string NextSceneName;

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

        // 몬스터의 죽음 이벤트 구독
        monster.stat.DieAction += OnMonsterDie;
    }
    private IEnumerator HandleObjectSequence()
    {
        while (currentIndex < objects.Length)
        {
            GameObject currentObject = objects[currentIndex];

            UnityEngine.UI.Image image = currentObject.GetComponent<UnityEngine.UI.Image>();
            TextMeshProUGUI text = currentObject.GetComponentInChildren<TextMeshProUGUI>();

            // 현재 오브젝트 활성화
            currentObject.SetActive(true);

            // 첫 번째 오브젝트가 페이드 인 후, 장풍 발사 키가 눌리기 전까지는 사라지지 않음

            // 첫 번째 오브젝트 페이드 인
            yield return StartCoroutine(FadeIn(image, text));

            // 장풍 발사 키 입력을 기다림
            while (!Input.GetKeyDown(KeyCode.C))
            {
                yield return null;
            }

            // 첫 번째 오브젝트 페이드 아웃
            yield return StartCoroutine(FadeOut(image, text));

            // 첫 번째 오브젝트 비활성화
            currentObject.SetActive(false);

            // 다음 오브젝트로
            currentIndex++;

            // 두 번째 오브젝트가 존재하면 바로 페이드 인
            if (currentIndex < objects.Length)
            {
                currentObject = objects[currentIndex];
                image = currentObject.GetComponent<UnityEngine.UI.Image>();
                text = currentObject.GetComponentInChildren<TextMeshProUGUI>();

                // 두 번째 오브젝트 페이드 인
                yield return StartCoroutine(FadeIn(image, text));
            }

            /*                // 두 번째 오브젝트 페이드 아웃
                yield return StartCoroutine(FadeOut(image, text));

                // 두 번째 오브젝트 비활성화
                currentObject.SetActive(false);*/
        }
    }

    // 몬스터가 죽었을 때 호출되는 메서드
    private void OnMonsterDie()
    {
        Debug.Log("몬스터 처치");

        fadeController = FindObjectOfType<UI_FadeController>();

        // 효과음 재생
        Managers.Sound.Play(audioClip);

        // 페이드 아웃 후 씬 이동
        fadeController.RegisterCallback(OnFadeOutComplete);
        fadeController.FadeOut();

    }

    private void OnFadeOutComplete()
    {
        StartCoroutine(Managers.Scene.LoadSceneAfterDelay(NextSceneName, 0.0f));
    }

    #region 페이드 인
    private IEnumerator FadeIn(UnityEngine.UI.Image image, TextMeshProUGUI text)
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
    private IEnumerator FadeOut(UnityEngine.UI.Image image, TextMeshProUGUI text)
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
