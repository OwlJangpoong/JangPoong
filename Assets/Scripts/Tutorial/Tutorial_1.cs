using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class Tutorial_1 : MonoBehaviour
{
    // �ƹ� Ű�� ���콺 Ŭ�� �Է� �Ǹ� 3�� ���� �����ְ� ���̵� �ƿ����� �������
    // 15�� ���� �ƹ� Ű�� ���콺 Ŭ���� ���ٸ� �ڵ����� ���� �ȳ� �޽��� ��µǰԲ�

    public GameObject[] objects;                // ������� ������ ������Ʈ��
    public float fadeDuration = 0.5f;           // ���̵� ��/�ƿ� �ð�
    public float visibleDuration = 3f;          // �ȳ� �޽���(������Ʈ)�� �������� �ð�
    public float autoSkipDuration = 15f;    // �Է��� ���� �� �ڵ����� �Ѿ�� �ð�
    private int currentIndex = 0;               // ���� Ȱ��ȭ�� ������Ʈ �ε���

    private void Start()
    {
        // ��� ������Ʈ�� ��Ȱ��ȭ
        foreach (GameObject obj in objects)
        {
            obj.SetActive(false);
        }

        // ������Ʈ�� �����ϸ� ������ ����
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

            // ���� ������Ʈ Ȱ��ȭ
            currentObject.SetActive(true);

            // ������ ������Ʈ�� �ƴ϶�� ���̵� ��/�ƿ�
            if (currentIndex < objects.Length - 1)
            {
                // ���̵� ��
                yield return StartCoroutine(FadeIn(image, text));

                // �Է� ��� �Ǵ� �ڵ� �Ѿ��
                float elapsedTime = 0f;

                while (elapsedTime < autoSkipDuration)
                {
                    // �Է� ����
                    if (Input.anyKeyDown || Input.GetMouseButtonDown(0))
                    {
                        break;
                    }

                    // ��� �ð� ����
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }

                // ������Ʈ�� ȭ�鿡 ���̴� �ð� ����
                yield return new WaitForSeconds(visibleDuration);

                // ���̵� �ƿ�
                yield return StartCoroutine(FadeOut(image, text));

            }
            else // ������ ������Ʈ�� ���̵� �θ� ����. (��� ȭ�鿡 ���̵��� ��)
            {
                yield return StartCoroutine(FadeIn(image, text));
            }

            // ���� ������Ʈ ��Ȱ��ȭ (������ ������Ʈ ����)
            if (currentIndex < objects.Length - 1)
            {
                currentObject.SetActive(false);
            }

            // ���� ������Ʈ��
            currentIndex++;
        }
    }

    #region ���̵� ��
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

        // ���� Alpha �� ����
        imageColor.a = 1f;
        textColor.a = 1f;

        image.color = imageColor;
        text.color = textColor;
    }
    #endregion

    #region ���̵� �ƿ�
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

        // ���� Alpha �� ����
        imageColor.a = 0f;
        textColor.a = 0f;

        image.color = imageColor;
        text.color = textColor;
    }
    #endregion
}
