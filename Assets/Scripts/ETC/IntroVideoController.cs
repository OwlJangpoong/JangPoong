using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class IntroVideoController : MonoBehaviour
{
    [Header("References")]
    public VideoPlayer videoPlayer;      // VideoPlayerHost 의 VideoPlayer
    public RawImage videoScreen;         // VideoLayer/VideoScreen
    public GameObject videoLayer;        // VideoLayer 패널
    public Button skipButton;            // SkipButton
    public Image fadeOverlay;            // FadeOverlay (검정)

    [Header("Fade")]
    public float fadeTime = 0.4f;

    [Header("Dialogue")]
    public GameObject dialogueUI;        // 이후 보여줄 대화 패널 루트
    public DialogueControllerLite dialogueController; // 너가 쓰는 간단 컨트롤러
    public DialogueText dialogueAfterVideo;           // 영상 후 대사 데이터

    void Start()
    {
        // 시작: 검정 → 페이드아웃 → 영상 재생
        dialogueUI.SetActive(false);
        videoLayer.SetActive(true);
        skipButton.onClick.AddListener(Skip);

        StartCoroutine(BeginSequence());
    }

    IEnumerator BeginSequence()
    {
        yield return Fade(1f, 0f, fadeTime);
        videoPlayer.Play();
        videoPlayer.loopPointReached += OnVideoEnd; // 끝났을 때 콜백
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        StartCoroutine(EndSequence());
    }

    void Skip()
    {
        if (videoPlayer.isPlaying) videoPlayer.Stop();
        StartCoroutine(EndSequence());
    }

    IEnumerator EndSequence()
    {
        // 영상 → 검정 페이드
        yield return Fade(0f, 1f, fadeTime);

        // 비디오 UI 숨김
        videoPlayer.loopPointReached -= OnVideoEnd;
        videoLayer.SetActive(false);

        // 대화 UI 보이기
        dialogueUI.SetActive(true);

        // 검정 → 게임 화면
        yield return Fade(1f, 0f, fadeTime);

        // 대사 시작
        if (dialogueController && dialogueAfterVideo)
            dialogueController.StartDialogue(dialogueAfterVideo);
    }

    IEnumerator Fade(float from, float to, float time)
    {
        Color c = fadeOverlay.color;
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