using System.Collections;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System;
using static System.Net.Mime.MediaTypeNames;
using UnityEngine.SceneManagement;

public class Tutorial_2 : MonoBehaviour
{

    public GameObject[] objects;                // ������� ������ ������Ʈ��
    public float fadeDuration = 0.5f;           // ���̵� ��/�ƿ� �ð�
    private int currentIndex = 0;               // ���� Ȱ��ȭ�� ������Ʈ �ε���

    public Monster monster;
    [SerializeField] private AudioClip audioClip;
    [SerializeField] private UI_FadeController fadeController;
    public string NextSceneName;

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

        // ������ ���� �̺�Ʈ ����
        monster.stat.DieAction += OnMonsterDie;
    }
    private IEnumerator HandleObjectSequence()
    {
        while (currentIndex < objects.Length)
        {
            GameObject currentObject = objects[currentIndex];

            UnityEngine.UI.Image image = currentObject.GetComponent<UnityEngine.UI.Image>();
            TextMeshProUGUI text = currentObject.GetComponentInChildren<TextMeshProUGUI>();

            // ���� ������Ʈ Ȱ��ȭ
            currentObject.SetActive(true);

            // ù ��° ������Ʈ�� ���̵� �� ��, ��ǳ �߻� Ű�� ������ �������� ������� ����

            // ù ��° ������Ʈ ���̵� ��
            yield return StartCoroutine(FadeIn(image, text));

            // ��ǳ �߻� Ű �Է��� ��ٸ�
            while (!Input.GetKeyDown(KeyCode.C))
            {
                yield return null;
            }

            // ù ��° ������Ʈ ���̵� �ƿ�
            yield return StartCoroutine(FadeOut(image, text));

            // ù ��° ������Ʈ ��Ȱ��ȭ
            currentObject.SetActive(false);

            // ���� ������Ʈ��
            currentIndex++;

            // �� ��° ������Ʈ�� �����ϸ� �ٷ� ���̵� ��
            if (currentIndex < objects.Length)
            {
                currentObject = objects[currentIndex];
                image = currentObject.GetComponent<UnityEngine.UI.Image>();
                text = currentObject.GetComponentInChildren<TextMeshProUGUI>();

                // �� ��° ������Ʈ ���̵� ��
                yield return StartCoroutine(FadeIn(image, text));
            }

            /*                // �� ��° ������Ʈ ���̵� �ƿ�
                yield return StartCoroutine(FadeOut(image, text));

                // �� ��° ������Ʈ ��Ȱ��ȭ
                currentObject.SetActive(false);*/
        }
    }

    // ���Ͱ� �׾��� �� ȣ��Ǵ� �޼���
    private void OnMonsterDie()
    {
        Debug.Log("���� óġ");

        fadeController = FindObjectOfType<UI_FadeController>();

        // ȿ���� ���
        Managers.Sound.Play(audioClip);

        // ���̵� �ƿ� �� �� �̵�
        fadeController.RegisterCallback(OnFadeOutComplete);
        fadeController.FadeOut();

    }

    private void OnFadeOutComplete()
    {
        StartCoroutine(Managers.Scene.LoadSceneAfterDelay(NextSceneName, 0.0f));
    }

    #region ���̵� ��
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

        // ���� Alpha �� ����
        imageColor.a = 1f;
        textColor.a = 1f;

        image.color = imageColor;
        text.color = textColor;
    }
    #endregion

    #region ���̵� �ƿ�
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

        // ���� Alpha �� ����
        imageColor.a = 0f;
        textColor.a = 0f;

        image.color = imageColor;
        text.color = textColor;
    }
    #endregion
}
