using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_TextColorWhenHover : MonoBehaviour
{
    private TMP_Text tmpText;
    public Color wantedColor;
    public Color originalColor;

    private void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
    }

    public void changeWhenHover()
    {
        tmpText.color = wantedColor;
    }

    public void changeWhenLeaves()
    {
        tmpText.color = originalColor;
    }

    private void OnEnable()
    {
        changeWhenLeaves();
    }
}