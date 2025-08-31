using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class EndingVideoController : MonoBehaviour
{
    [Header("Video")]
    public VideoPlayer videoPlayer;   // VideoPlayerHost 의 VideoPlayer
    public RawImage    videoScreen;   // VideoLayer/VideoScreen (RawImage)
    public GameObject  videoLayer;    // VideoLayer 루트 오브젝트

    [Header("Fade")]
    public Image fadeOverlay;         // 검정 이미지(알파 1로 시작 권장)
    public float fadeTime = 0.4f;

    [Header("After Video (선택)")]
    public TimelineSceneTransition transition; // 있으면 통계/씬전환 실행
    public bool showStatsOnEnd = true;         // 통계 UI 켜기
    public bool loadNextSceneOnEnd = false;    // 다음 씬 로드

    bool playing;

    // == 네가 한 줄로 호출할 공개 메서드 ==
    public void StartEndingVideo()
    {
        if (playing) return;
        StartCoroutine(CoPlay());
    }

    IEnumerator CoPlay()
    {
        playing = true;

        if (videoLayer) videoLayer.SetActive(true);

        // 검정 → 화면
        yield return Fade(1f, 0f, fadeTime);

        if (videoPlayer)
        {
            videoPlayer.loopPointReached -= OnVideoEnd;
            videoPlayer.loopPointReached += OnVideoEnd;
            videoPlayer.Play();
        }
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        StartCoroutine(CoFinish());
    }

    IEnumerator CoFinish()
    {
        // 화면 → 검정
        yield return Fade(0f, 1f, fadeTime);

        if (videoPlayer) videoPlayer.loopPointReached -= OnVideoEnd;
        if (videoLayer)  videoLayer.SetActive(false);

        // 영상 끝 후 행동
        if (transition)
        {
            if (showStatsOnEnd)     transition.EnableGameObject();
            if (loadNextSceneOnEnd) transition.LoadNextScene();
        }

        // 검정 → 화면(통계 UI 보여줄 거면)
        yield return Fade(1f, 0f, fadeTime);

        playing = false;
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