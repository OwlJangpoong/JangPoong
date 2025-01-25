using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_StageName : MonoBehaviour
{
    public TMP_Text text; // ���̵� ��/�ƿ��� �ؽ�Ʈ
    public float fadeDuration = 0.3f; // ���̵� ���� �ð�
    public float delay = 3f; // �������� �̸� �������� �ð�

    void Start()
    {
        text = GetComponentInChildren<TMP_Text>();
        StartCoroutine(FadeInAndOut());
    }

    private IEnumerator FadeInAndOut()
    {
        yield return StartCoroutine(FadeIn());
        yield return new WaitForSeconds(delay);
        yield return StartCoroutine(FadeOut());
    }

    private IEnumerator FadeIn()
    {
        Color color = text.color;
        color.a = 0;
        text.color = color;

        float timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(0, 1, timer / fadeDuration);
            text.color = color;
            yield return null;
        }
        color.a = 1;
        text.color = color;
    }

    private IEnumerator FadeOut()
    {
        Color color = text.color;
        color.a = 1;
        text.color = color;

        float timer = 0;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            color.a = Mathf.Lerp(1, 0, timer / fadeDuration);
            text.color = color;
            yield return null;
        }
        color.a = 0;
        text.color = color;
    }
}
