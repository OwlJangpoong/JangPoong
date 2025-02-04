using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_ItemUsedText : MonoBehaviour
{
    [SerializeField] private TMP_Text onItemUsedText;
    [SerializeField] private InventoryUI InventoryUI;

    private void Start()
    {
        onItemUsedText = GetComponent<TMP_Text>();
        
        GameObject ui_root = GameObject.FindWithTag("UI_Root");
        if (ui_root == null)
        {
            Debug.LogError("Can't find gameobject which has 'UI_Root' tag");
            return;
        }
        else
        {
            InventoryUI = Util.FindChild<InventoryUI>(ui_root, null, true);
        }


        if (InventoryUI != null)
        {
            InventoryUI.OnHpPotionUsed -= HpPotionUsedText;
            InventoryUI.OnHpPotionUsed += HpPotionUsedText;

            InventoryUI.OnManaPotionUsed -= ManaPotionUsedText;
            InventoryUI.OnManaPotionUsed += ManaPotionUsedText;

            InventoryUI.OnInvisiblePositionUsed -= InvisiblePotionUsedText;
            InventoryUI.OnInvisiblePositionUsed += InvisiblePotionUsedText;
        }
       
        
        
        // 모든 LevelUpToken 객체를 찾아 이벤트 구독 -> 코드 수정(250203)
        // LevelUpToken[] levelUpTokens = FindObjectsOfType<LevelUpToken>();
        // foreach (var token in levelUpTokens)
        // {
        //     token.OnLevelUpTokenUpdated -= LevelUpTokenUsedText;
        //     token.OnLevelUpTokenUpdated += LevelUpTokenUsedText;
        // }
        Managers.Player.OnTokenCntChanged -= LevelUpTokenUsedText;
        Managers.Player.OnTokenCntChanged += LevelUpTokenUsedText;

        onItemUsedText.text = string.Empty;
    }

    //오브젝트 파괴시 don't destroy로 살아있는 오브젝트의 이벤트를 구독 중이라면 해제해준다.
    //그렇지 않는 경우 오브젝트가 파괴되어도 don't destroy로 살이있는 오브젝트의 이벤트의 리스너 목록에 파괴된 오브젝트의 구독이 남아있게된다. 이벤트 발생시 파괴된 오브젝트를 참조하려하기 때문에 null reference error가 발생한다.
    private void OnDestroy()
    {
        if (Managers.Player != null)
        {
            Managers.Player.OnTokenCntChanged -= LevelUpTokenUsedText;
        }
    }

    public void HpPotionUsedText(float increase)
    {
        ResetTextAlpha();
        SetTextColor("#992D2D"); // HP 색상 (빨간색)
        onItemUsedText.gameObject.SetActive(true);
        onItemUsedText.text = "+ " + increase + " HP";
        StartCoroutine(HidePotionUsedText());
    }

    public void ManaPotionUsedText(int increase)
    {
        ResetTextAlpha();
        SetTextColor("#2D5299"); // MP 색상 (파란색)
        onItemUsedText.gameObject.SetActive(true);
        onItemUsedText.text = "+ " + increase + " MP";
        StartCoroutine(HidePotionUsedText());
    }

    public void InvisiblePotionUsedText(float durationTime)
    {
        ResetTextAlpha();
        SetTextColor("A1A1A1");
        onItemUsedText.gameObject.SetActive(true);
        onItemUsedText.text = "투명화 " + durationTime + "초";
        StartCoroutine(HidePotionUsedText());
    }

    public void LevelUpTokenUsedText(int tokenCnt)
    {
        ResetTextAlpha();
        SetTextColor("#358067");
        onItemUsedText.gameObject.SetActive(true);
        onItemUsedText.text = "장풍 공격력 증가";
        StartCoroutine(HidePotionUsedText());
    }

    private void ResetTextAlpha()
    {
        Color originalColor = onItemUsedText.color;
        onItemUsedText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1);
    }

    private void SetTextColor(string hexColor)
    {
        // 텍스트 색상 설정 (HEX 값으로 변경)
        if (ColorUtility.TryParseHtmlString(hexColor, out Color newColor))
        {
            onItemUsedText.color = newColor;
        }
    }

    private System.Collections.IEnumerator HidePotionUsedText()
    {
        yield return new WaitForSeconds(1f);

        float fadeDuration = 0.3f;
        float startAlpha = onItemUsedText.color.a;
        float elapsed = 0f;

        Color originalColor = onItemUsedText.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, 0, elapsed / fadeDuration);
            onItemUsedText.color = new Color(originalColor.r, originalColor.g, originalColor.b, newAlpha);
            yield return null;
        }

        onItemUsedText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);
        onItemUsedText.gameObject.SetActive(false);
    }
}

