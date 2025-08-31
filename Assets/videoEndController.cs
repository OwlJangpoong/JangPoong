using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class SimpleVideoToNextScene : MonoBehaviour
{
    [Header("Required")]
    public VideoPlayer videoPlayer;     // VideoPlayer 컴포넌트
    public string nextSceneName;        // 다음 씬 이름

    [Header("Optional Fade")]
    public Image fadeOverlay;           // 검정 이미지(없어도 됨)
    public float fadeTime = 0.4f;

    public bool autoPlayOnStart = true; // 씬 시작 시 자동 재생

    void Start()
    {
        if (!videoPlayer)
        {
            Debug.LogError("[SimpleVideoToNextScene] VideoPlayer가 비었습니다.");
            return;
        }

        videoPlayer.loopPointReached += OnVideoEnd;

        if (autoPlayOnStart)
            Play();
    }

    public void Play()
    {
        // 페이드아웃(검정 → 화면) 선택적
        if (fadeOverlay) StartCoroutine(Fade(1f, 0f, fadeTime));
        videoPlayer.Play();
    }

    void OnDestroy()
    {
        if (videoPlayer) videoPlayer.loopPointReached -= OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        StartCoroutine(CoFinish());
    }

    IEnumerator CoFinish()
    {
        // 화면 → 검정
        if (fadeOverlay) yield return Fade(0f, 1f, fadeTime);

        // 프로젝트의 Managers.Scene을 쓰면 여기를 교체해도 됨
        // Managers.Scene.LoadScene(nextSceneName);
        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator Fade(float from, float to, float time)
    {
        if (!fadeOverlay) yield break;
        var c = fadeOverlay.color;
        float t = 0f;
        while (t < time)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(from, to, t / time);
            fadeOverlay.color = new Color(c.r, c.g, c.b, a);
            yield return null;
        }
        fadeOverlay.color = new Color(c.r, c.g, c.b, to);
    }
}