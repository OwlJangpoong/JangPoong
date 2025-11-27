using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerForBattle : MonoBehaviour
{
    private Collider2D _collder2d;
    public PlayerStatsController player;
    public GameObject Walls;
    // Start is called before the first frame update
    void Start()
    {
        _collder2d = GetComponent<Collider2D>();
        player.IsInvisible = true;
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            StartBattle();
        }
    }

    private void StartBattle()
    {
        Walls.gameObject.SetActive(true);
        player.IsInvisible = false;
        gameObject.SetActive(false);
    }

    public void EndBattle()
    {
        Managers.Sound.Play("06 Victory Fanfare");
        Walls.gameObject.SetActive(false);
    }
    

    
}
