using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerForEnding : MonoBehaviour
{
    private Collider2D _collder2d;
    // Start is called before the first frame update
    void Start()
    {
        _collder2d = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            UI_FadeController fadeController =
                GameObject.FindWithTag("UI_Root").GetComponentInChildren<UI_FadeController>();
            // 콜백으로 씬 이동을 등록
            fadeController.RegisterCallback(() => Managers.Scene.LoadScene("2-3-2 inside the castle"));
        
            fadeController.FadeOut();
        }
    }
}
