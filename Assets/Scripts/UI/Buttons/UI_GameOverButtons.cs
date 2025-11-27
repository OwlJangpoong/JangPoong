using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_GameOverButtons : MonoBehaviour
{
    public Button restartButton;
    public Button exitButton;
    public NewPlayerMovement movement;

    private void Start()
    {
        exitButton.onClick.AddListener(Exit);
        restartButton.onClick.AddListener(Restart);

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player.GetComponent<NewPlayerMovement>() == null)
        {
            movement = Util.FindChild<NewPlayerMovement>(player);
        }
        else
        {
            movement = player.GetComponent<NewPlayerMovement>();
        }
        
    }

    public void Exit()
    {
        Managers.Scene.LoadScene("Main");
    }

    public void Restart()
    {
        movement.OnButtonClick_Restart();
        Managers.Scene.LoadScene("",true);
    }
}
