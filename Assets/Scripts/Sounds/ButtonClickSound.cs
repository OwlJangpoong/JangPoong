using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonClickSound : MonoBehaviour
{
    [SerializeField] private AudioClip onClickAudio;
    private Button button;

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.RemoveListener(ButtonOnClickSound);
        button.onClick.AddListener(ButtonOnClickSound);
    }

    public void ButtonOnClickSound()
    {
        Managers.Sound.Play(onClickAudio);
    }
}
